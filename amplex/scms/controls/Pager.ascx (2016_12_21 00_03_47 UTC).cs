using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace scms.controls
{
    public partial class Pager : System.Web.UI.UserControl
    {
        int? nPageSize = null;
        public int? PageSize
        {
            get { return nPageSize; }
            set { nPageSize = value; }
        }

        int? nPageNumber = null;
        public int? PageNumber
        {
            get { return nPageNumber; }
            set { nPageNumber = value; }
        }

        int? nCount = null;
        public int? Count
        {
            get { return nCount; }
            set { nCount = value; }
        }



        string strPageNumberParm = "p";
        public string PageNumberParm
        {
            get { return strPageNumberParm; }
            set { strPageNumberParm = value; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Visible)
            {
                SetupPager(nPageNumber, nPageSize, nCount);
            }
        }

				public void RebuildPager()
				{
					SetupPager(nPageNumber, nPageSize, nCount);
				}

        protected void SetupPager(int? nCurrentPage, int? nPageSize, int? nCount)
        {
            bool bShowPager = false;

            if (nCount.HasValue && nPageSize.HasValue)
            {
                bool bShowPrev = false;
                bool bShowNext = false;

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
                            strUrl = scms.HtmlHelper.InsertOrUpdateParm(Request.RawUrl, strPageNumberParm, nPage.ToString());
                            sb.AppendFormat("<div class=\"pager-page pager-page-inactive\" ><a href=\"{0}\">{1}</a></div>", strUrl, nPage + 1);
                        }
                    }
                    literalPagerPages.Text = sb.ToString();

                    if (nCurrentPage.HasValue && nCurrentPage.Value > 0)
                    {
                        string strPrevUrl = scms.HtmlHelper.InsertOrUpdateParm(Request.RawUrl, strPageNumberParm, (nCurrentPage.Value - 1).ToString());
                        anchorPagerPrev.HRef = strPrevUrl;
                        bShowPrev = true;
                    }

                    if (!nCurrentPage.HasValue || (nCurrentPage.HasValue && nCurrentPage.Value < nPages - 1))
                    {
                        int nNextPage = nCurrentPage.HasValue ? nCurrentPage.Value + 1 : 1;
                        string strPrevUrl = scms.HtmlHelper.InsertOrUpdateParm(Request.RawUrl, strPageNumberParm, (nNextPage).ToString());
                        anchorPagerNext.HRef = strPrevUrl;
                        bShowNext = true;
                    }
                }

                divPrevious.Visible = bShowPager && bShowPrev;
                divNext.Visible = bShowPager && bShowNext;
            }


            placeholderPager.Visible = bShowPager;
        }
    }
}