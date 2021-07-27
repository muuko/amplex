using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace scms.admin
{
    [ValidationPropertyAttribute("ModuleInstanceId")]
    public partial class ModuleSelector : System.Web.UI.UserControl
    {
        public delegate void ModuleSelectionChangedDelegate(int? nModuleInstanceId);
        public ModuleSelectionChangedDelegate OnModuleSelectionChanged = null;

        public int? SiteId 
        {
            get;
            set;
        }

        // restrict modules available to this type
        public int? PluginApplicationId
        {
            get;
            set;
        }
        public int? PluginModuleId
        {
            get;
            set;
        }

        protected bool bDisableEvents = false;

        // the selected module
        public int? ModuleInstanceId
        {
            get 
            {
                return (int?)ViewState["ModuleInstanceId"];

                /*
                int? nModuleInstanceId = null;
                string strSelectedValue = ddlShareModule.SelectedValue;
                if (!string.IsNullOrEmpty(strSelectedValue))
                {
                    nModuleInstanceId = int.Parse(strSelectedValue);
                }

                return nModuleInstanceId;
                */
            }

            set 
            {
                bDisableEvents = true;
                SetModuleInstanceId(value);
                ViewState["ModuleInstanceId"] = value;
                bDisableEvents = false;
            }
        }

        protected void SetModuleInstanceId(int? nModuleInstanceId)
        {
            bool bModuleSelected = false;

            if (nModuleInstanceId.HasValue)
            {
                try
                {
                    dc = new global::scms.data.ScmsDataContext();
                    LoadTemplates(dc);

                    // locate page containing module
                    var pageModule = (from p in dc.scms_page_plugin_modules
                                      where p.deleted == false
                                      join i in dc.scms_plugin_module_instances on p.instanceId equals i.id
                                      where i.deleted == false
                                      where i.id == nModuleInstanceId.Value
                                      select new { p.id, p.name, i.pluginAppId, i.pluginModuleId, p.pageId }).FirstOrDefault();
                    if( pageModule != null )
                    {
                        if (PluginApplicationId.HasValue && PluginModuleId.HasValue)
                        {
                            if ((pageModule.pluginAppId != PluginApplicationId.Value) || (pageModule.pluginModuleId != PluginModuleId.Value))
                            {
                                pageModule = null;
                            }
                        }
                    }

                    if (pageModule != null)
                    {
                        ddlModuleSource.SelectedValue = "page";
                        multiViewSharingSource.SetActiveView(viewShareSourcePage);
                        pageSelectorShare.PageId = pageModule.pageId;
                        LoadShareModules();
                    }
                    else
                    {
                        // locate template containing module
                        var templateModule = (from t in dc.scms_template_plugin_modules
                                          where t.deleted == false
                                          join i in dc.scms_plugin_module_instances on t.instanceId equals i.id
                                          where i.deleted == false
                                          where i.id == nModuleInstanceId.Value
                                          select new { t.id, t.name, i.pluginAppId, i.pluginModuleId, t.templateId }).FirstOrDefault();
                        if (templateModule != null)
                        {
                            if (PluginApplicationId.HasValue && PluginModuleId.HasValue)
                            {
                                if ((templateModule.pluginAppId != PluginApplicationId.Value) || (templateModule.pluginModuleId != PluginModuleId.Value))
                                {
                                    templateModule = null;
                                }
                            }
                        }

                        if (templateModule != null)
                        {
                            ddlModuleSource.SelectedValue = "template";
                            multiViewSharingSource.SetActiveView(viewShareSourceTemplate);

                            ListItem liTemplate = ddlTemplateShare.Items.FindByValue(templateModule.templateId.ToString());
                            if (liTemplate != null)
                            {
                                ddlTemplateShare.ClearSelection();
                                liTemplate.Selected = true;
                            }
                            LoadShareModules();
                        }
                    }

                    if (nModuleInstanceId.HasValue)
                    {

                        ListItem li = ddlShareModule.Items.FindByValue(nModuleInstanceId.Value.ToString());
                        if (li != null)
                        {
                            ddlShareModule.ClearSelection();
                            li.Selected = true;
                        }
                    }
                }
                catch (Exception ex)
                {
                    string strMessage = string.Format( "Exception thrown while locating module instance '{0}'.", nModuleInstanceId.Value );
                    ScmsEvent.Raise(strMessage, this, ex);
                }
            }

            if( !bModuleSelected)
            {
                // start over
                ddlModuleSource_SelectedIndexChanged(null, null);
            }
        }


        global::scms.data.ScmsDataContext dc = null;

        protected void Page_Init(object sender, EventArgs e)
        {
            pageSelectorShare.OnPageSelectionChanged += OnSharePageSelectionChanged;
            // OnSharePageSelectionChanged
        }

        protected void LoadTemplates(global::scms.data.ScmsDataContext dc)
        {
            ddlTemplateShare.DataValueField = "id";
            ddlTemplateShare.DataTextField = "name";
            var templates = from t in dc.scms_templates
                            where t.siteId == SiteId.Value
                            select new { t.id, t.name };
            ddlTemplateShare.DataSource = templates;
            ddlTemplateShare.DataBind();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            pageSelectorShare.SiteId = SiteId;
            pageSelectorShare.Inititialize();
            if (!ModuleInstanceId.HasValue)
            {
                if (SiteId.HasValue)
                {
                    dc = new global::scms.data.ScmsDataContext();
                    LoadTemplates(dc);


                    global::scms.ScmsSiteMapProvider provider = new global::scms.ScmsSiteMapProvider();
                    global::scms.ScmsSiteMapProvider.Site site;
                    string strError;
                    Exception exError;
                    if (provider.GetSite(SiteId.Value, out site, out strError, out exError))
                    {
                        ListItem liDefaultTemplate = ddlTemplateShare.Items.FindByValue(site.site.defaultTemplateId.ToString());
                        if (liDefaultTemplate != null)
                        {
                            ddlTemplateShare.ClearSelection();
                            liDefaultTemplate.Selected = true;
                        }
                    }

                    bDisableEvents = true;
                    ddlModuleSource_SelectedIndexChanged(null, null);
                    bDisableEvents = false;
                }
            }
            
        }


        protected void ddlModuleSource_SelectedIndexChanged(object sender, EventArgs args)
        {
            ViewState["ModuleInstanceId"] = null;

            bool bShowShareModules = false;
            switch (ddlModuleSource.SelectedValue.ToLower())
            {
                case "page":
                    multiViewSharingSource.SetActiveView(viewShareSourcePage);
                    bShowShareModules = true;
                    break;

                case "template":
                    multiViewSharingSource.SetActiveView(viewShareSourceTemplate);
                    bShowShareModules = true;
                    break;
            }

            placeholderShareModule.Visible = bShowShareModules;
            LoadShareModules();
        }

        protected void ddlTemplateShare_SelectedIndexChanged(object sender, EventArgs args)
        {
            ViewState["ModuleInstanceId"] = null;
            LoadShareModules();
        }

        protected void OnSharePageSelectionChanged(int? nPageId)
        {
            ViewState["ModuleInstanceId"] = null;
            LoadShareModules();
        }

        protected void LoadShareModules()
        {
            try
            {
                ddlShareModule.Items.Clear();
                ddlShareModule.DataSource = null;

                scms.data.ScmsDataContext dc = new scms.data.ScmsDataContext();
                System.Collections.Generic.Dictionary<string, int> modules = new System.Collections.Generic.Dictionary<string, int>();
                if (ddlModuleSource.SelectedValue == "page")
                {
                    int? nPageId = pageSelectorShare.PageId;
                    if (nPageId.HasValue)
                    {
                        var pageModules = from p in dc.scms_page_plugin_modules
                                          where p.pageId == nPageId.Value
                                          where p.deleted == false
                                          join i in dc.scms_plugin_module_instances on p.instanceId equals i.id
                                          where i.deleted == false
                                          select new { p.name, i.id, i.pluginAppId, i.pluginModuleId };
                        if (PluginApplicationId.HasValue && PluginModuleId.HasValue)
                        {
                            pageModules = pageModules.Where(i => i.pluginAppId == PluginApplicationId.Value && i.pluginModuleId == PluginModuleId.Value);
                        }

                        ddlShareModule.DataSource = pageModules;

                    }
                    
                }
                else
                {
                    int nTemplateId = int.Parse(ddlTemplateShare.SelectedValue);
                    var templateModules = from t in dc.scms_template_plugin_modules
                                          where t.templateId == nTemplateId
                                          where t.deleted == false
                                          join i in dc.scms_plugin_module_instances on t.instanceId equals i.id
                                          where i.deleted == false
                                          select new { t.name, i.id, i.pluginAppId, i.pluginModuleId };
                    if (PluginApplicationId.HasValue && PluginModuleId.HasValue)
                    {
                        templateModules = templateModules.Where(i => i.pluginAppId == PluginApplicationId.Value && i.pluginModuleId == PluginModuleId.Value);
                    }
                    ddlShareModule.DataSource = templateModules;
                }


                ddlShareModule.DataTextField = "name";
                ddlShareModule.DataValueField = "id";
                ddlShareModule.DataBind();

                ddlShareModule_SelectedIndexChanged(null, null);
            }
            catch (Exception ex)
            {
                string strMessage = string.Format("Failed loading share module");
                ScmsEvent.Raise(strMessage, this, ex);
            }
        }

        protected bool GetSelectedShareValues(out bool bShared, out bool? bPage, out int? nPageId, out int? nTemplateId, out int? nModuleInstanceId)
        {
            bool bSuccess = false;

            bShared = false;
            bPage = null;
            nPageId = null;
            nTemplateId = null;
            nModuleInstanceId = null;

            bShared = true;

            switch (ddlModuleSource.SelectedValue.ToLower())
            {
                case "page":
                    bPage = true;
                    break;

                case "template":
                    bPage = false;
                    break;

                default:
                    throw new Exception("unsupported");
            }

            if (bPage.Value)
            {
                nPageId = pageSelectorShare.PageId;
                if (nPageId.HasValue)
                {
                    int n;
                    if (int.TryParse(ddlShareModule.SelectedValue, out n))
                    {
                        nModuleInstanceId = n;
                        bSuccess = true;
                    }
                }
            }
            else
            {
                nTemplateId = int.Parse(ddlTemplateShare.SelectedValue);
                int n;
                if (int.TryParse(ddlShareModule.SelectedValue, out n))
                {
                    nModuleInstanceId = n;
                    bSuccess = true;
                }
            }


            return bSuccess;
        }

        protected void ddlShareModule_SelectedIndexChanged(object sender, EventArgs args)
        {
            
            string strSelectedValue = ddlShareModule.SelectedValue;
            int? nSelectedModule = null;
            if (!string.IsNullOrEmpty(strSelectedValue))
            {
                nSelectedModule = int.Parse(strSelectedValue);
            }

            ViewState["ModuleInstanceId"] = nSelectedModule;

            if (!bDisableEvents)
            {
                if (OnModuleSelectionChanged != null)
                {
                    OnModuleSelectionChanged(nSelectedModule);
                }
            }
        
        }
    }
}
 