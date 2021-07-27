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
    public partial class files : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Page.Title = "Admin - Files";
            siteDdl.OnSiteSelected += OnSiteSelected;

            if (!IsPostBack)
            {
                scms.admin.Admin master = (scms.admin.Admin)this.Master;
                master.NavType = scms.admin.Admin.ENavType.Files;

                DataBind();
            }
        }

        public override void DataBind()
        {
            base.DataBind();

            try
            {
                int nSiteId = siteDdl.SiteId.Value;

                // get root files directory for this site
                global::scms.data.ScmsDataContext dc = new global::scms.data.ScmsDataContext();
                var site = (from s in dc.scms_sites
                            where s.id == nSiteId
                            where s.deleted == false
                            select s).Single();


                string strFilesLocation = site.filesLocation;
                fileManager.FilesLocation = strFilesLocation;

                /*
                                txtName.Text = site.name;
                                pageSelectorHomePage.SiteId = site.id;
                                pageSelectorHomePage.PageId = site.homePageId;
                                txtHostNameRegex.Text = site.hostNameRegex;
				
				
                                var templates = from t in dc.scms_templates
                                                                where t.siteId == nSiteId
                                                                where t.deleted == false
                                                                orderby t.name
                                                                select t;
                                ddlDefaultTemplate.DataSource = templates;
                                ddlDefaultTemplate.DataTextField = "name";
                                ddlDefaultTemplate.DataValueField = "id";
                                ddlDefaultTemplate.DataBind();
                                ddlDefaultTemplate.SelectedValue = site.defaultTemplateId.ToString();

                                checkXmlSitemapEnabled.Checked = site.xmlSitemapEnabled;
                                if (site.xmlSitemapEnabled)
                                {
                                    txtXmlSitemapLocation.Text = site.xmlSitemapLocation;
                                }
                                else
                                {
                                    txtXmlSitemapLocation.Text = null;
                                }
                                checkXmlSitemapEnabled_checkChanged(null, null);
                */
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("Exception thrown in DataBind: '{0}'.", ex.ToString(), ex));
            }
        }


        protected void OnSiteSelected(int? nSiteId)
        {
            DataBind();
        }




        /*
        protected void btnSave_Click(object sender, EventArgs args)
        {
            if (Page.IsValid)
            {
                try
                {
                    int nSiteId = siteDdl.SiteId.Value;

                    global::scms.data.ScmsDataContext dc = new global::scms.data.ScmsDataContext();
                    var site = (from s in dc.scms_sites
                                            where s.id == nSiteId
                                            where s.deleted == false
                                            select s).Single();

                    site.name = txtName.Text.Trim();
                    site.id = pageSelectorHomePage.SiteId.Value;
                    site.homePageId = pageSelectorHomePage.PageId;
                    site.hostNameRegex = txtHostNameRegex.Text.Trim();

                    site.defaultTemplateId = int.Parse(ddlDefaultTemplate.SelectedValue);
                    site.xmlSitemapEnabled = checkXmlSitemapEnabled.Checked;
                    string strXmlSitemapLocation = null;
                    if (site.xmlSitemapEnabled)
                    {
                        strXmlSitemapLocation = txtXmlSitemapLocation.Text.Trim();
                        if (!strXmlSitemapLocation.StartsWith("/"))
                        {
                            strXmlSitemapLocation = "/" + strXmlSitemapLocation;
                        }
						
                    }
                    site.xmlSitemapLocation = strXmlSitemapLocation;

                    dc.SubmitChanges();
                    global::scms.CacheManager.Clear();
                    statusMessage.ShowSuccess("Site updated");
                }
                catch (Exception ex)
                {
                    string strMessage = string.Format("Failed saving site info, error: '{0}'.", ex.ToString());
                    statusMessage.ShowFailure(strMessage);
                }
            }
        }
        */
    }
}
