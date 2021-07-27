<%@ Page Language="C#" MasterPageFile="~/scms/admin/Admin.Master" AutoEventWireup="true" CodeBehind="template.aspx.cs" Inherits="scms.admin.TemplatePage" Title="Untitled Page" %>
<%@ Register Src="~/scms/admin/controls/PluginModuleInstances.ascx" TagPrefix="uc" TagName="PluginModuleInstances" %>
<%@ Register Src="~/scms/admin/controls/SiteDdl.ascx" TagName="SiteDdl" TagPrefix="uc" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>


<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
	 <uc:SiteDdl ID="siteDdl" runat="server" /> > <a runat="server" href="~/scms/admin/templates.aspx">templates</a> > <asp:Literal ID="literalTemplateName" runat="server"></asp:Literal>
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
			<asp:MenuItem Text="Modules" Value="Modules" Selected="true"></asp:MenuItem>
			<asp:MenuItem Text="Settings" Value="Settings"  ></asp:MenuItem>
		</Items>
	</asp:Menu>
	
	<asp:MultiView ID="multiView" runat="server" ActiveViewIndex="0">
		<asp:View ID="viewModules" runat="server">
			<uc:PluginModuleInstances ID="pluginModuleInstances" runat="server" />
			<div style="clear:both;margin-top:40px;"><a id="anchorNewModule" runat="server">New Module</a></div>
		</asp:View>
		<asp:View ID="viewSettings" runat="server">
			<table>
			<tr runat="server" visible="false">
				<td><strong>ID</strong></td>
				<td><asp:Literal ID="literalId" runat="server"></asp:Literal></td>
			</tr>
			<tr>
				<td><strong>Name</strong></td>
				<td><asp:TextBox ID="txtName" runat="server"></asp:TextBox></td>
			</tr>
			<tr>
				<td><strong>Master Page</strong></td>
				<td><asp:DropDownList ID="ddlMasterPage" runat="server"></asp:DropDownList></td>
			</tr>
		</table>
		<br />
		<asp:LinkButton ID="btnSave" runat="server" Text="Save" OnClick="bntSave_Click"></asp:LinkButton>
		</asp:View>
	</asp:MultiView>

	<br />
	<asp:Literal ID="literalMessage" runat="server"></asp:Literal>
</asp:Content>
