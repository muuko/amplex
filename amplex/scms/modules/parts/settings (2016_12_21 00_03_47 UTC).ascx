<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="settings.ascx.cs" Inherits="scms.modules.parts.settings" %>
<%@ Register Src="~/scms/modules/parts/sizes.ascx" TagPrefix="uc" TagName="sizes" %>
<%@ Register Src="~/scms/admin/controls/PageSelector.ascx" TagPrefix="uc" TagName="pageSelector" %>
<%@ Register Src="~/scms/admin/controls/StatusMessage.ascx" TagPrefix="uc" TagName="statusMessage" %>
<%@ Register Src="~/scms/admin/controls/ModuleSelector.ascx" TagPrefix="uc" TagName="moduleSelector" %>
<%@ Register Src="~/scms/admin/controls/PageModuleInstanceSelector.ascx" TagPrefix="uc" TagName="pageModuleInstanceSelector" %>

<asp:Menu 
	ID="menu" 
	runat="server" 
	Orientation="Horizontal"
	OnMenuItemClick="menu_Click"
 StaticMenuItemStyle-CssClass="tabbed-menu"
 StaticSelectedStyle-CssClass="tabbed-menu-selected"
	>
	<Items>
		<asp:MenuItem Text="General" Value="general" Selected="true"></asp:MenuItem>
		<asp:MenuItem Text="Sizes" Value="sizes"></asp:MenuItem>
	</Items>
</asp:Menu>
<asp:MultiView ID="mvSettings" runat="server" ActiveViewIndex="0">
	<asp:View ID="viewGeneral" runat="server">
		Parts Search Results Module
		<uc:pageModuleInstanceSelector ID="pagePluginModuleInstanceSelector" runat="server" />
		<div style="margin-top:20px;">
			<asp:LinkButton ID="btnSave" runat="server" OnClick="btnSave_Click" Text="Save"></asp:LinkButton>&nbsp;
			<uc:statusMessage ID="statusMessage" runat="server" />
		</div>
	</asp:View>
	<asp:View ID="viewSizes" runat="server">
		<uc:sizes ID="sizes" runat="server" />
	</asp:View>
</asp:MultiView>