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

namespace scms.modules.rss.rssList
{
	public partial class view : RootControl
	{
		protected scms.data.scms_rss_list rssList = null;
		bool bFirst = true;

		protected void Page_Load(object sender, EventArgs e)
		{
			if (!IsPostBack)
			{
				if (this.ModuleInstanceId.HasValue)
				{
					scms.data.ScmsDataContext dc = new scms.data.ScmsDataContext();
					rssList = (from rl in dc.scms_rss_lists
										 where rl.instanceId == this.ModuleInstanceId
										 select rl).FirstOrDefault();

					if (rssList != null)
					{
						Setup(rssList);
					}
				}
			}
		}

		protected void rptItemsTemplated_ItemDataBound(object sender, RepeaterItemEventArgs args)
		{
			switch (args.Item.ItemType)
			{
				case ListItemType.Item:
				case ListItemType.AlternatingItem:
					{
						scms.data.scms_rss_item item = args.Item.DataItem as scms.data.scms_rss_item;
						if (item != null)
						{
							System.Text.StringBuilder sb = new System.Text.StringBuilder();

							if (bFirst)
							{
								bFirst = false;
							}
							else
							{
								sb.Append(rssList.templateSeparatorHtml);
							}

							string strItem = SusbstituteTemplateVariables(rssList.templateHtml, item);
							sb.Append(sb);

							Literal literalItem = (Literal)args.Item.FindControl("literalItem");
							literalItem.Text = strItem;


						}
					}
					break;
			}
		}

		protected string SusbstituteTemplateVariables(string strText, scms.data.scms_rss_item item)
		{
			string strTextWithSubstitutions = strText;

			strTextWithSubstitutions = GetWithSubstitution(strTextWithSubstitutions, "TITLE", RemoveHtml(item.title));
			strTextWithSubstitutions = GetWithSubstitution(strTextWithSubstitutions, "DESCRIPTION", RemoveHtml(item.description));
			strTextWithSubstitutions = GetWithSubstitution(strTextWithSubstitutions, "LINKURL", item.link);

			return strTextWithSubstitutions;
		}

		protected string RemoveHtml(string strText)
		{
			string strTextNoHtml = null;

			string strPattern = "\\<.*?\\>";

			System.Text.RegularExpressions.Regex regex = new System.Text.RegularExpressions.Regex(strPattern);
			strTextNoHtml = regex.Replace(strText, string.Empty);

			return strTextNoHtml;
		}

		protected string GetWithSubstitution(string strText, string strVariable, string strVariableValue)
		{
			string strTextWithSubstitutions = strText;

			string strSimpleReplacePattern = string.Format("##{0}##", strVariable);
			strTextWithSubstitutions = strTextWithSubstitutions.Replace(strSimpleReplacePattern, strVariableValue);

			//string strPatternFormat = "(?<variable>(##{0}\\[[0-9]+\\]##))";
			string strPatternFormat = "##{0}\\[(?<length>([0-9]+))\\]##";
			string strPattern = string.Format(strPatternFormat, strVariable);


			int nIndex = 0;

			System.Text.RegularExpressions.Regex regex = new System.Text.RegularExpressions.Regex(strPattern);
			bool bDone = false;
			while (!bDone)
			{
				System.Text.RegularExpressions.Match match = regex.Match(strTextWithSubstitutions, nIndex);
				if (match.Success)
				{
					if (match.Captures.Count == 1)
					{
						System.Text.RegularExpressions.Group group = match.Groups["length"];
						if (group != null)
						{
							string strTrimLength = group.Value;
							int nTrimLength = int.Parse(strTrimLength);

							string strVariableValueTrimmed = strVariableValue;

							if (strVariableValueTrimmed.Length > nTrimLength)
							{
								strVariableValueTrimmed = strVariableValueTrimmed.Substring(0, nTrimLength);
								int nLastSpace = strVariableValueTrimmed.LastIndexOf(' ');
								if (nLastSpace > 0)
								{
									strVariableValueTrimmed = strVariableValueTrimmed.Substring(0, nLastSpace);
								}

								strVariableValueTrimmed += "...";
							}

							strTextWithSubstitutions = string.Concat(new string[] { 
								strTextWithSubstitutions.Substring(0, match.Index),
								strVariableValueTrimmed,
								strTextWithSubstitutions.Substring(match.Index + match.Length) });
							nIndex = match.Index + strVariableValueTrimmed.Length;
						}
						else
						{
							bDone = true;
						}
					}
					else
					{
						bDone = true;
						// unexpected
					}
				}
				else
				{
					bDone = true;
				}
			}


			return strTextWithSubstitutions;
		}

		protected void rptItems_ItemDataBound(object sender, RepeaterItemEventArgs args)
		{
			switch (args.Item.ItemType)
			{
				case ListItemType.Item:
				case ListItemType.AlternatingItem:
					{
						scms.data.scms_rss_item item = args.Item.DataItem as scms.data.scms_rss_item;
						if (item != null)
						{
							if (rssList.titleEnabled)
							{
								Literal literalTitle = (Literal)args.Item.FindControl("literalTitle");

								if (rssList.titleAsLink.HasValue && rssList.titleAsLink.Value)
								{
									literalTitle.Text = string.Format("<a href=\"{0}\">{1}</a>", item.link, item.title);
								}
								else
								{
									literalTitle.Text = item.title;
								}
							}
						}
					}
					break;
			}
		}

		public class dataItem
		{
			public string title
			{
				get;
				set;
			}

			public string link
			{
				get;
				set;
			}
		}

		protected void GetFeed(int nRssId, out scms.data.scms_rss rss, out System.Collections.Generic.IEnumerable<scms.data.scms_rss_item> iItems)
		{
			rss.classes.rssProcessor rssProcessor = new scms.modules.rss.classes.rssProcessor();
			rss = null;

			if (!rssProcessor.GetFeed(nRssId, out rss, out iItems))
			{
				throw new Exception("Failed getting feed");
			}
		}

		protected void Setup(scms.data.scms_rss_list rssList)
		{
			System.Collections.Generic.IEnumerable<scms.data.scms_rss_item> iItems;
			scms.data.scms_rss rss;
			GetFeed(rssList.rssId, out rss, out iItems);

			if (rssList.listLimitItems && (rssList.listReadMorePageId.HasValue) && (iItems.Count() > rssList.listMaxItems.Value))
			{

				string strReadMoreText = "Read More";
				if (!string.IsNullOrEmpty(rssList.listReadMoreText))
				{
					strReadMoreText = rssList.listReadMoreText;
				}
				anchorReadMore.InnerHtml = strReadMoreText;

				ScmsSiteMapProvider provider = new ScmsSiteMapProvider();
				ScmsSiteMapProvider.PageNode pageNode;
				string strError;
				Exception exError;
				if (provider.GetPageNode(rssList.listReadMorePageId.Value, out pageNode, out strError, out exError))
				{
					anchorReadMore.HRef = pageNode.page.url;
				}
			}
			else
			{
				divReadMore.Visible = false;
			}

			if (rssList.listLimitItems)
			{
				iItems = iItems.Take(rssList.listMaxItems.Value);
			}

			if (rssList.templateEnabled)
			{
				mv.SetActiveView(viewItemTemplated);

				literalHeaderHtml.Text = rssList.templateHeaderHtml;
				literalFooterHtml.Text = rssList.templateFooterHtml;

				rptItemsTemplated.DataSource = iItems;
				rptItemsTemplated.DataBind();
			}
			else
			{
				if (rssList.headingEnabled && !string.IsNullOrEmpty(rss.heading))
				{
					divTitle.InnerText = rss.heading;
				}
				else
				{
					divTitle.Visible = false;
				}

				rptItems.DataSource = iItems;
				rptItems.DataBind();
			}
		}

	}
}