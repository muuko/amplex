using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace amplex.sites.amplex._controls
{
	public partial class globalScript : System.Web.UI.UserControl
	{
		protected void Page_Load(object sender, EventArgs e)
		{
			if (Request.Browser != null)
			{
				string strScriptFormat = @"
<script type=""text/javascript"">
var _scms = {{
	Browser: {{
			IsMobileDevice: {0},
			ScreenPixelsWidth: {1},
			ScreenPixelsHeight: {2}
	}}
}};
</script>
";

				bool bIsMobileDevice = Request.Browser.IsMobileDevice;

				if (!bIsMobileDevice)
				{
					// if mobile forced by debug
					bool? bForceMobile = false;
					if (global::scms.Configuration.GetValue("debug-force-mobile", false, out bForceMobile))
					{
						if (bForceMobile.HasValue && bForceMobile.Value)
						{
							bIsMobileDevice = true;
						}
					}
				}

				string strScript = string.Format(strScriptFormat, 
					bIsMobileDevice.ToString().ToLower(), 
					Request.Browser.ScreenPixelsWidth, 
					Request.Browser.ScreenPixelsHeight);

				literal.Text = strScript;
			}
			


		}
	}
}