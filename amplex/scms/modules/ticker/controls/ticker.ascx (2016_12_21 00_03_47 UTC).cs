using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace scms.modules.ticker.controls
{
	public partial class ticker : System.Web.UI.UserControl
	{
		ScmsSiteMapProvider provider = null; 

		protected void Page_Load(object sender, EventArgs e)
		{
			if (!IsPostBack)
			{
				provider = new ScmsSiteMapProvider();

				try
				{
					tickerDataContext dc = new scms.modules.ticker.tickerDataContext();

					var tickerEntries = from te in dc.ticker_entries
															orderby te.ordinal
															select te;
					lvMarquee.DataSource = tickerEntries;
					lvMarquee.DataBind();
				}
				catch (Exception ex)
				{
					ScmsEvent.Raise("failed loding ticker entries", this, ex);
				}
			}
		}

		protected void lvMarquee_ItemDataBound(object sender, ListViewItemEventArgs args)
		{
			if (args.Item.ItemType == ListViewItemType.DataItem)
			{
				ListViewDataItem dataItem = (ListViewDataItem)args.Item;

				ticker_entry te = (ticker_entry)dataItem.DataItem;
				System.Web.UI.HtmlControls.HtmlGenericControl spanTickerLabel = (System.Web.UI.HtmlControls.HtmlGenericControl)args.Item.FindControl("spanTickerLabel");
				if (!string.IsNullOrEmpty(te.label))
				{
					spanTickerLabel.InnerText = te.label;
				}
				else
				{
					spanTickerLabel.Visible = false;
				}

				string strValue = te.value;
				string strUrl = null;
				ScmsSiteMapProvider.PageNode pageNode = null;
				if (te.pageId.HasValue)
				{
					string strError;
					Exception exError;
					if (provider.GetPageNode(te.pageId.Value, out pageNode, out strError, out exError))
					{
						strUrl = pageNode.page.url;
					}
					else
					{
						pageNode = null;
						ScmsEvent.Raise(strError, null, exError);
					}
				}
				else
				{
					strUrl = te.url;
				}

				if (string.IsNullOrEmpty(strValue) && !string.IsNullOrEmpty(strUrl))
				{
					strValue = strUrl;
				}

				System.Web.UI.HtmlControls.HtmlGenericControl spanLink = (System.Web.UI.HtmlControls.HtmlGenericControl)args.Item.FindControl("spanLink");
				System.Web.UI.HtmlControls.HtmlAnchor anchorLink = (System.Web.UI.HtmlControls.HtmlAnchor)args.Item.FindControl("anchorLink");
				if( string.IsNullOrEmpty(strUrl))
				{
					anchorLink.Visible = false;
					if (string.IsNullOrEmpty(strValue))
					{
						spanLink.Visible = false;
					}
					else
					{
						spanLink.InnerText = strValue;
					}
				}
				else
				{
					if (strUrl.StartsWith("http", StringComparison.OrdinalIgnoreCase))
					{
						anchorLink.Target = "_blank";
					}
					anchorLink.InnerText = strValue;
					anchorLink.HRef = strUrl;
				}
			}
			

		}
	}
}