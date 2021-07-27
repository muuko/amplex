<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="view.ascx.cs" Inherits="scms.modules.forms.form.view" %>
<%@ Register TagPrefix="recaptcha" Namespace="Recaptcha" Assembly="Recaptcha" %>

<style type="text/css">
	.uefid23
	{
		display:none;
	}
</style>

<asp:Panel ID="panelGenerated" runat="server" DefaultButton="btnSubmit">
	
	<asp:Panel ID="panelInputs" runat="server" CssClass = "uefid23" Visible="false">
		<label for="email">email</label><input type="text" name="email" id="email" />
		<label for="url">url</label><input type="text" name="url" id="url" />
		<label for="phone">phone</label><input type="text" name="phone" id="phone" />
	</asp:Panel>

	<asp:MultiView ID="multiView" runat="server">
		
		<asp:View ID="viewTable" runat="server">
			<asp:PlaceHolder ID="placeholderTableGen" runat="server"></asp:PlaceHolder>
		</asp:View>
		
		<asp:View ID="viewDiv" runat="server">
			<div id="divGen" runat="server">
			</div>
		</asp:View>
	</asp:MultiView>
	
	<asp:PlaceHolder ID="placeholderRecaptcha" runat="server"></asp:PlaceHolder>
	
	
	
	
	<asp:ValidationSummary
		id="validationSummary"
		runat="server"
		Enabled="false"
		 />
	<div id="divSubmit" runat="server" class="form-submit">
		<asp:Button ID="btnSubmit" runat="server" OnClick="btnSubmit_Click" Text="Submit" />
	</div>
</asp:Panel>

<asp:Literal ID="literalManualForm" runat="server"></asp:Literal>
