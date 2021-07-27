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


namespace amplex.sites.amplex._masters.controls
{
    public partial class mainnav : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
            }
        }

        protected void PopulateChildLinks(global::scms.ScmsSiteMapProvider sitemapProvider, MenuItem menuItem)
        {
            string strUrl = menuItem.NavigateUrl;
            SiteMapNode node = sitemapProvider.FindSiteMapNode(strUrl);
            if (node != null)
            {
                foreach (SiteMapNode nodeChild in node.ChildNodes)
                {
                    MenuItem menuItemChild = new MenuItem(nodeChild.Title, null, null, nodeChild.Url);
                    menuItem.ChildItems.Add(menuItemChild);
                    PopulateChildLinks(sitemapProvider, menuItemChild);
                }
            }
        }
    }
}