<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="settings.ascx.cs" Inherits="scms.modules.ticker.settings" %>
<%@ Register Src="~/scms/admin/controls/PageSelector.ascx" TagPrefix="uc" TagName="pageSelector" %>
<%@ Register Src="~/scms/admin/controls/StatusMessage.ascx" TagPrefix="uc" TagName="statusMessage" %>
<%@ Register Src="~/scms/admin/controls/ModuleSelector.ascx" TagPrefix="uc" TagName="moduleSelector" %>

<style type="text/css">
#tableMarquee th
{
	padding-right:20px;
}
</style>

<asp:MultiView ID="mv" runat="server">
	<asp:View ID="viewList" runat="server">
		<asp:ListView 
			ID="lvMarquee" 
			runat="server"
			OnItemDataBound="lvMarquee_ItemDataBound"
			>
			<EmptyDataTemplate>
				<em>no ticker items have been added</em>
			</EmptyDataTemplate>
			<LayoutTemplate>
				<table id="tableMarquee">
					<tr>
						<th>Label</th>
						<th>Value</th>
						<th>Link (Internal)</th>
						<th>Url (External)</th>
						<th>Action</th>
					</tr>
				<asp:PlaceHolder ID="itemPlaceholder" runat="server"></asp:PlaceHolder>
				</table>
			</LayoutTemplate>
			
			<ItemTemplate>
				<tr>
					<td>
						<%# Eval("label") %>
						<asp:HiddenField ID="hfId" runat="server" />
						<asp:HiddenField ID="hfOrdinal" runat="server" />
					</td>
					<td>
						<%# Eval("value") %>
					</td>
					<td>
						<asp:Literal ID="litLink" runat="server"></asp:Literal>
					</td>
					<td>
						<asp:Literal ID="litUrl" runat="server"></asp:Literal>
					</td>
					<td>
						<asp:ImageButton ID="btnUp" runat="server" ImageUrl="/scms/client/images/arrow_top.gif" OnCommand="onCommand" CommandName="Up" CommandArgument='<%# Eval("id") %>'  />
						<asp:ImageButton ID="btnDown" runat="server" ImageUrl="/scms/client/images/arrow_down.gif" OnCommand="onCommand" CommandName="Down" CommandArgument='<%# Eval("id") %>'  />
						<asp:ImageButton ID="btnEdit" runat="server" ImageUrl="/scms/client/images/action_edit.gif" OnCommand="onCommand" CommandName="customedit" CommandArgument='<%# Eval("id") %>' />
						<asp:ImageButton ID="btnDelete" runat="server" ImageUrl="/scms/client/images/action_delete.gif" OnCommand="onCommand" CommandName="customdelete" CommandArgument='<%# Eval("id") %>' OnClientClick="javascript: return confirm('Delete this marquee item?');" />
					</td>
				</tr>
			</ItemTemplate>
		</asp:ListView>	
		<div style="margin-top:20px;"><asp:LinkButton ID="btnNew" runat="server" OnClick="btnNew_Click" Text="New"></asp:LinkButton></div>
	</asp:View>
	<asp:View ID="viewEdit" runat="server">
		<table>
			<tr><td>ID:</td><asp:Literal ID="litId" runat="server"></asp:Literal></tr>
			<tr><td>Label:</td><td><asp:TextBox ID="txtLabel" runat="server"></asp:TextBox></td></tr>
			<tr><td>Value:</td><td><asp:TextBox ID="txtValue" runat="server"></asp:TextBox></td></tr>
			<tr><td>Link (Internal):</td><td><uc:pageSelector ID="pageLink" runat="server"  /></td></tr>
			<tr><td>Url (External):</td><td><asp:TextBox ID="txtUrl" runat="server" Width="400"></asp:TextBox></td></tr>
		</table>
		
		<div style="margin-top:20px;">
			<asp:LinkButton ID="btnSave" runat="server" OnClick="btnSave_Click" Text="Save"></asp:LinkButton>
			<asp:LinkButton ID="btnCancel" runat="server" OnClick="btnCancel_Click" Text="Cancel"></asp:LinkButton>
		</div>
		
	</asp:View>
</asp:MultiView>




<div>
	<uc:statusMessage ID="statusMessage" runat="server" />
</div>
