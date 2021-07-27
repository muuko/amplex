using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace merlinlawgroup.sites.mobile._controls
{
	public partial class footer : System.Web.UI.UserControl
	{
		protected void Page_Load(object sender, EventArgs e)
		{
			string strFullSiteUrl = scms.Configuration.GetValue("full-site-url", false);
			if (!string.IsNullOrEmpty(strFullSiteUrl))
			{
				anchorFullSite2.HRef = strFullSiteUrl;
			}
		}
	}
}