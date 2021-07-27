using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace scms.modules.security.password
{
    public partial class view : scms.RootControl
    {

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                ViewState["referrer"] = Request.UrlReferrer.AbsolutePath;
            }

            /*
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
            */
        }

        protected void OnContinueButtonClick(object sender, EventArgs args)
        {
            Return();
        }

        protected void Return()
        {
            try
            {
                string strUrl = Request.QueryString["returnUrl"];
                if (string.IsNullOrEmpty(strUrl))
                {
                    strUrl = (string)ViewState["referrer"];
                    if (string.IsNullOrEmpty(strUrl))
                    {
                        strUrl = "/";
                    }
                }
                Response.Redirect(strUrl, true);
            }
            catch (System.Threading.ThreadAbortException)
            {
            }
            catch (Exception ex)
            {
                ScmsEvent.Raise("Exception thrown while continuing", this, ex);
            }
        }
    }
}