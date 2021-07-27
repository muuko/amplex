using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace scms.modules.submission.submission
{
    public partial class edit : global::scms.RootControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            imageVotingUrlUp.SiteId = SiteId;
            imageVotingUrlDown.SiteId = SiteId;
            imageVotingActiveImageUrl.SiteId = SiteId;
            imageVotingInActiveImageUrl.SiteId = SiteId;
            imageVotingEvenImageUrl.SiteId = SiteId;

            if (!IsPostBack)
            {
                string strEditSubmissionId = Request.QueryString["sub"];
                if (!string.IsNullOrEmpty(strEditSubmissionId))
                {
                    int nSubmissionId = int.Parse(strEditSubmissionId);
                    LoadEditSubmission(nSubmissionId);

                    multiView.SetActiveView(viewEditSubmission);
                }
                else
                {
                    LoadModule();
                    EnableControls();
                }
            }
        }

        protected void LoadEditSubmission(int nSubmissionId)
        {
            scms.data.ScmsDataContext dc = new scms.data.ScmsDataContext();
            scms.data.scms_submission_module submissionModule = (from sm in dc.scms_submission_modules
                                                                 where sm.instanceId == ModuleInstanceId.Value
                                                                 select sm).FirstOrDefault();
            editSubmission.SiteId = this.SiteId;
            editSubmission.SubmissionModuleId = submissionModule.id;
            editSubmission.SubmissionId = nSubmissionId;
            editSubmission.LoadSettings();
        }

        protected void LoadModule()
        {
            try
            {
                if (ModuleInstanceId.HasValue)
                {
                    scms.data.ScmsDataContext dc = new scms.data.ScmsDataContext();
                    scms.data.scms_submission_module submissionModule = (from sm in dc.scms_submission_modules
                                                                         where sm.instanceId == ModuleInstanceId.Value
                                                                         select sm).FirstOrDefault();
                    if (submissionModule == null)
                    {
                        submissionModule = new scms.data.scms_submission_module();
                        submissionModule.instanceId = ModuleInstanceId.Value;
                        dc.scms_submission_modules.InsertOnSubmit(submissionModule);
                        dc.SubmitChanges();
                    }

                    checkAutoApproveSubmissions.Checked = submissionModule.autoApproveSubmission;
                    checkAutoFeatureSubmissions.Checked = submissionModule.autoFeatureSubmission;

                    // comments
                    checkCommentsEnabled.Checked = submissionModule.commentsEnabled;
                    if (submissionModule.commentsEnabled)
                    {
                        checkCommentsAutoApprove.Checked = submissionModule.commentsAutoApprove.HasValue && submissionModule.commentsAutoApprove.Value;
                        checkCommentsAuthRequired.Checked = submissionModule.commentsAuthenticationRequired.HasValue && submissionModule.commentsAuthenticationRequired.Value;
                    }

                    // voting
                    checkVotingEnabled.Checked = submissionModule.votingEnabled;
                    if (submissionModule.votingEnabled)
                    {
                        checkVotingAuthRequired.Checked = submissionModule.votingAuthenticationRequired.HasValue && submissionModule.votingAuthenticationRequired.Value;

                        ddlVotingMethod.ClearSelection();
                        ListItem liSelected = null;
                        if (!string.IsNullOrEmpty(submissionModule.votingMethod))
                        {
                            liSelected = ddlVotingMethod.Items.FindByValue(submissionModule.votingMethod);
                            if (liSelected != null)
                            {
                                liSelected.Selected = true;
                            }
                        }

                        imageVotingUrlUp.Path = submissionModule.votingUpImageUrl;
                        imageVotingUrlDown.Path = submissionModule.votingDownImageUrl;
                        txtVotingUpText.Text = submissionModule.votingUpText;
                        txtVotingDownText.Text = submissionModule.votingDownText;

                        imageVotingActiveImageUrl.Path = submissionModule.votingActiveImageUrl;
                        imageVotingInActiveImageUrl.Path = submissionModule.votingInActiveImageUrl;
                        imageVotingEvenImageUrl.Path = submissionModule.votingEvenImageUrl;
                        txtVotingSelectText.Text = submissionModule.votingSelectText;
                    }


                    // fields
                    checkTitleEnabled.Checked = submissionModule.titleEnabled;
                    if (submissionModule.titleEnabled)
                    {
                        checkTitleRequired.Checked = submissionModule.titleRequired.HasValue && submissionModule.titleRequired.Value;
                        txtTitleCssClass.Text = submissionModule.titleCssClass;
                    }
                    else
                    {
                        checkTitleRequired.Checked = false;
                        txtTitleCssClass.Text = null;
                    }

                    checkLinkEnabled.Checked = submissionModule.linkEnabled;
                    if (submissionModule.linkEnabled)
                    {
                        checkLinkRequired.Checked = submissionModule.linkRequired.HasValue && submissionModule.linkRequired.Value;
                        txtLinkCssClass.Text = submissionModule.linkCssClass;
                        txtLinkText.Text = submissionModule.linkText;
                    }
                    else
                    {
                        checkLinkRequired.Checked = false;
                        txtLinkCssClass.Text = null;
                        txtLinkText.Text = null;
                    }

                    checkImageEnabled.Checked = submissionModule.imageEnabled;
                    if (submissionModule.imageEnabled)
                    {
                        checkImageRequired.Checked = submissionModule.imageRequired.HasValue && submissionModule.imageRequired.Value;
                        txtImageCssClass.Text = submissionModule.imageCssClass;
                        txtImageWidth.Text = submissionModule.imageWidth.ToString();
                        txtImageHeight.Text = submissionModule.imageHeight.ToString();
                    }
                    else
                    {
                        submissionModule.imageRequired = null;
                        checkImageRequired.Checked = false;
                        txtImageWidth.Text = null;
                        txtImageHeight.Text = null;
                    }

                    checkVideoEnabled.Checked = submissionModule.videoEnabled;
                    if (submissionModule.videoEnabled)
                    {
                        checkVideoRequired.Checked = submissionModule.videoRequired.HasValue && submissionModule.videoRequired.Value;
                        txtVideoCssClass.Text = submissionModule.videoCssClass;
                    }
                    else
                    {
                        submissionModule.videoRequired = null;
                        checkVideoRequired.Checked = false;
                    }

                    checkDescriptionEnabled.Checked = submissionModule.descriptionEnabled;
                    if (submissionModule.descriptionEnabled)
                    {
                        checkDescriptionRequired.Checked = submissionModule.descriptionRequired.HasValue && submissionModule.descriptionRequired.Value;
                        txtDescriptionCssClass.Text = submissionModule.descriptionCssClass;
                    }
                    else
                    {
                        submissionModule.descriptionRequired = null;
                        checkDescriptionRequired.Checked = false;
                    }

                    checkEmailAddressEnabled.Checked = submissionModule.emailAddressEnabled;
                    if (submissionModule.emailAddressEnabled)
                    {
                        checkEmailAddressRequired.Checked = submissionModule.emailAddressRequired.HasValue && submissionModule.emailAddressRequired.Value;
                        txtEmailAddressCssClass.Text = submissionModule.emailAddressCssClass;
                    }
                    else
                    {
                        submissionModule.emailAddressRequired = null;
                        checkEmailAddressRequired.Checked = false;
                    }

                    checkUserIdEnabled.Checked = submissionModule.userIdEnabled;
                    if (submissionModule.userIdEnabled)
                    {
                        checkUserIdRequired.Checked = submissionModule.userIdRequired.HasValue && submissionModule.userIdRequired.Value;
                        txtUserIdCssClass.Text = submissionModule.userIdCssClass;
                    }
                    else
                    {
                        submissionModule.userIdRequired = null;
                        checkUserIdRequired.Checked = false;
                    }

                    checkSubmitterEnabled.Checked = submissionModule.submitterEnabled;
                    if (submissionModule.submitterEnabled)
                    {
                        checkSubmitterRequired.Checked = submissionModule.submitterRequired.HasValue && submissionModule.submitterRequired.Value;
                        txtSubmitterCssClass.Text = submissionModule.submitterCssClass;
                    }
                    else
                    {
                        submissionModule.submitterRequired = null;
                        checkSubmitterRequired.Checked = false;
                    }

                    checkDocumentCreditEnabled.Checked = submissionModule.documentCreditEnabled;
                    if (submissionModule.documentCreditEnabled)
                    {
                        checkDocumentCreditRequired.Checked = submissionModule.documentCreditRequired.HasValue && submissionModule.documentCreditRequired.Value;
                        txtDocumentCreditCssClass.Text = submissionModule.documentCreditCssClass;
                    }
                    else
                    {
                        submissionModule.documentCreditRequired = null;
                        checkDocumentCreditRequired.Checked = false;
                    }
                }
            }
            catch (Exception ex)
            {
                string strMessage = string.Format("Failed ensuring module for instance id '{0}'.", ModuleInstanceId);
                ScmsEvent.Raise(strMessage, this, ex);
            }
        }

        protected void checkCommentsEnabled_CheckedChanged(object sender, EventArgs args)
        {
            EnableControls();
        }

        protected void ddlVotingMethod_SelectedIndexChanged(object sender, EventArgs args)
        {
            EnableControls();
        }

        protected void EnableControls()
        {
            bool bCommentsEnabled = checkCommentsEnabled.Checked;
            labelCommentsAuthRequired.Enabled = bCommentsEnabled;
            checkCommentsAuthRequired.Enabled = bCommentsEnabled;
            labelCommentsAutoApprove.Enabled = bCommentsEnabled;
            checkCommentsAutoApprove.Enabled = bCommentsEnabled;

            bool bVotingEnabled = checkVotingEnabled.Checked;
            labelVotingAuthRequired.Enabled = bVotingEnabled;
            checkVotingAuthRequired.Enabled = bVotingEnabled;
            labelVotingMethod.Enabled = bVotingEnabled;
            ddlVotingMethod.Enabled = bVotingEnabled;

            bool bVotingUpDownEnabled = bVotingEnabled && (string.Compare(ddlVotingMethod.SelectedValue, "updown", true) == 0);
            labelVotingUpImageUrl.Enabled = bVotingUpDownEnabled;
            imageVotingUrlUp.Enabled = bVotingUpDownEnabled;
            labelVotingDownImageUrl.Enabled = bVotingUpDownEnabled;
            imageVotingUrlDown.Enabled = bVotingUpDownEnabled;
            labelVotingUpText.Enabled = bVotingUpDownEnabled;
            txtVotingUpText.Enabled = bVotingUpDownEnabled;
            labelVotingDownText.Enabled = bVotingUpDownEnabled;
            txtVotingDownText.Enabled = bVotingUpDownEnabled;

            bool bVotingFiveUpEnabled = bVotingEnabled && (string.Compare(ddlVotingMethod.SelectedValue, "fiveup", true) == 0);
            labelVotingActiveImageUrl.Enabled = bVotingFiveUpEnabled;
            imageVotingActiveImageUrl.Enabled = bVotingFiveUpEnabled;
            labelVotingInActiveImageUrl.Enabled = bVotingFiveUpEnabled;
            imageVotingInActiveImageUrl.Enabled = bVotingFiveUpEnabled;
            labelVotingEvenImageUrl.Enabled = bVotingFiveUpEnabled;
            imageVotingEvenImageUrl.Enabled = bVotingFiveUpEnabled;
            labelVotingSelectText.Enabled = bVotingFiveUpEnabled;
            txtVotingSelectText.Enabled = bVotingFiveUpEnabled;


            bool bTitleEnabled = checkTitleEnabled.Checked;
            checkTitleRequired.Enabled = bTitleEnabled;
            labelTitleCssClass.Enabled = bTitleEnabled;
            txtTitleCssClass.Enabled = bTitleEnabled;

            bool bLinkEnabled = checkLinkEnabled.Checked;
            checkLinkRequired.Enabled = bLinkEnabled;
            labelLinkCssClass.Enabled = bLinkEnabled;
            txtLinkCssClass.Enabled = bLinkEnabled;
            labelLinkText.Enabled = bLinkEnabled;
            txtLinkText.Enabled = bLinkEnabled;

            bool bImageEnabled = checkImageEnabled.Checked;
            checkImageRequired.Enabled = bImageEnabled;
            labelImageCssClass.Enabled = bImageEnabled;
            txtImageCssClass.Enabled = bImageEnabled;
            txtImageWidth.Enabled = bImageEnabled;
            labelImageWidth.Enabled = bImageEnabled;
            txtImageHeight.Enabled = bImageEnabled;
            labelImageHeight.Enabled = bImageEnabled;
            rvImageWidth.Enabled = bImageEnabled;
            rfvImageWidth.Enabled = bImageEnabled;
            rvImageHeight.Enabled = bImageEnabled;
            rfvImageHeight.Enabled = bImageEnabled;

            bool bVideoEnabled = checkVideoEnabled.Checked;
            checkVideoRequired.Enabled = bVideoEnabled;
            labelVideoCssClass.Enabled = bVideoEnabled;
            txtVideoCssClass.Enabled = bVideoEnabled;

            bool bDescriptionEnabled = checkDescriptionEnabled.Checked;
            checkDescriptionRequired.Enabled = bDescriptionEnabled;
            labelDescriptionCssClass.Enabled = bDescriptionEnabled;
            txtDescriptionCssClass.Enabled = bDescriptionEnabled;

            bool bEmailAddressEnabled = checkEmailAddressEnabled.Checked;
            checkEmailAddressRequired.Enabled = bEmailAddressEnabled;
            labelEmailAddressCssClass.Enabled = bEmailAddressEnabled;
            txtEmailAddressCssClass.Enabled = bEmailAddressEnabled;

            bool bUserIdEnabled = checkUserIdEnabled.Checked;
            checkUserIdRequired.Enabled = bUserIdEnabled;
            labelUserIdCssClass.Enabled = bUserIdEnabled;
            txtUserIdCssClass.Enabled = bUserIdEnabled;

            bool bSubmitterEnabled = checkSubmitterEnabled.Checked;
            checkSubmitterRequired.Enabled = bSubmitterEnabled;
            labelSubmitterCssClass.Enabled = bSubmitterEnabled;
            txtSubmitterCssClass.Enabled = bSubmitterEnabled;

            bool bDocumentCreditEnabled = checkDocumentCreditEnabled.Checked;
            checkDocumentCreditRequired.Enabled = bDocumentCreditEnabled;
            labelDocumentCreditCssClass.Enabled = bDocumentCreditEnabled;
            txtDocumentCreditCssClass.Enabled = bDocumentCreditEnabled;
        }

        protected void checkVotingEnabled_CheckedChanged(object sender, EventArgs args)
        {
            EnableControls();
        }

        protected void CheckedChanged(object sender, EventArgs args)
        {
            EnableControls();
        }


        protected void btnSave_Click(object sender, EventArgs args)
        {
            try
            {
                scms.data.ScmsDataContext dc = new scms.data.ScmsDataContext();
                scms.data.scms_submission_module submissionModule = (from sm in dc.scms_submission_modules
                                                                     where sm.instanceId == ModuleInstanceId.Value
                                                                     select sm).FirstOrDefault();
                if (submissionModule == null)
                {
                    submissionModule = new scms.data.scms_submission_module();
                    submissionModule.instanceId = ModuleInstanceId.Value;
                    dc.scms_submission_modules.InsertOnSubmit(submissionModule);
                }

                submissionModule.autoApproveSubmission = checkAutoApproveSubmissions.Checked;
                submissionModule.autoFeatureSubmission = checkAutoFeatureSubmissions.Checked;


                // comments
                submissionModule.commentsEnabled = checkCommentsEnabled.Checked;
                if (submissionModule.commentsEnabled)
                {
                    submissionModule.commentsAutoApprove = checkCommentsAutoApprove.Checked;
                    submissionModule.commentsAuthenticationRequired = checkCommentsAuthRequired.Checked;
                }
                else
                {
                    submissionModule.commentsAutoApprove = null;
                    submissionModule.commentsAuthenticationRequired = null;
                }

                // voting
                submissionModule.votingEnabled = checkVotingEnabled.Checked;
                submissionModule.votingAuthenticationRequired = null;
                submissionModule.votingMethod = null;
                submissionModule.votingUpImageUrl = null;
                submissionModule.votingDownImageUrl = null;
                submissionModule.votingUpText = null;
                submissionModule.votingDownText = null;
                submissionModule.votingActiveImageUrl = null;
                submissionModule.votingInActiveImageUrl = null;
                submissionModule.votingEvenImageUrl = null;
                submissionModule.votingSelectText = null;

                if (submissionModule.votingEnabled)
                {
                    submissionModule.votingAuthenticationRequired = checkVotingAuthRequired.Checked;
                    submissionModule.votingMethod = ddlVotingMethod.SelectedValue.ToLower();

                    switch (submissionModule.votingMethod)
                    {
                        case "updown":
                            submissionModule.votingUpImageUrl = imageVotingUrlUp.Path;
                            submissionModule.votingDownImageUrl = imageVotingUrlDown.Path;
                            submissionModule.votingUpText = txtVotingUpText.Text;
                            submissionModule.votingDownText = txtVotingDownText.Text;
                            break;

                        case "fiveup":
                            submissionModule.votingUpImageUrl = imageVotingUrlUp.Path;
                            submissionModule.votingDownImageUrl = imageVotingUrlDown.Path;
                            submissionModule.votingActiveImageUrl = imageVotingActiveImageUrl.Path;
                            submissionModule.votingInActiveImageUrl = imageVotingInActiveImageUrl.Path;
                            submissionModule.votingEvenImageUrl = imageVotingEvenImageUrl.Path;
                            submissionModule.votingSelectText = txtVotingSelectText.Text;
                            break;
                    }
                }
                else
                {

                }

                // fields
                submissionModule.titleEnabled = checkTitleEnabled.Checked;
                if (submissionModule.titleEnabled)
                {
                    submissionModule.titleRequired = checkTitleRequired.Checked;
                    submissionModule.titleCssClass = txtTitleCssClass.Text;
                }
                else
                {
                    submissionModule.titleRequired = null;
                    submissionModule.titleCssClass = null;
                }

                submissionModule.linkEnabled = checkLinkEnabled.Checked;
                if (submissionModule.linkEnabled)
                {
                    submissionModule.linkRequired = checkLinkRequired.Checked; ;
                    submissionModule.linkCssClass = txtLinkCssClass.Text;
                    submissionModule.linkText = txtLinkText.Text;
                }
                else
                {
                    submissionModule.linkRequired = null;
                    submissionModule.linkCssClass = null;
                    submissionModule.linkText = null;
                }

                submissionModule.imageEnabled = checkImageEnabled.Checked;
                if (submissionModule.imageEnabled)
                {
                    submissionModule.imageRequired = checkImageRequired.Checked;
                    submissionModule.imageCssClass = txtImageCssClass.Text;
                    submissionModule.imageHeight = null;
                    if (!string.IsNullOrEmpty(txtImageHeight.Text))
                    {
                        submissionModule.imageHeight = int.Parse(txtImageHeight.Text);
                    }

                    submissionModule.imageWidth = null;
                    if (!string.IsNullOrEmpty(txtImageWidth.Text))
                    {
                        submissionModule.imageWidth = int.Parse(txtImageWidth.Text);
                    }
                }
                else
                {
                    submissionModule.imageRequired = null;
                    submissionModule.imageCssClass = null;
                    submissionModule.imageWidth = null;
                    submissionModule.imageHeight = null;
                }

                submissionModule.videoEnabled = checkVideoEnabled.Checked;
                if (submissionModule.videoEnabled)
                {
                    submissionModule.videoRequired = checkVideoRequired.Checked;
                    submissionModule.videoCssClass = txtVideoCssClass.Text;
                }
                else
                {
                    submissionModule.videoRequired = null;
                    submissionModule.videoCssClass = null;
                }

                submissionModule.descriptionEnabled = checkDescriptionEnabled.Checked;
                if (submissionModule.descriptionEnabled)
                {
                    submissionModule.descriptionRequired = checkDescriptionRequired.Checked;
                    submissionModule.descriptionCssClass = txtDescriptionCssClass.Text;
                }
                else
                {
                    submissionModule.descriptionRequired = null;
                    submissionModule.descriptionCssClass = null;
                }

                submissionModule.emailAddressEnabled = checkEmailAddressEnabled.Checked;
                if (submissionModule.emailAddressEnabled)
                {
                    submissionModule.emailAddressRequired = checkEmailAddressRequired.Checked;
                    submissionModule.emailAddressCssClass = txtEmailAddressCssClass.Text;
                }
                else
                {
                    submissionModule.emailAddressRequired = null;
                    submissionModule.emailAddressCssClass = null;
                }

                submissionModule.userIdEnabled = checkUserIdEnabled.Checked;
                if (submissionModule.userIdEnabled)
                {
                    submissionModule.userIdRequired = checkUserIdRequired.Checked;
                    submissionModule.userIdCssClass = txtUserIdCssClass.Text;
                }
                else
                {
                    submissionModule.userIdRequired = null;
                    submissionModule.userIdCssClass = null;
                }


                submissionModule.submitterEnabled = checkSubmitterEnabled.Checked;
                if (submissionModule.submitterEnabled)
                {
                    submissionModule.submitterRequired = checkSubmitterRequired.Checked;
                    submissionModule.submitterCssClass = txtSubmitterCssClass.Text;
                }
                else
                {
                    submissionModule.submitterRequired = null;
                    submissionModule.submitterCssClass = null;
                }

                submissionModule.documentCreditEnabled = checkDocumentCreditEnabled.Checked;
                if (submissionModule.documentCreditEnabled)
                {
                    submissionModule.documentCreditRequired = checkDocumentCreditRequired.Checked;
                    submissionModule.documentCreditCssClass = txtDocumentCreditCssClass.Text;
                }
                else
                {
                    submissionModule.documentCreditRequired = null;
                    submissionModule.documentCreditCssClass = null;
                }


                dc.SubmitChanges();
                statusMessage.ShowSuccess("Settings saved");
            }
            catch (Exception ex)
            {
                ScmsEvent.Raise("Exception raised while saving settings", this, ex);
                statusMessage.ShowFailure("Failed saving settings");
            }
        }
    }
}