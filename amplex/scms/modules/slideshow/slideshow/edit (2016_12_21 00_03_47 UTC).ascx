<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="edit.ascx.cs" Inherits="scms.modules.slideshow.edit"  %>
<%@ Register Src="~/scms/admin/controls/StatusMessage.ascx" TagPrefix="uc" TagName="statusMessage" %>
<%@ Register Src="~/scms/modules/slideshow/slideshow/slideshowSettings.ascx" TagPrefix="uc" TagName="slideshowSettings" %>
<%@ Register Src="~/scms/modules/slideshow/slideshow/slides.ascx" TagPrefix="uc" TagName="slides" %>
<asp:Menu
    id="menuTabs"
    runat="server"
    Orientation="Horizontal"
	OnMenuItemClick="menuTabs_Click"
	StaticMenuItemStyle-CssClass="tabbed-menu"
	StaticHoverStyle-CssClass="tabbed-menu-hover"
	StaticSelectedStyle-CssClass="tabbed-menu-selected"
>
    <Items>
        <asp:MenuItem Text="General" Value="general"></asp:MenuItem>
        <asp:MenuItem Text="Slides" Value="slides" Selected="true"></asp:MenuItem>
    </Items>
</asp:Menu>

<asp:MultiView ID="multiView" runat="server">
    
    <asp:View ID="viewGeneral" runat="server">
        <uc:slideshowSettings 
            ID="slideshowSettings" 
            runat="server" />
    </asp:View>
    
    <asp:View ID="viewSlides" runat="server">
        <uc:slides ID="slides" runat="server" />
    </asp:View>
</asp:MultiView>

<uc:statusMessage ID="statusMessage" runat="server" />