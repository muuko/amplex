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
	public partial class Pages : System.Web.UI.Page
	{
		public enum EViewMode
		{
			Pages,
			Modules,
			Settings
		}

		protected int? nPageId = null;
		EViewMode viewMode = EViewMode.Pages;
		protected global::scms.ScmsSiteMapProvider.PageNode pageNode = null;


		protected void Page_Load(object sender, EventArgs e)
		{
			Title = "Admin - Pages";

			string strError;
			Exception exError;


			scms.admin.Admin master = (scms.admin.Admin)this.Master;
			master.NavType = Admin.ENavType.Pages;

			string strPageId = Request.QueryString["pid"];
			if (!string.IsNullOrEmpty(strPageId))
			{
				int n;
				if (int.TryParse(strPageId, out n))
				{
					nPageId = n;
				}
			}

			
			string strViewMode = Request.QueryString["show"];
			if (!string.IsNullOrEmpty(strViewMode))
			{
				try
				{
					viewMode = (EViewMode)Enum.Parse(typeof(EViewMode), strViewMode, true);
				}
				catch
				{
				}
			}


			global::scms.ScmsSiteMapProvider siteMapProvider = new global::scms.ScmsSiteMapProvider();

			global::scms.ScmsSiteMapProvider.WebsitePages webSitePages;
			if (!siteMapProvider.GetWebSitePages(out webSitePages, out strError, out exError))
			{
				throw new Exception(string.Format("ScmsSiteMapProvider.WebsitePage failed, returned '{0}'.", strError), exError);
			}

			global::scms.ScmsSiteMapProvider.Site site;
			if (!webSitePages.TryGetValue( breadcrumbs.SiteId.Value, out site))
			{
				throw new Exception(string.Format("No website exists for current site id '{0}'.", breadcrumbs.SiteId.Value));
			}

			if (!nPageId.HasValue)
			{
				// determine home page
				nPageId = site.site.homePageId;
			}

			if (nPageId.HasValue)
			{
				if (!siteMapProvider.GetPageNode(nPageId.Value, out pageNode, out strError, out exError))
				{
					throw new Exception( string.Format("Failed getting page node for page id '{0}', error '{1}'.", nPageId, strError ), exError );
				}
			}

			if (pageNode != null)
			{
				string strPageUrl = pageNode.page.url;
				if (!string.IsNullOrEmpty(site.site.canonicalHostName))
				{
					strPageUrl = string.Format("http://{0}{1}", site.site.canonicalHostName, strPageUrl);
				}
				anchorView.HRef = strPageUrl;

				anchorNewModule.HRef = string.Format("/scms/admin/newmodule.aspx?sid={0}&pid={1}", pageNode.page.siteid, nPageId);

				breadcrumbs.PageId = pageNode.page.id;
				breadcrumbs.SiteId = pageNode.page.siteid;

				pageListChildren.PageId = pageNode.page.id;
				pageListChildren.SiteId = pageNode.page.siteid;

				pluginModuleInstances.SiteId = pageNode.page.siteid;
				pluginModuleInstances.PageId = pageNode.page.id;
			}

			if (!IsPostBack)
			{
				pageSettings.PageId = nPageId;

				bool bDefaultShow = false;
				switch(viewMode)
				{
					case EViewMode.Pages:
						ShowChildren();
						breadcrumbs.viewMode = scms.admin.controls.AdminPagesBreadcrumbs.EViewMode.Pages;
						break;

					case EViewMode.Modules:
						ShowModules();
						breadcrumbs.viewMode = scms.admin.controls.AdminPagesBreadcrumbs.EViewMode.Modules;
						break;

					case EViewMode.Settings:
						ShowSettings();
						break;
				}
			}

			pageSettings.OnSaved += OnSettingsSaved;
		}


		protected void menuTabs_Click(object sender, MenuEventArgs args)
		{
			try
			{
				switch (menuTabs.SelectedValue.ToLower())
				{
					case "children":
						{
							if (viewMode != EViewMode.Pages)
							{
								string strUrl = string.Format("pages.aspx?pid={0}&show=pages", nPageId);
								Response.Redirect(strUrl, true);
							}
							else
							{
								ShowChildren();
							}
						}
						break;

					case "modules":
						if (viewMode != EViewMode.Modules)
						{
							string strUrl = string.Format("pages.aspx?pid={0}&show=modules", nPageId);
							Response.Redirect(strUrl, true);
						}
						else
						{
							ShowModules();
						}
						break;

					case "settings":
						ShowSettings();
						break;
				}
			}
			catch( System.Threading.ThreadAbortException )
			{
			}
		}

		protected void ShowChildren()
		{
			MenuItem menuItem = menuTabs.FindItem("Children");
			menuItem.Selected = true;
			multiView.SetActiveView(viewChildren);
		}

		protected void ShowModules()
		{
			MenuItem menuItem = menuTabs.FindItem("Modules");
			menuItem.Selected = true;
			multiView.SetActiveView(viewModules);
		}

		protected void ShowSettings()
		{
			MenuItem menuItem = menuTabs.FindItem("Settings");
			menuItem.Selected = true;
			multiView.SetActiveView(viewSettings);
		}

		protected void btnNewPage_Click(object sender, EventArgs args)
		{
			Page.Validate("new");
			if (Page.IsValid)
			{
				try
				{
					string strPageName = txtNewPageName.Text.Trim();

					global::scms.data.ScmsDataContext dc = new global::scms.data.ScmsDataContext();

					global::scms.data.scms_page page = new global::scms.data.scms_page();
					global::scms.data.scms_site site;
					site = (from s in dc.scms_sites
									where s.id == pageNode.page.siteid
									select s).Single();


					page.siteid = pageNode.page.siteid;
					page.parentId = nPageId;
                    page.securityInherit = true;
					string strFragment = null;
					bool bUniqueFragmentFound = false;
					int nTry = 1;
					do
					{
						if (nTry <= 1)
						{
							strFragment = CreateFragment(strPageName);
						}
						else
						{
							strFragment = CreateFragment( string.Format("{0}-{1}", strPageName, nTry));
						}
						var existingPage = (from ep in dc.scms_pages
																where ep.parentId == nPageId
																where ep.fragment == strFragment
																where ep.deleted == false
																select ep).FirstOrDefault();
						if (existingPage == null)
						{
							bUniqueFragmentFound = true;
							if (nTry > 1)
							{
								strPageName = string.Format("{0}-{1}", strPageName, nTry);
							}
						}
						nTry++;
					}
					while (!bUniqueFragmentFound);

					page.fragment = strFragment;
					page.linktext = strPageName;
					page.title = strPageName;
					page.lastUpdated = DateTime.Now;

					string strParentUrl = pageNode.page.url;
					if (!strParentUrl.EndsWith("/"))
					{
						strParentUrl += "/";
					}
					page.url = string.Format("{0}{1}", strParentUrl, strFragment);
					page.type = ddlNewPageType.SelectedValue[0];

					// determine ordinal
					int nOrdinal;
					var maxOrdinal = (from pmax in dc.scms_pages
														where pmax.parentId == nPageId.Value
														where pmax.deleted == false
														orderby pmax.ordinal descending
														select new { ordinal = pmax.ordinal }).FirstOrDefault();
					if (maxOrdinal == null)
					{
						nOrdinal = 0;
					}
					else
					{
						nOrdinal = maxOrdinal.ordinal + 1;
					}
					page.ordinal = nOrdinal;
					page.visible = true;
                    page.searchInclude = true;
					page.deleted = false;
					page.sitemapInclude = true;
					page.xmlSitemapInclude = true;
					page.xmlSitemapPriority = 0.5M;
					page.xmlSitemapUpdateFrequency = "monthly";

					if (page.type == 'P')
					{
						page.templateId = site.defaultTemplateId;
						page.masterId = (from t in dc.scms_templates
														 where t.id == page.templateId
														 where t.deleted == false
														 select t.masterId).Single();
					}
					else
					{
						page.masterId = null;
						page.templateId = null;
					}


					dc.scms_pages.InsertOnSubmit(page);
					dc.SubmitChanges();
					global::scms.CacheManager.Clear();

					string strUrl = string.Format("/scms/admin/pages.aspx?pid={0}&show=settings&followup=newmodule", page.id);
					Response.Redirect(strUrl, true);
				}
				catch (System.Threading.ThreadAbortException)
				{
				}
			}
		}

		static public string CreateFragment(string strPageName)
		{
			string strFragment = strPageName.ToLower();

			System.Text.StringBuilder sbFragment = new System.Text.StringBuilder();
			foreach (char ch in strFragment)
			{
				if (Char.IsLetterOrDigit(ch))
				{
					sbFragment.Append(ch);
				}
				else
				{
					sbFragment.Append('-');
				}
			}

			strFragment = sbFragment.ToString();
			bool bAnyReplacements = false;
			do
			{
				bAnyReplacements = false;
				if (strFragment.Contains("--"))
				{
					strFragment = strFragment.Replace("--", "-");
					bAnyReplacements = true;
				}
			}
			while (bAnyReplacements);

			strFragment = strFragment.TrimEnd(new char[] { '-' });

			return strFragment;
		}

		protected void OnSettingsSaved()
		{
			string strFollowup = Request.QueryString["followup"];
			if (!string.IsNullOrEmpty(strFollowup))
			{
				if (string.Compare(strFollowup, "newmodule", true) == 0)
				{
					bool bFollowup = false;

					// only add new module to 'P' type page (rather than alias, interal, redirect)
					if (nPageId.HasValue)
					{
						scms.data.ScmsDataContext dc = new scms.data.ScmsDataContext();
						var page = (from p in dc.scms_pages
												where p.id == nPageId.Value
												where p.deleted == false
												select p).FirstOrDefault();
						if (page != null)
						{
							if (page.type == 'P')
							{
								bFollowup = true;
							}
						}
					}

					if (bFollowup)
					{
						string strUrl = string.Format("/scms/admin/newmodule.aspx?sid={0}&pid={1}", pageNode.page.siteid, pageNode.page.id);
						Response.Redirect(strUrl, false);
					}
				}
			}
		}
	}
}
