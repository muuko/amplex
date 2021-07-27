<%@ Page Language="C#" MasterPageFile="~/scms/admin/Admin.Master" AutoEventWireup="true" CodeBehind="templates.aspx.cs" Inherits="scms.admin.templates" Title="Untitled Page" %>
<%@ Register Src="~/scms/admin/controls/SiteDdl.ascx" TagName="SiteDdl" TagPrefix="uc" %>
<%@ Register Src="~/scms/admin/controls/StatusMessage.ascx" TagName="statusMessage" TagPrefix="uc" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
	<uc:SiteDdl ID="siteDdl" runat="server"  /> > templates
	<hr />
	<asp:ListView ID="lvTemplates" runat="server" OnItemDataBound="lvTemplates_ItemDataBound">
		<LayoutTemplate>
			<table cellpadding="4">
				<tr>
					<td runat="server" visible="false"><span class="label">Id</span></td>
					<td><span class="label">Name</span></td>
					<td><span class="label">Master Name</span></td>
					<td><span class="label">Action</span></td>
				</tr>			
				<tr id="itemPlaceHolder" runat="server">
				</tr>
			</table>
		</LayoutTemplate>
		<ItemTemplate>
			<tr>
				<td runat="server" visible="false"><%# Eval("TemplateId") %></td>
				<td><a href="/scms/admin/template.aspx?sid=<%# Eval("SiteId") %>&tid=<%# Eval("TemplateId") %>"> <%# Eval("Name") %></a></td>
				<td><%# Eval("MasterName") %></td>
				<td>
					<asp:ImageButton 
						ID="btnDelete" 
						runat="server" 
						OnClientClick='javascript: return confirm("Delete this template?");'
						OnCommand="Delete" 
						CommandArgument='<%# Eval("TemplateId") %>' 
						ImageUrl="/scms/client/images/action_delete.gif" /></td>
			</tr>
		</ItemTemplate>
	</asp:ListView>
	
	<asp:Panel runat="server" DefaultButton="btnNewTemplate" style="margin-top:15px;padding: 5px 5px;height:22px;background-color:#eef;">
		<div style="display:inline;float:left;padding-top:5px;padding-right:5px;">New template</div>
		<div style="display:inline;float:left">
		
			<asp:TextBox id="txtnewTemplateName" runat="server" Width="100"></asp:TextBox>
			<asp:RequiredFieldValidator 
				ID="rfvnewTemplateName"
				runat="server"
				ControlToValidate="txtnewTemplateName"
				Display="Dynamic"
				ErrorMessage="*"
				ValidationGroup="new"
				></asp:RequiredFieldValidator>
		</div>					
		<div style="display:inline;float:left;margin-left:8px;padding-top:5px;padding-right:8px;">using Master</div>	
		<div style="display:inline;float:left;padding-right:8px;">
			<asp:DropDownList ID="ddlMasterPage" runat="server" >
			</asp:DropDownList>
		</div>

		<div style="display:inline;float:left;padding-top:4px;">
			<asp:LinkButton ID="btnNewTemplate" runat="server" OnClick="btnNew_Click" Text="Create" CausesValidation="true" ValidationGroup="new" ></asp:LinkButton>
		</div>
	</asp:Panel>
	
	<div style="width:800px;padding:20px;"><uc:statusMessage ID="statusMessage" runat="server" /></div>
	
</asp:Content>
