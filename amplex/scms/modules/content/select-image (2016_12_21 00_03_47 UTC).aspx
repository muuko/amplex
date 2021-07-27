<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="select-image.aspx.cs" Inherits="scms.modules.content.SelectImagePage" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<%@ Register Src="~/scms/modules/content/controls/FileManager.ascx" TagPrefix="uc" TagName="FileManager" %>
<%@ Register Src="~/scms/admin/controls/StatusMessage.ascx" TagPrefix="uc" TagName="statusMessage" %>

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title><asp:Literal ID="literalTitle" runat="server"></asp:Literal></title>
    <style type="text/css">
			body,html
			{
				color:#000;
				background:white;
				font-family:Verdana;
				font-size:12px;
				margin:0;
				padding: 0;
			}
			
			a:link, a:visited
			{
				color:Navy;
			}
			
			#header
			{
				padding-left: 10px;
			}
			
		
    </style>
   
</head>
<body >
    <form id="form1" runat="server">
				<div id="header">
					<h1><asp:Literal ID="literalTitle2" runat="server" /></h1>
				</div>
				<div id="main">
					<uc:FileManager ID="fileManager" runat="server" Mode="Select"   />
				</div>
			
    </form>
</body>
</html>
