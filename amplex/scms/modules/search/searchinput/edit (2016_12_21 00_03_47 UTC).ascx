<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="edit.ascx.cs" Inherits="scms.modules.search.search.edit" %>
<%@ Register Src="~/scms/admin/controls/SelectImage.ascx" TagPrefix="uc" TagName="selectImage" %>
<%@ Register Src="~/scms/admin/controls/StatusMessage.ascx" TagName="statusMessage" TagPrefix="uc" %>
<%@ Register Src="~/scms/admin/controls/PageSelector.ascx" TagName="pageSelector" TagPrefix="uc" %>

<table cellspacing="2" cellpadding="2">
	<tr>
		<td valign="top"><span class="label">CSS Class<br />Text Active</span></td>
		<td valign="top">
		    <asp:TextBox ID="txtCssClass" runat="server" Width="150" Text="search"></asp:TextBox>
		</td>
	</tr>
	<tr>
		<td><span class="label">Default Text</span></td>
		<td>
		    <asp:TextBox ID="txtDefaultText" runat="server" Width="150" Text="Enter Keyword(s)"></asp:TextBox>
		</td>
	</tr>
	<tr>
		<td valign="top"><span class="label">CSS Class<br />Text Active</span></td>
		<td valign="top">
		    <asp:TextBox ID="txtCssClassTextActive" runat="server" Width="150" Text="search-text-active"></asp:TextBox>
		</td>
	</tr>
	<tr>
		<td valign="top"><span class="label">CSS Class<br />Text Inactive</span></td>
		<td valign="top">
		    <asp:TextBox ID="txtCssClassTextInactive" runat="server" Width="150" Text="search-text-inactive"></asp:TextBox>
		</td>
	</tr>		
	<tr>
		<td valign="top"><span class="label">Validation<br />Error Message</span></td>
		<td valign="top">
			<asp:TextBox ID="txtValidationErrorMessage" runat="server" Width="400" Text="Please enter search keyword(s)"></asp:TextBox>
		</td>
	</tr>
	<tr>
		<td valign="top"><span class="label">CSS Class Button</span></td>
		<td valign="top">
		    <asp:TextBox ID="txtCssClasButton" runat="server" Width="150" Text="search-button"></asp:TextBox>
		</td>
	</tr>	
	<tr>
	    <td valign="top"><span class="label">Button Type</span></td>
	    <td valign="top">
	        <asp:RadioButtonList
	            id="ddlType"
	            runat="server"
	            AutoPostBack="true"
	            OnSelectedIndexChanged="ddlType_SelectedIndexChanged"
	            >
	            <asp:ListItem Text="standard button" Value="button" Selected="True"></asp:ListItem>
	            <asp:ListItem Text="image" Value="image" ></asp:ListItem>
	        </asp:RadioButtonList>
	    </td>
	</tr>
	<asp:MultiView ID="mvDetails" runat="server" ActiveViewIndex="0">
	    <asp:View ID="viewButton" runat="server">
	        <tr>
	            <td valign="top"><span class="label">Button Text</span></td>
	            <td valign="top"><asp:TextBox ID="txtButtonText" runat="server" Width="200" Text="Search"></asp:TextBox></td>
	        </tr>
	    </asp:View>
	    <asp:View ID="viewImage" runat="server">
	    	<tr>
        	    <td valign="top"><span class="label">Image</span></td>
    	        <td valign="top"><uc:selectImage ID="selectImage" runat="server" /></td>
	        </tr>
	    </asp:View>
	</asp:MultiView>
	<tr>
		<td valign="top"><span class="label">Search Results<br />Page Override</span></td>
		<td valign="top">
		    <uc:pageSelector ID="pageSearchResultsOverride" runat="server" />
		</td>
	</tr>	
</table>	
<div class="button-bar">
	<asp:LinkButton ID="btnSave" runat="server" Text="Save" OnClick="btnSave_Click"></asp:LinkButton>
	<uc:StatusMessage ID="statusMessage" runat="server" />
</div>			

