<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="UserList.ascx.cs" Inherits="scms.admin.controls.UserList" %>
<%@ Register Src="~/scms/admin/controls/StatusMessage.ascx" TagPrefix="uc" TagName="statusMessage" %>

<asp:Panel ID="panelSearch" runat="server" DefaultButton="btnSearch">
    <div style="margin-top:5px;margin-bottom:20px;padding: 5px 5px;background-color:#eef;clear:both;">
        User Name: <asp:TextBox ID="txtSearchUserName" runat="server" Width="50"></asp:TextBox>
        Email: <asp:TextBox ID="txtSearchEmailAddress" runat="server" Width="50"></asp:TextBox>
        First: <asp:TextBox ID="txtSearchFirstName" runat="server" Width="40"></asp:TextBox>
        Last: <asp:TextBox ID="txtSearchLastName" runat="server" Width="50"></asp:TextBox>
        Role: <asp:DropDownList ID="ddlSearchRole" runat="server"></asp:DropDownList>
        Organization: <asp:DropDownList ID="ddlSearchOrganization" runat="server"></asp:DropDownList>
        <asp:LinkButton ID="btnSearch" runat="server" Text="Search" OnClick="btnSearch_Click"></asp:LinkButton>
    </div>
</asp:Panel>

<style type="text/css">
.user-table th, .user-table td
{
	text-align:left;
}
</style>

<table class="user-table" border="0" cellpadding="0" cellspacing="0" width="100%">
    <tr>
        <th>User Name</th>
        <th>Email</th>
        <th>First</th>
        <th>Last</th>
        <th>Action</th>
    </tr>
    
<asp:Repeater 
ID="rptUsers"
runat="server"
OnItemDataBound="rptUsers_ItemDataBound"
>
    <ItemTemplate>
        <tr>
            <td><a id="anchorUserName" runat="server"></a></td>
            <td><%# Eval("email")%></td>
            <td><%# Eval("FirstName") %></td>
            <td><%# Eval("LastName") %></td>
            <td>
                <asp:ImageButton ID="btnEdit" runat="server" ImageUrl="/scms/client/images/action_edit.gif" OnCommand="Edit_Command" CommandName="CustomEdit" CommandArgument='<%# Eval("userName") %>'  />
                <asp:ImageButton ID="btnDelete" runat="server" ImageUrl="/scms/client/images/action_delete.gif" OnCommand="Delete_Command" CommandName="CustomDelete" CommandArgument='<%# Eval("userName") %>' OnClientClick='<%# GetDeleteMessage(Container.DataItem) %>' />
            </td>
        </tr>
    </ItemTemplate>
</asp:Repeater>
</table>

<div style="margin-left:4px;margin-top:8px;" id="divPager" runat="server">
	<asp:Repeater ID="rptPager" runat="server">
	    <HeaderTemplate>Pages:&nbsp; </HeaderTemplate>
		<ItemTemplate >
			<asp:LinkButton ID="lbPage" runat="server" OnCommand="lbPage_PageSelected" CommandName="page" CommandArgument="<%# (int)Container.DataItem %>" visible="<%# (int)Container.DataItem != nCurrentPage %>"  Text="<%# ((int)(Container.DataItem) + 1).ToString() %>"></asp:LinkButton>
			<asp:Literal ID="literalCurrentPage" runat="server" visible="<%# (int)Container.DataItem == nCurrentPage %>" Text='<%# "<strong>" + ((int)(Container.DataItem) + 1).ToString() + "</strong>" %>' ></asp:Literal>
		</ItemTemplate>
		<SeparatorTemplate>&nbsp;</SeparatorTemplate>
	</asp:Repeater>
</div>

<br />



<asp:Panel ID="panelNewUser" DefaultButton="btnNewUser" runat="server" style="margin-top:15px;padding: 5px 5px;background-color:#eef;clear:both;">
    User Name: 
    <asp:TextBox ID="txtUserName" runat="server" Width="100"></asp:TextBox>
    <asp:RequiredFieldValidator 
        ID="rfUserName" 
        runat="server"
        ControlToValidate="txtUserName"
        Display="Dynamic"
        ErrorMessage="*"
        ValidationGroup="newUser"
        ></asp:RequiredFieldValidator>
    Password: <asp:TextBox ID="txtPassword" runat="server" Width="100" TextMode="Password"></asp:TextBox>
    <asp:RequiredFieldValidator
        id="rfvPassword"
        runat="server"
        ControlToValidate="txtPassword"
        Display="Dynamic"
        ErrorMessage="*"
        ValidationGroup="newUser"
        ></asp:RequiredFieldValidator>
    Confirm: <asp:TextBox ID="txtPasswordConfirm" runat="server" Width="100" TextMode="Password"></asp:TextBox>
    <asp:RequiredFieldValidator 
        id="rfvPasswordConfirm"
        runat="server"
        ControlToValidate="txtPasswordConfirm"
        ErrorMessage="*"
        ValidationGroup="newUser"
        ></asp:RequiredFieldValidator>
    <asp:CompareValidator
        id="cvPassword"
        runat="server"
        ControlToValidate="txtPassword"
        ControlToCompare="txtPasswordConfirm"
        Display="Dynamic"
        ErrorMessage="Passwords don't match"
        ValidationGroup="newUser"></asp:CompareValidator>
	&nbsp;<asp:LinkButton ID="btnNewUser" runat="server" OnClick="btnNewUser_Click" Text="Create New User" ValidationGroup="newUser" CausesValidation="true" ></asp:LinkButton>
</asp:Panel>
<uc:statusMessage ID="statusMessage" runat="server" />
