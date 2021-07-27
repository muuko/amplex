<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="view.ascx.cs" Inherits="scms.modules.rss.rssList.view" %>
<%@ Register Src="~/scms/controls/Pager.ascx" TagPrefix="uc" TagName="pager" %>
   
<asp:MultiView ID="mv" runat="server" ActiveViewIndex="0">   
	<asp:View ID="viewItemNoTemplate" runat="server">

		<div class="scms-rssList">

			<div id="divTitle" runat="server" class="scms-rssList-title"></div>

			<asp:Repeater 
				ID="rptItems" 
				runat="server"
				OnItemDataBound="rptItems_ItemDataBound"
			>
				<ItemTemplate>
					<div class="scms-rssList-item">
						<div id="divTitle" class="scms-rssList-itemTitle" runat="server"><asp:Literal ID="literalTitle" runat="server"></asp:Literal></div>
					</div>
				</ItemTemplate>
			</asp:Repeater>

			<div id="divReadMore" runat="server" class="scms-rssList-read-more">
					<a id="anchorReadMore" runat="server"></a>
			</div>
		  
		</div>
	</asp:View>

	<asp:View ID="viewItemTemplated" runat="server">
		
		<asp:Literal ID="literalHeaderHtml" runat="server"></asp:Literal>
		
		<asp:Repeater 
				ID="rptItemsTemplated" 
				runat="server"
				OnItemDataBound="rptItemsTemplated_ItemDataBound"
			>
				<ItemTemplate>
					<asp:Literal ID="literalItem" runat="server"></asp:Literal>
			</ItemTemplate>
		</asp:Repeater>
		
		<asp:Literal ID="literalFooterHtml" runat="server"></asp:Literal>
		
	</asp:View>
	
</asp:MultiView>