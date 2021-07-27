<%@ Page Title="" Language="C#" MasterPageFile="~/scms/admin/Admin.Master" AutoEventWireup="true" CodeBehind="change-password.aspx.cs" Inherits="scms.admin.change_password" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
<h1>Admin - Change Password</h1>
<hr />
<asp:ChangePassword
    id="changePassword"
    runat="server"
    ContinueDestinationPageUrl="/scms"
    ></asp:ChangePassword>
</asp:Content>
