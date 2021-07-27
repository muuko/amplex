<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="view.ascx.cs" Inherits="scms.modules.navigation.breadcrumbs.view" %>
<div class="breadcrumbs">
	<asp:SiteMapPath 
		ID="siteMapPath" 
		runat="server"
		SiteMapProvider="ScmsSiteMapProvider"
		CssClass="breadcrumbs"
		CurrentNodeStyle-CssClass="current"
		NodeStyle-CssClass="node"
		PathSeparatorStyle-CssClass="separator"
		RootNodeStyle-CssClass="root"
		>
	</asp:SiteMapPath>
</div>