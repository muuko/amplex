<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="voting.ascx.cs" Inherits="scms.modules.submission.controls.voting" %>

<%-- 

<script src="/scms/modules/submission/public/js/voting.js" type="text/javascript">
</script>


<script type="text/javascript">

    $(document).ready(function() {
        $('#fivebuttons').voting(
            {
                vote: 1.5,
                votesText: '125 Votes'
        });
    });

</script>
--%>

<div class="submission-voting">
    <asp:MultiView ID="multiview" runat="server">

<!-- http://www.istockphoto.com/stock-illustration-4796726-thumbs-up-icon.php -->

        <asp:View ID="viewUpDown" runat="server">
            <div class="submission-voting-updownbutton">
                <asp:LinkButton ID="btnUp" runat="server" OnClick="btnUp_Click">
                    <img class="ov" id="imgUp" runat="server" src="/scms/modules/submission/public/images/up.gif"  />
                </asp:LinkButton>
                <asp:Literal ID="literalVotesUp" runat="server"></asp:Literal>
            </div>
            <div class="submission-voting-updownbutton">
                <asp:LinkButton ID="btnDown" runat="server" OnClick="btnDown_Click">
                    <img class="ov" id="imgDown" runat="server" src="/scms/modules/submission/public/images/down.gif"  />
                </asp:LinkButton>
                <asp:Literal ID="literalVotesDown" runat="server"></asp:Literal>
            </div>
            <div class="submission-voting-text" id="divVotingText" runat="server" ></div>
        </asp:View>
        
        <asp:View ID="viewFiveUp" runat="server">
            <div class="submission-voting-fivebuttons" id="fivebuttons" runat="server">
                <div class="submission-voting-fiveuputton">
                    <asp:LinkButton ID="btnUp1" runat="server" OnClick="btnUpMultiple_Click">
                        <img id="imgUp1" runat="server" src="/scms/modules/submission/public/images/up.gif"  />
                    </asp:LinkButton>
                </div>
                <div class="submission-voting-fiveuputton">
                    <asp:LinkButton ID="btnUp2" runat="server" OnClick="btnUpMultiple_Click">
                        <img id="imgUp2" runat="server" src="/scms/modules/submission/public/images/up.gif"  />
                    </asp:LinkButton>
                </div>
                <div class="submission-voting-fiveuputton">
                    <asp:LinkButton ID="btnUp3" runat="server" OnClick="btnUpMultiple_Click">
                        <img id="imgUp3" runat="server" src="/scms/modules/submission/public/images/up.gif"  />
                    </asp:LinkButton>
                </div>
                <div class="submission-voting-fiveuputton">
                    <asp:LinkButton ID="btnUp4" runat="server" OnClick="btnUpMultiple_Click">
                        <img id="imgUp4" runat="server" src="/scms/modules/submission/public/images/up.gif"  />
                    </asp:LinkButton>
                </div>
                <div class="submission-voting-fiveuputton">
                    <asp:LinkButton ID="btnUp5" runat="server" OnClick="btnUpMultiple_Click">
                        <img id="imgUp5" runat="server" src="/scms/modules/submission/public/images/up.gif"  />
                    </asp:LinkButton>
                </div>
            </div>
                
            <div class="submission-voting-current-vote" id="divCurrentVote" ></div>
            <div class="submission-voting-text" id="divFiveUpVotingText" runat="server" ></div>
        </asp:View>
        
    </asp:MultiView>
</div>