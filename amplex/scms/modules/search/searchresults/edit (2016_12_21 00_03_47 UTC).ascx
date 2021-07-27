<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="edit.ascx.cs" Inherits="scms.modules.search.searchresults.edit" %>
<%@ Register Src="~/scms/admin/controls/StatusMessage.ascx" TagName="statusMessage" TagPrefix="uc" %>
<table>
    <tr>
        <td><span class="label">Max Result Count</span></td>
        <td>
            <asp:TextBox ID="txtMaxResultCount" runat="server" Width="60" Text="100"></asp:TextBox>
            <asp:RangeValidator
                id="tvMaxResultCount"
                ControlToValidate="txtMaxResultCount"
                runat="server"
                Type="Integer"
                MinimumValue="1"
                MaximumValue="9999"
                Display="Dynamic"
                ErrorMessage="Enter a number from 1 to 9999, or leave blank for no limit"
                ></asp:RangeValidator>
        </td>
    </tr>
    <tr>
        <td><span class="label">Page Size</span></td>
        <td>
            <asp:TextBox ID="txtPageSize" runat="server" Width="60" Text="10"></asp:TextBox>
            <asp:RangeValidator
                id="rvPageSize"
                ControlToValidate="txtPageSize"
                runat="server"
                Type="Integer"
                MinimumValue="1"
                MaximumValue="9999"
                Display="Dynamic"
                ErrorMessage="Enter a number from 1 to 9999, or leave blank for no paging"
                ></asp:RangeValidator>
        </td>
    </tr>
    <tr>
        <td><span class="label">Max Keywords</span></td>
        <td>
            <asp:TextBox ID="txtMaxKeywords" runat="server" Width="60" Text="5"></asp:TextBox>
            <asp:RangeValidator
                id="rvMaxKeywords"
                ControlToValidate="txtMaxKeywords"
                runat="server"
                Type="Integer"
                MinimumValue="1"
                MaximumValue="20"
                Display="Dynamic"
                ErrorMessage="Enter number of keywords to search, from 1 to 20, or leave blank for no limits"
                ></asp:RangeValidator>
        </td>
    </tr>
    <tr>
        <td><span class="label">Thumbnail</span></td>
        <td>
            <asp:CheckBox ID="chkShowThumbnail" runat="server" AutoPostBack="true" OnCheckedChanged="chkShowThumbnail_CheckedChanged" Text="Show" Checked="true" />
            
            <asp:Label 
                AssociatedControlID="txtThumbnailWidth" 
                ID="lblThumbnailWidth" 
                runat="server" >Width:</asp:Label>
            <asp:TextBox ID="txtThumbnailWidth" runat="server" Width="60" Text="80"></asp:TextBox>
            <asp:RangeValidator 
                ID="rvThumbnailWidth" 
                ControlToValidate="txtThumbnailWidth"
                runat="server"
                Display="Dynamic"
                ErrorMessage="Invalid Width"
                Type="Integer"
                MinimumValue="0"
                MaximumValue="9999"
                ></asp:RangeValidator>
                
            <asp:Label 
                AssociatedControlID="txtThumbnailHeight" 
                ID="lblThumbnailHeight" 
                runat="server">Height:</asp:Label>
            <asp:TextBox 
                ID="txtThumbnailHeight" 
                runat="server" 
                Width="60"
                Text="80"></asp:TextBox>
            <asp:RangeValidator 
                ID="rvThumbnailHeight" 
                runat="server"
                ControlToValidate="txtThumbnailHeight"
                Display="Dynamic"
                ErrorMessage="Invalid Height"
                Type="Integer"
                MinimumValue="0"
                MaximumValue="9999"
                ></asp:RangeValidator>                
        </td>
    </tr>
    <tr>
        <td><span class="label">Url</span></td>
        <td><asp:CheckBox ID="chkShowUrl" runat="server" Text="Show" Checked="true" /></td>
    </tr>
    <tr>
        <td><span class="label">Read More</span></td>
        <td><asp:CheckBox ID="chkReadMore" runat="server" Text="Show" Checked="true" /></td>
    </tr>
    <tr>
        <td><span class="label">Prev / Next</span></td>
        <td><asp:CheckBox ID="chkPrevNext" runat="server" Text="Show" Checked="true" /></td>
    </tr>
</table>
<div class="button-bar">
	<asp:LinkButton ID="btnSave" runat="server" Text="Save" OnClick="btnSave_Click"></asp:LinkButton>
	<uc:StatusMessage ID="statusMessage" runat="server" />
</div>			