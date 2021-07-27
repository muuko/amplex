<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="fields.ascx.cs" Inherits="scms.modules.forms.form.controls.fields" %>
<%@ Register Src="~/scms/admin/controls/StatusMessage.ascx" TagName="statusMessage" TagPrefix="uc" %>
<asp:ListView ID="lvFields" runat="server" OnItemDataBound="lvFields_ItemDataBound">
	<EmptyDataTemplate>
		<div style="margin-top:10px;"><em>No data to display</em></div>
	</EmptyDataTemplate>
	<LayoutTemplate >
		<table>
			<tr>
				<th></th>
				<th align="left">Name</th>
				<th>Action</th>
			</tr>
			<asp:PlaceHolder ID="itemPlaceholder" runat="server"></asp:PlaceHolder>	
		</table>
	</LayoutTemplate>
	<ItemTemplate >
		<tr>
			<td><div style="display:none"><%# Eval("Id") %></div></td>
			<td>
				<asp:LinkButton ID="btnField" runat="server" OnCommand="Select" CommandArgument='<%# Eval("id") %>' Text='<%# Eval("Name") %>' />
			</td>
			<td>
				<asp:ImageButton ID="btnUp" runat="server" ImageUrl="/scms/client/images/arrow_top.gif" OnCommand="Move" CommandName="Up" CommandArgument='<%# Eval("id") %>'  />
				<asp:ImageButton ID="btnDown" runat="server" ImageUrl="/scms/client/images/arrow_down.gif" OnCommand="Move" CommandName="Down" CommandArgument='<%# Eval("id") %>'  />
				<asp:ImageButton ID="btnDelete" runat="server" ImageUrl="/scms/client/images/action_delete.gif" OnCommand="Delete" CommandName="CustomDelete" CommandArgument='<%# Eval("id") %>' OnClientClick="javascript: return confirm('Delete this field?');" />
			</td>
		</tr>
	</ItemTemplate>
	<EmptyDataTemplate><em>No Data To Display</em></EmptyDataTemplate>
</asp:ListView>
<uc:statusMessage ID="statusMessage" runat="server" />