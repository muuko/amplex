using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace scms.modules.special
{
  public class specialEventHandler : forms.IFormSubmissionEventHandler
  {
		protected const int nImageQuality = 80;

    public string UniqueName
    {
			get { return "scms.special.specialEventHandler"; }
    }

		public string Name
    {
			get { return "Create Special"; }
    }

    public string SettingsControlPath
    {
			get { return "/scms/modules/special/special/formsettings.ascx"; }
    }

    protected bool GetValueForField(bool bEnabled, int ? nFieldId, System.Collections.Generic.Dictionary<int, scms.data.scms_form_submission_fieldvalue> fieldValuesByFieldId, out string strValue)
    {
      bool bValueSet = false;
      strValue = null;

      if (bEnabled)
      {
        if (nFieldId.HasValue)
        {
          scms.data.scms_form_submission_fieldvalue fieldValue;
          if (fieldValuesByFieldId.TryGetValue(nFieldId.Value, out fieldValue))
          {
            if (!string.IsNullOrEmpty(fieldValue.value))
            {
              strValue = fieldValue.value;
              bValueSet = true;
            }
          }
				}
      }

      return bValueSet;
    }

    public bool FormSubmitted(int nEventHandlerId, int nModuleInstanceId, int nFormId, int nSubmissionId, System.Collections.Generic.Dictionary<int, scms.data.scms_form_submission_fieldvalue> fieldValuesByFieldId)
    {
      bool bSuccess = false;
      
      try
      {
        scms.data.ScmsDataContext dcScms = new scms.data.ScmsDataContext();
        scms.modules.special.data.specialDataContext dcSpecial = new scms.modules.special.data.specialDataContext();


        var feh = (from eh1 in dcScms.scms_form_eventhandlers
                   where eh1.id == nEventHandlerId
                   select eh1).Single();
        var seh = (from eh2 in dcSpecial.scms_special_form_eventhandlers
                   where eh2.eventHandlerId == nEventHandlerId
                   select eh2).Single();


        string strTitle = null;
        scms.data.scms_form_submission_fieldvalue fieldValue = null;
        if (seh.titleFieldId.HasValue)
        {
          if (fieldValuesByFieldId.TryGetValue(seh.titleFieldId.Value, out fieldValue))
          {
						strTitle = fieldValue.value;
          }
        }

        string strImageFilePath = null;
        if (seh.imageFieldId.HasValue)
        {
          if (fieldValuesByFieldId.TryGetValue(seh.imageFieldId.Value, out fieldValue))
          {
            strImageFilePath = fieldValue.value;

            if (seh.imageTranslate.HasValue && seh.imageTranslate.Value)
            {
              int nLastDot = strImageFilePath.LastIndexOf('.');
              string strImageFilePathNoExtension = strImageFilePath.Substring(0, nLastDot);
              string strImageFileExtension = strImageFilePath.Substring(nLastDot);

							int nPathIncrement = 1;
							string strTranslatedImageFilePath = null;
							bool bPathValid = false;
							while (nPathIncrement < 100)
							{
								strTranslatedImageFilePath = string.Format("{0}_{1}_{2}{3}", strImageFilePathNoExtension, nEventHandlerId, nPathIncrement++, strImageFileExtension);
								string strAbsolutePath = System.Web.HttpContext.Current.Server.MapPath(strTranslatedImageFilePath);
								if (!System.IO.File.Exists(strAbsolutePath))
								{
									bPathValid = true;
									break;
								}
							}
							if (!bPathValid)
							{
								throw new Exception(string.Format("unable to create unique file name for '{0}'.", strImageFilePathNoExtension));
							}
							

              if (!string.IsNullOrEmpty(seh.imageTranslationMode))
              {
                scms.ImageUtil imageUtil = new ImageUtil();
                ImageUtil.Mode mode = (ImageUtil.Mode)Enum.Parse(typeof(ImageUtil.Mode), seh.imageTranslationMode);
								if (imageUtil.TransformImage(strImageFilePath, ref strTranslatedImageFilePath, mode, seh.imageTranslationHeight, seh.imageTranslationWidth, nImageQuality))
                {
									strImageFilePath = strTranslatedImageFilePath;
                }
                else
                {
									throw new Exception("failed preparing image for use as special thumbnail");
                }
              }
            } 
          }
        }

        string strDescription = null;
        if (seh.descriptionFieldId.HasValue)
        {
					if (fieldValuesByFieldId.TryGetValue(seh.descriptionFieldId.Value, out fieldValue))
          {
						strDescription = fieldValue.value;
          }
        }

        DateTime ? dtDate = null;
        if (seh.associatedDateFieldId.HasValue)
        {
          if (fieldValuesByFieldId.TryGetValue(seh.associatedDateFieldId.Value, out fieldValue))
          {
            if (!string.IsNullOrEmpty(fieldValue.value))
            {
							dtDate = DateTime.Parse(fieldValue.value);
            }
          }
        }


        string strError = null;;
        Exception exError = null;
        ScmsSiteMapProvider.PageNode pageNodeParent;
        global::scms.ScmsSiteMapProvider siteMapProvider = new global::scms.ScmsSiteMapProvider();
        if (!siteMapProvider.GetPageNode(seh.parentPageId, out pageNodeParent, out strError, out exError))
        {
					throw new Exception(string.Format("Failed getting page node for parent page id '{0}', error '{1}'.", seh.parentPageId, strError), exError);
        }

        global::scms.data.scms_page page = new global::scms.data.scms_page();
        global::scms.data.scms_site site;
        site = (from s in dcScms.scms_sites
                where s.id == pageNodeParent.page.siteid
                select s).Single();

        string strPageName = strTitle;
        page.siteid = pageNodeParent.page.siteid;
        page.parentId = pageNodeParent.page.id;
        page.securityInherit = true;
        string strFragment = null;
        bool bUniqueFragmentFound = false;
        int nTry = 1;
        do
        {
          if (nTry <= 1)
          {
						strFragment = scms.admin.Pages.CreateFragment(strPageName);
          }
          else
          {
						strFragment = scms.admin.Pages.CreateFragment(string.Format("{0}-{1}", strPageName, nTry));
          }
          var existingPage = (from ep in dcScms.scms_pages
                              where ep.parentId == pageNodeParent.page.id
                              where ep.fragment == strFragment
                              where ep.deleted == false
                              select ep).FirstOrDefault();
          if (existingPage == null)
          {
            bUniqueFragmentFound = true;
            if (nTry > 1)
            {
							strPageName = string.Format("{0}-{1}", strPageName, nTry);
            }
          }
          nTry++;
        }
        while (!bUniqueFragmentFound);

        page.fragment = strFragment;
        page.linktext = strPageName;
        page.title = strPageName;
        page.lastUpdated = DateTime.Now;

        string strParentUrl = pageNodeParent.page.url;
        if (!strParentUrl.EndsWith("/"))
        {
            strParentUrl += "/";
        }
        page.url = string.Format("{0}{1}", strParentUrl, strFragment);
        page.summary = strDescription;
        page.title = strTitle;
        page.thumbnail = strImageFilePath;
        page.associatedDate = dtDate;
        page.type = 'P';

        // determine ordinal
        int nOrdinal;
        var minOrdinal = (from pmin in dcScms.scms_pages
													where pmin.parentId == pageNodeParent.page.id
													where pmin.deleted == false
													orderby pmin.ordinal ascending
                          select new { ordinal = pmin.ordinal }).FirstOrDefault();
        if (minOrdinal == null)
        {
					nOrdinal = 0;
        }
        else
        {
					nOrdinal = minOrdinal.ordinal - 1;
        }

				if (nOrdinal < 100)
				{
					// push current pages up 100
					var pages = from p in dcScms.scms_pages
											where p.parentId == pageNodeParent.page.id
											where p.deleted == false
											select p;
					foreach (var p in pages)
					{
						p.ordinal += 100;
					}
					dcScms.SubmitChanges();
					nOrdinal += 100;
				}

        page.ordinal = nOrdinal;
        page.visible = true;
        page.searchInclude = true;
        page.deleted = false;
        page.sitemapInclude = true;
        page.xmlSitemapInclude = true;
        page.xmlSitemapPriority = 0.5M;
        page.xmlSitemapUpdateFrequency = "monthly";


        if (seh.templateId.HasValue)
        {
          page.templateId = seh.templateId;
        }
        else
        {
          page.templateId = site.defaultTemplateId;
        }
        page.masterId = (from t in dcScms.scms_templates
                         where t.id == page.templateId
                         where t.deleted == false
                         select t.masterId).Single();



        dcScms.scms_pages.InsertOnSubmit(page);
        dcScms.SubmitChanges();

				scms.search.search search = new scms.search.search();
				search.RebuildPageIndex(page.id);

				global::scms.CacheManager.Clear();

        bSuccess = true;

      }
      catch (Exception ex)
      {
          string strMessage = string.Format("Failed creating special.");
          ScmsEvent.Raise(strMessage, this, ex);
      }

      return bSuccess;
    }
      
  }
}
