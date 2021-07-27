using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Mail;

namespace scms.modules.forms
{
    public class formsAutoResponderEventHandler : forms.IFormSubmissionEventHandler
    {
        public string UniqueName
        {
            get { return "scms.forms.formsAutoResponderEventHandler"; }
        }

        public string Name
        {
            get { return "Auto Responder"; }
        }

        public string SettingsControlPath
        {
            get { return "/scms/modules/forms/autoresponder/formsettings.ascx"; }
        }

        protected bool GetValueForField(bool bEnabled, int ? nFieldId, System.Collections.Generic.Dictionary<int, scms.data.scms_form_submission_fieldvalue> fieldValuesByFieldId, out string strValue)
        {
            bool bValueSet = false;
            strValue = null;

            if (bEnabled)
            {
                if (nFieldId.HasValue)
                {
                    scms.data.scms_form_submission_fieldvalue fieldValue;
                    if (fieldValuesByFieldId.TryGetValue(nFieldId.Value, out fieldValue))
                    {
                        if (!string.IsNullOrEmpty(fieldValue.value))
                        {
                            strValue = fieldValue.value;
                            bValueSet = true;
                        }
                    }
                }
            }

            return bValueSet;
        }

        protected string ReplaceFieldsWithValues(string strText, System.Collections.Generic.Dictionary<int, scms.data.scms_form_submission_fieldvalue> fieldValuesByFieldId)
        {
            string strResults = strText;

            if (!string.IsNullOrEmpty(strResults))
            {
                foreach (var field in fieldValuesByFieldId.Values)
                {
                    string strFieldById = string.Format("##{0}##", field.fieldid);
                    strResults = strResults.Replace(strFieldById, field.value);
                }
            }

            return strResults;
        }

        public bool FormSubmitted(int nEventHandlerId, int nModuleInstanceId, int nFormId, int nSubmissionId, System.Collections.Generic.Dictionary<int, scms.data.scms_form_submission_fieldvalue> fieldValuesByFieldId)
        {
            bool bSuccess = false;
            try
            {
                scms.data.ScmsDataContext dcScms = new scms.data.ScmsDataContext();
                
                var feh = (from eh1 in dcScms.scms_form_eventhandlers
                           where eh1.id == nEventHandlerId
                           select eh1).Single();
                var areh = (from eh2 in dcScms.scms_forms_autoresponder_eventhandlers
                           where eh2.eventHandlerId == nEventHandlerId
                           select eh2).Single();

                scms.data.scms_form_submission_fieldvalue fieldValue = null;
                if (!fieldValuesByFieldId.TryGetValue(areh.emailAddressFieldId, out fieldValue))
                    throw new Exception( "unexpected missing field value for email address");

                MailMessage message = new MailMessage();
                if (!string.IsNullOrEmpty(areh.from))
                {
                    try
                    {
                        message.From = new MailAddress(areh.from);
                    }
                    catch( Exception )
                    {
                        throw new Exception(string.Format("Failed adding from email address '{0}'.", areh.from));
                    }
                }

                try
                {
                    message.To.Add(new MailAddress(fieldValue.value));
                }
                catch (Exception)
                {
                    throw new Exception(string.Format( "Failed adding to email address '{0}'.", fieldValue.value));
                }

                if (!string.IsNullOrEmpty(areh.cc))
                {
                    string[] astrCC = areh.cc.Split(new char[] { ',', ';' });
                    foreach (string strCC in astrCC)
                    {
                        try
                        {
                            message.CC.Add(new MailAddress(strCC));
                        }
                        catch
                        {
                            string strMessage = string.Format("Failed adding cc email address '{0}'.", strCC);
                            ScmsEvent.Raise(strMessage, this, null);
                        }
                    }
                }

                if (!string.IsNullOrEmpty(areh.bcc))
                {
                    string[] astrBCC = areh.bcc.Split(new char[] { ',', ';' });
                    foreach (string strBCC in astrBCC)
                    {
                        try
                        {
                            message.Bcc.Add(new MailAddress(strBCC));
                        }
                        catch
                        {
                            string strMessage = string.Format("Failed adding cc email address '{0}'.", strBCC);
                            ScmsEvent.Raise(strMessage, this, null);
                        }
                    }
                }

                message.Subject = ReplaceFieldsWithValues(areh.subject, fieldValuesByFieldId);
                message.Body = ReplaceFieldsWithValues(areh.body, fieldValuesByFieldId);
                message.IsBodyHtml = areh.bodyIsHtml;

                SmtpClient client = new SmtpClient();
                client.Send(message);
                bSuccess = true;

            }
            catch (Exception ex)
            {
                string strMessage = string.Format("Failed sending auto responder message.");
                ScmsEvent.Raise(strMessage, this, ex);
            }

            return bSuccess;
        }
        
    }
}
