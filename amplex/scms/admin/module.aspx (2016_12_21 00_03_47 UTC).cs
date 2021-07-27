using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Xml.Linq;

namespace scms.admin
{
	public partial class module : System.Web.UI.Page
	{
		protected int? nSiteId = null;
		protected int? nTemplateModuleId = null;
		protected int? nPageModuleId = null;

		protected int? nModuleInstanceId = null;

		protected global::scms.data.scms_plugin_application pluginApplication = null;
		protected global::scms.data.scms_plugin_module pluginModule = null;
		protected global::scms.data.scms_plugin_module_instance pluginModuleInstance = null;

		protected global::scms.data.scms_site site = null;

		protected global::scms.data.scms_page page = null;
		protected global::scms.data.scms_page_plugin_module pagePmi = null;
		
		protected global::scms.data.scms_template template = null;
		protected global::scms.data.scms_template_plugin_module templatePmi = null;

    protected bool bOwn = false;
    protected bool bSharing = false;
    protected bool bShared = false;
    protected global::scms.data.scms_page pageOwner = null;
    protected global::scms.data.scms_template templateOwner = null;
        
		protected void Page_Load(object sender, EventArgs e)
		{
			pagesBreadcrumbs.viewMode = scms.admin.controls.AdminPagesBreadcrumbs.EViewMode.Modules;
		}

		protected void Page_Init(object sender, EventArgs e)
		{
			Title = "Admin - Module";

      string strModuleInstanceId = Request.QueryString["id"];
      if( !string.IsNullOrEmpty(strModuleInstanceId))
      {
				int n;
				if( int.TryParse(strModuleInstanceId, out n))
				{
					nModuleInstanceId = n;
				}
      }

      string strPageModuleId = Request.QueryString["pmid"];
      if (!string.IsNullOrEmpty(strPageModuleId))
      {
				int n;
				if (int.TryParse(strPageModuleId, out n))
				{
					nPageModuleId = n;
				}
      }

			string strTemplateModuleId = Request.QueryString["tmid"];
			if (!string.IsNullOrEmpty(strTemplateModuleId))
			{
				int n;
				if (int.TryParse(strTemplateModuleId, out n))
				{
					nTemplateModuleId = n;
				}
			}

			LoadModuleInstanceData();
			SetupOwnership();
            
			if (pluginModule != null)
			{
				Control control = LoadControl(pluginModule.controlPathEditModule);
				if (control is global::scms.RootControl)
				{
					global::scms.RootControl rootControl = (global::scms.RootControl)control;
					rootControl.ModuleInstanceId = pluginModuleInstance.id;

					if (page != null)
					{
						rootControl.PageId = page.id;
						rootControl.SiteId = page.siteid;
						anchorView.Visible = true;


						/* xxx
						string strUrl = page.url;

						global::scms.ScmsSiteMapProvider siteMapProvider = new global::scms.ScmsSiteMapProvider();

						global::scms.ScmsSiteMapProvider.WebsitePages webSitePages;
						Exception 
						if (!siteMapProvider.GetWebSitePages(out webSitePages, out strError, out exError))
						{
							throw new Exception(string.Format("ScmsSiteMapProvider.WebsitePage failed, returned '{0}'.", strError), exError);
						}

						global::scms.ScmsSiteMapProvider.Site site;
						if (!webSitePages.TryGetValue(breadcrumbs.SiteId.Value, out site))
						{
							throw new Exception(string.Format("No website exists for current site id '{0}'.", breadcrumbs.SiteId.Value));
						}

						page.siteid;*/

						string strPageUrl = page.url;
						if (site != null)
						{
							if (!string.IsNullOrEmpty(site.canonicalHostName))
							{
								strPageUrl = string.Format("http://{0}{1}", site.canonicalHostName, strPageUrl);
							}
						}

						anchorView.HRef = strPageUrl;
					}
					else
					{
						if (template != null)
						{
							rootControl.SiteId = template.siteId;
						}
					}
				}
				// placeholderDdl.Controls.Add(control);
				viewSettings.Controls.Add(control);
			}



			if( !IsPostBack )
			{
				if (nPageModuleId.HasValue || nTemplateModuleId.HasValue)
				{
					LoadModuleInstance();
				}

				if (nSiteId.HasValue)
				{
					if (page != null)
					{
						pagesBreadcrumbs.SiteId = nSiteId;
						pagesBreadcrumbs.PageId = page.id;
						multiViewHeader.SetActiveView(viewPageModule);
					}
					else
					{
						multiViewHeader.SetActiveView(viewTemplateModule);
					}

					ShowSettings();
				}

				scms.admin.Admin master = (scms.admin.Admin)this.Master;
				if (page != null)
				{
					master.NavType = Admin.ENavType.Pages;
				}
				else if (template != null)
				{
					master.NavType = Admin.ENavType.Templates;
				}
			}
		}

        protected void SetupOwnership()
        {
            try
            {
                global::scms.data.ScmsDataContext dc = new global::scms.data.ScmsDataContext();

                bOwn = false;
                bShared = false;
                pageOwner = null;
                templateOwner = null;

                int ? nModuleInstanceId = null;
                if (nPageModuleId.HasValue)
                {
                    // should be in page
                    var pagePlusModule = (from ppmi in dc.scms_page_plugin_modules
                                          where ppmi.id == nPageModuleId.Value
                                          where ppmi.deleted == false
                                          join p in dc.scms_pages on ppmi.pageId equals p.id
                                          select new { p, ppmi }).Single();
                    bOwn = pagePlusModule.ppmi.owner;
                    nModuleInstanceId = pagePlusModule.ppmi.instanceId;
                }

                if (nTemplateModuleId.HasValue)
                {
                    var templatePlusModule = (from tpmi in dc.scms_template_plugin_modules
                                              where tpmi.id == nTemplateModuleId.Value
                                              where tpmi.deleted == false
                                              join t in dc.scms_templates on tpmi.templateId equals t.id
                                              where t.deleted == false
                                              select new { t, tpmi }).Single();
                    
                    bOwn = templatePlusModule.tpmi.owner;
                    nModuleInstanceId = templatePlusModule.tpmi.instanceId;
                }


                // need module isntance
                if (nModuleInstanceId.HasValue)
                {
                    if (bOwn)
                    {
                        // find if shared
                        var pagesSharing = from ppmi in dc.scms_page_plugin_modules
                                           join p in dc.scms_pages on ppmi.pageId equals p.id
                                           where ppmi.instanceId == nModuleInstanceId.Value
                                           // where ((nPageModuleId == null) || (ppmi.pageId != nCurrentPageId.Value))
                                           orderby p.url
                                           select p;
                        if (page != null)
                        {
                            pagesSharing = pagesSharing.Where(p => p.id != page.id);
                        }

                        if( pagesSharing.Count() > 0 )
                        {
                            bSharing = true;

                            System.Text.StringBuilder sbSharedPages = new System.Text.StringBuilder();
                            foreach (var pageSharing in pagesSharing)
                            {
                                sbSharedPages.AppendFormat("<a href=\"/scms/admin/pages.aspx?pid={0}\">{1}</a><br />", pageSharing.id, pageSharing.url);
                            }
                            literalSharedPages.Text = sbSharedPages.ToString();
                        }
                        else
                        {
                            literalSharedPages.Text = "none";
                        }

                        var templatesSharing = from tpmi in dc.scms_template_plugin_modules
                                                join t in dc.scms_templates on tpmi.templateId equals t.id
                                                where tpmi.instanceId == nModuleInstanceId
//                                                where ((nTemplateModuleId == null) || (tpmi.templateId != template.id))
                                                orderby t.name
                                                select t;
                        if (template != null)
                        {
                            templatesSharing = templatesSharing.Where(t => t.id != template.id);
                        }

                        if( templatesSharing.Count() > 0)
                        {
                            bSharing = true;
                            System.Text.StringBuilder sbSharedTemplates = new System.Text.StringBuilder();
                            foreach (var templateSharing in templatesSharing)
                            {
                                sbSharedTemplates.AppendFormat("<a href=\"/scms/admin/template.aspx?sid={0}&tid={1}\">{2}</a><br />", nSiteId.Value, templateSharing.id, templateSharing.name);
                            }
                            literalSharedTemplates.Text = sbSharedTemplates.ToString();
                        }
                        else
                        {
                            literalSharedTemplates.Text = "none";
                        }
                    }
                    else
                    {
                        // locate owner
                        pageOwner = (from ppmi in dc.scms_page_plugin_modules
                                     join p in dc.scms_pages on ppmi.pageId equals p.id
                                     where ppmi.instanceId == nModuleInstanceId
                                     where ppmi.owner == true
                                     select p).FirstOrDefault();

                        if (pageOwner == null)
                        {
                            templateOwner = (from tpmi in dc.scms_template_plugin_modules
                                             join t in dc.scms_templates on tpmi.instanceId equals nModuleInstanceId
                                             where tpmi.owner == true
                                             select t).FirstOrDefault();
                        }
                    }
                }

                if (bOwn)
                {
                    if (bSharing)
                    {
                        multiViewOwnership.SetActiveView(viewOwnerSharing);
                    }
                    else
                    {
                        multiViewOwnership.SetActiveView(viewOwnerNotSharing);
                    }
                }
                else
                {
                    multiViewOwnership.SetActiveView(viewShared);

                    if (pageOwner != null)
                    {
                        anchorOwner.HRef = string.Format("/scms/admin/pages.aspx?pid={0}", pageOwner.id);
                        anchorOwner.InnerText = pageOwner.url;
                    }

                    if (templateOwner != null)
                    {
                        anchorOwner.HRef = string.Format("/scms/admin/template.aspx?sid={0}&tid={1}", nSiteId, templateOwner.id);
                        anchorOwner.InnerText = string.Format("{0} template", templateOwner.name);

                    }
                }
            }
            catch (Exception ex)
            {
                string strMessage = "Failed setting up module ownership information";
                ScmsEvent.Raise(strMessage, this, ex);
            }
        }

		protected void LoadModuleInstance()
		{

			if (template != null)
			{
				placeholderDdl.SiteId = nSiteId;
				placeholderDdl.MasterFileId = template.masterId;

				placeholderDdl.LoadPlaceholders();
			}

			if (pluginModuleInstance != null)
			{
				literalModuleInstanceId.Text = pluginModuleInstance.id.ToString();
				checkWrapModule.Checked = pluginModuleInstance.wrapModule;
				txtCssClassWrap.Text = pluginModuleInstance.cssClassWrap;
			}

			if (pagePmi != null)
			{
				literalModuleName.Text = pagePmi.name;

				txtModuleName.Text = pagePmi.name;
				placeholderDdl.SelectedValue = pagePmi.placeHolder;
				literalOrdinal.Text = pagePmi.ordinal.ToString();
				checkOwner.Checked = pagePmi.owner;
				literalPageModuleInstanceId.Text = pagePmi.id.ToString();
				checkOverrideTemplate.Checked = pagePmi.overrideTemplate;

				multiViewAdvanced.SetActiveView(viewAdvancedPage);
			}
			else
			{
				if (templatePmi != null)
				{
					anchorTemplate.InnerText = template.name;
					anchorTemplate.HRef = string.Format("/scms/admin/template.aspx?sid={0}&tid={1}", nSiteId, template.id);
					literalModuleName.Text = templatePmi.name;

					txtModuleName.Text = templatePmi.name;
					placeholderDdl.SelectedValue = templatePmi.placeHolder;
					literalOrdinal.Text = templatePmi.ordinal.ToString();
					checkOwner.Checked = templatePmi.owner;
					literalTemplateModuleInstanceId.Text = templatePmi.id.ToString();

					multiViewAdvanced.SetActiveView(viewAdvancedTemplate);
				}
			}
		}

		protected void LoadModuleInstanceData()
		{
			global::scms.data.ScmsDataContext dc = new global::scms.data.ScmsDataContext();


            if (nModuleInstanceId.HasValue)
            {
                if ((!nPageModuleId.HasValue) && (!nTemplateModuleId.HasValue))
                {
                    var p = (from ppmi in dc.scms_page_plugin_modules
                             where ppmi.instanceId == nModuleInstanceId.Value
                             where ppmi.deleted == false
                             select ppmi).FirstOrDefault();
                    if( p != null )
                    {
                        nPageModuleId = p.id;
                    }

                    if (!nPageModuleId.HasValue)
                    {
                        var t = (from tpmi in dc.scms_template_plugin_modules
                                 where tpmi.instanceId == nModuleInstanceId.Value
                                 select tpmi).FirstOrDefault();
                        if (t != null)
                        {
                            nTemplateModuleId = t.id;
                        }
                    }

                }
            }


			if (nPageModuleId.HasValue)
			{
				// should be in page
				var pagePlusModule = (from ppmi in dc.scms_page_plugin_modules
															where ppmi.id == nPageModuleId.Value
															where ppmi.deleted == false
															join p in dc.scms_pages on ppmi.pageId equals p.id
															select new { p, ppmi }).Single();
				page = pagePlusModule.p;
				pagePmi = pagePlusModule.ppmi;
				nModuleInstanceId = pagePmi.instanceId;

				template = (from t in dc.scms_templates
				  					where t.id == page.templateId
										where t.deleted == false
										select t).Single();
			}

			if (nTemplateModuleId.HasValue)
			{
				var templatePlusModule = (from tpmi in dc.scms_template_plugin_modules
																	where tpmi.id == nTemplateModuleId.Value
																	where tpmi.deleted == false
																	join t in dc.scms_templates on tpmi.templateId equals t.id
																	where t.deleted == false
																	select new { t, tpmi }).Single();
				template = templatePlusModule.t;
				templatePmi = templatePlusModule.tpmi;
				nModuleInstanceId = templatePmi.instanceId;
			}


			// need module isntance
			if( nModuleInstanceId.HasValue )
			{
				// determine generic info
				// plugin app, module, instance
				var module = (from pmi in dc.scms_plugin_module_instances
												where pmi.id == nModuleInstanceId
												where pmi.deleted == false
												join pm in dc.scms_plugin_modules on pmi.pluginModuleId equals pm.id
												join pa in dc.scms_plugin_applications on pmi.pluginAppId equals pa.id
												select new { pa, pm, pmi }).SingleOrDefault();
				if (module != null)
				{
					nSiteId = module.pmi.siteId;

					pluginApplication = module.pa;
					pluginModule = module.pm;
					pluginModuleInstance = module.pmi;
				}
			}

			if (nSiteId.HasValue)
			{
				site = (from s in dc.scms_sites 
								where s.id == nSiteId.Value
								select s).Single();
			}
		}

		protected void menuTabs_Click(object sender, MenuEventArgs args)
		{
			switch (menuTabs.SelectedValue.ToLower())
			{
				case "advanced":
					ShowAdvanced();
					break;

				case "settings":
					ShowSettings();
					break;
			}
		}

		protected void ShowAdvanced()
		{
			multiViewBody.SetActiveView(viewAdvanced);
		}

		protected void ShowSettings()
		{
			multiViewBody.SetActiveView(viewSettings);
		}

		protected void btnSaveAdvanced_Click(object sender, EventArgs args)
		{
			try
			{
				global::scms.data.ScmsDataContext dc = new global::scms.data.ScmsDataContext();


				var pluginModuleInstance = (from pmi in dc.scms_plugin_module_instances
																		where pmi.id == nModuleInstanceId
																		where pmi.deleted == false
																		select pmi).Single();

				pluginModuleInstance.wrapModule = checkWrapModule.Checked;
				if( pluginModuleInstance.wrapModule )
				{
					pluginModuleInstance.cssClassWrap = txtCssClassWrap.Text;
				}
				else
				{
					pluginModuleInstance.cssClassWrap = null;
				}
				
				string strPlaceholder = placeholderDdl.SelectedValue;
				if (nPageModuleId.HasValue)
				{
					var pageModule = (from pm in dc.scms_page_plugin_modules
														where pm.id == nPageModuleId.Value
														where pm.deleted == false
														select pm).Single();
					pageModule.name = txtModuleName.Text.Trim();

					// check if placeholder changed
					if (string.Compare(pageModule.placeHolder, strPlaceholder, true) != 0)
					{
						pageModule.placeHolder = strPlaceholder;

						// reordinate
						int nOrdinal = 0;
						var moduleSiblings = (from pm in dc.scms_page_plugin_modules
																	where pm.pageId == pageModule.pageId
																	where pm.deleted == false
																	where pm.placeHolder.ToLower() == strPlaceholder.ToLower()
																	where pm.id != pageModule.id
																	select pm);
						foreach (var moduleSibling in moduleSiblings)
						{
							moduleSibling.ordinal = nOrdinal;
							nOrdinal++;
						}
						pageModule.ordinal = nOrdinal;
					}
					pageModule.overrideTemplate = checkOverrideTemplate.Checked;
				}
				else if (nTemplateModuleId.HasValue)
				{
					var templateModule = (from tm in dc.scms_template_plugin_modules
																where tm.id == nTemplateModuleId.Value
																where tm.deleted == false
																select tm).Single();
					templateModule.name = txtModuleName.Text.Trim();

					if (string.Compare(templateModule.placeHolder, strPlaceholder, true) != 0)
					{
						templateModule.placeHolder = strPlaceholder;

						// reordinate
						int nOrdinal = 0;
						var moduleSiblings = (from tm in dc.scms_template_plugin_modules
																	where tm.templateId == templateModule.templateId
																	where tm.deleted == false
																	where tm.placeHolder.ToLower() == strPlaceholder.ToLower()
																	where tm.id != templateModule.id
																	select tm);
						foreach (var moduleSibling in moduleSiblings)
						{
							moduleSibling.ordinal = nOrdinal;
							nOrdinal++;
						}
						templateModule.ordinal = nOrdinal;
					}
				}
				else
				{
					throw new Exception("Unexpected no value for page id or template id");
				}

				dc.SubmitChanges();
				global::scms.CacheManager.Clear();

				status.ShowSuccess("Success");
			}
			catch( Exception ex )
			{
				string strMessage = string.Format( "Exception thrown during save: '{0}'.", ex.ToString() );
				status.ShowFailure( strMessage);
			}
																

			
		}

		protected void btnTakeOwnership_Click(object sender, EventArgs args)
		{
            try
            {
                scms.data.ScmsDataContext dc = new scms.data.ScmsDataContext();

                if (pagePmi != null)
                {
                    var pagePmi2 = (from ppmi in dc.scms_page_plugin_modules
                                    where ppmi.id == pagePmi.id
                                    select ppmi).Single();
                    pagePmi2.owner = true;
                }

                if (templatePmi != null)
                {
                    var templatePmi2 = (from ttmi in dc.scms_template_plugin_modules
                              where ttmi.id == templatePmi.id
                              select ttmi).Single();
                    templatePmi2.owner = true;
                }

                // locate owner(s)
                var ppmiOwners = from ppmi in dc.scms_page_plugin_modules
                                 join p in dc.scms_pages on ppmi.pageId equals p.id
                                 where ppmi.instanceId == nModuleInstanceId
                                 where ppmi.owner == true
                                 select ppmi;

                var tpmiOwners = from tpmi in dc.scms_template_plugin_modules
                                 join t in dc.scms_templates on tpmi.instanceId equals nModuleInstanceId
                                 where tpmi.owner == true
                                 select tpmi;

                foreach (var ppmiOwner in ppmiOwners)
                {
                    ppmiOwner.owner = false;
                }

                foreach (var tpmiOwner in tpmiOwners)
                {
                    tpmiOwner.owner = false;
                }

                dc.SubmitChanges();
                status.ShowSuccess("Ownership taken");

                SetupOwnership();
                checkOwner.Checked = true;
            }
            catch (Exception ex)
            {
                string strMessage = "Failed obtaining ownership";
                status.ShowFailure(strMessage);
                ScmsEvent.Raise(strMessage, this, ex);
            }
        }

		/*
		protected void btnDelete_Click(object sender, EventArgs args)
		{
			bool bSuccess = false;

			try
			{
				global::scms.data.ScmsDataContext dc = new global::scms.data.ScmsDataContext();
				dc.Connection.Open();
				dc.Transaction = dc.Connection.BeginTransaction();

				string strRedirectUrl = null;


				if (nPageModuleId.HasValue)
				{
					var pageModule = (from pm in dc.scms_page_plugin_modules
														where pm.id == nPageModuleId.Value
														where pm.deleted == false
														select pm).Single();
					pageModule.deleted = true;
					strRedirectUrl = string.Format("/scms/admin/pages.aspx?pid={0}&show=modules", pageModule.pageId);
				}
				else if (nTemplateModuleId.HasValue)
				{
					var templateModule = (from tm in dc.scms_template_plugin_modules
																where tm.id == nTemplateModuleId.Value
																where tm.deleted == false
																select tm).Single();
					templateModule.deleted = true;
					strRedirectUrl = string.Format("/scms/admin/template.aspx?sid={0}&tid={1}&show=modules", templateModule.siteId, templateModule.templateId);
				}
				else
				{
					throw new Exception("Unexpected no value for page id or template id");
				}

				var instance = (from pmi in dc.scms_plugin_module_instances
												where pmi.id == nModuleInstanceId
												select pmi).Single();
				instance.deleted = true;


				dc.SubmitChanges();
				dc.Transaction.Commit();
				global::scms.CacheManager.Clear();

				Response.Redirect(strRedirectUrl);
			}
			catch (System.Threading.ThreadAbortException)
			{
			}
			catch (Exception ex)
			{
				string strMessage = string.Format("Failed deleting module, exception thrown: {0}.", ex.ToString());
				status.ShowFailure(strMessage);
			}
			
		}
		*/
	}
}
