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

namespace scms.modules.navigation.sitemap
{
    public partial class view : scms.modules.navigation.ViewSitemapControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                string strSiteMap;
                string strError;
                Exception exError;
                if (!BuildSiteMap(out strSiteMap, out strError, out exError))
                {
                    // TODO log this error
                    throw new Exception("Failed building sitemap");
                }
                literalSitemap.Text = strSiteMap;
            }
        }
    }
}