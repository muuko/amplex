<%@ Page Language="C#" MasterPageFile="~/scms/admin/Admin.Master" AutoEventWireup="true" CodeBehind="sites.aspx.cs" Inherits="scms.admin.sites" Title="Admin - Sites" %>
<%@ Register Src="~/scms/admin/controls/SiteDdl.ascx" TagName="SiteDdl" TagPrefix="uc" %>
<%@ Register Src="~/scms/admin/controls/StatusMessage.ascx" TagName="statusMessage" TagPrefix="uc" %>
<%@ Register Src="~/scms/admin/controls/PageSelector.ascx" TagPrefix="uc" TagName="pageSelector" %>


<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
	<uc:SiteDdl ID="siteDdl" runat="server" />
	<hr />
	
	<div class="pagePanel">
		<fieldset>
			<legend>General</legend>
			<table>
				<tr>
					<td class="pageLabel">Name</td>
					<td><asp:TextBox ID="txtName" runat="server"></asp:TextBox></td>
				</tr>
				<tr>
					<td class="pageLabel">Home Page</td>
					<td>
						<uc:pageSelector ID="pageSelectorHomePage" runat="server" Enabled="false" />
						<asp:RequiredFieldValidator 
							ID="rfvHomePage" 
							runat="server"
							ControlToValidate="pageSelectorHomePage"
							Display="Dynamic"
							ErrorMessage="*"
							></asp:RequiredFieldValidator>
					</td>
				</tr>
				<tr>
					<td class="pageLabel">Host Name Regex</td>
					<td><asp:TextBox ID="txtHostNameRegex" runat="server" Width="300"></asp:TextBox></td>
				</tr>
				<tr>
				    <td class="pageLabel">Canonical Host Name</td>
				    <td><asp:TextBox ID="txtCanonicalHostNameRegex" runat="server" Width="300"></asp:TextBox></td>
				</tr>
				<tr>
					<td class="pageLabel">Default Template</td>
					<td>
						<asp:DropDownList ID="ddlDefaultTemplate" runat="server"></asp:DropDownList>
						<asp:RequiredFieldValidator
							id="rfvDefaultTemplate"
							runat="server"
							ControlToValidate="ddlDefaultTemplate"
							Display="Dynamic"
							ErrorMessage="*"
							></asp:RequiredFieldValidator>
					</td>
				</tr>
			</table>
		</fieldset>
	</div>

	<div class="pagePanel">
		<fieldset>
			<legend>Cache Control</legend>
			<asp:CheckBox ID="checkCacheEnabled" runat="server" OnCheckedChanged="EnableControls" Text="Enabled" Checked="false" AutoPostBack="true" />
			<table>
				<tr>
					<td class="pageLabel">Cache Control</td>
					<td>
					    <asp:DropDownList ID="ddlCacheControl" runat="server">
					        <asp:ListItem Selected="True"></asp:ListItem>
					        <asp:ListItem>public</asp:ListItem>
					        <asp:ListItem>private</asp:ListItem>
					        <asp:ListItem>no-cache</asp:ListItem>
					    </asp:DropDownList>
					</td>
				</tr>
				<tr>
					<td class="pageLabel">Expires (seconds)</td>
					<td>
					    <asp:TextBox ID="txtCacheExpires" runat="server"></asp:TextBox>
					    <asp:CompareValidator 
					        id="cvCacheExpires"
					        runat="server"
					        ControlToValidate="txtCacheExpires" 
					        Operator="GreaterThanEqual" 
					        ValueToCompare="0" 
					        Type="Integer"
					        ErrorMessage="integer >= 0" 
					        Display="Dynamic"></asp:CompareValidator>
					</td>
				</tr>
				<tr>
					<td class="pageLabel">Max Age (seconds)</td>
					<td>
					    <asp:TextBox ID="txtCacheMaxAge" runat="server"></asp:TextBox>
					    <asp:CompareValidator 
					        id="cvCacheMaxAge"
					        runat="server"
					        ControlToValidate="txtCacheMaxAge" 
					        Operator="GreaterThanEqual" 
					        ValueToCompare="0" 
					        Type="Integer"
					        ErrorMessage="integer >= 0" 
					        Display="Dynamic"></asp:CompareValidator>
					</td>
				</tr>
				
			</table>
		</fieldset>
	</div>

    
	<div class="pagePanel">	
		<fieldset>
			<legend>SEO</legend>
			<table>
				<tr>
					<td class="pageLabel" valign="top">Xml Sitemap</td>
					<td valign="top">
						<asp:CheckBox ID="checkXmlSitemapEnabled" runat="server" Text="Generate" AutoPostBack="true" OnCheckedChanged="EnableControls" /><br />
						Url&nbsp <asp:TextBox ID="txtXmlSitemapLocation" runat="server" Width="200"></asp:TextBox>
						<asp:CustomValidator
							id="cvXmlSitemap"
							runat="server"
							Display="Dynamic"
							ErrorMessage="*"
							OnServerValidate="cvXmlSitemap_ServerValidate"
							></asp:CustomValidator>
					</td>
				</tr>
			</table>
		</fieldset>
	</div>
	
	<div class="pagePanel">	
		<fieldset>
			<legend>Mobile Redirect</legend>
			<asp:CheckBox ID="checkMobileRedirectEnabled" runat="server" Text="Enabled" AutoPostBack="true"  OnCheckedChanged="EnableControls" />
			<table>
				<tr>
					<td class="pageLabel" valign="top">Url</td>
					<td valign="top">
						<asp:TextBox ID="txtMobileRedirectUrl" runat="server" MaxLength="1024" Width="400"></asp:TextBox>
						<asp:RequiredFieldValidator 
							ID="rfvMobileRedirectUrl"
							runat="server"
							ControlToValidate="txtMobileRedirectUrl"
							Display="Dynamic"
							ErrorMessage="*"></asp:RequiredFieldValidator>
							
					</td>
				</tr>
				<tr>
					<td class="pageLabel" valign="top">Append Path</td>
					<td valign="top">
						<asp:CheckBox ID="checkMobileRedirectAppendPath" runat="server" />
					</td>
				</tr>
				<tr>
					<td class="pageLabel" valign="top">Append Query String</td>
					<td valign="top">
						<asp:CheckBox ID="checkMobileRedirectAppendQueryString" runat="server" />
					</td>
				</tr>
				<tr>
					<td class="pageLabel" valign="top">Mobile To Full Query String</td>
					<td valign="top">
						<asp:TextBox ID="txtMobileToFullQueryString" runat="server" MaxLength="64" Text="full=1"></asp:TextBox>
					</td>
				</tr>
			</table>
		</fieldset>
	</div>
	
	
	<asp:LinkButton ID="btnSave" runat="server" Text="Save" OnClick="btnSave_Click"  CausesValidation="true"></asp:LinkButton><uc:statusMessage ID="statusMessage" runat="server" />
		
</table>
		
</asp:Content>

