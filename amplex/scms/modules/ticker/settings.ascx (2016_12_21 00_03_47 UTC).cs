using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace scms.modules.ticker
{
  public partial class settings : scms.RootControl
  {

		protected void Page_Load(object sender, EventArgs e)
    {
			statusMessage.Clear();
			pageLink.SiteId = this.SiteId;
      if (!IsPostBack)
      {
				LoadSettings();
				mv.SetActiveView(viewList);
      }
    }

    protected void LoadSettings()
    {
			try
      {
				if (!SiteId.HasValue)
              throw new Exception("unexpected no value for siteid");

				tickerDataContext dc = new scms.modules.ticker.tickerDataContext();

				var tickerEntries = from te in dc.ticker_entries
														orderby te.ordinal
														select te;
				lvMarquee.DataSource = tickerEntries;
				lvMarquee.DataBind();

				
				


/*
				amplex.scms.modules.parts.classes.partsDataContext partsDc = new amplex.scms.modules.parts.classes.partsDataContext();
				amplex.scms.modules.parts.classes.cat_setting settings = (from s in partsDc.cat_settings
																																 where s.siteId == SiteId.Value
																																 select s).FirstOrDefault();
 */ 
      }
      catch (Exception ex)
      {
          ScmsEvent.Raise("failed loading settings", this, ex);
          statusMessage.ShowFailure("Failed loading parts settings");
      }
    }

		protected void lvMarquee_ItemDataBound(object sender, ListViewItemEventArgs args)
		{
			ListViewItem lvItem = args.Item;
			switch (lvItem.ItemType)
			{
				case ListViewItemType.DataItem:
					{
						ListViewDataItem lvDataItem = (ListViewDataItem)lvItem;
						ticker_entry tickerEntry = (ticker_entry)lvDataItem.DataItem;

						if (tickerEntry.pageId.HasValue)
						{
							global::scms.ScmsSiteMapProvider provider = new ScmsSiteMapProvider();
							string strError;
							Exception exError;
							global::scms.ScmsSiteMapProvider.PageNode pageNode;
							if (provider.GetPageNode(tickerEntry.pageId.Value, out pageNode, out strError, out exError))
							{
								Literal litLink = (Literal)lvItem.FindControl("litLink");
								litLink.Text = pageNode.page.url;
							}
						}

						if (!string.IsNullOrEmpty(tickerEntry.url))
						{
							Literal litUrl = (Literal)lvItem.FindControl("litUrl");

							litUrl.Text = string.Format("<a target=\"_blank\" href=\"{0}\">{0}</a>", tickerEntry.url);
						}
					}
					break;
			}
		}

		protected void btnNew_Click(object sender, EventArgs args)
		{
			litId.Text = null;
			txtLabel.Text = null;
			txtValue.Text = null;
			pageLink.PageId = null;
			txtUrl.Text = null;
			mv.SetActiveView(viewEdit);
		}

		protected void btnCancel_Click(object sender, EventArgs args)
		{
			LoadSettings();
			mv.SetActiveView(viewList);
		}

		protected void btnSave_Click(object sender, EventArgs args)
		{
			if (pageLink.PageId.HasValue && !string.IsNullOrEmpty(txtUrl.Text))
			{
				statusMessage.ShowFailure("Please choose only one of an internal Link or and external Url (or neither).");
			}
			else
			{

				try
				{
					tickerDataContext dc = new scms.modules.ticker.tickerDataContext();

					int? nId = null;
					if (!string.IsNullOrEmpty(litId.Text))
					{
						nId = int.Parse(litId.Text);
					}

					ticker_entry tickerEntry = null;
					if (nId.HasValue)
					{
						tickerEntry = (from te in dc.ticker_entries
													 where te.id == nId.Value
													 select te).FirstOrDefault();
					}

					if (tickerEntry == null)
					{
						tickerEntry = new ticker_entry();
						dc.ticker_entries.InsertOnSubmit(tickerEntry);
						int nOrdinal = (from te in dc.ticker_entries
														orderby te.ordinal descending
														select te.ordinal).FirstOrDefault();
						tickerEntry.ordinal = nOrdinal + 1;
					}

					tickerEntry.label = txtLabel.Text;
					tickerEntry.value = txtValue.Text;
					tickerEntry.pageId = pageLink.PageId;
					if (!tickerEntry.pageId.HasValue)
					{
						string strUrl = txtUrl.Text.Trim();
						if (!string.IsNullOrEmpty(strUrl))
						{
							if (!strUrl.StartsWith("/"))
							{
								if (!strUrl.StartsWith("http", StringComparison.OrdinalIgnoreCase))
								{
									strUrl = string.Concat(new string[] { "http://", strUrl });
								}
							}
						}
						tickerEntry.url = strUrl;
					}

					dc.SubmitChanges();

					LoadSettings();
					mv.SetActiveView(viewList);

				}
				catch (Exception ex)
				{
					ScmsEvent.Raise("failed saving ticker settings", this, ex);
					statusMessage.ShowFailure("Failed saving ticker settings");
				}
			}
		}

		protected void onCommand(object objSender, CommandEventArgs args)
		{
			int nId;
			nId = int.Parse(args.CommandArgument.ToString());
			switch (args.CommandName.ToLower())
			{
				case "up":
					Move(nId, true);
					break;

				case "down":
					Move(nId, false);
					break;

				case "customdelete":
					Delete(nId);
					break;

				case "customedit":
					Edit(nId);
					break;

			}
		}

		protected void Edit(int nId)
		{
			try
			{
				tickerDataContext dc = new scms.modules.ticker.tickerDataContext();

				var tickerEntry = (from te in dc.ticker_entries
													 where te.id == nId
													 select te).FirstOrDefault();

				if (tickerEntry == null)
				{
					statusMessage.ShowFailure("Ticker entry not found, reloading");
				}
				else
				{
					litId.Text = tickerEntry.id.ToString();
					txtLabel.Text = tickerEntry.label;
					txtValue.Text = tickerEntry.value;
					pageLink.PageId = tickerEntry.pageId;
					txtUrl.Text = tickerEntry.url;
					mv.SetActiveView(viewEdit);
				}
			}
			catch (Exception ex)
			{
				ScmsEvent.Raise("failed loading ticker enter", this, ex);
				statusMessage.ShowFailure("Failed loading ticker entry, see system event log for more info");
			}
		}

		protected void Delete(int nId)
		{
			try
			{
				tickerDataContext dc = new scms.modules.ticker.tickerDataContext();

				var tickerEntry = (from te in dc.ticker_entries
													 where te.id == nId
													 select te).Single();
				dc.ticker_entries.DeleteOnSubmit(tickerEntry);
				dc.SubmitChanges();
			}
			catch (Exception ex)
			{
				ScmsEvent.Raise("failed deleting ticker enter", this, ex);
				statusMessage.ShowFailure("Failed deleting ticker entry, see system event log for more info");
			}
			finally
			{
				LoadSettings();
			}
		}

		protected void Move(int nId, bool bUp)
		{
			try
			{
				tickerDataContext dc = new scms.modules.ticker.tickerDataContext();

				var tickerEntries = from te in dc.ticker_entries
														orderby te.ordinal
														select te;
				
				int nOrdinal = 0;
				ticker_entry tickerToMove = null;
				foreach (var ticker in tickerEntries)
				{
					ticker.ordinal = nOrdinal;
					if( ticker.id == nId )
					{
						tickerToMove = ticker;
					}

					nOrdinal++;
				}

				bool bSkip = false;
				if (bUp)
				{
					// if first skip
					if (tickerToMove.ordinal == 0)
					{
						bSkip = true;
					}
				}
				else
				{
					// if last
					if (tickerToMove.ordinal == (nOrdinal - 1))
					{
						bSkip = true;
					}
				}

				if (!bSkip)
				{
					foreach (var ticker in tickerEntries)
					{
						if (bUp)
						{
							if (ticker.ordinal == (tickerToMove.ordinal - 1))
							{
								ticker.ordinal = ticker.ordinal + 1;
								tickerToMove.ordinal -= 1;
								break;
							}
						}
						else
						{
							if (ticker.ordinal == (tickerToMove.ordinal + 1))
							{
								ticker.ordinal -= 1;
								tickerToMove.ordinal +=1;
								break;
							}
						}
					}

					dc.SubmitChanges();
					LoadSettings();
				}

			}
			catch (Exception ex)
			{
				ScmsEvent.Raise("failed moving ticker enter", this, ex);
				statusMessage.ShowFailure("failed moving ticker entry, see event log for more info");
			}
		}

		

	}
}