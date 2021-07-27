<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="editfield.ascx.cs" Inherits="scms.modules.forms.form.controls.editfield" %>
<asp:Panel ID="panelErrorSummary" runat="server" DefaultButton="btnSave">
<asp:ValidationSummary 
	style="margin-top:20px;border:solid 1px color:red;"
	ID="validationSummary" 
	runat="server"
	ValidationGroup="EditField"
	DisplayMode="BulletList"
	ShowMessageBox="true"
	ShowSummary="true"
	HeaderText="Please correct these items:"
	 />
</asp:Panel>
<asp:Panel ID="panel" runat="server" DefaultButton="btnSave">
	<table>
		<tr>
			<td><span class="label">Name*</span></td>
			<td><asp:TextBox ID="txtName" runat="server"></asp:TextBox>
				<asp:RequiredFieldValidator
					ID="rqName"
					runat="server"
					ControlToValidate="txtName"
					ErrorMessage="Name is required"
					Display="Dynamic"
					ValidationGroup="EditField"
					SetFocusOnError="true"
	></asp:RequiredFieldValidator>
					<asp:CustomValidator 
						id="cvName"
						runat="server"
						ControlToValidate="txtName"
						ErrorMessage="this name is already in use"
						Display="Dynamic"
						ValidationGroup="EditField"
						OnServerValidate="cvName_ServerValidate"
						></asp:CustomValidator>
			</td>
		</tr>
		<tr>
			<td><span class="label">Label</span></td>
			<td><asp:TextBox ID="txtLabel" runat="server"></asp:TextBox>
					<asp:RequiredFieldValidator 
						ID="rqvLabel"
						runat="server"
						ControlToValidate="txtLabel"
						ErrorMessage="Label is required"
						Display="Dynamic"
						ValidationGroup="EditField"
						Enabled="false"
						></asp:RequiredFieldValidator>
			</td>
		</tr>
		<tr>
			<td><span class="label">Type</span></td>
			<td><asp:DropDownList ID="ddlType" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlType_SelectedIndexChanged" >
				<asp:ListItem Text="Text" Value="text" Selected="True"></asp:ListItem>
				<asp:ListItem Text="Text Area" Value="textarea"></asp:ListItem>
				<asp:ListItem Text="Check Box" Value="checkbox"></asp:ListItem>
				<asp:ListItem Text="Check Box List" Value="checkboxlist"></asp:ListItem>
				<asp:ListItem Text="Drop Down List" Value="dropdownlist"></asp:ListItem>
				<asp:ListItem Text="Radio Button List" Value="radiobuttonlist"></asp:ListItem>
				<asp:ListItem Text="File Upload" Value="fileupload"></asp:ListItem>
				<asp:ListItem Text="Advanced: Literal" Value="literal"></asp:ListItem>
				<asp:ListItem Text="Advanced: Hidden" Value="hidden"></asp:ListItem>
				<asp:ListItem Text="Auto: IP Address" Value="auto:ipaddress"></asp:ListItem>
				<asp:ListItem Text="Auto: URL" Value="auto:url"></asp:ListItem>
				<asp:ListItem Text="Auto: Referrer" Value="auto:referrer"></asp:ListItem>
				<asp:ListItem Text="Auto: User ID" Value="auto:userid"></asp:ListItem>
			</asp:DropDownList></td>
		</tr>
		<tr id="trFileTypes" runat="server">
		    <td ><span class="label">File Types</span></td>
		    <td valign="top">
		        <asp:TextBox 
		            ID="txtFileTypes" 
		            runat="server" 
		            Width="250"
		            />
		    </td>
		</tr>
		
		<asp:PlaceHolder ID="placeholderRepeatOptions" runat="server" Visible="false">
		
		<tr>
		    <td valign="top"><span class="label">Repeat Direction</span></td>
		    <td valign="top">
		        <asp:DropDownList ID="ddlRepeatDirection" runat="server">
		            <asp:ListItem Selected="True"></asp:ListItem>
		            <asp:ListItem Text="Horizontal" Value="Horizontal"></asp:ListItem>
		            <asp:ListItem Text="Vertical" Value="Vertical"></asp:ListItem>
		        </asp:DropDownList>
		    </td>
		</tr>
		<tr>
		    <td valign="top"><span class="label">Repeat Layout</span></td>
		    <td valign="top">
		        <asp:DropDownList ID="ddlRepeatLayout" runat="server">
		            <asp:ListItem Selected="True"></asp:ListItem>
		            <asp:ListItem Text="Table" Value="Table"></asp:ListItem>
		            <asp:ListItem Text="Flow" Value="Flow"></asp:ListItem>
		        </asp:DropDownList>
		    </td>
		</tr>		
		<tr>
		    <td valign="top"><span class="label">Repeat Columns</span></td>
		    <td valign="top">
		        <asp:TextBox ID="txtRepeatColumns" runat="server" Width="100" MaxLength="4"></asp:TextBox>
		        <asp:RangeValidator
		            id="rvRepeatColumns"
		            runat="server"
		            ControlToValidate="txtRepeatColumns"
		            Display="Dynamic"
		            ErrorMessage=" 1 to 99"
		            ValidationGroup="EditField"
		            Type="Integer"
		            MinimumValue="1"
		            MaximumValue="99"
		            ></asp:RangeValidator>
		    </td>
		</tr>
		
		
		</asp:PlaceHolder>
		
		<tr id="trOptions" runat="server">
			<td valign="top"><span class="label">Options</span></td>
			<td valign="top">
				<table>
					<tr>
						<th>Name</th>
						<th>Value</th>
						<th>Action</th>
					</tr>
					<asp:Repeater ID="rptOptions" runat="server">
						<ItemTemplate>
							<tr>
								<td><%# Eval("name") %></td>
								<td><%# Eval("value") %></td>
								<td>
									<asp:ImageButton ID="btnUp" runat="server" ImageUrl="/scms/client/images/arrow_top.gif" OnCommand="MoveOption" CommandName="Up" CommandArgument='<%# Eval("id") %>'  />
									<asp:ImageButton ID="btnDown" runat="server" ImageUrl="/scms/client/images/arrow_down.gif" OnCommand="MoveOption" CommandName="Down" CommandArgument='<%# Eval("id") %>'  />
									<asp:ImageButton ID="btnDelete" runat="server" ImageUrl="/scms/client/images/action_delete.gif" OnCommand="DeleteOption" CommandName="CustomDelete" CommandArgument='<%# Eval("id") %>' OnClientClick="javascript: return confirm('Delete this option?');" />						
								</td>
							</tr>
						</ItemTemplate>
					</asp:Repeater>
					<tr>
						<td>
							<asp:TextBox ID="txtOptionName" runat="server"></asp:TextBox>
							<asp:RequiredFieldValidator
								id="rfvOptionName"
								runat="server"
								ControlToValidate="txtOptionName"
								Display="Dynamic"
								ErrorMessage="Option Name is required"
								ValidationGroup="newOption"
								></asp:RequiredFieldValidator>
						</td>
						<td>
							<asp:TextBox ID="txtOptionValue" runat="server"></asp:TextBox>
						</td>
						<td><asp:LinkButton ID="btnNewOption" runat="server" Text="New Option" ValidationGroup="newOption" OnClick="btnNewOption_Click"></asp:LinkButton></td>
					</tr>
				</table>
				
			</td>
		</tr>
		<tr id="trDefaultValue" runat="server" >
			<td valign="top"><span class="label">Default Value</span></td>
			<td valign="top">
				<asp:TextBox ID="txtDefault" runat="server"></asp:TextBox>
				<asp:TextBox ID="txtDefaultMultiline" runat="server" TextMode="MultiLine" Rows="8" Width="400"></asp:TextBox>
				<asp:CheckBox ID="checkDefault" runat="server" />
				<asp:DropDownList ID="ddlDefault" runat="server"></asp:DropDownList>
			</td>
		</tr>
		<tr id="trWidth" runat="server">
			<td><span class="label">Width</span></td>
			<td>
				<asp:TextBox ID="txtWidth" runat="server" Width="50"></asp:TextBox>px
				<asp:RangeValidator 
					ID="rvWidth" 
					runat="server" 
					Type="Integer" 
					ControlToValidate="txtWidth" 
					MinimumValue="0" 
					MaximumValue="99999"
					ErrorMessage="width should be between 0-99999"
					Display="Dynamic"
					ValidationGroup="EditField"
					></asp:RangeValidator>
				
			</td>
		</tr>
		<tr id="trMaxLength" runat="server">
			<td><span class="label">Max Length</span></td>
			<td>
				<asp:TextBox ID="txtMaxLength" runat="server" Width="50"></asp:TextBox>
				<asp:RangeValidator
					id="rvMaxLength"
					runat="server"
					Type="Integer"
					ControlToValidate="txtMaxLength"
					MinimumValue="0"
					MaximumValue="99999"
					ErrorMessage="max length should be between 0-99999"
					Display="Dynamic"
					ValidationGroup="EditField"></asp:RangeValidator>
			</td>
		</tr>
		<asp:PlaceHolder ID="placeholderTextAreaSettings" runat="server">
			<tr>
				<td><span class="label">Columns</span></td>
				<td><asp:TextBox ID="txtColumns" runat="server" Width="50"></asp:TextBox></td>
			</tr>
			<tr>
				<td><span class="label">Rows</span></td>
				<td><asp:TextBox ID="txtRows" runat="server" Width="50"></asp:TextBox></td>
			</tr>
		</asp:PlaceHolder>
		
		<asp:PlaceHolder ID="placeholderRedact" runat="server">
		    <tr>
		        <td><span class="label">Redact In Notification</span></td>
		        <td><asp:CheckBox ID="checkRedactInNotification" runat="server" /></td>
		    </tr>
		</asp:PlaceHolder>
		
		
		<asp:PlaceHolder id="placeholderRequired" runat="server">
		    <tr>
			    <td><span class="label">Required</span></td>
			    <td><asp:CheckBox ID="checkRequired" runat="server" /></td>
		    </tr>
		    <tr>
		        <td><span class="label">Required Text</span></td>
		        <td><asp:TextBox ID="txtRequiredText" runat="server"></asp:TextBox></td>
		    </tr>
		    <tr>
					<td><span class="label">Validation Display</span></td>
					<td><asp:DropDownList ID="ddlValidationDisplay" runat="server">
						<asp:ListItem Text="None" Value="None"></asp:ListItem>
						<asp:ListItem Text="Static" Value="Static"></asp:ListItem>
						<asp:ListItem Text="Dynamic" Value="Dynamic"></asp:ListItem>
					</asp:DropDownList></td>
		    </tr>
		</asp:PlaceHolder>
		
		<asp:PlaceHolder ID="placeholderValidation" runat="server">
			<tr>
				<td><span class="label">Validation Regex</span></td>
				<td><asp:TextBox ID="txtValidationRegex" runat="server" Width="300"></asp:TextBox></td>
			</tr>
			<tr>
				<td><span class="label">Validation Failed Message</span></td>
				<td><asp:TextBox ID="txtValidationErrorMessage" runat="server" Width="400"></asp:TextBox></td>
			</tr>
		</asp:PlaceHolder>
		<asp:PlaceHolder ID="placeholderClassOverrides" runat="server">
		    <tr>
			    <td valign="top"><span class="label">Css Class Row</span></td>
			    <td valign="top"><asp:TextBox ID="txtCssClassOverrideRow" runat="server" Width="80"></asp:TextBox></td>
		    </tr>
		    <tr>
			    <td valign="top"><span class="label">Css Class Cell Label</span></td>
			    <td valign="top"><asp:TextBox ID="txtCssClassOverrideCellLabel" runat="server" Width="80"></asp:TextBox></td>
		    </tr>
		    <tr>
			    <td valign="top"><span class="label">Css Class Cell Value</span></td>
			    <td valign="top"><asp:TextBox ID="txtCssClassOverrideCellValue" runat="server" Width="80"></asp:TextBox></td>
		    </tr>
		</asp:PlaceHolder>
		<asp:PlaceHolder ID="placeholderPostSettings" runat="server">
			<tr>
				<td><span class="label">Re Post</span></td>
				<td><asp:CheckBox ID="checkPost" runat="server" AutoPostBack="true" OnCheckedChanged="checkPost_CheckedChanged" /></td>
			</tr>
			<tr>
				<td><span class="label">Re Post ID</span></td>
				<td>
					<asp:TextBox ID="txtPostId" runat="server" Width="100"></asp:TextBox>
					<asp:CustomValidator
						id="cvPostId"
						Runat="server"
						ErrorMessage="Re Post ID is required whe re-post is selected"
						Display="Dynamic"
						ValidationGroup="EditField"
						OnServerValidate="cvPostId_ServerValidate"
						></asp:CustomValidator>
				</td>
			</tr>
		</asp:PlaceHolder>
		<tr>
			<td colspan="2">
				<asp:LinkButton ID="btnSave" runat="server" Text="Save" OnClick="btnSave_Click" CausesValidation="true" ValidationGroup="EditField" ></asp:LinkButton>
				<asp:LinkButton ID="btnCancel" runat="server" Text="Cancel" OnClick="btnCancel_Click"></asp:LinkButton>
			</td>
		</tr>
	</table>
</asp:Panel>
