using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace scms.modules.submission
{
    public partial class formsettings : System.Web.UI.UserControl, scms.modules.forms.IFormSubmissionEventHandlerSettings
    {
        /*
        public delegate void SettingsSavedDelegate();
        public delegate void SettingsCancelledDelegate();
        public SettingsSavedDelegate OnSettingsSaved = null;
        public SettingsCancelledDelegate OnSettingsCancelled = null;
        */


        public int? SiteId
        {
            get;
            set;
        }

        public int? FormId
        {
            get;
            set;
        }

        public int? EventHandlerId
        {
            get;
            set;
        }

        scms.modules.forms.SettingsSavedDelegate settingsSavedDelegate = null;
        scms.modules.forms.SettingsCancelledDelegate settingsCancelledDelegate = null;

        public void SetDelegates(scms.modules.forms.SettingsSavedDelegate settingsSavedDelegate, scms.modules.forms.SettingsCancelledDelegate settingsCancelledDelegate)
        {
            this.settingsSavedDelegate = settingsSavedDelegate;
            this.settingsCancelledDelegate = settingsCancelledDelegate;
        }

        protected int? SubmissionApplicationId
        {
            get { return (int?)ViewState["SubmissionApplicationId"]; }
            set { ViewState["SubmissionApplicationId"] = value; }
        }

        protected int? SubmissionModuleId
        {
            get { return (int?)ViewState["SubmissionModuleId"]; }
            set { ViewState["SubmissionModuleId"] = value; }
        }


        protected void Page_Init(object sender, EventArgs args)
        {
            moduleSelector.SiteId = SiteId;

            int? nSubmissionApplicationId = SubmissionApplicationId;
            int? nSubmissionModuleId = SubmissionModuleId;

            if (!nSubmissionApplicationId.HasValue)
            {
                try
                {
                    scms.data.ScmsDataContext dc = new scms.data.ScmsDataContext();
                    nSubmissionApplicationId = (from a in dc.scms_plugin_applications
                                                where a.name == "submission"
                                                select a.id).Single();

                    nSubmissionModuleId = (from m in dc.scms_plugin_modules
                                           where m.name == "submission"
                                           select m.id).Single();

                    ViewState["SubmissionApplicationId"] = nSubmissionApplicationId;
                    ViewState["SubmissionModuleId"] = nSubmissionModuleId;

                }
                catch (Exception ex)
                {
                    string strMessage = "Exception thrown while looking up submission application module";
                    ScmsEvent.Raise(strMessage, this, ex);
                }
            }
            moduleSelector.PluginApplicationId = SubmissionApplicationId;
            moduleSelector.PluginModuleId = SubmissionModuleId;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            moduleSelector.OnModuleSelectionChanged += ModuleSelectionChanged;
        }

        protected void ModuleSelectionChanged(int? nModuleId)
        {
            try
            {
                scms.data.scms_submission_module submissionModule = null;

                if (nModuleId.HasValue)
                {
                    scms.data.ScmsDataContext dc = new scms.data.ScmsDataContext();

                    submissionModule =
                        (from sm in dc.scms_submission_modules
                         where sm.instanceId == nModuleId.Value
                         select sm).FirstOrDefault();


                }
                EnableControls(submissionModule);
            }
            catch (Exception ex)
            {
                string strMessage = string.Format("ModuleSelectionChanged, exception thrown for id '{0}'.", nModuleId);
                ScmsEvent.Raise(strMessage, this, ex);
            }
        }

        protected void PopulateFormFields()
        {
            try
            {
                scms.data.ScmsDataContext dc = new scms.data.ScmsDataContext();
                var fields = from f in dc.scms_form_fields
                             where f.formid == FormId.Value
                             where f.deleted == false
                             orderby f.name
                             select f;
                scms.data.scms_form_field[] aFields = fields.ToArray();

                PopulateFormFields(ddlTitle, aFields);
                PopulateFormFields(ddlLink, aFields);
                PopulateFormFields(ddlImage, aFields);
                PopulateFormFields(ddlVideo, aFields);
                PopulateFormFields(ddlDescription, aFields);
                PopulateFormFields(ddlEmailAddress, aFields);
                PopulateFormFields(ddlUserId, aFields);
                PopulateFormFields(ddlSubmitter, aFields);
                PopulateFormFields(ddlDocumentCredit, aFields);
            }
            catch (Exception ex)
            {
                string strMessage = "Failed populating form fields.";
                ScmsEvent.Raise(strMessage, this, ex);
            }
        }


        protected void PopulateFormFields(DropDownList ddlFields, scms.data.scms_form_field[] aFields)
        {
            ddlFields.Items.Clear();
            ddlFields.Items.Add("");
            ddlFields.AppendDataBoundItems = true;
            ddlFields.DataTextField = "name";
            ddlFields.DataValueField = "id";
            ddlFields.DataSource = aFields;
            ddlFields.DataBind();
        }

        public bool LoadSettings()
        {
            bool bSuccess = false;

            try
            {
                PopulateFormFields();

                if (EventHandlerId.HasValue)
                {
                    scms.data.ScmsDataContext dc = new scms.data.ScmsDataContext();
                    scms.data.scms_form_eventhandler eventHandler =
                        (from eh in dc.scms_form_eventhandlers
                         where eh.id == EventHandlerId.Value
                         select eh).FirstOrDefault();

                    scms.data.scms_submission_form_eventhandler submissionFormEventHandler =
                        (from sfeh in dc.scms_submission_form_eventhandlers
                         where sfeh.eventHandlerId == EventHandlerId.Value
                         select sfeh).FirstOrDefault();

                    if (submissionFormEventHandler != null)
                    {
                        scms.data.scms_submission_module submissionModule =
                           (from sm in dc.scms_submission_modules
                            where sm.id == submissionFormEventHandler.submissionModuleId
                            select sm).FirstOrDefault();

                        EnableControls(submissionModule);

                        moduleSelector.SiteId = SiteId;
                        moduleSelector.ModuleInstanceId = submissionModule.instanceId;

                        ddlTitle.ClearSelection();
                        if (submissionFormEventHandler.titleFieldId.HasValue)
                        {
                            ddlTitle.SelectedValue = submissionFormEventHandler.titleFieldId.ToString();
                        }

                        ddlLink.ClearSelection();
                        if (submissionFormEventHandler.linkFieldId.HasValue)
                        {
                            ddlLink.SelectedValue = submissionFormEventHandler.linkFieldId.ToString();
                        }

                        ddlImage.ClearSelection();
                        if (submissionFormEventHandler.imageFieldId.HasValue)
                        {
                            ddlImage.SelectedValue = submissionFormEventHandler.imageFieldId.ToString();
                        }

                        ddlVideo.ClearSelection();
                        if (submissionFormEventHandler.videoFieldId.HasValue)
                        {
                            ddlVideo.SelectedValue = submissionFormEventHandler.videoFieldId.ToString();
                        }

                        ddlDescription.ClearSelection();
                        if (submissionFormEventHandler.descriptionFieldId.HasValue)
                        {
                            ddlDescription.SelectedValue = submissionFormEventHandler.descriptionFieldId.ToString();
                        }

                        ddlEmailAddress.ClearSelection();
                        if (submissionFormEventHandler.emailAddressFieldId.HasValue)
                        {
                            ddlEmailAddress.SelectedValue = submissionFormEventHandler.emailAddressFieldId.ToString();
                        }

                        ddlUserId.ClearSelection();
                        if (submissionFormEventHandler.userIdFieldId.HasValue)
                        {
                            ddlUserId.SelectedValue = submissionFormEventHandler.userIdFieldId.ToString();
                        }

                        ddlSubmitter.ClearSelection();
                        if (submissionFormEventHandler.submitterFieldId.HasValue)
                        {
                            ddlSubmitter.SelectedValue = submissionFormEventHandler.submitterFieldId.ToString();
                        }

                        ddlDocumentCredit.ClearSelection();
                        if (submissionFormEventHandler.documentCreditFieldId.HasValue)
                        {
                            ddlDocumentCredit.SelectedValue = submissionFormEventHandler.documentCreditFieldId.ToString();
                        }


                    }

                    bSuccess = true;
                }
            }
            catch (Exception ex)
            {
                ScmsEvent.Raise("Exception thrown while loading settings", this, ex);
            }

            return bSuccess;
        }

        protected void EnableControls(bool bEnabled, bool bRequired, Label label, Label labelFormField, DropDownList ddl, RequiredFieldValidator rfv)
        {
            label.Enabled = bEnabled;
            ddl.Enabled = bEnabled;
            rfv.Enabled = bEnabled && bRequired;
        }

        protected void EnableControls(scms.data.scms_submission_module submissionModule)
        {
            EnableControls(
                (submissionModule != null) && (submissionModule.titleEnabled),
                (submissionModule != null) && (submissionModule.titleRequired.HasValue && submissionModule.titleRequired.Value),
                labelTitle,
                labelTitleFormField,
                ddlTitle,
                rfvTitle);

            EnableControls(
                (submissionModule != null) && (submissionModule.linkEnabled),
                (submissionModule != null) && (submissionModule.linkRequired.HasValue && submissionModule.linkRequired.Value),
                labelLink,
                labelLinkFormField,
                ddlLink,
                rfvLink);

            EnableControls(
                (submissionModule != null) && (submissionModule.imageEnabled),
                (submissionModule != null) && (submissionModule.imageRequired.HasValue && submissionModule.imageRequired.Value),
                labelImage,
                labelImageFormField,
                ddlImage,
                rfvImage);

            EnableControls(
                (submissionModule != null) && (submissionModule.videoEnabled),
                (submissionModule != null) && (submissionModule.videoRequired.HasValue && submissionModule.videoRequired.Value),
                labelVideo,
                labelVideoFormField,
                ddlVideo,
                rfvVideo);

            EnableControls(
                (submissionModule != null) && (submissionModule.descriptionEnabled),
                (submissionModule != null) && (submissionModule.descriptionRequired.HasValue && submissionModule.descriptionRequired.Value),
                labelDescription,
                labelDescriptionFormField,
                ddlDescription,
                rfvDescription);

            EnableControls(
                (submissionModule != null) && (submissionModule.emailAddressEnabled),
                (submissionModule != null) && (submissionModule.emailAddressRequired.HasValue && submissionModule.emailAddressRequired.Value),
                labelEmailAddress,
                labelEmailAddressFormField,
                ddlEmailAddress,
                rfvEmailAddress);

            EnableControls(
                (submissionModule != null) && (submissionModule.userIdEnabled),
                (submissionModule != null) && (submissionModule.userIdRequired.HasValue && submissionModule.userIdRequired.Value),
                labelUserId,
                labelUserIdFormField,
                ddlUserId,
                rfvUserId);

            EnableControls(
                (submissionModule != null) && (submissionModule.submitterEnabled),
                (submissionModule != null) && (submissionModule.submitterRequired.HasValue && submissionModule.submitterRequired.Value),
                labelSubmitter,
                labelSubmitterFormField,
                ddlSubmitter,
                rfvSubmitter);

            EnableControls(
                (submissionModule != null) && (submissionModule.documentCreditEnabled),
                (submissionModule != null) && (submissionModule.documentCreditRequired.HasValue && submissionModule.documentCreditRequired.Value),
                labelDocumentCredit,
                labelDocumentCreditFormField,
                ddlDocumentCredit,
                rfvDocumentCredit);

            /*
            EnableControls(checkTitleEnabled, checkTitleRequired, labelTitleFormField, ddlTitle, rfvTitle, labelTitleCssClass, txtTitleCssClass);
            EnableControls(checkImageEnabled, checkImageRequired, labelImageFormField, ddlImage, rfvImage, labelImageCssClass, txtImageCssClass);
            EnableControls(checkVideoEnabled, checkVideoRequired, labelVideoFormField, ddlVideo, rfvVideo, labelVideoCssClass, txtVideoCssClass);
            EnableControls(checkDescriptionEnabled, checkDescriptionRequired, labelDescriptionFormField, ddlDescription, rfvDescription, labelDescriptionCssClass, txtDescriptionCssClass);
            EnableControls(checkEmailAddressEnabled, checkEmailAddressRequired, labelEmailAddressFormField, ddlEmailAddress, rfvEmailAddress, labelEmailAddressCssClass, txtEmailAddressCssClass);
            EnableControls(checkUserIdEnabled, checkUserIdRequired, labelUserIdFormField, ddlUserId, rfvUserId, labelUserIdCssClass, txtUserIdCssClass);
           * */
        }

        protected void btnSaveSettings_Clicked(object sender, EventArgs args)
        {
            bool bSuccess = false;

            try
            {
                scms.data.ScmsDataContext dc = new scms.data.ScmsDataContext();

                if (EventHandlerId.HasValue)
                {
                    scms.data.scms_form_eventhandler eventHandler =
                        (from eh in dc.scms_form_eventhandlers
                         where eh.id == EventHandlerId.Value
                         select eh).FirstOrDefault();

                    scms.data.scms_submission_form_eventhandler submissionFormEventHandler =
                        (from sfeh in dc.scms_submission_form_eventhandlers
                         where sfeh.eventHandlerId == EventHandlerId.Value
                         select sfeh).FirstOrDefault();

                    if (submissionFormEventHandler == null)
                    {
                        submissionFormEventHandler = new scms.data.scms_submission_form_eventhandler();
                        submissionFormEventHandler.formId = FormId.Value;
                        submissionFormEventHandler.eventHandlerId = EventHandlerId.Value;
                        dc.scms_submission_form_eventhandlers.InsertOnSubmit(submissionFormEventHandler);
                    }

                    scms.data.scms_submission_module submissionModule =
                        (from sm in dc.scms_submission_modules
                         where sm.instanceId == moduleSelector.ModuleInstanceId.Value
                         select sm).Single();
                    submissionFormEventHandler.submissionModuleId = submissionModule.id;


                    submissionFormEventHandler.titleFieldId = null;
                    string strTitleFieldId = ddlTitle.SelectedValue;
                    if (!string.IsNullOrEmpty(strTitleFieldId))
                    {
                        submissionFormEventHandler.titleFieldId = int.Parse(strTitleFieldId);
                    }

                    submissionFormEventHandler.linkFieldId = null;
                    string strLinkFieldId = ddlLink.SelectedValue;
                    if (!string.IsNullOrEmpty(strLinkFieldId))
                    {
                        submissionFormEventHandler.linkFieldId = int.Parse(strLinkFieldId);
                    }

                    submissionFormEventHandler.imageFieldId = null;
                    string strImageFieldId = ddlImage.SelectedValue;
                    if (!string.IsNullOrEmpty(strImageFieldId))
                    {
                        submissionFormEventHandler.imageFieldId = int.Parse(strImageFieldId);
                    }

                    submissionFormEventHandler.videoFieldId = null;
                    string strVideoFieldId = ddlVideo.SelectedValue;
                    if (!string.IsNullOrEmpty(strVideoFieldId))
                    {
                        submissionFormEventHandler.videoFieldId = int.Parse(strVideoFieldId);
                    }

                    submissionFormEventHandler.descriptionFieldId = null;
                    string strDescriptionFieldId = ddlDescription.SelectedValue;
                    if (!string.IsNullOrEmpty(strDescriptionFieldId))
                    {
                        submissionFormEventHandler.descriptionFieldId = int.Parse(strDescriptionFieldId);
                    }

                    submissionFormEventHandler.emailAddressFieldId = null;
                    string strEmailAddressFieldId = ddlEmailAddress.SelectedValue;
                    if (!string.IsNullOrEmpty(strEmailAddressFieldId))
                    {
                        submissionFormEventHandler.emailAddressFieldId = int.Parse(strEmailAddressFieldId);
                    }

                    submissionFormEventHandler.userIdFieldId = null;
                    string strUserIdFieldId = ddlUserId.SelectedValue;
                    if (!string.IsNullOrEmpty(strUserIdFieldId))
                    {
                        submissionFormEventHandler.userIdFieldId = int.Parse(strUserIdFieldId);
                    }

                    submissionFormEventHandler.submitterFieldId = null;
                    string strSubmitterFieldId = ddlSubmitter.SelectedValue;
                    if (!string.IsNullOrEmpty(strSubmitterFieldId))
                    {
                        submissionFormEventHandler.submitterFieldId = int.Parse(strSubmitterFieldId);
                    }

                    submissionFormEventHandler.documentCreditFieldId = null;
                    string strDocumentCreditFieldId = ddlDocumentCredit.SelectedValue;
                    if (!string.IsNullOrEmpty(strDocumentCreditFieldId))
                    {
                        submissionFormEventHandler.documentCreditFieldId = int.Parse(strDocumentCreditFieldId);
                    }

                    dc.SubmitChanges();


                    bSuccess = true;

                    if (settingsSavedDelegate != null)
                    {
                        settingsSavedDelegate();
                    }
                }


            }
            catch (Exception ex)
            {
                ScmsEvent.Raise("Exception thrown while loading settings", this, ex);
            }
        }

        protected void btnCancel_Clicked(object sender, EventArgs args)
        {
            if (settingsCancelledDelegate != null)
            {
                settingsCancelledDelegate();
            }
        }
    }
}