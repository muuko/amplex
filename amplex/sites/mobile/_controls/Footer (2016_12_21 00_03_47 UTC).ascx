<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="footer.ascx.cs" Inherits="merlinlawgroup.sites.mobile._controls.footer" %>
<!--
<div id="footer-locations-bg">
	<div id="footer-locations-top" >
		<asp:PlaceHolder ID="footer_locations" runat="server"></asp:PlaceHolder>
	</div>
	<div id="footer-locations-bottom">
		<a id="footer-phone" href="tel:877-449-4700"></a>
	</div>
</div>
-->
<div data-role="navbar">
		<ul>
			<li><a rel="external" data-ajax="false" href="/login?returnurl=%2fadmin">Login</a></li>
			<li><a id="anchorFullSite2" runat="server" href="/">Full Site</a></li>
			<li><a data-ajax="false" href="/contact-us">Contact Us</a></li>
		</ul>
</div>
<div id="footer">
	<div id="footer-bg">
		<a id="footer-logo" title="Amplex Mobile Home" data-ajax="false" href="/"><img src="/sites/mobile/images/core/blank.gif" alt="Amplex Mobile Home" /></a>
		<ul id="footer-locations">
			<li><a href="/locations/clearwater-fl">Clearwater, FL</a></li>
			<li><a href="/locations/plant-city-fl">Plant City, FL</a></li>
		</ul>
		<a id="footer-phone" href="tel:800-565-2928">800.565.2928</a>
	</div>
</div>
