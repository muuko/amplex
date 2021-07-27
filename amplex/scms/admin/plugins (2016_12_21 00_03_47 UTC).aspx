<%@ Page Language="C#" MasterPageFile="~/scms/admin/Admin.Master" AutoEventWireup="true" CodeBehind="plugins.aspx.cs" Inherits="scms.admin.plugins" Title="Admin - Plugins" %>


<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

<style type="text/css">
 .admin-content p
 {
 	margin: 2px 0 2px 0;
 	padding: 0;
 }
 .admin-content ul
 {
 	margin: 0px 0 0 8px;
 	padding:0;
 }
 
 .admin-content li
 {
 	margin:0;
 	padding:0;
 }
 
</style>


<hr />
<asp:Repeater 
	ID="rptPlugins"
	runat="server"
	OnItemDataBound="rptPlugins_ItemDataBound"
	>
	<ItemTemplate >
		<strong><a href="/scms/admin/pluginsettings.aspx?id=<%# Eval("id") %>" ><%# Eval("name") %></strong></a><br />
		<p><%# Eval("description") %></p>
		<p>
			<asp:Repeater
				id="rptModules"
				runat="server">
				<HeaderTemplate><table></HeaderTemplate>
				<ItemTemplate>
					<tr><td style="width:100px"><em><%# Eval("name") %></em></td><td><%# Eval("description") %></td></tr>
				</ItemTemplate>
				<FooterTemplate></table></FooterTemplate>
			</asp:Repeater>
		</p>
		<br />
		
	</ItemTemplate>
</asp:Repeater>
</asp:Content>
