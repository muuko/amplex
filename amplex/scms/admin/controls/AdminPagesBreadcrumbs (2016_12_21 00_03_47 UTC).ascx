<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="AdminPagesBreadcrumbs.ascx.cs" Inherits="scms.admin.controls.AdminPagesBreadcrumbs" %>
<%@ Register Src="~/scms/admin/controls/SiteDdl.ascx" TagName="SiteDdl" TagPrefix="uc" %>
<uc:SiteDdl ID="siteDdl" runat="server"  /> > <a runat="server" id="anchorPages" href="~/scms/admin/pages.aspx" >pages</a>
<asp:Repeater ID="rptPageBreadCrumbs" runat="server" OnItemDataBound="rptPageBreadCrumbs_ItemDataBound">
	<ItemTemplate>
		> <a id="anchorPage" runat="server"></a>
	</ItemTemplate>
</asp:Repeater>