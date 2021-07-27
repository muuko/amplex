<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="slideshowSettings.ascx.cs" Inherits="scms.modules.slideshow.slideshowSettings" %>
<%@ Register Src="~/scms/admin/controls/StatusMessage.ascx" TagPrefix="uc" TagName="statusMessage" %>


<table class="admin-settings">
    <tr>
        <td><label>Slideshow Type</label></td>
        <td class="value">
            <asp:DropDownList ID="ddlType" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlType_SelectedIndexChanged">
                <asp:ListItem Text="Basic" Value="basic" Selected="True"></asp:ListItem>
                <asp:ListItem Text="Template" Value="template"></asp:ListItem>
                <asp:ListItem Text="Custom" Value="custom"></asp:ListItem>
            </asp:DropDownList>
        </td>
    </tr>

    <asp:PlaceHolder ID="placeholderSize" runat="server">
        <tr>
            <td><label>Width</label></td>
            <td class="value">
                <asp:TextBox 
                    ID="txtWidth" 
                    runat="server"
                    Width="60"
                    ></asp:TextBox>
                <asp:RequiredFieldValidator 
                    ID="rfvWidth"
                    runat="server"
                    ControlToValidate="txtWidth"
                    Display="Dynamic"
                    ErrorMessage="*"
                    ValidationGroup="slideshowSettings">
                </asp:RequiredFieldValidator>
                <asp:RangeValidator
                    id="rvWidth"
                    runat="server"
                    ControlToValidate="txtWidth"
                    Display="Dynamic"
                    ErrorMessage="0-9999"
                    Type="Integer"
                    MinimumValue="0"
                    MaximumValue="9999"
                    ValidationGroup="slideshowSettings">
                </asp:RangeValidator>
            </td>
        </tr>

        <tr>
            <td><label>Height</label></td>
            <td class="value">
                <asp:TextBox 
                    ID="txtHeight" 
                    Width="60"
                    runat="server"></asp:TextBox>
                <asp:RequiredFieldValidator 
                    ID="rfvHeight"
                    runat="server"
                    ControlToValidate="txtHeight"
                    Display="Dynamic"
                    ErrorMessage="*"
                    ValidationGroup="slideshowSettings" />
                <asp:RangeValidator
                    id="rvHeight"
                    runat="server"
                    ControlToValidate="txtHeight"
                    Display="Dynamic"
                    ErrorMessage="0-9999"
                    Type="Integer"
                    MinimumValue="0"
                    MaximumValue="9999"
                    ValidationGroup="slideshowSettings"></asp:RangeValidator>
            </td>
        </tr>
    </asp:PlaceHolder>
    
    <asp:PlaceHolder ID="placeholderTransition" runat="server">
        <tr>
            <td><label>Transition Type</label></td>
            <td class="value">
                <asp:DropDownList 
                    ID="ddlTransitionType"
                    runat="server">
                    <asp:ListItem Text="slide" Value="slide"></asp:ListItem>
                    <asp:ListItem Text="fade" Value="fade"></asp:ListItem>
                    <asp:ListItem Text="blindX" Value="blindX">blindX</asp:ListItem>
                    <asp:ListItem Text="blindY" Value="blindY">blindY</asp:ListItem>
                    <asp:ListItem Text="blindZ" Value="blindZ">blindZ</asp:ListItem>
                    <asp:ListItem Text="cover" Value="cover">cover</asp:ListItem>
                    <asp:ListItem Text="curtainX" Value="curtainX">curtainX</asp:ListItem>
                    <asp:ListItem Text="curtainY" Value="curtainY">curtainY</asp:ListItem>
                    <asp:ListItem Text="fade" Value="fade">fade</asp:ListItem>
                    <asp:ListItem Text="fadeZoom" Value="fadeZoom">fadeZoom</asp:ListItem>
                    <asp:ListItem Text="growX" Value="growX">growX</asp:ListItem>
                    <asp:ListItem Text="growY" Value="growY">growY</asp:ListItem>
                    <asp:ListItem Text="none" Value="none">none</asp:ListItem>
                    <asp:ListItem Text="scrollUp" Value="scrollUp">scrollUp</asp:ListItem>
                    <asp:ListItem Text="scrollDown" Value="scrollDown">scrollDown</asp:ListItem>
                    <asp:ListItem Text="scrollLeft" Value="scrollLeft">scrollLeft</asp:ListItem>
                    <asp:ListItem Text="scrollRight" Value="scrollRight">scrollRight</asp:ListItem>
                    <asp:ListItem Text="scrollHorz" Value="scrollHorz">scrollHorz</asp:ListItem>
                    <asp:ListItem Text="scrollVert" Value="scrollVert">scrollVert</asp:ListItem>
                    <asp:ListItem Text="shuffle" Value="shuffle">shuffle</asp:ListItem>
                    <asp:ListItem Text="slideX" Value="slideX">slideX</asp:ListItem>
                    <asp:ListItem Text="slideY" Value="slideY">slideY</asp:ListItem>
                    <asp:ListItem Text="toss" Value="toss">toss</asp:ListItem>
                    <asp:ListItem Text="turnUp" Value="turnUp">turnUp</asp:ListItem>
                    <asp:ListItem Text="turnDown" Value="turnDown">turnDown</asp:ListItem>
                    <asp:ListItem Text="turnLeft" Value="turnLeft">turnLeft</asp:ListItem>
                    <asp:ListItem Text="turnRight" Value="turnRight">turnRight</asp:ListItem>
                    <asp:ListItem Text="uncover" Value="uncover">uncover</asp:ListItem>
                    <asp:ListItem Text="wipe" Value="wipe">wipe</asp:ListItem>
                    <asp:ListItem Text="zoom" Value="zoom">zoom</asp:ListItem>
                </asp:DropDownList>
            </td>
        </tr>
        <tr>
            <td><label>Transition Speed</label>
                <asp:RequiredFieldValidator 
                    ID="rfvTransitionSpeed"
                    runat="server"
                    ControlToValidate="txtTransitionSpeed"
                    Display="Dynamic"
                    ErrorMessage="*"
                    ValidationGroup="slideshowSettings" />

            </td>
            <td class="value">
                <asp:TextBox 
                    ID="txtTransitionSpeed" 
                    runat="server"
                    Width="60"
                ></asp:TextBox> milliseconds
                <asp:RangeValidator
                    id="tvTransitionSpeed"
                    runat="server"
                    ControlToValidate="txtTransitionSpeed"
                    Display="Dynamic"
                    ErrorMessage="0-99999"
                    Type="Integer"
                    MinimumValue="0"
                    MaximumValue="99999"
                    ValidationGroup="slideshowSettings"></asp:RangeValidator>                
            </td>
        </tr>
        <tr>
            <td><label>Pause Time</label>
            <asp:RequiredFieldValidator 
                    ID="rfvPauseTime"
                    runat="server"
                    ControlToValidate="txtPauseTime"
                    Display="Dynamic"
                    ErrorMessage="*"
                    ValidationGroup="slideshowSettings" />
            </td>
            <td class="value">
                <asp:TextBox
                    id="txtPauseTime"
                    runat="server"
                    Width="60"
                    >
                </asp:TextBox> milliseconds
                <asp:RangeValidator
                    id="rvPauseTime"
                    runat="server"
                    ControlToValidate="txtPauseTime"
                    Display="Dynamic"
                    ErrorMessage="0-99999"
                    Type="Integer"
                    MinimumValue="0"
                    MaximumValue="99999"
                    ValidationGroup="slideshowSettings"></asp:RangeValidator>                     
            </td>
        </tr>
        <tr>
            <td><label>Random</label></td>
            <td class="value"><asp:CheckBox ID="checkRandom" runat="server" /></td>
        </tr>
        <tr>
            <td><label>Pause on Hover</label></td>
            <td class="value"><asp:CheckBox ID="checkPauseOnHover" runat="server" /></td>
        </tr>        
        <tr>
            <td><label>Advance On Click</label></td>
            <td class="value"><asp:CheckBox ID="checkAdvanceOnClick" runat="server" /></td>
        </tr>                            
    </asp:PlaceHolder>

    <asp:PlaceHolder ID="placeholderPagerSettings" runat="server">
        <tr><td  valign="top"><label>Use Pager</label></td>
        <td class="value" valign="top">

            <asp:CheckBox ID="checkUsePager" runat="server" OnCheckedChanged="checkUsePager_CheckedChanged" runat="server" />&nbsp;
            <table>
                <tr><td>Location:</td>
                    <td><asp:DropDownList ID="ddlPagerLocation" runat="server">
                        <asp:ListItem Text="After" Value="after" Selected="True"></asp:ListItem>
                        <asp:ListItem Text="Before" Value="before"></asp:ListItem>
                        <asp:ListItem Text="Content Managed (mark location with class)" Value="content"></asp:ListItem>
                    </asp:DropDownList></td>
                </tr>
                <tr><td>Type:</td>
                    <td><asp:DropDownList ID="ddlPagerType" runat="server">
                        <asp:ListItem Text="Text" Value="text" selected="True"></asp:ListItem>
                        <asp:ListItem Text="Thumbnail" Value="thumbnail"></asp:ListItem>
                        <asp:ListItem Text="Custom (content managed)" Value="custom"></asp:ListItem>
                        </asp:DropDownList>
                        <asp:CustomValidator
                            id="cvPagerType"
                            runat="server"
                            ControlToValidate="ddlPagerType"
                            OnServerValidate="cvPagerType_ServerValidate"
                            Display="Dynamic"
                            ValidationGroup="slideshowSettings"
                         ></asp:CustomValidator>
                    </td>
                </tr>
                <tr><td>Css Class:</td>
                    <td><asp:TextBox ID="txtPagerCssClass" runat="server" Width="200px"></asp:TextBox></td>
                </tr>
                <tr><td>Thumbnail:</td>
                    <td>Width <asp:TextBox ID="txtPagerWidth" runat="server" Width="40px"></asp:TextBox>
                        Height <asp:TextBox ID="txtPagerHeight" runat="server" Width="40px"></asp:TextBox>
                        
                    </td>
                </tr>
                
            </table>
            
             
             
        </td>
        </tr>
    </asp:PlaceHolder>


    <asp:PlaceHolder ID="placeholderAdvanced" runat="server">
        <tr>
            <td><label>Advanced</label></td>
            <td class="value"><asp:CheckBox ID="checkAdvanced" runat="server" AutoPostBack="true" OnCheckedChanged="checkAdvanced_CheckedChanged" /></td>
        </tr>
    </asp:PlaceHolder>

    <asp:PlaceHolder ID="placeholderSelectorButtons" runat="server">
        <tr>
            <td><label>Has Selector Buttons</label></td>
            <td class="value"><asp:CheckBox ID="checkHasSelectorButtons" runat="server" /></td>
        </tr>
    </asp:PlaceHolder>
    
    <asp:PlaceHolder ID="placeholderItemTemplate" runat="server">
        <tr>
            <td valign="top"><label>Item Template</label></td>
            <td valign="top" class="value">
                <div style="margin-bottom:4px;">
                    Prebuilt template: 
                    <asp:DropDownList ID="ddlLoadTemplate" runat="server">
                        <asp:ListItem Text="Image-Title-Content" Value="itc" Selected="True"></asp:ListItem>
                    </asp:DropDownList>&nbsp; 
                    <asp:LinkButton ID="btnLoadTemplate" runat="server" OnClick="btnLoadTemplate_Click" Text="Load"></asp:LinkButton><br />
                 
                </div>
                <asp:TextBox ID="txtItemTemplate" runat="server" TextMode="MultiLine" Columns="60" Rows="8"></asp:TextBox>
                <div style="margin-top:6px;">
                    <em>Keywords:</em>
                    <table cellpadding="4">
                        <tr><td>$SLIDESHOW-CLASS$</td><td>$HEADING$</td></tr>
                        <tr><td>$WIDTH$</td><td>$LINK$</td></tr>
                        <tr><td>$HEIGHT$</td><td>$IMAGE$</td></tr>
                        <tr><td>$TRANSITION-TYPE$</td><td>$CONTENT$</td></tr>
                    </table>
                </div>
            </td>
        </tr>
    </asp:PlaceHolder>
    
    <asp:PlaceHolder ID="placeholderTemplates" runat="server">
        <tr>
            <td valign="top"><label>Header Template</label></td>
            <td valign="top" class="value"><asp:TextBox ID="txtHeaderTemplate" runat="server"></asp:TextBox></td>
        </tr>
        <tr>
            <td valign="top"><label>Footer Template</label></td>
            <td valign="top" class="value"><asp:TextBox ID="txtFooterTemplate" runat="server"></asp:TextBox></td>
        </tr>
    </asp:PlaceHolder>
    
    <asp:PlaceHolder ID="placeholderHeaderScript" runat="server">
        <tr>
            <td><label>Header Script</label></td>
            <td class="value"><asp:TextBox ID="txtHeaderScript" runat="server"></asp:TextBox></td>
        </tr>
    </asp:PlaceHolder>
    


    <tr>
        <td colspan="2">
            <asp:LinkButton 
                ID="btnSave"
                runat="server"
                OnClick="btnSave_Click"
                ValidationGroup="slideshowSettings"
                CausesValidation="true"
                Text="Save"
            ></asp:LinkButton>
            <br />
            <uc:statusMessage ID="statusMessage" runat="server" />
        </td>
    </tr>    
    
</table>

