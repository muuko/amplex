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

namespace scms.modules.search.searchresults
{
	public partial class view : scms.RootControl
	{
		public int? MaxResultCount
		{
			get { return (int?)ViewState["MaxResultCount"]; }
			set { ViewState["MaxResultCount"] = value; }
		}

		public int? PageSize
		{
			get { return (int?)ViewState["PageSize"]; }
			set { ViewState["PageSize"] = value; }
		}

		public int? CurrentPage
		{
			get { return (int?)ViewState["CurrentPage"]; }
			set { ViewState["CurrentPage"] = value; }
		}

		public int? MaxKeywords
		{
			get { return (int?)ViewState["MaxKeywords"]; }
			set { ViewState["MaxKeywords"] = value; }
		}

		public bool ShowThumbnail
		{
			get
			{
				bool? bShowThumbnail = (bool?)ViewState["ShowThumbnail"];
				return bShowThumbnail.HasValue && bShowThumbnail.Value;
			}

			set { ViewState["ShowThumbnail"] = value; }
		}

		public int? ThumbnailWidth
		{
			get { return (int?)ViewState["ThumbnailWidth"]; }
			set { ViewState["ThumbnailWidth"] = value; }
		}

		public int? ThumbnailHeight
		{
			get { return (int?)ViewState["ThumbnailHeight"]; }
			set { ViewState["ThumbnailHeight"] = value; }
		}

		public bool ShowUrl
		{
			get
			{
				bool? bShowUrl = (bool?)ViewState["ShowUrl"];
				return bShowUrl.HasValue && bShowUrl.Value;
			}

			set { ViewState["ShowUrl"] = value; }
		}

		public bool ShowReadMore
		{
			get
			{
				bool? bShowReadMore = (bool?)ViewState["ShowReadMore"];
				return bShowReadMore.HasValue && bShowReadMore.Value;
			}

			set { ViewState["ShowReadMore"] = value; }
		}

		public string ReadMoreText
		{
			get { return (string)ViewState["ReadMoreText"]; }
			set { ViewState["ReadMoreText"] = value; }
		}

		public bool? ShowPrevNextButtons
		{
			get
			{
				bool? bShowPrevNextButtons = (bool?)ViewState["ShowPrevNextButtons"];
				return bShowPrevNextButtons.HasValue && bShowPrevNextButtons.Value;
			}

			set { ViewState["ShowPrevNextButtons"] = value; }
		}


		public string Query
		{
			get { return (string)ViewState["Query"]; }
			set { ViewState["Query"] = value; }
		}

		protected void LoadModule()
		{
			try
			{
				scms.data.ScmsDataContext dc = new scms.data.ScmsDataContext();
				scms.data.scms_search_results_module module = null;
				module = (from m in dc.scms_search_results_modules
									where m.instanceId == ModuleInstanceId.Value
									select m).FirstOrDefault();
				if (module == null)
				{
					module = new scms.data.scms_search_results_module();
					module.instanceId = ModuleInstanceId.Value;

					module.maxResultCount = MaxResultCount;
					module.pageSize = PageSize;
					module.maxKeywords = MaxKeywords;
					module.showThumbnail = ShowThumbnail;
					module.thumbnailWidth = ThumbnailWidth;
					module.showUrl = ShowUrl;
					module.showReadMore = ShowReadMore;
					module.showPrevNext = ShowPrevNextButtons.Value;
					dc.scms_search_results_modules.InsertOnSubmit(module);
					dc.SubmitChanges();
				}
				else
				{
					MaxResultCount = module.maxResultCount;
					PageSize = module.pageSize;
					MaxKeywords = module.maxKeywords;
					ShowThumbnail = module.showThumbnail;
					ThumbnailHeight = module.thumbnailHeight;
					ThumbnailWidth = module.thumbnailWidth;
					ShowUrl = module.showUrl;
					ShowReadMore = module.showReadMore;
					ShowPrevNextButtons = module.showPrevNext;
				}
			}
			catch (Exception ex)
			{
				string strMessage = string.Format("Failed loading search results module with instance id '{0}'.", ModuleInstanceId);
				ScmsEvent.Raise(strMessage, this, ex);
			}
		}

		protected void Page_Load(object sender, EventArgs e)
		{
			if (!IsPostBack)
			{
				LoadModule();

				Query = Request.QueryString["q"];
				string strCurrentPage = Request.QueryString["p"];
				if (!string.IsNullOrEmpty(strCurrentPage))
				{
					int n;
					if (int.TryParse(strCurrentPage, out n))
					{
						CurrentPage = n;
					}
					else
					{
						string strMessage = string.Format("Failed parsing query paramater p, page, '{0}' as integer", strCurrentPage);
						ScmsEvent.Raise(strMessage, this, null);
					}
				}


				PerformSearch();
			}
		}

		public view()
		{
			MaxResultCount = 200;
			PageSize = 10;
			CurrentPage = 0;
			MaxKeywords = 5;

			ShowThumbnail = true;
			ThumbnailHeight = 80;
			ThumbnailWidth = 80;
			ShowUrl = true;
			ShowReadMore = true;
			ShowPrevNextButtons = true;
		}

		protected void PerformSearch()
		{
			scms.search.search search = new scms.search.search();
			if (search.Init())
			{
				string strQuery = Query;
				int? nMaxKeywords = MaxKeywords;
				int? nMaxResultCount = MaxResultCount;
				int? nPageSize = PageSize;
				int? nPage = CurrentPage;

				scms.data.scms_search_target[] aTargets = null;
				int? nTotal;

				rptSearchResults.DataSource = null;

				bool bShowPager = false;
				if (search.Search(strQuery, nMaxKeywords, nPage, nPageSize, nMaxResultCount, out aTargets, out nTotal))
				{
					rptSearchResults.DataSource = aTargets;

					// setup pager
					if (nPageSize.HasValue && nTotal.HasValue)
					{
						if (nTotal > nPageSize.Value)
						{
							int nPages = 1 + ((nTotal.Value - 1) / nPageSize.Value);
							int[] aPagerPages = new int[nPages];
							for (int nIndex = 0; nIndex < nPages; nIndex++)
							{
								aPagerPages[nIndex] = nIndex;
							}

							rptPager.DataSource = aPagerPages;
							rptPager.DataBind();

							bool? bShowPrevNextButtons = ShowPrevNextButtons;
							if (bShowPrevNextButtons.HasValue && bShowPrevNextButtons.Value)
							{
								int nCurrentPage = nPage.HasValue ? nPage.Value : 0;
								if (nCurrentPage < (nPages - 1))
								{
									anchorNext.HRef = GetUrl(nCurrentPage + 1);
								}

								if (nCurrentPage > 0)
								{
									anchorPrevious.HRef = GetUrl(nCurrentPage - 1);
								}
							}
							else
							{
								divPagePrev.Visible = false;
								divPageNext.Visible = false;
							}

							bShowPager = true;
						}
					}
				}
				if (!bShowPager)
				{
					divResultsPager.Visible = false;
					divPagePrev.Visible = false;
					divPageNext.Visible = false;
				}
				rptSearchResults.DataBind();




				string strQuerySanitized = null;
				if (!string.IsNullOrEmpty(strQuery))
				{
					strQuerySanitized = strQuery.Replace("<", "").Replace(">", "");
				}

				string strSearchSummary = null;
				if (nTotal.HasValue && nTotal.Value > 0)
				{
					int nStartIndex = 1;
					int nEndIndex = nTotal.Value;
					if (nPage.HasValue && nPageSize.HasValue)
					{
						nStartIndex = nPage.Value * nPageSize.Value + 1;
						nEndIndex = nStartIndex + aTargets.Length - 1;
					}

					strSearchSummary = string.Format("Showing results {0} to {1} of {2} for the search phrase <strong>'{3}'</strong>.", nStartIndex, nEndIndex, nTotal, strQuerySanitized);
				}
				else
				{
					strSearchSummary = string.Format("No results where found for the search phrase <strong>'{0}'</strong>.", strQuerySanitized);
				}

				divSearchSummary.InnerHtml = strSearchSummary;
			}
		}

		protected void rptSearchResults_OnItemDataBound(object sender, RepeaterItemEventArgs args)
		{
			scms.data.scms_search_target target = (scms.data.scms_search_target)args.Item.DataItem;
			scms.data.scms_page page = target.scms_page;

			string strUrl = page.url;
			if (!string.IsNullOrEmpty(target.queryString))
			{
				string strSeparator = "?";
				if (strUrl.Contains(strSeparator))
				{
					strSeparator = "&";
				}
				strUrl = string.Concat(new string[] { strUrl, strSeparator, target.queryString });
			}

			string strTargetLinkText = page.linktext;
			string strTargetTitle = page.title;

			if (target.titleOverride != null)
			{
				strTargetLinkText = target.titleOverride;
				strTargetTitle = target.titleOverride;
			}

			HtmlAnchor anchorTitle = args.Item.FindControl("anchorTitle") as HtmlAnchor;
			if (anchorTitle != null)
			{
				anchorTitle.HRef = strUrl;
				anchorTitle.InnerText = strTargetLinkText;
			}

			HtmlGenericControl divThumbnail = args.Item.FindControl("divThumbnail") as HtmlGenericControl;
			if (divThumbnail != null)
			{
				bool bShowThumbnail = ShowThumbnail;
				if (bShowThumbnail)
				{
					HtmlAnchor anchorThumbnail = args.Item.FindControl("anchorThumbnail") as HtmlAnchor;

					string strImageUrl = page.thumbnail;
					if (!string.IsNullOrEmpty(target.thumbnailOverride))
					{
						strImageUrl = target.thumbnailOverride;
					}

					if (string.IsNullOrEmpty(strImageUrl))
					{
						if (anchorThumbnail != null)
						{
							anchorThumbnail.Visible = false;
						}
					}
					else
					{
						anchorThumbnail.HRef = strUrl;

						HtmlImage imgThumbnail = args.Item.FindControl("imgThumbnail") as HtmlImage;
						if (imgThumbnail != null)
						{
							string strThumbnailUrl = string.Format("/Image.ashx?src={0}&m=stretch&h={1}&w={2}",
								HttpUtility.UrlEncode(strImageUrl),
								ThumbnailHeight,
								ThumbnailWidth);

							imgThumbnail.Src = strThumbnailUrl;
							imgThumbnail.Alt = strTargetTitle;
						}
					}
				}
				else
				{
					divThumbnail.Visible = false;
				}
			}

			HtmlGenericControl divSummary = args.Item.FindControl("divSummary") as HtmlGenericControl;
			if (divSummary != null)
			{
				string strSummary = page.summary;

				if (!string.IsNullOrEmpty(target.summaryOverride))
				{
					strSummary = target.summaryOverride;
				}

				if (string.IsNullOrEmpty(strSummary))
				{
					divSummary.Visible = false;
				}
				else
				{
					divSummary.InnerHtml = strSummary;
				}
			}

			HtmlGenericControl divUrl = args.Item.FindControl("divUrl") as HtmlGenericControl;
			if (divUrl != null)
			{
				if (ShowUrl)
				{
					string strFullUrl = string.Format("{0}://{1}{2}", Request.Url.Scheme, Request.Url.Authority, strUrl);
					divUrl.InnerText = strFullUrl;
				}
				else
				{
					divUrl.Visible = false;
				}
			}

			HtmlGenericControl divReadMore = args.Item.FindControl("divReadMore") as HtmlGenericControl;
			if (divReadMore != null)
			{
				if (ShowReadMore)
				{
					HtmlAnchor anchorReadMore = args.Item.FindControl("anchorReadMore") as HtmlAnchor;
					if (anchorReadMore != null)
					{
						anchorReadMore.HRef = strUrl;
						anchorReadMore.Title = strTargetTitle;

						string strReadMoreText = ReadMoreText;
						if (!string.IsNullOrEmpty(strReadMoreText))
						{
							anchorReadMore.InnerText = ReadMoreText;
						}
					}
				}
				else
				{
					divReadMore.Visible = false;
				}
			}
		}

		protected void rptPager_OnItemDataBound(object sender, RepeaterItemEventArgs args)
		{
			switch (args.Item.ItemType)
			{
				case ListItemType.Item:
				case ListItemType.AlternatingItem:
					{
						HtmlGenericControl divPageInactive = args.Item.FindControl("divPageInactive") as HtmlGenericControl;
						HtmlGenericControl divPageActive = args.Item.FindControl("divPageActive") as HtmlGenericControl;

						int nCurrentPage = CurrentPage.HasValue ? CurrentPage.Value : 0;
						int nPage = (int)args.Item.DataItem;
						if (nPage == nCurrentPage)
						{
							divPageInactive.Visible = false;
							divPageActive.InnerText = (nPage + 1).ToString();
						}
						else
						{
							divPageActive.Visible = false;
							HtmlAnchor anchorPage = args.Item.FindControl("anchorPage") as HtmlAnchor;
							anchorPage.InnerText = (nPage + 1).ToString();

							string strUrl = GetUrl(nPage);
							anchorPage.HRef = strUrl;
						}
					}
					break;
			}
		}

		protected string GetUrl(int nPageNumber)
		{
			string strRawUrl = Request.RawUrl;
			string strUrl = strRawUrl;

			int nQueryStart = strRawUrl.IndexOf('?');
			if (nQueryStart > 0)
			{
				string strPath = strRawUrl.Substring(0, nQueryStart);
				string strQuery = strRawUrl.Substring(nQueryStart + 1);

				System.Text.StringBuilder sbQuery = new System.Text.StringBuilder();

				string[] astrParms = strQuery.Split(new char[] { '&' }, StringSplitOptions.RemoveEmptyEntries);
				foreach (string strParm in astrParms)
				{
					if (!strParm.ToLower().Contains("p="))
					{
						if (sbQuery.Length > 0)
						{
							sbQuery.Append("&");
						}
						else
						{
							sbQuery.Append("?");
						}

						sbQuery.Append(strParm);
					}
				}

				if (sbQuery.Length > 0)
				{
					sbQuery.Append("&");
				}
				else
				{
					sbQuery.Append("?");
				}
				sbQuery.AppendFormat("p={0}", nPageNumber);
				strUrl = string.Concat(strPath, sbQuery.ToString());
			}


			// string strUrl = string.Format("{0}?q={1}&sp={2}", Request.Url.AbsolutePath, HttpUtility.UrlEncode(Query), nPageNumber);

			return strUrl;
		}
	}
}