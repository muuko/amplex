<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="view.ascx.cs" Inherits="scms.modules.parts.part.view" %>
<%@ Register Src="~/scms/admin/controls/SelectImage.ascx" TagPrefix="uc" TagName="selectImage" %>
<style type="text/css">
.part-title
{
}

.part-label
{
	font-style:italic;
}

.part-content
{
	margin-top: 20px;
}

.part-image
{
	margin-bottom: 10px;
}

</style>


<h1 id="title" runat="server" class="part-title">The Title</h1>
<div id="divBotanicalName" runat="server"><span class="part-label">Botanical Name:</span>&nbsp; <span>
<asp:Literal ID="literalBotanicalName" runat="server">the botanical name</asp:Literal></span></div>
<div id="divSize" runat="server"><span class="part-label">Size:</span>&nbsp; <span><asp:Literal ID="literalSize" runat="server">the size</asp:Literal></span></div>
<div id="divPrice" runat="server"><span class="part-label">Price:</span>
	<asp:PlaceHolder ID="placeholderActualPrice" runat="server" Visible="false" >
		<span><asp:Literal ID="literalPrice" runat="server"></asp:Literal></span>
	</asp:PlaceHolder>
	<asp:PlaceHolder ID="placeholderLoginToViewPrice" runat="server">
		<span><a id="anchorLogin" runat="server" href="/login">Login to View Price</a></span>
	</asp:PlaceHolder>
</div>
<div class="part-content">
	<div class="part-image">
		<img id="imgPart" runat="server" class="part-image" alt="" src="" />
		<asp:PlaceHolder ID="placeholderEditImge" runat="server">
			<div>
				<uc:selectImage ID="selectImage" runat="server" />
				<asp:LinkButton ID="btnUpdateImage" runat="server" Text="[Update]" OnClick="btnUpdateImage_Click"></asp:LinkButton>
			</div>
		</asp:PlaceHolder>
	</div>
	<p class="part-summary" runat="server" id="divPartSummary"></p>
</div>
<div>
	<a id="anchorReturnToCatalog" runat="server" href="/products/plant-catalog">&lt;&lt; Return to Plant Catalog</a>
</div>
