<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="formsettings.ascx.cs" Inherits="scms.modules.special.special.formsettings" %>
<%@ Register Src="~/scms/admin/controls/PageSelector.ascx" TagPrefix="uc" TagName="pageSelector" %>
<%@ Register Src="~/scms/admin/controls/StatusMessage.ascx" TagPrefix="uc" TagName="statusMessage" %>
<%@ Register Src="~/scms/admin/controls/SiteDdl.ascx" TagPrefix="uc" TagName="siteDdl" %>

<strong>Destination Site:</strong> 
<asp:DropDownList ID="ddlSite2" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlSite_SelectedIndexChanged"></asp:DropDownList>
<br />

<strong>Destination Page:</strong> <uc:pageSelector ID="pageSelector" runat="server"  />
<asp:RequiredFieldValidator 
    ID="rfvPageSelector" 
    runat="server" 
    ValidationGroup="special-formsettings"
    ControlToValidate="pageSelector"
    ErrorMessage="required"
    ></asp:RequiredFieldValidator>
<br />
<strong>Template:</strong> <asp:DropDownList id="ddlTemplate" runat="server"></asp:DropDownList>
<br />
<table>
    <tr>
        <th>Special Field</th>
        <th></th>
    </tr>
    <tr>
        <td valign="top"><asp:Label ID="labelTitle" runat="server">Title</asp:Label></td>
        <td valign="top">
            <asp:Label ID="labelTitleFormField" runat="server">Form Field:</asp:Label> <asp:DropDownList ID="ddlTitle" runat="server"></asp:DropDownList>
            <asp:RequiredFieldValidator ID="rfvTitle" runat="server" ControlToValidate="ddlTitle" Display="Dynamic" ErrorMessage="*" ValidationGroup="special-formsettings" ></asp:RequiredFieldValidator>
        </td>
    </tr>
    
    <tr>
        <td valign="top"><asp:Label ID="labelImage" runat="server">Image</asp:Label></td>
        <td valign="top">
            <asp:Label ID="labelImageFormField" runat="server">Form Field:</asp:Label> <asp:DropDownList ID="ddlImage" runat="server"></asp:DropDownList>
            <asp:RequiredFieldValidator ID="rfvImage" runat="server" ControlToValidate="ddlImage" Display="Dynamic" ErrorMessage="*" ValidationGroup="special-formsettings"></asp:RequiredFieldValidator>
        </td>
        <td valign="top"><asp:Label ID="labelTransformImage" runat="server"></asp:Label></td>
        <td valign="top">
            <table>
                <tr>
                    <td>transformation mode:</td>
                    <td>
                        <asp:DropDownList ID="ddlImageTransformationMode" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlImageTransformationMode_selectedIndexChanged">
                            <asp:ListItem Value="">[none]</asp:ListItem>
                            <asp:ListItem>stretch</asp:ListItem>
                            <asp:ListItem Enabled="false">crop</asp:ListItem>
                            <asp:ListItem Enabled="false">fill</asp:ListItem>
                            <asp:ListItem>grow</asp:ListItem>
                        </asp:DropDownList>
                    </td>
                </tr>
                <tr>
                    <td>height:</td>
                    <td>
                        <asp:TextBox ID="txtThumbnailHeight" runat="server"></asp:TextBox>
                        <asp:CompareValidator 
                            ID="cvTxtThumbnailHeight" 
                            runat="server"
                            ControlToValidate="txtThumbnailHeight"
                            Display="Dynamic"
                            ErrorMessage="must be > 0"
                            Type="Integer"
                            ValueToCompare="0"
                            Operator="GreaterThan"
                            ValidationGroup="special-formsettings"
                            ></asp:CompareValidator>
                    </td>
                </tr>
                <tr>
                    <td>width:</td>
                    <td><asp:TextBox ID="txtThumbnailWidth" runat="server"></asp:TextBox>
                        <asp:CompareValidator 
                            ID="cvThumbnailWidth" 
                            runat="server"
                            ControlToValidate="txtThumbnailWidth"
                            Display="Dynamic"
                            ErrorMessage="must be > 0"
                            Type="Integer"
                            ValueToCompare="0"
                            Operator="GreaterThan"
                            ValidationGroup="special-formsettings"
                            ></asp:CompareValidator>                    
                    </td>
                </tr>
                <tr>
                    <td>background color:</td>
                    <td><asp:TextBox ID="txtThumbnailBackgroundColor" runat="server" Enabled="false"></asp:TextBox></td>
                </tr>
                
            </table>
        </td>
    </tr>
     
    <tr>
        <td valign="top"><asp:Label ID="labelDescription" runat="server">Description</asp:Label></td>
        <td valign="top">
            <asp:Label ID="labelDescriptionFormField" runat="server">Form Field:</asp:Label> <asp:DropDownList ID="ddlDescription" runat="server"></asp:DropDownList>
            <asp:RequiredFieldValidator ID="rfvDescription" runat="server" ControlToValidate="ddlDescription" Display="Dynamic" ErrorMessage="*" ValidationGroup="special-formsettings" ></asp:RequiredFieldValidator>
        </td>
    </tr>

    <tr>
        <td valign="top"><asp:Label ID="labelAssociatedDate" runat="server">Associated Date</asp:Label></td>
        <td valign="top">
            <asp:Label ID="label2" runat="server">Form Field:</asp:Label> <asp:DropDownList ID="ddlAssociatedDate" runat="server"></asp:DropDownList>
            <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ControlToValidate="ddlAssociatedDate" Display="Dynamic" ErrorMessage="*" ValidationGroup="special-formsettings" ></asp:RequiredFieldValidator>
        </td>
    </tr>
        
</table>
<asp:LinkButton ID="btnSaveSettings" runat="server" Text="Save" OnClick="btnSaveSettings_Clicked" CausesValidation="true" ValidationGroup="special-formsettings"></asp:LinkButton>
<asp:LinkButton ID="btnCancel" runat="server" Text="Cancel" OnClick="btnCancel_Clicked"></asp:LinkButton>
<uc:statusMessage ID="statusMessage" runat="server" />
