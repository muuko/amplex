using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace scms.modules.security
{
    public partial class settings : scms.RootControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                psLogin.SiteId = SiteId;
                LoadSettings();
            }
        }

        protected void LoadSettings()
        {
            try
            {
                if (!SiteId.HasValue)
                    throw new Exception("unexpected no value for siteid");
                scms.data.ScmsDataContext dc = new scms.data.ScmsDataContext();
                var settings = ( from s in dc.scms_security_settings 
                                 where s.siteid == SiteId.Value 
                                 select s ).FirstOrDefault();
                if (settings != null)
                {
                    checkSslPublicEnabled.Checked = settings.sslPublicEnabled.HasValue && settings.sslPublicEnabled.Value;
                    
                    psLogin.PageId = settings.pageIdLogin;
                    checkUserEmailValidationRequired.Checked = settings.requireUserEmailValidation;
                    // checkUserEmailValidationIsHtml.Checked = settings.userEmailValidationEmailIsHtml.HasValue && settings.userEmailValidationEmailIsHtml.Value;
                    // txtSubject.Text = settings.userEmailValidationEmailSubject;
                    // txtBody.Text = settings.userEmailValidationEmailText;
                }

                EnableUserEmailValidationControls();
            }
            catch (Exception ex)
            {
                ScmsEvent.Raise("failed loading security settings", this, ex);
                statusMessage.ShowFailure("Failed loading security settings");
            }
        }

        protected void btnSave_Click(object sender, EventArgs args)
        {
            try
            {
                scms.data.ScmsDataContext dc = new scms.data.ScmsDataContext();
                scms.data.scms_security_setting settings = null;
                settings = (from s in dc.scms_security_settings
                                where s.siteid == SiteId.Value
                                select s).FirstOrDefault();
                if (settings == null)
                {
                    settings = new scms.data.scms_security_setting();
                    settings.siteid = SiteId.Value;
                    dc.scms_security_settings.InsertOnSubmit(settings);
                }

                settings.sslPublicEnabled = checkSslPublicEnabled.Checked;
                settings.pageIdLogin = psLogin.PageId;
                settings.requireUserEmailValidation = checkUserEmailValidationRequired.Checked;
                // settings.userEmailValidationEmailIsHtml = checkUserEmailValidationIsHtml.Checked;
                // settings.userEmailValidationEmailSubject =txtSubject.Text;
                // settings.userEmailValidationEmailText = txtBody.Text;

                dc.SubmitChanges();
                statusMessage.ShowSuccess("Settings updated");

            }
            catch (Exception ex)
            {
                ScmsEvent.Raise("failed saving security settings", this, ex);
                statusMessage.ShowFailure("Failed saving security settings");
            }
        }

        protected void checkUserEmailValidationRequired_CheckedChanged(object sender, EventArgs args)
        {
            EnableUserEmailValidationControls();
        }

        protected void EnableUserEmailValidationControls()
        {
            bool bEnabled = checkUserEmailValidationRequired.Checked;

            checkUserEmailValidationIsHtml.Enabled = bEnabled;
            txtSubject.Enabled = bEnabled;
            txtBody.Enabled = bEnabled;
            labelBody.Disabled = !bEnabled;
            labelIsHtml.Disabled = !bEnabled;
            labelSubject.Disabled = !bEnabled;
        }


    }
}