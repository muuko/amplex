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

namespace scms.modules.parts.part
{
	public partial class edit : global::scms.RootControl
	{
		protected void Page_Load(object sender, EventArgs e)
		{
			if (!IsPostBack)
			{

				try
				{
					global::scms.data.ScmsDataContext dc = new scms.data.ScmsDataContext();

					/*
                    var pageList = (from pl in dc.scms_navigation_pagelists
                                    where pl.instanceId == this.ModuleInstanceId.Value
                                    select pl).FirstOrDefault();*/

					/* EnableControls(null, null); */

				}
				catch (Exception ex)
				{
          string strMessage = string.Format( "Exception thrown while loading part for module id '{0}'.", this.ModuleInstanceId );
          ScmsEvent.Raise(strMessage, this, ex);
					throw ex;
				}
			}
		}

		protected void btnSave_Click(object sender, EventArgs args)
		{
		}
	}
}