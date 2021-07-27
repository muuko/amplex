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

namespace scms.admin
{
    public partial class loginPage : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

            MembershipUser user = Membership.GetUser();


            if (!IsPostBack)
            {
                global::scms.data.ScmsDataContext dc = new global::scms.data.ScmsDataContext();
                int nUsers = dc.aspnet_Users.Where(u => u.IsAnonymous == false).Count();
                if (nUsers == 0)
                {
                    MultiView multiView = (MultiView)loginView.FindControl("multiView");
                    View viewCreateDefaultUser = (View)multiView.FindControl("viewCreateDefaultUser");
                    multiView.SetActiveView(viewCreateDefaultUser);
                }
            }
        }

        protected void btnCreateUser_Click(object sender, EventArgs args)
        {
            Page.Validate();
            if (Page.IsValid)
            {
                EnsureAdministratorRole();

                MembershipCreateStatus status;
                TextBox txtUserName = (TextBox)loginView.FindControl("txtUserName");
                TextBox txtPassword = (TextBox)loginView.FindControl("txtPassword");
                TextBox txtEmail = (TextBox)loginView.FindControl("txtEmail");
                MembershipUser user = Membership.CreateUser(txtUserName.Text.Trim(), txtPassword.Text.Trim(), txtEmail.Text.Trim(), null, null, true, out status);

                MultiView multiView = (MultiView)loginView.FindControl("multiView");
                if (status == MembershipCreateStatus.Success)
                {
                    Roles.AddUserToRole(user.UserName, "administrator");
                    View viewLogin = (View)multiView.FindControl("viewLogin");
                    multiView.SetActiveView(viewLogin);
                }
                else
                {
                    string strMessage = string.Format("Failed creating user: {0}", status.ToString());

                    global::scms.admin.controls.StatusMessage statusMessage = (global::scms.admin.controls.StatusMessage)multiView.FindControl("statusMessage");
                    statusMessage.ShowFailure(strMessage);
                }
            }
        }

        protected void EnsureAdministratorRole()
        {
            bool bAdministratorFound = false;
            string[] astrRoles = Roles.GetAllRoles();
            foreach (string strRole in astrRoles)
            {
                if (string.Compare(strRole, "administrator", true) == 0)
                {
                    bAdministratorFound = true;
                    break;
                }
            }

            if (!bAdministratorFound)
            {
                Roles.CreateRole("administrator");
            }

        }

        protected void login_loggedIn(object sender, EventArgs args)
        {
            MembershipUser user = Membership.GetUser();

            Response.Redirect("~/scms/admin/pages.aspx", false);
        }
    }
}
