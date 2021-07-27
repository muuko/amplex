<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="view.ascx.cs" Inherits="scms.modules.parts.catalog.view" %>
<%@ Register Src="~/scms/admin/controls/SelectImage.ascx" TagPrefix="uc" TagName="selectImage" %>
<%@ Register Src="~/scms/controls/pager.ascx" TagPrefix="uc" TagName="pager"  %>
<style type="text/css">

	.catalog-search p, td, span, label, strong, a
	{
		font-size: 14px;
		line-height: 16px;
	}

	#catalog-search
	{
		margin-top:20px;
		position:relative;
	}
	
	#catalog-search-for
	{
		width:650px;
	}
	
	#catalog-search-for-label
	{
		padding-top:2px;
		display:block;
		width:120px;
		text-align:right;
	}
	
	#catalog-search-for-value
	{
		position:absolute;
		top:0;
		left:130px;
	}
	
	#catalog-search-in
	{
		width:650px;
	}
	#catalog-search-in-label
	{
		position:absolute;
		top:34px;
		width:120px;
		text-align:right;
	}
	#catalog-search-in-value
	{
		position:absolute;
		top:33px;
		left:130px;
	}
	
	#catalog-search-by-alpha-label
	{
		position:absolute;
		top:70px;
		width:120px;
		text-align:right;
	}
	

	.catalog-search-button
	{
		position:absolute;
		left: 460px;
		height:40px;
		top: -10px;
		background-color: #ecffb2;
		color: #7ba600;
		
	}
	
	#catalog-search-results
	{
		margin-top:90px;
	}
	
	.catalog-select-alphabet
	{
		position:absolute;
		top:70px;
		left:130px;
	}
	
	.catalog-select-alphabet a
	{
		display:block;
		float:left;
		width:16px;
		text-align:center;
	}
	
	.catalog-select-alphabet a:link, .catalog-select-alphabet a:visited
	{
		text-decoration:none;
		font-weight:normal;
	}
	
	.catalog-select-alphabet a:hover, .catalog-select-alphabet a:active, .catalog-select-alphabet a.active
	{
		text-decoration:underline;
		font-weight:bold;
	}
	
	#catalog-search-results-body
	{
		
	}
	
	.catalog-search-results-table
	{
		font-family: "Lucida Sans Unicode", "Lucida Grande", Sans-Serif;
		font-size: 12px;
		width: 100%	;
		text-align: left;
		border-collapse: collapse;
	}
	
	.catalog-search-results-table th, .catalog-search-results-table th a:link, .catalog-search-results-table th a:visited
	{
		font-size: 13px;
		font-weight: normal;
		line-height:18px;
		color: #040;
		background: #96bf3f;
	}
	
	.catalog-search-results-table th
	{
		
		padding: 8px;
		
		border-top: 4px solid #798c22;
		border-bottom: 1px solid #fff;
		
	}
	
	.catalog-search-results-table td
	{
		padding: 8px;
		background: #ecffb2; 
		border-bottom: 1px solid #fff;
		color: #040;
		border-top: 1px solid transparent;
	}
	
	.catalog-search-results-table td.catalog-part-image
	{
		text-align:center;
		width: 130px;
	}
	
	.catalog-search-results-table td a:link, .catalog-search-results-table td a:visited
	{
		text-decoration: none;
	}
	
	.catalog-search-results-table tr:hover td
	{
		background: #b6df5f;
		color: #040;
		cursor:pointer;
	}
	
	td.catalog-part-image img 
	{ 
		border: solid 1px #798c22; 
	}
	
	td.catalog-part-price, th.catalog-part-price
	{
		text-align:right;
		padding-right:25px;
	}
	
	
	#catalog-search-results-pager
	{
		clear:both;
		display:block;
		height:auto;
		overflow:auto;
	}
	
	#catalog-search-results-pager .pager .pager-page
	{
		border: 1px solid #b6df5f;
	}
	
	
	#catalog-search-results-pager .pager .pager-page-inactive, 
	#catalog-search-results-pager .pager .pager-page-inactive a:link,
	#catalog-search-results-pager .pager .pager-page-inactive a:visited
	{
		background-color: #ecffb2;
		color:#b6df5f;
		text-decoration:none;
	}
	
	#catalog-search-results-pager .pager .pager-page-active
	{
		background-color: #b6df5f;
		color:#ecffb2;
		text-decoration:none;
	}
	
	#catalog-search-results-pager .pager .pager-page-inactive a:hover,
	#catalog-search-results-pager .pager .pager-page-inactive a:active
	{
		background-color: #96bf3f;
		color:#fcffd2;
		text-decoration:none;
	}
	
</style>
<script type="text/javascript">
	(function($) {
	
	
	

		$(document).ready(function() {

			$('.catalog-search-input').focus().select();

			$('.catalog-search-results-table td').click(function() {

				var $tr = $(this).closest('tr');
				var $a = $tr.find('a');
				var href = $a.attr('href');
				if (href != null) {
					document.location.href = href;
				}

			});

		});

	})(jQuery);
</script>


<div id="catalog-search">
	<asp:Panel DefaultButton="btnWhatever" runat="server">
		<strong id="catalog-search-for-label">Search for:  </strong>
		<div id="catalog-search-for-value">
			<asp:TextBox CssClass="catalog-search-input" ID="txtSearch" runat="server" Width="300"></asp:TextBox><br />
		</div>
		<strong id="catalog-search-in-label">in:</strong>
		<div id="catalog-search-in-value">
			<asp:CheckBox ID="checkCommonName" runat="server" Text="Common Name" />
			<asp:CheckBox ID="checkBotanicalName" runat="server" Text="Botanical Name" />
			<asp:CheckBox ID="checkDescription" runat="server" Text="Description" />
		</div>
		<asp:Button class="catalog-search-button"  ID="btnWhatever" runat="server" OnClick="btnSearch_Click" Text="Search Plant Catalog"   />
		<strong id="catalog-search-by-alpha-label">or Select Letter:</strong>
		<div class="catalog-select-alphabet" id="divCatalogSelectAlphabet" runat="server">
				<a href="?sl=A">A</a>
				<a href="?sl=B">B</a>
				<a href="?sl=A" class="active">C</a>
				<a href="?sl=B">D</a>
				<a href="?sl=A">E</a>
				<a href="?sl=B">F</a>
		</div>
	</asp:Panel>
	<div id="catalog-search-results">
		
		<div id="catalog-search-results-body">
			<asp:PlaceHolder ID="placeholderTableResults" runat="server">
			<table class="catalog-search-results-table">
				<tr>
					<th>&nbsp;</th>
					<th class="catalog-part-common">Common Name</th>
					<th class="catalog-part-botanical">Botanical Name</th>
					<th class="catalog-part-price">
						<asp:Literal ID="literalPriceHeader" runat="server">Price</asp:Literal>
						<a id="anchorViewPricesHeader" runat="server" visible="false" >Login<br />To View Prices</a>
					</th>
				</tr>
				
				<asp:ListView 
					ID="lvResults" 
					runat="server"
					OnItemDataBound="lvResults_ItemDataBound"
				> 
					<LayoutTemplate><asp:PlaceHolder ID="itemPlaceholder" runat="server"></asp:PlaceHolder></LayoutTemplate>
					<ItemTemplate>
						<tr id="trCatPart" runat="server">
							<td class="catalog-part-image"><img id="imgThumbnail" src="" runat="server" /></td>
							<td class="catalog-part-common"><a id="anchorCommon" runat="server" ><%# Eval("sage_Description1")%></a></td>
							<td class="catalog-part-botanical"><a id="anchorBotanical" runat="server"><%# Eval("sage_Description2")%></a></td>
							<td class="catalog-part-price"><span id="spanPrice" runat="server"></span></td>
						</tr>
					</ItemTemplate>
					
					<EmptyDataTemplate>
						<tr><td colspan="4"><em>Search returned no results.</em></td></tr>
					</EmptyDataTemplate>
					
				</asp:ListView>
				
			</table>
			</asp:PlaceHolder>
		
		</div>
		<div id="catalog-search-results-pager">
			<uc:pager ID="pager" runat="server" />
		</div>
	<%--
		<asp:CheckBox ID="checkShowImages" runat="server" Text="Show Images" />
--%>		
	</div>
	
</div>

