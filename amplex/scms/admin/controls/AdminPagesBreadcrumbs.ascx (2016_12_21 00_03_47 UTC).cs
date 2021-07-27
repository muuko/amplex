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
	public partial class AdminPagesBreadcrumbs : System.Web.UI.UserControl
	{
		public enum EViewMode
		{
			Pages,
			Modules
		}

		public EViewMode viewMode = EViewMode.Pages;


		protected int? nPageId = null;
		public int ? PageId
		{
			get { return nPageId; }
			set { nPageId = value; }
		}

		public int? SiteId
		{
			get  {return siteDdl.SiteId; }
			set { siteDdl.SiteId = value; }
		}

		protected void Page_Load(object sender, EventArgs e)
		{
			siteDdl.OnSiteSelected += OnSiteSelected;
			if (!IsPostBack)
			{
				LoadBreadCrumbs();
			}
		}

		protected void LoadBreadCrumbs()
		{
			// base.DataBind();
			string strError;
			Exception exError;
			global::scms.ScmsSiteMapProvider siteMapProvider = new global::scms.ScmsSiteMapProvider();
			System.Collections.Generic.List<global::scms.ScmsSiteMapProvider.PageNode> lPageNodes;
			if (siteMapProvider.GetPath(nPageId, out lPageNodes, out strError, out exError))
			{
				rptPageBreadCrumbs.DataSource = lPageNodes;
			}
			else
			{
				// todo error
				rptPageBreadCrumbs.DataSource = null;

			}
			rptPageBreadCrumbs.DataBind();
		}

		// protected global::scms.admin.controls.SiteDdl.SiteSelected
		protected void OnSiteSelected(int? nSiteId)
		{
			try
			{
				Response.Redirect("~/scms/admin/pages.aspx", false);
			}
			catch (System.Threading.ThreadAbortException)
			{
			}
		}

		protected void rptPageBreadCrumbs_ItemDataBound(object sender, RepeaterItemEventArgs args)
		{
			HtmlAnchor anchorPage = (HtmlAnchor)args.Item.FindControl("anchorPage");
			if (anchorPage != null)
			{
				global::scms.ScmsSiteMapProvider.PageNode pageNode = (global::scms.ScmsSiteMapProvider.PageNode)args.Item.DataItem;

				string strShow = null;
				if (PageId.HasValue && (PageId.Value == pageNode.page.id) )
				{
					strShow = string.Format("&show={0}", viewMode.ToString());
				}

				string strUrl = ResolveUrl(string.Format("~/scms/admin/pages.aspx?pid={0}{1}", pageNode.page.id, strShow));
				anchorPage.HRef = strUrl;
				string strText = pageNode.page.fragment;
				if (string.IsNullOrEmpty(strText))
				{
					strText = "[home]";
				}

				anchorPage.InnerText = strText;
			}
		}
	}
}