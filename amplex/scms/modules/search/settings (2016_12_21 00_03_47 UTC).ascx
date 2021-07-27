<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="settings.ascx.cs" Inherits="scms.modules.search.settings" %>
<%@ Register Src="~/scms/admin/controls/PageSelector.ascx" TagPrefix="uc" TagName="pageSelector" %>
<%@ Register Src="~/scms/admin/controls/StatusMessage.ascx" TagPrefix="uc" TagName="statusMessage" %>
Search Result Page:&nbsp; <uc:pageSelector ID="pageSelector" runat="server" />
<div style="margin-top:20px;">
<asp:LinkButton ID="btnSave" runat="server" OnClick="btnSave_Click" Text="Save"></asp:LinkButton>&nbsp;
<asp:LinkButton ID="btnRebuildIndex" runat="server" OnClick="btnRebuildIndex_Click" Text="Rebuild Index"></asp:LinkButton>
<uc:statusMessage ID="statusMessage" runat="server" />
</div>