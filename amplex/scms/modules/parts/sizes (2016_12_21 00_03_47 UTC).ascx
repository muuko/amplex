<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="sizes.ascx.cs" Inherits="amplex.scms.modules.parts.Sizes" %>
<%@ Register Src="~/scms/admin/controls/StatusMessage.ascx" TagPrefix="uc" TagName="statusMessage" %>

<style type="text/css">

	.list-table td
	{
		line-height: 24px;
	}
	.admin-settings td
	{
		padding: 3px 0;
	}
	
	

</style>

<asp:MultiView ID="multiView" runat="server" ActiveViewIndex="0">
	<asp:View ID="viewList" runat="server">
		<table class="list-table" border="0" cellpadding="0" cellspacing="0" width="100%">
			<tr>
				<th align="left">ID</th>
				<th align="left">Name</th>
				<th align="center">Action</th>
			</tr>
		    
			<asp:Repeater 
				ID="rptData"
				runat="server"
				OnItemDataBound="rptData_ItemDataBound">
				<ItemTemplate>
					<tr>
						<td><%# Eval("size.id")%></td>
						<td><%# Eval("size.name") %></td>
						<td align="center">
							<asp:ImageButton ID="btnUp" runat="server" ImageUrl="/scms/client/images/arrow_top.gif" OnCommand="Move" CommandName="Up" CommandArgument='<%# Eval("size.id") %>'  />
							<asp:ImageButton ID="btnDown" runat="server" ImageUrl="/scms/client/images/arrow_down.gif" OnCommand="Move" CommandName="Down" CommandArgument='<%# Eval("size.id") %>'  />						
							<asp:ImageButton ID="btnReferences" runat="server" ImageUrl="/scms/client/images/action_references.png" OnCommand="ShowReferences" CommandName="" CommandArgument='<%# Eval("size.id") %>' />
							<asp:ImageButton ID="btnEdit" runat="server" ImageUrl="/scms/client/images/action_edit.gif" OnCommand="Edit_Command" CommandName="CustomEdit" CommandArgument='<%# Eval("size.id") %>'  />
							<asp:ImageButton ID="btnDelete" runat="server" ImageUrl="/scms/client/images/action_delete.gif"  OnCommand="Delete_Command" CommandName="CustomDelete" CommandArgument='<%# Eval("size.id") %>' OnClientClick='<%# GetDeleteMessage(Container.DataItem) %>' />
						</td>
					</tr>
				</ItemTemplate>
			</asp:Repeater>
		</table>
		<div class="button-bar">
			<asp:LinkButton ID="btnNew" runat="server" Text="New" OnClick="btnNew_Click"></asp:LinkButton>
		</div>
	</asp:View>
	<asp:View ID="viewEdit" runat="server">
		<table class="admin-settings" border="0" cellpadding="0" cellspacing="0">
			<tr>
				<td><label>ID</label></td>
				<td class="value"><asp:HiddenField ID="hiddenId" runat="server" /><asp:TextBox ID="txtId" runat="server" MaxLength="11" /></td>
			</tr>
			<tr>
				<td><label>Name</label></td>
				<td class="value"><asp:TextBox ID="txtName" runat="server"></asp:TextBox>
				<asp:RequiredFieldValidator 
					ID="rfvName" 
					runat="server" 
					ValidationGroup="feed" 
					Display="Dynamic"
					ErrorMessage="*"
					ControlToValidate="txtName"></asp:RequiredFieldValidator>
				</td>
			</tr>
		</table>
		<div class="button-bar">
			<asp:LinkButton ID="btnSave" runat="server" ValidationGroup="feed" CausesValidation="true" OnClick="btnSave_Click" Text="Save"></asp:LinkButton>
			<asp:LinkButton ID="btnCancel" runat="server" OnClick="btnCancel_Click" Text="Cancel"></asp:LinkButton>
		</div>
		
	</asp:View>
	<asp:View ID="viewReferences" runat="server">
		<asp:Repeater 
			ID="rptReferences" 
			runat="server"
			OnItemDataBound="rptReferences_ItemDataBound"
			>
			<HeaderTemplate>
				<ul>
			</HeaderTemplate>
			<ItemTemplate>
				<li><a id="anchorPart" runat="server"></a></li>
			</ItemTemplate>
			<FooterTemplate>
				</ul>
			</FooterTemplate>
		</asp:Repeater>
		
		<div class="button-bar">
			<asp:LinkButton ID="btnReturnToSizes" runat="server" OnClick="btnCancel_Click" Text="Return to sizes"></asp:LinkButton>
		</div>
		
	</asp:View>
</asp:MultiView>

<uc:statusMessage ID="statusMessage" runat="server" />