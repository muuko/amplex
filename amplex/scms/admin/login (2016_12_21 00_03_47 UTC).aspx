<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="login.aspx.cs" Inherits="scms.admin.loginPage" %>
<%@ Register Src="~/scms/admin/controls/StatusMessage.ascx" TagName="statusMessage" TagPrefix="uc" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>Admin Login</title>
</head>
<body>
    <form id="form1" runat="server">
			<asp:LoginView ID="loginView" runat="server">
				<AnonymousTemplate>
					<div style="width:300px;margin-left:auto;margin-right:auto;">
						<asp:MultiView 
							ID="multiView" 
							runat="server"
							ActiveViewIndex="0">
							<asp:View ID="viewLogin" runat="server">
								<asp:Login
									id="login"
									runat="server"
									DisplayRememberMe="false"
									VisibleWhenLoggedIn="false"
							
									></asp:Login>
							</asp:View>
							<asp:View ID="viewCreateDefaultUser" runat="server">
								<fieldset>
									<legend><strong>Create Admin User</strong></legend>
									<table>
										<tr>
											<td>User Name</td>
											<td><asp:TextBox ID="txtUserName" runat="server" Width="150"></asp:TextBox>
												<asp:RequiredFieldValidator 
													ID="rfvUserName" 
													runat="server"
													ControlToValidate="txtUserName"
													Display="Dynamic"
													ErrorMessage="*">
													</asp:RequiredFieldValidator>
											</td>
										</tr>
										<tr>
											<td>Password</td>
											<td><asp:TextBox ID="txtPassword" TextMode="Password" runat="server" Width="150"></asp:TextBox>
												<asp:RequiredFieldValidator 
													ID="RequiredFieldValidator1" 
													runat="server"
													ControlToValidate="txtUserName"
													Display="Dynamic"
													ErrorMessage="*">
													</asp:RequiredFieldValidator>
											</td>
										</tr>
										<tr>
											<td>Confirm</td>
											<td><asp:TextBox ID="txtConfirmPassword" TextMode="Password" runat="server" Width="150"></asp:TextBox>
												<asp:CompareValidator
													id="cvConfirmPassword"
													runat="server"
													ControlToCompare="txtPassword"
													ControlToValidate="txtConfirmPassword"
													Display="Dynamic"
													ErrorMessage="Passwords do not match"
													Operator="Equal"
													></asp:CompareValidator>
											</td>
										</tr>
										<tr>
											<td>Email</td>
											<td><asp:TextBox ID="txtEmail" runat="server" Width="200"></asp:TextBox></td>
										</tr>
									</table>
									<asp:LinkButton 
										ID="btnCreateUser" 
										runat="server" 
										Text="Create User"
										CausesValidation="true"
										OnClick="btnCreateUser_Click"
										></asp:LinkButton>
										
										<uc:statusMessage ID="statusMessage" runat="server" />
								</fieldset>
							</asp:View>
						</asp:MultiView>
					</div>
				</AnonymousTemplate>
				<LoggedInTemplate>
					<div style="width:300px;margin-left:auto;margin-right:auto;text-align:center">
						Welcome, <asp:LoginName runat="server" /> <asp:LoginStatus runat="server" />
					</div>
				</LoggedInTemplate>
			</asp:LoginView>
    </form>
</body>
</html>
