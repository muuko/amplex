<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="formsettings.ascx.cs" Inherits="scms.modules.forms.autoresponder.formsettings" %>
<%@ Register Src="~/scms/admin/controls/PageSelector.ascx" TagPrefix="uc" TagName="pageSelector" %>
<%@ Register Src="~/scms/admin/controls/StatusMessage.ascx" TagPrefix="uc" TagName="statusMessage" %>

<!-- TinyMCE -->
<script type="text/javascript" src="/scms/client/jscript/tiny_mce/tiny_mce.js"></script>
<script type="text/javascript">
</script>
<!-- /TinyMCE -->

<table>
    <tr>
        <td style="width:100px;"><strong>to email field*</strong></td>
        <td>
            <asp:DropDownList ID="ddlEmailAddress" runat="server"></asp:DropDownList>
            <asp:RequiredFieldValidator 
                ID="rfvDdlEmailAddress" 
                runat="server"
                ControlToValidate="ddlEmailAddress"
                display="Dynamic"
                ErrorMessage="required"
                ValidationGroup="autoresponder"></asp:RequiredFieldValidator>
        </td>
    </tr>
    <tr>
        <td><strong>from email</strong></td>
        <td><asp:TextBox ID="txtFromEmailAddress" runat="server" Width="400"></asp:TextBox></td>
    </tr>
    <tr>
        <td><strong>cc</strong></td>
        <td><asp:TextBox ID="txtCc" runat="server" Width="400"></asp:TextBox></td>
    </tr>
    <tr>
        <td><strong>bcc</strong></td>
        <td><asp:TextBox ID="txtBcc" runat="server" Width="400"></asp:TextBox></td>
    </tr>
    <tr>
        <td valign="top"><strong>subject</strong></td>
        <td valign="top">
            <asp:TextBox ID="txtSubject" runat="server" Width="400"></asp:TextBox>
            <asp:RequiredFieldValidator 
                ID="rfvTxtSubjectd" 
                runat="server" 
                ControlToValidate="txtSubject"
                Display="Dynamic"
                ErrorMessage="required"
                ValidationGroup="autoresponder"
                ></asp:RequiredFieldValidator><br />
                <span style="color:gray;">use ##fieldname## for substitutions</span><br /><br />
        </td>
    </tr>
    <tr>
        <td valign="top"><strong>body</strong></td>
        <td valign="top">
            <asp:MultiView ID="mvBody" runat="server" ActiveViewIndex="0">
                <asp:View ID="viewBodyHtml" runat="server">
                    <textarea name="txtBodyHtml" id="txtBodyHtml" runat="server" cols="20" rows="20" ></textarea>
                </asp:View>
                <asp:View ID="viewBodyText" runat="server">
                    <textarea name="txtBodyText" id="txtBodyText" runat="server" cols="20" rows="10" style="width:650px;"></textarea>
                </asp:View>
            </asp:MultiView><br />
                <span style="color:gray;">use ##fieldname## for substitutions</span><br />
            <br />
            <asp:CustomValidator
                id="cvBody"
                runat="server"
                ControlToValidate="rblMode"
                Display="Dynamic"
                ErrorMessage="Required <br />"
                OnServerValidate="cvBody_ServerValidate"
                ValidationGroup="autoresponder"></asp:CustomValidator>
            <asp:RadioButtonList RepeatDirection="Horizontal" ID="rblMode" runat="server" AutoPostBack="true" OnSelectedIndexChanged="rblMode_SelectedIndexChanged">
                <asp:ListItem Selected="True">html</asp:ListItem>
                <asp:ListItem>text</asp:ListItem>
            </asp:RadioButtonList>
        
        </td>
    </tr>
</table>

<asp:LinkButton ID="btnSaveSettings" runat="server" Text="Save" OnClick="btnSaveSettings_Clicked" CausesValidation="true" ValidationGroup="autoresponder"></asp:LinkButton>
<asp:LinkButton ID="btnCancel" runat="server" Text="Cancel" OnClick="btnCancel_Clicked"></asp:LinkButton>
<uc:statusMessage ID="statusMessage" runat="server" />
