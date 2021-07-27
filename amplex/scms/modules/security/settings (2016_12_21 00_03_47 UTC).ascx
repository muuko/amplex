<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="settings.ascx.cs" Inherits="scms.modules.security.settings" %>
<%@ Register Src="~/scms/admin/controls/StatusMessage.ascx" TagPrefix="uc" TagName="statusMessage" %>
<%@ Register Src="~/scms/admin/controls/PageSelector.ascx" TagPrefix="uc" TagName="pageSelector" %>

<asp:ScriptManagerProxy
    id="scriptManagerProxy"
    runat="server">
</asp:ScriptManagerProxy>
<style>
    
    .invitation
    {
        margin:10px;
        width:500px;
        
    }
    .invitation tr td:nth-child(2)
    {
        padding-left:20px;
        color: Red;
    }
</style>

<strong>Enable https for Public website</strong> <asp:CheckBox ID="checkSslPublicEnabled" runat="server" /><br />
<br />
<strong>Login Page:</strong>&nbsp; <uc:pageSelector ID="psLogin" runat="server" /><br />
<asp:UpdatePanel ID="updatePanel" runat="server">
    <ContentTemplate>

        <strong>Require User Email Validation</strong> &nbsp; 
            <asp:CheckBox 
                ID="checkUserEmailValidationRequired" 
                runat="server" 
                AutoPostBack="true" 
                OnCheckedChanged="checkUserEmailValidationRequired_CheckedChanged" /><br />
                <br />


            <fieldset style="width:510px;">
                <legend >Invitation Email</legend>
                    <table class="invitation" cellpadding="0" cellspacing="0" style="">
                        
                        <tr>
                            <td><label id="labelIsHtml" runat="server">Is Html</label></td>
                            <td><asp:CheckBox ID="checkUserEmailValidationIsHtml" runat="server" /></td>
                        </tr>
                        
                        <tr>
                            <td><label id="labelSubject" runat="server">Subject</label></td>
                            <td><asp:TextBox ID="txtSubject" runat="server" style="width:400px;" MaxLength="256"></asp:TextBox></td>
                        </tr>
                        <tr>
                            <td valign="top"><label id="labelBody" runat="server">Body</label></td>
                            <td valign="top"><asp:TextBox ID="txtBody" runat="server" style="width:400px;height:250px;" TextMode="MultiLine"></asp:TextBox></td>
                        </tr>
                        
                        
                    </table>
            </fieldset>    
    </ContentTemplate>
</asp:UpdatePanel>    
    
    



<div style="margin-top:20px;">
<asp:LinkButton ID="btnSave" runat="server" OnClick="btnSave_Click" Text="Save"></asp:LinkButton>&nbsp;
<uc:statusMessage ID="statusMessage" runat="server" />
</div>