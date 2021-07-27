<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="view.ascx.cs" Inherits="scms.modules.security.profile.view" %>
<%@ Register Src="~/scms/admin/controls/StatusMessage.ascx" TagPrefix="uc" TagName="statusMessage" %>


<label>User ID</label><br />
<asp:Literal ID="literalUserId" runat="server"></asp:Literal><br />
<br />
<label>First Name</label><br />
<asp:TextBox ID="txtFirstName" runat="server"></asp:TextBox><br />
<br />
<label>Last Name</label><br />
<asp:TextBox ID="txtLastName" runat="server"></asp:TextBox><br />
<br />
<label>Email Address</label><br />
<asp:TextBox ID="txtEmailAddress" runat="server" Width="200"></asp:TextBox><br />
<br />
<label>Company</label><br />
<asp:TextBox ID="txtCompany" runat="server" Width="200"></asp:TextBox><br />
<br />
<asp:Button ID="btnSave" runat="server" OnClick="btnSave_Click" Text="Save" /> <asp:Button ID="btnCancel" runat="server" Text="Cancel" OnClick="btnCancel_Click" /><br />
<uc:statusMessage ID="statusMessage" runat="server" />


<%-- 
<asp:MultiView ID="mv" runat="server" ActiveViewIndex="0">
    <asp:View ID="viewLogin" runat="server">
        <asp:Panel ID="panelLogin" runat="server" DefaultButton="btnLogin">
            <table>
                <tr>
                    <td valign="top"><strong>User Name:</strong></td>
                    <td valign="top">
                        <asp:TextBox TabIndex="1" id="txtUserName" runat="server"></asp:TextBox><br />
                        <asp:LinkButton TabIndex="-1" ID="btnForgotUserName" runat="server" Text="Forgot User Name?" OnClick="btnForgotUserName_Click"></asp:LinkButton>
                    </td>
                </tr>
                <tr>
                    <td valign="top"><strong>Password:</strong></td>
                    <td valign="top">
                        <asp:TextBox TabIndex="2" ID="txtPassword" runat="server" TextMode="Password"></asp:TextBox><br />
                        <asp:LinkButton TabIndex="-1" ID="btnForgotPassword" runat="server" Text="Forgot Password?" OnClick="btnForgotPassword_Click"></asp:LinkButton>
                    </td>
                </tr>
                <tr>
                    <td colspan="2">
                        <asp:Button TabIndex="3" ID="btnLogin" runat="server" Text="Login" OnClick="btnLogin_Click" /><br />
                        <uc:statusMessage ID="statusMessage" runat="server" />
                    </td>
                </tr>
            </table>
        </asp:Panel>
    </asp:View>
    
    <asp:View ID="viewForgotUserName" runat="server">
        <asp:MultiView ID="mvForgotUserName" runat="server" ActiveViewIndex="0">
            <asp:View ID="viewForgotUserNameForm" runat="server">
                <table>
                    <tr>
                        <td><strong>Email Address:</strong></td>
                        <td><asp:TextBox ID="txtForgotUserNameEmail" runat="server"></asp:TextBox></td>
                    </tr>
                    <tr>
                        <td colspan="2">
                            <asp:Button ID="btnForgotUserNameSubmit" runat="server" Text="Request User Name" OnClick="btnForgotUserNameSubmit_Click" />
                            <uc:statusMessage ID="statusMessageForgotUserName" runat="server" />
                        </td>
                    </tr>
                </table>
            </asp:View>
            <asp:View ID="viewForgotUserNameAck" runat="server">
                Your user name has been sent to the email address associated with this account.
            </asp:View>
        </asp:MultiView>
        <br /><br />
        <asp:LinkButton ID="btnReturnToLogin" runat="server" Text="<< Return to login" OnClick="btnBackToLogin_Click"></asp:LinkButton>
    </asp:View>
    
    <asp:View ID="viewForgotPassword" runat="server">
        <asp:MultiView ID="mvForgotPassword" runat="server" ActiveViewIndex="0">
            <asp:View ID="viewForgotPasswordForm" runat="server">
                <table>
                    <tr>
                        <td><strong>User Name:</strong></td>
                        <td><asp:TextBox ID="txtForgotPasswordUserName" runat="server"></asp:TextBox></td>
                    </tr>
                    <tr>
                        <td colspan="2">
                            <asp:Button ID="btnForgotPasswordSubmit" runat="server" Text="Reset Password" OnClick="btnForgotPasswordSubmit_Click" />
                            <uc:statusMessage ID="statusMessageForgotPassword" runat="server" />
                        </td>
                    </tr>
                </table>
            </asp:View>
            <asp:View ID="viewForgotPasswordAck" runat="server">
                Your password has been sent to the email address associated with this account.
            </asp:View>
        </asp:MultiView>
        <br /><br />
        <asp:LinkButton ID="btnReturnToLogin2" runat="server" Text="<< Return to login" OnClick="btnBackToLogin_Click"></asp:LinkButton>
    </asp:View>
</asp:MultiView>

--%>