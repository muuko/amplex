<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="PageList.ascx.cs" Inherits="scms.admin.controls.PageListControl" %>
<%@ Register Src="~/scms/admin/controls/StatusMessage.ascx" TagPrefix="uc" TagName="statusMessage"  %>

<div class="pageTable" style="clear:both;">
	<div class="pageRow">
		<div runat="server" visible="false" class="pageCellHeader pageCellId">ID</div>
		<div class="pageCellHeader pageCellLinkText">Link Text</div>
		<div class="pageCellHeader pageCellType">Type</div>
		<div class="pageCellHeader pageCellVisible">Visible</div>
		<div class="pageCellHeader pageCellAction">Action</div>
	</div>

		
<asp:ListView ID="lvPages" runat="server" >
	<LayoutTemplate >
		<asp:PlaceHolder ID="itemPlaceholder" runat="server"></asp:PlaceHolder>	
	</LayoutTemplate>
	<EmptyDataTemplate>
		<div style="margin-top:10px;"><em>No data to display</em></div>
	</EmptyDataTemplate>
	<ItemTemplate >
		<div class="pageRow">
			<div runat="server" visible="false" class="pageCell pageCellId"><%# Eval("id")%></div>
			<div class="pageCell pageCellLinkText" ><a href="/scms/admin/pages.aspx?pid=<%# Eval("id") %>"><%# Eval("linktext") %></a></div>
			<div class="pageCell pageCellType" ><%# Eval("type") %></div>
			<div class="pageCell pageCellVisible"><%# Eval("visible") %></div>
			<div class="pageCell pageCellAction">
				<asp:ImageButton ID="btnUp" runat="server" ImageUrl="/scms/client/images/arrow_top.gif" OnCommand="Move" CommandName="Up" CommandArgument='<%# Eval("id") %>'  />
				<asp:ImageButton ID="btnDown" runat="server" ImageUrl="/scms/client/images/arrow_down.gif" OnCommand="Move" CommandName="Down" CommandArgument='<%# Eval("id") %>'  />
				<asp:ImageButton ID="btnDelete" runat="server" ImageUrl="/scms/client/images/action_delete.gif" OnCommand="Delete" CommandName="CustomDelete" CommandArgument='<%# Eval("id") %>' OnClientClick="javascript: return confirm('Delete this child page?');" />
			</div>
		</div>
	</ItemTemplate>
</asp:ListView>
</div>
<div style="clear:both;"></div>

<uc:statusMessage id="statusMessage" runat="server"></uc:statusMessage>