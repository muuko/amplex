using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Security;

namespace scms.admin.controls
{
    public partial class RolesList : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadRoles();
            }

        }

        protected void LoadRoles()
        {
            try
            {
                string[] astrRoles = Roles.GetAllRoles();
                rptRoles.DataSource = astrRoles;
                rptRoles.DataBind();
            }
            catch (Exception ex)
            {
                global::scms.ScmsEvent.Raise("Exception thrown while loading roles", this, ex);
            }
        }

        protected void btnRole_Command(object sender, CommandEventArgs args)
        {
            try
            {
                string strRoleName = (string)args.CommandArgument;
                string strUrl = string.Format("/scms/admin/security.aspx?view=users&r={0}", strRoleName);
                Response.Redirect(strUrl, true);
            }
            catch (System.Threading.ThreadAbortException)
            {
            }
        }


        protected void btnNewRole_Click(object sender, EventArgs args)
        {
            try
            {
                string strRoleName = txtRoleName.Text.Trim();

                if (!Roles.RoleExists(strRoleName))
                {
                    Roles.CreateRole(strRoleName);
                    LoadRoles();
                    string strMessage = string.Format("role '{0}' created", strRoleName);
                    statusMessage.ShowSuccess(strMessage);
                }
                else
                {
                    string strMessage = string.Format("a role named '{0}' already exists", strRoleName);
                    statusMessage.ShowFailure(strMessage);
                }
            }
            catch (Exception ex)
            {
                string strMessage = string.Format("An error occurred while creating the role");
                statusMessage.ShowFailure(strMessage);
                global::scms.ScmsEvent.Raise(strMessage, this, ex);
            }
        }

        protected void Delete_Command(object sender, CommandEventArgs args)
        {
            try
            {
                string strRoleName = (string)args.CommandArgument;
                if (string.Compare(strRoleName, "administrator", true) == 0)
                {
                    statusMessage.ShowFailure("Administrator role cannot be deleted");
                }
                else
                {
                    Roles.DeleteRole(strRoleName, false);
                    LoadRoles();
                    string strMessage = string.Format("Role '{0}' deleted", strRoleName);
                    statusMessage.ShowSuccess(strMessage);
                }
            }
            catch (Exception ex)
            {
                string strMessage = string.Format("An error occurred while deleting the role");
                statusMessage.ShowFailure(strMessage);
                global::scms.ScmsEvent.Raise(strMessage, this, ex);
            }
        }

        protected string GetDeleteMessage(object obj)
        {
            string strMessage = string.Format("javascript: return confirm(\"Delete '{0}' role?\");", obj.ToString());
            return strMessage;
        }
    }
}