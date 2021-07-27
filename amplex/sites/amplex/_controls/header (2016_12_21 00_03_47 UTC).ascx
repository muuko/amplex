<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="header.ascx.cs" Inherits="amplex.sites.amplex._masters.controls.header" %>
<%@ Register Src="~/sites/amplex/_controls/mainnav.ascx" TagName="mainnav" TagPrefix="uc" %>

<div class="header-upper">
	<asp:PlaceHolder ID="search" runat="server"></asp:PlaceHolder><p align="right"><span class="number">(800) 565-2928</span> <a href="http://www.facebook.com/AmplexPlants"><img alt="Amplex on Facebook" src="/sites/amplex/images/ico-facebook.jpg" width="29" height="23" /></a><a href="https://twitter.com/amplexplants"><img alt="Amplex on Twitter" src="/sites/amplex/images/ico-twitter.jpg" width="31" height="23" /></a><a href="http://www.youtube.com/amplexplants"><img alt="Amplex on YouTube" src="/sites/amplex/images/ico-youtube.jpg" width="29" height="23" /></a><a href="/contact/become-a-customer">Sign Up</a><a href="/login">Login</a><a href="/contact">Contact Us</a><a href="/about/locations">Locations</a></p>
</div>
<div class="header-lower">
	<div class="logo left">
		<span><a href="/"><img src="/sites/amplex/images/logo2.jpg" alt="Amplex" /></a></span>
	</div>
	<uc:mainnav id="mainnav" runat="server"></uc:mainnav>
</div>
