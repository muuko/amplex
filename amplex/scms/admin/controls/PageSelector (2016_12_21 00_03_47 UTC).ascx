<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="PageSelector.ascx.cs" Inherits="scms.admin.controls.PageSelector" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxControlToolkit" %>
<asp:LinkButton ID="btnPageId" runat="server" OnClientClick="return false;">[...]</asp:LinkButton>
<asp:TextBox ID="txtPageId" runat="server" Width="100" ReadOnly="true"></asp:TextBox>
<asp:Panel ID="panelPopup" runat="server" style="display: none; visibility: hidden;"  >
	<div style="border:solid 1px navy;background-color:White;width:300px;height:226px;">
		<asp:TreeView ID="tvSelectPage" runat="server" 
			Height="200" 
			style="overflow:auto;"
			OnTreeNodePopulate="Node_Populate"
			OnSelectedNodeChanged="tvSelectPage_SelectedNodeChanged"
		>
    <Nodes>
		</Nodes>
		</asp:TreeView>
		<div style="padding:6px;">
			<asp:LinkButton ID="btnCancel" runat="server" Text="Cancel" OnClick="btnCancel_Click"></asp:LinkButton>
			<asp:LinkButton ID="btnClear" runat="server" Text="Clear" OnClick="btnClear_Click"></asp:LinkButton>
		</div>
	</div>
</asp:Panel>

<ajaxControlToolkit:PopupControlExtender ID="popup" runat="server"
	TargetControlId = "txtPageId"
	PopupControlID="panelPopup"
	Position="Bottom"
	
	></ajaxControlToolkit:PopupControlExtender>

<ajaxControlToolkit:PopupControlExtender ID="PopupControlExtender1" runat="server"
	TargetControlId = "btnPageId"
	PopupControlID="panelPopup"
	Position="Bottom"
	></ajaxControlToolkit:PopupControlExtender>