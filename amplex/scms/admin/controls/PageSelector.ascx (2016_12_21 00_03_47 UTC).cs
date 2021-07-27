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
    [ValidationPropertyAttribute("PageId")]
    public partial class PageSelector : System.Web.UI.UserControl
    {
        public delegate void PageSelectionChangedDelegate(int? nPageId);
        public PageSelectionChangedDelegate OnPageSelectionChanged = null;

        public bool ShowSelectedPage
        {
            get
            {
                bool? bShow = (bool?)ViewState["ShowSelectedPage"];
                return !bShow.HasValue || bShow.Value;
            }

            set
            {
                ViewState["ShowSelectedPage"] = value;
            }
        }

        protected int? nSiteId = null;
        public int? SiteId
        {
            get
            {
                nSiteId = (int?)ViewState["SiteId"];
                return nSiteId;
            }
            set
            {
                ViewState["SiteId"] = value;
                nSiteId = value;
            }
        }

        protected int? nPageId = null;
        public int? PageId
        {
            get
            {
                nPageId = (int?)ViewState["PageId"];
                return nPageId;
            }
            set
            {
                ViewState["PageId"] = value;
                nPageId = value;

                txtPageId.Text = null;
                if (nPageId.HasValue)
                {
                    global::scms.ScmsSiteMapProvider siteMapProvider = new global::scms.ScmsSiteMapProvider();
                    string strError;
                    Exception exError;

                    global::scms.ScmsSiteMapProvider.PageNode pageNode;
                    if (siteMapProvider.GetPageNode(nPageId.Value, out pageNode, out strError, out exError))
                    {
                        txtPageId.Text = pageNode.page.linktext;
                    }
                }
            }
        }

        public bool Enabled
        {
            set
            {
                txtPageId.Enabled = value;
            }
        }

        protected void SelectPage(int? nPageId)
        {
            PageId = nPageId;
            if (SiteId.HasValue)
            {
                global::scms.ScmsSiteMapProvider siteMapProvider = new global::scms.ScmsSiteMapProvider();
                string strError;
                Exception exError;

                global::scms.ScmsSiteMapProvider.Site site;
                tvSelectPage.Nodes.Clear();

                if (siteMapProvider.GetSite(nSiteId.Value, out site, out strError, out exError))
                {
                    int nHomePageId = site.site.homePageId.Value;
                    global::scms.ScmsSiteMapProvider.PageNode pageNodeHomePage;
                    if (siteMapProvider.GetPageNode(nHomePageId, out pageNodeHomePage, out strError, out exError))
                    {
                        TreeNode treeNodeHomePage = new TreeNode(pageNodeHomePage.page.linktext, nHomePageId.ToString());
                        treeNodeHomePage.PopulateOnDemand = true;
                        treeNodeHomePage.SelectAction = TreeNodeSelectAction.Select;
                        tvSelectPage.Nodes.Add(treeNodeHomePage);
                    }
                }


                if (PageId.HasValue)
                {
                    global::scms.ScmsSiteMapProvider.PageNode pageNode;
                    if (siteMapProvider.GetPageNode(nPageId.Value, out pageNode, out strError, out exError))
                    {
                        txtPageId.Text = pageNode.page.linktext;
                    }
                }
            }
        }

        public void Inititialize()
        {
            btnPageId.Visible = !ShowSelectedPage;
            txtPageId.Visible = ShowSelectedPage;

            global::scms.ScmsSiteMapProvider siteMapProvider = new global::scms.ScmsSiteMapProvider();
            string strError;
            Exception exError;

            if (nSiteId.HasValue)
            {
                tvSelectPage.Nodes.Clear();

                global::scms.ScmsSiteMapProvider.Site site;

                if (siteMapProvider.GetSite(nSiteId.Value, out site, out strError, out exError))
                {
                    int nHomePageId = site.site.homePageId.Value;
                    global::scms.ScmsSiteMapProvider.PageNode pageNodeHomePage;
                    if (siteMapProvider.GetPageNode(nHomePageId, out pageNodeHomePage, out strError, out exError))
                    {
                        TreeNode treeNodeHomePage = new TreeNode(pageNodeHomePage.page.linktext, nHomePageId.ToString());
                        treeNodeHomePage.PopulateOnDemand = true;
                        treeNodeHomePage.SelectAction = TreeNodeSelectAction.Select;
                        tvSelectPage.Nodes.Add(treeNodeHomePage);
                    }
                }
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                Inititialize();
            }
        }

        protected void tvSelectPage_SelectedNodeChanged(object sender, EventArgs e)
        {
            //			txtPageId.Text = tvSelectPage.SelectedNode.Text;
            PageId = int.Parse(tvSelectPage.SelectedNode.Value);
            Inititialize();

            if (OnPageSelectionChanged != null)
            {
                OnPageSelectionChanged(PageId);
            }

        }

        protected void btnCancel_Click(object sender, EventArgs args)
        {
        }

        protected void btnClear_Click(object sender, EventArgs args)
        {
            //		txtPageId.Text = null;
            PageId = null;

            if (OnPageSelectionChanged != null)
            {
                OnPageSelectionChanged(PageId);
            }
        }

        protected void Node_Populate(object sender, System.Web.UI.WebControls.TreeNodeEventArgs e)
        {
            if (e.Node.ChildNodes.Count == 0)
            {
                string strError;
                Exception exError;
                int nPageId = int.Parse(e.Node.Value);
                global::scms.ScmsSiteMapProvider siteMapProvider = new global::scms.ScmsSiteMapProvider();
                global::scms.ScmsSiteMapProvider.PageNode pageNode;
                if (siteMapProvider.GetPageNode(nPageId, out pageNode, out strError, out exError))
                {
                    foreach (int nChildId in pageNode.children.Values)
                    {
                        global::scms.ScmsSiteMapProvider.PageNode pageNodeChild;
                        if (siteMapProvider.GetPageNode(nChildId, out pageNodeChild, out strError, out exError))
                        {
                            TreeNode nodeChild = new TreeNode(pageNodeChild.page.linktext, nChildId.ToString());
                            nodeChild.Text = pageNodeChild.page.linktext;
                            nodeChild.SelectAction = TreeNodeSelectAction.Select;
                            if (pageNodeChild.children.Count > 0)
                            {
                                nodeChild.PopulateOnDemand = true;
                            }
                            e.Node.ChildNodes.Add(nodeChild);
                        }
                    }
                }
            }
        }

        void FillAuthors(TreeNode node)
        {
            /*
        foreach (DataRow row in authors.Tables[0].Rows)

        {

        TreeNode newNode = new 

                     TreeNode(row["au_fname"].ToString() + " " + 

                     row["au_lname"].ToString(), 

                     row["au_id"].ToString());

        newNode.PopulateOnDemand = true;

        newNode.SelectAction = TreeNodeSelectAction.Expand;

        node.ChildNodes.Add(newNode);

        }
            */
        }


        /*
                void FillTitlesForAuthors(TreeNode node)

                {

                    string authorID = node.Value;

                    string connString = System.Configuration.ConfigurationSettings.

        ConnectionStrings["NorthwindConnnection"].ConnectionString;

                    SqlConnection connection = new SqlConnection(connString);

                    SqlCommand command = new SqlCommand("Select T.title, 

        T.title_id From titles T" +

        " Inner Join titleauthor TA on 

        T.title_id = TA.title_id " + 

                            " Where TA.au_id = '" + authorID + "'", connection);

                    SqlDataAdapter adapter = new SqlDataAdapter(command);

                    DataSet titlesForAuthors = new DataSet();

                    adapter.Fill(titlesForAuthors);

                    if (titlesForAuthors.Tables.Count > 0)

                    {

                        foreach (DataRow row in titlesForAuthors.Tables[0].Rows)

                        {

                            TreeNode newNode = new TreeNode(

        row["title"].ToString(), row["title_id"].ToString());

                            newNode.PopulateOnDemand = false;

                            newNode.SelectAction = TreeNodeSelectAction.None;

                            node.ChildNodes.Add(newNode);

                        }

                    }

                }
        */

    }
}