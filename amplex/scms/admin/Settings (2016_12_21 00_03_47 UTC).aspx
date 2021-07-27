<%@ Page Title="Admin - Settings" Language="C#" MasterPageFile="~/scms/admin/Admin.Master" AutoEventWireup="true" CodeBehind="Settings.aspx.cs" Inherits="scms.admin.Settings" %>
<%@ Register Src="~/scms/admin/controls/StatusMessage.ascx" TagPrefix="uc" TagName="statusMessage" %>


<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <hr />
    <asp:CheckBox ID="checkShowAdminEditLinks" runat="server" Text="Enable Edit Controls" /><br />
    <asp:CheckBox ID="checkSslEnabled" runat="server" Text="SSL Enabled" /><br />
    <asp:CheckBox ID="checkUseSslForAdmin" runat="server" Text="Use SSL for Admin" />
    
    <div style="margin-top:20px;">
        <asp:LinkButton ID="btnSave" runat="server" CausesValidation="false" OnClick="btnSave_Click" Text="Save"></asp:LinkButton>
        <uc:statusMessage ID="statusMessage" runat="server" />
    </div>
   

</asp:Content>
