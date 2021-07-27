<%@ Page Language="C#" MasterPageFile="~/scms/admin/Admin.master" AutoEventWireup="true" CodeBehind="module.aspx.cs" Inherits="scms.admin.module" Title="Untitled Page" EnableViewStateMac="false" ValidateRequest="false" EnableEventValidation="false"	 %>
<%@ Register Src="~/scms/admin/controls/AdminPagesBreadcrumbs.ascx" TagName="AdminPagesBreadcrumbs" TagPrefix="uc" %>
<%@ Register Src="~/scms/admin/controls/SiteDdl.ascx" TagName="SiteDdl" TagPrefix="uc" %>
<%@ Register Src="~/scms/admin/controls/StatusMessage.ascx" TagName="Status" TagPrefix="uc" %>
<%@ Register Src="~/scms/admin/controls/PlaceholderDdl.ascx" TagName="PlaceholderDdl" TagPrefix="uc" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
	<asp:MultiView ID="multiViewHeader" runat="server" ActiveViewIndex="0">
		<asp:View ID="viewPageModule" runat="server">
			<uc:AdminPagesBreadcrumbs ID="pagesBreadcrumbs" runat="server"/>
		</asp:View>
		<asp:View ID="viewTemplateModule" runat="server">
			<uc:SiteDdl ID="siteDdl" runat="server" /> > <a href="~/scms/admin/templates.aspx" runat="server">templates</a> > <a id="anchorTemplate" runat="server"></a></asp:Literal>
		</asp:View>
	</asp:MultiView>
	: <asp:Literal ID="literalModuleName" runat="server"></asp:Literal>
	<a style="margin-left:25px;" id="anchorView" runat="server" href="#" visible="false"><strong style="color:gray;font-size:12px;">View This Page</strong></a> 
	<hr />	
	

	<asp:Menu 
		ID="menuTabs" 
		runat="server"
		Orientation="Horizontal"
		OnMenuItemClick="menuTabs_Click"
		StaticMenuItemStyle-CssClass="tabbed-menu"
		StaticHoverStyle-CssClass="tabbed-menu-hover"
		StaticSelectedStyle-CssClass="tabbed-menu-selected"
	>
		<Items>
		    <asp:MenuItem Text="Settings" Value="Settings" Selected="true" ></asp:MenuItem>
			<asp:MenuItem Text="Advanced" Value="Advanced" ></asp:MenuItem>
			
		</Items>
	</asp:Menu>
	
	<asp:MultiView id="multiViewBody" runat="server">
		<asp:View ID="viewAdvanced" runat="server">
		
			<table cellspacing="2" cellpadding="2">
				<tr runat="server" visible="false" >
					<td><span class="label">Instance Id</span></td>
					<td><asp:Literal ID="literalModuleInstanceId" runat="server"></asp:Literal></td>
				</tr>
				<tr>
					<td><span class="label">Name</span></td>
					<td><asp:TextBox ID="txtModuleName" runat="server" Width="200"></asp:TextBox></td>
				</tr>
				<tr>
					<td><span class="label">Placeholder</span></td>
					<td><uc:PlaceholderDdl ID="placeholderDdl" runat="server" /></td>
				</tr>
				<tr runat="server" visible="false">
					<td><span class="label">Ordinal</span></td>
					<td><asp:Literal ID="literalOrdinal" runat="server"></asp:Literal></td>
				</tr>
				<tr>
					<td valign="top"><span class="label">Owner</span></td>
					<td valign="top">
					    <table cellpadding="0" cellspacing="0"><tr><td valign="top"><asp:CheckBox ID="checkOwner" runat="server" Enabled="false" /></td>
					    <td valign="top" style="padding-left:10px;">
						
						<asp:MultiView ID="multiViewOwnership" runat="server" ActiveViewIndex="0">
						    <asp:View ID="viewOwnerNotSharing" runat="server"></asp:View>
						    <asp:View ID="viewOwnerSharing" runat="server">
						                  <table cellpadding="4" cellspacing="0">
						                    <tr><td valign="top" rowspan="3">Shared By:</td></tr>
						                    <tr><td valign="top">Templates</td><td valign="top"><asp:Literal ID="literalSharedTemplates" runat="server"></asp:Literal></td></tr>
						                    <tr><td valign="top">Pages</td><td valign="top"><asp:Literal ID="literalSharedPages" runat="server"></asp:Literal></td></tr>
						                  </table>
						    </asp:View>
						    <asp:View ID="viewShared" runat="server">
    						    Owner: <a href="#" ID="anchorOwner" runat="server"></a><br />
    						    <asp:LinkButton ID="btnTakeOwnership" runat="server" Text="Take Ownership" OnClick="btnTakeOwnership_Click"></asp:LinkButton>
						    </asp:View>
						</asp:MultiView>
						</td></tr></table>
					</td>
				</tr>
				<tr>
					<td valign="top"><span class="label">Wrap In Div</span></td>
					<td valign="top">
						<asp:CheckBox ID="checkWrapModule" runat="server" /><br />
						Css Class&nbsp; <asp:TextBox ID="txtCssClassWrap" runat="server"></asp:TextBox>
					</td>
				</tr>
				
				<asp:MultiView ID="multiViewAdvanced" runat="server">
					<asp:View ID="viewAdvancedPage" runat="server">
						<tr runat="server" visible="false">
							<td colspan="2" style="padding-top:15px;"><em>Page Specific Options</em></td>
						</tr>
						<tr runat="server" visible="false">
							<td><span class="label">Page Module Id</span></td>
							<td><asp:Literal ID="literalPageModuleInstanceId" runat="server"></asp:Literal></td>
						</tr>
						<tr>
							<td><span class="label">Override<br />Template</span></td>
							<td><asp:CheckBox ID="checkOverrideTemplate" runat="server" /></td>
						</tr>
					</asp:View>
					<asp:View ID="viewAdvancedTemplate" runat="server">
						<tr runat="server" visible="false">
							<td colspan="2" style="padding-top:15px;"><em>Template Specific Options</em></td>
						</tr>
						<tr runat="server" visible="false">
							<td><span class="label">Template Module Id</span></td>
							<td><asp:Literal ID="literalTemplateModuleInstanceId" runat="server"></asp:Literal></td>
						</tr>						
					</asp:View>
					
				</asp:MultiView>
			
			</table>
			
			<div class="button-bar">
				<asp:LinkButton ID="btnSaveAdvanced" runat="server" Text="Save" OnClick="btnSaveAdvanced_Click"></asp:LinkButton>
				<uc:Status ID="status" runat="server" />
				
			</div>
			
		</asp:View>
		
		<asp:View ID="viewSettings" runat="server">
			<asp:PlaceHolder ID="placeholderSettings" runat="server"></asp:PlaceHolder>
		</asp:View>
		
	</asp:MultiView>
	
</asp:Content>
