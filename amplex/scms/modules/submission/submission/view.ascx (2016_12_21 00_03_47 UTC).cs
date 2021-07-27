using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace scms.modules.submission.submission
{
    public partial class view : scms.RootControl
    {
        protected enum Mode
        {
            Featured,
            UnFeatured,
            Approved,
            UnApproved,
            All,
            Deleted,
            Single
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            Mode mode = Mode.Featured;

            string strId = Request.QueryString["sub"];
            if (!string.IsNullOrEmpty(strId))
            {
                int nSubmissionId;
                if (int.TryParse(strId, out nSubmissionId))
                {
                    mode = Mode.Single;
                    ShowSingleSubmission(nSubmissionId);
                }
            }

            if (mode != Mode.Single)
            {
                string strMode = Request.QueryString["m"];
                if (!string.IsNullOrEmpty(strMode))
                {
                    try
                    {
                        mode = (Mode)Enum.Parse(typeof(Mode), strMode, true);
                    }
                    catch (Exception ex)
                    {
                        string strMessage = string.Format("Failed parsing parm '{0}' as submission mode", strMode);
                        global::scms.ScmsEvent.Raise(strMessage, this, ex);
                    }
                }

                int? nPageNumber = null;
                int? nPageSize = null;


                switch (mode)
                {
                    case Mode.All:
                    case Mode.Deleted:
                    case Mode.UnApproved:
                    case Mode.UnFeatured:
                        if (!this.Page.User.IsInRole("Administrator"))
                        {
                            RootPage page = this.Page as RootPage;
                            page.RedirectToLogin();
                        }
                        break;
                }

                nPageSize = 5;
                string strPage = Request.Params["p"];
                if (!string.IsNullOrEmpty(strPage))
                {
                    int n;
                    if (int.TryParse(strPage, out n))
                    {
                        nPageNumber = n;
                    }
                }


                ShowMultipleSubmissions(mode, nPageNumber, nPageSize);
            }
        }



        scms.data.scms_submission_module submissionModule = null;
        protected scms.data.scms_submission_module SubmissionModule
        {
            get
            {
                return submissionModule;
            }

            set
            {
                submissionModule = value;
            }
        }


        protected void ShowMultipleSubmissions(Mode mode, int? nPageNumber, int? nPageSize)
        {
            try
            {
                scms.data.ScmsDataContext dc = new scms.data.ScmsDataContext();

                var submissionModule = (from sm in dc.scms_submission_modules
                                        where sm.instanceId == this.ModuleInstanceId
                                        select sm).FirstOrDefault();
                SubmissionModule = submissionModule;
                if (submissionModule != null)
                {
                    int? count = null;

                    bool? bApproved = null;
                    bool? bFeatured = null;
                    bool? bIncludeDeleted = null;
                    switch (mode)
                    {
                        case Mode.All:
                            break;

                        case Mode.Deleted:
                            bIncludeDeleted = true;
                            break;

                        case Mode.Approved:
                            bApproved = true;
                            break;

                        case Mode.UnApproved:
                            bApproved = false;
                            break;

                        case Mode.Featured:
                            bApproved = true;
                            bFeatured = true;
                            break;

                        case Mode.UnFeatured:
                            bApproved = true;
                            bFeatured = false;
                            break;
                    }

                    var submissions = dc.scms_submissions_get(submissionModule.id,
                        null,
                        nPageSize,
                        nPageNumber,
                        "date",
                        false,
                        bApproved,
                        bFeatured,
                        null,
                        null,
                        bIncludeDeleted,
                        ref count);

                    lvSubmissions.DataSource = submissions;
                    lvSubmissions.DataBind();

                    SetupPager(nPageNumber, nPageSize, count);
                }

            }
            catch (Exception ex)
            {
                string strMessage = "Failed loading multiple submissions";
                ScmsEvent.Raise(strMessage, this, ex);
            }


            mv.SetActiveView(viewMultiple);
        }

        protected void SetupPager(int? nCurrentPage, int? nPageSize, int? nCount)
        {
            bool bShowPager = false;

            if (nCount.HasValue && nPageSize.HasValue)
            {
                if (nCount.Value > nPageSize.Value)
                {
                    bShowPager = true;

                    int nPages = 1 + ((nCount.Value - 1) / nPageSize.Value);

                    System.Text.StringBuilder sb = new System.Text.StringBuilder();
                    for (int nPage = 0; nPage < nPages; nPage++)
                    {
                        bool bThisPage = false;

                        if (nCurrentPage.HasValue)
                        {
                            if (nCurrentPage.Value == nPage)
                            {
                                bThisPage = true;
                            }
                        }
                        else
                        {
                            if (nPage == 0)
                            {
                                bThisPage = true;
                            }
                        }

                        if (bThisPage)
                        {
                            sb.AppendFormat("<div class=\"pager-page pager-page-active\" >{0}</div>", nPage + 1);
                        }
                        else
                        {
                            string strUrl = null;
                            strUrl = scms.HtmlHelper.InsertOrUpdateParm(Request.RawUrl, "p", nPage.ToString());
                            sb.AppendFormat("<div class=\"pager-page pager-page-inactive\" ><a href=\"{0}\">{1}</a></div>", strUrl, nPage + 1);
                        }
                    }
                    literalPagerPages.Text = sb.ToString();

                    if (nCurrentPage.HasValue && nCurrentPage.Value > 0)
                    {
                        string strPrevUrl = scms.HtmlHelper.InsertOrUpdateParm(Request.RawUrl, "p", (nCurrentPage.Value - 1).ToString());
                        anchorPagerPrev.HRef = strPrevUrl;
                    }

                    if (!nCurrentPage.HasValue || (nCurrentPage.HasValue && nCurrentPage.Value < nPages - 1))
                    {
                        int nNextPage = nCurrentPage.HasValue ? nCurrentPage.Value + 1 : 1;
                        string strPrevUrl = scms.HtmlHelper.InsertOrUpdateParm(Request.RawUrl, "p", (nNextPage).ToString());
                        anchorPagerNext.HRef = strPrevUrl;
                    }
                }
            }


            placeholderPager.Visible = bShowPager;
        }

        protected void lvSubmissions_ItemDataBound(object sender, ListViewItemEventArgs args)
        {
            if (args.Item.ItemType == ListViewItemType.DataItem)
            {
                ListViewDataItem dataItem = (ListViewDataItem)args.Item;
                scms.data.scms_submission_submission submissionDataItem = (scms.data.scms_submission_submission)dataItem.DataItem;

                scms.modules.submission.controls.submission submissionControl = args.Item.FindControl("submission") as scms.modules.submission.controls.submission;

                submissionControl.Submission = submissionDataItem;
                submissionControl.LoadSubmission();
            }
        }

        protected void ShowSingleSubmission(int nSubmissionId)
        {
            try
            {
                submissionSingle.SubmissionId = nSubmissionId;

                scms.data.ScmsDataContext dc = new scms.data.ScmsDataContext();
                var data = (from sm in dc.scms_submission_modules
                            where sm.instanceId == ModuleInstanceId.Value
                            join s in dc.scms_submission_submissions on sm.id equals s.submissionModuleId
                            where s.id == nSubmissionId
                            where s.deleted == false
                            select new { sm, s }).Single();

                submissionSingle.SubmissionModule = data.sm;
                submissionSingle.SubmissionModuleId = data.sm.id;
                submissionSingle.Submission = data.s;
                submissionSingle.SubmissionId = data.s.id;
                submissionSingle.LoadSubmission();
                mv.SetActiveView(viewSingleSubmission);
            }
            catch (Exception ex)
            {
                string strMessage = string.Format("Failed loading single submission for id '{0}'.", nSubmissionId);
                ScmsEvent.Raise(strMessage, this, ex);
                mv.SetActiveView(viewNone);
            }
        }

    }
}