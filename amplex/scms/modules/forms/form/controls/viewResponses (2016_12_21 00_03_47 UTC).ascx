<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="viewResponses.ascx.cs" Inherits="scms.modules.forms.form.controls.viewResponses" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxControlToolkit" %>
<asp:MultiView id="multiView" runat="server" ActiveViewIndex="0">
	<asp:View ID="viewList" runat="server">
		<table style="width:100%;">
			<asp:Literal ID="literalHeaders" runat="server"></asp:Literal>
			<asp:Literal ID="literalValues" runat="server"></asp:Literal>
		</table>
		<div style="margin-left:4px;margin-top:8px;" id="divPager" runat="server">
			<asp:Repeater ID="rptPager" runat="server">
			    <HeaderTemplate>Pages:&nbsp; </HeaderTemplate>
				<ItemTemplate >
					<asp:LinkButton ID="lbPage" runat="server" OnCommand="lbPage_PageSelected" CommandName="page" CommandArgument="<%# (int)Container.DataItem %>" visible="<%# (int)Container.DataItem != nCurrentPage %>"  Text="<%# ((int)(Container.DataItem) + 1).ToString() %>"></asp:LinkButton>
					<asp:Literal ID="literalCurrentPage" runat="server" visible="<%# (int)Container.DataItem == nCurrentPage %>" Text='<%# "<strong>" + ((int)(Container.DataItem) + 1).ToString() + "</strong>" %>' ></asp:Literal>
				</ItemTemplate>
				<SeparatorTemplate>&nbsp;</SeparatorTemplate>
			</asp:Repeater>
		</div>
		<div style="margin-top:15px;">
            Showing <asp:TextBox ID="txtFrom" runat="server" Width="60" style="text-align:right"></asp:TextBox> 
            <asp:ImageButton ID="btnFrom" runat="server" ImageUrl="~/scms/client/images/calendar.gif" />
            <ajaxControlToolkit:CalendarExtender
                id="ceFrom"
                runat="server"
                TargetControlID="txtFrom"
                PopupButtonID="btnFrom"
                >
            </ajaxControlToolkit:CalendarExtender>
            <asp:RangeValidator
                id="rvFrom"
                runat="server"
                ControlToValidate="txtFrom"
                Display="Dynamic"
                ErrorMessage="??"
                Type="Date"
                MinimumValue="1/1/1900"
                MaximumValue="1/1/2100"
                ValidationGroup="Refresh"
                ></asp:RangeValidator>
            <asp:RequiredFieldValidator
                id="rfvFrom"
                runat="server"
                ControlToValidate="txtFrom"
                Display="Dynamic"
                ErrorMessage="*"
                ValidationGroup="Refresh"
                ></asp:RequiredFieldValidator>
            To <asp:TextBox ID="txtTo" runat="server" Width="60"></asp:TextBox>
            <asp:ImageButton ID="btnTo" runat="server" ImageUrl="~/scms/client/images/calendar.gif" />
            <ajaxControlToolkit:CalendarExtender
                id="ceTo"
                runat="server"
                TargetControlID="txtTo"
                PopupButtonID="btnTo"
                >
            </ajaxControlToolkit:CalendarExtender>
            <asp:RangeValidator
                id="rvTo"
                runat="server"
                ControlToValidate="txtTo"
                Display="Dynamic"
                ErrorMessage="??"
                Type="Date"
                MinimumValue="1/1/1900"
                MaximumValue="1/1/2100"
                ValidationGroup="Refresh"
                ></asp:RangeValidator>
            <asp:RequiredFieldValidator
                id="rfvTo"
                runat="server"
                ControlToValidate="txtTo"
                Display="Dynamic"
                ErrorMessage="*"
                ValidationGroup="Refresh"
                ></asp:RequiredFieldValidator>
            <asp:CompareValidator
                id="cvDates"
                runat="server"
                ControlToValidate="txtTo"
                ControlToCompare="txtFrom"
                Display="Dynamic"
                ErrorMessage="To >= From"
                ValidationGroup="Refresh"
                Type="Date"
                Operator="GreaterThanEqual"
                ></asp:CompareValidator>
            &nbsp;
            <asp:LinkButton ID="btnRefresh" runat="server" Text="Refresh" OnClick="btnRefresh_Click" ValidationGroup="Refresh"></asp:LinkButton> |
            <asp:LinkButton ID="lbtnExport" runat="server" Text="Export to Excel" OnClick="btnExport_Clicked" ValidationGroup="Refresh" />
		</div>
	</asp:View>
	<asp:View ID="viewDetail" runat="server">
		<table cellspacing="10">
			<asp:Literal ID="literalDetails" runat="server"></asp:Literal>
		</table>
		<asp:LinkButton ID="lbShowList" runat="server" Text="Return to list" OnClick="lbShowList_Click"></asp:LinkButton> |
		<asp:LinkButton ID="btnDeleteSubmission" runat="server" Text="Delete" OnClientClick="return confirm('Delete this submission?');" OnCommand="btnDelete_Command" ></asp:LinkButton>
	</asp:View>
	<asp:View ID="viewNoResponses" runat="server">
		<em>No data to display</em>
	</asp:View>
</asp:MultiView>
