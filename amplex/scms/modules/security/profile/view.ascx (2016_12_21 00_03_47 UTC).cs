using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Security;

namespace scms.modules.security.profile
{
    public partial class view : scms.RootControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (!Page.User.Identity.IsAuthenticated)
                    throw new Exception("Unexpected, user is not authenticated");

                //System.Web.Security.RolePrincipal principal = (System.Web.Security.RolePrincipal)Page.User;
                MembershipUser user = Membership.GetUser();

                literalUserId.Text = user.UserName;
                txtEmailAddress.Text = user.Email;

                EnsureUser((Guid)user.ProviderUserKey);
                scms.data.ScmsDataContext dcs = new scms.data.ScmsDataContext();
                scms.data.scms_user scmsUser = (from u in dcs.scms_users
                                                where u.userid == (Guid)user.ProviderUserKey
                                                select u).Single();
                txtFirstName.Text = scmsUser.firstName;
                txtLastName.Text = scmsUser.lastName;

                scms.data.ScmsDataContext dc = new scms.data.ScmsDataContext();
                EnsureOrganization(dc, (Guid)user.ProviderUserKey);

								throw new Exception("check correct user id i used here");
							/*
                scms.data.scms_organization org = (from ou in dc.scms_organization_users
                                    where ou.user_id == (Guid)user.ProviderUserKey
                                    join o in dc.scms_organizations on ou.organization_id equals o.id
                                    select o).Single(); 
                txtCompany.Text = org.name;*/
						}
        }

        protected void EnsureUser(Guid guidUserId)
        {
            scms.data.ScmsDataContext dc = new scms.data.ScmsDataContext();
            scms.data.scms_user user = (from u in dc.scms_users
                                        where u.userid == guidUserId
                                        select u).FirstOrDefault();
            if( user == null )
            {
                user = new scms.data.scms_user();
                user.userid = guidUserId;
                dc.scms_users.InsertOnSubmit(user);
                dc.SubmitChanges();

            }
        }

        protected void EnsureOrganization(scms.data.ScmsDataContext dc, Guid guidUserId)
        {
					throw new Exception("check user id used here");
					/*
            scms.data.scms_organization  org = (from ou in dc.scms_organization_users
                                where ou.user_id == guidUserId
                                join o in dc.scms_organizations on ou.organization_id equals o.id
                                select o).FirstOrDefault();
            if (org == null)
            {
                org = new scms.data.scms_organization();
                dc.scms_organizations.InsertOnSubmit(org);
                dc.SubmitChanges();

                scms.data.scms_organization_user ou = new scms.data.scms_organization_user();
                dc.scms_organization_users.InsertOnSubmit(ou);
								throw new Exception("check user ids");
                ou.organization_id = org.id;
                ou.user_id = guidUserId;
                dc.SubmitChanges();
            }
					 * */

        }

        protected void btnSave_Click(object sender, EventArgs args)
        {
            try
            {
                MembershipUser mu = Membership.GetUser();
                Guid guidUserId = (Guid)mu.ProviderUserKey;

                scms.data.ScmsDataContext dcs = new scms.data.ScmsDataContext();
                scms.data.scms_user user = (from u in dcs.scms_users
                                            where u.userid == guidUserId
                                            select u).Single();
                user.firstName = txtFirstName.Text.Trim();
                user.lastName = txtLastName.Text.Trim();
                dcs.SubmitChanges();


								throw new Exception("check which id is used here");
								/*
									scms.data.ScmsDataContext dcScms = new scms.data.ScmsDataContext();
									scms.data.scms_organization org = (from ou in dcScms.scms_organization_users
																			where ou.user_id == guidUserId
																			join o in dcScms.scms_organizations on ou.organization_id equals o.id
																			select o).Single();
									org.name = txtCompany.Text.Trim();
									dcScms.SubmitChanges();
							 

									mu.Email = txtEmailAddress.Text.Trim();
								 * * */
								statusMessage.ShowSuccess("Profile updated");
                                    
            }
            catch( Exception ex)
            {
                ScmsEvent.Raise("failed saving profile", this, ex);
                statusMessage.ShowFailure("Failed updating profile");
            }
        }

        protected void btnCancel_Click(object sender, EventArgs args)
        {
            Response.Redirect("~/account");
        }

        /*
        protected void btnLogin_Click(object sender, EventArgs args)
        {
            try
            {
                string strUserName = txtUserName.Text.Trim();
                string strPassword = txtPassword.Text.Trim();

                if (System.Web.Security.Membership.Provider.ValidateUser(strUserName, strPassword))
                {
                    System.Web.Security.FormsAuthentication.SetAuthCookie(strUserName, false);
                    string strUrl = Request.QueryString["returnUrl"];
                    if (string.IsNullOrEmpty(strUrl))
                    {
                        strUrl = "/";
                    }
                    Response.Redirect(strUrl, true);
                }
                else
                {
                    statusMessage.ShowFailure("Either account does not exist or password is not valid.");
                }
            }
            catch (System.Threading.ThreadAbortException)
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
        */

    }
}