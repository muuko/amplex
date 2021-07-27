using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace scms.modules.submission.controls
{
    public partial class editSubmission : System.Web.UI.UserControl
    {
        public int? SiteId
        {
            get { return (int?)ViewState["SiteId"]; }
            set { ViewState["SiteId"] = value; }
        }

        public int? SubmissionModuleId
        {
            get { return (int?)ViewState["SubmissionModuleId"]; }
            set { ViewState["SubmissionModuleId"] = value; }
        }

        public int? SubmissionId
        {
            get { return (int?)ViewState["SubmissionId"]; }
            set { ViewState["SubmissionId"] = value; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            statusMessage.Clear();
            if (false && !IsPostBack)
            {
                int n;

                string strSiteId = Request.QueryString["scms_sid"];
                if (int.TryParse(strSiteId, out n))
                {
                    SiteId = n;
                }

                string strSubmissionModuleId = Request.QueryString["subm"];
                if (int.TryParse(strSubmissionModuleId, out n))
                {
                    SubmissionModuleId = n;
                }

                string strSubmissionId = Request.QueryString["sub"];
                if (int.TryParse(strSubmissionId, out n))
                {
                    SubmissionId = n;
                }

                LoadSettings();
            }
        }

        public void LoadSettings()
        {
            try
            {
                int? nSiteId = SiteId;
                int? nSubmissionModuleId = SubmissionModuleId;
                int? nSubmissionId = SubmissionId;


                literalId.Text = nSubmissionId.ToString();

                if (nSiteId.HasValue && nSubmissionModuleId.HasValue && SubmissionId.HasValue)
                {
                    global::scms.data.ScmsDataContext dc = new global::scms.data.ScmsDataContext();
                    var submissionModule = (from sm in dc.scms_submission_modules
                                            where sm.id == nSubmissionModuleId.Value
                                            select sm).FirstOrDefault();

                    var submission = (from s in dc.scms_submission_submissions
                                      where s.id == nSubmissionId.Value
                                      select s).FirstOrDefault();

                    literalVotingMethod.Text = submissionModule.votingMethod;

                    txtTitle.Text = submission.title;
                    txtDescription.Text = submission.description;
                    txtLink.Text = submission.linkUrl;

                    selectImage.Path = submission.imageUrl;
                    selectImage.SiteId = nSiteId.Value;

                    txtVideoUrl.Text = submission.videoUrl;
                    txtSubmitter.Text = submission.submitter;
                    txtDocumentCredit.Text = submission.documentCredit;

                    txtSubmittedOn.Text = submission.submissionTime.ToShortDateString();
                    txtSubmittedOnTime.Text = submission.submissionTime.ToString("HH:mm");

                    txtApprovedOn.Text = null;
                    if (submission.approvedTime.HasValue)
                    {
                        txtApprovedOn.Text = submission.approvedTime.Value.ToShortDateString();
                        txtApprovedOnTime.Text = submission.approvedTime.Value.ToString("HH:MM");
                    }

                    txtFeaturedOn.Text = null;
                    if (submission.featuredTime.HasValue)
                    {
                        txtFeaturedOn.Text = submission.featuredTime.Value.ToShortDateString();
                        txtFeaturedOnTime.Text = submission.featuredTime.Value.ToString("HH:MM");
                    }


                    literalVote.Text = submission.vote.ToString();
                    literalVotes.Text = submission.votes.ToString();
                    txtVotesUp.Text = submission.votesUp.ToString();
                    txtVotesDown.Text = submission.votesDown.ToString();
                    txtVotes1.Text = submission.votes1.ToString();
                    txtVotes2.Text = submission.votes2.ToString();
                    txtVotes3.Text = submission.votes3.ToString();
                    txtVotes4.Text = submission.votes4.ToString();
                    txtVotes5.Text = submission.votes5.ToString();
                    checkDeleted.Checked = submission.deleted;
                }
            }
            catch (Exception ex)
            {
                string strMessage = "Failed loading submission or submission module";
                statusMessage.ShowFailure(strMessage);
                global::scms.ScmsEvent.Raise(strMessage, this, ex);
            }
        }

        protected void btnSave_Click(object sender, EventArgs args)
        {
            try
            {
                int? nSiteId = SiteId;
                int? nSubmissionModuleId = SubmissionModuleId;
                int? nSubmissionId = SubmissionId;

                if (nSiteId.HasValue && nSubmissionModuleId.HasValue && SubmissionId.HasValue)
                {
                    global::scms.data.ScmsDataContext dc = new global::scms.data.ScmsDataContext();
                    var submissionModule = (from sm in dc.scms_submission_modules
                                            where sm.id == nSubmissionModuleId.Value
                                            select sm).FirstOrDefault();

                    var submission = (from s in dc.scms_submission_submissions
                                      where s.id == nSubmissionId.Value
                                      select s).FirstOrDefault();

                    txtTitle.Text = submission.title = txtTitle.Text;
                    submission.description = txtDescription.Text;
                    submission.linkUrl = txtLink.Text;
                    submission.imageUrl = selectImage.Path;
                    submission.videoUrl = txtVideoUrl.Text;
                    submission.submitter = txtSubmitter.Text;
                    submission.documentCredit = txtDocumentCredit.Text;

                    if (!string.IsNullOrEmpty(txtSubmittedOn.Text.Trim()))
                    {
                        DateTime dtSubmitted = DateTime.Parse(txtSubmittedOn.Text.Trim());

                        string strSubmittedOnTime = txtSubmittedOnTime.Text.Trim();
                        string[] astrSubmittedOnTimeComponents = strSubmittedOnTime.Split(new char[] { ':' });

                        if (astrSubmittedOnTimeComponents.Length > 0)
                        {
                            int nHours = 0;
                            nHours = int.Parse(astrSubmittedOnTimeComponents[0]);
                            dtSubmitted = dtSubmitted.AddHours(nHours);

                            if (astrSubmittedOnTimeComponents.Length > 1)
                            {
                                int nMinutes = 0;
                                nMinutes = int.Parse(astrSubmittedOnTimeComponents[1]);
                                dtSubmitted = dtSubmitted.AddMinutes(nMinutes);
                            }
                        }

                        submission.submissionTime = dtSubmitted;
                    }

                    submission.approvedTime = null;
                    if (!string.IsNullOrEmpty(txtApprovedOn.Text.Trim()))
                    {
                        DateTime dtApproved = DateTime.Parse(txtApprovedOn.Text.Trim());

                        string strApprovedOnTime = txtApprovedOnTime.Text.Trim();
                        string[] astrApprovedOnTimeComponents = strApprovedOnTime.Split(new char[] { ':' });

                        if (astrApprovedOnTimeComponents.Length > 0)
                        {
                            int nHours = 0;
                            nHours = int.Parse(astrApprovedOnTimeComponents[0]);
                            dtApproved = dtApproved.AddHours(nHours);

                            if (astrApprovedOnTimeComponents.Length > 1)
                            {
                                int nMinutes = 0;
                                nMinutes = int.Parse(astrApprovedOnTimeComponents[1]);
                                dtApproved = dtApproved.AddMinutes(nMinutes);
                            }
                        }

                        submission.approvedTime = dtApproved;
                    }

                    submission.featuredTime = null;
                    if (!string.IsNullOrEmpty(txtFeaturedOn.Text.Trim()))
                    {
                        DateTime dtFeatured = DateTime.Parse(txtFeaturedOn.Text.Trim());

                        string strFeaturedOnTime = txtFeaturedOnTime.Text.Trim();
                        string[] astrFeaturedOnTimeComponents = strFeaturedOnTime.Split(new char[] { ':' });

                        if (astrFeaturedOnTimeComponents.Length > 0)
                        {
                            int nHours = 0;
                            nHours = int.Parse(astrFeaturedOnTimeComponents[0]);
                            dtFeatured = dtFeatured.AddHours(nHours);

                            if (astrFeaturedOnTimeComponents.Length > 1)
                            {
                                int nMinutes = 0;
                                nMinutes = int.Parse(astrFeaturedOnTimeComponents[1]);
                                dtFeatured = dtFeatured.AddMinutes(nMinutes);
                            }
                        }

                        submission.featuredTime = dtFeatured;
                    }

                    submission.votesUp = 0;
                    string strVotesUp = txtVotesUp.Text.Trim();
                    if (!string.IsNullOrEmpty(strVotesUp))
                    {
                        submission.votesUp = int.Parse(strVotesUp);
                    }

                    submission.votesDown = 0;
                    string strVotesDown = txtVotesDown.Text.Trim();
                    if (!string.IsNullOrEmpty(strVotesDown))
                    {
                        submission.votesDown = int.Parse(strVotesDown);
                    }

                    submission.votes1 = 0;
                    string strVotes1 = txtVotes1.Text.Trim();
                    if (!string.IsNullOrEmpty(strVotes1))
                    {
                        submission.votes1 = int.Parse(strVotes1);
                    }

                    submission.votes2 = 0;
                    string strVotes2 = txtVotes2.Text.Trim();
                    if (!string.IsNullOrEmpty(strVotes2))
                    {
                        submission.votes2 = int.Parse(strVotes2);
                    }

                    submission.votes3 = 0;
                    string strVotes3 = txtVotes3.Text.Trim();
                    if (!string.IsNullOrEmpty(strVotes3))
                    {
                        submission.votes3 = int.Parse(strVotes3);
                    }

                    submission.votes4 = 0;
                    string strVotes4 = txtVotes4.Text.Trim();
                    if (!string.IsNullOrEmpty(strVotes4))
                    {
                        submission.votes4 = int.Parse(strVotes4);
                    }

                    submission.votes5 = 0;
                    string strVotes5 = txtVotes5.Text.Trim();
                    if (!string.IsNullOrEmpty(strVotes5))
                    {
                        submission.votes5 = int.Parse(strVotes5);
                    }

                    switch (submissionModule.votingMethod)
                    {
                        case "fiveup":
                            submission.votes = submission.votes1 +
                                submission.votes2 +
                                submission.votes3 +
                                submission.votes4 +
                                submission.votes5;

                            submission.vote = null;
                            if (submission.votes.Value > 0)
                            {
                                submission.vote = (decimal)(submission.votes1 +
                                    2 * submission.votes2 +
                                    3 * submission.votes3 +
                                    4 * submission.votes4 +
                                    5 * submission.votes5) / (decimal)submission.votes.Value;

                            }

                            literalVotes.Text = submission.votes.ToString();
                            literalVote.Text = submission.vote.ToString();
                            break;


                        case "updown":
                            submission.votes = submission.votesUp +
                                submission.votesDown;
                            literalVotes.Text = submission.votes.ToString();

                            submission.vote = null;
                            literalVote.Text = submission.vote.ToString();
                            break;

                        default:
                            throw new Exception("unknown voting method");
                    }

                    submission.deleted = checkDeleted.Checked;
                    dc.SubmitChanges();
                }
                else
                {
                    throw new Exception("Unexpected missing id(s)");
                }

                statusMessage.ShowSuccess("Submission Updated");
            }
            catch (Exception ex)
            {
                string strMessage = "Failed saving submission";
                statusMessage.ShowFailure(strMessage);
                global::scms.ScmsEvent.Raise(strMessage, this, ex);
            }
        }

        protected void btnCancel_Click(object sender, EventArgs args)
        {
            try
            {
                global::scms.data.ScmsDataContext dc = new global::scms.data.ScmsDataContext();
                var page = (from p in dc.scms_pages
                            join pm in dc.scms_page_plugin_modules on p.id equals pm.pageId
                            join sb in dc.scms_submission_modules on pm.instanceId equals sb.instanceId
                            where sb.id == SubmissionModuleId.Value
                            where pm.owner == true
                            where pm.deleted == false
                            select p).FirstOrDefault();

                string strUrl = "~";
                if (page != null)
                {
                    strUrl = string.Format("{0}?sub={1}", page.url, SubmissionId);
                }
                strUrl = ResolveUrl(strUrl);
                Response.Redirect(strUrl);
            }
            catch (System.Threading.ThreadAbortException)
            {
            }
            catch (Exception ex)
            {
                string strMessage = "Failed locating page";
                global::scms.ScmsEvent.Raise(strMessage, this, ex);
            }
        }

        protected void btnSubmittedOn_Click(object sender, EventArgs args)
        {
            txtSubmittedOn.Text = DateTime.Now.ToShortDateString();
            txtSubmittedOnTime.Text = DateTime.Now.ToString("HH:mm");
        }

        protected void btnApprovedNow_Click(object sender, EventArgs args)
        {
            txtApprovedOn.Text = DateTime.Now.ToShortDateString();
            txtApprovedOnTime.Text = DateTime.Now.ToString("HH:mm");
        }

        protected void btnFeaturedNow_Click(object sender, EventArgs args)
        {
            txtFeaturedOn.Text = DateTime.Now.ToShortDateString();
            txtFeaturedOnTime.Text = DateTime.Now.ToString("HH:mm");
        }

        protected void btnApprovedClear_Click(object sender, EventArgs args)
        {
            txtApprovedOn.Text = null;
            txtApprovedOnTime.Text = null;
        }

        protected void btnFeaturedClear_Click(object sender, EventArgs args)
        {
            txtFeaturedOn.Text = null;
            txtFeaturedOnTime.Text = null;
        }
    }
}