<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="RolesList.ascx.cs" Inherits="scms.admin.controls.RolesList" %>
<%@ Register Src="~/scms/admin/controls/StatusMessage.ascx" TagPrefix="uc" TagName="statusMessage" %>


<div style="color:Black;font-weight:bold; width:650px;clear:both;margin-bottom:15px;">
    <div style="width:300px;float:left;">Name</div>
    <div style="width:100px;float:left;text-align:center;">Action</div>
</div>

<asp:Repeater 
ID="rptRoles"
runat="server">
    <ItemTemplate>
        <div style="width:650px;clear:both;">
            <div style="width:300px;float:left;"><%# Container.DataItem.ToString() %></div>
            <div style="width:100px;float:left;text-align:center;">
							<asp:ImageButton ToolTip="View Users" ID="btnUsers" runat="server" ImageUrl="/scms/client/images/action_user.gif" OnCommand="btnRole_Command" CommandName="CustomDelete" CommandArgument='<%# Container.DataItem %>'  />
							<asp:ImageButton ToolTip="Delete Role" ID="btnDelete" runat="server" ImageUrl="/scms/client/images/action_delete.gif" OnCommand="Delete_Command" CommandName="CustomDelete" CommandArgument='<%# Container.DataItem %>' OnClientClick='<%# GetDeleteMessage(Container.DataItem) %>' />
						</div>
        </tr>
    </ItemTemplate>
</asp:Repeater>
<br />



<asp:Panel ID="panelNewRole" DefaultButton="btnNewRole" runat="server" style="margin-top:15px;padding: 5px 5px;background-color:#eef;clear:both;">
    Role Name: 
    <asp:TextBox ID="txtRoleName" runat="server" Width="100"></asp:TextBox>
    <asp:RequiredFieldValidator 
        ID="rfRoleName" 
        runat="server"
        ControlToValidate="txtRoleName"
        Display="Dynamic"
        ErrorMessage="*"
        ValidationGroup="newRole"
        ></asp:RequiredFieldValidator>
	&nbsp;<asp:LinkButton ID="btnNewRole" runat="server" OnClick="btnNewRole_Click" Text="Create New Role" ValidationGroup="newRole" CausesValidation="true" ></asp:LinkButton>
</asp:Panel>
<uc:statusMessage ID="statusMessage" runat="server" />