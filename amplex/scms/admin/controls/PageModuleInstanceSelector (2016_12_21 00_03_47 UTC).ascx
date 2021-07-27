<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="PageModuleInstanceSelector.ascx.cs" Inherits="scms.admin.PageModuleInstanceSelector" %>
<%@ Register Src="~/scms/admin/controls/PageSelector.ascx" TagName="pageSelector" TagPrefix="uc" %>

<uc:pageSelector 
    ID="pageSelectorShare" 
    runat="server"
/>
    
<asp:PlaceHolder ID="placeholderShareModule" runat="server" >
    <asp:Label ID="labelShareModule" runat="server" Text="module name: " AssociatedControlID="ddlShareModule"></asp:Label> 
    <asp:DropDownList ID="ddlShareModule" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlShareModule_SelectedIndexChanged"></asp:DropDownList>
</asp:PlaceHolder>