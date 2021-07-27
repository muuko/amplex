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
    public partial class DownloadFile : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                string strPath = Request.QueryString["file"];
                Response.ContentType = "application/x-unknown";

                string strFileName = System.IO.Path.GetFileName(strPath);
                string strContentDisposition = string.Format("attachment; filename={0}", strFileName);
                Response.AddHeader("Content-Disposition", strContentDisposition);

                // string strFile = "/scms/modules/content/images/document.jpg";
                // string strPath = Server.MapPath(strFile);
                Response.WriteFile(strPath);

                Response.End();
            }
            catch (System.Threading.ThreadAbortException)
            {
            }
            catch (Exception ex)
            {
                throw new Exception("Exception thrown during download.", ex);
            }

        }
    }
}
