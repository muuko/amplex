<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Pager.ascx.cs" Inherits="scms.controls.Pager" %>
<asp:PlaceHolder ID="placeholderPager" runat="server">
    <div class="pager">
        <div id="divPrevious" runat="server" class="pager-page pager-page-inactive">
            <a id="anchorPagerPrev" runat="server" class="pager-prev">Prev</a>
        </div>
        
        <asp:Literal ID="literalPagerPages" runat="server"></asp:Literal>
        
        <div id="divNext" runat="server" class="pager-page pager-page-inactive">
            <a id="anchorPagerNext" runat="server" class="pager-next">Next</a>
        </div>
    </div>
</asp:PlaceHolder>