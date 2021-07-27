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
    public partial class StatusMessage : System.Web.UI.UserControl
    {
        protected string strErrorMessage = null;
        protected string strSuccessMessage = null;
        protected bool bShowSuccess = false;
        protected bool bShowError = false;

        protected void Page_Load(object sender, EventArgs e)
        {
            DataBind();

        }

        public override void DataBind()
        {
            base.DataBind();

            literalSuccessMessage.Text = strSuccessMessage;
            divSuccess.Visible = bShowSuccess;

            literalFailureMessage.Text = strErrorMessage;
            divFailure.Visible = bShowError;
        }

        public void ShowSuccess(string strMessage)
        {
            strSuccessMessage = strMessage;
            bShowSuccess = true;

            DataBind();
            //literalSuccessMessage.Text = strMessage;
            //divSuccess.Visible = true;
        }

        public void ShowFailure(string strMessage)
        {
            strErrorMessage = strMessage;
            bShowError = true;

            DataBind();
            //literalFailureMessage.Text = strMessage;
            //divFailure.Visible = true;
        }

        public void Clear()
        {
            strSuccessMessage = null;
            bShowSuccess = false;
            bShowError = false;
            DataBind();
        }
    }
}