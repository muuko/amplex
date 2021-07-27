using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;

namespace scms.admin.controls
{
    public partial class SiteDdl : System.Web.UI.UserControl
    {
        protected bool bUseSiteInSession = true;
        public bool UseSiteInSession
        {
            get { return bUseSiteInSession; }
            set { bUseSiteInSession = true; }
        }

        protected int? nSiteId = null;
        public int? SiteId
        {
            get
            {
                if (bUseSiteInSession)
                {
                    nSiteId = (int?)Session["sid"];
                }

                return nSiteId;
            }

            set
            {
                nSiteId = value;
                if (bUseSiteInSession)
                {
                    Session["sid"] = nSiteId;
                }
            }
        }

        public delegate void SiteSelected(int? nSiteId);
        public SiteSelected OnSiteSelected = null;

        protected void Page_Init(object sender, EventArgs e)
        {
            LoadSites();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (nSiteId.HasValue)
                {
                    if (string.Compare(nSiteId.Value.ToString(), ddlSite.SelectedValue, true) != 0)
                    {
                        ddlSite.SelectedValue = nSiteId.Value.ToString();
                    }
                }
            }
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

                    bool bDefault = false;
                    if (bUseSiteInSession && SiteId.HasValue)
                    {
                        if (site.site.id == SiteId.Value)
                        {
                            bDefault = true;
                        }
                    }
                    else
                    {

                        if (string.IsNullOrEmpty(site.site.hostNameRegex))
                        {
                            bDefault = true;
                        }
                        else
                        {
                            System.Text.RegularExpressions.Regex regex = new System.Text.RegularExpressions.Regex(site.site.hostNameRegex);
                            System.Text.RegularExpressions.Match match = regex.Match(Request.Url.Host);
                            if (match.Success)
                            {
                                bDefault = true;
                            }
                        }
                    }

                    if (bDefault)
                    {
                        ddlSite.ClearSelection();
                        SiteId = int.Parse(li.Value);
                        li.Selected = true;
                    }

                    ddlSite.Items.Add(li);
                }
            }
        }

        protected void ddlSite_SelectedIndexChanged(object sender, EventArgs args)
        {
            if (OnSiteSelected != null)
            {
                SiteId = int.Parse(ddlSite.SelectedValue);
                OnSiteSelected(SiteId);
            }
        }
    }
}