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
    public partial class TemplatePage : System.Web.UI.Page
    {
        int? nSiteId = null;
        int? nTemplateId = null;
        global::scms.Template template = null;

        protected void Page_Load(object sender, EventArgs e)
        {
            siteDdl.OnSiteSelected += SiteSelected;

            Page.Title = "Admin - Template";

            scms.admin.Admin master = (scms.admin.Admin)this.Master;
            master.NavType = Admin.ENavType.Templates;

            int n;

            string strSiteId = Request.QueryString["sid"];
            if (int.TryParse(strSiteId, out n))
            {
                nSiteId = n;
            }

            string strTemplateId = Request.QueryString["tid"];
            if (int.TryParse(strTemplateId, out n))
            {
                nTemplateId = n;
            }


            LoadTemplate();

            literalMessage.Text = null;

            pluginModuleInstances.SiteId = nSiteId;
            pluginModuleInstances.TemplateId = nTemplateId;


            if (!IsPostBack)
            {

                literalTemplateName.Text = template.ScmsTemplate.name;


                anchorNewModule.HRef = string.Format("/scms/admin/newmodule.aspx?sid={0}&tid={1}", nSiteId, nTemplateId);

                bool bDefaultShow = true;
                string strShow = Request.QueryString["show"];
                if (!string.IsNullOrEmpty(strShow))
                {
                    if (string.Compare(strShow, "modules", true) == 0)
                    {
                        ShowModules();
                        bDefaultShow = false;
                    }
                }

                if (bDefaultShow)
                {
                    ShowModules();
                }
            }
        }

        protected void LoadTemplate()
        {
            template = null;
            if (nSiteId.HasValue && nTemplateId.HasValue)
            {
                string strError = null;
                Exception exError = null;
                if (!global::scms.Templates.GetTemplate(nSiteId.Value, nTemplateId.Value, out template, out strError, out exError))
                {
                    throw new Exception(string.Format("Failed getting template: '{0}'", strError, exError));
                }
            }
        }

        protected void menuTabs_Click(object sender, MenuEventArgs args)
        {
            switch (menuTabs.SelectedValue.ToLower())
            {
                case "settings":
                    ShowSettings();
                    break;

                case "modules":
                    ShowModules();
                    break;
            }
        }

        protected void ShowSettings()
        {
            if (nSiteId.HasValue && nTemplateId.HasValue && (template != null))
            {
                string strError;
                Exception exError;

                if (!LoadMasterPages(nSiteId.Value, out strError, out exError))
                {
                    throw new Exception(strError, exError);
                }

                LoadSettings();
            }
            else
            {
                literalMessage.Text = "Site or Template missing";
            }

            // todo set menu active?
            multiView.SetActiveView(viewSettings);
        }

        protected void ShowModules()
        {
            multiView.SetActiveView(viewModules);
        }

        protected bool LoadMasterPages(int nSiteId, out string strError, out Exception exError)
        {
            bool bSuccess = false;

            strError = null;
            exError = null;

            try
            {
                global::scms.data.ScmsDataContext dc = new global::scms.data.ScmsDataContext();
                var master = from m in dc.scms_masters
                             where m.siteId == nSiteId
                             where m.deleted == false
                             select m;
                ddlMasterPage.DataSource = master;
                ddlMasterPage.DataTextField = "name";
                ddlMasterPage.DataValueField = "id";
                ddlMasterPage.DataBind();

                bSuccess = true;
            }
            catch (Exception ex)
            {
                strError = "Exception thrown while loading master";
                exError = ex;
            }

            return bSuccess;
        }

        protected void LoadSettings()
        {

            literalId.Text = template.ScmsTemplate.id.ToString();
            txtName.Text = template.ScmsTemplate.name;
            ddlMasterPage.SelectedValue = template.ScmsTemplate.scms_master.id.ToString();

        }

        protected void bntSave_Click(object sender, EventArgs args)
        {
            try
            {
                global::scms.data.ScmsDataContext dc = new global::scms.data.ScmsDataContext();
                global::scms.data.scms_template template = (from t in dc.scms_templates
                                                            where t.siteId == nSiteId.Value
                                                            where t.id == nTemplateId
                                                            where t.deleted == false
                                                            select t).Single();
                template.name = txtName.Text.Trim();
                template.masterId = int.Parse(ddlMasterPage.SelectedValue);

                dc.SubmitChanges();
                global::scms.CacheManager.Clear();
                LoadTemplate();
                ShowSettings();

                literalMessage.Text = "Template Updated";
            }
            catch (Exception ex)
            {
                throw new Exception("Exception thrown in btnSave_Click", ex);
            }
        }

        protected void SiteSelected(int? nSiteId)
        {
            try
            {
                Response.Redirect(ResolveUrl("~/scms/admin/templates.aspx"), true);
            }
            catch (System.Threading.ThreadAbortException)
            {
            }
        }

    }
}
