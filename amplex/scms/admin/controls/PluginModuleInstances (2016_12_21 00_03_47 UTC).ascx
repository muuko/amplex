<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="PluginModuleInstances.ascx.cs" Inherits="scms.admin.controls.PluginModuleInstances" %>
<%@ Register Src="~/scms/admin/controls/StatusMessage.ascx" TagPrefix="uc" TagName="StatusMessage" %>

<div class="pmiTable" style="clear:both;">
	<div class="pmiRow">
		<div runat="server" visible="false" class="pmiCellHeader pmiCellId">ID</div>
		<div class="pmiCellHeader pmiCellName" >Name</div>
		<div class="pmiCellHeader pmiCellPlugin">Plugin</div>
		<div class="pmiCellHeader pmiCellPlaceHolder" >PlaceHolder</div>
		<div class="pmiCellHeader pmicellOwner">Owner</div>
		<div class="pmiCellHeader pmiCellTemplateOverride" id="divCellTemplateOverride" runat="server" >Template<br />Override</div>
		<div class="pmiCellHeader pmiCellAction">Action</div>
	</div>
</div>
<div style="clear:both"></div>
		
<asp:ListView ID="lvPluginModuleInstances" runat="server" OnItemDataBound="lvPluginModuleInstances_ItemDataBound">
	<EmptyDataTemplate>
		<div style="margin-top:10px;"><em>No data to display</em></div>
	</EmptyDataTemplate>
	<LayoutTemplate >
		<asp:PlaceHolder ID="itemPlaceholder" runat="server"></asp:PlaceHolder>	
	</LayoutTemplate>
	<ItemTemplate >
		<div class="pmiRow">
			<div runat="server" visible="false" class="pmiCell pmiCellId"><%# Eval("Id")%></div>
			<div class="pmiCell pmiCellName" ><a href="<%# Eval("EditUrl") %>"><%# Eval("Name") %></a></div>
			<div class="pmiCell pmiCellPlugin"><%# Eval("PluginApplication") + "." + Eval("PluginModule") %></div>
			<div class="pmiCell pmiCellPlaceHolder" ><%# Eval("PlaceHolder") %></div>
			<div class="pmiCell pmicellOwner"><%# Eval("Owner") %></div>
			<div class="pmiCell pmiCellTemplateOverride" id="divCellTemplateOverride" runat="server" ></div>
			<div class="pmiCell pmiCellAction">
				<asp:ImageButton ID="btnUp" runat="server" ImageUrl="/scms/client/images/arrow_top.gif" OnCommand="MoveModuleInstance" CommandName="Up" CommandArgument='<%# Eval("id") %>'  />
				<asp:ImageButton ID="btnDown" runat="server" ImageUrl="/scms/client/images/arrow_down.gif" OnCommand="MoveModuleInstance" CommandName="Down" CommandArgument='<%# Eval("id") %>'  />
				<asp:ImageButton ID="btnDelete" runat="server" ImageUrl="/scms/client/images/action_delete.gif" OnCommand="Delete" CommandName="CustomDelete" CommandArgument='<%# Eval("id") %>' OnClientClick="javascript: return confirm('Delete this module?');" />
			</div>
		</div>
	</ItemTemplate>
</asp:ListView>
<div style="display:block;clear:both;"><uc:StatusMessage ID="statusMessage" runat="server" /></div>