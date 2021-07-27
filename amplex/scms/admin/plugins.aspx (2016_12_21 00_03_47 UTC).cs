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
    public partial class plugins : System.Web.UI.Page
    {
        scms.data.ScmsDataContext dc = null;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                scms.admin.Admin master = (scms.admin.Admin)this.Master;
                master.NavType = scms.admin.Admin.ENavType.Plugins;

                LoadPlugins();
            }
        }

        protected void LoadPlugins()
        {
            try
            {
                if (dc == null)
                {
                    dc = new scms.data.ScmsDataContext();
                }
                var plugins = from p in dc.scms_plugin_applications
                              orderby p.name
                              select p;
                rptPlugins.DataSource = plugins;
                rptPlugins.DataBind();
            }
            catch (Exception ex)
            {
                string strMessage = string.Format("Failed loading plugins");
                ScmsEvent.Raise(new ScmsEvent(strMessage, this, ex));
            }

        }

        protected void rptPlugins_ItemDataBound(object sender, RepeaterItemEventArgs args)
        {
            try
            {
                scms.data.scms_plugin_application plugin = (scms.data.scms_plugin_application)args.Item.DataItem;
                if (dc == null)
                {
                    dc = new scms.data.ScmsDataContext();
                }
                var pluginModules = from pm in dc.scms_plugin_modules
                                    where pm.pluginAppId == plugin.id
                                    orderby pm.name
                                    select pm;

                Repeater rptPluginModules = (Repeater)args.Item.FindControl("rptModules");
                rptPluginModules.DataSource = pluginModules;
                rptPluginModules.DataBind();
            }
            catch (Exception ex)
            {
                string strMessage = string.Format("rptPlugins_ItemDataBound, exception thrown");
                ScmsEvent.Raise(new ScmsEvent(strMessage, this, ex));
            }


        }
    }
}
