using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace scms.modules.security.login
{
	public partial class view : scms.RootControl
  {
    protected void Page_Load(object sender, EventArgs e)
    {
			if (!IsPostBack)
			{
				System.Web.UI.HtmlControls.HtmlForm form = (System.Web.UI.HtmlControls.HtmlForm)this.Page.Master.FindControl("form");
				form.Attributes.Add("data-ajax", "false");
			}

			bool bLoggedIn = Page.User.Identity.IsAuthenticated;
			placeHolderLoggedIn.Visible = bLoggedIn;
			placeHolderNotLoggedIn.Visible = !bLoggedIn;

			if (bLoggedIn)
			{
				literalUser.Text = Page.User.Identity.Name;
			}
			else
			{
				if (!IsPostBack )
				{
					txtUserName.Focus();
				}
			}
    }

      protected bool ValidateUserEmail(Guid userId)
      {
          bool bRequireUserEmailValidation = false;
          scms.data.ScmsDataContext dc = new scms.data.ScmsDataContext();
          var securitySettings = (from ss in dc.scms_security_settings
                                  select ss).FirstOrDefault();
          if (securitySettings != null)
          {
              bRequireUserEmailValidation = securitySettings.requireUserEmailValidation;
          }

          bool bUserEmailValidated = false;
          if (bRequireUserEmailValidation)
          {
              var scmsUser = (from su in dc.scms_users
                              where su.userid == userId
                              select su).FirstOrDefault();
              if (scmsUser != null)
              {
								bUserEmailValidated = scmsUser.emailvalidated;
              }
          }

          return !bRequireUserEmailValidation || bUserEmailValidated;
      }

			protected void btnLogOut_Clicked(object sender, EventArgs args)
			{
				System.Web.Security.FormsAuthentication.SignOut();
				Response.Redirect("~/", true);
			}

      protected void btnLogin_Click(object sender, EventArgs args)
      {
          try
          {
              string strUserName = txtUserName.Text.Trim();
              string strPassword = txtPassword.Text.Trim();

              if( System.Web.Security.Membership.Provider.ValidateUser(strUserName, strPassword))
              {
                  System.Web.Security.MembershipUser membershipUser = System.Web.Security.Membership.GetUser(strUserName);

                  if (ValidateUserEmail((Guid)membershipUser.ProviderUserKey))
                  {
                      System.Web.Security.FormsAuthentication.SetAuthCookie(strUserName, false);
                      string strUrl = Request.QueryString["returnUrl"];
                      if (string.IsNullOrEmpty(strUrl))
                      {
                          strUrl = "/";
                      }
                      Response.Redirect(strUrl, true);
                  }
              }
              else
              {
                  statusMessage.ShowFailure("Either account does not exist or password is not valid.");
              }
          }
          catch( System.Threading.ThreadAbortException)
          {
          }
          catch (Exception ex)
          {
              ScmsEvent.Raise("Exception thrown while processing login", this, ex);
              statusMessage.ShowFailure("An error occurred while processing login information.");
          }
      }

      protected void btnForgotUserName_Click(object sender, EventArgs args)
      {
          mv.SetActiveView(viewForgotUserName);
      }

      protected void btnForgotPassword_Click(object sender, EventArgs args)
      {
          mv.SetActiveView(viewForgotPassword);
      }

      protected void btnBackToLogin_Click(object sender, EventArgs args)
      {
          mv.SetActiveView(viewLogin);
      }

      
      protected void btnForgotUserNameSubmit_Click(object sender, EventArgs args)
      {
          try
          {
              string strEmail = txtForgotUserNameEmail.Text.Trim();
              string strUserName = System.Web.Security.Membership.GetUserNameByEmail(strEmail);

              if (!string.IsNullOrEmpty(strUserName))
              {
                  System.Net.Mail.MailMessage message = new System.Net.Mail.MailMessage();
                  message.To.Add(strEmail);

                  message.Subject = "the information you requested";
                  message.Body = string.Format("The user name for your {0} account is {1}.", Request.Url.DnsSafeHost, strUserName);

                  System.Net.Mail.SmtpClient client = new System.Net.Mail.SmtpClient();
                  client.Send(message);

                  mvForgotUserName.SetActiveView(viewForgotUserNameAck);
              }
          }
          catch (Exception ex)
          {
              string strMessage = string.Format("Exception thrown while recovering user name for email '{0}'.", txtForgotUserNameEmail.Text);
              ScmsEvent.Raise(strMessage, this, ex);
              statusMessageForgotUserName.ShowFailure("An error occurred while recovering user name");
          }
      }

      protected void btnForgotPasswordSubmit_Click(object sender, EventArgs args)
      {
          try
          {
              string strUserName = txtForgotPasswordUserName.Text.Trim();
              System.Web.Security.MembershipUser membershipUser = System.Web.Security.Membership.GetUser(strUserName);
              if (membershipUser != null)
              {
                  System.Net.Mail.MailMessage message = new System.Net.Mail.MailMessage();

                  if (string.IsNullOrEmpty(membershipUser.Email))
                  {
                      statusMessageForgotPassword.ShowFailure("Unable to reset password, there is no email associated with this account.");
                  }
                  else
                  {
                      message.To.Add(membershipUser.Email);

                      string strPassword = membershipUser.ResetPassword();

                      message.Subject = "the information you requested";
                      message.Body = string.Format("The new password for your {0} account is:  {1}", Request.Url.DnsSafeHost, strPassword);

                      System.Net.Mail.SmtpClient client = new System.Net.Mail.SmtpClient();
                      client.Send(message);
                  }

                  mvForgotPassword.SetActiveView(viewForgotPasswordAck);
              }
              else
              {
                  statusMessageForgotPassword.ShowFailure("There is no account associated with this user name.");
              }
          }
          catch (Exception ex)
          {
              string strMessage = string.Format("Exception thrown while recovering user name for email '{0}'.", txtForgotUserNameEmail.Text);
              ScmsEvent.Raise(strMessage, this, ex);
              statusMessageForgotUserName.ShowFailure("An error occurred while recovering user name");
          }
      }

  }
}