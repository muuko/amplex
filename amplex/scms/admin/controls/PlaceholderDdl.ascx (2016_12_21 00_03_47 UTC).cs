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

namespace scms.admin.controls
{
	public partial class PlaceholderDdl : System.Web.UI.UserControl
	{
		protected int? nSiteId = null;
		public int? SiteId
		{
			get
			{
				object objSiteId = ViewState["nSiteId"];
				if (objSiteId != null)
				{
					nSiteId = (int)objSiteId;
				}
				return nSiteId;
			}
			set
			{
				nSiteId = value;
				ViewState["nSiteId"] = value;
			}
		}

		protected int? nMasterFileId = null;
		public int? MasterFileId
		{
			get
			{
				object objMasterFileId = ViewState["nMasterFileId"];
				if (objMasterFileId != null)
				{
					nMasterFileId = (int)objMasterFileId;
				}
				return nMasterFileId;
			}
			set
			{
				nMasterFileId = value;
				ViewState["nMasterFileId"] = value;
			}
		}

		protected string selectedValue = null;
		public string SelectedValue
		{
			get { return ddlPlaceholders.SelectedValue; }
			set
			{
				ddlPlaceholders.ClearSelection();

				ListItem listItem = ddlPlaceholders.Items.FindByText(value);
				if (listItem == null)
				{
					listItem = new ListItem("*" + value);
					ddlPlaceholders.Items.Add(listItem);
					literalWarning.Visible = true;
				}
				else
				{
					literalWarning.Visible = false;
				}
				listItem.Selected = true;
			}
		}

		protected void ddlPlaceholders_SelectedIndexChanged(object sender, EventArgs args)
		{
			bool bExistsInMaster = false;

			string strSelectedValue = ddlPlaceholders.SelectedValue;
			if (!string.IsNullOrEmpty(strSelectedValue))
			{
				if (strSelectedValue[0] != '*')
				{
					bExistsInMaster = true;
				}
			}

			literalWarning.Visible = !bExistsInMaster;
		}

		protected void Page_Load(object sender, EventArgs e)
		{
			if (!IsPostBack)
			{
			}
		}

		public void LoadPlaceholders()
		{
			try
			{
				string strError;
				Exception exError;
				global::scms.Master master;

				nSiteId = SiteId;
				nMasterFileId = MasterFileId;
				if (global::scms.Masters.GetMaster(nSiteId.Value, nMasterFileId.Value, out master, out strError, out exError))
				{
					System.Collections.Generic.SortedList<string, string> slPlaceholdersAll;
					System.Collections.Generic.SortedList<string, string> slPlaceholders;
					if (master.GetPlaceholders(out slPlaceholdersAll, out slPlaceholders, out strError, out exError))
					{
						selectedValue = ddlPlaceholders.SelectedValue;
						if (!string.IsNullOrEmpty(selectedValue))
						{
							if (selectedValue[0] == '*')
							{
								selectedValue = selectedValue.Substring(1);
							}
						}

						ddlPlaceholders.Items.Clear();
						if (checkShowAll.Checked)
						{
							ddlPlaceholders.DataSource = slPlaceholdersAll.Values;
						}
						else
						{
							ddlPlaceholders.DataSource = slPlaceholders.Values;
						}
						ddlPlaceholders.DataBind();

						if (!string.IsNullOrEmpty(selectedValue))
						{
							SelectedValue = selectedValue;
						}
					}
				}
			}
			catch( Exception ex )
			{
				scms.ScmsEvent.Raise("failed loading placeholders", this, ex);
			}
		}

		protected void checkShowAll_CheckChanged(object sender, EventArgs args)
		{
			LoadPlaceholders();
		}

	}
}