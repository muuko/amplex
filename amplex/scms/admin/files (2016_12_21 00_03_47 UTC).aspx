<%@ Page Language="C#" MasterPageFile="~/scms/admin/Admin.Master" AutoEventWireup="true" CodeBehind="files.aspx.cs" Inherits="scms.admin.files" Title="Admin - Sites" %>
<%@ Register Src="~/scms/admin/controls/SiteDdl.ascx" TagName="SiteDdl" TagPrefix="uc" %>
<%@ Register Src="~/scms/modules/content/controls/FileManager.ascx" TagName="FileManager" TagPrefix="uc" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
		<uc:SiteDdl ID="siteDdl" runat="server"  />
		<hr />
		<uc:FileManager ID="fileManager" runat="server" Mode="Manage" />
</asp:Content>
