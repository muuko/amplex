<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="formsettings.ascx.cs" Inherits="scms.modules.submission.formsettings" %>
<%@ Register Src="~/scms/admin/controls/ModuleSelector.ascx" TagPrefix="uc" TagName="moduleSelector" %>
<strong>Submission Module:</strong> <uc:moduleSelector ID="moduleSelector" runat="server" />
<asp:RequiredFieldValidator 
    ID="rfvModuleSelector" 
    runat="server" 
    ValidationGroup="submission-formsettings"
    ControlToValidate="moduleSelector"
    ></asp:RequiredFieldValidator>
<br />
<table>
    <tr>
        <th>Submission Field</th>
        <th></th>
    </tr>
    <tr>
        <td valign="top"><asp:Label ID="labelTitle" runat="server">Title</asp:Label></td>
        <td valign="top">
            <asp:Label ID="labelTitleFormField" runat="server">Form Field:</asp:Label> <asp:DropDownList ID="ddlTitle" runat="server"></asp:DropDownList>
            <asp:RequiredFieldValidator ID="rfvTitle" runat="server" ControlToValidate="ddlTitle" Display="Dynamic" ErrorMessage="*" ValidationGroup="submission-formsettings" ></asp:RequiredFieldValidator>
        </td>
    </tr>
    
    <tr>
        <td valign="top"><asp:Label ID="labelLink" runat="server">Link</asp:Label></td>
        <td valign="top">
            <asp:Label ID="labelLinkFormField" runat="server">Form Field:</asp:Label> <asp:DropDownList ID="ddlLink" runat="server"></asp:DropDownList>
            <asp:RequiredFieldValidator ID="rfvLink" runat="server" ControlToValidate="ddlLink" Display="Dynamic" ErrorMessage="*" ValidationGroup="submission-formsettings"></asp:RequiredFieldValidator>
        </td>
    </tr>

    <tr>
        <td valign="top"><asp:Label ID="labelImage" runat="server">Image</asp:Label></td>
        <td valign="top">
            <asp:Label ID="labelImageFormField" runat="server">Form Field:</asp:Label> <asp:DropDownList ID="ddlImage" runat="server"></asp:DropDownList>
            <asp:RequiredFieldValidator ID="rfvImage" runat="server" ControlToValidate="ddlImage" Display="Dynamic" ErrorMessage="*" ValidationGroup="submission-formsettings"></asp:RequiredFieldValidator>
        </td>
    </tr>
 
    <tr>
        <td valign="top"><asp:Label ID="labelVideo" runat="server">Video</asp:Label></td>
        <td valign="top">
            <asp:Label ID="labelVideoFormField" runat="server">Form Field:</asp:Label> <asp:DropDownList ID="ddlVideo" runat="server"></asp:DropDownList>
            <asp:RequiredFieldValidator ID="rfvVideo" runat="server" ControlToValidate="ddlVideo" Display="Dynamic" ErrorMessage="*" ValidationGroup="submission-formsettings" ></asp:RequiredFieldValidator>
        </td>
    </tr>
    
    <tr>
        <td valign="top"><asp:Label ID="labelDescription" runat="server">Description</asp:Label></td>
        <td valign="top">
            <asp:Label ID="labelDescriptionFormField" runat="server">Form Field:</asp:Label> <asp:DropDownList ID="ddlDescription" runat="server"></asp:DropDownList>
            <asp:RequiredFieldValidator ID="rfvDescription" runat="server" ControlToValidate="ddlDescription" Display="Dynamic" ErrorMessage="*" ValidationGroup="submission-formsettings" ></asp:RequiredFieldValidator>
        </td>
    </tr>

    <tr>
        <td valign="top"><asp:Label ID="labelEmailAddress" runat="server">EmailAddress</asp:Label></td>
        <td valign="top">
            <asp:Label ID="labelEmailAddressFormField" runat="server">Form Field:</asp:Label> <asp:DropDownList ID="ddlEmailAddress" runat="server"></asp:DropDownList>
            <asp:RequiredFieldValidator ID="rfvEmailAddress" runat="server" ControlToValidate="ddlEmailAddress" Display="Dynamic" ErrorMessage="*" ValidationGroup="submission-formsettings" ></asp:RequiredFieldValidator>
        </td>
    </tr>
    
    <tr>
        <td valign="top"><asp:Label ID="labelUserId" runat="server">UserId</asp:Label></td>
        <td valign="top">
            <asp:Label ID="labelUserIdFormField" runat="server">Form Field:</asp:Label> <asp:DropDownList ID="ddlUserId" runat="server"></asp:DropDownList>
            <asp:RequiredFieldValidator ID="rfvUserId" runat="server" ControlToValidate="ddlUserId" Display="Dynamic" ErrorMessage="*" ValidationGroup="submission-formsettings" ></asp:RequiredFieldValidator>
        </td>
    </tr>
    
    <tr>
        <td valign="top"><asp:Label ID="labelSubmitter" runat="server">Submitter</asp:Label></td>
        <td valign="top">
            <asp:Label ID="labelSubmitterFormField" runat="server">Form Field:</asp:Label> <asp:DropDownList ID="ddlSubmitter" runat="server"></asp:DropDownList>
            <asp:RequiredFieldValidator ID="rfvSubmitter" runat="server" ControlToValidate="ddlSubmitter" Display="Dynamic" ErrorMessage="*" ValidationGroup="submission-formsettings" ></asp:RequiredFieldValidator>
        </td>
    </tr>
    
    <tr>
        <td valign="top"><asp:Label ID="labelDocumentCredit" runat="server">DocumentCredit</asp:Label></td>
        <td valign="top">
            <asp:Label ID="labelDocumentCreditFormField" runat="server">Form Field:</asp:Label> <asp:DropDownList ID="ddlDocumentCredit" runat="server"></asp:DropDownList>
            <asp:RequiredFieldValidator ID="rfvDocumentCredit" runat="server" ControlToValidate="ddlDocumentCredit" Display="Dynamic" ErrorMessage="*" ValidationGroup="submission-formsettings" ></asp:RequiredFieldValidator>
        </td>
    </tr>
    

        
</table>
<asp:LinkButton ID="btnSaveSettings" runat="server" Text="Save" OnClick="btnSaveSettings_Clicked" CausesValidation="true" ValidationGroup="submission-formsettings"></asp:LinkButton>
<asp:LinkButton ID="btnCancel" runat="server" Text="Cancel" OnClick="btnCancel_Clicked"></asp:LinkButton>
