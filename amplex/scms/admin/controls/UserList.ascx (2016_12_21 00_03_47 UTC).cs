using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Security;

namespace scms.admin.controls
{
  public partial class UserList : System.Web.UI.UserControl
	{
    protected int nPageSize = 0;
    protected int nCurrentPage = 0;

    protected int? PageNumber
    {
      get { return (int?)ViewState["PageNumber"]; }
      set { ViewState["PageNumber"] = value; }
    }

    protected void Page_Load(object sender, EventArgs e)
	  {
      if (!IsPostBack)
      {
				LoadRoles();
				LoadOrganizations();

        // preload if requested in url
        string strView = Request.QueryString["view"];
        if (string.Compare(strView, "users", true) == 0)
        {
          string strRole = Request.QueryString["r"];
          if (!string.IsNullOrEmpty(strRole))
          {
            ListItem liRole = ddlSearchRole.Items.FindByValue(strRole);
            if (liRole != null)
            {
              ddlSearchRole.ClearSelection();
              liRole.Selected = true;
            }
          }

					string strOrganizationId = Request.QueryString["o"];
					if (!string.IsNullOrEmpty(strOrganizationId))
					{
						ListItem liOrganization = ddlSearchOrganization.Items.FindByValue(strOrganizationId);
						if (liOrganization != null)
						{
							ddlSearchOrganization.ClearSelection();
							liOrganization.Selected = true;
						}
					}
        }

        LoadUsers();
      }
		}

    protected void LoadRoles()
	  {
      try
      {
        ddlSearchRole.Items.Add(new ListItem("(any)", ""));
        ddlSearchRole.AppendDataBoundItems = true;
        string [] astrRoles = Roles.GetAllRoles();
        ddlSearchRole.DataSource = astrRoles;
        ddlSearchRole.DataBind();
      }
      catch (Exception ex)
      {
				global::scms.ScmsEvent.Raise("Exception thrown while loading roles", this, ex);
      }
		}

		protected void LoadOrganizations()
		{
			try
			{
				
				ddlSearchOrganization.AppendDataBoundItems = true;
				ddlSearchOrganization.Items.Add(new ListItem("(any)", ""));

				scms.data.ScmsDataContext dc = new scms.data.ScmsDataContext();
				var orgs = from o in dc.scms_orgs
									 where o.deleted == false
									 orderby o.name
									 select o;
				ddlSearchOrganization.DataTextField = "name";
				ddlSearchOrganization.DataValueField = "id";
				ddlSearchOrganization.DataSource = orgs;
				ddlSearchOrganization.DataBind();
			}
			catch (Exception ex)
			{
				global::scms.ScmsEvent.Raise("Exception thrown while loading organizations", this, ex);
			}
		}

    protected class RepeaterUser
    {
        
        public Guid userId
        {
            get;
            set;
        }

        public string username
        {
            get;
            set;
        }
        
        public string email
        {
            get;
            set;
        }

        public string firstName
        {
            get;
            set;
        }

        public string lastName
        {
            get;
            set;
        }
    }

		protected void LoadUsers()
	  {
      try
	    {
        bool bShowPager = false;

        global::scms.data.ScmsDataContext dc = new global::scms.data.ScmsDataContext();

        string strSearchUserName = txtSearchUserName.Text.Trim();
        string strSearchEmailAddress = txtSearchEmailAddress.Text.Trim();
        string strSearchFirstName = txtSearchFirstName.Text.Trim();
        string strSearchLastName = txtSearchLastName.Text.Trim();
				string strSearchOrganizationId = ddlSearchOrganization.SelectedValue;
				int? nSearchOrganizationId = null;
				if (!string.IsNullOrEmpty(strSearchOrganizationId))
				{
					nSearchOrganizationId = int.Parse(strSearchOrganizationId);
				}

        string strSearchRole = null;
        if( !string.IsNullOrEmpty( ddlSearchRole.SelectedValue ))
        {
					strSearchRole = ddlSearchRole.SelectedValue;
        }

        bool bAscending = true;

        int ? nCount = 0;

        var users = dc.scms_get_users(
          strSearchUserName,
          strSearchEmailAddress,
          strSearchFirstName,
          strSearchLastName,
          strSearchRole,
					nSearchOrganizationId,
          nCurrentPage,
          nPageSize,
          "username",
          bAscending,
          ref nCount);

        rptUsers.DataSource = users.Select(u => 
            new RepeaterUser
            { 
                userId = u.userid, 
                username = u.username,
                email = u.email,
                firstName = u.firstName,
                lastName = u.lastName
            }).ToArray();
            
        rptUsers.DataBind();

        if (bShowPager)
        {
            if (nPageSize > 0)
            {
                int nPages = 1 + ((nCount.Value - 1) / nPageSize);
                if (nPages > 1)
                {
                    bShowPager = true;
                    System.Collections.Generic.List<int> lPages = new System.Collections.Generic.List<int>();
                    for (int nPage = 0; nPage < nPages; nPage++)
                    {
                        lPages.Add(nPage);
                    }
                    rptPager.DataSource = lPages;
                    rptPager.DataBind();
                }
            }
            else
            {
                bShowPager = false;
            }
        }

        divPager.Visible = bShowPager;
			}
      catch (Exception ex)
      {
          global::scms.ScmsEvent.Raise("Exception thrown while loading users", this, ex);
      }
		}

    protected void btnNewUser_Click(object sender, EventArgs args)
    {
        try
        {
            string strUserName = txtUserName.Text.Trim();

            if (Membership.GetUser(strUserName) == null)
            {
                string strPassword = txtPassword.Text.Trim();
                MembershipUser user = Membership.CreateUser(strUserName, strPassword);
                string strUrl = string.Format("/scms/admin/user.aspx?u={0}&p={1}", user.UserName, strPassword);

                Response.Redirect(strUrl, true);
            }
            else
            {
                string strMessage = string.Format("a user named '{0}' already exists", strUserName);
                statusMessage.ShowFailure(strMessage);
            }
        }
        catch (System.Threading.ThreadAbortException)
        {
        }
        catch (Exception ex)
        {
            string strMessage = string.Format("An error occurred while creating the user.  The password may not meet minimum requirements.");
            statusMessage.ShowFailure(strMessage);
            global::scms.ScmsEvent.Raise(strMessage, this, ex);
        }
    }

    protected void Delete_Command(object sender, CommandEventArgs args)
    {
        try
        {
            string strUser = (string)args.CommandArgument;

            bool bLastAdministrator = false;
            if (Roles.IsUserInRole("Administrators"))
            {
                string[] astrAdministrators = Roles.GetUsersInRole("Administrators");
                if (astrAdministrators.Length <= 1)
                {
                    bLastAdministrator = true;
                }
            }

            if (bLastAdministrator)
            {
                string strMessage = string.Format( "User '{0}' is the last administrator and cannot be deleted.", strUser);
                statusMessage.ShowFailure(strMessage);
            }
            else
            {
                if (Membership.DeleteUser(strUser, true))
                {
                    LoadUsers();
                    string strMessage = string.Format("User '{0}' deleted", strUser);
                    statusMessage.ShowSuccess(strMessage);
                }
                else
                {
                    string strMessage = string.Format("Failed deleting user '{0}'", strUser);
                    statusMessage.ShowFailure(strMessage);
                }
            }
        }
        catch (Exception ex)
        {
            string strMessage = string.Format("An error occurred while deleting the user");
            statusMessage.ShowFailure(strMessage);
            global::scms.ScmsEvent.Raise(strMessage, this, ex);
        }
    }

    protected void Edit_Command(object sender, CommandEventArgs args)
    {
        try
        {
            string strUserName = (string)args.CommandArgument;
            string strUrl = string.Format("/scms/admin/user.aspx?u={0}", strUserName);
            Response.Redirect(strUrl, true);
        }
        catch (System.Threading.ThreadAbortException)
        {
        }
        catch (Exception ex)
        {
            string strMessage = string.Format("An error occurred while editing the user");
            statusMessage.ShowFailure(strMessage);
            global::scms.ScmsEvent.Raise(strMessage, this, ex);
        }
    }


    protected string GetDeleteMessage(object obj)
    {
        RepeaterUser user = (RepeaterUser)obj;
        string strMessage = string.Format("javascript: return confirm(\"Delete user '{0}'?\");", user.username);
        return strMessage;
    }

    protected void rptUsers_ItemDataBound(object sender, RepeaterItemEventArgs args)
    {
        RepeaterUser user = (RepeaterUser)args.Item.DataItem;

        System.Web.UI.HtmlControls.HtmlAnchor anchorUserName = (System.Web.UI.HtmlControls.HtmlAnchor)args.Item.FindControl("anchorUserName");
        if (anchorUserName != null)
        {
            anchorUserName.HRef = string.Format("/scms/admin/user.aspx?u={0}", user.username);
            anchorUserName.InnerText = user.username;
        }
    }

    protected void btnSearch_Click(object sender, EventArgs args)
    {
        LoadUsers();
    }

    protected void lbPage_PageSelected(object sender, CommandEventArgs args)
    {
        int nPage = int.Parse(args.CommandArgument.ToString());
        nCurrentPage = nPage;
        LoadUsers();
    }



	}	
}