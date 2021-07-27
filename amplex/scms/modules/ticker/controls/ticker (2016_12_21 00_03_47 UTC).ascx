<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ticker.ascx.cs" Inherits="scms.modules.ticker.controls.ticker" %>





<div id="ticker-wrap">
	<div id="ticker">
		<div id="ticker-left"> </div>
		<div id="ticker-content"> 
			<div id="ticker-top">
				<div class="ticker-contents">
					<asp:PlaceHolder id="ticker_top" runat="server"></asp:PlaceHolder>
				</div>
			</div>
			
			<div id="ticker-bottom">
				<div class="ticker-contents">
					<asp:ListView 
						ID="lvMarquee" 
						runat="server"
						OnItemDataBound="lvMarquee_ItemDataBound"
						>
						<LayoutTemplate>
							<ul><asp:PlaceHolder ID="itemPlaceholder" runat="server"></asp:PlaceHolder></ul>
						</LayoutTemplate>
						<ItemTemplate>
							<li>
								<span id="spanTickerLabel" runat="server" class="ticker-label dot"></span>
								<span id="spanLink" runat="server" class="ticker-link">
									<a id="anchorLink" runat="server"></a>
								</span>
							</li>
						
						</ItemTemplate>
						
						</asp:ListView>
					<%-- <ul>
						<li><span class="ticker-label dot">AUG 01-03</span><span class="ticker-link"><a href="/contact/contact-us">EXHIBITING AT MANTS</a></span></li>
						<li><span class="ticker-label dot">SEP 03</span><span class="ticker-link"><a href="/contact/contact-us">CLOSED FOR LABOR DAY</a></span></li>
						<li><span class="ticker-label dot">JAN 9-11</span><span class="ticker-link"><a href="/contact/contact-us">EXHIBITING AT MANTS</a></span></li>
					</ul> --%>
				</div>
			</div>
			
		</div>
		<div id="ticker-right"> </div>
	</div>
</div>
