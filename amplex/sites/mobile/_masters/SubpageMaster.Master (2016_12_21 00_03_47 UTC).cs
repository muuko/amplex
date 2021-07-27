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

namespace sites.mobile.masters
{
	public partial class SubpageMaster : System.Web.UI.MasterPage
	{
		protected void Page_Load(object sender, EventArgs e)
		{


			if (!IsPostBack)
			{
				if (this.Page is scms.RootPage)
				{
					scms.RootPage rootPage = (scms.RootPage)this.Page;
				}
			}

		}
	}
}
