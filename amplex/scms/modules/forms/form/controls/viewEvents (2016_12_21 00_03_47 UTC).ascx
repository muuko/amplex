<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="viewEvents.ascx.cs" Inherits="scms.modules.forms.form.controls.viewEvents" %>
<%@ Register Src="~/scms/admin/controls/StatusMessage.ascx" TagPrefix="uc" TagName="statusMessage" %>
<%@ Register Src="~/scms/modules/submission/controls/formsettings.ascx" TagPrefix="uc" TagName="formSettings" %>
<div class="pagePanel">
    <fieldset >
        <legend>Events</legend>
        <asp:ListView ID="lvEvents" runat="server" OnItemDataBound="lvEvents_ItemDataBound">
            <LayoutTemplate>
                <table>
                    <tr>
                        <th>Event</th>
                        <th>Action</th>
                    </tr>
                    <asp:PlaceHolder ID="ItemPlaceHolder" runat="server"></asp:PlaceHolder>
                </table>
            </LayoutTemplate>
            <EmptyDataTemplate>
                <em>Nothing to display</em>
            </EmptyDataTemplate>
            <ItemTemplate>
                <tr>
                    <td><asp:LinkButton ID="btnEdit" runat="server" Text='<%# Eval("Name") %>' CommandName="CustomEdit" CommandArgument='<%# Eval("id") %>' OnCommand="Edit" />
                    </td>
                    <td>
        				<asp:ImageButton ID="btnUp" runat="server" ImageUrl="/scms/client/images/arrow_top.gif" OnCommand="Move" CommandName="Up" CommandArgument='<%# Eval("id") %>'  />
				        <asp:ImageButton ID="btnDown" runat="server" ImageUrl="/scms/client/images/arrow_down.gif" OnCommand="Move" CommandName="Down" CommandArgument='<%# Eval("id") %>'  />
				        <asp:ImageButton ID="btnDelete" runat="server" ImageUrl="/scms/client/images/action_delete.gif" OnCommand="Delete" CommandName="CustomDelete" CommandArgument='<%# Eval("id") %>' OnClientClick="javascript: return confirm('Delete this event?');" />
				    <td>
                </tr>
            </ItemTemplate>
        </asp:ListView>
    </fieldset>
</div>


<asp:Panel ID="panelNewEvent" DefaultButton="btnNewEvent" runat="server" style="padding: 5px 5px 25px 5px;background-color:#eef;clear:both;">
    <div style="display:inline;float:left;margin-left:8px;padding-top:0px;padding-right:5px;"><span class="label">New Event of type</span> &nbsp;&nbsp; <asp:DropDownList ID="ddlEvents" runat="server"></asp:DropDownList></div>
    <div style="display:inline;float:left;margin-left:8px;padding-top:2px;"><asp:LinkButton ID="btnNewEvent" runat="server" OnClick="btnNewEvent_Click" Text="Create"></asp:LinkButton></div>
</asp:Panel>
<br />

<asp:Panel ID="panelEditEvent" runat="server" CssClass="pagePanel" >
	<fieldset>
		<legend>Edit Event</legend>
		<asp:PlaceHolder ID="placeholderEventHandlerSettings" runat="server"></asp:PlaceHolder>
	</fieldset>
</asp:Panel>

<uc:statusMessage ID="statusMessage" runat="server" />