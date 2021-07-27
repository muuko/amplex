<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="slide.ascx.cs" Inherits="scms.modules.slideshow.slide" %>
<%@ Register Src="~/scms/admin/controls/SelectImage.ascx" TagPrefix="uc" TagName="selectImage" %>
<%@ Register Src="~/scms/admin/controls/PageSelector.ascx" TagPrefix="uc" TagName="pageSelector" %>
<table class="admin-settings">
    <tr>
        <td style="width:150px"><label>Name</label>
            <asp:RequiredFieldValidator
                id="rfvName"
                runat="server"
                ControlToValidate="txtName"
                Display="Dynamic"
                ErrorMessage="*"
                ValidationGroup="slide"
             ></asp:RequiredFieldValidator>
        </td>
        <td class="value">
            <asp:TextBox 
                ID="txtName"
                runat="server"
                width="100"
                MaxLength="32" />
         </td>
    </tr>
    <asp:PlaceHolder ID="placeholderHeading" runat="server">
        <tr>
            <td><label>Heading</label></td>
            <td class="value">
                <asp:TextBox
                    id="txtHeading"
                    runat="server"
                    width="200"
                    MaxLength="256" />
            </td>
        </tr>
    </asp:PlaceHolder>
    
    <tr>
        <td><label>Image Url</label></td>
        <td class="value"><uc:selectImage ID="selectImage" runat="server" /></td>
    </tr>
    <tr>
        <td><label>Link Url</label></td>
        <td class="value"><asp:TextBox ID="txtLinkUrl" runat="server" Width="400" MaxLength="1024"></asp:TextBox>
            <uc:pageSelector ID="pageSelector" runat="server" ShowSelectedPage="false" />
        </td>
    </tr>
    
    <asp:PlaceHolder ID="placeholderContent" runat="server">
        <tr>
            <td valign="top"><label>Content</label></td>
            <td class="value" valign="top">
                <asp:TextBox ID="txtContent" runat="server" TextMode="MultiLine" Columns="80" Rows="4" runat="server"></asp:TextBox>
            </td>
        </tr>
    </asp:PlaceHolder>
    
    <tr>
        <td><label>Advanced</label></td>
        <td class="value">
            <asp:CheckBox ID="checkAvanced" runat="server" AutoPostBack="true" OnCheckedChanged="checkAdvanced_Click" />
        </td>
    </tr>

    <asp:PlaceHolder ID="placeholderAdvanced" runat="server">
        <tr><td valign="top"><label>Custom Override</label></td>
            <td class="value" valign="top">
                <asp:TextBox ID="txtCustomOverride" runat="server" TextMode="MultiLine" Columns="80" Rows="4" runat="server"></asp:TextBox>
            </td>
        </tr>
    </asp:PlaceHolder>
</table>

