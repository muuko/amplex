<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="editform.ascx.cs" Inherits="scms.modules.forms.form.controls.editform" %>
<%@ Register Src="~/scms/admin/controls/PageSelector.ascx" TagPrefix="uc" TagName="pageSelector" %>
<%@ Register Src="~/scms/admin/controls/StatusMessage.ascx" TagPrefix="uc" TagName="statusMessage" %>

<asp:Panel ID="panelFormEdit" runat="server" CssClass="pagePanel" >
	<fieldset >
		<legend>Settings</legend>
		
		
				<td valign="top">
					<table cellspacing="5">
						<tr>
							<td><span class="label">Name</span></td>
							<td>
								<asp:TextBox ID="txtFormName" runat="server" Width="120"></asp:TextBox>
								<asp:RequiredFieldValidator
									id="rfvFormName"
									runat="server"
									ControlToValidate="txtFormName"
									ErrorMessage="*"
									Display="Dynamic"
									ValidationGroup="Form"
									></asp:RequiredFieldValidator>
							</td>
						</tr>
						<tr>
							<td><span class="label">Thank You Page</span></td>
							<td><uc:pageSelector ID="pageSelectorThankYouPage" runat="server" /></td>
						</tr>
						
						<tr>
							<td valign="top"><span class="label">Re Post Form</span></td>
							<td valign="top">
								<asp:CheckBox id="checkPost" runat="server" AutoPostBack="true" OnCheckedChanged="checkPost_CheckedChanged" />
								Url: <asp:TextBox ID="txtPostUrl" runat="server" Width="200"></asp:TextBox>
								<asp:CustomValidator
									id="cvPost"
									runat="server"
									ErrorMessage="*"
									Display="Dynamic"
									ValidationGroup="Form"
									OnServerValidate="cvPost_ServerValidate"
									></asp:CustomValidator>
							</td>
						</tr>
					</table>
					<fieldset>
						<legend>Notifications</legend>
					
					<table>
						<tr>
							<td valign="top">
	
								<span class="label">Send Notification:</span> 
							</td>
							<td>
								<asp:CheckBox ID="checkNotify" runat="server" AutoPostBack="true" OnCheckedChanged="checkNotify_CheckedChanged" />
								
								To Email: <asp:TextBox ID="txtNotifyEmailAddress" runat="server" width="200"></asp:TextBox>
								<asp:CustomValidator
									id="cvNotify"
									runat="server"
									ErrorMessage="*"
									Display="Dynamic"
									ValidationGroup="Form"
									OnServerValidate="cvNotify_ServerValidate"
									></asp:CustomValidator>
				</td>
				</tr>
				<tr>
					<td colspan="2" valign="top">
									
				<br />
				<span class="label">Conditional Notification Email Addresses:</span><br /><br />
				<table>
					<tr>
						<th>Field</th>
						<th>Contains Value</th>
						<th>Recipient Email</th>
						<th>Action</th>
					</tr>
					<asp:Repeater ID="rptNotifyByField" runat="server">
						<ItemTemplate>
							<tr>
								<td><%# Eval("name") %></td>
								<td><%# Eval("value") %></td>
								<td><%# Eval("email") %></td>
								<td>
									<asp:ImageButton ID="btnEdit" runat="server" ImageUrl="/scms/client/images/action_edit.gif" OnCommand="EditNotify" CommandName="CustomEdit" CommandArgument='<%# Container.DataItem.ToString() %>'  />
									<asp:ImageButton ID="btnDelete" runat="server" ImageUrl="/scms/client/images/action_delete.gif" OnCommand="DeleteNotify" CommandName="CustomDelete" CommandArgument='<%# Container.DataItem.ToString() %>' OnClientClick="javascript: return confirm('Delete this option?');" />
								</td>
							</tr>
						</ItemTemplate>
					</asp:Repeater>
					<tr>
						<td>
							<asp:DropDownList ID="ddlNotifyByFieldField" runat="server"></asp:DropDownList>
							<asp:RequiredFieldValidator
								id = "rfvDdlNotifyByFieldField"
								runat="server"
								controlToValidate ="ddlNotifyByFieldField"
								Display="Dynamic"
								ErrorMessage="*"></asp:RequiredFieldValidator>
						</td>
						<td>
							<asp:TextBox ID="txtNotifyByFieldValue" Width="200" runat="server"></asp:TextBox>
							<asp:RequiredFieldValidator
								id="rfvNotifyByFieldValue"
								runat="server"
								ControlToValidate="txtNotifyByFieldValue"
								Display="Dynamic"
								ErrorMessage="*"
								ValidationGroup="NotifyByField"
								></asp:RequiredFieldValidator>
						</td>
						<td>
							<asp:TextBox ID="txtNotifyByFieldEmail" Width="200" runat="server"></asp:TextBox>
							<asp:RequiredFieldValidator
								id="rfvNotifyByFieldEmail"
								runat="server"
								ControlToValidate="txtNotifyByFieldEmail"
								Display="Dynamic"
								ErrorMessage="*"
								ValidationGroup="NotifyByField"></asp:RequiredFieldValidator>
						</td>
						<td>
							<asp:LinkButton ID="btnNewNotify" runat="server" Text="New" CausesValidation="true" ValidationGroup="NotifyByField" OnClick="btnNewNotifyByField_Click" ></asp:LinkButton>
							<asp:LinkButton ID="btnSaveNotify" runat="server" Text="Ok" CausesValidation="true" ValidationGroup="NotifyByField" OnClick="btnSaveNotifyByField_Click" Visible="false" ></asp:LinkButton>
							<asp:LinkButton ID="btnCancelNotify" runat="server" Text="Cancel" CausesValidation="false" OnClick="btnCancelNotifyByField_Click" Visible="false" ></asp:LinkButton>
						</td>
					</tr>
				</table>
									
									
									
							</td>
						</tr>
					</table>
					</fieldset>
					<br />
					<fieldset>
					<legend>Form Generation</legend>
					
					
					<table>
						<tr>
							<td valign="top"><span class="label">Generate Form</span></td>
							<td valign="top">
								<asp:CheckBox ID="checkGenerate" runat="server" AutoPostBack="true" OnCheckedChanged="checkGenerate_CheckedChanged" />
								Type: 
								<asp:DropDownList ID="ddlGenerationType" runat="server" OnSelectedIndexChanged="ddlGenerationType_SelectedIndexChanged" AutoPostBack="true">
									<asp:ListItem Text="Table" Value="table"></asp:ListItem>
									<asp:ListItem Text="Div" Value="div"></asp:ListItem>
									<asp:ListItem Text="Manual" Value="manual" ></asp:ListItem>
								</asp:DropDownList>
							</td>
						</tr>
						<tr>
							<td valign="top"><span class="label">Spam Protection</span></td>
							<td valign="top">
								<table>
									<tr>
										<td>
											<asp:CheckBox ID="checkDummyFields" runat="server" Text="Dummy Fields" />
										</td>
										<td>
											<asp:CheckBox ID="checkSession" runat="server" Text="Session" />
										</td>
									</tr>
									<tr>
										<td>
											<asp:CheckBox ID="checkReferrer" runat="server" Text="Referrer" />
										</td>
										<td>
											<asp:CheckBox ID="checkRecaptcha" runat="server" Text="Recaptcha" />
										</td>
									</tr>
								</table>
							</td>
						</tr>
						<asp:MultiView ID="multiviewGenSettings" runat="server" ActiveViewIndex="0">
							<asp:View ID="viewAutoGenSettings" runat="server">
								<tr>
									<td valign="top"><span class="label">Submit Text</span></td>
									<td valign="top">
										<asp:TextBox ID="txtSubmitText" runat="server"></asp:TextBox>
									</td>
								</tr>
								<tr>
									<td valign="top"><span class="label">Summary Validation</span></td>
									<td valign="top">
										<asp:CheckBox ID="checkSummaryValidationEnabled" runat="server" AutoPostBack="true" Text="Enabled" OnCheckedChanged="checkSummaryValidationEnabled_CheckedChanged" />
										<table>
											<tr>
												<td valign="middle">
													<asp:Label ID="labelSummaryValidationCssClass" runat="server" AssociatedControlID="txtSummaryValidationCssClass" Text="Css Class"></asp:Label>
												</td>
												<td valign="middle">
													<asp:TextBox ID="txtSummaryValidationCssClass" runat="server"></asp:TextBox>
												</td>
											</tr>
											<tr>
												<td valign="middle">
													<asp:Label ID="labelSummaryValidationHeaderText" runat="server" AssociatedControlID="txtSummaryValidationHeaderText" Text="Header Text"></asp:Label>
												</td>
												<td valign="middle">
													<asp:TextBox ID="txtSummaryValidationHeaderText" runat="server"></asp:TextBox>
												</td>
											</tr>
											<tr>
												<td valign="middle">
													<asp:Label ID="labelSummaryValidationDisplayMode" runat="server" AssociatedControlID="ddlSummaryValidationDisplayMode" Text="Display Mode"></asp:Label>
												</td>
												<td valign="middle">
													<asp:DropDownList ID="ddlSummaryValidationDisplayMode" runat="server">
														<asp:ListItem Text="Bullet List" Value="BulletList" Selected="True"></asp:ListItem>
														<asp:ListItem Text="List" Value="List"></asp:ListItem>
														<asp:ListItem Text="Single Paragraph" Value="SingleParagraph"></asp:ListItem>
													</asp:DropDownList>
												</td>
											</tr>
											<tr>
												<td valign="middle"><asp:Label ID="labelSummaryValidationEnableClientScript" runat="server" Text="Client Script" AssociatedControlID="checkSummaryValidationEnableClientScript"></asp:Label></td>
												<td valign="middle"><asp:CheckBox ID="checkSummaryValidationEnableClientScript" runat="server" Text="Enabled" /></td>
											</tr>
											<tr>
												<td valign="middle"><asp:Label ID="labelSummaryValidationShowSummary" runat="server" Text="Summary" AssociatedControlID="checkSummaryValidationShowSummary" ></asp:Label></td>
												<td valign="middle"><asp:CheckBox ID="checkSummaryValidationShowSummary" runat="server" Text="Show" /></td>
											</tr>
											<tr>
												<td valign="middle"><asp:Label ID="labelSummaryValidationShowMessageBox" runat="server" Text="Message Box" AssociatedControlID="checkSummaryValidationShowMessageBox" ></asp:Label></td>
												<td valign="middle"><asp:CheckBox ID="checkSummaryValidationShowMessageBox" runat="server" Text="Show" /></td>
											</tr>
									</table>
									</td>
								</tr>
								<tr>
									<td><span class="label">Css Class Container</span></td>
									<td><asp:TextBox ID="txtCssClassContainer" runat="server"></asp:TextBox></td>
								</tr>
								<tr>
									<td><span class="label">Css Class Table</span></td>
									<td><asp:TextBox ID="txtCssClassTable" runat="server"></asp:TextBox></td>
								</tr>
								<tr>
									<td><span class="label">Css Class Row</span></td>
									<td><asp:TextBox ID="txtCssClassRow" runat="server"></asp:TextBox></td>
								</tr>
								<tr>
									<td><span class="label">Css Class Label</span></td>
									<td><asp:TextBox ID="txtCssClassLabel" runat="server"></asp:TextBox></td>
								</tr>
								<tr>
									<td><span class="label">Css Class Value</span></td>
									<td><asp:TextBox ID="txtCssClassValue" runat="server"></asp:TextBox></td>
								</tr>
								<tr>
									<td><span class="label">Css Class Input Row</span></td>
									<td><asp:TextBox ID="txtCssClassInputRow" runat="server"></asp:TextBox></td>
								</tr>
								
								</asp:View>
								<asp:View ID="viewManualGenSettings" runat="server">
									<tr>
										<td colspan="2"><span class="label">Raw Html</span><br />
											<asp:TextBox ID="txtManualForm" runat="server" TextMode="MultiLine" Rows="20" Columns="40">
											</asp:TextBox><br />
											<em>to submit, use onclick="javascript:ManualSubmit_<%=FormId %>();"</em>
											
										</td>
									</tr>
								</asp:View>
							</asp:MultiView>
					</table>
					</fieldset>
				
			
			
			<asp:LinkButton ID="btnSaveForm" runat="server" OnClick="btnSaveForm_Clicked" Text="Save" ValidationGroup="Form"></asp:LinkButton>
			<br />
			<uc:statusMessage ID="statusMessage" runat="server" />
				
			
	</fieldset>
</asp:Panel>
