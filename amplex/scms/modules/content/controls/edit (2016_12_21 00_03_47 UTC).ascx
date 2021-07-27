<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="edit.ascx.cs" Inherits="scms.modules.content.edit"  %>
<%@ Register Src="~/scms/admin/controls/StatusMessage.ascx" TagPrefix="uc" TagName="statusMessage" %>

<!-- TinyMCE -->
<script type="text/javascript" src="/scms/client/jscript/tiny_mce/tiny_mce.js"></script>
<script type="text/javascript">
</script>
<!-- /TinyMCE -->





<asp:MultiView ID="multiView" runat="server" >
	<asp:View ID="viewWysiwyg" runat="server">
		<textarea name="txtWysiwygContent" cols="25" rows="20" id="txtWysiwygContent" runat="server"></textarea>		
	</asp:View>
	<asp:View ID="viewLiteral" runat="server">
		<textarea name="txtLiteralContent" style="width:800px" cols="80" rows="20" id="txtLiteralContent" runat="server"></textarea>		
	</asp:View>
</asp:MultiView>

<div>
	<asp:RadioButtonList ID="rblMode" runat="server" RepeatDirection="Horizontal" AutoPostBack="true" OnSelectedIndexChanged="rblMode_SelectedIndexChanged">
		<asp:ListItem Text="wysiwyg" Value="wysiwyg" Selected="True"></asp:ListItem>
		<asp:ListItem Text="literal" Value="literal"></asp:ListItem>
	</asp:RadioButtonList>
</div>

<div style="margin-top:10px;width:800px;">
	<div style="display:inline;text-align:right;float:right;">Revision:&nbsp; <asp:DropDownList ID="ddlVersion" runat="server" OnSelectedIndexChanged="ddlVersion_SelectedIndexChanged" AutoPostBack="true"></asp:DropDownList></div>
	<div style="display:inline;float:left;"><asp:LinkButton id="btnSave" runat="server" Text="Save" OnClick="btnSave_Click"></asp:LinkButton></div>
</div>
<div style="clear:both;"><uc:statusMessage ID="statusMessage" runat="server" /></div>


