using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace amplex.sites.failsourcing._controls
{
    public partial class MainNavItem : global::scms.modules.navigation.ViewSubnavControl
    {
        public string Path
        {
            get;
            set;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
					ShowRoot = false;
            /* if (!IsPostBack)*/
            {
                /*
                this.nMaxDepth = subnav.maxDepth;
                this.ShowChildren = subnav.showChildren;
                this.cssClassActive = subnav.cssClassActive;
                this.PinNavigationToHomePage = subnav.pinNavigationToHomePage;
                this.PinDepth = subnav.pinDepth;
                this.FloatingShowSiblingsIfNoChildren = subnav.showSiblingsIfNoChildren;
                */

                /*
                this.nMaxDepth = 2;
                this.PinDepth = 1;
                this.ShowChildren = true;
                this.PinNavigationToHomePage = false;
                */


                this.cssClassActive = "active";
                this.FloatingShowSiblingsIfNoChildren = false;


                global::scms.RootPage rootPage = this.Page as global::scms.RootPage;
                if (rootPage != null)
                {
                    global::scms.ScmsSiteMapProvider provider = new global::scms.ScmsSiteMapProvider();
                    SiteMapNode siteMapNode = provider.FindSiteMapNode(Path);
                    global::scms.ScmsSiteMapProvider.PageNode pageNode = null;
                    string strError;
                    Exception exError;
                    if (provider.GetPageNode(siteMapNode, out pageNode, out strError, out exError))
                    {
                        this.PageId = pageNode.page.id;
                        this.SiteId = pageNode.page.siteid;

                        string strSubnav;
                        if (LoadSubnav(out strSubnav, out strError, out exError))
                        {
                            System.Web.UI.WebControls.Literal literal = new Literal();
                            this.Controls.Add(literal);
                            literal.Text = string.Format("{0}", strSubnav);
                        }
                        else
                        {
                            // todo log this
                            throw new Exception(string.Format("Failed loading subnav: '{0}'.", strError, exError));
                        }
                    }
                }
            }
        }
    }
}