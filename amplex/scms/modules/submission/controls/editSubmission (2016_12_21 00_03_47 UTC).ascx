<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="editSubmission.ascx.cs" Inherits="scms.modules.submission.controls.editSubmission" %>
<%@ Register Src="~/scms/admin/controls/StatusMessage.ascx" TagPrefix="uc" TagName="statusMessage" %>
<%@ Register Src="~/scms/admin/controls/SelectImage.ascx" TagPrefix="uc" TagName="selectImage" %>


<table cellspacing="10">
    <tr>
        <td><label>ID</label></td>
        <td><asp:Literal ID="literalId" runat="server"></asp:Literal></td>
    </tr>
    <tr>
        <td><label>Title</label></td>
        <td><asp:TextBox ID="txtTitle" runat="server" Width="400"></asp:TextBox></td>
    </tr>
    <tr>
        <td><label>Description</label></td>
        <td><asp:TextBox ID="txtDescription" runat="server" Width="400" TextMode="MultiLine" Columns="80" Rows="4"></asp:TextBox></td>
    </tr>
    <tr>
        <td><label>Link</label></td>
        <td><asp:TextBox ID="txtLink" runat="server" MaxLength="1024" Width="400"></asp:TextBox></td>
    </tr>    
    <tr>
        <td><label>Image</label></td>
        <td><uc:selectImage ID="selectImage" runat="server" /></td>
    </tr>
    <tr>
        <td><label>Video Url</label></td>
        <td><asp:TextBox ID="txtVideoUrl" runat="server" MaxLength="1024" Width="400"></asp:TextBox></td>
    </tr>
    <tr>
        <td><label>Submitter</label></td>
        <td><asp:TextBox ID="txtSubmitter" runat="server" Width="200"></asp:TextBox></td>
    </tr>
    <tr>
        <td><label>Document Credit</label></td>
        <td><asp:TextBox ID="txtDocumentCredit" runat="server" Width="200"></asp:TextBox></td>
    </tr>
    <tr>
        <td><label>Submitted On</label></td>
        <td>
            <asp:TextBox ID="txtSubmittedOn" runat="server" Width="85"></asp:TextBox>
            <asp:RangeValidator 
                ID="rvSubmittedOn" 
                runat="server"
                ControlToValidate="txtSubmittedOn"
                Type="Date"
                MinimumValue="1/1/1900"
                MaximumValue="1/1/2200"
                Display="Dynamic"
                ErrorMessage="?"></asp:RangeValidator>
            HH:MM <asp:TextBox ID="txtSubmittedOnTime" runat="server" Width="40"></asp:TextBox>
            <asp:RegularExpressionValidator
                id="rxSubmittedOnTime"
                runat="server"
                ControlToValidate="txtSubmittedOnTime"
                Display="Dynamic"
                ErrorMessage="?"
                ValidationExpression="^((([0-1][0-9])|(2[0-3]))(:[0-5][0-9])?)|(24(:00)?)?$"
                ></asp:RegularExpressionValidator>
            <asp:LinkButton ID="btnSubmittedOn" runat="server" OnClick="btnSubmittedOn_Click" Text="Now" />
                
        </td>
    </tr>
    <tr>
        <td><label>Approved On</label></td>
        <td>
            <asp:TextBox ID="txtApprovedOn" runat="server" Width="85"></asp:TextBox>
            <asp:RangeValidator 
                ID="rvApprovedOn" 
                runat="server"
                ControlToValidate="txtApprovedOn"
                Type="Date"
                MinimumValue="1/1/1900"
                MaximumValue="1/1/2200"
                Display="Dynamic"
                ErrorMessage="?"></asp:RangeValidator>            
            HH:MM <asp:TextBox ID="txtApprovedOnTime" runat="server" Width="40"></asp:TextBox>
            <asp:RegularExpressionValidator
                id="rxApprovedOn"
                runat="server"
                ControlToValidate="txtApprovedOnTime"
                Display="Dynamic"
                ErrorMessage="?"
                ValidationExpression="^((([0-1][0-9])|(2[0-3]))(:[0-5][0-9])?)|(24(:00)?)?$"
                ></asp:RegularExpressionValidator>            
            <asp:LinkButton ID="btnApprovedNow" runat="server" OnClick="btnApprovedNow_Click" Text="Now" />
            <asp:LinkButton ID="btnApprovedClear" runat="server" OnClick="btnApprovedClear_Click" Text="Clear" />
        </td>
    </tr>
    <tr>
        <td><label>Featured On</label></td>
        <td>
            <asp:TextBox ID="txtFeaturedOn" runat="server" Width="85"></asp:TextBox>
            <asp:RangeValidator 
                ID="rvFeaturedOn" 
                runat="server"
                ControlToValidate="txtFeaturedOn"
                Type="Date"
                MinimumValue="1/1/1900"
                MaximumValue="1/1/2200"
                Display="Dynamic"
                ErrorMessage="?"></asp:RangeValidator>                
            HH:MM <asp:TextBox ID="txtFeaturedOnTime" runat="server" Width="40"></asp:TextBox>
            <asp:RegularExpressionValidator
                id="rxFeaturedOn"
                runat="server"
                ControlToValidate="txtFeaturedOnTime"
                Display="Dynamic"
                ErrorMessage="?"
                ValidationExpression="^((([0-1][0-9])|(2[0-3]))(:[0-5][0-9])?)|(24(:00)?)?$"
                ></asp:RegularExpressionValidator>                
            <asp:LinkButton ID="btnFeaturedNow" runat="server" OnClick="btnFeaturedNow_Click" Text="Now" />
            <asp:LinkButton ID="btnFeaturedClear" runat="server" OnClick="btnFeaturedClear_Click" Text="Clear" />
        </td>
    </tr>
    <tr>
        <td valign="top"><label>Votes<br />[<asp:Literal ID="literalVotingMethod" runat="server"></asp:Literal>]</label></td>
        <td valign="top" >
            <div style="padding-bottom:10px;">vote: <asp:Literal ID="literalVote" runat="server"></asp:Literal>&nbsp;
            votes: <asp:Literal ID="literalVotes" runat="server"></asp:Literal></div>
            
            <div style="padding-bottom:10px;">up: <asp:TextBox ID="txtVotesUp" runat="server" Width="30"></asp:TextBox>
            down: <asp:TextBox ID="txtVotesDown" runat="server" Width="30"></asp:TextBox></div>
            <div style="padding-bottom:10px;">1: <asp:TextBox ID="txtVotes1" runat="server" Width="30"></asp:TextBox>
            2: <asp:TextBox ID="txtVotes2" runat="server" Width="30"></asp:TextBox>
            3: <asp:TextBox ID="txtVotes3" runat="server" Width="30"></asp:TextBox>
            4: <asp:TextBox ID="txtVotes4" runat="server" Width="30"></asp:TextBox>
            5: <asp:TextBox ID="txtVotes5" runat="server" Width="30"></asp:TextBox></div>
            
        </td>
    </tr>
    <tr>
        <td><label>Deleted</label></td>
        <td>
            <asp:CheckBox ID="checkDeleted" runat="server" />
        </td>
    </tr>
    
    
    
</table>

<div>
    <asp:LinkButton ID="btnSave" runat="server" Text="Save" OnClick="btnSave_Click" CausesValidation="true"></asp:LinkButton>
    <asp:LinkButton ID="btnCancel" runat="server" Text="Cancel" OnClick="btnCancel_Click" CausesValidation="false"></asp:LinkButton>
</div>
<uc:statusMessage ID="statusMessage" runat="server" />
