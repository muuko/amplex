<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="PageSettings.ascx.cs" Inherits="scms.admin.controls.PageSettings" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxControlToolkit" %>
<%@ Register Src="~/scms/admin/controls/PageSelector.ascx" TagPrefix="uc" TagName="pageSelector" %>
<%@ Register Src="~/scms/admin/controls/StatusMessage.ascx" TagPrefix="uc" TagName="statusMessage" %>
<%@ Register Src="~/scms/admin/controls/SelectImage.ascx" TagPrefix="uc" TagName="selectImage" %>

<asp:Panel ID="panelGeneral" runat="server" >
	<div class="pagePanel">
		<fieldset>
			<legend >General</legend>
			<div class="pageRow"><div class="pageLabel">Page Type</div><div class="pageValue">
			<asp:DropDownList ID="ddlPageType" runat="server" OnSelectedIndexChanged="ddlPageType_SelectedIndexChanged" AutoPostBack="true">
				<asp:ListItem Text="Content Page" Value="P"></asp:ListItem>
				<asp:ListItem Text="Redirect" Value="R"></asp:ListItem>
				<asp:ListItem Text="Alias" Value="A"></asp:ListItem>
				<asp:ListItem Text="Internal" Value="I"></asp:ListItem>
			</asp:DropDownList></div></div>
			<div id="divTemplate" runat="server" class="pageRow">
				<div class="pageLabel">Template</div>
				<div class="pageValue"><asp:DropDownList ID="ddlTemplate" runat="server"></asp:DropDownList>
					<asp:RequiredFieldValidator 
						ID="rfvTemplate" 
						runat="server"
						ControlToValidate="ddlTemplate"
						Display="Dynamic"
						ErrorMessage="*"
						ValidationGroup="Page"></asp:RequiredFieldValidator>
				</div>
			</div>
			
			<asp:PlaceHolder ID="placeholderNavigation" runat="server">
				<div class="pageRow">
					<div class="pageLabel">Parent</div>
					<div class="pageValue"><uc:pageSelector ID="parentPageSelector" runat="server" />
						<asp:RequiredFieldValidator 
							ID="rfvParentPage" 
							runat="server"
							Display="Dynamic"
							ControlToValidate="parentPageSelector"
							ErrorMessage="*"
							ValidationGroup="navigation"></asp:RequiredFieldValidator>
						<asp:CustomValidator
							id="cvParentPage"
							runat="server"
							Display="Dynamic"
							ErrorMessage="parent cannot be self"
							ControlToValidate="parentPageSelector"
							OnServerValidate="verifyPageSelectorNotSelf_ServerValidate"
							ValidationGroup="navigation"></asp:CustomValidator>
					</div>
				</div>
				<div class="pageRow">
					<div class="pageLabel">Link Text</div>
					<div class="pageValue"><asp:TextBox ID="txtLinkText" runat="server" Width="150"></asp:TextBox>
						<asp:RequiredFieldValidator
							id="rfvLinkText"
							runat="server"
							ControlToValidate="txtLinkText"
							Display="Dynamic"
							ErrorMessage="*"
							ValidationGroup="navigation"></asp:RequiredFieldValidator>
					</div>
				</div>
				<div class="pageRow">
					<div class="pageLabel">Slug</div>
					<div class="pageValue"><asp:TextBox ID="txtFragment" runat="server" Enabled="false" Width="150"></asp:TextBox>
						<asp:RequiredFieldValidator
							id="rfvFragment"
							runat="server"
							ControlToValidate="txtFragment"
							Display="Dynamic"
							ErrorMessage="*"
							ValidationGroup="navigation"></asp:RequiredFieldValidator>
					&nbsp;<asp:LinkButton ID="btnEditFragment" runat="server" Text="Edit" OnClick="btnEditFragment_Click"></asp:LinkButton></div></div>
				<div class="pageRow"><div class="pageLabel">Navigation</div><div class="pageValue"><asp:CheckBox ID="checkVisible" runat="server" Text="Include" /></div></div>
			</asp:PlaceHolder>
			<div class="pageRow"><div class="pageLabel">Html Sitemap</div>
				<div class="pageValue">
					<asp:CheckBox ID="checkIncludeInSitemap" runat="server" Text="Include" AutoPostBack="true" OnCheckedChanged="checkIncludeInSitemap_CheckChanged" />
						&nbsp;&nbsp;Link Text <asp:TextBox ID="txtSitemapLinkText" runat="server" Width="200"></asp:TextBox>
				</div>
			</div>
			<div class="pageRow"><div class="pageLabel">Search</div>
				<div class="pageValue">
					<asp:CheckBox ID="checkIncludeInSearch" runat="server" Text="Include"  />
				</div>
			</div>		
		</fieldset>
	</div>
	
	<div class="pagePanel">
		<fieldset>
			<legend>Advanced</legend>
			<div class="pageRow">
				<div class="pageLabel">View State Override</div>
				<div class="pageValue"><asp:CheckBox ID="checkViewStateOverride" runat="server" AutoPostBack="true" OnCheckedChanged="checkViewStateOverride_checkedChanged" /></div>
			</div>
			<div class="pageRow">
				<div class="pageLabel">View State Enabled</div>
				<div class="pageValue"><asp:CheckBox ID="checkViewStateEnabled" runat="server" /></div>
			</div>
		</fieldset>
	</div>
</asp:Panel>

<asp:Panel ID="panelSecurity" runat="server" CssClass="pagePanel">
	<fieldset>
	
		<legend>Security</legend>
	
		<div class="pageRow"><div class="pageLabel">Inherit</div>
		<div class="pageValue">
		    <asp:CheckBox ID="checkSecurityInherit" runat="server" AutoPostBack="true" OnCheckedChanged="checkSecurityInherit_CheckedChanged" />
		    </div>
		</div>
		
		<div class="pageRow">
			<div class="pageLabel">Login Required</div>
			<div class="pageValue"><asp:CheckBox ID="checkSecurityLoginRequired" runat="server" AutoPostBack="true" OnCheckedChanged="checkSecurityLoginRequired_CheckedChanged" /></div>
		</div>
		
		<div class="pageRow">
			<div class="pageLabel">Restrict to Roles</div>
			<div class="pageValue">
			    <asp:CheckBoxList ID="cblSecurityRoles" runat="server" RepeatDirection="Horizontal" RepeatLayout="Flow"></asp:CheckBoxList>
			</div>
		</div>
		
		<div class="pageRow">
		    <div class="pageLabel">Use Https Protocol</div>
		    <div class="pageValue">
		        <asp:CheckBox ID="checkSecurityProtocolSecure" runat="server" Text="" AutoPostBack="true" OnCheckedChanged="checkSecurityProtocolSecure_CheckedChanged" />
		    </div>
		</div>
		
		<div class="pageRow">
		    <div class="pageLabel">Force Protocol</div>
		    <div class="pageValue">
		        <asp:CheckBox ID="checkSecurityProtocolForce" runat="server" Text="" />
		    </div>
		</div>
	</fieldset>
</asp:Panel>


<asp:Panel ID="panelSeo" runat="server"  CssClass="pagePanel">
	<fieldset  >
		<legend>Seo</legend>
		<div class="pageRow"><div class="pageLabel">Title</div><div class="pageValue"><asp:TextBox ID="txtTitle" runat="server" Width="400"></asp:TextBox></div></div>
		<div class="pageRow"><div class="pageLabel">Keywords</div><div class="pageValue"><asp:TextBox ID="txtKeywords" runat="server" Width="400" TextMode="MultiLine" Rows="2"></asp:TextBox></div></div>
		<div class="pageRow"><div class="pageLabel">Description</div><div class="pageValue"><asp:TextBox ID="txtDescription" runat="server" Width="400" TextMode="MultiLine" Rows="4"></asp:TextBox></div></div>
		<div class="pageRow"><div class="pageLabel">Canonical</div><div class="pageValue">
			<ul>
				<li><asp:RadioButton ID="btnRadioCanonicalNone" runat="server" Text="None" GroupName="canonical" AutoPostBack="true" OnCheckedChanged="radioCanonical_CheckedChanged" /></li>
				<li>
					<asp:RadioButton ID="btnRadioCanonicalPage" runat="server" Text="Page" GroupName="canonical" AutoPostBack="true" OnCheckedChanged="radioCanonical_CheckedChanged" />
					<uc:pageSelector ID="pageSelectorCanonical" runat="server" />
					<asp:CustomValidator 
						ID="cvCanonicalPage"
						runat="server"
						OnServerValidate="cvCanonicalPage_ServerValidate"
						Display="Dynamic"
						ErrorMessage="*"
						ValidationGroup="seo"
						></asp:CustomValidator>
				</li>
				<li>
					<asp:RadioButton ID="btnRadioCanonicalUrl" runat="server" Text="Url" GroupName="canonical" AutoPostBack="true" OnCheckedChanged="radioCanonical_CheckedChanged" />
					<asp:TextBox ID="txtCanonicalUrl" runat="server" Width="350" Enabled="false"></asp:TextBox>
					<asp:CustomValidator 
						ID="cvCanonicalUrl"
						runat="server"
						OnServerValidate="cvCanonicalUrl_ServerValidate"
						Display="Dynamic"
						ErrorMessage="*"
						ValidationGroup="seo"
						></asp:CustomValidator>
				</li>
			</ul>
		</div>
		<div class="pageRow">
			<div class="pageLabel">Xml Sitemap</div>
				<div class="pageValue">
				<asp:CheckBox ID="checkIncludeInXmlSitemap" runat="server" Text="Include" AutoPostBack="true" OnCheckedChanged="checkIncludeInXmlSitemap_CheckedChanged" />
						<div style="padding:10px">
						Priority <asp:TextBox ID="txtXmlSitemapPriority" runat="server" Width="50"></asp:TextBox>
						<asp:RangeValidator
							ID="rvSiteMapPriority" 
							runat="server" 
							Display="Dynamic" 
							ErrorMessage="Please format as decimal number: X.Y, from 0 to 1"
							ControlToValidate="txtXmlSitemapPriority"
							MinimumValue="0"
							MaximumValue="1"
							Type="Double"
							ValidationGroup="seo"
							></asp:RangeValidator>
						<br />
						Change Frequency 
						<asp:DropDownList ID="ddlXmlSitemapFrequency" runat="server">
							<asp:ListItem>hourly</asp:ListItem>
							<asp:ListItem>daily</asp:ListItem>
							<asp:ListItem>weekly</asp:ListItem>
							<asp:ListItem>monthly</asp:ListItem>
							<asp:ListItem>yearly</asp:ListItem>
							<asp:ListItem>never</asp:ListItem>
						</asp:DropDownList>
					
					</div>
				</div>
		</div>
		
	</fieldset>
</asp:Panel>

<asp:Panel ID="panelSummary" runat="server"  CssClass="pagePanel">
	<fieldset>
		<legend>Summary</legend>
		<div class="pageRow"><div class="pageLabel">Description</div><div class="pageValue"><asp:TextBox ID="txtSummary" runat="server" Width="400" TextMode="MultiLine" Rows="4"></asp:TextBox></div></div>
		<div class="pageRow">
		    <div class="pageLabel">Thumbnail</div>
		    <div class="pageValue"><uc:selectImage ID="selectImage" runat="server"  />
		    </div>
		</div>
		<div class="pageRow">
		    <div class="pageLabel">Associated Date</div>
		    <div class="pageValue">
		        <asp:TextBox ID="txtAssociatedDate" runat="server" Width="70"></asp:TextBox>
		        <asp:ImageButton ID="btnAssociatedDate" runat="server" ImageUrl="~/scms/client/images/calendar.gif" />
		        <ajaxControlToolkit:CalendarExtender
                    id="ceAssociatedDate"
                    runat="server"
                    TargetControlID="txtAssociatedDate"
                    PopupButtonID="btnAssociatedDate"
                >
                </ajaxControlToolkit:CalendarExtender>
                <asp:CompareValidator
                    id="cvAssociatedDate"
                    runat="server"
                    ControlToValidate="txtAssociatedDate"
                    Display="Dynamic"
                    ErrorMessage="?"
                    Type="Date"
                    Operator="DataTypeCheck"
                    ValidationGroup="summary"
                    ></asp:CompareValidator>
		    </div>
		</div>
	</fieldset>
</asp:Panel>

<asp:Panel ID="panelRedirect" runat="server" CssClass="pagePanel">
	<fieldset>
		<legend>Redirect Settings</legend>
		<div class="pageRow">
			<div class="pageLabel">Destination</div>
			<div class="pageValue">
				<ul style="margin: 0 0 5px 0;">
					<li style="margin:0;">
						<asp:RadioButton ID="btnRadioRedirectPage" runat="server" Text="Select Page" GroupName="redirect" AutoPostBack="true" OnCheckedChanged="radioRedirect_CheckChanged" />
						<uc:pageSelector ID="pageSelectorRedirectPage" runat="server" />
						<asp:CustomValidator 
							ID="cvRedirectPage"
							runat="server"
							ValidationGroup="redirect"
							Display="Dynamic"
							ErrorMessage="*"
							OnServerValidate="cvRedirectPage_ServerValidate"
							>
							</asp:CustomValidator>
					</li>
					<li  style="margin:0;padding:0;">
						<asp:RadioButton ID="btnRadioRedirectUrl" runat="server" Text="Enter Url" GroupName="redirect"  AutoPostBack="true" OnCheckedChanged="radioRedirect_CheckChanged" />
						<asp:TextBox ID="txtRedirectUrl" runat="server" Width="350"></asp:TextBox><asp:CustomValidator 
							ID="cvRedirectUrl"
							runat="server"
							ValidationGroup="redirect"
							Display="Dynamic"
							ErrorMessage="*"
							OnServerValidate="cvRedirectUrl_ServerValidate"
							>
							</asp:CustomValidator>						
					</li>
				</ul>
				<div>
				<asp:CheckBox ID="checkRedirectPermanent" runat="server" Text="Permanent" /></div>
			</div>
		</div>
	</fieldset>
</asp:Panel>

<asp:Panel ID="panelAlias" runat="server" CssClass="pagePanel">
	<fieldset>
		<legend>Alias Settings</legend>
		<div class="pageRow">
			<div class="pageLabel">Destination Page</div>
			<div class="pageValue"><uc:pageSelector ID="pageSelectorAlias" runat="server" /></div>
		</div>
	</fieldset>
</asp:Panel>

<asp:Panel ID="panelInternal" runat="server" CssClass="pagePanel">
	<fieldset>
		<legend>Internal Settings</legend>
		<div class="pageRow">
			<div class="pageLabel">Destination Url</div>
			<div class="pageValue"><asp:TextBox ID="txtInteralUrl" runat="server" Width="300"></asp:TextBox></div>
		</div>
	</fieldset>
</asp:Panel>

<asp:LinkButton ID="btnSave" runat="server" CausesValidation="false" OnClick="btnSave_Click" Text="Save"></asp:LinkButton>
<uc:statusMessage ID="statusMessage" runat="server" />