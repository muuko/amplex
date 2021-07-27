<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="submission.ascx.cs" Inherits="scms.modules.submission.controls.submission" %>
<%@ Register Src="~/scms/modules/submission/controls/voting.ascx" TagPrefix="uc" TagName="voting" %>
<style type="text/css">


</style>
<div class="submission" >
    <div class="submission-admin" id="divAdmin" runat="server">
        <asp:LinkButton ID="btnEdit" runat="server" Text="Edit" OnClick="btnEdit_Click"></asp:LinkButton>
        <asp:CheckBox ID="checkApproved" runat="server" Text="Approved" Enabled="false" /><asp:CheckBox ID="checkFeatured" runat="server" Text="Featured" Enabled="false" /><asp:CheckBox ID="checkDeleted" runat="server" Text="Deleted" Enabled="false" />
    </div>
    <div class="submission-title" id="divTitle" runat="server" ><h2 id="titleHeading" runat="server"><asp:HyperLink ID="hlHeading" runat="server"></asp:HyperLink></h2></div>
    
    <div class="submission-subtitle" id="divSubTitle" runat="server">
        <div class="submission-voting" id="divVoting" runat="server">
            <uc:voting ID="voting" runat="server" />
        </div>
        <div class="submission-postdate" id="divPostDate" runat="server"></div>
    </div>
    
    <div class="submission-image" id="divImage" runat="server">
        <asp:Image ID="image" runat="server" />
    </div>
    
    <div class="submission-video" id="divVideo" runat="server" visible="false">
    </div>
    
    <div class="submission-description" id="divDescription" runat="server">
    </div>
    
    <div class="submission-source" id="divSource" runat="server">
        <div class="submission-submitter" id="divSubmitter" runat="server">
            <label>Submitter:</label> <span id="spanSubmitter" runat="server"></span>
        </div>
        <div class="submission-credit" id="divCredit" runat="server">
            <label id="lableDocumentCredit" runat="server">Credit:</label> <span id="spanDocumentCredit" runat="server"></span>
        </div>
    </div>
    <div class="submission-link" id="divSubmissionLink" runat="server"></div>
    
    <%-- comments here --%>
    <%-- sharing here --%>
</div>
