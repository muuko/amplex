using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;

namespace scms.modules.content
{
    public partial class view : scms.modules.content.ViewContentControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                string strError;
                Exception exError;

                scms.data.scms_content content;
                if (GetContent(out content, out strError, out exError))
                {
                    literalContent.Text = content.content;
                }
            }
        }
    }
}