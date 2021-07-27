<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="AdminBreadcrumbs.ascx.cs" Inherits="scms.admin.controls.AdminBreadcrumbs" %>
<%@ Register Src="~/scms/admin/controls/AdminPagesBreadcrumbs.ascx" TagName="AdminPagesBreadcrumbs" TagPrefix="uc" %>
<%@ Register Src="~/scms/admin/controls/SiteDdl.ascx" TagName="SiteDdl" TagPrefix="uc" %>

<asp:MultiView ID="multiViewHeader" runat="server" ActiveViewIndex="0">
	<asp:View ID="viewPageModule" runat="server">
		<uc:AdminPagesBreadcrumbs ID="pagesBreadcrumbs" runat="server"/> 
	</asp:View>
	<asp:View ID="viewTemplateModule" runat="server">
		<uc:SiteDdl ID="siteDdl" runat="server" /> > <a id="A1" href="~/scms/admin/templates.aspx" runat="server">templates</a> > <a id="anchorTemplate" runat="server"></a><asp:Literal ID="literalTemplateName" runat="server"></asp:Literal>
	</asp:View>
</asp:MultiView>
