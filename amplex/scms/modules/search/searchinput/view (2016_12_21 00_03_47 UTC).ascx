<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="view.ascx.cs" Inherits="scms.modules.search.search.view"  %>


<div id="divSearch" runat="server" >
	<div id="search-input-wrap">
		<input  
				type="text" 
				id="txtKeywords" 
				runat="server" 
		 />
	 </div>
	 <div id="search-button-wrap">
		<input 
				type="button" 
				id="btnInput" 
				runat="server" 
				value="Search" 
	    />
	</div>
</div>