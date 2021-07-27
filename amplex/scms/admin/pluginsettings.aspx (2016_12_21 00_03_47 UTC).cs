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
    public partial class pluginsettings : System.Web.UI.Page
    {
        protected int? pluginId = null;
        protected scms.data.scms_plugin_application plugin = null;
        scms.data.ScmsDataContext dc = null;

        protected void Page_Init(object sender, EventArgs e)
        {
            try
            {
                string strPluginId = Request.QueryString["id"];
                if (string.IsNullOrEmpty(strPluginId))
                {
                    throw new Exception("missing required query parameter 'id'");
                }
                else
                {
                    int n;
                    if (!int.TryParse(strPluginId, out n))
                    {
                        string strMessage = string.Format("failed parsing query parameter id '{0}' as int.", strPluginId);
                        throw new Exception(strMessage);
                    }
                    pluginId = n;

                    if (dc == null)
                    {
                        dc = new scms.data.ScmsDataContext();
                    }
                    if (LoadPlugin(dc))
                    {
                        literalPluginName.Text = plugin.name;

                        Control control = LoadControl(plugin.controlPathSettings);
                        if (control != null)
                        {
                            RootControl rootControl = control as RootControl;
                            if (rootControl != null)
                            {
                                rootControl.SiteId = siteDdl.SiteId;
                            }
                            this.viewSettings.Controls.Add(control);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                scms.ScmsEvent.Raise(new scms.ScmsEvent("pluginsettings.Page_Init() failed", this, ex));
            }
        }


        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                scms.admin.Admin master = (scms.admin.Admin)this.Master;
                master.NavType = scms.admin.Admin.ENavType.Plugins;

                //string strPluginId = Request.QueryString["id"];
                //if (!string.IsNullOrEmpty(strPluginId))
                //{
                //  scms.data.ScmsDataContext dc = new scms.data.ScmsDataContext();
                //  scms.data.scms_plugin_application plugin = null;
                //  if (LoadPlugin(dc, out plugin))
                //  {
                //  }
                //}
            }
        }

        protected bool LoadPlugin(scms.data.ScmsDataContext dc)
        {
            bool bSuccess = false;

            try
            {
                if (pluginId.HasValue)
                {
                    plugin = (from p in dc.scms_plugin_applications
                              where p.id == pluginId.Value
                              select p).FirstOrDefault();
                    if (plugin != null)
                    {
                        bSuccess = true;
                    }
                    else
                    {
                        throw new Exception(string.Format("Plugin appliction id '{0}' not fuond", plugin.id));
                    }
                }
                else
                {
                    throw new Exception("No plugin application id provided");
                }
            }
            catch (Exception ex)
            {
                scms.ScmsEvent.Raise(new scms.ScmsEvent("pluginsettings.LoadPlugin() failed", this, ex));
            }

            return bSuccess;
        }

        protected void menu_menuItemClick(object sender, EventArgs args)
        {
            switch (menu.SelectedValue)
            {
                case "settings":
                    multiView.SetActiveView(viewSettings);
                    break;

                case "instances":
                    LoadInstances();
                    multiView.SetActiveView(viewInstances);
                    break;
            }
        }

        protected void LoadInstances()
        {
            try
            {
                lvInstances.DataSource = null;
                if (plugin != null)
                {
                    if (dc == null)
                    {
                        dc = new scms.data.ScmsDataContext();
                    }

                    var modules = from pm in dc.scms_plugin_modules
                                  where pm.pluginAppId == plugin.id
                                  orderby pm.name
                                  select pm;
                    lvInstances.DataSource = modules;
                }
                lvInstances.DataBind();
            }
            catch (Exception ex)
            {
                scms.ScmsEvent.Raise(new scms.ScmsEvent("LoadInstances failed", this, ex));
            }
        }

        protected void viewInstances_ItemDataBound(object sender, ListViewItemEventArgs args)
        {
            if (args.Item.ItemType == ListViewItemType.DataItem)
            {
                Repeater rptPages = (Repeater)args.Item.FindControl("rptPages");
                Repeater rptTemplates = (Repeater)args.Item.FindControl("rptTemplates");

                Panel panelPages = (Panel)args.Item.FindControl("panelPages");
                Panel panelTemplates = (Panel)args.Item.FindControl("panelTemplates");

                ListViewDataItem dataItem = (ListViewDataItem)args.Item;
                scms.data.scms_plugin_module module = (scms.data.scms_plugin_module)dataItem.DataItem;

                if (dc == null)
                {
                    dc = new scms.data.ScmsDataContext();
                }
                bool bAnyPages = false;
                var pages = from pmi in dc.scms_plugin_module_instances
                            where pmi.pluginAppId == module.pluginAppId
                            where pmi.pluginModuleId == module.id
                            where pmi.deleted == false
                            join ppm in dc.scms_page_plugin_modules on pmi.id equals ppm.instanceId
                            where ppm.deleted == false
                            join p in dc.scms_pages on ppm.pageId equals p.id
                            where p.deleted == false
                            orderby p.url
                            select new { url = p.url, instanceId = ppm.id };
                if (pages.Count() > 0)
                {
                    bAnyPages = true;
                    rptPages.DataSource = pages;
                    rptPages.DataBind();
                }
                else
                {
                    panelPages.Visible = false;
                }

                bool bAnyTemplates = false;
                var templates = from pmi in dc.scms_plugin_module_instances
                                where pmi.pluginAppId == module.pluginAppId
                                where pmi.pluginModuleId == module.id
                                where pmi.deleted == false
                                join tpm in dc.scms_template_plugin_modules on pmi.id equals tpm.instanceId
                                where tpm.deleted == false
                                join t in dc.scms_templates on tpm.templateId equals t.id
                                where t.deleted == false
                                orderby t.name
                                select new { name = t.name, instanceId = tpm.id };
                if (templates.Count() > 0)
                {
                    bAnyTemplates = true;
                    rptTemplates.DataSource = templates;
                    rptTemplates.DataBind();
                }
                else
                {
                    panelTemplates.Visible = false;
                }

                if (!(bAnyTemplates || bAnyPages))
                {
                    Panel panelNone = (Panel)args.Item.FindControl("panelNone");
                    panelNone.Visible = true;
                }


                //there is an issue i think that the wrong site is showing up
            }
        }


    }
}
