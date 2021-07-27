<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="footer.ascx.cs" Inherits="amplex.sites.amplex._controls.footer" %>


<div class="footer">
	<div class="footer-main">

    	<table width="940" border="0">
          <tr>
            <td valign="top" class="footer-column-left" >
                <asp:PlaceHolder ID="footer_left_placeholder" runat="server"></asp:PlaceHolder>
                <%-- 
                <ul>
                    <li>
                        <h3><a href="/about">About</a></h3>
                        <p><a href="/about/overview">Overview</a></p>
                        <p><a href="/about/our-staff">Our Staff</a></p>
                        <p><a href="/about/locations">Locations</a></p>
                    </li>
                    <li>
                        <h3><a href="/products">Products</a></h3>
                        <p><a href="#">Sub Item Nav</a></p>
                        <p><a href="#">Sub Item Nav</a></p>
                        <p><a href="#">Sub Item Nav</a></p>
                        <p><a href="#">Sub Item Nav</a></p>
                    </li>
                    <li>
                        <h3><a href="/contact">Contact</a></h3>
                        <p><a href="/contact/contact-us">Contact Us</a></p>
                        <p><a href="/contact/become-a-customer">Become a Customer</a></p>
                    </li>
                </ul> 
                --%>
            </td>
            <td valign="top">
                <asp:PlaceHolder ID="footer_right_placeholder" runat="server"></asp:PlaceHolder>
                <%--
                <p align="right"><a href="http://www.facebook.com"><img src="/sites/amplex/images/ico-facebook-large.png" width="32" height="32" /></a><a href="http://www.twitter.com"><img src="/sites/amplex/images/ico-twitter-large.png" width="32" height="32" /></a><a href="http://www.youtube.com/"><img src="/sites/amplex/images/ico-youtube-large.png" width="32" height="32" /></a><br /><br /></p>
                <p align="right">Copyright 2012. Amplex.</p>
                <p align="right"><a href="/about/privacy-policy">Privacy Policy</a>  ::  <a href="/about/disclaimer">Disclaimer</a></p>
                <h3 align="right">(800) 565-2928</h3>
                --%>
            </td>
          </tr>
        </table>

    </div>
</div>
