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
	public partial class PageSettings : System.Web.UI.UserControl
	{
		public delegate void SettingsSavedDelegate();
		public SettingsSavedDelegate OnSaved = null;

		protected int? nPageId = null;
		public int? PageId
		{
			set 
			{ 
				nPageId = value;
				ViewState["nPageId"] = value;
			}
			get 
			{
				nPageId = (int?)ViewState["nPageId"];
				return nPageId; 
			}
		}

		protected bool bIsHomePage = false;
		public bool IsHomePage
		{
			set
			{
				bIsHomePage = value;
				ViewState["bIsHomePage"] = value;
			}
			get
			{
				object objIsHomePage = ViewState["bIsHomePage"];
				if (objIsHomePage != null)
				{
					bIsHomePage = (bool)objIsHomePage;
				}

				return bIsHomePage;
			}
		}

		protected void Page_Load(object sender, EventArgs e)
		{
			if (!IsPostBack)
			{
				DataBind();
			}
		}

		public override void DataBind()
		{
			base.DataBind();

      cblSecurityRoles.DataSource = Roles.GetAllRoles();
      cblSecurityRoles.DataBind();


			if (nPageId.HasValue)
			{
				global::scms.data.ScmsDataContext dc = new global::scms.data.ScmsDataContext();
				var page = (from p in dc.scms_pages
										where p.id == nPageId.Value
										where p.deleted == false
										select p).Single();

				var site = (from s in dc.scms_sites
										where s.id == page.siteid
										select s).Single();

				IsHomePage = (site.homePageId == nPageId.Value);

				ddlPageType.SelectedValue = page.type.ToString();

				parentPageSelector.SiteId = page.siteid;
				parentPageSelector.PageId = page.parentId;

				var templates = from t in dc.scms_templates
												 where t.siteId == page.siteid
												 where t.deleted == false
												 orderby t.name
												 select t;
				ddlTemplate.DataSource = templates;
				ddlTemplate.DataValueField = "id";
				ddlTemplate.DataTextField = "name";
				if (page.templateId.HasValue)
				{
					ddlTemplate.SelectedValue = page.templateId.Value.ToString();
				}
				ddlTemplate.DataBind();

				txtFragment.Text = page.fragment;
				txtLinkText.Text = page.linktext;
				checkVisible.Checked = page.visible;

				// view state override
				checkViewStateOverride.Checked = page.viewStateEnabled.HasValue;
				checkViewStateEnabled.Checked = false;
				if (page.viewStateEnabled.HasValue)
				{
					checkViewStateEnabled.Checked = page.viewStateEnabled.Value;
				}
				checkViewStateOverride_checkedChanged(null, null);

				checkIncludeInSitemap.Checked = page.sitemapInclude;
				txtSitemapLinkText.Text = page.sitemapLinkText;

        checkIncludeInSearch.Checked = page.searchInclude;

        // security
        checkSecurityInherit.Checked = page.securityInherit;
        if (!page.securityInherit)
        {
          checkSecurityLoginRequired.Checked = page.securityLoginRequired.HasValue && page.securityLoginRequired.Value;
          if (checkSecurityLoginRequired.Checked)
          {
            if (page.securityRestrictToRoles.HasValue && page.securityRestrictToRoles.Value)
            {
							var restrictedRoles = from pr in dc.scms_page_roles
                                      join ar in dc.aspnet_Roles on pr.RoleId equals ar.RoleId
                                      select ar.RoleName;
              foreach (var role in restrictedRoles)
              {
                ListItem liRole = cblSecurityRoles.Items.FindByValue(role);
                if (liRole != null)
                {
                    liRole.Selected = true;
                }
              }
            }
          }

          checkSecurityProtocolSecure.Checked = page.securityProtocolSecure.HasValue && page.securityProtocolSecure.Value;
          checkSecurityProtocolForce.Checked = !checkSecurityProtocolSecure.Checked && page.securityProtocolForce.HasValue && page.securityProtocolForce.Value;
        }
                
                
                

				// seo
				txtTitle.Text = page.title;
				txtKeywords.Text = page.keywords;
				txtDescription.Text = page.description;
				pageSelectorCanonical.SiteId = page.siteid;
				if (page.canonicalPageId.HasValue)
				{
					btnRadioCanonicalPage.Checked = true;
					pageSelectorCanonical.PageId = page.canonicalPageId;
				}
				else
				{
					if (!string.IsNullOrEmpty(page.canonicalUrl))
					{
						btnRadioCanonicalUrl.Checked = true;
					}
					else
					{
						btnRadioCanonicalNone.Checked = true;
					}
				}

				checkIncludeInXmlSitemap.Checked = page.xmlSitemapInclude;
				if (page.xmlSitemapInclude)
				{
					txtXmlSitemapPriority.Text = (page.xmlSitemapPriority ?? 0.0m).ToString();
					if (!string.IsNullOrEmpty(page.xmlSitemapUpdateFrequency))
					{
						ListItem listItem = ddlXmlSitemapFrequency.Items.FindByValue(page.xmlSitemapUpdateFrequency);
						if (listItem == null)
						{
							listItem = new ListItem(page.xmlSitemapUpdateFrequency);
							ddlXmlSitemapFrequency.Items.Add(listItem);
						}
						listItem.Selected = true;
					}
				}

				txtSummary.Text = page.summary;
        selectImage.SiteId = page.siteid;
				selectImage.Path = page.thumbnail;
        string strAssociatedDate = null;
        if (page.associatedDate != null)
        {
            strAssociatedDate = page.associatedDate.Value.ToShortDateString();
        }
        txtAssociatedDate.Text = strAssociatedDate;

				pageSelectorRedirectPage.SiteId = page.siteid;
				if (page.redirectPageId.HasValue)
				{
					pageSelectorRedirectPage.PageId = page.redirectPageId.Value;
					btnRadioRedirectPage.Checked = true;
				}
				else
				{
					if (!string.IsNullOrEmpty(page.redirectUrl))
					{
						txtRedirectUrl.Text = page.redirectUrl;
						btnRadioRedirectUrl.Checked = true;
					}
					else
					{
						btnRadioRedirectPage.Checked = true;
					}
				}
				checkRedirectPermanent.Checked = page.redirectPermanent.HasValue ? page.redirectPermanent.Value : false;

				pageSelectorAlias.SiteId = page.siteid;
				pageSelectorAlias.PageId = page.aliasPageId;

				ddlPageType_SelectedIndexChanged(null, null);
				checkIncludeInSitemap_CheckChanged(null, null);
				radioCanonical_CheckedChanged(null, null);
				checkIncludeInXmlSitemap_CheckedChanged(null, null);

				radioRedirect_CheckChanged(null, null);

			}

            EnableSecurityControls();
		}

		protected void ddlPageType_SelectedIndexChanged(object sender, EventArgs args)
		{
			bool bShowNavigation = false;
			bool bShowTemplate = false;
			bool bShowSeo = false;
			bool bShowRedirect = false;
			bool bShowAlias = false;
			bool bShowInternal = false;
			bool bShowPanelSummary = false;

			switch (ddlPageType.SelectedValue.ToLower()[0])
			{
				case 'p':
					bShowNavigation = !IsHomePage;
					bShowTemplate = true;
					bShowSeo = true;
					bShowPanelSummary = true;
					break;

				case 'r':
					bShowNavigation = !IsHomePage;
                    bShowSeo = true;
					bShowRedirect = true;
					break;

				case 'a':
					bShowNavigation = !IsHomePage;
                    bShowSeo = true;
					bShowAlias = true;
					break;

				case 'i':
					bShowNavigation = !IsHomePage;
					bShowSeo = true;
					bShowPanelSummary = true;
					bShowInternal = true;
					break;
			}

			placeholderNavigation.Visible = bShowNavigation;
			panelSeo.Visible = bShowSeo;
			panelSummary.Visible = bShowPanelSummary;
			divTemplate.Visible = bShowTemplate;
			panelRedirect.Visible = bShowRedirect;
			panelAlias.Visible = bShowAlias;
			panelInternal.Visible = bShowInternal;
		}

		protected void btnEditFragment_Click(object sender, EventArgs args)
		{
			txtFragment.Enabled = true;
		}

		protected void radioCanonical_CheckedChanged(object sender, EventArgs args)
		{
			bool bEnablePageItems = false;
			bool bEnableUrlItems = false;

			bEnablePageItems = btnRadioCanonicalPage.Checked;
			bEnableUrlItems = btnRadioCanonicalUrl.Checked;

			pageSelectorCanonical.Enabled = bEnablePageItems;
			txtCanonicalUrl.Enabled = bEnableUrlItems;
		}

		protected void checkViewStateOverride_checkedChanged(object sender, EventArgs args)
		{
			bool bEnable = checkViewStateOverride.Checked;
			checkViewStateEnabled.Enabled = bEnable;
		}

		protected void checkIncludeInXmlSitemap_CheckedChanged(object sender, EventArgs args)
		{
			bool bEnableControls = checkIncludeInXmlSitemap.Checked;
			txtXmlSitemapPriority.Enabled = bEnableControls;
			ddlXmlSitemapFrequency.Enabled = bEnableControls;
		}

		protected void checkIncludeInSitemap_CheckChanged(object sender, EventArgs args)
		{
			txtSitemapLinkText.Enabled = checkIncludeInSitemap.Checked;
		}

		protected void radioRedirect_CheckChanged(object sender, EventArgs args)
		{
			bool bRedirectPageEnabled = false;
			bool bRedirectUrlEnabled = false;

			if (btnRadioRedirectPage.Checked)
			{
				bRedirectPageEnabled = true;
			}

			if (btnRadioRedirectUrl.Checked)
			{
				bRedirectUrlEnabled = true;
			}

			pageSelectorRedirectPage.Enabled = bRedirectPageEnabled;
			txtRedirectUrl.Enabled = bRedirectUrlEnabled;
		}

		protected void btnSave_Click(object sender, EventArgs args)
		{
			bool bValidateNavigation = false;
			bool bValidateTemplate = false;
			bool bValidateSeo = false;
			bool bValidateSummary = false;
			bool bValidateRedirect = false;
			bool bValidateAlias = false;
			bool bValidateInternal = false;

			switch (ddlPageType.SelectedValue.ToLower()[0])
			{
				case 'p':
					bValidateNavigation = !IsHomePage;
					bValidateSeo = true;
					bValidateSummary = true;
					break;

				case 'r':
					bValidateNavigation = !IsHomePage;
					bValidateRedirect = true;
					break;

				case 'a':
					bValidateNavigation = !IsHomePage;
					bValidateAlias = true;
					break;

				case 'i':
					bValidateNavigation = !IsHomePage;
					bValidateSeo = true;
					bValidateSummary = true;
					bValidateInternal = true;
					break;
			}

			if (bValidateNavigation)
			{
				Page.Validate("navigation");
			}


			if (bValidateTemplate)
			{
				Page.Validate("template");
			}

			if( bValidateSeo)
			{
				Page.Validate("seo");
			}

			if (bValidateSummary)
			{
				Page.Validate("summary");
			}

			if (bValidateRedirect)
			{
				Page.Validate("redirect");
			}

			if (bValidateAlias)
			{
				Page.Validate("alias");
			}

			if (bValidateInternal)
			{
				Page.Validate("internal");
			}

			if (Page.IsValid)
			{
				try
				{
					bool bPathRecalc = false;

					global::scms.data.ScmsDataContext dc = new global::scms.data.ScmsDataContext();
					global::scms.data.scms_page page = null;

					page = (from p in dc.scms_pages
									where p.id == PageId.Value
									where p.deleted == false
									select p).Single();

					page.type = ddlPageType.SelectedValue[0];

					page.templateId = int.Parse(ddlTemplate.SelectedValue);
					var template = (from t in dc.scms_templates
													where t.deleted == false
													where t.id == page.templateId
													select t).Single();
					page.masterId = template.masterId;

					if (!IsHomePage)
					{
						if (page.parentId.Value != parentPageSelector.PageId.Value)
						{
							page.parentId = parentPageSelector.PageId;

							// reorder new sibling pages
							var pageSiblings = (from p in dc.scms_pages
																	where p.parentId == page.parentId
																	where p.deleted == false
																	where p.id != page.id
																	orderby p.ordinal
																	select p);
							
							int nOrdinal = 0;
							foreach (var pageSibling in pageSiblings)
							{
								pageSibling.ordinal = nOrdinal;
								nOrdinal++;
							}
							page.ordinal = nOrdinal;

							bPathRecalc = true;
						}
					}

					page.linktext = txtLinkText.Text.Trim();
					if (string.Compare(page.fragment, txtFragment.Text.Trim()) != 0)
					{
						page.fragment = txtFragment.Text.Trim();
						bPathRecalc = true;
					}
					page.visible = checkVisible.Checked;

					page.viewStateEnabled = null;
					if (checkViewStateOverride.Checked)
					{
						page.viewStateEnabled = checkViewStateEnabled.Checked;
					}


          // security
          page.securityInherit = checkSecurityInherit.Checked;
          System.Collections.Generic.List<string> lRoles = new System.Collections.Generic.List<string>();
          if (page.securityInherit)
          {
              page.securityLoginRequired = null;
              page.securityRestrictToRoles = null;
              page.securityProtocolSecure = null;
              page.securityProtocolForce = null;
          }
          else
          {
              page.securityLoginRequired = checkSecurityLoginRequired.Checked;

              if (page.securityLoginRequired.HasValue && page.securityLoginRequired.Value)
              {
                  foreach ( ListItem li in cblSecurityRoles.Items)
                  {
                      if (li.Selected)
                      {
                          lRoles.Add(li.Value);
                      }
                  }
              }

              page.securityRestrictToRoles = lRoles.Count > 0;

              page.securityProtocolSecure = checkSecurityProtocolSecure.Checked;
              page.securityProtocolForce = !checkSecurityProtocolSecure.Checked && checkSecurityProtocolForce.Checked;
          }

          // find current roles
          var rolesCurrent = from ur1 in dc.scms_page_roles
                             join ar1 in dc.aspnet_Roles on ur1.RoleId equals ar1.RoleId
                             select new { ur1, ar1 };
          // remove any from db if they are not now selected
          // remove from list if they are
          foreach (var roleCurrent in rolesCurrent)
          {
              string strRole = roleCurrent.ar1.RoleName;
              if (lRoles.Contains(strRole))
              {
                  lRoles.Remove(strRole);
              }
              else
              {
                  dc.scms_page_roles.DeleteOnSubmit(roleCurrent.ur1);
              }
          }

          // add any roles left in list
          // lookup role ids
          var ars = from ar in dc.aspnet_Roles
                   select ar;
          foreach (string strRole2 in lRoles)
          {
              scms.data.scms_page_role pr = new scms.data.scms_page_role();
              pr.pageid = nPageId.Value;

              pr.RoleId = ars.Where(ar2 => ar2.RoleName == strRole2).Select(ar3 => ar3.RoleId).Single();
              dc.scms_page_roles.InsertOnSubmit(pr);
          }



					// html sitemap
					page.sitemapInclude = checkIncludeInSitemap.Checked;
					if (page.sitemapInclude)
					{
						page.sitemapLinkText = txtSitemapLinkText.Text.Trim();
					}
					else
					{
						page.sitemapLinkText = null;
					}

                    page.searchInclude = checkIncludeInSearch.Checked;

					// seo
					page.title = txtTitle.Text.Trim();
					page.keywords = txtKeywords.Text.Trim();
					page.description = txtDescription.Text.Trim();
					if (btnRadioCanonicalNone.Checked)
					{
						page.canonicalPageId = null;
						page.canonicalUrl = null;
					}
					else
					{
						if (btnRadioCanonicalPage.Checked)
						{
							page.canonicalPageId = pageSelectorCanonical.PageId;
							page.canonicalUrl = null;
						}
						else
						{
							if (btnRadioCanonicalUrl.Checked)
							{
								page.canonicalPageId = null;
								page.canonicalUrl = txtCanonicalUrl.Text.Trim();
							}
							else
							{
								throw new Exception("unexpected no canonical radio selected");
							}
						}
					}
					if (checkIncludeInXmlSitemap.Checked)
					{
						page.xmlSitemapInclude = true;
						if (!string.IsNullOrEmpty(txtXmlSitemapPriority.Text))
						{
							page.xmlSitemapPriority = decimal.Parse(txtXmlSitemapPriority.Text);
							page.xmlSitemapUpdateFrequency = ddlXmlSitemapFrequency.SelectedValue;
						}
						else
						{
							page.xmlSitemapPriority = null;
						}
					}
					else
					{
						page.xmlSitemapInclude = false;
						page.xmlSitemapPriority = null;
						page.xmlSitemapUpdateFrequency = null;
					}

					// summary
					page.summary = txtSummary.Text.Trim();
                    page.thumbnail = selectImage.Path; //  txtThumbnail.Text.Trim();
                    page.associatedDate = null;
                    if (!string.IsNullOrEmpty(txtAssociatedDate.Text))
                    {
                        page.associatedDate = DateTime.Parse(txtAssociatedDate.Text);
                    }

					// redirect
					if (page.type == 'R')
					{
						page.redirectPageId = pageSelectorRedirectPage.PageId;
						if (string.IsNullOrEmpty(txtRedirectUrl.Text.Trim()))
						{
							page.redirectUrl = null;
						}
						else
						{
							page.redirectUrl = txtRedirectUrl.Text.Trim();
						}
						if (page.redirectPageId.HasValue || !string.IsNullOrEmpty(page.redirectUrl))
						{
							// if any redirect, check if permanent
							page.redirectPermanent = checkRedirectPermanent.Checked;
						}
						else
						{
							page.redirectPermanent = false;
						}
					}
					else
					{
						page.redirectPageId = null;
						page.redirectUrl = null;
						page.redirectPermanent = false;
					}


					// alias
					if (page.type == 'A')
					{
						page.aliasPageId = pageSelectorAlias.PageId;
					}
					else
					{
						page.aliasPageId = null;
					}

					// internal
					if (page.type == 'I')
					{
						page.internalUrl = txtInteralUrl.Text;
					}
					else
					{
						page.internalUrl = null;
					}


					if (bPathRecalc)
					{
						RecalculatePaths(page, dc);
					}
					dc.SubmitChanges();

					scms.search.search search = new scms.search.search();
					search.RebuildPageIndex(page.id);

					if (bPathRecalc)
					{
						Response.Redirect(Request.RawUrl, false);
					}
					else
					{
						statusMessage.ShowSuccess("Page update success.");
					}

					global::scms.CacheManager.Clear();
					if (OnSaved != null)
					{
						OnSaved();
					}
				}
				catch (Exception ex)
				{
                    string strMessage = "Failed updating page";
                    ScmsEvent.Raise(strMessage, this, ex);
				}
			}
		}


		public static void RecalculatePaths(global::scms.data.scms_page page, global::scms.data.ScmsDataContext dc)
		{
			string strParentUrl = "/";

			var parent = (from p in dc.scms_pages
										where p.id == page.parentId
										select p).SingleOrDefault();

			if (parent != null)
			{
				strParentUrl = parent.url;
			}
			if (!strParentUrl.EndsWith("/"))
			{
				strParentUrl += "/";
			}

			page.url = string.Format("{0}{1}", strParentUrl, page.fragment);

			RecalcChildPaths(page.id, page.url, dc);

		}

		public static void RecalcChildPaths(int nParentPageId, string strParentUrl, global::scms.data.ScmsDataContext dc)
		{
			if (!strParentUrl.EndsWith("/"))
			{
				strParentUrl += "/";
			}

			// get all pages with this parent pageid
			var pages = from p in dc.scms_pages
									where p.parentId == nParentPageId
									select p;
			foreach (global::scms.data.scms_page page in pages)
			{
				page.url = string.Format("{0}{1}", strParentUrl, page.fragment);
				RecalcChildPaths(page.id, page.url, dc);
			}
		}

		protected void cvRedirectPage_ServerValidate(object sender, System.Web.UI.WebControls.ServerValidateEventArgs args)
		{
			bool bValid = false;
			if( btnRadioRedirectPage.Checked )
			{
				if( pageSelectorRedirectPage.PageId.HasValue )
				{
					bValid = true;
				}
			}
			else
			{
				bValid = true;
			}

			args.IsValid = bValid;
		}

		protected void cvRedirectUrl_ServerValidate(object sender, System.Web.UI.WebControls.ServerValidateEventArgs args)
		{
			bool bValid = false;
			if( btnRadioRedirectUrl.Checked )
			{
				if( !string.IsNullOrEmpty(txtRedirectUrl.Text.Trim()))
				{
					bValid = true;
				}
			}
			else
			{
				bValid = true;
			}

			args.IsValid = bValid;
		}

		protected void cvCanonicalPage_ServerValidate(object sender, System.Web.UI.WebControls.ServerValidateEventArgs args)
		{
			bool bValid = false;
			if (btnRadioCanonicalPage.Checked)
			{
				if (pageSelectorCanonical.PageId.HasValue)
				{
					bValid = true;
				}
			}
			else
			{
				bValid = true;
			}

			args.IsValid = bValid;
		}

		protected void cvCanonicalUrl_ServerValidate(object sender, System.Web.UI.WebControls.ServerValidateEventArgs args)
		{
			bool bValid = false;
			if (btnRadioCanonicalUrl.Checked)
			{
				if (!string.IsNullOrEmpty(txtCanonicalUrl.Text.Trim()))
				{
					bValid = true;
				}
			}
			else
			{
				bValid = true;
			}

			args.IsValid = bValid;
		}

		protected void verifyPageSelectorNotSelf_ServerValidate(object sender, System.Web.UI.WebControls.ServerValidateEventArgs args)
		{
			bool bValid = false;
			if (string.IsNullOrEmpty(args.Value))
			{
				bValid = true;
			}
			else
			{
				nPageId = PageId;
				if (nPageId.HasValue)
				{
					int nSelectedPageId = int.Parse(args.Value);
					if (nSelectedPageId != nPageId.Value)
					{
						bValid = true;
					}
				}
			}

			args.IsValid = bValid;
		}

        protected void checkSecurityInherit_CheckedChanged(object sender, EventArgs args)
        {
            EnableSecurityControls();
        }

        protected void checkSecurityLoginRequired_CheckedChanged(object sender, EventArgs args)
        {
            EnableSecurityControls();
        }

        protected void checkSecurityProtocolSecure_CheckedChanged(object sender, EventArgs args)
        {
            EnableSecurityControls();
        }

        protected void EnableSecurityControls()
        {
            bool bEnableInherit = false;
            bool bEnableRequireLogin = false;
            bool bEnableRestrictToRoles = false;
            bool bEnableProtocolSecure = false;
            bool bEnableProtocolForce = false;

            if (!IsHomePage)
            {
                bEnableInherit = true;
            }

            if (!checkSecurityInherit.Checked)
            {
                bEnableRequireLogin = true;
                if (checkSecurityLoginRequired.Checked)
                {
                    bEnableRestrictToRoles = true;
                }

                bEnableProtocolSecure = true;
                if (!checkSecurityProtocolSecure.Checked)
                {
                    bEnableProtocolForce = true;
                }
            }

            checkSecurityInherit.Enabled = bEnableInherit;
            checkSecurityLoginRequired.Enabled = bEnableRequireLogin;
            cblSecurityRoles.Enabled = bEnableRestrictToRoles;
            checkSecurityProtocolSecure.Enabled = bEnableProtocolSecure;
            checkSecurityProtocolForce.Enabled = bEnableProtocolForce;

        }
	}
}