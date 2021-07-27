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

namespace scms.modules.navigation.subnav
{
	public partial class view : global::scms.modules.navigation.ViewSubnavControl
  {
    /*
    public string CssClassSubnav
    {
        get { return cssClassSubnav; }
        set { cssClassSubnav = value; }
    }

    public string CssClassActive
    {
        get { return cssClassActive; }
        set { cssClassActive = value; }
    }

    public int? MaxDepth
    {
        get { return nMaxDepth; }
        set { nMaxDepth = value; }
    }
    */

    protected void Page_Load(object sender, EventArgs e)
		{
			if (!IsPostBack)
			{
				this.ShowRoot = false;
				if (this.ModuleInstanceId.HasValue)
				{
					scms.data.ScmsDataContext dc = new scms.data.ScmsDataContext();
					scms.data.scms_navigation_subnav subnav = (from sn in dc.scms_navigation_subnavs
																										 where sn.instanceId == this.ModuleInstanceId.Value
																										 select sn).FirstOrDefault();
					if (subnav != null)
					{
						this.nMaxDepth = subnav.maxDepth;
						this.ShowChildren = subnav.showChildren;
						this.cssClassActive = subnav.cssClassActive;
						this.PinNavigationToHomePage = subnav.pinNavigationToHomePage;
						this.PinDepth = subnav.pinDepth;
						this.FloatingShowSiblingsIfNoChildren = subnav.showSiblingsIfNoChildren;
						this.MaxChildrenPerNode = subnav.maxChildrenPerNode;
					}
				}

				string strSubnav;
				string strError;
				Exception exError;
				if (LoadSubnav(out strSubnav, out strError, out exError))
				{
					literalSubnav.Text = string.Format("{0}", strSubnav);
				}
				else
				{
					// todo log this
					throw new Exception(string.Format("Failed loading subnav: '{0}'.", strError, exError));
				}
			}
		}
  }
}