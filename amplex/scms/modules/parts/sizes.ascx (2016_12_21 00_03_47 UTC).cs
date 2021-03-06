using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using global::scms;

namespace amplex.scms.modules.parts
{
	public partial class Sizes : System.Web.UI.UserControl
	{


		protected void Page_Load(object sender, EventArgs e)
		{
			statusMessage.Clear();
			if (!IsPostBack)
			{
				LoadItems();
			}
		}

		protected class SizeListViewItem
		{
			public amplex.scms.modules.parts.classes.cat_size size
			{
				get;
				set;
			}

			public bool InUse
			{
				get;
				set;
			}
		}

		protected void LoadItems()
		{
			try
			{
				System.Collections.Generic.List<SizeListViewItem> lItems = new List<SizeListViewItem>();

				amplex.scms.modules.parts.classes.partsDataContext dc = new amplex.scms.modules.parts.classes.partsDataContext();
				var sizesPlusCount = 
					from s in dc.cat_sizes
					join p in dc.cat_parts on s.id equals p.sage_ProductCategoryDesc2 into g
					orderby s.ordinal
					select new  { s, g };
				foreach (var spc in sizesPlusCount)
				{
					int n = spc.g.Count();
					SizeListViewItem item = new SizeListViewItem { size = spc.s, InUse = n > 0 };
					lItems.Add(item);
				}

				rptData.DataSource = lItems;
				rptData.DataBind();
			}
			catch (Exception ex)
			{
				ScmsEvent.Raise("Failed loading", this, ex);
				statusMessage.ShowFailure("Failed loading, see event log for more information.");
			}
		}

		protected string strPartViewUrl = null;
		
		protected void rptReferences_ItemDataBound(object sender, RepeaterItemEventArgs args)
		{
			RepeaterItem rptItem = args.Item;
			switch (rptItem.ItemType)
			{

				case ListItemType.Item:
				case ListItemType.AlternatingItem:
					{
						amplex.scms.modules.parts.classes.cat_part part = (amplex.scms.modules.parts.classes.cat_part)args.Item.DataItem;
						System.Web.UI.HtmlControls.HtmlAnchor anchor = (System.Web.UI.HtmlControls.HtmlAnchor)args.Item.FindControl("anchorPart");
						anchor.HRef = string.Format("{0}?p={1}", strPartViewUrl, part.sage_ID);
						anchor.InnerText = part.sage_Description1;

					}
					break;

			}
		}

		protected void rptData_ItemDataBound(object sender, RepeaterItemEventArgs args)
		{
			RepeaterItem rptItem = args.Item;
			switch (rptItem.ItemType)
			{

				case ListItemType.Item:
				case ListItemType.AlternatingItem:
				{
					SizeListViewItem slvItem = (SizeListViewItem)args.Item.DataItem;
					ImageButton btnDelete = (ImageButton)rptItem.FindControl("btnDelete");
					if (btnDelete != null)
					{
						if (slvItem.InUse)
						{
							btnDelete.ImageUrl = "/scms/client/images/action_delete_disabled.gif";
							btnDelete.Enabled = false;
						}
					}
				}
				break;

			}
		}

		protected void Delete_Command(object sender, CommandEventArgs args)
		{
			try
			{
				amplex.scms.modules.parts.classes.partsDataContext dc = new amplex.scms.modules.parts.classes.partsDataContext();
				var size = (from s in dc.cat_sizes
										where s.id == args.CommandArgument.ToString()
										select s).FirstOrDefault();

				if (size == null)
				{
					statusMessage.ShowFailure("size not found");
				}
				else
				{
					var partInUse = (from p in dc.cat_parts
													 where p.sage_ProductCategoryDesc2 == size.id
													 select p).FirstOrDefault();

					if (partInUse != null)
					{
						statusMessage.ShowFailure("size is in use");
					}
					else
					{
						dc.cat_sizes.DeleteOnSubmit(size);
						dc.SubmitChanges();

						LoadItems();
						multiView.SetActiveView(viewList);
					}
				}
			}
			catch (Exception ex)
			{
				ScmsEvent.Raise("Failed deleting", this, ex);
				statusMessage.ShowFailure("Failed deleting, see event log for more information.");
			}
		}

		protected void Edit_Command(object sender, CommandEventArgs args)
		{
			try
			{
				amplex.scms.modules.parts.classes.partsDataContext dc = new amplex.scms.modules.parts.classes.partsDataContext();
				var size = (from s in dc.cat_sizes
										where s.id == args.CommandArgument.ToString()
										select s).FirstOrDefault();

				if (size == null)
				{
					statusMessage.ShowFailure("size not found");
				}
				else
				{
					bool bInUse = false;
					var part = (from p in dc.cat_parts
											where p.sage_ProductCategoryDesc2 == size.id
											select p).FirstOrDefault();
					bInUse = part != null;

					hiddenId.Value = size.id;
					txtId.Text = size.id;
					txtName.Text = size.name;

					if (bInUse)
					{
						txtId.Enabled = false;
						txtName.Focus();
					}
					else
					{
						txtId.Enabled = true;
						txtId.Focus();
					}

					multiView.SetActiveView(viewEdit);
				}
			}
			catch (Exception ex)
			{
				ScmsEvent.Raise("Failed loading item", this, ex);
				statusMessage.ShowFailure("Failed loading item, see event log for more information.");
			}
				
		}

		protected string GetDeleteMessage(object obj)
		{
			SizeListViewItem slvItem = (SizeListViewItem)obj;

			string strSize = string.Format("{0} {1}", slvItem.size.id, slvItem.size.name );
			string strMessage = string.Format("javascript: return confirm(\"Delete size '{0}'?\");", strSize );
			return strMessage;
		}

		protected void btnNew_Click(object sender, EventArgs args)
		{
			hiddenId.Value = null;
			txtId.Text = null;
			txtId.Enabled = true;
			txtName.Text = null;
			multiView.SetActiveView(viewEdit);
		}

		protected void btnSave_Click(object sender, EventArgs args)
		{
			try
			{
				string strId = txtId.Text.Trim();
				amplex.scms.modules.parts.classes.cat_size size = null;

				amplex.scms.modules.parts.classes.partsDataContext dc = new amplex.scms.modules.parts.classes.partsDataContext();



				bool bAnyErrors = false;

				string strIdPrevious = hiddenId.Value;
				string strIdCurrent = txtId.Text.Trim();

				// if new or changed
				if (string.Compare(strIdPrevious, strIdCurrent, true) != 0 )
				{

					amplex.scms.modules.parts.classes.cat_size sizeCurrent = null;
					sizeCurrent = (from s in dc.cat_sizes
												 where s.id == strIdCurrent
												 select s).FirstOrDefault();

					if (sizeCurrent != null)
					{
						bAnyErrors = true;
						string strMessage = string.Format("size id '{0}' already exists", strIdCurrent);
						statusMessage.ShowFailure(strMessage);
					}
				}

				// if new
				if (!bAnyErrors)
				{
					if (string.IsNullOrEmpty(strIdPrevious))
					{
						var sizeMax = (from s in dc.cat_sizes
													 orderby s.ordinal descending
													 select s).FirstOrDefault();
						int nOrdinal = 1;
						if (sizeMax != null)
						{
							nOrdinal = sizeMax.ordinal + 1;
						}

						size = new amplex.scms.modules.parts.classes.cat_size();
						size.id = strIdCurrent;
						size.ordinal = nOrdinal;
						dc.cat_sizes.InsertOnSubmit(size);
					}
					else
					{
						size = (from s in dc.cat_sizes
										where s.id == strIdCurrent
										select s).FirstOrDefault();

						if (size == null)
						{
							bAnyErrors = true;
							statusMessage.ShowFailure("size not found");
						}
					}
				}

				if (!bAnyErrors)
				{
					size.name = txtName.Text.Trim();
					dc.SubmitChanges();
					statusMessage.ShowSuccess("item updated");
					multiView.SetActiveView(viewList);
					LoadItems();
				}
				
			}
			catch (Exception ex)
			{
				ScmsEvent.Raise("Failed updating item", this, ex);
				statusMessage.ShowFailure("Failed updating item, see event log for details");
			}
		}

		protected void btnCancel_Click(object sender, EventArgs args)
		{
			multiView.SetActiveView(viewList);
		}

		protected void ShowReferences(object objSender, CommandEventArgs args)
		{
			LoadSettings();

			string strSizeId = args.CommandArgument.ToString();
			amplex.scms.modules.parts.classes.partsDataContext dc = new amplex.scms.modules.parts.classes.partsDataContext();
			var parts = from p in dc.cat_parts
									where p.sage_ProductCategoryDesc2 == strSizeId
									select p;
			
			rptReferences.DataSource = parts;
			rptReferences.DataBind();

			multiView.SetActiveView(viewReferences);

		}

		protected void LoadSettings()
		{
			try
			{
				amplex.scms.modules.parts.classes.partsDataContext dcParts = new amplex.scms.modules.parts.classes.partsDataContext();
				var settings = (from s in dcParts.cat_settings
										select s).FirstOrDefault();
				global::scms.ScmsSiteMapProvider siteMapProvider = new ScmsSiteMapProvider();
				global::scms.ScmsSiteMapProvider.PageNode pageNode = null;
				string strError;
				Exception exError;
				if (!siteMapProvider.GetPageNode(settings.searchResultsPageId.Value, out pageNode, out strError, out exError))
				{
					throw new Exception(string.Format("getpagenode failed: '{0}'", strError), exError);
				}
				strPartViewUrl = pageNode.page.url;
			}
			catch (Exception ex)
			{
				global::scms.ScmsEvent.Raise("Exception thrown while loading settings", this, ex);
			}
		}

		protected void Move(object objSender, CommandEventArgs args)
		{
			bool bUp = string.Compare(args.CommandName, "up", true) == 0;
			string strId = (string)args.CommandArgument;

			amplex.scms.modules.parts.classes.partsDataContext dc = new amplex.scms.modules.parts.classes.partsDataContext();
			var sizes = from s in dc.cat_sizes
									orderby s.ordinal
									select s;

			amplex.scms.modules.parts.classes.cat_size priorSize = null;
			amplex.scms.modules.parts.classes.cat_size nextSize = null;

			var thisSize = sizes.Where(s => (s.id == strId)).Single();

			int nOrdinal = 0;
			bool bFound = false;
			foreach (var size in sizes)
			{
				size.ordinal = nOrdinal;

				if (string.Compare(size.id, thisSize.id, true) == 0 )
				{
					bFound = true;
				}
				else
				{
					if (bFound)
					{
						if (nextSize == null)
						{
							nextSize = size;
						}
					}
					else
					{
						priorSize = size;
					}
				}

				nOrdinal++;
			}

			if (bUp)
			{
				if (priorSize != null)
				{
					int nOrdinalTemp = thisSize.ordinal;
					thisSize.ordinal = priorSize.ordinal;
					priorSize.ordinal = nOrdinalTemp;
				}
			}
			else
			{
				if (nextSize != null)
				{
					int nOrdinalTemp = thisSize.ordinal;
					thisSize.ordinal = nextSize.ordinal;
					nextSize.ordinal = nOrdinalTemp;
				}
			}

			dc.SubmitChanges();
			
			global::scms.CacheManager.Clear();
			LoadItems();
		}

	}

		
}