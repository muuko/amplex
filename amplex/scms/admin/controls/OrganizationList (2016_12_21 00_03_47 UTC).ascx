<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="OrganizationList.ascx.cs" Inherits="scms.admin.controls.OrganizationList" %>
<%@ Register Src="~/scms/admin/controls/StatusMessage.ascx" TagPrefix="uc" TagName="statusMessage" %>

<div style="color:Black;font-weight:bold; width:650px;clear:both;margin-bottom:15px;">
    <div style="width:300px;float:left;">Name</div>
    <div style="width:100px;float:left;text-align:center;">Action</div>
</div>

<asp:Repeater 
ID="rpt"
runat="server">
	<ItemTemplate>
      <div style="width:650px;clear:both;">
				<div style="width:300px;float:left;"><asp:LinkButton ID="btnEdit" runat="server" Text='<%# Eval("name") %>' OnCommand="btn_Command" CommandName="edit" CommandArgument='<%# Eval("id") %>' ></asp:LinkButton>
			</div>
			<div style="width:100px;float:left;text-align:center;">
				<asp:ImageButton ToolTip="View Users" ID="btnUsers" runat="server" ImageUrl="/scms/client/images/action_user.gif" OnCommand="btn_Command" CommandName="users" CommandArgument='<%# Eval("id") %>'  />
				<asp:ImageButton ID="btnDelete" runat="server" ImageUrl="/scms/client/images/action_delete.gif" OnCommand="Delete_Command" CommandName="CustomDelete" CommandArgument='<%# Eval("id") %>' OnClientClick='<%# GetDeleteMessage(Container.DataItem) %>' />
			</div>
		</tr>
	</ItemTemplate>
</asp:Repeater>
<br />



<asp:Panel ID="panelNew" DefaultButton="btnNew" runat="server" style="margin-top:15px;padding: 5px 5px;background-color:#eef;clear:both;">
    Name: 
    <asp:TextBox ID="txtName" runat="server" Width="100"></asp:TextBox>
    <asp:RequiredFieldValidator 
        ID="rfName" 
        runat="server"
        ControlToValidate="txtName"
        Display="Dynamic"
        ErrorMessage="*"
        ValidationGroup="new"
        ></asp:RequiredFieldValidator>
	&nbsp;<asp:LinkButton ID="btnNew" runat="server" OnClick="btnNew_Click" Text="Create New Organization" ValidationGroup="new" CausesValidation="true" ></asp:LinkButton>
</asp:Panel>
<uc:statusMessage ID="statusMessage" runat="server" />