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
    public partial class NewModule : System.Web.UI.Page
    {
        protected int? nSiteId = null;
        protected int? nPageId = null;
        protected int? nTemplateId = null;
        protected global::scms.ScmsSiteMapProvider.PageNode pageNode = null;
        global::scms.data.ScmsDataContext dc = null;

        // for setting default module to content
        int? nContentPluginApplicationId = null;

        protected void Page_Init(object sender, EventArgs e)
        {
            pageSelectorShare.OnPageSelectionChanged += OnSharePageSelectionChanged;
            // OnSharePageSelectionChanged
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            Title = "Admin - New Module";


            string strSiteId = Request.QueryString["sid"];
            if (!string.IsNullOrEmpty(strSiteId))
            {
                int n;
                if (int.TryParse(strSiteId, out n))
                {
                    nSiteId = n;
                }
            }
            pageSelectorShare.SiteId = nSiteId;

            string strPageId = Request.QueryString["pid"];
            if (!string.IsNullOrEmpty(strPageId))
            {
                int n;
                if (int.TryParse(strPageId, out n))
                {
                    nPageId = n;
                }
            }

            string strTemplateId = Request.QueryString["tid"];
            if (!string.IsNullOrEmpty(strTemplateId))
            {
                int n;
                if (int.TryParse(strTemplateId, out n))
                {
                    nTemplateId = n;
                }
            }

            if (true || !IsPostBack) // dont touch this works
            {
                scms.admin.Admin master = (scms.admin.Admin)this.Master;
                if (nPageId.HasValue)
                {
                    master.NavType = Admin.ENavType.Pages;
                }
                else if (nTemplateId.HasValue)
                {
                    master.NavType = Admin.ENavType.Templates;
                }

                DataBind();

                txtName.Focus();

            }

            if (IsPostBack)
            {
                string strEventArgument = Request["__EVENTARGUMENT"];
                if (string.Compare(strEventArgument, "radio", true) == 0)
                {
                    LoadShareModules();
                }
            }
            else
            {
                EnableControls();
            }
        }

        public override void DataBind()
        {

            base.DataBind();

            dc = new global::scms.data.ScmsDataContext();
            if (nSiteId.HasValue)
            {

                var pluginApplications = from pa in dc.scms_plugin_applications
                                         orderby pa.name
                                         select pa;
                lvPluginApplications.DataSource = pluginApplications;
                lvPluginApplications.DataBind();


                global::scms.ScmsSiteMapProvider provider = new global::scms.ScmsSiteMapProvider();

                string strError;
                Exception exError;
                if (nPageId.HasValue)
                {
                    global::scms.ScmsSiteMapProvider.PageNode pageNode;

                    if (provider.GetPageNode(nPageId.Value, out pageNode, out strError, out exError))
                    {
                        nTemplateId = pageNode.page.templateId;
                        // var template global::scms.Templates.GetTemplate(nSiteId, nTemplateId
                    }


                    placeholderOverride.Visible = true;
                }

                if (nTemplateId.HasValue)
                {
                    global::scms.Template template;
                    if (global::scms.Templates.GetTemplate(nSiteId.Value, nTemplateId.Value, out template, out strError, out exError))
                    {
                        placeholderDdl.SiteId = nSiteId.Value;
                        placeholderDdl.MasterFileId = template.ScmsTemplate.masterId;
                        placeholderDdl.LoadPlaceholders();
                    }
                }

                if (!IsPostBack)
                {
                    ddlTemplateShare.DataValueField = "id";
                    ddlTemplateShare.DataTextField = "name";
                    var templates = from t in dc.scms_templates
                                    where t.siteId == nSiteId.Value
                                    select new { t.id, t.name };
                    ddlTemplateShare.DataSource = templates;
                    ddlTemplateShare.DataBind();

                    ScmsSiteMapProvider.Site site;
                    if (provider.GetSite(nSiteId.Value, out site, out strError, out exError))
                    {
                        ListItem liDefaultTemplate = ddlTemplateShare.Items.FindByValue(site.site.defaultTemplateId.ToString());
                        if (liDefaultTemplate != null)
                        {
                            ddlTemplateShare.ClearSelection();
                            liDefaultTemplate.Selected = true;
                        }
                    }
                }
            }
        }


        protected void lvPluginApplications_ItemDataBound(object sender, ListViewItemEventArgs args)
        {
            if (args.Item.ItemType == ListViewItemType.DataItem)
            {
                ListViewDataItem dataItem = (ListViewDataItem)args.Item;
                global::scms.data.scms_plugin_application pluginApplication = (global::scms.data.scms_plugin_application)dataItem.DataItem;
                if (string.Compare(pluginApplication.name, "content", true) == 0)
                {
                    nContentPluginApplicationId = pluginApplication.id;
                }


                ListView lvPluginModules = (ListView)args.Item.FindControl("lvPluginModules");
                if (lvPluginModules != null)
                {
                    var pluginModules = from pm in dc.scms_plugin_modules
                                        where pm.pluginAppId == pluginApplication.id
                                        orderby pm.name
                                        select pm;

                    lvPluginModules.DataSource = pluginModules;
                    lvPluginModules.DataBind();
                }
            }
        }

        protected void lvPluginModules_ItemDataBound(object sender, ListViewItemEventArgs args)
        {
            if (args.Item.ItemType == ListViewItemType.DataItem)
            {
                ListViewDataItem dataItem = (ListViewDataItem)args.Item;
                global::scms.data.scms_plugin_module pluginModule = (global::scms.data.scms_plugin_module)dataItem.DataItem;

                PlaceHolder placeholderRadio = (PlaceHolder)args.Item.FindControl("placeholderRadio");
                if (placeholderRadio != null)
                {
                    HtmlGenericControl radio = new HtmlGenericControl("input");
                    radio.Attributes["type"] = "radio";
                    radio.Attributes["name"] = "radio";
                    string strValue = string.Format("{0}:{1}", pluginModule.pluginAppId.ToString(), pluginModule.id.ToString());
                    radio.Attributes["value"] = strValue;
                    radio.Attributes["onClick"] = Page.ClientScript.GetPostBackEventReference(radio, "radio");
                    placeholderRadio.Controls.Add(radio);




                    if (IsPostBack)
                    {
                        string strRadio = Request.Params["radio"];
                        if (!string.IsNullOrEmpty(strRadio))
                        {
                            if (string.Compare(strRadio, strValue, true) == 0)
                            {
                                radio.Attributes["checked"] = "true";
                            }
                        }
                    }
                    else
                    {
                        if (string.Compare(pluginModule.name, "content", true) == 0)
                        {
                            if (nContentPluginApplicationId.HasValue)
                            {
                                if (pluginModule.pluginAppId == nContentPluginApplicationId.Value)
                                {
                                    radio.Attributes["checked"] = "true";
                                }
                            }
                        }
                    }
                }
            }
        }


        protected void btnNew_Click(object sender, EventArgs args)
        {
            if (Page.IsValid)
            {
                // string strRadio = Request.Params["radioModule"];

                string strRadioSelected = null;

                strRadioSelected = Request.Params["radio"];
                if (string.IsNullOrEmpty(strRadioSelected))
                {
                    throw new Exception("could not find selected radio module");
                }

                string[] astrComponents = strRadioSelected.Split(new char[] { ':' }, StringSplitOptions.RemoveEmptyEntries);
                if (astrComponents.Length != 2)
                {
                    throw new Exception("failed parsing selected radio module");
                }

                int nPluginApplicationId = int.Parse(astrComponents[0]);
                int nPluginModuleId = int.Parse(astrComponents[1]);

                if (nSiteId.HasValue && (nPageId.HasValue || nTemplateId.HasValue))
                {
                    try
                    {
                        string strDestinationUrl = null;

                        dc = new global::scms.data.ScmsDataContext();
                        dc.Connection.Open();
                        dc.Transaction = dc.Connection.BeginTransaction();


                        var pluginApplications = (from pa in dc.scms_plugin_applications
                                                  where pa.id == nPluginApplicationId
                                                  select pa).Single();
                        var pluginModule = (from pm in dc.scms_plugin_modules
                                            where pm.pluginAppId == nPluginApplicationId
                                            where pm.id == nPluginModuleId
                                            select pm).Single();

                        string strModuleNameEntered = txtName.Text.Trim();
                        string strModulInstanceName;
                        string strError;
                        Exception exError;
                        if (!GetUniqueModuleName(dc, pluginModule.name, strModuleNameEntered, nPageId, nTemplateId, out strModulInstanceName, out strError, out exError))
                        {
                            throw new Exception(string.Format("Failed getting unique name for module: '{0}'.", strError), exError);
                        }


                        bool bShared = false;
                        bool? bSharedPage = null;
                        int? nSharedPageId = null;
                        int? nSharedTemplateId = null;
                        int? nSharedModuleInstanceId = null;
                        if (!GetSelectedShareValues(out bShared, out bSharedPage, out nSharedPageId, out nSharedTemplateId, out nSharedModuleInstanceId))
                        {
                            throw new Exception("Unexpected failure getting selected share values");
                        }

                        global::scms.data.scms_plugin_module_instance instance = null;
                        if (bShared)
                        {
                            instance = (from i in dc.scms_plugin_module_instances
                                        where i.id == nSharedModuleInstanceId.Value
                                        select i).Single();
                        }
                        else
                        {
                            instance = new global::scms.data.scms_plugin_module_instance();
                            instance.pluginAppId = nPluginApplicationId;
                            instance.pluginModuleId = nPluginModuleId;
                            instance.siteId = nSiteId.Value;
                            instance.deleted = false;

                            dc.scms_plugin_module_instances.InsertOnSubmit(instance);

                            dc.SubmitChanges();
                        }

                        if (nPageId.HasValue)
                        {
                            // determine ordinal
                            int nOrdinal;
                            var maxOrdinalModule = (from ppmiMax in dc.scms_page_plugin_modules
                                                    where ppmiMax.siteId == nSiteId.Value
                                                    where ppmiMax.pageId == nPageId.Value
                                                    where ppmiMax.deleted == false
                                                    orderby ppmiMax.ordinal descending
                                                    select new { ordinal = ppmiMax.ordinal }).FirstOrDefault();
                            if (maxOrdinalModule == null)
                            {
                                nOrdinal = 0;
                            }
                            else
                            {
                                nOrdinal = maxOrdinalModule.ordinal + 1;
                            }


                            global::scms.data.scms_page_plugin_module ppmi = new global::scms.data.scms_page_plugin_module();
                            ppmi.siteId = nSiteId.Value;
                            ppmi.pageId = nPageId.Value;
                            ppmi.instanceId = instance.id;
                            ppmi.name = strModulInstanceName;
                            ppmi.ordinal = nOrdinal;
                            ppmi.owner = !bShared;
                            ppmi.placeHolder = placeholderDdl.SelectedValue;
                            ppmi.overrideTemplate = checkOverrideTemplate.Checked;
                            ppmi.deleted = false;
                            dc.scms_page_plugin_modules.InsertOnSubmit(ppmi);
                            dc.SubmitChanges();

                            strDestinationUrl = string.Format("/scms/admin/module.aspx?pmid={0}", ppmi.id);
                        }
                        else if (nTemplateId.HasValue)
                        {
                            // determine ordinal
                            int nOrdinal;
                            var maxOrdinalModule = (from tmiMax in dc.scms_template_plugin_modules
                                                    where tmiMax.siteId == nSiteId.Value
                                                    where tmiMax.templateId == nTemplateId
                                                    where tmiMax.deleted == false
                                                    orderby tmiMax.ordinal descending
                                                    select new { ordinal = tmiMax.ordinal }).FirstOrDefault();
                            if (maxOrdinalModule == null)
                            {
                                nOrdinal = 0;
                            }
                            else
                            {
                                nOrdinal = maxOrdinalModule.ordinal + 1;
                            }


                            global::scms.data.scms_template_plugin_module tmi = new global::scms.data.scms_template_plugin_module();
                            tmi.siteId = nSiteId.Value;
                            tmi.templateId = nTemplateId.Value;
                            tmi.instanceId = instance.id;
                            tmi.name = strModulInstanceName;
                            tmi.ordinal = nOrdinal;
                            tmi.owner = !bShared;
                            tmi.placeHolder = placeholderDdl.SelectedValue;
                            tmi.deleted = false;
                            dc.scms_template_plugin_modules.InsertOnSubmit(tmi);
                            dc.SubmitChanges();

                            strDestinationUrl = string.Format("/scms/admin/module.aspx?tmid={0}", tmi.id);
                        }

                        dc.Transaction.Commit();
                        global::scms.CacheManager.Clear();
                        Response.Redirect(strDestinationUrl, false);
                    }
                    catch (Exception ex)
                    {
                        if (dc.Transaction != null)
                        {
                            dc.Transaction.Rollback();
                        }
                        string strMessage = "Exception thrown in btnNew_Click";
                        throw new Exception(strMessage, ex);
                    }
                    finally
                    {
                        dc.Connection.Close();
                    }
                }
            }
        }

        public static bool GetUniqueModuleName(global::scms.data.ScmsDataContext dc, string strPluginModuleName, string strModuleNameEntered, int? nPageId, int? nTemplateId, out string strInstanceName, out string strError, out Exception exError)
        {
            bool bSuccess = false;

            strInstanceName = null;
            strError = null;
            exError = null;

            if (nPageId.HasValue || nTemplateId.HasValue)
            {
                string strBaseName = strModuleNameEntered;
                if (string.IsNullOrEmpty(strBaseName))
                {
                    strBaseName = strPluginModuleName;
                }

                try
                {
                    int nIncrement = 0;

                    bool bUniqueNameFound = false;
                    while (!bUniqueNameFound)
                    {
                        string strTryName = null;
                        if (nIncrement > 0)
                        {
                            strTryName = string.Format("{0}({1})", strBaseName, nIncrement);
                        }
                        else
                        {
                            strTryName = strBaseName;
                        }

                        if (nPageId.HasValue)
                        {
                            var pagePluginModule = (from ppm in dc.scms_page_plugin_modules
                                                    where ppm.pageId == nPageId.Value
                                                    where ppm.name == strTryName
                                                    where ppm.deleted == false
                                                    select ppm).FirstOrDefault();
                            if (pagePluginModule == null)
                            {
                                strInstanceName = strTryName;
                                bUniqueNameFound = true;
                            }
                        }
                        else if (nTemplateId.HasValue)
                        {

                            var templatePluginModule = (from tpm in dc.scms_template_plugin_modules
                                                        where tpm.templateId == nTemplateId.Value
                                                        where tpm.name == strTryName
                                                        where tpm.deleted == false
                                                        select tpm).FirstOrDefault();

                            if (templatePluginModule == null)
                            {
                                strInstanceName = strTryName;
                                bUniqueNameFound = true;
                            }
                        }
                        else
                        {
                            throw new Exception("Unexpected");
                        }

                        nIncrement++;
                    }

                    bSuccess = bUniqueNameFound;
                }
                catch (Exception ex)
                {
                    strError = "Exception thrown";
                    exError = ex;
                }
            }

            return bSuccess;
        }

        protected void EnableControls()
        {
            bool bSharingEnabled = checkShare.Checked;
            labelModuleSource.Enabled = bSharingEnabled;
            ddlModuleSource.Enabled = bSharingEnabled;
        }

        protected void checkShare_CheckChanged(object sender, EventArgs args)
        {
            EnableControls();
            if (checkShare.Checked)
            {
                ddlModuleSource_SelectedIndexChanged(null, null);
            }
            else
            {
                multiViewSharingSource.SetActiveView(viewSharingDisabled);
            }
        }

        protected void ddlModuleSource_SelectedIndexChanged(object sender, EventArgs args)
        {
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
            LoadShareModules();
        }

        protected void OnSharePageSelectionChanged(int? nPageId)
        {
            LoadShareModules();
        }

        protected void LoadShareModules()
        {
            try
            {
                string strRadioSelected = Request.Params["radio"];

                string[] astrComponents = strRadioSelected.Split(new char[] { ':' }, StringSplitOptions.RemoveEmptyEntries);
                if (astrComponents.Length != 2)
                {
                    throw new Exception("failed parsing selected radio module");
                }

                int nPluginApplicationId = int.Parse(astrComponents[0]);
                int nPluginModuleId = int.Parse(astrComponents[1]);


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
                                          where i.pluginAppId == nPluginApplicationId
                                          where i.pluginModuleId == nPluginModuleId
                                          where i.deleted == false
                                          select new { p.name, i.id };
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
                                          where i.pluginAppId == nPluginApplicationId
                                          where i.pluginModuleId == nPluginModuleId
                                          where i.deleted == false
                                          select new { t.name, i.id };
                    ddlShareModule.DataSource = templateModules;
                }


                ddlShareModule.DataTextField = "name";
                ddlShareModule.DataValueField = "id";
                ddlShareModule.DataBind();
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

            if (checkShare.Checked)
            {
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

            }
            else
            {
                bSuccess = true;
                bShared = false;
            }


            return bSuccess;
        }

        protected void cvShare_ServerValidate(object sender, ServerValidateEventArgs args)
        {
            bool bIsValid = false;

            bool bShared = false;
            bool? bPage = null;
            int? nPageId = null;
            int? nTemplateId = null;
            int? nModuleInstanceId = null;
            if (GetSelectedShareValues(out bShared, out bPage, out nPageId, out nTemplateId, out nModuleInstanceId))
            {
                bIsValid = true;
            }

            args.IsValid = bIsValid;
        }
    }
}

