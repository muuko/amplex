<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ModuleSelector.ascx.cs" Inherits="scms.admin.ModuleSelector" %>
<%@ Register Src="~/scms/admin/controls/PageSelector.ascx" TagName="pageSelector" TagPrefix="uc" %>


<asp:Label ID="labelModuleSource" runat="server" AssociatedControlID="ddlModuleSource" Text="from"></asp:Label>
<asp:DropDownList 
    ID="ddlModuleSource" 
    runat="server"
    AutoPostBack="true"
    OnSelectedIndexChanged="ddlModuleSource_SelectedIndexChanged"
    >
    <asp:ListItem Text="Page" Value="page"></asp:ListItem>
    <asp:ListItem Text="Template" Value="template"></asp:ListItem>
</asp:DropDownList>
<asp:MultiView ID="multiViewSharingSource" runat="server" ActiveViewIndex="0">
    <asp:View ID="viewSharingDisabled" runat="server"></asp:View>
    <asp:View ID="viewShareSourcePage" runat="server">
        <uc:pageSelector 
            ID="pageSelectorShare" 
            runat="server"
             />
    </asp:View>
    <asp:View ID="viewShareSourceTemplate" runat="server">
        <asp:DropDownList ID="ddlTemplateShare" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlTemplateShare_SelectedIndexChanged">
        </asp:DropDownList>
    </asp:View>
</asp:MultiView>
<asp:PlaceHolder ID="placeholderShareModule" runat="server" >
    <asp:Label ID="labelShareModule" runat="server" Text="module name: " AssociatedControlID="ddlShareModule"></asp:Label> 
    <asp:DropDownList ID="ddlShareModule" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlShareModule_SelectedIndexChanged"></asp:DropDownList>
</asp:PlaceHolder>