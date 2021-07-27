<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="edit.ascx.cs" Inherits="scms.modules.forms.form.edit" %>
<%@ Register Src="~/scms/modules/forms/form/controls/editform.ascx" TagName="editform" TagPrefix="uc" %>
<%@ Register Src="~/scms/modules/forms/form/controls/fields.ascx" TagName="fields" TagPrefix="uc" %>
<%@ Register Src="~/scms/modules/forms/form/controls/editfield.ascx" TagName="editfield" TagPrefix="uc" %>
<%@ Register Src="~/scms/modules/forms/form/controls/viewResponses.ascx" TagName="responses" TagPrefix="uc" %>
<%@ Register Src="~/scms/modules/forms/form/controls/viewEvents.ascx" TagName="events" TagPrefix="uc" %>
<%@ Register Src="~/scms/admin/controls/StatusMessage.ascx" TagName="statusMessage" TagPrefix="uc" %>

Form: <asp:DropDownList ID="ddlForm" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlForm_SelectedIndexChanged" ></asp:DropDownList>
<asp:Panel style="display:inline" ID="panelNewForm" runat="server" DefaultButton="btnNewForm">
<span style="padding-left:60px;">New Form</span> <asp:TextBox ID="txtNewForm" runat="server" Width="80"></asp:TextBox> 
<asp:RequiredFieldValidator 
	ID="rfvNewForm" 
	runat="server"
	ControlToValidate="txtNewForm"
	Display="Dynamic"
	ErrorMessage="*"
	ValidationGroup="new"
	></asp:RequiredFieldValidator>
<asp:LinkButton 
	ID="btnNewForm" 
	runat="server" 
	OnClick="btnNewForm_OnClick" 
	CausesValidation="true" 
	ValidationGroup="new"
	Text="Go"
	></asp:LinkButton>
<asp:CustomValidator
	id="cvNewFormUnique"
	runat="server"
	ControlToValidate="txtNewForm"
	Display="Dynamic"
	ErrorMessage="This name is already in use"
	ValidationGroup="new"
	OnServerValidate="cvNewFormUnique_OnServerValidate"
	></asp:CustomValidator>
</asp:Panel>

<asp:Menu 
	style="margin-top:10px;"
		ID="menuTabs" 
		runat="server"
		Orientation="Horizontal"
		OnMenuItemClick="menuTabs_Click"
		StaticMenuItemStyle-CssClass="tabbed-menu"
		StaticHoverStyle-CssClass="tabbed-menu-hover"
		StaticSelectedStyle-CssClass="tabbed-menu-selected"
	>
		<Items>
			<asp:MenuItem Text="Responses" Value="responses" Selected="true"></asp:MenuItem>
			<asp:MenuItem Text="Settings" Value="settings" ></asp:MenuItem>
			<asp:MenuItem Text="Fields" Value="fields"></asp:MenuItem>
			<asp:MenuItem Text="Events" Value="events"></asp:MenuItem>
		</Items>
</asp:Menu>


<asp:MultiView ID="multiView" runat="server" ActiveViewIndex="0" >
	<asp:View ID="viewResponses" runat="server">
		<uc:responses ID="responses" runat="server" />		
	</asp:View>

	<asp:View ID="viewSettings" runat="server">
		<asp:Panel ID="panelForm" runat="server" style="margin-top:10px;">
			<uc:editform ID="editForm" runat="server" />
		</asp:Panel>
	</asp:View>
	
	<asp:View ID="viewFields" runat="server">
		<asp:Panel ID="panelFields" runat="server" CssClass="pagePanel">
			<fieldset >
				<legend>Fields</legend>
				<uc:fields id="fields" runat="server"></uc:fields>
			</fieldset>
			
			<asp:Panel ID="panelNewField" DefaultButton="btnNewField" runat="server" style="padding: 5px 5px;background-color:#eef;clear:both;">
				<div style="display:inline;float:left;margin-left:8px;padding-top:5px;padding-right:5px;"><span class="label">New Field&nbsp;&nbsp; Label</span></div>
				<asp:TextBox id="txtNewFieldLabel" runat="server" Width="250"></asp:TextBox>
				<asp:RequiredFieldValidator 
					ID="rfvNewField"
					runat="server"
					ControlToValidate="txtNewFieldLabel"
					Display="Dynamic"
					ErrorMessage="*"
					ValidationGroup="newfield"
					></asp:RequiredFieldValidator>
				<asp:LinkButton ID="btnNewField" runat="server" OnClick="btnNewField_Click" Text="Create" CausesValidation="true" ValidationGroup="newfield" ></asp:LinkButton>
			</asp:Panel>	
		</asp:Panel>
		
		<asp:Panel ID="panelEditField" runat="server" CssClass="pagePanel">
			<fieldset>
				<legend>Edit Field</legend>
				<uc:editfield ID="editField" runat="server" />
			</fieldset>
		</asp:Panel>
	</asp:View>
	<asp:View ID="viewEvents" runat="server">
	    <uc:events ID="events" runat="server" />
	</asp:View>
	
</asp:MultiView>



<uc:statusMessage ID="statusMessage" runat="server" />
