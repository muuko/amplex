<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="PlaceholderDdl.ascx.cs" Inherits="scms.admin.controls.PlaceholderDdl" %>
<asp:DropDownList ID="ddlPlaceholders" AutoPostBack="true" OnSelectedIndexChanged="ddlPlaceholders_SelectedIndexChanged" runat="server"></asp:DropDownList> 
<asp:CheckBox ID="checkShowAll" runat="server" AutoPostBack="true" OnCheckedChanged="checkShowAll_CheckChanged" Text="Show All" />
<asp:Literal ID="literalWarning" runat="server" Visible="false" ><span style="color:red;margin-left:6px;">Not found.</span></asp:Literal>