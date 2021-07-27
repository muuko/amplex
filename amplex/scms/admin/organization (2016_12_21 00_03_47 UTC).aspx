<%@ Page Language="C#" MasterPageFile="~/scms/admin/Admin.Master" AutoEventWireup="true" CodeBehind="organization.aspx.cs" Inherits="scms.admin.Organization" Title="Admin - Security - Organization" %>
<%@ Register Src="~/scms/admin/controls/StatusMessage.ascx" TagPrefix="uc" TagName="statusMessage" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
<style type="text/css">
    label
    {
    	font-weight:bold;
    	margin-right:10px;
    }
    
    tr.scms-user-row
    {
    	height:28px;
    }
</style>

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

	<hr />
	<a href="/scms/admin/security.aspx?view=organizations"><< all organizations</a>

	<table>
		<tr class="scms-user-row">
			<td><label >Organization Name:</label></td>
	    <td><asp:Literal ID="literalName" runat="server" ></asp:Literal></td>
	  </tr>
	   
		<asp:ListView 
			ID="lvOrganizations" 
			runat="server"
			OnItemDataBound="lvOrganizations_ItemDataBound"
			>
			<LayoutTemplate><asp:Literal ID="itemPlaceholder" runat="server"></asp:Literal></LayoutTemplate>
			<ItemTemplate>
				<tr class="scms-user-row">
					<td><label><asp:Literal ID="literalAttribute" runat="server"></asp:Literal></label></td>
					<td><asp:HiddenField ID="hiddenAttributeId" runat="server" /><asp:TextBox ID="txtAttributeValue" runat="server" Width="250" ></asp:TextBox></td>
				</tr>
			</ItemTemplate>
		</asp:ListView>
		
		<tr>
			<td>
				<asp:LinkButton ID="btnSave" runat="server" OnClick="btnSave_Click" Text="Save"></asp:LinkButton>
			</td>
		</tr>
</table>
    

    <uc:statusMessage ID="statusMessage" runat="server" />   

</asp:Content>
