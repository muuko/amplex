<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="edit.ascx.cs" Inherits="scms.modules.navigation.subnav.edit" %>
<%@ Register Src="~/scms/admin/controls/StatusMessage.ascx" TagPrefix="uc" TagName="StatusMessage" %>

<table cellspacing="2" cellpadding="2">
	<tr>
		<td><span class="label">Active Css Class</span></td>
		<td>
			<asp:TextBox ID="txtCssClassActive" runat="server" Width="100"></asp:TextBox>
		</td>
	</tr>	
	<tr>
		<td><span class="label">Maximum Depth</span></td>
		<td>
			<asp:TextBox id="txtMaximumDepth" runat="server" Width="20"></asp:TextBox>
			<asp:CompareValidator 
				ID="rvMaximumDepth" 
				runat="server" 
				Operator="GreaterThanEqual"
				ControlToValidate="txtMaximumDepth"
				ValueToCompare="0"
				Type="Integer" 
				Display="Dynamic" 
				ErrorMessage="Please leave blank or choose a number zero or greater"></asp:CompareValidator>
		</td>
	</tr>
	<tr>
		<td valign="top"><span class="label">Show Children</span></td>
		<td valign="top"><asp:CheckBox ID="checkShowChildren" runat="server" AutoPostBack="true" OnCheckedChanged="checkShowChildren_checkedChanged" />
		</td>
	</tr>
	<tr>
		<td valign="top"><span class="label">Max Children Per Node</span></td>
		<td valign="top"><asp:TextBox ID="txtMaxChildrenPerNode" runat="server" Width="20"></asp:TextBox>
			<asp:CompareValidator
				id="cvMaxChildrenPerNode"
				runat="server"
				Type="Integer"
				Operator="DataTypeCheck"
				ControlToValidate="txtMaxChildrenPerNode"
				ErrorMessage="Integer 0 or greater"
				Display="Dynamic"
				></asp:CompareValidator>
		</td>
		
	</tr>
	<tr>
		<td valign="top"><span class="label">Pin Navigation<br />to Home Page</span></td>
		<td valign="top">
			<asp:CheckBox 
				ID="checkPinNavigationToHomePage" 
				runat="server"
				AutoPostBack="true"
				OnCheckedChanged="checkPinNavigationToHomePage_CheckedChanged"
				 /></td>
	</tr>
	<tr>
		<td valign="top"><span class="label">Pin Depth</span></td>
		<td valign="top">
			<asp:TextBox ID="txtPinDepth" runat="server" Width="40"></asp:TextBox>
			<asp:RangeValidator 
				ID="rvPinDepth" 
				runat="server"
				ControlToValidate="txtPinDepth"
				Type="Integer"
				MinimumValue="0"
				MaximumValue="10"
				Display="Dynamic"
				ErrorMessage="<br />Please choose a number from 0 to 10, or leave blank"
				>
			</asp:RangeValidator>
		</td>
	</tr>
	<tr>
		<td valign="top"><span class="label">Show Siblings<br />if No Children</span></td>
		<td valign="top"><asp:CheckBox ID="checkShowSiblingsIfNoChildren" runat="server" /></td>
	</tr>
</table>	
<div class="button-bar">
	<asp:LinkButton ID="btnSave" runat="server" Text="Save" OnClick="btnSave_Click" CausesValidation="true"></asp:LinkButton>
	<uc:StatusMessage ID="statusMessage" runat="server" />
</div>			
