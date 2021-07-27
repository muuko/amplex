using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace scms.admin.controls
{
    public partial class SelectImage : System.Web.UI.UserControl
    {
        public bool Enabled
        {
            get
            {
                return txtPath.Enabled;
            }

            set
            {
                txtPath.Enabled = value;
                anchorSource.Visible = value;
                literalSourceDisabled.Visible = !value;
            }
        }

        public string Path
        {
            get
            {
                return txtPath.Text;
            }
            set
            {
                txtPath.Text = value;
            }
        }

				protected scms.modules.content.FileManager.ESelectType selectType = scms.modules.content.FileManager.ESelectType.Image;
				public scms.modules.content.FileManager.ESelectType SelectType
				{
					get { return selectType; }
					set { selectType = value; }
				}

        public int ? SiteId
        {
            get
            {
                return (int?)ViewState["SiteId"];
            }
            set
            {
                ViewState["SiteId"] = value;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            RegisterScript();
            if (!IsPostBack)
            {
                anchorSource.HRef = string.Format("javascript:ShowSelectWindowWithSource({0},'{1}');", SiteId.ToString(), txtPath.ClientID);
            }
        }

        protected void RegisterScript()
        {
string strScriptFormat = @"

function ShowSelectWindowWithSource(siteid,source)
{{
	var popupWidth = 900;
	var popupHeight = 400;

	var left,top;
	left = (screen.width/2)-(popupWidth/2);
	top = (screen.height/3)-(popupHeight/2);
	
	var url;
	url = ""/scms/modules/content/select-image.aspx?type={0}&sid="" + siteid+ ""&target=document.aspnetForm."" + source + "".value&preselectUrl="" + document.getElementById(source).value;
	
	var selectWindow;
	selectWindow = open
	(
		url,
		""SelectImage"",
		""modal=yes,width="" + popupWidth + "",height="" + popupHeight + "",resizable=1,status=1,scrollbars=1,top="" + top + "",left="" + left
	);
	if( selectWindow.focus )
	{{
		selectWindow.focus();
	}}
}}";

						string strScript = string.Format(strScriptFormat, selectType.ToString());
            this.Page.ClientScript.RegisterClientScriptBlock(typeof(string), "ShowSelectWindowWithSource", strScript, true);
        }
    }
}