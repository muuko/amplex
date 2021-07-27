<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="view.ascx.cs" Inherits="scms.modules.submission.submission.view" %>
<%@ Register Src="~/scms/modules/submission/controls/submission.ascx" TagPrefix="uc" TagName="submission" %>

<asp:MultiView ID="mv" runat="server" ActiveViewIndex="0">
    <asp:View ID="viewNone" runat="server">
    </asp:View>
    
    <asp:View ID="viewSingleSubmission" runat="server">
        <uc:submission 
            ID="submissionSingle" 
            runat="server" />
    </asp:View>
    
    <asp:View ID="viewMultiple" runat="server">
        <asp:ListView
            id="lvSubmissions"
            runat="server"
            OnItemDataBound="lvSubmissions_ItemDataBound"
            >
            <LayoutTemplate>
                <asp:PlaceHolder ID="itemPlaceHolder" runat="server" ></asp:PlaceHolder>
            </LayoutTemplate>
            <ItemTemplate>
                <uc:submission
                    id="submission"
                    runat="server"
                    SubmissionModule='<%# SubmissionModule %>'
                    Submission='<%# Container.DataItem %>'
                    />
            </ItemTemplate>
        </asp:ListView>
        <asp:PlaceHolder ID="placeholderPager" runat="server">
            <div class="pager">
                <div class="pager-page pager-page-inactive">
                    <a id="anchorPagerPrev" runat="server">Prev</a>
                </div>
                
                <asp:Literal ID="literalPagerPages" runat="server"></asp:Literal>
                
                <div class="pager-page pager-page-inactive">
                    <a id="anchorPagerNext" runat="server" class="submission-pager-next">Next</a>
                </div>
            </div>
        </asp:PlaceHolder>
    </asp:View>
    
</asp:MultiView>