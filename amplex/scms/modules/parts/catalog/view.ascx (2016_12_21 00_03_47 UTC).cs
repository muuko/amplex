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

namespace scms.modules.parts.catalog
{
	public partial class view :  RootControl 
	{
		protected int ? nPageNumber = null;
		protected const int nPageSize = 50;
		protected amplex.scms.modules.parts.classes.cat_setting settings = null;
		protected string strPartViewUrl = null;
		protected bool bShowPrices = false;
		protected string strSearchResultsUrl = null;

		protected void Page_Load(object sender, EventArgs e)
		{

			string strTextToSearch = Request.QueryString["ss"];
			
			string strPreselectLetter = null;
			if( string.IsNullOrEmpty(strTextToSearch))
			{
				strPreselectLetter = Request.QueryString["sl"];
			}

			string strPageNumber = Request.QueryString["p"];
			if (!string.IsNullOrEmpty(strPageNumber))
			{
				int n;
				if (int.TryParse(strPageNumber, out n))
				{
					nPageNumber = n;
				}
			}

			LoadSettings();

			if (!IsPostBack)
			{
				

				if (!bShowPrices)
				{
					literalPriceHeader.Visible = false;
					anchorViewPricesHeader.Visible = true;
					anchorViewPricesHeader.HRef = string.Format("/login?returnUrl={0}", HttpUtility.UrlEncode(Request.RawUrl));
				}


				txtSearch.Text = strTextToSearch;

				if (!string.IsNullOrEmpty(strPreselectLetter))
				{
					strPreselectLetter = strPreselectLetter.ToUpper();
				}
				System.Text.StringBuilder sbAlphabet = new System.Text.StringBuilder();
				foreach (var letter in "ABCDEFGHIJKLMNOPRSTUVWXYZ")
				{
					string strClass = null;
					if (!string.IsNullOrEmpty(strPreselectLetter))
					{
						if (strPreselectLetter[0] == letter)
						{
							strClass = " class=\"active\"";
						}
					}

					sbAlphabet.AppendFormat("<a alt=\"plants starting with letter '{0}'\" {1} href=\"?sl={0}\">{0}</a>", letter, strClass);
				}
				divCatalogSelectAlphabet.InnerHtml = sbAlphabet.ToString();

				
				// setup inputs from request
				// common
				bool bSearchInCommonName = true;
				string strSearchInCommonName = Request.QueryString["sic"];
				if( !string.IsNullOrEmpty(strSearchInCommonName))
				{
					if(( strSearchInCommonName[0] == '0') || (string.Compare(strSearchInCommonName, "false", true) == 0))
					{
						bSearchInCommonName = false;
					}
				}
				checkCommonName.Checked = bSearchInCommonName;

				// botanical
				bool bSearchInBotanicalName = true;
				string strSearchInBotanicalName = Request.QueryString["sib"];
				if( !string.IsNullOrEmpty(strSearchInBotanicalName))
				{
					if ((strSearchInBotanicalName[0] == '0') || (string.Compare(strSearchInBotanicalName, "false", true) == 0))
					{
						bSearchInBotanicalName = false;
					}
				}
				checkBotanicalName.Checked = bSearchInBotanicalName;

				// description
				bool bSearchInDescription = true;
				string strSearchInDescription = Request.QueryString["sid"];
				if( !string.IsNullOrEmpty(strSearchInDescription))
				{
					if ((strSearchInDescription[0] == '0') || (string.Compare(strSearchInDescription, "false", true) == 0))
					{
						bSearchInDescription = false;
					}
				}
				checkDescription.Checked = bSearchInDescription;

				bool bSearchByPreselectLetter = !string.IsNullOrEmpty(strPreselectLetter);
				if (bSearchByPreselectLetter)
				{
					strTextToSearch = strPreselectLetter;
					bSearchInCommonName = true;
					bSearchInBotanicalName = false;
					bSearchInDescription = false;
				}

				strSearchResultsUrl = CalculateSearchUrl(strTextToSearch, bSearchInCommonName, bSearchInBotanicalName, bSearchInDescription, !bSearchByPreselectLetter, nPageNumber);
				strSearchResultsUrl = HttpUtility.UrlEncode(strSearchResultsUrl);

				strTextToSearch = PrepSearchText(strTextToSearch);
				PerformSearch(strTextToSearch, bSearchInCommonName, bSearchInBotanicalName, bSearchInDescription, !bSearchByPreselectLetter, nPageNumber);
			}
		}

		protected void LoadSettings()
		{
			try
			{
				amplex.scms.modules.parts.classes.partsDataContext dcParts = new amplex.scms.modules.parts.classes.partsDataContext();
				settings = (from s in dcParts.cat_settings
										where s.siteId == SiteId.Value
										select s).FirstOrDefault();
				scms.ScmsSiteMapProvider siteMapProvider = new ScmsSiteMapProvider();
				scms.ScmsSiteMapProvider.PageNode pageNode = null;
				string strError;
				Exception exError;
				if (!siteMapProvider.GetPageNode(settings.searchResultsPageId.Value, out pageNode, out strError, out exError))
				{
					throw new Exception( string.Format( "getpagenode failed: '{0}'", strError), exError);
				}
				strPartViewUrl = pageNode.page.url;

				bShowPrices = Page.User.Identity.IsAuthenticated;
			}
			catch (Exception ex)
			{
				scms.ScmsEvent.Raise("Exception thrown while loading settings", this, ex);
			}
		}


		protected void btnSearch_Click(object sender, EventArgs args)
		{
			string strText = txtSearch.Text.Trim();
			if (!string.IsNullOrEmpty(strText))
			{
				bool bSearchInCommonName = checkCommonName.Checked;
				bool bSearchInBotanicalName = checkBotanicalName.Checked;
				bool bSearchInDescription = checkDescription.Checked;

				string strUrl = CalculateSearchUrl(strText, bSearchInCommonName, bSearchInBotanicalName, bSearchInDescription, true, null);

				try
				{
					Response.Redirect(strUrl, true);
				}
				catch (System.Threading.ThreadAbortException)
				{
				}
			}
		}

		protected string PrepSearchText(string strText)
		{
			string strSearchText = null;

			if (!string.IsNullOrEmpty(strText))
			{
				System.Collections.Generic.List<char> lch = new System.Collections.Generic.List<char>();
				foreach (var ch in strText)
				{
					if (char.IsLetterOrDigit(ch))
					{
						lch.Add(ch);
					}
					else if (ch == ',')
					{
						lch.Add(',');
					}
					else if (char.IsWhiteSpace(ch))
					{
						lch.Add(',');
					}
				}
				strSearchText = new string(lch.ToArray());
			}

			return strSearchText;
		}

		protected string CalculateSearchUrl(string strSearchText, bool bSearchInCommonName, bool bSearchInBotanicalName, bool bSearchInDescription, bool bPowerSearch, int? nPageNumber)
		{
			string strSearchUrl = null;

			string strUrlSearchParm = null;
			if (bPowerSearch)
			{
				// research this cause the 'back' functionality fails
				strUrlSearchParm = string.Format("ss={0}", HttpUtility.UrlEncode(strSearchText));
			}
			else
			{
				strUrlSearchParm = string.Format("sl={0}", strSearchText);
			}

			string strCurrentUrl = Request.RawUrl;
			int nFirstSeparator = strCurrentUrl.IndexOf('?');
			if (nFirstSeparator > 0)
			{
				strCurrentUrl = strCurrentUrl.Substring(0, nFirstSeparator);
			}

			strSearchUrl = string.Format("{0}?{1}&p={2}&sic={3}&sib={4}&sid={5}",
				strCurrentUrl,
				strUrlSearchParm,
				nPageNumber,
				bSearchInCommonName,
				bSearchInBotanicalName,
				bSearchInDescription);

			return strSearchUrl;
		}

		protected void PerformSearch( string strSearchText, bool bSearchInCommonName, bool bSearchInBotanicalName, bool bSearchInDescription, bool bPowerSearch, int ? nPageNumber)
		{
			bool bShowResults = false;

			try
			{
				if (!string.IsNullOrEmpty(strSearchText))
				{
					int? nCount = null;
					amplex.scms.modules.parts.classes.partsDataContext dcParts = new amplex.scms.modules.parts.classes.partsDataContext();
					var parts = dcParts.cat_part_search(strSearchText, bSearchInCommonName, bSearchInBotanicalName, false, bSearchInDescription, "Description1", bPowerSearch, nPageSize, nPageNumber, ref nCount);

					
					pager.PageNumber = nPageNumber;
					pager.PageSize = nPageSize;
					pager.Count = nCount;
					
					
					lvResults.DataSource = parts;
					lvResults.DataBind();
					bShowResults = true;

					
				}
			}
			catch (Exception ex)
			{
				scms.ScmsEvent.Raise("catalog search failed", this, ex);
			}

			placeholderTableResults.Visible = bShowResults;

		}

		protected void lvResults_ItemDataBound(object sender, ListViewItemEventArgs args)
		{
			ListViewItem lvItem = args.Item;
			if (lvItem.ItemType == ListViewItemType.DataItem)
			{
				ListViewDataItem lvDataItem = (ListViewDataItem)lvItem;

				amplex.scms.modules.parts.classes.cat_part part = (amplex.scms.modules.parts.classes.cat_part)lvDataItem.DataItem;

				string strPrice = string.Empty;
				HtmlGenericControl controlPrice = (HtmlGenericControl)lvItem.FindControl("spanPrice");
				if (bShowPrices)
				{
					if (part.sage_price.HasValue && (part.sage_price.Value > 0))
					{
						strPrice = part.sage_price.Value.ToString("c");
					}
					else
					{
						strPrice = "Market Price";
					}
				}
				controlPrice.InnerText = strPrice;

				string strUrl = string.Format("{0}?p={1}&searchResults={2}", strPartViewUrl, part.sage_ID, strSearchResultsUrl);

				HtmlImage imgThumbnail = (HtmlImage)lvItem.FindControl("imgThumbnail");
				if (!string.IsNullOrEmpty(part.imageUrl))
				{
					string strSource = string.Format( "/image.ashx?src={0}&m=grow&w=120&h=90", HttpUtility.UrlEncode(part.imageUrl));
					imgThumbnail.Src = strSource;
				}
				else
				{
					imgThumbnail.Visible = false;
				}

				HtmlAnchor anchorCommon = (HtmlAnchor)lvItem.FindControl("anchorCommon");
				anchorCommon.HRef = strUrl;

				HtmlAnchor anchorBotanical = (HtmlAnchor)lvItem.FindControl("anchorBotanical");
				anchorBotanical.HRef = strUrl;
			}
		}
	}
}