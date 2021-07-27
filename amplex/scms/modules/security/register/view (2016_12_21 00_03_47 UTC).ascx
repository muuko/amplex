<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="view.ascx.cs" Inherits="scms.modules.security.register.view" %>
<%@ Register Src="~/scms/admin/controls/StatusMessage.ascx" TagPrefix="uc" TagName="statusMessage" %>

<asp:MultiView ID="mv" runat="server" ActiveViewIndex="0">
    <asp:View ID="viewRegister" runat="server">
        <label>Email Address</label><br />
        <asp:TextBox ID="txtEmailAddress" runat="server" Width="200"></asp:TextBox><br />
        <asp:RequiredFieldValidator
            id="rfvEmailAddress"
            runat="server"
            Display="Dynamic"
            ControlToValidate="txtEmailAddress"
            ErrorMessage = "Required<br />"
            ValidationGroup="register">
            </asp:RequiredFieldValidator>
        <asp:CustomValidator 
            ID="cvEmailAddress"
            runat="server"
            Display="Dynamic"
            ControlToValidate="txtEmailAddress"
            OnServerValidate="cvEmailAddress_ServerValidate"
            ValidationGroup="register"
            ></asp:CustomValidator>
        <br />
        <label>Company / Organization</label><br />
        
        <asp:TextBox ID="txtCompany" runat="server" Width="200"></asp:TextBox><br />
        <asp:RequiredFieldValidator
            id="rfvCompany"
            runat="server"
            ControlToValidate="txtCompany"
            Display="Dynamic"
            ErrorMessage="Required<br />"
            ValidationGroup="register"
            ></asp:RequiredFieldValidator>
        <br />
        <label>Password</label><br />
        <asp:TextBox ID="txtPassword" runat="server" Width="140" TextMode="Password"></asp:TextBox><br />
        <asp:RequiredFieldValidator 
            ID="rfvPassword"
            runat="server"
            ControlToValidate="txtPassword"
            Display="Dynamic"
            ErrorMessage="Required<br />"
            ValidationGroup="register"
        ></asp:RequiredFieldValidator>
        <asp:CustomValidator
            id="cvPassword"
            runat="server"
            ControlToValidate="txtPassword"
            ValidationGroup="register"
            OnServerValidate="cvPassword_ServerValidate"
            ></asp:CustomValidator>
        <span style="color:#888;">Minimium 6 characters.</span><br />
        <br />
        <label>Confirm Password</label><br />
        <asp:TextBox ID="txtConfirmPassword" runat="server" Width="140" TextMode="Password"></asp:TextBox><br />
        <br />

        <asp:Button ID="btnRegister" 
            runat="server" 
            Text="Register" 
            OnClick="btnRegister_Click" 
            ValidationGroup="register"
            CausesValidation="true"
             /><br />
             <br />
        <uc:statusMessage ID="statusMessage" runat="server" />
    </asp:View>
    <asp:View ID="viewAck" runat="server">
        Thank you for registering.<br />
        <br />
        A confirmation email has been sent to you at <asp:Literal ID="literalEmailAddress" runat="server"></asp:Literal>.<br />
        <br />
        Click the link in the email to setup your promotions.
    </asp:View>
</asp:MultiView>

