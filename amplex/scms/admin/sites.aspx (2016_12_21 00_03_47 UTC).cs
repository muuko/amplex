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
  public partial class sites : System.Web.UI.Page
  {
    protected void Page_Load(object sender, EventArgs e)
    {
      Page.Title = "Admin - Sites";
      siteDdl.OnSiteSelected += OnSiteSelected;

      if (!IsPostBack)
      {
        scms.admin.Admin master = (scms.admin.Admin)this.Master;
        master.NavType = scms.admin.Admin.ENavType.Sites;

        DataBind();
      }
    }

    public override void DataBind()
		{
      base.DataBind();

      try
			{
        int nSiteId = siteDdl.SiteId.Value;

        global::scms.data.ScmsDataContext dc = new global::scms.data.ScmsDataContext();
        var site = (from s in dc.scms_sites
                    where s.id == nSiteId
                    where s.deleted == false
                    select s).Single();

        txtName.Text = site.name;
        pageSelectorHomePage.SiteId = site.id;
        pageSelectorHomePage.PageId = site.homePageId;
        txtHostNameRegex.Text = site.hostNameRegex;
        txtCanonicalHostNameRegex.Text = site.canonicalHostName;


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

        if (site.cacheEnabled.HasValue && site.cacheEnabled.Value)
				{
          checkCacheEnabled.Checked = true;
          if( !string.IsNullOrEmpty(site.cacheControl))
          {
            ListItem li = ddlCacheControl.Items.FindByValue(site.cacheControl);
            if( li == null )
            {
              li = new ListItem(site.cacheControl, site.cacheControl );
              ddlCacheControl.Items.Add(li);
            }
            ddlCacheControl.ClearSelection();
            li.Selected = true;
          }

          if (site.cacheExpiresSeconds.HasValue)
          {
						txtCacheExpires.Text = site.cacheExpiresSeconds.ToString();
          }

          if (site.cacheMaxAgeSeconds.HasValue)
          {
						txtCacheMaxAge.Text = site.cacheMaxAgeSeconds.ToString();
          }
				}

        checkXmlSitemapEnabled.Checked = site.xmlSitemapEnabled;
        if (site.xmlSitemapEnabled)
        {
            txtXmlSitemapLocation.Text = site.xmlSitemapLocation;
        }
        else
        {
            txtXmlSitemapLocation.Text = null;
        }

				
				checkMobileRedirectEnabled.Checked = false;
				txtMobileRedirectUrl.Text = string.Empty;
				checkMobileRedirectAppendPath.Checked = false;
				checkMobileRedirectAppendQueryString.Checked = false;
				txtMobileToFullQueryString.Text = "full=1";
				var mobileRedirect = (from mr in dc.scms_mobileRedirects
															where mr.siteId == site.id
															select mr).FirstOrDefault();
				if (mobileRedirect != null)
				{
					checkMobileRedirectEnabled.Checked = mobileRedirect.enabled;
					if (checkMobileRedirectEnabled.Checked)
					{
						txtMobileRedirectUrl.Text = mobileRedirect.url;
						checkMobileRedirectAppendPath.Checked = mobileRedirect.appendPath;
						checkMobileRedirectAppendQueryString.Checked = mobileRedirect.appendQueryString;
						txtMobileToFullQueryString.Text = mobileRedirect.mobileToFullQueryString;
					}
				}

				EnableControls(null, null);

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

      protected void cvXmlSitemap_ServerValidate(object sender, ServerValidateEventArgs args)
      {
          bool bIsValid = false;

          if (checkXmlSitemapEnabled.Checked)
          {
              if (!string.IsNullOrEmpty(txtXmlSitemapLocation.Text))
              {
                  bIsValid = true;
              }
          }
          else
          {
              bIsValid = true;
          }

          args.IsValid = bIsValid;
      }

			protected void btnSave_Click(object sender, EventArgs args)
			{
				Page.Validate();
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
						site.homePageId = pageSelectorHomePage.PageId;
						site.hostNameRegex = txtHostNameRegex.Text.Trim();
						site.canonicalHostName = txtCanonicalHostNameRegex.Text.Trim();

						site.defaultTemplateId = int.Parse(ddlDefaultTemplate.SelectedValue);

						site.cacheEnabled = checkCacheEnabled.Checked;
						if (site.cacheEnabled.Value)
						{
							site.cacheControl = ddlCacheControl.SelectedValue;

							site.cacheExpiresSeconds = null;
							string strCacheExpiresSeconds = txtCacheExpires.Text;
							if (!string.IsNullOrEmpty(strCacheExpiresSeconds))
							{
								site.cacheExpiresSeconds = int.Parse(strCacheExpiresSeconds);
							}

							site.cacheMaxAgeSeconds = null;
							string strCacheMaxAgeSeconds = txtCacheMaxAge.Text;
							if (!string.IsNullOrEmpty(strCacheMaxAgeSeconds))
							{
								site.cacheMaxAgeSeconds = int.Parse(strCacheMaxAgeSeconds);
							}
						}

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
						var mobileRedirect = (from mr in dc.scms_mobileRedirects
																	where mr.siteId == site.id
																	select mr).FirstOrDefault();
						if (mobileRedirect == null)
						{
							mobileRedirect = new scms.data.scms_mobileRedirect();
							mobileRedirect.siteId = site.id;
							dc.scms_mobileRedirects.InsertOnSubmit(mobileRedirect);
						}

						mobileRedirect.enabled = checkMobileRedirectEnabled.Checked;
						if (mobileRedirect.enabled)
						{
							string strUrl = txtMobileRedirectUrl.Text.Trim();
							if (!strUrl.StartsWith("http", StringComparison.OrdinalIgnoreCase))
							{
								if (strUrl.StartsWith("/"))
								{
									strUrl = "http:/" + strUrl;
								}
								else
								{
									strUrl = "http://" + strUrl;
								}
								txtMobileRedirectUrl.Text = strUrl;
							}
							mobileRedirect.url = strUrl;
							mobileRedirect.appendPath = checkMobileRedirectAppendPath.Checked;
							mobileRedirect.appendQueryString = checkMobileRedirectAppendQueryString.Checked;
							mobileRedirect.mobileToFullQueryString = txtMobileToFullQueryString.Text;
						}
						
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


		protected void EnableControls(object sender, EventArgs args)
		{
			txtXmlSitemapLocation.Enabled = checkXmlSitemapEnabled.Checked;

			bool bCacheEnabled = checkCacheEnabled.Checked;
			ddlCacheControl.Enabled = bCacheEnabled;
			txtCacheExpires.Enabled = bCacheEnabled;
			cvCacheExpires.Enabled = bCacheEnabled;
			txtCacheMaxAge.Enabled = bCacheEnabled;
			cvCacheMaxAge.Enabled = bCacheEnabled;

			bool bMobileRedirectEnabled = checkMobileRedirectEnabled.Checked;
			txtMobileRedirectUrl.Enabled = bMobileRedirectEnabled;
			checkMobileRedirectAppendPath.Enabled = bMobileRedirectEnabled;
			checkMobileRedirectAppendQueryString.Enabled = bMobileRedirectEnabled;
			txtMobileToFullQueryString.Enabled = bMobileRedirectEnabled;
			rfvMobileRedirectUrl.Enabled = bMobileRedirectEnabled;
		}

    
  }
}
