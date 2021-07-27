<%@ Page Language="C#" MasterPageFile="~/scms/admin/Admin.Master" AutoEventWireup="true" CodeBehind="newmodule.aspx.cs" Inherits="scms.admin.NewModule" Title="Untitled Page" %>
<%@ Register Src="~/scms/admin/controls/AdminBreadcrumbs.ascx" TagName="breadcrumbs" TagPrefix="uc" %>
<%@ Register Src="~/scms/admin/controls/PlaceholderDdl.ascx" TagName="placeholderDdl" TagPrefix="uc" %>
<%@ Register Src="~/scms/admin/controls/PageSelector.ascx" TagName="pageSelector" TagPrefix="uc" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
	<uc:breadcrumbs ID="breadcrumbs" runat="server" />	
	<hr />

	<h2>Choose Plugin Module</h2>
		<asp:Panel ID="panelNewModule" runat="server" DefaultButton="btnNew" >
			<asp:ListView ID="lvPluginApplications" runat="server" OnItemDataBound="lvPluginApplications_ItemDataBound">
				<LayoutTemplate>
					<ul><asp:PlaceHolder ID="itemPlaceholder" runat="server"></asp:PlaceHolder></ul>
				</LayoutTemplate>
			
				<ItemTemplate>
					<li class="plugin-application">
						<strong><%# Eval("name") %></strong> - <%# Eval("description") %>
						<br />
							<asp:ListView ID="lvPluginModules" runat="server" OnItemDataBound="lvPluginModules_ItemDataBound" >
								<LayoutTemplate><ul><asp:PlaceHolder ID="itemPlaceholder" runat="server"></asp:PlaceHolder></ul></LayoutTemplate>
								<ItemTemplate>
									<li>
										<div style="display:inline;float:left;">
											<asp:PlaceHolder ID="placeholderRadio" runat="server"></asp:PlaceHolder>
										</div>
										<div style="display:inline;float:left">
											<strong><%# Eval("name") %></strong> -
											<%# Eval("description") %>
										</div>
										<div style="clear:both"></div>
									</li>
								</ItemTemplate>
							</asp:ListView>
					</li>
				</ItemTemplate>
			</asp:ListView>

			<span class="label">Placeholder:</span><br /><uc:placeholderDdl ID="placeholderDdl" runat="server" />
			<asp:PlaceHolder ID="placeholderOverride" runat="server" Visible="false">&nbsp;<asp:CheckBox ID="checkOverrideTemplate" runat="server" />Override Template</asp:PlaceHolder><br /><br />
			
			<span class="label">Sharing:</span><br /><asp:CheckBox ID="checkShare" runat="server" Text="Share existing module" AutoPostBack="true" OnCheckedChanged="checkShare_CheckChanged" />
			    <asp:Label ID="labelModuleSource" runat="server" AssociatedControlID="ddlModuleSource" Text="from"></asp:Label>
			    <asp:DropDownList 
			        ID="ddlModuleSource" 
			        runat="server"
			        AutoPostBack="true"
			        OnSelectedIndexChanged="ddlModuleSource_SelectedIndexChanged"
			        >
			        <asp:ListItem Text="Page" Value="page"></asp:ListItem>
			        <asp:ListItem Text="Template" Value="template"></asp:ListItem>
			    </asp:DropDownList>
			    <asp:MultiView ID="multiViewSharingSource" runat="server" ActiveViewIndex="0">
			        <asp:View ID="viewSharingDisabled" runat="server"></asp:View>
			        <asp:View ID="viewShareSourcePage" runat="server">
			            <uc:pageSelector 
			                ID="pageSelectorShare" 
			                runat="server" />
			        </asp:View>
			        <asp:View ID="viewShareSourceTemplate" runat="server">
			            <asp:DropDownList ID="ddlTemplateShare" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlTemplateShare_SelectedIndexChanged">
			            </asp:DropDownList>
			        </asp:View>
			    </asp:MultiView>
			    <asp:PlaceHolder ID="placeholderShareModule" runat="server" >
			        <asp:Label ID="labelShareModule" runat="server" Text="module name: " AssociatedControlID="ddlShareModule"></asp:Label> 
			        <asp:DropDownList ID="ddlShareModule" runat="server"></asp:DropDownList>
			    </asp:PlaceHolder>
			    <asp:CustomValidator
			        id="cvShare"
			        runat="server"
			        Display="Dynamic"
			        ErrorMessage="<br />Please select a module to share, or uncheck the sharing checkbox."
			        OnServerValidate="cvShare_ServerValidate"
			        >
			    </asp:CustomValidator>
			<br />
			<br />
			<span class="label">Name:</span><br /><asp:TextBox ID="txtName" runat="server" Width="150"></asp:TextBox><br />
			
			<br />
			<asp:LinkButton ID="btnNew" runat="server" OnClick="btnNew_Click" Text="Create Module"></asp:LinkButton>
	</asp:Panel>

</asp:Content>
