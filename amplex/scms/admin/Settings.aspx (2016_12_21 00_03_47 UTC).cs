using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace scms.admin
{
    public partial class Settings : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                scms.admin.Admin master = (scms.admin.Admin)this.Master;
                master.NavType = scms.admin.Admin.ENavType.Settings;

                LoadSettings();
            }
        }


        protected void LoadSettings()
        {
            bool ? bShowAdminEditLinks;
            scms.Configuration.GetValue("show-admin-edit-links", true, out bShowAdminEditLinks);
            checkShowAdminEditLinks.Checked = bShowAdminEditLinks.HasValue && bShowAdminEditLinks.Value;

            bool? bSslEnabled;
            scms.Configuration.GetValue("ssl-enabled", false, out bSslEnabled);
            checkSslEnabled.Checked = bSslEnabled.HasValue && bSslEnabled.Value;

            bool? bUseSslForAdmin;
            scms.Configuration.GetValue("use-ssl-for-admin", false, out bUseSslForAdmin);
            checkUseSslForAdmin.Checked = bUseSslForAdmin.HasValue && bUseSslForAdmin.Value;
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            bool bAnyErrors = false;

            bool bShowAdminEditLinks = checkShowAdminEditLinks.Checked;
            if (!scms.Configuration.SetValue("show-admin-edit-links", bShowAdminEditLinks.ToString()))
            {
                bAnyErrors = true;
            }

            bool bUseSslForAdmin = checkUseSslForAdmin.Checked;
            if (!scms.Configuration.SetValue("use-ssl-for-admin", bUseSslForAdmin.ToString()))
            {
                bAnyErrors = true;
            }

            bool bSslEnabled = checkSslEnabled.Checked;
            if (!scms.Configuration.SetValue("ssl-enabled", bSslEnabled.ToString()))
            {
                bAnyErrors = true;
            }
            

            if (!bAnyErrors)
            {
                statusMessage.ShowSuccess("Settings updated");
            }
            else
            {
                statusMessage.ShowFailure("Failed saving at least one setting");
            }
        }
    }
}
