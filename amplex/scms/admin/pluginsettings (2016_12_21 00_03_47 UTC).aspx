<%@ Page Language="C#" MasterPageFile="~/scms/admin/Admin.Master" AutoEventWireup="true" CodeBehind="pluginsettings.aspx.cs" Inherits="scms.admin.pluginsettings" Title="Admin - Plugins" %>
<%@ Register Src="~/scms/admin/controls/SiteDdl.ascx" TagName="SiteDdl" TagPrefix="uc" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <uc:SiteDdl ID="siteDdl" runat="server"  />
    <strong><asp:Literal ID="literalPluginName" runat="server"></asp:Literal></strong>
<hr />
<asp:Menu
	id="menu"
	runat="server"
	Orientation="Horizontal"
	OnMenuItemClick="menu_menuItemClick"
	StaticMenuItemStyle-CssClass="tabbed-menu"
	StaticHoverStyle-CssClass="tabbed-menu-hover"
	StaticSelectedStyle-CssClass="tabbed-menu-selected"	
	>
	<Items>
		<asp:MenuItem Text="Settings" Value="settings" Selected="true"></asp:MenuItem>
		<asp:MenuItem Text="Instances" Value="instances"></asp:MenuItem>
	</Items>
</asp:Menu>
<asp:MultiView
	id="multiView"
	runat="server"
	ActiveViewIndex="0"
	>
	<asp:View ID="viewSettings" runat="server">
		<asp:Panel
			id="panelNoSettings"
			runat="server"
			Visible="false">
			<em>This plugin has not settings.</em>
		</asp:Panel>
	</asp:View>
	
	<asp:View ID="viewInstances" runat="server">
		<asp:ListView 
			ID="lvInstances" 
			runat="server"
			OnItemDataBound="viewInstances_ItemDataBound"
			>
			<EmptyDataTemplate><em>This plugin has no modules.</em></EmptyDataTemplate>
			<LayoutTemplate><asp:PlaceHolder ID="ItemPlaceholder" runat="server"></asp:PlaceHolder></LayoutTemplate>
			<ItemSeparatorTemplate><br /></ItemSeparatorTemplate>
			<ItemTemplate>
				<strong><%# Eval("name") %></strong><br />
				<asp:Panel ID="panelNone" runat="server" Visible="false">
					<em>There are no instances of this module.</em>
				</asp:Panel>
				
				<asp:Panel ID="panelTemplates" runat="server" >
					<em>Templates</em><br />
					<asp:Repeater
						id="rptTemplates"
						runat="server"
						>
						<HeaderTemplate><div style="margin-left:5px;margin-bottom:5px;"></HeaderTemplate>
						<ItemTemplate>
							<a href="/scms/admin/module.aspx?tmid=<%# Eval("instanceId") %>"><%# Eval("name")%></a><br />
						</ItemTemplate>
						<FooterTemplate></div></FooterTemplate>
					</asp:Repeater>
				</asp:Panel>
				
				<asp:Panel ID="panelPages" runat="server">
					<em>Pages</em><br />
					<asp:Repeater
						id="rptPages"
						runat="server"
						>
						<HeaderTemplate><div style="margin-left:5px;margin-bottom:5px;"></HeaderTemplate>
						<ItemTemplate>
							<a href="/scms/admin/module.aspx?pmid=<%# Eval("instanceId") %>"><%# Eval("url")%></a><br />
						</ItemTemplate>
						<FooterTemplate></div></FooterTemplate>
					</asp:Repeater>
				</asp:Panel>
				


				
			</ItemTemplate>
		</asp:ListView>
	</asp:View>
</asp:MultiView>


</asp:Content>
