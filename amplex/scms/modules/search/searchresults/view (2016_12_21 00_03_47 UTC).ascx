<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="view.ascx.cs" Inherits="scms.modules.search.searchresults.view" %>

<div class="search-results">
    <div id="divSearchSummary" runat="server" class="search-results-summary">
    </div>
    
	<asp:Repeater 
	    ID="rptSearchResults" 
	    runat="server"
	    OnItemDataBound="rptSearchResults_OnItemDataBound" >
	    <ItemTemplate>
	        
	        <div id="divItem" runat="server" class="search-item">

	            
	            <div id="divBody" runat="server" class="search-item-body">
	                <div id="divThumbnail" runat="server" class="search-item-thumbnail">
	                    <a id="anchorThumbnail" runat="server"><img id="imgThumbnail" runat="server" /></a>
	                </div>
									<div id="divTitle" runat="server" class="search-item-title">
		                <a id="anchorTitle" runat="server"></a>
									</div>
									<div id="divUrl" runat="server" class="search-item-url"></div>	                
		                <div id="divSummary" runat="server" class="search-item-summary"></div>
								</div>
	            
	            
	            
	            <div id="divReadMore" runat="server" class="search-item-readmore">
	                <a id="anchorReadMore" runat="server" href="#">Read More</a>
	            </div>
	        </div>
	        
	    </ItemTemplate>
	</asp:Repeater>
	
	<div id="divResultsPager"  runat="server" class="search-results-pager">
        <div id="divPagePrev" runat="server" class="search-results-page search-results-page-inactive"><a id="anchorPrevious" runat="server">Prev</a></div>
	    <asp:Repeater ID="rptPager" runat="server" OnItemDataBound="rptPager_OnItemDataBound">
	        <ItemTemplate>
	            <div id="divPageInactive" runat="server" class="search-results-page search-results-page-inactive"><a id="anchorPage" runat="server"></a></div>
	            <div id="divPageActive" runat="server" class="search-results-page search-results-page-active"></div>
	        </ItemTemplate>
	    </asp:Repeater>
	    <div id="divPageNext" runat="server" class="search-results-page search-results-page-inactive"><a id="anchorNext" runat="server">Next</a></div>
	</div>
</div>