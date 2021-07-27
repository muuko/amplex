<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="edit.ascx.cs" Inherits="scms.modules.navigation.pagelist.edit" %>
<%@ Register Src="~/scms/admin/controls/StatusMessage.ascx" TagPrefix="uc" TagName="StatusMessage" %>
<%@ Register Src="~/scms/admin/controls/PageSelector.ascx" TagPrefix="uc" TagName="PageSelector" %>


<table cellspacing="2" cellpadding="2">
	<tr>
		<td valign="top"><span class="label">Root Page</span></td>
		<td valign="top"><uc:PageSelector ID="pageSelectorRootPage" runat="server" />
		    <asp:CheckBox ID="checkIncludeChildren" runat="server" Text="Include Children" AutoPostBack="true" OnCheckedChanged="EnableControls" />
		    <asp:CheckBox ID="checkHideParentNodes" runat="server" Text="Hide Parent Nodes" />
		</td>
	</tr>
	<tr>
	    <td valign="top"><span class="label">Template</span></td>
	    <td valign="top">
	        <asp:CheckBox ID="checkTemplateEnabled" runat="server" Text="Enabled" AutoPostBack="true" OnCheckedChanged="EnableControls" />
	    </td>
	</tr>
	<asp:MultiView ID="multiView" runat="server">
	    <asp:View ID="viewTemplate" runat="server">
	        <tr>
	            <td valign="top"><span class="label">Header Template</span></td>
	            <td valign="top">
                    <asp:TextBox ID="txtHeaderTemplate" runat="server" Columns="60" Rows="2" TextMode="MultiLine"></asp:TextBox>
	            </td>
	        </tr>
	        <tr>
	            <td valign="top"><span class="label">Group Template</span></td>
	            <td valign="top">
	                <asp:CheckBox ID="checkGroupTemplateEnabled" runat="server" Text="Enabled" AutoPostBack="true" OnCheckedChanged="EnableControls" />
	                <span style="margin-left:20px;">&nbsp;</span><asp:Label ID="labelItemsPerGroup" runat="server" Text="Items Per Group:" AssociatedControlID="txtItemsPerGroup"></asp:Label>
	                <asp:TextBox ID="txtItemsPerGroup" runat="server"></asp:TextBox>
	                <asp:RequiredFieldValidator
	                    id="rfvItemsPerGroup"
	                    runat="server"
	                    ControlToValidate="txtItemsPerGroup"
	                    Display="Dynamic"
	                    ErrorMessage="reqired"
                    ></asp:RequiredFieldValidator>
                    <asp:RangeValidator
                        id="rvItemsPerGroup"
                        runat="server"
                        ControlToValidate="txtItemsPerGroup"
                        Display="Dynamic"
                        MinimumValue="1"
                        MaximumValue="10000"
                        Type="Integer"
                        ErrorMessage="integer 1 -> 10000"></asp:RangeValidator>
	            </td>
	        </tr>
	        <tr>
	            <td valign="top"><span class="label">Group Header Template</span></td>
	            <td valign="top">
	                <asp:TextBox ID="txtGroupHeaderTemplate" runat="server" Columns="60" Rows="2" TextMode="MultiLine"></asp:TextBox>
	            </td>
	        </tr>
	        <tr>
	            <td valign="top"><span class="label">Item Template</span></td>
	            <td valign="top">
                    <asp:TextBox ID="txtItemTemplate" runat="server" Columns="60" Rows="6" TextMode="MultiLine"></asp:TextBox><br />
                    To substitue settings from pages insert these tokens:<br />
                    #TITLE#, #LINKTEXT#, #URL#, #LONGDATE#, #SHORTDATE#, <br />
                    #DESCRIPTION#, #IMAGEURL#, #IMAGEURL_ENCODED#
	            </td>
	        </tr>
	        <tr>
	            <td valign="top"><span class="label">Separator Template</span></td>
	            <td valign="top">
                    <asp:TextBox ID="txtSeparatorTemplate" runat="server" Columns="60" Rows="3" TextMode="MultiLine"></asp:TextBox>
	            </td>
	        </tr>
	        <tr>
	            <td valign="top"><span class="label">Group Footer Template</span></td>
	            <td valign="top">
	                <asp:TextBox ID="txtGroupFooterTemplate" runat="server" Columns="60" Rows="2" TextMode="MultiLine"></asp:TextBox>
	            </td>
	        </tr>
	        <tr>
	            <td valign="top"><span class="label">Footer Template</span></td>
	            <td valign="top">
                    <asp:TextBox ID="txtFooterTemplate" runat="server" Columns="60" Rows="2" TextMode="MultiLine"></asp:TextBox>
	            </td>
	        </tr>	        
	    </asp:View>
	    <asp:View ID="viewNoTemplate" runat="server">
            <tr>
	                <td valign="top"><span class="label">Title</span></td>
	                <td valign="top">
	                    <asp:CheckBox ID="checkTitleEnabled" runat="server" Text="Enabled" AutoPostBack="true" OnCheckedChanged="EnableControls" />&nbsp;
	                    <asp:CheckBox ID="checkTitleAsLink" runat="server" Text="Show as link" />
	                </td>
	            </tr>
                <tr>
	                <td valign="top"><span class="label">Link Text</span></td>
	                <td valign="top">
	                    <asp:CheckBox ID="checkLinkEnabled" runat="server" Text="Enabled" AutoPostBack="true" OnCheckedChanged="EnableControls" />&nbsp;
	                    <asp:CheckBox ID="checkLinkAsLink" runat="server" Text="Show as link" />
	                </td>
	            </tr>
	            <tr>
	                <td valign="top"><span class="label">Associated Date</span></td>
	                <td valign="top">
	                    <asp:CheckBox ID="checkAssociatedDateEnabled" runat="server" Text="Enabled" AutoPostBack="true" OnCheckedChanged="EnableControls" />&nbsp;
	                    <asp:Label ID="labelAssociatedDateFormat" runat="server">Format:</asp:Label> <asp:TextBox ID="txtAssociatedDateFormat" runat="server" MaxLength="64" Width="100"  />
	                </td>
	            </tr>
	            <tr>
	                <td valign="top"><span class="label">Thumbnail</span></td>
	                <td valign="top">
	                    <asp:CheckBox ID="checkThumbnailEnabled" runat="server" Text="Enabled" AutoPostBack="true" OnCheckedChanged="EnableControls" />&nbsp;
	                    <asp:CheckBox ID="checkThumbnailAsLink" runat="server" Text="Show as link" />
	                    <asp:Label ID="labelThumbnailWidth" runat="server">Width:</asp:Label> <asp:TextBox ID="txtThumbnailWidth" runat="server" Width="40"></asp:TextBox>
	                    <asp:RangeValidator 
	                        id="rvThumbnailWidth"
	                        runat="server"
	                        MinimumValue="0" 
	                        MaximumValue="1000" 
	                        Type="Integer" 
	                        ControlToValidate="txtThumbnailWidth"
	                        Display="Dynamic"
	                        ErrorMessage="0-1000"
	                    ></asp:RangeValidator>
	                    <asp:Label ID="labelThumbnailHeight" runat="Server">Height:</asp:Label> <asp:TextBox ID="txtThumbnailHeight" runat="server" Width="40"></asp:TextBox>
	                    <asp:RangeValidator 
	                        id="rvThumbnailHeight"
	                        runat="server"
	                        MinimumValue="0" 
	                        MaximumValue="1000" 
	                        Type="Integer" 
	                        ControlToValidate="txtThumbnailHeight"
	                        Display="Dynamic"
	                        ErrorMessage="0-1000"
	                    ></asp:RangeValidator>
	                </td>
	            </tr>
	            <tr>
	                <td valign="top"><span class="label">Description</span></td>
	                <td valign="top">
	                    <asp:CheckBox ID="checkDescriptionEnabled" runat="server" Text="Enabled" AutoPostBack="true" OnCheckedChanged="EnableControls" />&nbsp;
	                    <asp:CheckBox ID="checkDescriptionTruncated" runat="server" Text="Truncate" AutoPostBack="true" OnCheckedChanged="EnableControls" />
	                    <asp:Label ID="labelDescriptionTruncationLength" runat="server" >To Length: </asp:Label><asp:TextBox ID="txtDescriptionTruncationLength" runat="server" Width="40"></asp:TextBox>
	                    <asp:RangeValidator 
	                        id="rvDescriptionTruncationLength"
	                        runat="server"
	                        MinimumValue="0" 
	                        MaximumValue="10000" 
	                        Type="Integer" 
	                        ControlToValidate="txtDescriptionTruncationLength"
	                        Display="Dynamic"
	                        ErrorMessage="0-10000"
	                    ></asp:RangeValidator>
	                    <asp:RequiredFieldValidator
	                        id="rfvDescriptionTruncationLength"
	                        runat="server"
	                        ControlToValidate="txtDescriptionTruncationLength"
	                        Display="Dynamic"
	                        ErrorMessage="required"></asp:RequiredFieldValidator>
	                </td>
	            </tr>
	            <tr>
	                <td valign="top"><span class="label">Item Read More</span></td>
	                <td valign="top">
	                    <asp:CheckBox ID="checkItemReadMoreEnabled" runat="server" Text="Enabled" AutoPostBack="true" OnCheckedChanged="EnableControls" />&nbsp;
	                    <asp:Label ID="labelItemReadMoreText" runat="server">Text: </asp:Label><asp:TextBox ID="txtItemReadMoreText" runat="server" Width="100" MaxLength="256"></asp:TextBox>
	                    <asp:RequiredFieldValidator
	                        id="rfvReadItemMoreText"
	                        runat="server"
	                        ControlToValidate="txtItemReadMoreText"
	                        Display="Dynamic"
	                        ErrorMessage="required"
	                        ></asp:RequiredFieldValidator>
	                </td>
	            </tr>	    
	    </asp:View>
	</asp:MultiView>
	
	<tr>
	    <td valign="top"><span class="label">Limit Items</span></td>
	    <td valign="top">
	        <asp:CheckBox ID="checkListLimitItems" runat="server" Text="Limit Items" AutoPostBack="true" OnCheckedChanged="EnableControls" />&nbsp;
	        <asp:Label ID="labelListMaxItems" runat="server" Text="Max Items: "></asp:Label>
	        <asp:TextBox ID="txtListMaxItems" runat="server" Width="40" MaxLength="4"></asp:TextBox>
	        <asp:RequiredFieldValidator 
	            ID="rfvListMaxItems" 
	            runat="server"
	            ControlToValidate="txtListMaxItems"
	            Display="Dynamic"
	            ErrorMessage="required"
	            ></asp:RequiredFieldValidator>
	        <asp:RangeValidator
	            id="rvListMaxItems"
	            runat="server"
	            ControlToValidate="txtListMaxItems"
	            Display="Dynamic"
	            ErrorMessage="1-99"
	            MinimumValue="1"
	            MaximumValue="99"
	            ></asp:RangeValidator>
	        <br />
	        
	        <asp:CheckBox ID="checkListReadMore" runat="server" Text="Read More" AutoPostBack="true" OnCheckedChanged="EnableControls" />&nbsp;
	        <asp:Label ID="labelListReadMoreText" runat="server" Text="Text: "></asp:Label>
	        <asp:TextBox ID="txtListReadMoreText" runat="server" Width="100" MaxLength="256"></asp:TextBox>&nbsp;
	        <asp:RequiredFieldValidator
	            id="rfvReadMoreText"
	            runat="server"
	            ControlToValidate="txtListReadMoreText"
	            Display="Dynamic"
	            ErrorMessage="required"></asp:RequiredFieldValidator>
	        <asp:Label ID="labelListReadMorePage" runat="server" Text="Destination Page: "></asp:Label><uc:PageSelector ID="pageSelectorListReadMorePage" runat="server" />
	        <asp:RequiredFieldValidator
	            id="rfvReadMorePage"
	            runat="server"
	            ControlToValidate="pageSelectorListReadMorePage"
	            Display="Dynamic"
	            ErrorMessage="required"></asp:RequiredFieldValidator>
	    </td>
	</tr>
	<tr>
	    <td valign="top"><span class="label">Paging</span></td>
	    <td valign="top">
	        <asp:CheckBox ID="checkPagingEnabled" runat="server" Text="Enabled" AutoPostBack="true" OnCheckedChanged="EnableControls" />&nbsp;
	        <asp:Label ID="labelPageSize" runat="server" Text="Page Size: "></asp:Label><asp:TextBox ID="txtPageSize" runat="server" Width="40" MaxLength="4"></asp:TextBox>
	        <asp:RequiredFieldValidator 
	            ID="rfvPageSize"
	            runat="server"
	            ControlToValidate="txtPageSize"
	            Display="Dynamic"
	            ErrorMessage="required"></asp:RequiredFieldValidator>
	        <asp:RangeValidator
	            id="rvPageSize"
	            runat="server"
	            ControlToValidate="txtPageSize"
	            Display="Dynamic"
	            ErrorMessage="1-1000"
	            MinimumValue="1"
	            MaximumValue="1000"
	            Type="Integer"></asp:RangeValidator>
	        
	    </td>
	</tr>
	<tr>
	    <td valign="top"><span class="label">Sort</span></td>
	    <td valign="top">
	        <asp:CheckBox ID="checkSortAscending" runat="server" Text="Ascending" Checked="true" />
        </td>
	</tr>
	
	<%--
	<tr>
		<td><span class="label">Wrap in Html Element</span></td>
		<td>
			<asp:CheckBox ID="checkWrapInHtmlElement" runat="server" Checked="false" AutoPostBack="true" OnCheckedChanged="checkWrapInHtmlElement_checkChanged" />
		</td>
	</tr>
	<tr>
		<td><span class="label">Html Element Type</span></td>
		<td><asp:TextBox ID="txtHtmlElementType" runat="server" Width="40"></asp:TextBox>
		<asp:CustomValidator
			id="cvHtmlElementType"
			runat="server"
			ErrorMessage="please enter html tag value such as h1, h2, div, span, etc."
			Display="Dynamic"
			OnServerValidate="cvHtmlElementType_ServerValidate"
			ValidateEmptyText="true"
			></asp:CustomValidator>
		</td>
	</tr>
	<tr>
		<td><span class="label">Html Element Css Class</span></td>
		<td><asp:TextBox ID="txtHtmlCssClass" runat="server"></asp:TextBox></td>
	</tr>
	--%>
</table>	
<div class="button-bar">
	<asp:LinkButton ID="btnSave" runat="server" Text="Save" OnClick="btnSave_Click"></asp:LinkButton>
	<uc:StatusMessage ID="statusMessage" runat="server" />
</div>			


