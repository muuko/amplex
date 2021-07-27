<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="view.ascx.cs" Inherits="scms.modules.navigation.pagelist.view" %>
<%@ Register Src="~/scms/controls/Pager.ascx" TagPrefix="uc" TagName="pager" %>
<asp:MultiView ID="multiView" runat="server">
    
    <asp:View ID="viewTemplate" runat="server">
        <asp:Literal ID="literalTemplate" runat="server"></asp:Literal>
    </asp:View>
    
    <asp:View ID="viewNoTemplate" runat="server">
    
        <div class="scms-pagelist">
    
            <asp:Repeater 
                ID="rptNoTemplatePageListItem" 
                runat="server"
                OnItemDataBound="rptNoTemplatePageListItem_ItemDataBound"
                >
                <ItemTemplate>
                    <div class="scms-pagelist-item">
                        <div id="divLeft" class="scms-pagelist-left" runat="server">
                            <asp:Literal ID="literalThumbnail" runat="server"></asp:Literal>
                        </div>
                        <div id="divRight" class="scms-pagelist-right" runat="server">
                            <div id="divHeading" class="scms-pagelist-heading" runat="server">
                                <div id="divDate" class="scms-pagelist-date" runat="server">
                                    <asp:Literal ID="literalDate" runat="server"></asp:Literal>
                                </div>
                                <div id="divTitle" class="scms-pagelist-title" runat="server">
                                    <asp:Literal ID="literalTitle" runat="server"></asp:Literal>
                                </div>
                                <div id="divLinkText" class="scms-pagelist-linktext" runat="server">
                                    <asp:Literal ID="literalLinkText" runat="server"></asp:Literal>
                                </div>
                            </div>
                            <div id="divDescription" class="scms-pagelist-description" runat="server">
                            </div>
                        </div>
                    </div>
                </ItemTemplate>
            </asp:Repeater>
        
            <div id="divReadMore" runat="server" class="scms-pagelist-read-more">
                <a id="anchorReadMore" runat="server"></a>
            </div>
            
            <uc:pager ID="pager" runat="server" />
            
        </div>
        
    </asp:View>
    
</asp:MultiView>
