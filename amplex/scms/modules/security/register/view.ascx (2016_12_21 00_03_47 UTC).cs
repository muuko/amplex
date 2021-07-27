using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Security;
using System.Security.Authentication;
using System.Security.Principal;
using System.Net.Mail;
using scms.data;

namespace scms.modules.security.register
{
    public partial class view : scms.RootControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
        }


        protected void btnRegister_Click(object sender, EventArgs args)
        {
            if (Page.IsValid)
            {

                bool bSuccess = false;

                bool bUserCreated = false;

                MembershipUser membershipUser = null;


                // create the user
                try
                {
                    string strEmailAddress = txtEmailAddress.Text.Trim().ToLower();
                    string strPassword = txtPassword.Text.Trim();

                    // todo, turn off auto approve
                    membershipUser = Membership.CreateUser(strEmailAddress, strPassword);
                    membershipUser.IsApproved = false;
                    membershipUser.Email = strEmailAddress;

                    bUserCreated = true;
                }
                catch (Exception ex)
                {
                    ScmsEvent.Raise("Exception thrown while creating membership user", this, ex);
                }

                

                // create the scms_user & scms_organization
                bool bOrganizationSetup = false;
                scms_user user = null;
                scms_org org = null;
                if (bUserCreated)
                {
                    try
                    {
                        scms.data.ScmsDataContext dc = new scms.data.ScmsDataContext();
                        org = new scms_org();
                        org.name = txtCompany.Text.Trim();
                        dc.scms_orgs.InsertOnSubmit(org);
                        dc.SubmitChanges();
												throw new Exception("this needs to be updated");
											/*
                        scms_organization_user orgUser = new scms_organization_user();
                        orgUser.organizationId = org.id;
												*/
                        // orgUser.user_id = (Guid)membershipUser.ProviderUserKey;
                        dc.SubmitChanges();

                        bOrganizationSetup = true;
                    }
                    catch (Exception ex)
                    {
                        ScmsEvent.Raise("Exception thrown while creating organization", this, ex);
                    }
                }

                // send the invitation
                if (bUserCreated && bOrganizationSetup)
                {
                    try
                    {
                        ScmsDataContext dc = new ScmsDataContext();

                        scms.data.scms_user scmsUser = (from su in dc.scms_users
                                                       where su.userid == (Guid)membershipUser.ProviderUserKey
                                                       select su).FirstOrDefault();
                        if (scmsUser == null)
                        {
                            scmsUser = new scms_user();
                            scmsUser.userid = (Guid)membershipUser.ProviderUserKey;
                            dc.scms_users.InsertOnSubmit(scmsUser);
                        }
												throw new Exception("not valid yet");
											/* scmsUser.emailValidated = false;

											scms_registration_pending registrationPending = new scms_registration_pending();
											registrationPending.userId = (Guid)membershipUser.ProviderUserKey;
											registrationPending.key = Guid.NewGuid();
											registrationPending.dtCreated = DateTime.Now;
											dc.scms_registration_pendings.InsertOnSubmit(registrationPending);
											
											if (SendInvitationEmail(membershipUser.Email, (Guid)membershipUser.ProviderUserKey, registrationPending.key))
											{
													dc.SubmitChanges();
													bSuccess = true;
											}
											 * */
										}
                    catch (Exception ex)
                    {
                        ScmsEvent.Raise("Exception setting up user attributes and pending registration", this, ex);
                    }
                }

                if (bSuccess)
                {
                    literalEmailAddress.Text = membershipUser.Email;
                    mv.SetActiveView(viewAck);
                }
                else
                {
                    statusMessage.ShowFailure("Failed adding user or sending email.");

                    try
                    {
                        if (bOrganizationSetup)
                        {
                            ScmsDataContext dc2 = new ScmsDataContext();
                            org = (from o in dc2.scms_orgs
                                   where o.id == org.id
                                   select o).FirstOrDefault();

                            if (org != null)
                            {
                                dc2.scms_orgs.DeleteOnSubmit(org);
                                dc2.SubmitChanges();
                            }
                        }

                        
                    }
                    catch (Exception ex)
                    {
                        ScmsEvent.Raise("Exception while deleting failed org", this, ex);
                    }

                    try
                    {
                        Membership.DeleteUser(membershipUser.UserName, true);
                    }
                    catch (Exception ex)
                    {
                        ScmsEvent.Raise("Exception while deleting failed user", this, ex);
                    }
                }
            }
        }

        protected bool SendInvitationEmail(string strEmailAddress, Guid guidUser, Guid guidKey)
        {
            bool bSuccess = false;

            try
            {
                System.Net.Mail.MailMessage message = new MailMessage();
                message.To.Add(strEmailAddress);
                message.IsBodyHtml = false;
                message.Subject = string.Format("Welcome to {0}", System.Configuration.ConfigurationManager.AppSettings["promote_business_name"]); ;
                message.Body = string.Format( 
@"Welcome to cart promotions (whatever)
Click this link to validate your email address and setup your first promotion:
http://{0}/register-validate?user={1}key={2}
", Request.Url.Authority, guidUser, guidKey);

                System.Net.Mail.SmtpClient smtpClient = new SmtpClient();
                smtpClient.Send(message);

                bSuccess = true;
            }
            catch (Exception ex)
            {
                ScmsEvent.Raise("Exception occurred while sending invitation email", this, ex);
            }

            return bSuccess;
        }

        protected void cvPassword_ServerValidate(object sender, ServerValidateEventArgs args)
        {
            bool bValid = false;

            string strPassword = txtPassword.Text.Trim();
            string strConfirmPassword = txtConfirmPassword.Text.Trim();

            if (string.Compare(strPassword, strConfirmPassword, false) != 0)
            {
                cvPassword.ErrorMessage = "Passwords do not match<br />";
            }
            else
            {
                if (strPassword.Length < 6)
                {
                    cvPassword.ErrorMessage = "Passwords must be a minimum of 6 characters<br />";
                }
                else
                {
                    bValid = true;
                }
            }

            args.IsValid = bValid;
        }

        protected void cvEmailAddress_ServerValidate(object sender, ServerValidateEventArgs args)
        {
            bool bValid = false;

            try
            {
                string strEmailAddress = txtEmailAddress.Text.Trim();
                MembershipUser existingUser = Membership.GetUser(strEmailAddress);

                // storing users with userid = email address
                if (existingUser == null)
                {
                    string strUserName = Membership.GetUserNameByEmail(strEmailAddress);
                    if (string.IsNullOrEmpty(strUserName))
                    {
                        bValid = true;
                    }
                }

                string strMessage = string.Format("There is already a user with this email address.<br />Click <a href=\"/login\">here</a> to login.<br /><br />");
                cvEmailAddress.ErrorMessage = strMessage;
            }
            catch (Exception ex)
            {
                string strMessage = string.Format("An error occurred while looking up user");
                cvEmailAddress.ErrorMessage = strMessage;
                ScmsEvent.Raise("Failed looking up user", this, ex);
            }

            args.IsValid = bValid;
        }

        /*
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

        protected void EnsureOrganization(promoteDataContext dc, Guid guidUserId)
        {
            organization org = (from ou in dc.organization_users
                                where ou.user_id == guidUserId
                                join o in dc.organizations on ou.organization_id equals o.id
                                select o).FirstOrDefault();
            if (org == null)
            {
                org = new organization();
                dc.organizations.InsertOnSubmit(org);
                dc.SubmitChanges();

                organization_user ou = new organization_user();
                dc.organization_users.InsertOnSubmit(ou);
                ou.organization_id = org.id;
                ou.user_id = guidUserId;
                dc.SubmitChanges();
            }

        }
        */

        /*
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

                promoteDataContext dc = new promoteDataContext();
                organization org = (from ou in dc.organization_users
                                    where ou.user_id == guidUserId
                                    join o in dc.organizations on ou.organization_id equals o.id
                                    select o).Single();
                org.name = txtCompany.Text.Trim();
                dc.SubmitChanges();

                mu.Email = txtEmailAddress.Text.Trim();
                statusMessage.ShowSuccess("Profile updated");
                                    
            }
            catch( Exception ex)
            {
                ScmsEvent.Raise("failed saving profile", this, ex);
                statusMessage.ShowFailure("Failed updating profile");
            }
        }
        */

    }
}