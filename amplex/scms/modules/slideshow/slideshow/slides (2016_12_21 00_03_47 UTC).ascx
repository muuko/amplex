<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="slides.ascx.cs" Inherits="scms.modules.slideshow.slides" %>
<%@ Register Src="~/scms/admin/controls/StatusMessage.ascx" TagPrefix="uc" TagName="statusMessage" %>
<%@ Register src="~/scms/modules/slideshow/slideshow/slide.ascx" TagPrefix="uc" TagName="slide" %>


<asp:MultiView ID="mv" runat="server" ActiveViewIndex="0">
    <asp:View 
        ID="viewSlides"
        runat="server">

        
            <asp:Repeater
                id="rptSlides"
                runat="server"
                OnItemDataBound="rptSlides_ItemDataBound"
                >
                <HeaderTemplate>
                <table cellpadding="5">
                    <tr>
                        <th align="left">Thumbnail</th>
                        <th align="left">Name</th>
                        <th align="left">Link</th>
                        <th align="center">Action</th>
                    </tr>
                </HeaderTemplate>
                <ItemTemplate>
                    <tr>
                        <td valign="top">
                            <asp:ImageButton ID="ibThumbnail" runat="server" OnCommand="ibThumbnail_Command" CommandArgument='<%# Eval("id") %>' /> 
                        </td>
                        
                        <td valign="top">
                            <%# Eval("name") %>
                        </td>
                        
                        <td valign="top">
                            <%# Eval("linkUrl") %>
                        </td>
                        <td valign="top" align="center">
                            <asp:ImageButton ID="btnEdit" runat="server" ImageUrl="/scms/client/images/action_edit.gif" OnCommand="Edit_Command" CommandArgument='<%# Eval("id") %>'  
                            /><asp:ImageButton ID="btnUp" runat="server" ImageUrl="/scms/client/images/arrow_top.gif" OnCommand="Move_Command" CommandName="Up" CommandArgument='<%# Eval("id") %>'  
                            /><asp:ImageButton ID="btnDown" runat="server" ImageUrl="/scms/client/images/arrow_down.gif" OnCommand="Move_Command" CommandName="Down" CommandArgument='<%# Eval("id") %>'  
                            /><asp:ImageButton ID="btnDelete" runat="server" ImageUrl="/scms/client/images/action_delete.gif" OnCommand="Delete_Command" CommandArgument='<%# Eval("id") %>' OnClientClick="javascript: return confirm('Delete this slide?');" />
                        </td>
                    </tr>
                </ItemTemplate>
                <FooterTemplate>
                        </table>
                </FooterTemplate>
            </asp:Repeater>
            

        <br />
        <asp:LinkButton ID="btnNew" runat="server" Text="New" OnClick="btnNew_Click"></asp:LinkButton>
    </asp:View>
    
    <asp:View
        id="viewDetail"
        runat="server"
    >
        <asp:LinkButton ID="btnReturnToSlides" runat="server" Text="<< Return to Slides" OnClick="btnReturnToSlides_Click"></asp:LinkButton>
        <uc:slide ID="slide" runat="server" />
        <asp:LinkButton ID="btnSave" runat="server" Text="Save" OnClick="btnSave_Click" ValidationGroup="slide"></asp:LinkButton>
        
    </asp:View>
</asp:MultiView>

<uc:statusMessage ID="statusMessage" runat="server" />