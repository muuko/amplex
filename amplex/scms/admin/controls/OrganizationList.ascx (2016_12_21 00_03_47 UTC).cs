using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Security;

namespace scms.admin.controls
{
  public partial class OrganizationList : System.Web.UI.UserControl
	{
		protected void Page_Load(object sender, EventArgs e)
		{
				if (!IsPostBack)
				{
						Load();
				}
		}

		protected void Load()
		{
				try
				{
					scms.data.ScmsDataContext dc = new scms.data.ScmsDataContext();
					var orgs = from o in dc.scms_orgs
										 where o.deleted == false
										 orderby o.name
										 select o;

					rpt.DataSource = orgs;
					rpt.DataBind();

				}
				catch (Exception ex)
				{
						global::scms.ScmsEvent.Raise("Exception thrown while loading roles", this, ex);
				}
		}

		protected void btn_Command(object sender, CommandEventArgs args)
		{
			try
			{
				string strOrgId = (string)args.CommandArgument;
				string strUrl = "/scms/admin/security.aspx";
				switch (args.CommandName.ToLower())
				{
					case "users":
						strUrl = string.Format("/scms/admin/security.aspx?view=users&o={0}", strOrgId);
						break;

					case "edit":
						strUrl = string.Format("/scms/admin/organization.aspx?o={0}", strOrgId );
						break;

					default:
						throw new Exception(string.Format( "unknown command name: '{0}'", args.CommandName));
				}

				
				Response.Redirect(strUrl, true);
			}
			catch (System.Threading.ThreadAbortException)
			{
			}
			catch (Exception ex)
			{
				string strMessage = string.Format("An error occurred while creating the organization");
				statusMessage.ShowFailure(strMessage);
				global::scms.ScmsEvent.Raise(strMessage, this, ex);
			}
		}
    

		protected void btnNew_Click(object sender, EventArgs args)
		{
				try
				{
					string strOrganizationName = txtName.Text;

					scms.data.ScmsDataContext dc = new scms.data.ScmsDataContext();
					var orgExisting = (from o in dc.scms_orgs
														where o.name.ToLower() == strOrganizationName.ToLower()
														where o.deleted == false
														select o).FirstOrDefault();

					if( orgExisting == null )
					{
						scms.data.scms_org org = new scms.data.scms_org();
						dc.scms_orgs.InsertOnSubmit(org);
						org.name = strOrganizationName;

						dc.SubmitChanges();
						txtName.Text = null;
						Load();
						string strMessage = string.Format("organization '{0}' created", strOrganizationName);
						statusMessage.ShowSuccess(strMessage);
					}
					else
					{
							string strMessage = string.Format("an organization named '{0}' already exists", strOrganizationName);
							statusMessage.ShowFailure(strMessage);
					}
				}
				catch (Exception ex)
				{
						string strMessage = string.Format("An error occurred while creating the organization");
						statusMessage.ShowFailure(strMessage);
						global::scms.ScmsEvent.Raise(strMessage, this, ex);
				}
		}

		protected void Delete_Command(object sender, CommandEventArgs args)
		{
			try
			{
				int nOrganizationId = int.Parse((string)args.CommandArgument);
				scms.data.ScmsDataContext dc = new scms.data.ScmsDataContext();
				var org = (from o in dc.scms_orgs
									 where o.id == nOrganizationId
									 where o.deleted == false
									 select o).FirstOrDefault();
				if (org != null)
				{
					string strMessage;
					bool bAnyErrors = false;
					var userNames = from u in dc.scms_users
													where u.orgId ==  org.id
													select u.aspnet_User.UserName;
					foreach (var user in userNames)
					{
						try
						{
							Membership.DeleteUser(user);
						}
						catch(Exception ex)
						{
							strMessage = string.Format("An error occurred while deleting user '{0}'", user);
							global::scms.ScmsEvent.Raise(strMessage, this, ex);
							bAnyErrors = true;
						}
					}

					org.deleted = true;
					dc.SubmitChanges();
					Load();

					strMessage = string.Format("Organization '{0}' and related users deleted.", org.name);
					if (bAnyErrors)
					{
						strMessage += "  WARNING:  Not all users were deleted from the system.";
					}
					statusMessage.ShowSuccess(strMessage);
				}
			}
			catch (Exception ex)
			{
				string strMessage = string.Format("An error occurred while deleting the organization");
				statusMessage.ShowFailure(strMessage);
				global::scms.ScmsEvent.Raise(strMessage, this, ex);
			}
		}

		protected string GetDeleteMessage(object obj)
		{
			scms.data.scms_org org = (scms.data.scms_org)obj;
			string strMessage = string.Format("javascript: return confirm(\"Delete '{0}' organization and all associated users?\");", org.name);
			return strMessage;
		}
	}
}