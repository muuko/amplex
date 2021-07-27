<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SiteDdl.ascx.cs" Inherits="scms.admin.controls.SiteDdl" %>
	<asp:DropDownList 
		ID="ddlSite" 
		runat="server" 
		AutoPostBack="true" 
		OnSelectedIndexChanged="ddlSite_SelectedIndexChanged"
	></asp:DropDownList>