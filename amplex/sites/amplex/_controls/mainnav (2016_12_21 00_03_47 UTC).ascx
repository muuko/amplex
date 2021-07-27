<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="mainnav.ascx.cs" Inherits="amplex.sites.amplex._masters.controls.mainnav" %>
<%@ Register Src="~/sites/amplex/_controls/MainNavItem.ascx" TagPrefix="uc" TagName="MainNavItem" %>

<div id="main-nav-wrap">
    <div id="main-nav">
        <div class="xmenu" id="nav_about" >
            <a href="/about"><img class="ov" src="/sites/amplex/images/core/nav-about.jpg" /></a>
            <uc:MainNavItem
                id="menuAbout"
                runat="server"
                Path="/about"
                ExpandAll="true"
                MaxDepth="0" />
        </div>
        <div class="xmenu" id="nav_specials" >
            <a href="/featured-items" ><image class="ov" src="/sites/amplex/images/core/nav-specials.jpg"></image></a>
            <uc:MainNavItem
                id="menuFeatured"
                runat="server"
                Path="/featured-items"
                ExpandAll="true"
                MaxDepth="0"
                MaxChildrenPerNode="15"
                 />
        </div>

        <div class="xmenu" id="nav_products">
            <a href="/products"><img class="ov" src="/sites/amplex/images/core/nav-products.jpg" /></a>
            <uc:MainNavItem
                id="menuProducts"
                runat="server"
                Path="/products"
                ExpandAll="true"
                MaxDepth="0" />
            
        </div>
        <div class="xmenu" id="nav_news">
            <a href="/news"><img class="ov" src="/sites/amplex/images/core/nav-news.jpg" /></a>
                <uc:MainNavItem
                    id="menuNews"
                    runat="server"
                    Path="/news"
                    ExpandAll="true"
                    MaxDepth="0"
                    />
                
            </div>
            
        <div class="xmenu" id="nav_contact">
            <a href="/contact"><img class="ov" src="/sites/amplex/images/core/nav-contact.jpg" /></a>
                <uc:MainNavItem
                    id="menuContact"
                    runat="server"
                    Path="/contact"
                    ExpandAll="true"
                    MaxDepth="0"
                    />
        </div>
    </div>
</div>