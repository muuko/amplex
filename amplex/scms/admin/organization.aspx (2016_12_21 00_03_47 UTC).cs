using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Xml.Linq;

namespace scms.admin
{
	public partial class Organization : System.Web.UI.Page
	{
		// protected int? nPageId = null;
		// protected global::scms.ScmsSiteMapProvider.PageNode pageNode = null;

		protected void Page_Init(object sender, EventArgs args)
		{
			// LoadOrganizations();
		}

		protected void Page_Load(object sender, EventArgs e)
		{
			Title = "Admin - Security - Organization";

			string strError;
			Exception exError;

			scms.admin.Admin master = (scms.admin.Admin)this.Master;
			master.NavType = Admin.ENavType.Security;

			string strOrganizationId = Request.QueryString["o"];
			int nOrganizationId;
			if (!int.TryParse(strOrganizationId, out nOrganizationId))
			{
				Response.Redirect("/scms/admin/security.aspx");
			}

			if (!IsPostBack)
			{
				LoadOrganization(nOrganizationId);
			}

		}

		protected void LoadOrganizationAttributes(scms.data.ScmsDataContext dc, out System.Collections.Generic.IEnumerable<scms.data.scms_org_attr> ieOrganizationAttributes)
		{
			ieOrganizationAttributes = null;

			try
			{
			}
			catch (Exception ex)
			{
			}

		}

		public class ListItemOrgAttrValue
		{
			public int AttributeId { get; set; }
			public string Name { get; set; }
			public string Value { get; set; }
		}

		protected bool LoadOrganization(int nOrganizationId)
		{
			bool bSuccess = false;
			try
			{
				scms.data.ScmsDataContext dc = new scms.data.ScmsDataContext();

				// get the org
				var org = (from o in dc.scms_orgs 
									 where o.id == nOrganizationId 
									 select o).FirstOrDefault();
				if( org == null )
				{
					throw new Exception(string.Format("unexpected organization id '{0}' not found.", nOrganizationId));
				}
				literalName.Text = org.name;

				// get the values
				var nvOrgAttrValues = from oa in dc.scms_org_attrs
															where oa.id == nOrganizationId
															join av in dc.scms_org_attr_values on oa.id equals av.attrId into gj
															from subav in gj.DefaultIfEmpty()
															select new ListItemOrgAttrValue { AttributeId = oa.id, Name= oa.name, Value = (subav == null) ? string.Empty : subav.value };
				lvOrganizations.DataSource = nvOrgAttrValues;
				lvOrganizations.DataBind();
			}
			catch (Exception ex)
			{
				string strMessage = string.Format("Exception thrown while loading organization '{0}'", nOrganizationId);
				ScmsEvent.Raise(strMessage, this, ex);
			}

			return bSuccess;
		}

		protected void lvOrganizations_ItemDataBound(object sender, ListViewItemEventArgs args)
		{
			switch (args.Item.ItemType)
			{
				case ListViewItemType.DataItem:
					ListViewDataItem dataItem = (ListViewDataItem)args.Item;
					ListItemOrgAttrValue oav = (ListItemOrgAttrValue)dataItem.DataItem;

					Literal literalAttribute = (Literal)args.Item.FindControl("literalAttribute");
					TextBox txtAttributeValue = (TextBox)args.Item.FindControl("txtAttributeValue");
					HiddenField hiddenAttributeId = (HiddenField)args.Item.FindControl("hiddenAttributeId");
					hiddenAttributeId.Value = oav.AttributeId.ToString();
					literalAttribute.Text = oav.Name;
					txtAttributeValue.Text = oav.Value;

					break;
			}
		}


		protected void btnSave_Click(object sender, EventArgs args)
		{
			int nOrganizationId = int.Parse(Request.QueryString["o"]);
			
			try
			{
				scms.data.ScmsDataContext dc = new scms.data.ScmsDataContext();

				var org = (from o in dc.scms_orgs
									 where o.id == nOrganizationId
									 select o).FirstOrDefault();
				if (org == null)
				{
					throw new Exception( string.Format( "unexpected missing organization '{0}'.", nOrganizationId ));
				}

				foreach (var item in lvOrganizations.Items)
				{
					TextBox txtAttributeValue = (TextBox)item.FindControl("txtAttributeValue");
					HiddenField hiddenAttributeId = (HiddenField)item.FindControl("hiddenAttributeId");
					string strAttributeValue = txtAttributeValue.Text;
					int nAttributeID = int.Parse(hiddenAttributeId.Value);

					var attributeValue = (from oav in dc.scms_org_attr_values
										 where oav.orgId == nOrganizationId
										 where oav.attrId == nAttributeID
										 select oav).FirstOrDefault();
					if (attributeValue != null)
					{
						if (string.IsNullOrEmpty(strAttributeValue))
						{
							dc.scms_org_attr_values.DeleteOnSubmit(attributeValue);
						}
						else
						{
							if (string.Compare(attributeValue.value, strAttributeValue) != 0)
							{
								attributeValue.value = strAttributeValue;
							}
						}
					}
					else
					{
						if (!string.IsNullOrEmpty(strAttributeValue))
						{
							attributeValue = new scms.data.scms_org_attr_value { attrId = nAttributeID, orgId = nOrganizationId, value = strAttributeValue };
							dc.scms_org_attr_values.InsertOnSubmit(attributeValue);
						}
					}
					dc.SubmitChanges();
					statusMessage.ShowSuccess("Organization saved");
				}
			}
			catch (Exception ex)
			{
				string strMessage = string.Format("Exception thrown while saving organization '{0}'", nOrganizationId);
				ScmsEvent.Raise(strMessage, this, ex);
				statusMessage.ShowFailure("failed saving organization");
			}
			
		}
	}
}
