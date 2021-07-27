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

namespace scms.modules.forms.form.controls
{
    public partial class viewResponses : System.Web.UI.UserControl
    {
        static int nMaxDisplayLength = 20;
        static int nMaxColumns = 7;
        static int nPageSize = 20;
        protected int nCurrentPage = 0;

        public int? FormId
        {
            get { return (int?)ViewState["FormId"]; }
            set
            {
                ViewState["FormId"] = value;
            }
        }

        protected void Page_Init(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                txtFrom.Text = DateTime.Now.AddMonths(-1).ToShortDateString();
                txtTo.Text = DateTime.Now.ToShortDateString();
            }
        }

        protected string GetBaseUrl()
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();

            string strPath = Request.Url.GetLeftPart(UriPartial.Path);
            sb.Append(strPath);

            string strQuery = Request.Url.Query.TrimStart(new char[] { '?' });
            string[] astrQueryParts = strQuery.Split(new char[] { '&' }, StringSplitOptions.RemoveEmptyEntries);

            bool bFirst = true;
            foreach (string strQueryPart in astrQueryParts)
            {
                string[] astrNameValue = strQueryPart.Split(new char[] { '=' }, StringSplitOptions.RemoveEmptyEntries);
                if (string.Compare(astrNameValue[0], "fsid", true) != 0)
                {
                    if (bFirst)
                    {
                        sb.Append("?");
                    }
                    else
                    {
                        sb.Append("&");
                    }
                    sb.Append(strQueryPart);
                }
            }

            return sb.ToString();

        }

        public void LoadResponses(bool bForceShowList)
        {
            int? nFormId = FormId;
            if (nFormId.HasValue)
            {
                string strSelectedSubmission = Request.QueryString["fsid"];
                if (!bForceShowList && !string.IsNullOrEmpty(strSelectedSubmission))
                {
                    int nSelectedSubmission = int.Parse(strSelectedSubmission);
                    ShowDetails(nSelectedSubmission);
                }
                else
                {
                    ShowList();
                }
            }
            else
            {
                multiView.SetActiveView(viewNoResponses);
            }
        }

        protected void ShowDetails(int nSubmissionId)
        {
            multiView.SetActiveView(viewDetail);

            btnDeleteSubmission.CommandArgument = nSubmissionId.ToString();

            int nFormId = FormId.Value;

            string strBaseUrl = GetBaseUrl();

            scms.data.ScmsDataContext dc = new scms.data.ScmsDataContext();

            var form = (from f in dc.scms_forms
                        where f.id == nFormId
                        where f.deleted == false
                        select f).FirstOrDefault();

            if (form != null)
            {
                var submission = (from s in dc.scms_form_submissions
                                  where s.id == nSubmissionId
                                  select s).FirstOrDefault();

                // get the fields
                var fields = from f in dc.scms_form_fields
                             where f.deleted == false
                             where f.formid == nFormId
                             orderby f.ordinal
                             select f;

                // store in array
                scms.data.scms_form_field[] aFields = fields.ToArray();
                // create lookup by id
                System.Collections.Hashtable htFieldIndexById = new Hashtable(aFields.Length);
                for (int nIndex = 0; nIndex < aFields.Length; nIndex++)
                {
                    scms.data.scms_form_field field = aFields[nIndex];
                    htFieldIndexById[field.id] = nIndex;
                }

                // get the values
                var values = from v in dc.scms_form_submission_fieldvalues
                             where v.formsubmissionid == nSubmissionId
                             select v;

                // get option values
                var optionValues = from ov in dc.scms_form_submission_optionfieldvalues
                                   where ov.formsubmissionid == nSubmissionId
                                   select ov;

                System.Text.StringBuilder sb = new System.Text.StringBuilder();
                sb.AppendFormat("<tr><td><strong>id</strong></td><td>{0}</td></tr>\r\n", nSubmissionId);
                sb.AppendFormat("<tr><td><strong>date</strong></td><td>{0} {1}</td></tr>\r\n", submission.submissionTime.ToShortDateString(), submission.submissionTime.ToShortTimeString());

                scms.data.scms_form_submission_fieldvalue[] aFieldValues = new scms.data.scms_form_submission_fieldvalue[aFields.Length];
                foreach (scms.data.scms_form_submission_fieldvalue sfv in submission.scms_form_submission_fieldvalues)
                {
                    if (htFieldIndexById.ContainsKey(sfv.fieldid))
                    {
                        int nIndex = (int)htFieldIndexById[sfv.fieldid];
                        aFieldValues[nIndex] = sfv;
                    }
                }

                // output the fields
                for (int nIndex = 0; nIndex < aFields.Length; nIndex++)
                {
                    scms.data.scms_form_field f = aFields[nIndex];


                    scms.data.scms_form_submission_fieldvalue sfv = aFieldValues[nIndex];
                    if (sfv != null)
                    {
                        string strValue = sfv.value;
                        switch (sfv.scms_form_field.type)
                        {
                            case "fileupload":
                                {
																	string strUrl = strValue;
																	string strLink = null;

																	if (!string.IsNullOrEmpty(strValue))
																	{
																		strLink = string.Format("<a target=\"_blank\" href=\"{0}\">{0}</a>", strValue);
																	}

																	if (IsImage(strValue))
																	{
																		string strThumbnailUrl = string.Format("/Image.ashx?src={0}&m=stretch&w=80&h=80", HttpUtility.UrlEncode(sfv.value));
																		string strImage = string.Format("{0}<br /><img src=\"{1}\" />", strLink, strThumbnailUrl);
																		sb.AppendFormat("<tr><td valign=\"top\"><strong>{0}</strong></td><td>{1}</td></tr>\r\n", f.name, strImage);
																		strValue = null;
																	}
																	else
																	{
																		sb.AppendFormat("<tr><td><strong>{0}</strong></td><td>{1}</td></tr>", f.name, strLink);
																		strValue = null;
																	}
                                }
                                break;

                        }

                        if (strValue != null)
                        {
                            sb.AppendFormat("<tr><td><strong>{0}</strong></td><td>{1}</td></tr>\r\n", f.name, strValue);
                        }
                    }


                }

                literalDetails.Text = sb.ToString();
            }
        }

        protected bool IsImage(string strFileName)
        {
            bool bIsImage = false;

            if (!string.IsNullOrEmpty(strFileName))
            {
                int nLastSlash = strFileName.LastIndexOf(".");
                if (nLastSlash > 0)
                {
                    string strExtension = strFileName.Substring(nLastSlash + 1).ToLower();
                    switch (strExtension)
                    {
                        case "bmp":
                        case "jpg":
                        case "tif":
                        case "png":
                            bIsImage = true;
                            break;
                    }

                }
            }

            return bIsImage;
        }


        protected bool GetDates(out DateTime? dtFrom, out DateTime? dtTo)
        {
            bool bSuccess = false;

            dtFrom = null;
            dtTo = null;
            DateTime dt;
            if (DateTime.TryParse(txtFrom.Text, out dt))
            {
                dtFrom = dt;
                if (DateTime.TryParse(txtTo.Text, out dt))
                {
                    dtTo = dt;
                    bSuccess = true;
                }
            }

            return bSuccess;
        }

        protected void ShowList()
        {
            int? nCount = null;

            int? nFormId = FormId;

            bool bShowPager = false;

            string strBaseUrl = GetBaseUrl();
            string strSelectUrlFormat = string.Format("{0}&fsid={{0}}", strBaseUrl);

            DateTime? dtFrom = null;
            DateTime? dtTo = null;
            if (GetDates(out dtFrom, out dtTo))
            {
                scms.data.ScmsDataContext dc = new scms.data.ScmsDataContext();
                var form = (from f in dc.scms_forms
                            where f.id == nFormId.Value
                            where f.deleted == false
                            select f).FirstOrDefault();

                if (form != null)
                {


                    // get the fields
                    var fields = from f in dc.scms_form_fields
                                 where f.deleted == false
                                 where f.formid == nFormId.Value
                                 orderby f.ordinal
                                 select f;

                    // store in array
                    scms.data.scms_form_field[] aFields = fields.ToArray();
                    // create lookup by id
                    System.Collections.Hashtable htFieldIndexById = new Hashtable(aFields.Length);
                    for (int nIndex = 0; nIndex < aFields.Length; nIndex++)
                    {
                        scms.data.scms_form_field field = aFields[nIndex];
                        htFieldIndexById[field.id] = nIndex;
                    }

                    // output headers
                    System.Text.StringBuilder sbHeaders = new System.Text.StringBuilder();
                    sbHeaders.Append("<tr>\r\n");
                    sbHeaders.Append("<td><strong>id</strong></td>\r\n");
                    sbHeaders.Append("<td><strong>date</strong></td>\r\n");

                    int nColumn = 0;
                    foreach (scms.data.scms_form_field f in aFields)
                    {
                        if (nColumn >= nMaxColumns)
                        {
                            break;
                        }

                        sbHeaders.AppendFormat("<td><strong>{0}</strong></td>\r\n", f.name);
                        nColumn++;
                    }
                    sbHeaders.Append("</tr>\r\n");
                    literalHeaders.Text = sbHeaders.ToString();


                    System.Text.StringBuilder sbBody = new System.Text.StringBuilder();

                    var submissions = from s in dc.scms_form_submissions_get(form.id, nPageSize, nCurrentPage, null, false, dtFrom, dtTo.Value.AddDays(1), ref nCount)
                                      join sfv in dc.scms_form_submission_fieldvalues on s.formid equals sfv.formid into g
                                      select new { s, s.scms_form_submission_fieldvalues };

                    foreach (var submission in submissions)
                    {

                        sbBody.Append("<tr>\r\n");

                        string strSelectUrl = string.Format(strSelectUrlFormat, submission.s.id);
                        sbBody.AppendFormat("<td><a href=\"{0}\">{1}</a></td>\r\n", strSelectUrl, submission.s.id);
                        sbBody.AppendFormat("<td>{0} {1}</td>\r\n", submission.s.submissionTime.ToShortDateString(), submission.s.submissionTime.ToShortTimeString());

                        scms.data.scms_form_submission_fieldvalue[] aFieldValues = new scms.data.scms_form_submission_fieldvalue[aFields.Length];

                        foreach (scms.data.scms_form_submission_fieldvalue sfv in submission.scms_form_submission_fieldvalues)
                        {
                            if (htFieldIndexById.ContainsKey(sfv.fieldid))
                            {
                                int nIndex = (int)htFieldIndexById[sfv.fieldid];
                                aFieldValues[nIndex] = sfv;
                            }
                        }

                        // output the fields
                        for (int nIndex = 0; (nIndex < aFields.Length) && (nIndex < nMaxColumns); nIndex++)
                        {
                            string strValue = null;
                            scms.data.scms_form_submission_fieldvalue sfv = aFieldValues[nIndex];
                            if (sfv != null)
                            {
                                strValue = sfv.value;
                            }

                            if (!string.IsNullOrEmpty(strValue))
                            {
                                if (strValue.Length > nMaxDisplayLength)
                                {
                                    strValue = strValue.Substring(0, nMaxDisplayLength - 3) + "...";
                                }
                            }



                            sbBody.AppendFormat("<td>{0}</td>\r\n", strValue);
                        }

                        sbBody.Append("</tr>\r\n");
                    }
                    literalValues.Text = sbBody.ToString();


                    int nPages = 1 + ((nCount.Value - 1) / nPageSize);
                    if (nPages > 1)
                    {
                        bShowPager = true;
                        System.Collections.Generic.List<int> lPages = new System.Collections.Generic.List<int>();
                        for (int nPage = 0; nPage < nPages; nPage++)
                        {
                            lPages.Add(nPage);
                        }
                        rptPager.DataSource = lPages;
                        rptPager.DataBind();
                    }
                }



                if (nCount.HasValue && (nCount > 0))
                {
                    multiView.SetActiveView(viewList);
                }
                else
                {
                    multiView.SetActiveView(viewNoResponses);
                }
            }

            divPager.Visible = bShowPager;
        }

        protected void lbPage_PageSelected(object sender, CommandEventArgs args)
        {
            int nPage = int.Parse(args.CommandArgument.ToString());
            nCurrentPage = nPage;
            LoadResponses(true);
        }

        protected void lbShowList_Click(object sender, EventArgs args)
        {
            LoadResponses(true);
        }

        protected void btnExport_Clicked(object sender, EventArgs args)
        {
            DateTime? dtFrom = null;
            DateTime? dtTo = null;
            if (GetDates(out dtFrom, out dtTo))
            {
                Response.Clear();
                Response.Buffer = true;
                Response.ContentType = "application/vnd.ms-excel";
                Response.AddHeader("content-disposition", "attachment;filename=reponses.xls");
                Response.Charset = "";
                this.EnableViewState = false;

                System.IO.StringWriter sw = new System.IO.StringWriter();
                sw.Write("<table border=\"1\">");

                int? nFormId = FormId;
                if (nFormId.HasValue)
                {
                    scms.data.ScmsDataContext dc = new scms.data.ScmsDataContext();
                    var form = (from f in dc.scms_forms
                                where f.id == nFormId.Value
                                where f.deleted == false
                                select f).FirstOrDefault();

                    if (form != null)
                    {
                        // get the fields
                        var fields = from f in dc.scms_form_fields
                                     where f.deleted == false
                                     where f.formid == nFormId.Value
                                     orderby f.ordinal
                                     select f;

                        // store in array
                        scms.data.scms_form_field[] aFields = fields.ToArray();
                        // create lookup by id
                        System.Collections.Hashtable htFieldIndexById = new Hashtable(aFields.Length);
                        for (int nIndex = 0; nIndex < aFields.Length; nIndex++)
                        {
                            scms.data.scms_form_field field = aFields[nIndex];
                            htFieldIndexById[field.id] = nIndex;
                        }

                        // output headers
                        sw.WriteLine("<tr>");
                        sw.WriteLine("<th>id</th>");
                        sw.WriteLine("<th>date</th>");

                        int nColumn = 0;
                        foreach (scms.data.scms_form_field f in aFields)
                        {
                            sw.WriteLine(string.Format("<th>{0}</th>", f.name));
                            nColumn++;
                        }
                        sw.WriteLine("</tr>");


                        int? nCount = null;
                        var submissions = from s in dc.scms_form_submissions_get(form.id, 0, 0, null, false, dtFrom, dtTo.Value.AddDays(1), ref nCount)
                                          join sfv in dc.scms_form_submission_fieldvalues on s.formid equals sfv.formid into g
                                          orderby s.id
                                          select new { s, s.scms_form_submission_fieldvalues };

                        foreach (var submission in submissions)
                        {
                            sw.WriteLine(string.Format("<tr><td>{0}</td>", submission.s.id));
                            sw.WriteLine("<td>{0} {1}</td>", submission.s.submissionTime.ToShortDateString(), submission.s.submissionTime.ToShortTimeString());

                            scms.data.scms_form_submission_fieldvalue[] aFieldValues = new scms.data.scms_form_submission_fieldvalue[aFields.Length];
                            foreach (scms.data.scms_form_submission_fieldvalue sfv in submission.scms_form_submission_fieldvalues)
                            {
                                int nIndex = (int)htFieldIndexById[sfv.fieldid];
                                aFieldValues[nIndex] = sfv;
                            }

                            // output the fields
                            for (int nIndex = 0; nIndex < aFields.Length; nIndex++)
                            {
                                string strValue = null;
                                scms.data.scms_form_submission_fieldvalue sfv = aFieldValues[nIndex];
                                if (sfv != null)
                                {
                                    strValue = sfv.value;
                                }

                                sw.WriteLine(string.Format("<td>{0}</td>", strValue));
                            }

                            sw.WriteLine("</tr>");
                        }
                    }
                }

                sw.Write("</table>");
                Response.Write(sw.ToString());
                Response.End();
            }
        }

        protected void btnRefresh_Click(object sender, EventArgs args)
        {
            ShowList();
        }

        protected void btnDelete_Command(object sender, CommandEventArgs args)
        {
            try
            {
                int nSubmissionId = int.Parse((string)args.CommandArgument);
                scms.data.ScmsDataContext dc = new scms.data.ScmsDataContext();
                var submission = (from fs in dc.scms_form_submissions
                                  where fs.id == nSubmissionId
                                  where fs.deleted == false
                                  select fs).FirstOrDefault();
                if (submission != null)
                {
                    submission.deleted = true;
                    dc.SubmitChanges();
                }

                ShowList();
            }
            catch (Exception ex)
            {
                string strMessage = string.Format("Failed marking form submission '{0}' as deleted.", args.CommandArgument);
                ScmsEvent.Raise(strMessage, this, ex);
            }


        }


    }
}