using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using global::scms;

namespace projectz.scms.modules.rss
{
	public partial class feeds : System.Web.UI.UserControl
	{
		protected void Page_Load(object sender, EventArgs e)
		{
			statusMessage.Clear();
			if (!IsPostBack)
			{
				LoadFeeds();
			}
		}

		protected void LoadFeeds()
		{
			try
			{
				global::scms.data.ScmsDataContext dc = new global::scms.data.ScmsDataContext();
				var feeds = from f in dc.scms_rsses
										orderby f.name
										select f;
				rptData.DataSource = feeds;
				rptData.DataBind();
			}
			catch (Exception ex)
			{
				ScmsEvent.Raise("Failed loading feeds", this, ex);
				statusMessage.ShowFailure("Failed loading feeds, see event log for more information.");
			}
		}

		protected void rptData_ItemDataBound(object sender, RepeaterItemEventArgs args)
		{
		}

		protected void Delete_Command(object sender, CommandEventArgs args)
		{
			try
			{
				int nId = int.Parse((string)args.CommandArgument);

				global::scms.data.ScmsDataContext dc = new global::scms.data.ScmsDataContext();
				var feed = (from f in dc.scms_rsses
										where f.id == nId
										select f).FirstOrDefault();
				if (feed == null)
				{
					statusMessage.ShowFailure("Rss feed not found");
				}
				else
				{
					dc.scms_rsses.DeleteOnSubmit(feed);
					dc.SubmitChanges();

					LoadFeeds();
					multiView.SetActiveView(viewList);
				}
			}
			catch (Exception ex)
			{
				ScmsEvent.Raise("Failed deleting rss feed", this, ex);
				statusMessage.ShowFailure("Failed deleting rss feed, see event log for more information.");
			}
		}

		protected void Edit_Command(object sender, CommandEventArgs args)
		{
			try
			{
				int nId = int.Parse((string)args.CommandArgument);

				global::scms.data.ScmsDataContext dc = new global::scms.data.ScmsDataContext();
				var feed = (from f in dc.scms_rsses
										where f.id == nId
										select f).FirstOrDefault();
				if (feed == null)
				{
					statusMessage.ShowFailure("Rss feed not found");
				}
				else
				{
					litId.Text = feed.id.ToString();
					txtName.Text = feed.name;
					txtHeading.Text = feed.heading;
					txtFeedUrl.Text = feed.feedUrl;

					string strLastUpdate = string.Empty;
					if( feed.lastUpdate.HasValue )
					{
						strLastUpdate = string.Concat( new string [] { feed.lastUpdate.Value.ToShortDateString(), " ", feed.lastUpdate.Value.ToShortTimeString() });
					}
					litLastUpdate.Text = strLastUpdate;
					txtExpiresSeconds.Text = feed.expiresSeconds.ToString();
					checkRetainDropOff.Checked = feed.retainDropOff;
					txtCategories.Text = feed.categories;

					txtFeedUrl.Focus();
					multiView.SetActiveView(viewEdit);
				}
			}
			catch (Exception ex)
			{
				ScmsEvent.Raise("Failed loading rss feed", this, ex);
				statusMessage.ShowFailure("Failed loading rss feed, see event log for more information.");
			}
		}

		protected string GetDeleteMessage(object obj)
		{
			global::scms.data.scms_rss feed = (global::scms.data.scms_rss)obj;

			string strMessage = string.Format("javascript: return confirm(\"Delete feed '{0}'?\");", feed.name);
			return strMessage;
		}

		protected void btnNew_Click(object sender, EventArgs args)
		{
			litId.Text = null;
			txtName.Text = null;
			txtHeading.Text = null;
			txtFeedUrl.Text = null;
			litLastUpdate.Text = null;
			txtExpiresSeconds.Text = "14400"; // 4 hours
			checkRetainDropOff.Checked = false;
			txtCategories.Text = null;
			multiView.SetActiveView(viewEdit);
		}

		protected void btnSave_Click(object sender, EventArgs args)
		{
			try
			{
				global::scms.data.ScmsDataContext dc = new global::scms.data.ScmsDataContext();

				global::scms.data.scms_rss rss = null;
				string strFeedId = litId.Text;
				if (string.IsNullOrEmpty(strFeedId))
				{
					rss = new global::scms.data.scms_rss();
					dc.scms_rsses.InsertOnSubmit(rss);
				}
				else
				{
					int nId = int.Parse(litId.Text);
					rss = (from r in dc.scms_rsses
								 where r.id == nId
								 select r).Single();
				}
				rss.name = txtName.Text.Trim();
				rss.heading = txtHeading.Text.Trim();
				rss.feedUrl = txtFeedUrl.Text.Trim();
				rss.expiresSeconds = int.Parse(txtExpiresSeconds.Text);
				rss.retainDropOff = checkRetainDropOff.Checked;
				string [] astrCategories = txtCategories.Text.Split(new char[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries);
				rss.categories = string.Join(",", astrCategories);

				dc.SubmitChanges();
				statusMessage.ShowSuccess("Feed updated");
				
				
				multiView.SetActiveView(viewList);
				LoadFeeds();
			}
			catch (Exception ex)
			{
				ScmsEvent.Raise("Failed updating feed", this, ex);
				statusMessage.ShowFailure("Failed updating feed, see event log for details");
			}
		}

		protected void btnCancel_Click(object sender, EventArgs args)
		{
			multiView.SetActiveView(viewList);
		}
	}
}