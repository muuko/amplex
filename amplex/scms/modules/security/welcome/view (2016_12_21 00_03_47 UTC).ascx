<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="view.ascx.cs" Inherits="scms.modules.security.welcome.view" %>
<div class="welcome">
    <asp:MultiView ID="mv" runat="server" >
        <asp:View ID="viewLoggedIn" runat="server">
            Welcome <asp:Literal ID="literalUser" runat="server" Text="The User"></asp:Literal> &nbsp;|&nbsp; 
            <asp:LinkButton ID="btnLogout" runat="server" Text="Logout" OnClick="btnLogout_Click"></asp:LinkButton>
        </asp:View>
        <asp:View ID="viewNotLoggedIn" runat="server">
            <asp:LinkButton ID="btnLogin" runat="server" Text="Login" OnClick="btnLogin_Click"></asp:LinkButton>
        </asp:View>
    </asp:MultiView>
</div>