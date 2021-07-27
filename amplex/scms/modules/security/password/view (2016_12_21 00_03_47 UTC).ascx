<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="view.ascx.cs" Inherits="scms.modules.security.password.view" %>
<div id="change-password">
    <asp:ChangePassword
        id="changePassword"
        runat="server"
        SuccessText="Password Updated"
        OnCancelButtonClick="OnContinueButtonClick"
        OnContinueButtonClick="OnContinueButtonClick"
    ></asp:ChangePassword>
</div>