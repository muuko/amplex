<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="FileManager.ascx.cs" Inherits="scms.modules.content.FileManager" %>
<%@ Register Src="~/scms/admin/controls/StatusMessage.ascx" TagPrefix="uc" TagName="statusMessage" %>

<style type="text/css">
	.clear
	{
		clear:both;
	}
	
	.file-manager
	{
		background-color:White;
	}
	
	.file-manager table td:nth-child(4), .file-manager .left-border
	{
		border-left: solid 1px #ddd;
	}
	
	.file-manager-heading-row
	{
		font-size: 125%;
		font-weight:bold;
		color:#333;
		background-color:#ddd;
		height:1.6em;
	}

	.file-manager-heading-row td
	{
		border-bottom: solid 1px #888;
	}	

	.file-manager-heading
	{
		margin-left: 12px;
	}
	
	.file-manager-panel
	{
		padding:10px;
		
	}
	
	.file-manager-document-panel
	{
		padding-bottom:10px;
		margin-bottom:10px;
		
		height: 200px;
		height: auto;
		min-height: 200px;
		overflow:auto;
	}
	
	.file-upload-panel
	{
		width:430px;
	}
	
	.file-manager-file
	{
		display:inline;
		float:left;
		width:120px;
		text-align:center;
		padding:10px;
		margin:10px;
	}
	
	.file-manager-file-inactive
	{
		background-color:#eee;
	}
	
	.file-manager-file-active
	{
		background-color:#ccf;
	}

	.file-manager-thumnail	
	{
		border:none;
		padding-bottom:5px;
	}
	
	.file-manager-file-name
	{
	}
	
	#left-spacer
	{
		width: 25px;
	}
	
	#action-row
	{
		color: #ddd;
		background-color: #fff;
	}
	
	#action-row td
	{
		border-top: solid 1px #ddd;
		height:25px;
		padding-left:6px;
		padding-top: .5em;
		padding-bottom: .5em;
	}
	
	#action-row-2
	{
		color: #666;
		background-color: #fff;
	}
	
	#action-row-2 td
	{
		border-top: solid 1px #ddd;
		height:25px;
		padding-left:6px;
		padding-top: .5em;
		padding-bottom: .5em;
	}
	
	
</style>

<div class="file-manager">
	<uc:statusMessage ID="statusMessage" runat="server" />

	<table cellpadding="0" cellspacing="0" width="100%">
		<tr class="file-manager-heading-row">
			<td id="left-spacer"></td>
			<td><div class="file-manager-heading">Folders</div></td>
			<td style="width:50px;"><div class="file-manager-heading" style="width:50px;">&nbsp;</div></td>
			<td><div class="file-manager-heading">Files</div></td>
		</tr>
		<tr>
			<td ></td>
			<td valign="top">
				<div class="file-manager-panel">
					<asp:TreeView 
						ID="treeViewFolders" 
						runat="server" ShowExpandCollapse="true" ShowLines="true"  ExpandDepth="0"
						OnSelectedNodeChanged="treeViewFolders_SelectedNodeChanged"
						OnTreeNodePopulate="treeViewFolders_TreeNodePopulate" 
						SelectedNodeStyle-Font-Bold="true"
						>

					</asp:TreeView>
				</div>
			</td>
			<td></td>
			<td class="left-border" valign="top">
				<div class="file-manager-panel">
					<div class="file-manager-document-panel">
					
						<asp:Repeater ID="rptFiles" runat="server" OnItemDataBound="rptFiles_ItemDataBound">
							<ItemTemplate>
								<div id="divFile" runat="server" class="file-manager-file file-manager-file-inactive">
									<asp:ImageButton 
										CssClass="file-manager-thumnail" 
										id="ibImageFile" 
										runat="server" 
										ImageUrl="http://www.coinbug.com/images/logo.gif"
										OnCommand="ibImageFile_Command" CommandName="focus" CommandArgument='<%# Container.DataItem %>'
										/><br />
									<span id="spanFileName" runat="server" class="file-manager-file-name">logo.jpg</span>
								</div>
							</ItemTemplate>
						</asp:Repeater>
					</div>
				</div>
			</td>
		</tr>
		<tr id="action-row">
			<td></td>
			<td><asp:Panel ID="panelFolderAction" runat="server" DefaultButton="btnFolderAction">
							<asp:DropDownList ID="ddlFolderAction" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlFolderAction_SelectedIndexChanged">
								<asp:ListItem Text="New" Value="new" Selected="True"></asp:ListItem>
								<asp:ListItem Text="Rename" Value="rename"></asp:ListItem>
								<asp:ListItem Text="Delete" Value="delete"></asp:ListItem>
							</asp:DropDownList>
							<asp:TextBox ID="txtNewFolder" runat="server" Width="80"></asp:TextBox>
							<asp:LinkButton ID="btnFolderAction" runat="server" OnClick="btnFolderAction_Click" Text="Go" ></asp:LinkButton>
						</asp:Panel></td>
			<td></td>
			<td class="left-border">
				<asp:Panel ID="panelFileAction" runat="server" DefaultButton="btnFolderAction">
					<asp:DropDownList ID="ddlFileAction" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlFileAction_SelectedIndexChanged">
						<asp:ListItem Text="Select" Value="select"></asp:ListItem>
						<asp:ListItem Text="Download" Value="download"></asp:ListItem>
						<asp:ListItem Text="Rename" Value="rename"></asp:ListItem>
						<asp:ListItem Text="Delete" Value="delete"></asp:ListItem>
					</asp:DropDownList>
					<asp:TextBox ID="txtFileAction" runat="server" Width="240"></asp:TextBox>
					<asp:LinkButton ID="btnFileAction" runat="server" OnClick="btnFileAction_Click" Text="Go" ></asp:LinkButton>
			        <a id="anchorDownload" runat="server" target="_blank" href="#">Go</a>
				</asp:Panel>	
			</td>
		</tr>
		
		<tr id="action-row-2">
			<td ></td>
			<td style="vertical-align:top;">
				<div style="width:200px;">
				<asp:CheckBox ID="checkShowSystemFolders" runat="server" Text=" Show System Folders" AutoPostBack="true" OnCheckedChanged="checkShowSystemFolders_CheckedChanged" />
				</div>
			</td>
			<td></td>
			<td class="left-border">
			
				<asp:Panel CssClass="file-upload-panel clear" ID="panelUpload" runat="server" DefaultButton="btnUpload">
					<asp:FileUpload ID="fileUpload" runat="server" /><asp:RequiredFieldValidator
						id="rfvFileIUpload" 
						runat="server"
						Display="Dynamic"
						ErrorMessage="*"
						ControlToValidate="fileUpload"
						ValidationGroup="fileUpload"
						></asp:RequiredFieldValidator>&nbsp; 
					<asp:LinkButton ID="btnUpload" runat="server" Text="Upload File" OnClick="btnUpload_Click" CausesValidation="true" ValidationGroup="fileUpload"></asp:LinkButton>
				</asp:Panel>

			</td>
		</tr>
	</table>

</div>
