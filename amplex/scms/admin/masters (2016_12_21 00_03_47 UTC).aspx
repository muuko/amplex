<%@ Page Language="C#" MasterPageFile="~/scms/admin/Admin.Master" AutoEventWireup="true" CodeBehind="masters.aspx.cs" Inherits="scms.admin.masters" Title="Untitled Page" EnableEventValidation="false" %>
<%@ Register Src="~/scms/admin/controls/SiteDdl.ascx" TagName="SiteDdl" TagPrefix="uc" %>
<%@ Register Src="~/scms/admin/controls/StatusMessage.ascx" TagName="statusMessage" TagPrefix="uc" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">

	<script type="text/javascript" language="javascript">

function ShowSelectWindowWithSource(siteid,source)
{
	var left,top;
	left = window.screenLeft - 150;
	top = window.screenTop - 50;
	
	var url;
	url = "/scms/modules/content/select-image.aspx?type=any&sid=" + siteid+ "&target=document.aspnetForm." + source + ".value&preselectUrl=" + document.getElementById(source).value;
	
	var selectWindow;
	selectWindow = open
	(
		url,
		"SelectImage",
		"modal=yes,width=800,height=400,resizable=1,status=1,scrollbars=1,top=" + top + ",left=" + left
	);
	if( selectWindow.focus )
	{
		selectWindow.focus();
	}
}
</script>	

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
	<uc:SiteDdl ID="siteDdl" runat="server"  /> > masters
	<hr />
	<asp:ListView ID="lvMasters" runat="server" OnItemDataBound="lvMasters_ItemDataBound">
		<LayoutTemplate>
			<table cellpadding="4">
				<tr>
					<td runat="server" visible="false"><span class="label">Id</span></td>
					<td><span class="label">Name</span></td>
					<td><span class="label">Path</span></td>
					<td><span class="label">Action</span></td>
				</tr>			
				<tr id="itemPlaceHolder" runat="server">
				</tr>
			</table>
		</LayoutTemplate>
		<ItemTemplate>
			<tr>
				<td runat="server" visible="false"><%# Eval("Id") %></td>
				<td><%# Eval("Name") %></td>
				<td><%# Eval("Path") %></td>
				<td>
					<asp:ImageButton 
						ID="btnDelete" 
						runat="server" 
						OnClientClick='javascript: return confirm("Delete this master?");'
						OnCommand="Delete" 
						CommandArgument='<%# Eval("Id") %>' 
						ImageUrl="/scms/client/images/action_delete.gif" /></td>
			</tr>
		</ItemTemplate>
	</asp:ListView>
	
	<asp:Panel runat="server" DefaultButton="btnNewMaster" style="margin-top:15px;padding: 5px 5px;height:22px;background-color:#eef;">
		<div style="display:inline;float:left;padding-top:5px;padding-right:5px;">New master</div>
		<div style="display:inline;float:left">
		
			<asp:TextBox id="txtnewMasterName" runat="server" Width="60"></asp:TextBox>
			<asp:RequiredFieldValidator 
				ID="rfvnewMasterName"
				runat="server"
				ControlToValidate="txtnewMasterName"
				Display="Dynamic"
				ErrorMessage="*"
				ValidationGroup="new"
				></asp:RequiredFieldValidator>
		</div>					
		<div style="display:inline;float:left;margin-left:8px;padding-top:5px;padding-right:8px;">Path:</div>	
		<div style="display:inline;float:left;padding-right:8px;">
			<asp:TextBox ID="txtPath" runat="server" Width="200"></asp:TextBox>
			<asp:RequiredFieldValidator
				id="rfvPath"
				runat="server"
				ControlToValidate="txtPath"
				Display="Dynamic"
				ErrorMessage="*"
				ValidationGroup="new"
				></asp:RequiredFieldValidator>
			<a id="anchorSource" name="anchorSource" runat="server" href="javascript:ShowSelectWindowWithSource('src');">[...]</a>
		</div>

		<div style="display:inline;float:left;padding-top:4px;">
			<asp:LinkButton ID="btnNewMaster" runat="server" OnClick="btnNew_Click" Text="Create" CausesValidation="true" ValidationGroup="new" ></asp:LinkButton>
		</div>
	</asp:Panel>
	
	<div style="width:800px;padding:20px;"><uc:statusMessage ID="statusMessage" runat="server" /></div>
	
</asp:Content>
