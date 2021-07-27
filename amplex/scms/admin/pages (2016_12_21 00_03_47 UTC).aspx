<%@ Page Language="C#" MasterPageFile="~/scms/admin/Admin.Master" AutoEventWireup="true" CodeBehind="pages.aspx.cs" Inherits="scms.admin.Pages" Title="Untitled Page" %>
<%@ Register Src="~/scms/admin/controls/PluginModuleInstances.ascx" TagPrefix="uc" TagName="PluginModuleInstances" %>
<%@ Register Src="~/scms/admin/controls/AdminPagesBreadcrumbs.ascx" TagName="AdminPagesBreadcrumbs" TagPrefix="uc" %>
<%@ Register Src="~/scms/admin/controls/PageSettings.ascx" TagPrefix="uc" TagName="PageSettings" %>
<%@ Register Src="~/scms/admin/controls/PageList.ascx" TagPrefix="uc" TagName="PageList" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

	<uc:AdminPagesBreadcrumbs ID="breadcrumbs" runat="server"/><a style="margin-left:25px;" id="anchorView" runat="server" href="#"><strong style="color:gray;font-size:12px;">View This Page</strong></a>
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
			<asp:MenuItem Text="Children" Value="Children" Selected="true"></asp:MenuItem>
			<asp:MenuItem Text="Modules" Value="Modules"></asp:MenuItem>
			<asp:MenuItem Text="Settings" Value="Settings"  ></asp:MenuItem>
		</Items>
	</asp:Menu>
	
	<asp:MultiView id="multiView" runat="server">
		<asp:View ID="viewChildren" runat="server">
			<uc:PageList ID="pageListChildren" runat="server" />
			<asp:Panel ID="panelNew" DefaultButton="btnNewPage" runat="server" style="margin-top:15px;padding: 5px 5px;background-color:#eef;clear:both;">
				<div style="display:inline;float:left;padding-top:5px;padding-right:5px;">New</div>
				<div style="display:inline;float:left;padding-top:2px;padding-right:10px;">
					<asp:DropDownList ID="ddlNewPageType" runat="server" >
						<asp:ListItem Text="Content Page" Value="P" Selected="True"></asp:ListItem>
						<asp:ListItem Text="Redirect" Value="R"></asp:ListItem>
						<asp:ListItem Text="Alias" Value="A"></asp:ListItem>
						<asp:ListItem Text="Internal" Value="I"></asp:ListItem>
					</asp:DropDownList>
				</div>
				<div style="display:inline;float:left;padding-top:5px;padding-right:5px;">Position</div>
				<div style="display:inline;float:left;padding-top:2px;padding-right:5px;">
					<asp:DropDownList ID="ddlNewPagePosition" runat="server">
						<asp:ListItem Text="First" Value="first"></asp:ListItem>
						<asp:ListItem Text="Last" Value="last" Selected="True"></asp:ListItem>
					</asp:DropDownList>
				</div>
				<div style="display:inline;float:left;margin-left:8px;padding-top:5px;padding-right:5px;">Name</div>
				<asp:TextBox id="txtNewPageName" runat="server" Width="100"></asp:TextBox>
				<asp:RequiredFieldValidator 
					ID="rfvNewPageName"
					runat="server"
					ControlToValidate="txtNewPageName"
					Display="Dynamic"
					ErrorMessage="*"
					ValidationGroup="new"
					></asp:RequiredFieldValidator>
				<asp:LinkButton ID="btnNewPage" runat="server" OnClick="btnNewPage_Click" Text="Create" CausesValidation="true" ValidationGroup="new" ></asp:LinkButton>
			</asp:Panel>
		</asp:View>
		
		<asp:View ID="viewModules" runat="server">
			<uc:PluginModuleInstances ID="pluginModuleInstances" runat="server" />
			<div style="clear:both;margin-top:40px;">
				<a id="anchorNewModule" runat="server">New Module</a>
			</div>
		</asp:View>
		
		<asp:View ID="viewSettings" runat="server">
			<uc:PageSettings ID="pageSettings" runat="server" />
		</asp:View>
	</asp:MultiView>
	


</asp:Content>
