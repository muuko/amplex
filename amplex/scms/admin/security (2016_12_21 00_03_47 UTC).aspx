<%@ Page Language="C#" MasterPageFile="~/scms/admin/Admin.Master" AutoEventWireup="true" CodeBehind="security.aspx.cs" Inherits="scms.admin.Security" Title="Admin - Security" %>
<%@ Register Src="~/scms/admin/controls/RolesList.ascx" TagPrefix="uc" TagName="rolesList" %>
<%@ Register Src="~/scms/admin/controls/UserList.ascx" TagPrefix="uc" TagName="userList" %>
<%@ Register Src="~/scms/admin/controls/OrganizationList.ascx" TagPrefix="uc" TagName="organizationList" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

	<hr />
	
	<asp:Menu 
		ID="menuTabs" 
		runat="server"
		Orientation="Horizontal"
		OnMenuItemClick="menuTabs_Click"
		StaticMenuItemStyle-CssClass="tabbed-menu"
		StaticHoverStyle-CssClass="tabbed-menu-hover"
		StaticSelectedStyle-CssClass="tabbed-menu-selected"
	>
		<Items>
			<asp:MenuItem Text="Users" Value="users" Selected="true"></asp:MenuItem>
			<asp:MenuItem Text="Roles" Value="roles"></asp:MenuItem>
			<asp:MenuItem Text="Organizations" Value="organizations"></asp:MenuItem>
		</Items>
	</asp:Menu>
	
	<asp:MultiView id="multiView" runat="server" ActiveViewIndex="0">
		<asp:View ID="viewUsers" runat="server">
			<uc:userList ID="userList" runat="server" />
    </asp:View>
		
		<asp:View ID="viewRoles" runat="server">
			<uc:rolesList ID="rolesList" runat="server" />
		</asp:View>
		
		<asp:View ID="viewOrganizations" runat="server">
			<uc:organizationList ID="organizationList" runat="server" />
		</asp:View>
		
		<asp:View ID="viewSettings" runat="server">
		</asp:View>
	</asp:MultiView>
	


</asp:Content>

