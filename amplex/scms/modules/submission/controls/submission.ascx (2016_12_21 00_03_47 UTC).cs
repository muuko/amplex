using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace scms.modules.submission.controls
{
    public partial class submission : System.Web.UI.UserControl
    {
        protected scms.data.scms_submission_module submissionModule = null;
        public scms.data.scms_submission_module SubmissionModule
        {
            get
            {
                return submissionModule;
            }

            set
            {
                submissionModule = value;
                if (submissionModule != null)
                {
                    SubmissionModuleId = submissionModule.id;
                }
                else
                {
                    SubmissionModuleId = null;
                }
            }
        }

        protected scms.data.scms_submission_submission scmsSubmission = null;
        public scms.data.scms_submission_submission Submission
        {
            get
            {
                return scmsSubmission;
            }

            set
            {
                scmsSubmission = value;
                if (scmsSubmission != null)
                {
                    SubmissionId = scmsSubmission.id;
                }
                else
                {
                    SubmissionId = null;
                }
            }

            /*
            get
            {
                return (scms.data.scms_submission_submission)ViewState["submission"];
            }

            set
            {
                ViewState["submission"] = value;
            }
             * */
        }

        public int? SubmissionModuleId
        {
            get { return (int?)ViewState["SubmissionModuleId"]; }

            set
            {
                ViewState["SubmissionModuleId"] = value;
            }
        }

        public int? SubmissionId
        {
            get { return (int?)ViewState["SubmissionId"]; }

            set
            {
                ViewState["SubmissionId"] = value;
            }
        }

        protected bool CanUserVote()
        {
            bool bUserCanVote = false;

            string strCookieKey = string.Format("submission[{0}]-votes", SubmissionModuleId);
            HttpCookie cookie = Request.Cookies[strCookieKey];
            if (cookie == null)
            {
                cookie = new HttpCookie(strCookieKey);
            }

            string strValue = cookie.Values[SubmissionId.ToString()];
            if (string.IsNullOrEmpty(strValue))
            {
                bUserCanVote = true;
                cookie.Values[SubmissionId.ToString()] = "1";
                cookie.Expires = DateTime.Now.AddYears(10);
                Response.Cookies.Add(cookie);
            }

            return bUserCanVote;
        }

        protected bool GetData(scms.data.ScmsDataContext dc, out scms.data.scms_submission_module submissionModule, out scms.data.scms_submission_submission submission)
        {
            bool bSuccess = false;

            submissionModule = null;
            submission = null;

            try
            {
                int nSubmissionId = SubmissionId.Value;

                if ((submissionModule == null) || (submission == null))
                {
                    var details = (from m in dc.scms_submission_modules
                                   where m.id == SubmissionModuleId.Value
                                   join s in dc.scms_submission_submissions on m.id equals s.submissionModuleId
                                   where s.id == nSubmissionId
                                   select new { submissionModule = m, submission = s }
                                  ).FirstOrDefault();
                    if (details != null)
                    {
                        submissionModule = details.submissionModule;
                        submission = details.submission;
                    }
                }

                if ((submissionModule != null) && (submission != null))
                {
                    bSuccess = true;
                }
            }
            catch (Exception ex)
            {
                string strMessage = "GetData failed looking up module / submission.";
                ScmsEvent.Raise(strMessage, this, ex);
            }


            return bSuccess;
        }


        protected void OnVoteFiveUpVote(int? nId, int nIndex)
        {
            try
            {
                scms.data.ScmsDataContext dc = new scms.data.ScmsDataContext();
                scms.data.scms_submission_module submissionModule = null;
                scms.data.scms_submission_submission submission = null;
                if (!GetData(dc, out submissionModule, out submission))
                    throw new Exception("GetData failed");

                if (!submissionModule.votingEnabled)
                    throw new Exception("Voting is not enabled");

                if (string.Compare(submissionModule.votingMethod, "fiveup", true) != 0)
                    throw new Exception("Unexpected voting method");

                if (submissionModule.votingAuthenticationRequired.HasValue && submissionModule.votingAuthenticationRequired.Value)
                {
                    if (!Page.User.Identity.IsAuthenticated)
                    {
                        scms.RootPage page = this.Page as scms.RootPage;

                        string strQueryString = string.Format("sv=1&svm={0}&svs={1}&svmode=fu&svi={2}", SubmissionModuleId, SubmissionId, nIndex);
                        string strRawUrl = Request.RawUrl;
                        if (strRawUrl.Contains('?'))
                        {
                            strRawUrl += "&";
                        }
                        else
                        {
                            strRawUrl += "?";
                        }
                        string strReturnUrl = string.Format("{0}{1}", strRawUrl, strQueryString);
                        page.RedirectToLogin(strReturnUrl);
                    }
                }

                if (CanUserVote())
                {
                    if (!submission.votes.HasValue)
                        submission.votes = 0;

                    if (!submission.votes1.HasValue)
                        submission.votes1 = 0;

                    if (!submission.votes2.HasValue)
                        submission.votes2 = 0;

                    if (!submission.votes3.HasValue)
                        submission.votes3 = 0;

                    if (!submission.votes4.HasValue)
                        submission.votes4 = 0;

                    if (!submission.votes5.HasValue)
                        submission.votes5 = 0;


                    submission.votes += 1;

                    switch (nIndex)
                    {
                        case 0:
                            submission.votes1++;
                            break;

                        case 1:
                            submission.votes2++;
                            break;

                        case 2:
                            submission.votes3++;
                            break;

                        case 3:
                            submission.votes4++;
                            break;

                        case 4:
                            submission.votes5++;
                            break;
                    }


                    CalcFiveUpVotes(ref submission);
                    dc.SubmitChanges();

                    Response.Redirect(Request.RawUrl);

                    /*
                    voting.FiveUp_Vote = submission.vote;
                    voting.FiveUp_VotesText = string.Format("{0} votes", submission.votes);
                    voting.DataBind();
                     */
                }
            }
            catch (System.Threading.ThreadAbortException)
            {
            }
            catch (Exception ex)
            {
                ScmsEvent.Raise("Failed registering up/down vote", this, ex);
            }
        }

        protected void CalcFiveUpVotes(ref scms.data.scms_submission_submission submission)
        {
            submission.votes = submission.votes1 + submission.votes2 + submission.votes3 + submission.votes4 + submission.votes5;

            if (submission.votes > 0)
            {
                submission.vote = (decimal)((float)(1 * submission.votes1 +
                             2 * submission.votes2 +
                             3 * submission.votes3 +
                             4 * submission.votes4 +
                             5 * submission.votes5) / (float)submission.votes);
            }
            else
            {
                submission.vote = 0;
            }
        }

        protected void OnVoteUpDownVote(int? nId, bool bUp)
        {
            try
            {
                scms.data.ScmsDataContext dc = new scms.data.ScmsDataContext();
                scms.data.scms_submission_module submissionModule = null;
                scms.data.scms_submission_submission submission = null;
                if (!GetData(dc, out submissionModule, out submission))
                    throw new Exception("GetData failed");

                if (!submissionModule.votingEnabled)
                    throw new Exception("Voting is not enabled");

                if (string.Compare(submissionModule.votingMethod, "updown", true) != 0)
                    throw new Exception("Unexpected voting method");

                if (submissionModule.votingAuthenticationRequired.HasValue && submissionModule.votingAuthenticationRequired.Value)
                {
                    if (!Page.User.Identity.IsAuthenticated)
                    {
                        scms.RootPage page = this.Page as scms.RootPage;

                        string strQueryString = string.Format("sv=1&svm={0}&svs={1}&svmode=ud&svv={2}", SubmissionModuleId, SubmissionId, bUp ? '1' : '0');
                        string strRawUrl = Request.RawUrl;
                        if (strRawUrl.Contains('?'))
                        {
                            strRawUrl += "&";
                        }
                        else
                        {
                            strRawUrl += "?";
                        }
                        string strReturnUrl = string.Format("{0}{1}", strRawUrl, strQueryString);
                        page.RedirectToLogin(strReturnUrl);
                    }
                }

                if (CanUserVote())
                {


                    if (!submission.votes.HasValue)
                        submission.votes = 0;
                    if (!submission.votesUp.HasValue)
                        submission.votesUp = 0;
                    if (!submission.votesDown.HasValue)
                        submission.votesDown = 0;


                    submission.votes += 1;

                    if (bUp)
                    {
                        submission.votesUp += 1;
                    }
                    else
                    {
                        submission.votesDown += 1;
                    }
                    dc.SubmitChanges();

                    Response.Redirect(Request.RawUrl);

                    /*
                    voting.VotesUp = submission.votesUp.Value;
                    voting.VotesDown = submission.votesDown.Value;
                    voting.NumberOfVotes = submission.votes.Value;
                    voting.DataBind();
                    */
                }
            }
            catch (System.Threading.ThreadAbortException)
            {
            }
            catch (Exception ex)
            {
                ScmsEvent.Raise("Failed registering up/down vote", this, ex);
            }
        }

        protected void ProcessParms()
        {
            if (Page.User.Identity.IsAuthenticated)
            {
                // for vote redirects due to forced logins
                string strRedirectVote = Request.QueryString["sv"];
                if (!string.IsNullOrEmpty(strRedirectVote))
                {
                    bool bModuleOk = false;
                    string strSubmissionModuleId = Request.QueryString["svm"];
                    if (!string.IsNullOrEmpty(strSubmissionModuleId))
                    {
                        int n;
                        if (int.TryParse(strSubmissionModuleId, out n))
                        {
                            if (SubmissionModuleId.HasValue && SubmissionModuleId.Value == n)
                            {
                                bModuleOk = true;
                            }
                        }
                    }

                    bool bSubmissionOk = false;
                    if (bModuleOk)
                    {
                        string strSubmissionId = Request.QueryString["svs"];
                        int n;
                        if (int.TryParse(strSubmissionId, out n))
                        {
                            if (SubmissionId.HasValue && SubmissionId.Value == n)
                            {
                                bSubmissionOk = true;
                            }
                        }
                    }

                    if (bSubmissionOk)
                    {
                        scms.modules.submission.controls.voting.EMode mode = voting.EMode.UpDown;
                        string strMode = Request.QueryString["svmode"];
                        switch (strMode)
                        {
                            case "ud":
                                mode = voting.EMode.UpDown;
                                break;

                            case "fu":
                                mode = voting.EMode.FiveUp;
                                break;
                        }

                        string strVote = Request.QueryString["svv"];
                        switch (mode)
                        {
                            case voting.EMode.UpDown:
                                {
                                    bool bUp = string.Compare(strVote, "1") == 0;
                                    OnVoteUpDownVote(SubmissionId.Value, bUp);
                                }
                                break;

                            case voting.EMode.FiveUp:
                                {
                                    int nIndex;
                                    if (int.TryParse(strVote, out nIndex))
                                    {
                                        OnVoteFiveUpVote(SubmissionId.Value, nIndex);
                                    }
                                }
                                break;
                        }
                    }

                }
            }

        }

        protected void Page_Load(object sender, EventArgs e)
        {
            voting.OnUpDownVote += OnVoteUpDownVote;
            voting.OnFiveUpVote += OnVoteFiveUpVote;
            if (!IsPostBack)
            {
                ProcessParms();
            }
        }

        public override void DataBind()
        {

            // LoadSubmission();
            base.DataBind();
        }

        public void LoadSubmission()
        {
            try
            {
                scms.data.scms_submission_module submissionModule = SubmissionModule;
                scms.data.scms_submission_submission submission = Submission;
                if ((submissionModule == null) || (submission == null))
                {
                    if (SubmissionId.HasValue || SubmissionModuleId.HasValue)
                    {
                        throw new NotImplementedException();
                    }
                }

                if ((submission != null) && (submissionModule != null))
                {
                    bool bShowTitle = false;
                    if (submissionModule.titleEnabled)
                    {
                        if (!string.IsNullOrEmpty(submission.title))
                        {
                            hlHeading.NavigateUrl = scms.HtmlHelper.InsertOrUpdateParm(Request.RawUrl, "sub", submission.id.ToString());
                            hlHeading.Text = submission.title;
                            bShowTitle = true;
                        }

                        if (bShowTitle)
                        {
                            if (!string.IsNullOrEmpty(submissionModule.descriptionCssClass))
                            {
                                string strTitleClass = divTitle.Attributes["class"];
                                if (string.IsNullOrEmpty(strTitleClass))
                                {
                                    strTitleClass = submissionModule.descriptionCssClass;
                                }
                                else
                                {
                                    strTitleClass = string.Format("{0} {1}", strTitleClass, submissionModule.descriptionCssClass);
                                }

                                divTitle.Attributes["class"] = strTitleClass;
                            }
                        }
                    }
                    divTitle.Visible = bShowTitle;


                    bool bShowVoting = false;
                    if (submissionModule.votingEnabled)
                    {
                        bShowVoting = true;

                        switch (submissionModule.votingMethod.ToLower())
                        {
                            case "updown":
                                voting.Mode = voting.EMode.UpDown;
                                voting.UpImagePath = submissionModule.votingUpImageUrl;
                                voting.DownImagePath = submissionModule.votingDownImageUrl;
                                voting.UpText = submissionModule.votingUpText;
                                voting.DownText = submissionModule.votingDownText;
                                voting.NumberOfVotes = submission.votes ?? 0;
                                voting.VotesUp = submission.votesUp ?? 0;
                                voting.VotesDown = submission.votesDown ?? 0;
                                break;

                            case "fiveup":
                                voting.Mode = voting.EMode.FiveUp;
                                voting.FiveUp_ActiveImagePath = submissionModule.votingActiveImageUrl;
                                voting.FiveUp_InActiveImagePath = submissionModule.votingInActiveImageUrl;
                                voting.FiveUp_EvenImagePath = submissionModule.votingEvenImageUrl;
                                voting.FiveUp_UpText = submissionModule.votingSelectText;
                                voting.FiveUp_Vote = 0;

                                CalcFiveUpVotes(ref submission);
                                voting.FiveUp_VotesText = string.Format("{0} votes", submission.votes);
                                voting.FiveUp_Vote = (decimal)submission.vote;

                                break;
                        }

                        voting.LoadVoting();
                    }
                    divVoting.Visible = bShowVoting;
                    divPostDate.InnerText = submission.submissionTime.ToShortDateString();


                    bool bShowImage = false;
                    if (submissionModule.imageEnabled)
                    {
                        string strImageUrl = submission.imageUrl;
                        if (!string.IsNullOrEmpty(strImageUrl))
                        {
                            string strImageWidthParm = string.Format("&w={0}", submissionModule.imageWidth.Value);
                            string strImageHeightParm = strImageHeightParm = string.Format("&h={0}", submissionModule.imageHeight.Value);

                            string strThumbnailUrl = string.Format("/Image.ashx?src={0}&m=grow{1}{2}",
                                HttpUtility.UrlEncode(strImageUrl),
                                strImageWidthParm,
                                strImageHeightParm);

                            image.ImageUrl = strThumbnailUrl;
                            bShowImage = true;
                        }

                        if (bShowImage)
                        {
                            if (!string.IsNullOrEmpty(submissionModule.imageCssClass))
                            {
                                string strImageClass = divImage.Attributes["class"];
                                if (string.IsNullOrEmpty(strImageClass))
                                {
                                    strImageClass = submissionModule.descriptionCssClass;
                                }
                                else
                                {
                                    strImageClass = string.Format("{0} {1}", strImageClass, submissionModule.descriptionCssClass);
                                }

                                divImage.Attributes["class"] = strImageClass;
                            }
                        }
                    }
                    divImage.Visible = bShowImage;


                    bool bShowDescription = false;
                    if (submissionModule.descriptionEnabled)
                    {
                        string strDescription = submission.description;
                        if (!string.IsNullOrEmpty(strDescription))
                        {
                            divDescription.InnerText = strDescription;
                            bShowDescription = true;
                        }

                        if (bShowDescription)
                        {
                            if (!string.IsNullOrEmpty(submissionModule.descriptionCssClass))
                            {
                                string strDescriptionClass = divDescription.Attributes["class"];
                                if (string.IsNullOrEmpty(strDescriptionClass))
                                {
                                    strDescriptionClass = submissionModule.descriptionCssClass;
                                }
                                else
                                {
                                    strDescriptionClass = string.Format("{0} {1}", strDescriptionClass, submissionModule.descriptionCssClass);
                                }

                                divDescription.Attributes["class"] = strDescriptionClass;
                            }
                        }
                    }
                    divDescription.Visible = bShowDescription;

                    bool bShowSubmitter = false;
                    if (submissionModule.submitterEnabled)
                    {
                        string strSubmitter = submission.submitter;
                        if (string.IsNullOrEmpty(strSubmitter))
                        {
                            strSubmitter = "anonymous";
                        }
                        spanSubmitter.InnerHtml = strSubmitter;

                        AddClass(divSubmitter, submissionModule.submitterCssClass);
                        bShowSubmitter = true;
                    }
                    divSubmitter.Visible = bShowSubmitter;

                    bool bShowDocumentCredit = false;
                    if (submissionModule.documentCreditEnabled)
                    {
                        string strDocumentCredit = submission.documentCredit;
                        if (string.IsNullOrEmpty(strDocumentCredit))
                        {
                            strDocumentCredit = "dunno";
                        }
                        spanDocumentCredit.InnerHtml = strDocumentCredit;

                        string strLabel = null;
                        if (!string.IsNullOrEmpty(submission.imageUrl))
                        {
                            strLabel = "Photo Credit:";
                        }
                        else
                        {
                            if (!string.IsNullOrEmpty(submission.videoUrl))
                            {
                                strLabel = "Video Credit:";
                            }
                        }
                        if (!string.IsNullOrEmpty(strLabel))
                        {
                            lableDocumentCredit.InnerHtml = strLabel;
                        }

                        AddClass(divCredit, submissionModule.documentCreditCssClass);
                        bShowDocumentCredit = true;
                    }
                    divCredit.Visible = bShowDocumentCredit;
                    divSource.Visible = bShowSubmitter || bShowDocumentCredit;


                    bool bShowLink = false;
                    if (submissionModule.linkEnabled)
                    {
                        if (!string.IsNullOrEmpty(submission.linkUrl))
                        {
                            string strLinkText = submissionModule.linkText;
                            if (string.IsNullOrEmpty(strLinkText))
                            {
                                strLinkText = string.Format("<a href=\"{0}\">{0}</a>", submission.linkUrl);
                            }
                            else
                            {
                                if (strLinkText.Contains("$submitter$") && !string.IsNullOrEmpty(submission.submitter))
                                {
                                    string strAnchor = string.Format("<a href=\"{0}\">{1}</a>", submission.linkUrl, submission.submitter);
                                    strLinkText = string.Format(strLinkText.Replace("$submitter$", strAnchor));
                                }
                                else
                                {
                                    strLinkText = string.Format("<a href=\"{0}\">{1}</a>", submission.linkUrl, strLinkText);
                                }
                            }
                            divSubmissionLink.InnerHtml = strLinkText;
                            AddClass(divSubmissionLink, submissionModule.linkCssClass);
                            bShowLink = true;
                        }
                    }
                    divSubmissionLink.Visible = bShowLink;


                    bool bShowAdmin = false;
                    if (Page.User.IsInRole("administrator"))
                    {
                        bShowAdmin = true;

                        checkFeatured.Checked = submission.featuredTime != null;
                        checkApproved.Checked = submission.approvedTime != null;
                        checkDeleted.Checked = submission.deleted;
                    }
                    divAdmin.Visible = bShowAdmin;
                }

            }
            catch (Exception ex)
            {
                string strMessage = string.Format("Exception thrown while loading from supplied submission '{0} or loading submission {0}", Submission != null ? Submission.id : 0, SubmissionId);
                ScmsEvent.Raise(strMessage, this, ex);
            }
        }

        protected void AddClass(System.Web.UI.HtmlControls.HtmlGenericControl control, string strClass)
        {
            if (!string.IsNullOrEmpty(strClass))
            {
                string strCurrentClass = control.Attributes["class"];
                if (string.IsNullOrEmpty(strCurrentClass))
                {
                    strClass = string.Format("{0} {1}", strCurrentClass, strClass);
                }

                control.Attributes["class"] = strClass;
            }
        }


        protected void btnEdit_Click(object sender, EventArgs args)
        {
            try
            {
                scms.data.ScmsDataContext dc = new scms.data.ScmsDataContext();
                var submissionModule = (from sm in dc.scms_submission_modules
                                        where sm.id == SubmissionModuleId
                                        select sm).Single();

                string strEditUrl = string.Format("~/scms/admin/module.aspx?sub={0}&id={1}", SubmissionId, submissionModule.instanceId);

                Response.Redirect(strEditUrl);
            }
            catch (System.Threading.ThreadAbortException)
            {
            }
        }
    }
}