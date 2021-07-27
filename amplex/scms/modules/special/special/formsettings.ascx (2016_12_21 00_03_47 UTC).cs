using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace scms.modules.special.special
{
	public partial class formsettings : System.Web.UI.UserControl, scms.modules.forms.IFormSubmissionEventHandlerSettings
  {
    /*
    public delegate void SettingsSavedDelegate();
    public delegate void SettingsCancelledDelegate();
    public SettingsSavedDelegate OnSettingsSaved = null;
    public SettingsCancelledDelegate OnSettingsCancelled = null;
    */


    public int? SiteId
    {
      get;
      set;
    }

    public int? FormId
    {
      get;
      set;
    }

    public int? EventHandlerId
    {
      get;
      set;
    }

    scms.modules.forms.SettingsSavedDelegate settingsSavedDelegate = null;
    scms.modules.forms.SettingsCancelledDelegate settingsCancelledDelegate = null;

    public void SetDelegates(scms.modules.forms.SettingsSavedDelegate settingsSavedDelegate, scms.modules.forms.SettingsCancelledDelegate settingsCancelledDelegate)
    {
      this.settingsSavedDelegate = settingsSavedDelegate;
      this.settingsCancelledDelegate = settingsCancelledDelegate;
    }

    protected int? SpecialApplicationId
    {
      get { return (int?)ViewState["SpecialApplicationId"]; }
      set { ViewState["SpecialApplicationId"] = value; }
    }

    protected void LoadTemplates(global::scms.data.ScmsDataContext dc, int nSiteId)
    {
			ddlTemplate.Items.Clear();

      ListItem liDefault = new ListItem("[Default]", string.Empty);
      liDefault.Selected = true;
      ddlTemplate.Items.Add(liDefault);

      ddlTemplate.AppendDataBoundItems = true;
      
      ddlTemplate.DataValueField = "id";
      ddlTemplate.DataTextField = "name";
      var templates = from t in dc.scms_templates
                      where t.siteId == nSiteId
                      select new { t.id, t.name };
      ddlTemplate.DataSource = templates;
      ddlTemplate.DataBind();
    }

		protected void LoadSites()
		{
			global::scms.ScmsSiteMapProvider siteMapProvider = new global::scms.ScmsSiteMapProvider();
			global::scms.ScmsSiteMapProvider.WebsitePages websitePages;
			string strError;
			Exception exError;
			if (siteMapProvider.GetWebSitePages(out websitePages, out strError, out exError))
			{
				foreach (global::scms.ScmsSiteMapProvider.Site site in websitePages.Values)
				{
					ListItem li = new ListItem(site.site.name, site.site.id.ToString());
					ddlSite2.Items.Add(li);
				}
			}
		}

		protected void ddlSite_SelectedIndexChanged(object sender, EventArgs args)
		{
			int? nSiteId = null;
			int n;
			if( int.TryParse(ddlSite2.SelectedValue, out n))
			{
				nSiteId = n;
			}

			pageSelector.SiteId = nSiteId;
			pageSelector.PageId = null;
			pageSelector.Inititialize();

			if (nSiteId.HasValue)
			{
				scms.data.ScmsDataContext dc = new scms.data.ScmsDataContext();
				LoadTemplates(dc, nSiteId.Value);
			}
		}


    protected void Page_Init(object sender, EventArgs args)
    {
			LoadSites();
			/*
			ddlSite2.ClearSelection();
			ListItem li = ddlSite2.Items.FindByValue(SiteId.Value.ToString());
			if (li != null)
			{
				li.Selected = true;
			}
			ddlSite_SelectedIndexChanged(null, null);
			 * */

			/*
			// ddlSite.SiteId = SiteId;
      pageSelector.SiteId = ddlSite.SiteId;
      pageSelector.Inititialize();

			
			LoadTemplates(dc, ddlSite.SiteId.Value);
			*/

			scms.data.ScmsDataContext dc = new scms.data.ScmsDataContext();


      int? nSpecialApplicationId = SpecialApplicationId;

      if (!nSpecialApplicationId.HasValue)
      {
        try
        {
          
          nSpecialApplicationId = (from a in dc.scms_plugin_applications
                                      where a.name == "special"
                                      select a.id).Single();

          ViewState["SpecialApplicationId"] = nSpecialApplicationId;

        }
        catch (Exception ex)
        {
          string strMessage = "Exception thrown while looking up special application module";
          ScmsEvent.Raise(strMessage, this, ex);
        }
      }

      // moduleSelector.PluginApplicationId = SpecialApplicationId;
      // moduleSelector.PluginModuleId = SpecialModuleId;
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        // moduleSelector.OnModuleSelectionChanged += ModuleSelectionChanged;
    }

    protected void PopulateFormFields()
    {
      try
      {
        scms.data.ScmsDataContext dc = new scms.data.ScmsDataContext();
        var fields = from f in dc.scms_form_fields
                     where f.formid == FormId.Value
                     where f.deleted == false
                     orderby f.name
                     select f;
        scms.data.scms_form_field[] aFields = fields.ToArray();

        PopulateFormFields(ddlTitle, aFields);
        PopulateFormFields(ddlImage, aFields);
        PopulateFormFields(ddlDescription, aFields);
        PopulateFormFields(ddlAssociatedDate, aFields);
      }
      catch (Exception ex)
      {
        string strMessage = "Failed populating form fields.";
        ScmsEvent.Raise(strMessage, this, ex);
      }
    }


    protected void PopulateFormFields(DropDownList ddlFields, scms.data.scms_form_field[] aFields)
    {
      ddlFields.Items.Clear();
      ddlFields.Items.Add("");
      ddlFields.AppendDataBoundItems = true;
      ddlFields.DataTextField = "name";
      ddlFields.DataValueField = "id";
      ddlFields.DataSource = aFields;
      ddlFields.DataBind();
    }

    public bool LoadSettings()
    {
      bool bSuccess = false;

      try
      {
        PopulateFormFields();

        if (EventHandlerId.HasValue)
        {
          scms.data.ScmsDataContext dcScms = new scms.data.ScmsDataContext();
          scms.data.scms_form_eventhandler eventHandler =
              (from eh in dcScms.scms_form_eventhandlers
               where eh.id == EventHandlerId.Value
               select eh).FirstOrDefault();


          scms.modules.special.data.specialDataContext dcSpecials = new scms.modules.special.data.specialDataContext();
          scms.modules.special.data.scms_special_form_eventhandler specialFormEventHandler =
              (from sfeh in dcSpecials.scms_special_form_eventhandlers
               where sfeh.eventHandlerId == EventHandlerId.Value
               select sfeh).FirstOrDefault();

          if (specialFormEventHandler != null)
          {
              /*
                                      scms.modules.special.data.scms_special_module specialModule = 
                                         (from sm in dcSpecials.scms_special_modules
                                          where sm.id == specialFormEventHandler.specialModuleId
                                          select sm).FirstOrDefault();

                                      moduleSelector.SiteId = SiteId;
                                      moduleSelector.ModuleInstanceId = specialModule.instanceId;
              */


						int? nSelectedSiteId = null;
						if (specialFormEventHandler.siteId.HasValue)
						{
							nSelectedSiteId = specialFormEventHandler.siteId;
						}
						else
						{
							nSelectedSiteId = SiteId;
						}

						ddlSite2.ClearSelection();
						ListItem li = ddlSite2.Items.FindByValue(nSelectedSiteId.Value.ToString());
						li.Selected = true;

						ddlSite_SelectedIndexChanged(null, null);

						
						//LoadTemplates(dcScms, ddlSite.SiteId.Value);
            //pageSelector.SiteId = ddlSite.SiteId;
						//pageSelector.Inititialize();
            pageSelector.PageId = specialFormEventHandler.parentPageId;

            string strSelectedTemplateId = null;
            if (specialFormEventHandler.templateId.HasValue)
            {
                strSelectedTemplateId = specialFormEventHandler.templateId.ToString();
                ListItem liTemplate = ddlTemplate.Items.FindByValue(strSelectedTemplateId);
                if (liTemplate != null)
                {
                    ddlTemplate.ClearSelection();
                    liTemplate.Selected = true;
                }
            }

            ddlTitle.ClearSelection();
            if (specialFormEventHandler.titleFieldId.HasValue)
            {
                ddlTitle.SelectedValue = specialFormEventHandler.titleFieldId.ToString();
            }

            ddlImage.ClearSelection();
            if (specialFormEventHandler.imageFieldId.HasValue)
            {
                ddlImage.SelectedValue = specialFormEventHandler.imageFieldId.ToString();
            }

						ddlImageTransformationMode.ClearSelection();
						txtThumbnailWidth.Text = null;
						txtThumbnailHeight.Text = null;
            if (specialFormEventHandler.imageTranslate.HasValue && specialFormEventHandler.imageTranslate.Value)
            {
                ddlImageTransformationMode.SelectedValue = specialFormEventHandler.imageTranslationMode;
                if (specialFormEventHandler.imageTranslationHeight.HasValue)
                {
                    txtThumbnailHeight.Text = specialFormEventHandler.imageTranslationHeight.Value.ToString();
                }

                if (specialFormEventHandler.imageTranslationWidth.HasValue)
                {
                    txtThumbnailWidth.Text = specialFormEventHandler.imageTranslationWidth.Value.ToString();
                }
            }

            ddlDescription.ClearSelection();
            if (specialFormEventHandler.descriptionFieldId.HasValue)
            {
                ddlDescription.SelectedValue = specialFormEventHandler.descriptionFieldId.ToString();
            }

            ddlAssociatedDate.ClearSelection();
            if (specialFormEventHandler.associatedDateFieldId.HasValue)
            {
                ddlAssociatedDate.SelectedValue = specialFormEventHandler.associatedDateFieldId.ToString();
            }

          }

          bSuccess = true;
        }
      }
      catch (Exception ex)
			{
        ScmsEvent.Raise("Exception thrown while loading settings", this, ex);
      }

      return bSuccess;
    }

    protected void btnSaveSettings_Clicked(object sender, EventArgs args)
    {
      bool bSuccess = false;

      Page.Validate("special-formsettings");
      if (Page.IsValid)
      {
        try
				{
          scms.data.ScmsDataContext dcScms = new scms.data.ScmsDataContext();
          scms.modules.special.data.specialDataContext dcSpecials = new scms.modules.special.data.specialDataContext();

          if (EventHandlerId.HasValue)
          {
            scms.data.scms_form_eventhandler eventHandler =
                (from eh in dcScms.scms_form_eventhandlers
                 where eh.id == EventHandlerId.Value
                 select eh).FirstOrDefault();

            scms.modules.special.data.scms_special_form_eventhandler specialFormEventHandler =
                (from sfeh in dcSpecials.scms_special_form_eventhandlers
                 where sfeh.eventHandlerId == EventHandlerId.Value
                 select sfeh).FirstOrDefault();

            if (specialFormEventHandler == null)
            {
              specialFormEventHandler = new scms.modules.special.data.scms_special_form_eventhandler();
              specialFormEventHandler.formId = FormId.Value;
              specialFormEventHandler.eventHandlerId = EventHandlerId.Value;
              dcSpecials.scms_special_form_eventhandlers.InsertOnSubmit(specialFormEventHandler);
            }

						int nSiteId = int.Parse(ddlSite2.SelectedValue);
						specialFormEventHandler.siteId = nSiteId;
            specialFormEventHandler.parentPageId = pageSelector.PageId.Value;

            int? nTemplateId = null;
            string strSelectedTemplateId = ddlTemplate.SelectedValue;
            if (!string.IsNullOrEmpty(strSelectedTemplateId))
            {
              int n;
              if (int.TryParse(strSelectedTemplateId, out n))
              {
								nTemplateId = n;
              }
            }
            specialFormEventHandler.templateId = nTemplateId;

            specialFormEventHandler.titleFieldId = null;
            string strTitleFieldId = ddlTitle.SelectedValue;
            if (!string.IsNullOrEmpty(strTitleFieldId))
            {
              specialFormEventHandler.titleFieldId = int.Parse(strTitleFieldId);
            }

            specialFormEventHandler.imageFieldId = null;
            string strImageFieldId = ddlImage.SelectedValue;
            if (!string.IsNullOrEmpty(strImageFieldId))
            {
							specialFormEventHandler.imageFieldId = int.Parse(strImageFieldId);
            }
						specialFormEventHandler.imageTranslate = null;
						specialFormEventHandler.imageTranslationBackgroundColor = null;
						specialFormEventHandler.imageTranslationHeight = null;
						specialFormEventHandler.imageTranslationWidth = null;
						specialFormEventHandler.imageTranslationMode = null;

            string strTransformationMode = ddlImageTransformationMode.SelectedValue;
            if (!string.IsNullOrEmpty(strTransformationMode))
            {
              specialFormEventHandler.imageTranslate = true;
              specialFormEventHandler.imageTranslationMode = strTransformationMode;

              string strHeight = txtThumbnailHeight.Text.Trim();
              if (!string.IsNullOrEmpty(strHeight))
              {
                  specialFormEventHandler.imageTranslationHeight = int.Parse(strHeight);
              }
              string strWidth = txtThumbnailWidth.Text.Trim();
              if (!string.IsNullOrEmpty(strWidth))
              {
								specialFormEventHandler.imageTranslationWidth = int.Parse(strWidth);
              }
            }
            else
            {
							specialFormEventHandler.imageTranslate = false;
            }


            specialFormEventHandler.descriptionFieldId = null;
            string strDescriptionFieldId = ddlDescription.SelectedValue;
            if (!string.IsNullOrEmpty(strDescriptionFieldId))
            {
							specialFormEventHandler.descriptionFieldId = int.Parse(strDescriptionFieldId);
            }

            specialFormEventHandler.associatedDateFieldId = null;
            string strAssociatedFieldId = ddlAssociatedDate.SelectedValue;
            if (!string.IsNullOrEmpty(strAssociatedFieldId))
            {
							specialFormEventHandler.associatedDateFieldId = int.Parse(strAssociatedFieldId);
            }

            dcSpecials.SubmitChanges();


            bSuccess = true;

            if (settingsSavedDelegate != null)
            {
							settingsSavedDelegate();
            }
          }
      }
      catch (Exception ex)
      {
          ScmsEvent.Raise("Exception thrown while loading settings", this, ex);
          statusMessage.ShowFailure("Failed saving settings");
      }
    }
  }

  protected void btnCancel_Clicked(object sender, EventArgs args)
  {
    if (settingsCancelledDelegate != null)
    {
			settingsCancelledDelegate();
    }
  }

  protected void ddlImageTransformationMode_selectedIndexChanged(object sender, EventArgs args)
  {
      EnableControls();
  }

      protected void EnableControls()
      {
          bool bModeSelected = false;
          string strMode = ddlImageTransformationMode.SelectedValue;
          if (!string.IsNullOrEmpty(strMode))
          {
              bModeSelected = true;
          }
          txtThumbnailHeight.Enabled = bModeSelected;
          txtThumbnailWidth.Enabled = bModeSelected;
      }
  }
}