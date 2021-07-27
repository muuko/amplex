<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="edit.ascx.cs" Inherits="scms.modules.navigation.pagedetail.edit" %>
<%@ Register Src="~/scms/admin/controls/StatusMessage.ascx" TagPrefix="uc" TagName="StatusMessage" %>

<table cellspacing="2" cellpadding="2">
	<tr>
		<td><span class="label">Type</span></td>
		<td>
			<asp:DropDownList ID="ddlType" runat="server">
				<asp:ListItem Text="Link Text" Value="linktext" Selected="True"></asp:ListItem>
				<asp:ListItem Text="Title" Value="title" ></asp:ListItem>
				<asp:ListItem Text="Description" Value="description"></asp:ListItem>
				<asp:ListItem Text="Thumbnail" Value="thumbnail"></asp:ListItem>
				<asp:ListItem Text="Date Short" Value="dateshort"></asp:ListItem>
				<asp:ListItem Text="Date Long" Value="datelong"></asp:ListItem>
			</asp:DropDownList>
		</td>
	</tr>
	<tr>
		<td><span class="label">Wrap in Html Element</span></td>
		<td>
			<asp:CheckBox ID="checkWrapInHtmlElement" runat="server" Checked="false" AutoPostBack="true" OnCheckedChanged="checkWrapInHtmlElement_checkChanged" />
		</td>
	</tr>
	<tr>
		<td><span class="label">Html Element Type</span></td>
		<td><asp:TextBox ID="txtHtmlElementType" runat="server" Width="40"></asp:TextBox>
		<asp:CustomValidator
			id="cvHtmlElementType"
			runat="server"
			ErrorMessage="please enter html tag value such as h1, h2, div, span, etc."
			Display="Dynamic"
			OnServerValidate="cvHtmlElementType_ServerValidate"
			ValidateEmptyText="true"
			></asp:CustomValidator>
		</td>
	</tr>
	<tr>
		<td><span class="label">Html Element Css Class</span></td>
		<td><asp:TextBox ID="txtHtmlCssClass" runat="server"></asp:TextBox></td>
	</tr>
</table>	
<div class="button-bar">
	<asp:LinkButton ID="btnSave" runat="server" Text="Save" OnClick="btnSave_Click"></asp:LinkButton>
	<uc:StatusMessage ID="statusMessage" runat="server" />
</div>			
