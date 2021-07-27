<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="edit.ascx.cs" Inherits="scms.modules.submission.submission.edit" %>
<%@ Register Src="~/scms/admin/controls/SelectImage.ascx" TagPrefix="uc" TagName="selectImage"  %>
<%@ Register Src="~/scms/admin/controls/StatusMessage.ascx" TagPrefix="uc" TagName="statusMessage" %>
<%@ Register Src="~/scms/modules/submission/controls/editSubmission.ascx" TagPrefix="uc" TagName="editSubmission" %>

<asp:MultiView ID="multiView" runat="server" ActiveViewIndex="0">
    <asp:View ID="viewSettings" runat="server">

        <div class="pagePanel">
            <fieldset>
                <legend>General Settings</legend>
                <table>
                    <tr><td style="width:200px"><label>Auto Approve Submissions</label></td><td><asp:CheckBox id="checkAutoApproveSubmissions" runat="server" /></td></tr>
                    <tr><td><label>Auto Feature Submissions</label></td><td><asp:CheckBox ID="checkAutoFeatureSubmissions" runat="server" /></td></tr>
                </table>
            </fieldset>
        </div>

        <div class="pagePanel">
            <fieldset>
                <legend>Comments</legend>
                <table>
                    <tr>
                        <td style="width:200px"><label>Enabled</label></td>
                        <td><asp:CheckBox ID="checkCommentsEnabled" runat="server" AutoPostBack="true" OnCheckedChanged="checkCommentsEnabled_CheckedChanged" /></td>
                    </tr>
                    <tr>
                        <td><asp:Label ID="labelCommentsAutoApprove" runat="server">Auto Approve</asp:Label></td>
                        <td><asp:CheckBox ID="checkCommentsAutoApprove" runat="server" /></td>
                    </tr>
                    <tr>
                        <td><asp:Label ID="labelCommentsAuthRequired" runat="server">Authentication Required</asp:Label></td>
                        <td><asp:CheckBox ID="checkCommentsAuthRequired" runat="server" /></td>
                    </tr>
                </table>
            </fieldset>
        </div>

        <div class="pagePanel">
            <fieldset>
                <legend>Voting</legend>
                <table>
                    <tr>
                        <td style="width:200px"><label>Enabled</label></td>
                        <td><asp:CheckBox ID="checkVotingEnabled" runat="server" AutoPostBack="true" OnCheckedChanged="checkVotingEnabled_CheckedChanged" /></td>
                    </tr>
                    <tr>
                        <td><asp:Label ID="labelVotingAuthRequired" runat="server">Authentication Required</asp:Label></td>
                        <td><asp:CheckBox ID="checkVotingAuthRequired" runat="server" /></td>
                    </tr>
                    <tr>
                        <td><asp:Label ID="labelVotingMethod" runat="server">Method</asp:Label></td>
                        <td><asp:DropDownList ID="ddlVotingMethod" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlVotingMethod_SelectedIndexChanged">
                            <asp:ListItem Text="Up/Down" Value="updown"></asp:ListItem>
                            <asp:ListItem Text="Five Up" Value="fiveup"></asp:ListItem>
                        </asp:DropDownList></td>
                    </tr>
                    <tr>
                        <td><asp:Label ID="labelVotingUpImageUrl" runat="server">Up/Down - Up Image Url</asp:Label></td>
                        <td><uc:selectImage ID="imageVotingUrlUp" runat="server" /></td>
                    </tr>
                    <tr>
                        <td><asp:Label ID="labelVotingDownImageUrl" runat="server">Up/Down - Down Image Url</asp:Label></td>
                        <td><uc:selectImage ID="imageVotingUrlDown" runat="server" /></td>
                    </tr>
                    <tr>
                        <td><asp:Label ID="labelVotingUpText" runat="server">Up/Down - Up Text</asp:Label></td>
                        <td><asp:TextBox ID="txtVotingUpText" runat="server" Width="50"></asp:TextBox></td>
                    </tr>
                    <tr>
                        <td><asp:Label ID="labelVotingDownText" runat="server">Up/Down - Down Text</asp:Label></td>
                        <td><asp:TextBox ID="txtVotingDownText" runat="server" Width="50"></asp:TextBox></td>
                    </tr>
                    <tr>
                        <td><asp:Label ID="labelVotingActiveImageUrl" runat="server">Five Up - Active Image Url</asp:Label></td>
                        <td><uc:selectImage ID="imageVotingActiveImageUrl" runat="server" /></td>
                    </tr>
                    <tr>
                        <td><asp:Label ID="labelVotingInActiveImageUrl" runat="server">Five Up - InActive Image Url</asp:Label></td>
                        <td><uc:selectImage ID="imageVotingInActiveImageUrl" runat="server" /></td>
                    </tr>
                    <tr>
                        <td><asp:Label ID="labelVotingEvenImageUrl" runat="server">Five Up - Even Image Url</asp:Label></td>
                        <td><uc:selectImage ID="imageVotingEvenImageUrl" runat="server" /></td>
                    </tr>
                    <tr>
                        <td valign="top"><asp:Label ID="labelVotingSelectText" runat="server">Five Up - Select Text</asp:Label></td>
                        <td valign="top"><asp:TextBox ID="txtVotingSelectText" runat="server" Width="400"></asp:TextBox><br />
                            <span style="color:Gray">five tips separated by commas, exa.: Bad, Fair, Ok, Good, Great</span>
                        </td>
                    </tr>
                </table>
            </fieldset>
        </div>

        <div class="pagePanel">
            <fieldset>
                <legend>Fields</legend>
                <table>
                    
                    <tr>
                        <td valign="top" style="width:200px">Title</td>
                        <td valign="top">
                            <asp:CheckBox ID="checkTitleEnabled" runat="server" OnCheckedChanged="CheckedChanged" Text="Enabled" AutoPostBack="true" />&nbsp;
                            <asp:CheckBox ID="checkTitleRequired" runat="server" Text="Required" />&nbsp;&nbsp;
                            <asp:Label ID="labelTitleCssClass" runat="server">Css Class:</asp:Label> <asp:TextBox ID="txtTitleCssClass" runat="server" Width="150"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td valign="top" style="width:200px">Link</td>
                        <td valign="top">
                            <asp:CheckBox ID="checkLinkEnabled" runat="server" OnCheckedChanged="CheckedChanged" Text="Enabled" AutoPostBack="true" />&nbsp;
                            <asp:CheckBox ID="checkLinkRequired" runat="server" Text="Required" />&nbsp;&nbsp;
                            <asp:Label ID="labelLinkCssClass" runat="server">Css Class:</asp:Label> <asp:TextBox ID="txtLinkCssClass" runat="server" Width="150"></asp:TextBox>&nbsp;&nbsp;
                            <asp:Label ID="labelLinkText" runat="server">Link Text:</asp:Label> <asp:TextBox ID="txtLinkText" runat="server" Width="150"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td valign="top">Image</td>
                        <td valign="top">
                            <asp:CheckBox ID="checkImageEnabled" runat="server" OnCheckedChanged="CheckedChanged" Text="Enabled" AutoPostBack="true" />&nbsp;
                            <asp:CheckBox ID="checkImageRequired" runat="server" Text="Required" />&nbsp;&nbsp;
                            <asp:Label ID="labelImageCssClass" runat="server">Css Class:</asp:Label> <asp:TextBox ID="txtImageCssClass" runat="server" Width="150"></asp:TextBox>&nbsp;
                            <asp:Label ID="labelImageWidth" runat="server">Width: </asp:Label><asp:TextBox ID="txtImageWidth" runat="server" Width="40"></asp:TextBox>
                            <asp:RangeValidator 
                                ID="rvImageWidth" 
                                runat="server" 
                                Type="Integer" 
                                MinimumValue="0" 
                                MaximumValue="9999" 
                                ControlToValidate="txtImageWidth"
                                Display="Dynamic"
                                ErrorMessage="??"
                                ValidationGroup="submission"
                                >
                            </asp:RangeValidator>
                            <asp:RequiredFieldValidator
                                ID="rfvImageWidth" 
                                runat="server" 
                                ControlToValidate="txtImageWidth"
                                Display="Dynamic"
                                ErrorMessage="*"
                                ValidationGroup="submission"
                                >
                            </asp:RequiredFieldValidator>
                            <asp:Label ID="labelImageHeight" runat="server">Height: </asp:Label><asp:TextBox ID="txtImageHeight" runat="server" Width="40"></asp:TextBox>
                            <asp:RangeValidator 
                                ID="rvImageHeight" 
                                runat="server" 
                                Type="Integer" 
                                MinimumValue="0" 
                                MaximumValue="9999" 
                                ControlToValidate="txtImageHeight"
                                Display="Dynamic"
                                ErrorMessage="??"
                                ValidationGroup="submission"
                                ></asp:RangeValidator>
                            <asp:RequiredFieldValidator
                                ID="rfvImageHeight" 
                                runat="server" 
                                ControlToValidate="txtImageHeight"
                                Display="Dynamic"
                                ErrorMessage="*"
                                ValidationGroup="submission"
                                >
                                </asp:RequiredFieldValidator>
                            
                        </td>
                    </tr>
                 
                    <tr>
                        <td valign="top">Video</td>
                        <td valign="top">
                            <asp:CheckBox ID="checkVideoEnabled" runat="server" OnCheckedChanged="CheckedChanged" Text="Enabled" AutoPostBack="true" />&nbsp;
                            <asp:CheckBox ID="checkVideoRequired" runat="server" Text="Required"  />&nbsp;&nbsp;
                            <asp:Label ID="labelVideoCssClass" runat="server">Css Class:</asp:Label> <asp:TextBox ID="txtVideoCssClass" runat="server" Width="150"></asp:TextBox>
                        </td>
                    </tr>
                    
                    <tr>
                        <td valign="top">Description</td>
                        <td valign="top">
                            <asp:CheckBox ID="checkDescriptionEnabled" runat="server" OnCheckedChanged="CheckedChanged" Text="Enabled" AutoPostBack="true" />&nbsp;
                            <asp:CheckBox ID="checkDescriptionRequired" runat="server" Text="Required" />&nbsp;&nbsp;
                            <asp:Label ID="labelDescriptionCssClass" runat="server">Css Class:</asp:Label> <asp:TextBox ID="txtDescriptionCssClass" runat="server" Width="150"></asp:TextBox>
                        </td>
                    </tr>

                    <tr>
                        <td valign="top">EmailAddress</td>
                        <td valign="top">
                            <asp:CheckBox ID="checkEmailAddressEnabled" runat="server" OnCheckedChanged="CheckedChanged" Text="Enabled" AutoPostBack="true" />&nbsp;
                            <asp:CheckBox ID="checkEmailAddressRequired" runat="server" Text="Required" />&nbsp;&nbsp;
                            <asp:Label ID="labelEmailAddressCssClass" runat="server">Css Class:</asp:Label> <asp:TextBox ID="txtEmailAddressCssClass" runat="server" Width="150"></asp:TextBox>
                        </td>
                    </tr>
                    
                    <tr>
                        <td valign="top">UserId</td>
                        <td valign="top">
                            <asp:CheckBox ID="checkUserIdEnabled" runat="server" OnCheckedChanged="CheckedChanged" Text="Enabled" AutoPostBack="true" />&nbsp;
                            <asp:CheckBox ID="checkUserIdRequired" runat="server" Text="Required" />&nbsp;&nbsp;
                            <asp:Label ID="labelUserIdCssClass" runat="server">Css Class:</asp:Label> <asp:TextBox ID="txtUserIdCssClass" runat="server" Width="150"></asp:TextBox>
                        </td>
                    </tr>        
                    
                    <tr>
                        <td valign="top">Submitter</td>
                        <td valign="top">
                            <asp:CheckBox ID="checkSubmitterEnabled" runat="server" OnCheckedChanged="CheckedChanged" Text="Enabled" AutoPostBack="true" />&nbsp;
                            <asp:CheckBox ID="checkSubmitterRequired" runat="server" Text="Required" />&nbsp;&nbsp;
                            <asp:Label ID="labelSubmitterCssClass" runat="server">Css Class:</asp:Label> <asp:TextBox ID="txtSubmitterCssClass" runat="server" Width="150"></asp:TextBox>
                        </td>
                    </tr>        
                    
                    <tr>
                        <td valign="top">DocumentCredit</td>
                        <td valign="top">
                            <asp:CheckBox ID="checkDocumentCreditEnabled" runat="server" OnCheckedChanged="CheckedChanged" Text="Enabled" AutoPostBack="true" />&nbsp;
                            <asp:CheckBox ID="checkDocumentCreditRequired" runat="server" Text="Required" />&nbsp;&nbsp;
                            <asp:Label ID="labelDocumentCreditCssClass" runat="server">Css Class:</asp:Label> <asp:TextBox ID="txtDocumentCreditCssClass" runat="server" Width="150"></asp:TextBox>
                        </td>
                    </tr>        
                    
                </table>
            </fieldset>
        </div>

        <asp:LinkButton 
            ID="btnSave" 
            runat="server" 
            OnClick="btnSave_Click" 
            Text="Save"
            ValidationGroup="submission"
            CausesValidation="true"
            ></asp:LinkButton>
        <uc:statusMessage ID="statusMessage" runat="server" />
    </asp:View>
    
    <asp:View ID="viewEditSubmission" runat="server">
        <uc:editSubmission ID="editSubmission" runat="server" />
    </asp:View>
</asp:MultiView>