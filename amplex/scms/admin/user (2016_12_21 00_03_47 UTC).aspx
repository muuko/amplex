<%@ Page Language="C#" MasterPageFile="~/scms/admin/Admin.Master" AutoEventWireup="true" CodeBehind="user.aspx.cs" Inherits="scms.admin.User" Title="Admin - Security - User" %>
<%@ Register Src="~/scms/admin/controls/StatusMessage.ascx" TagPrefix="uc" TagName="statusMessage" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
<style type="text/css">
    label
    {
    	font-weight:bold;
    	margin-right:10px;
    }
    
    tr.scms-user-row
    {
    	height:28px;
    }
</style>

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

	<hr />
	<a href="/scms/admin/security.aspx"><< all users</a>

    <table cellspacing="20">
        <tr>
            <td valign="top">
                <table>
                    <tr class="scms-user-row">
	                    <td><label >User Name:</label></td>
	                    <td><asp:Literal ID="literalUserName" runat="server" ></asp:Literal></td>
	                </tr>
	                <tr class="scms-user-row">
                        <td><label>Email Address:</label></td>
	                    <td><asp:TextBox ID="txtEmailAddress" runat="server" Width="250" ></asp:TextBox></td>
	                </tr>
	                <tr class="scms-user-row">
	                    <td><label>First Name:</label></td>
	                    <td><asp:TextBox ID="txtFirstName" runat="server" Width="100"></asp:TextBox></td>
	                </tr>
	                <tr class="scms-user-row">
	                    <td><label>Last Name:</label></td>
	                    <td><asp:TextBox ID="txtLastName" runat="server" Width="150"></asp:TextBox></td>
	                </tr>
	                <tr class="scms-user-row">
										<td><label>Organization:</label></td>
										<td><asp:DropDownList ID="ddlOrganization" runat="server"></asp:DropDownList></td>
	                </tr>
	                <tr>
	                    <td valign="top"><label>Roles:</label></td>
	                    <td valign="top"><asp:CheckBoxList ID="cblRoles" runat="server"></asp:CheckBoxList></td>
	                </tr>
            	    
	            </table>	
	            <div >
	                <asp:LinkButton ID="btnSave" runat="server" OnClick="btnSave_Click" Text="[Save]"></asp:LinkButton> &nbsp;
	                <asp:LinkButton ID="btnSendWelcomeEmail" runat="server" Visible="false" Enabled="false" OnClick="btnSendWelcomeEmail_Click" Text="[Send Welcome Email]"></asp:LinkButton>
	            </div>
            </td>
            <td valign="top">
                <asp:Panel ID="panelChangePassword" runat="server" DefaultButton="btnSetPassword">
                    <table>
                        <tr class="scms-user-row">
	                        <td><label>Password:</label>
	                            <asp:RequiredFieldValidator 
	                             ID="rfvPassword"
	                             runat="server"
	                             Display="Dynamic"
	                             ErrorMessage="*"
	                             ControlToValidate="txtPassword"
	                             ValidationGroup="password"
	                             ></asp:RequiredFieldValidator></td>
	                        <td><asp:TextBox ID="txtPassword" runat="server" TextMode="Password" ></asp:TextBox></td>
	                    </tr>
	                    <tr class="scms-user-row">
	                        <td><label>Confirm Password:
    	                        
	                        </label></td>
	                        <td><asp:TextBox ID="txtConfirmPassword" runat="server" TextMode="Password"></asp:TextBox></td>
    	                    
	                    </tr>
                    </table>
                    <asp:CompareValidator
	                                id="cvConfirmPassword"
	                                runat="server"
	                                ControlToValidate="txtConfirmPassword"
	                                ControlToCompare="txtPassword"
	                                Operator="Equal"
	                                Type="String"
	                                Display="Dynamic"
	                                ErrorMessage="<br />Passwords must match"
	                                ValidationGroup="password"
	                                ></asp:CompareValidator>
                    <asp:CustomValidator
                        id="custPassword"
                        runat="server"
                        ControlToValidate="txtPassword"
                        Display="Dynamic"
                        ErrorMessage="<br />Password must be at least 5 characcters"
                        ValidationGroup="password"
                        OnServerValidate="custPassword_ServerValidate"
                        ></asp:CustomValidator>
                    <br />
                    
	                <asp:LinkButton ID="btnSetPassword" runat="server" Text="Set Password"  OnClick="btnSetPassword_Click" ValidationGroup="password"></asp:LinkButton>
	                <br />
	                <uc:statusMessage ID="statusMessageSetPassword" runat="server" />   
	            </asp:Panel>
            </td>
        </tr>
    </table>
    

    <uc:statusMessage ID="statusMessage" runat="server" />   

</asp:Content>
