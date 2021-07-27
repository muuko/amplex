<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="edit.ascx.cs" Inherits="scms.modules.rss.rssList.edit" %>
<%@ Register Src="~/scms/admin/controls/StatusMessage.ascx" TagPrefix="uc" TagName="StatusMessage" %>
<%@ Register Src="~/scms/admin/controls/PageSelector.ascx" TagPrefix="uc" TagName="PageSelector" %>

<style type="text/css">
.rss-list td, .rss-list span
{
	line-height: 24px;
}
</style>
<table class="rss-list" cellspacing="2" cellpadding="2">
	<tr>
		<td valign="top"><span class="label">Rss Feed</span></td>
		<td valign="top">
			<asp:DropDownList ID="ddlFeeds" runat="server"></asp:DropDownList>
			<asp:RequiredFieldValidator
				id="rfvFeeds"
				runat="server"
				ControlToValidate="ddlFeeds"
				Display="Dynamic"
				ErrorMessage="*"
				ValidationGroup="feedlist"
			></asp:RequiredFieldValidator>
		</td>
	</tr>
	
	<tr>
		<td valign="top"><span class="label">Templated</span></td>
		<td valign="top">
			<asp:CheckBox id="checkTemplateEnabled" runat="server" Text="Enabled" OnCheckedChanged="EnableControls" AutoPostBack="true" />
			<br />
			<em>use substitution variables ##TITLE##, ##DESCRIPTION##, ##LINKURL##, ##TITLE[50]##, ##DESCRIPTION[100]## (to trim)</em>
		</td>
	</tr>

	<tr>
    <td valign="top"><span class="label">Header Template</span></td>
    <td valign="top">
			<asp:TextBox 
				ID="txtHeaderTemplate" 
				runat="server" 
				TextMode="MultiLine"
				Columns="60" 
				Rows="6"></asp:TextBox>
    </td>
	</tr>
	
	<tr>
    <td valign="top"><span class="label">Item Template</span></td>
    <td valign="top">
			<asp:TextBox 
				ID="txtItemTemplate" 
				runat="server" 
				TextMode="MultiLine"
				Columns="60" 
				Rows="6"></asp:TextBox>
    </td>
	</tr>
	
	<tr>
    <td valign="top"><span class="label">Separator Template</span></td>
    <td valign="top">
			<asp:TextBox 
				ID="txtSeparatorTemplate" 
				runat="server" 
				TextMode="MultiLine"
				Columns="60" 
				Rows="6"></asp:TextBox>
    </td>
	</tr>
	
	<tr>
    <td valign="top"><span class="label">Footer Template</span></td>
    <td valign="top">
			<asp:TextBox 
				ID="txtFooterTemplate" 
				runat="server" 
				TextMode="MultiLine"
				Columns="60" 
				Rows="6"></asp:TextBox>
    </td>
	</tr>

	<tr>
    <td valign="top"><span class="label">Heading</span></td>
    <td valign="top">
			<asp:CheckBox ID="checkHeadingEnabled" runat="server" Text="Enabled" />&nbsp;
    </td>
	</tr>

	<tr>
    <td valign="top"><span class="label">Title</span></td>
    <td valign="top">
			<asp:CheckBox ID="checkTitleEnabled" runat="server" Text="Enabled" AutoPostBack="true" OnCheckedChanged="EnableControls" />&nbsp;
			<asp:CheckBox ID="checkTitleAsLink" runat="server" Text="Show as link" />
    </td>
	</tr>
	
	<tr>
    <td valign="top"><span class="label">Limit Items</span></td>
    <td valign="top">
      <asp:CheckBox ID="checkListLimitItems" runat="server" Text="Limit Items" AutoPostBack="true" OnCheckedChanged="EnableControls" />&nbsp;
      <asp:Label ID="labelListMaxItems" runat="server" Text="Max Items: "></asp:Label>
      <asp:TextBox ID="txtListMaxItems" runat="server" Width="40" MaxLength="4"></asp:TextBox>
      <asp:RequiredFieldValidator 
          ID="rfvListMaxItems" 
          runat="server"
          ControlToValidate="txtListMaxItems"
          Display="Dynamic"
          ErrorMessage="required"
          ValidationGroup="feedlist"
          ></asp:RequiredFieldValidator>
      <asp:RangeValidator
          id="rvListMaxItems"
          runat="server"
          ControlToValidate="txtListMaxItems"
          Display="Dynamic"
          ErrorMessage="1-99"
          MinimumValue="1"
          MaximumValue="99"
          ValidationGroup="feedlist"
          ></asp:RangeValidator>
      <br />
      
      <asp:CheckBox ID="checkListReadMore" runat="server" Text="Read More" AutoPostBack="true" OnCheckedChanged="EnableControls" />&nbsp;
      <asp:Label ID="labelListReadMoreText" runat="server" Text="Text: "></asp:Label>
      <asp:TextBox ID="txtListReadMoreText" runat="server" Width="100" MaxLength="256"></asp:TextBox>&nbsp;
      <asp:RequiredFieldValidator
          id="rfvReadMoreText"
          runat="server"
          ControlToValidate="txtListReadMoreText"
          Display="Dynamic"
          ValidationGroup="feedlist"
          ErrorMessage="required"></asp:RequiredFieldValidator>
      <asp:Label ID="labelListReadMorePage" runat="server" Text="Destination Page: "></asp:Label><uc:PageSelector ID="pageSelectorListReadMorePage" runat="server" />
      <asp:RequiredFieldValidator
          id="rfvReadMorePage"
          runat="server"
          ControlToValidate="pageSelectorListReadMorePage"
          Display="Dynamic"
          ValidationGroup="feedlist"
          ErrorMessage="required"></asp:RequiredFieldValidator>
    </td>
	</tr>
</table>	
<div class="button-bar">
	<asp:LinkButton ID="btnSave" runat="server" ValidationGroup="feedlist" CausesValidation="true" Text="Save" OnClick="btnSave_Click" ></asp:LinkButton>
	<uc:StatusMessage ID="statusMessage" runat="server" />
</div>			


