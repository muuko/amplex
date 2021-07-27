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
	public partial class User : System.Web.UI.Page
	{
		// protected int? nPageId = null;
		// protected global::scms.ScmsSiteMapProvider.PageNode pageNode = null;

		protected void Page_Init(object sender, EventArgs args)
		{
			LoadOrganizations();
		}

		protected void LoadOrganizations()
		{
			try
			{
				ddlOrganization.AppendDataBoundItems = true;
				ddlOrganization.Items.Add(new ListItem("(none)", ""));
				scms.data.ScmsDataContext dc = new scms.data.ScmsDataContext();
				var orgs = from o in dc.scms_orgs
									 where o.deleted == false
									 orderby o.name
									 select o;
				ddlOrganization.DataTextField = "name";
				ddlOrganization.DataValueField = "id";
				ddlOrganization.DataSource = orgs;
				ddlOrganization.DataBind();
			}
			catch (Exception ex)
			{
				string strMessage = "Exception thrown while loading organizations '{0}'";
				ScmsEvent.Raise(strMessage, this, ex);
			}
		}

		protected void Page_Load(object sender, EventArgs e)
		{
			Title = "Admin - Security - User";

			string strError;
			Exception exError;

			scms.admin.Admin master = (scms.admin.Admin)this.Master;
			master.NavType = Admin.ENavType.Security;

      string strUserName = Request.QueryString["u"];
      if (string.IsNullOrEmpty(strUserName))
      {
        Response.Redirect("/scms/admin/security.aspx");
      }

      if (!IsPostBack)
      {
        LoadUser(strUserName);
      }


            /*
			string strPageId = Request.QueryString["pid"];
			if (!string.IsNullOrEmpty(strPageId))
			{
				int n;
				if (int.TryParse(strPageId, out n))
				{
					nPageId = n;
				}
			}


			global::scms.ScmsSiteMapProvider siteMapProvider = new global::scms.ScmsSiteMapProvider();
			if (!nPageId.HasValue)
			{
				// determine home page
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
				anchorView.HRef = pageNode.page.url;

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

				bool bDefaultShow = true;
				string strShow = Request.QueryString["show"];
				if (!string.IsNullOrEmpty(strShow))
				{
					switch( strShow )
					{
						case "settings":
							ShowSettings();
							bDefaultShow = false;
							break;

						case "modules":
							ShowModules();
							bDefaultShow = false;
							break;
					}
				}

				if( bDefaultShow)
				{
					ShowChildren();
				}
			}

			pageSettings.OnSaved += OnSettingsSaved;
            */
		}

	protected bool LoadUser(string strUserName)
	{
		bool bSuccess = false;

		try
		{
			MembershipUser membershipUser = Membership.GetUser(strUserName);
			if (membershipUser != null)
			{
				literalUserName.Text = membershipUser.UserName;
				txtEmailAddress.Text = membershipUser.Email;

				Guid guidUser = (Guid)membershipUser.ProviderUserKey;
				global::scms.data.ScmsDataContext dc = new scms.data.ScmsDataContext();
				var user = (from u in dc.scms_users
										where u.userid == guidUser
										select u ).FirstOrDefault();
				if (user != null)
				{
					txtFirstName.Text = user.firstName;
					txtLastName.Text = user.lastName;
					ddlOrganization.SelectedValue = user.orgId.ToString();
				}

				cblRoles.Items.Clear();
				var roles = Roles.GetAllRoles();
				foreach (string strRole in roles)
				{
					ListItem item = new ListItem();
					item.Text = strRole;
					item.Value = strRole;
					item.Selected = Roles.IsUserInRole(strUserName, strRole);
					cblRoles.Items.Add(item);
				}
				cblRoles.DataBind();

				string strPassword = Request.QueryString["p"];
				if (!string.IsNullOrEmpty(strPassword))
				{
					btnSendWelcomeEmail.Visible = true;
					btnSendWelcomeEmail.Enabled = true;
				}

				bSuccess = true;
			}
			else
			{
				string strMessage = string.Format("Membershp user not found'{0}'", strUserName);
				ScmsEvent.Raise(strMessage, this, null);
			}
		}
		catch (Exception ex)
		{
			string strMessage = string.Format("Exception thrown while loading user '{0}'", strUserName);
			ScmsEvent.Raise(strMessage, this, ex);
		}

		return bSuccess;
	}

  protected void btnSave_Click(object sender, EventArgs args)
	{
    string strUserName = Request.QueryString["u"];
    try
	  {
      MembershipUser membershipUser = Membership.GetUser(strUserName);
      if (membershipUser == null)
      {
				throw new Exception( string.Format( "Unexpected membership user '{0}' does not exist.", strUserName ));
      }

      membershipUser.Email = txtEmailAddress.Text;
      Membership.UpdateUser(membershipUser);

      Guid guidUser = (Guid)membershipUser.ProviderUserKey;
      global::scms.data.ScmsDataContext dc = new scms.data.ScmsDataContext();
      global::scms.data.scms_user scmsUser = (from u in dc.scms_users
                                              where u.userid == guidUser
                                              select u).FirstOrDefault();
      if (scmsUser == null)
      {
        scmsUser = new scms.data.scms_user();
        scmsUser.userid = guidUser;
        dc.scms_users.InsertOnSubmit(scmsUser);
      }
      
      scmsUser.firstName = txtFirstName.Text.Trim();
      scmsUser.lastName = txtLastName.Text.Trim();
			string strOrganizationId = ddlOrganization.SelectedValue;
			if (string.IsNullOrEmpty(strOrganizationId))
			{
				scmsUser.orgId = null;
			}
			else
			{
				scmsUser.orgId = int.Parse(strOrganizationId);
			}

      dc.SubmitChanges();

      bool bThisUserAdminRemoved = false;

      foreach (ListItem liRole in cblRoles.Items)
      {
        if (liRole.Selected)
        {
          if (!Roles.IsUserInRole(strUserName, liRole.Value))
          {
						Roles.AddUserToRole(strUserName, liRole.Value);
          }
        }
        else
	      {
          if (Roles.IsUserInRole(strUserName, liRole.Value))
	        {
            bool bOkToRemove = false;
            if (string.Compare(liRole.Value, "administrator", true) == 0)
            {
              bOkToRemove = Roles.GetUsersInRole("Administrator").Length > 1;

              if( bOkToRemove )
              {
								bThisUserAdminRemoved = string.Compare(Membership.GetUser().UserName, strUserName, true) == 0;
              }
            }
            else
            {
							bOkToRemove = true;
            }

            if (bOkToRemove)
            {
							Roles.RemoveUserFromRole(strUserName, liRole.Value);
            }
            else
            {
							statusMessage.ShowFailure( string.Format( "Cannot remove final user '{0}' from Administrator role.", strUserName ));
            }
					}
				}
      }

      statusMessage.ShowSuccess("User updated");
      if (bThisUserAdminRemoved)
      {
          Response.Redirect(Request.RawUrl);
      }
		}
    catch (Exception ex)
    {
      string strMessage = string.Format( "Failed saving user '{0}'.", strUserName);
      statusMessage.ShowFailure(strMessage);
      ScmsEvent.Raise(strMessage, this, ex);
    }
  }

        protected void btnSetPassword_Click(object sender, EventArgs args)
        {
            if (Page.IsValid)
            {
                string strNewPassword = txtPassword.Text.Trim();
                if (!string.IsNullOrEmpty(strNewPassword))
                {
                    string strUserName = Request.QueryString["u"];
                    MembershipUser membershipUser = Membership.GetUser(strUserName);
                    string strPassword = membershipUser.ResetPassword();
                    membershipUser.ChangePassword(strPassword, strNewPassword);

                    statusMessageSetPassword.ShowSuccess("Password updated");
                }
            }
        }


    protected void custPassword_ServerValidate(object sender, ServerValidateEventArgs args)
    {
      bool bValid = false;

      string strPassword = txtPassword.Text.Trim();
      if (strPassword.Length >= 5)
      {
          bValid = true;
      }

      args.IsValid = bValid;
    }

		protected void btnSendWelcomeEmail_Click(object sender, EventArgs args)
		{
			string strUser = Request.QueryString["u"];
			string strPassword = Request.QueryString["p"];
			string strEmail = txtEmailAddress.Text.Trim();

			bool bContinue = true;
			MembershipUser membershipUser = Membership.GetUser(strUser);
			if (membershipUser == null)
			{
				statusMessage.ShowFailure(string.Format("failed loading user '{0}'", strUser));
				bContinue = false;
			}

			if (bContinue)
			{
				if (string.IsNullOrEmpty(strEmail))
				{
					statusMessage.ShowFailure("email address has not been set, please add user's email address and save before sending a welcome email");
					bContinue = false;
				}
			}

			if (bContinue)
			{
				if (string.Compare(strEmail, membershipUser.Email, true) != 0)
				{
					statusMessage.ShowFailure("please save this modified user before sending a welcome email");
					bContinue = false;
				}
			}

			if (bContinue)
			{
				SendWelcomeEmail(strUser, strPassword, strEmail);
			}
		}

		protected void SendWelcomeEmail(string strUserName, string strPassword, string strEmail)
		{
			bool? bSendWelcomeEmail = false;

			bool bContinue = true;
			if (!scms.Configuration.GetValue("welcome-email-send", false, out bSendWelcomeEmail))
			{
				ScmsEvent.Raise("not sending welcome email, 'welcome-email-send' not set or false", this, null);
				bContinue = false;
			}

			if( bContinue && bSendWelcomeEmail.HasValue && bSendWelcomeEmail.Value )
			{
				string strWelcomeEmailFrom = string.Empty;
				string strWelcomeEmailBcc = string.Empty;
				string strWelcomeEmailSubject = string.Empty;
				string strWelcomeEmailBody = string.Empty;


				if (bContinue)
				{
					strWelcomeEmailFrom = scms.Configuration.GetValue("welcome-email-from", false);
					if (string.IsNullOrEmpty(strWelcomeEmailFrom))
					{
						ScmsEvent.Raise("not sending welcome email, 'welcome-email-from' not set or empty", this, null);
						bContinue = false;
					}
				}

				if (bContinue)
				{
					strWelcomeEmailBcc = scms.Configuration.GetValue("welcome-email-bcc", false);
					if (string.IsNullOrEmpty(strWelcomeEmailBcc))
					{
						ScmsEvent.Raise("not sending welcome email, 'welcome-email-bcc' not set or empty", this, null);
						bContinue = false;
					}
				}

				if (bContinue)
				{
					strWelcomeEmailSubject = scms.Configuration.GetValue("welcome-email-subject", false);
					if (string.IsNullOrEmpty(strWelcomeEmailSubject))
					{
						ScmsEvent.Raise("not sending welcome email, 'welcome-email-subject' is not set or empty", this, null);
						bContinue = false;
					}
				}

				if (bContinue)
				{
					strWelcomeEmailBody = scms.Configuration.GetValue("welcome-email-body", false);
					if (string.IsNullOrEmpty(strWelcomeEmailBody))
					{
						ScmsEvent.Raise("not sending welcome email, 'welcome-email-body' is not set or empty", this, null);
						bContinue = false;
					}
				}

				if (bContinue)
				{
					try
					{
						System.Net.Mail.MailMessage message = new System.Net.Mail.MailMessage();
						message.From = new System.Net.Mail.MailAddress(strWelcomeEmailFrom);

						message.To.Add(strEmail);

						string [] astrWelcomeEmailBcc = strWelcomeEmailBcc.Split(new char[] { ',', ';', ' ' }, StringSplitOptions.RemoveEmptyEntries);
						foreach (string strEmailBcc in astrWelcomeEmailBcc)
						{
							message.Bcc.Add(strEmailBcc);
						}

						strWelcomeEmailSubject = WelcomeEmailSubstitute(strWelcomeEmailSubject, strUserName, strEmail, strPassword);
						message.Subject = strWelcomeEmailSubject;

						strWelcomeEmailBody = WelcomeEmailSubstitute(strWelcomeEmailBody, strUserName, strEmail, strPassword);
						message.Body = strWelcomeEmailBody;

						System.Net.Mail.SmtpClient client = new System.Net.Mail.SmtpClient();
						client.Send(message);

						statusMessage.ShowSuccess("welcome email sent successfully");
					}
					catch (Exception ex)
					{
						ScmsEvent.Raise("failed sending welcome email", this, ex);
						statusMessage.ShowFailure("failed sending welcome email, see event log for more information");
					}
				}
			}
		}

		protected string WelcomeEmailSubstitute(string strText, string strUser, string strEmail, string strPassword)
		{
			string strResult = strText;

			strResult = strResult.Replace("##USER##", strUser);
			strResult = strResult.Replace("##EMAIL##", strEmail);
			strResult = strResult.Replace("##PASSWORD##", strPassword);

			return strResult;
		}

        
	}
}

