using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace scms.modules.security.welcome
{
    public partial class view : scms.RootControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            bool bLoggedIn = false;
            if (this.Page.User != null)
            {
                if (this.Page.User.Identity.IsAuthenticated)
                {
                    bLoggedIn = true;
                    literalUser.Text = this.Page.User.Identity.Name;
                }
            }

            mv.SetActiveView(bLoggedIn ? viewLoggedIn : viewNotLoggedIn);
        }

        protected void btnLogin_Click(object sender, EventArgs args)
        {
            try
            {
                scms.RootPage page = this.Page as scms.RootPage;
                if (page != null)
                {
                    page.RedirectToLogin();
                }

                Response.Redirect(System.Web.Security.FormsAuthentication.LoginUrl, true);
            }
            catch (System.Threading.ThreadAbortException)
            {
            }
        }

        protected void btnLogout_Click(object sender, EventArgs args)
        {
            System.Web.Security.FormsAuthentication.SignOut();
            Session.Abandon();
            Response.Redirect(Request.RawUrl);
        }
    }
}