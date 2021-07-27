<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="feeds.ascx.cs" Inherits="projectz.scms.modules.rss.feeds" %>
<%@ Register Src="~/scms/admin/controls/StatusMessage.ascx" TagPrefix="uc" TagName="statusMessage" %>

<style type="text/css">

	.feeds-table td
	{
		line-height: 24px;
	}

</style>

<asp:MultiView ID="multiView" runat="server" ActiveViewIndex="0">
	<asp:View ID="viewList" runat="server">
		<table class="feeds-table" border="0" cellpadding="0" cellspacing="0" width="100%">
			<tr>
				<th align="left">Name</th>
				<th align="left">Url</th>
				<th align="center">Action</th>
			</tr>
		    
			<asp:Repeater 
				ID="rptData"
				runat="server"
				OnItemDataBound="rptData_ItemDataBound">
				<ItemTemplate>
					<tr>
						<td><%# Eval("name")%></td>
						<td><%# Eval("feedUrl") %></td>
						<td align="center">
							<asp:ImageButton ID="btnEdit" runat="server" ImageUrl="/scms/client/images/action_edit.gif" OnCommand="Edit_Command" CommandName="CustomEdit" CommandArgument='<%# Eval("id") %>'  />
							<asp:ImageButton ID="btnDelete" runat="server" ImageUrl="/scms/client/images/action_delete.gif" OnCommand="Delete_Command" CommandName="CustomDelete" CommandArgument='<%# Eval("id") %>' OnClientClick='<%# GetDeleteMessage(Container.DataItem) %>' />
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
				<td class="value"><asp:Literal ID="litId" runat="server"></asp:Literal></td>
			</tr>
			<tr>
				<td><label>Name</label></td>
				<td class="value"><asp:TextBox ID="txtName" runat="server" Width="200"></asp:TextBox>
				<asp:RequiredFieldValidator 
					ID="rfvName" 
					runat="server" 
					ValidationGroup="feed" 
					Display="Dynamic"
					ErrorMessage="*"
					ControlToValidate="txtName"></asp:RequiredFieldValidator>
				</td>
			</tr>
			<tr>
				<td><label>Heading</label></td>
				<td class="value"><asp:TextBox ID="txtHeading" runat="server" Width="400"></asp:TextBox></td>
			</tr>			
			<tr>
				<td><label>Feed Url</label></td>
				<td class="value" ><asp:TextBox ID="txtFeedUrl" runat="server" Width="400"></asp:TextBox>
				<asp:RequiredFieldValidator 
					ID="rfvFeedUrl" 
					runat="server" 
					ValidationGroup="feed" 
					Display="Dynamic"
					ErrorMessage="*"
					ControlToValidate="txtFeedUrl"></asp:RequiredFieldValidator>
				</td>
			</tr>
			<tr>
				<td><label>Last Update</label></td>
				<td class="value"><asp:Literal ID="litLastUpdate" runat="server"></asp:Literal></td>
			</tr>
			<tr>
				<td><label>Max Age (Seconds)</label></td>
				<td class="value" ><asp:TextBox ID="txtExpiresSeconds" runat="server"></asp:TextBox>
					<asp:RequiredFieldValidator 
						ID="rfvExpiresSeconds" 
						runat="server" 
						ValidationGroup="feed" 
						Display="Dynamic"
						ErrorMessage="*"
						ControlToValidate="txtExpiresSeconds"></asp:RequiredFieldValidator>
						<asp:CompareValidator
						id="cvExpiresSeconds"
						runat="server"
						ValidationGroup="feed"
						Operator="GreaterThan"
						Type="Integer"
						ControlToValidate="txtExpiresSeconds"
						Display="Dynamic"
						ErrorMessage="integer &gt;=900"
						ValueToCompare="900"></asp:CompareValidator>
				</td>
			</tr>
			<tr>
				<td><label>Retain Drop-Off</label></td>
				<td class="value"><asp:CheckBox ID="checkRetainDropOff" runat="server" /></td>
			</tr>
			<tr>
				<td><label>Categories (leave blank if not limited)</label></td>
				<td class="value"><asp:TextBox ID="txtCategories" runat="server"></asp:TextBox></td>
			</tr>
		</table>
		<div class="button-bar">
			<asp:LinkButton ID="btnSave" runat="server" ValidationGroup="feed" CausesValidation="true" OnClick="btnSave_Click" Text="Save"></asp:LinkButton>
			<asp:LinkButton ID="btnCancel" runat="server" OnClick="btnCancel_Click" Text="Cancel"></asp:LinkButton>
		</div>
		
	</asp:View>
</asp:MultiView>

<uc:statusMessage ID="statusMessage" runat="server" />