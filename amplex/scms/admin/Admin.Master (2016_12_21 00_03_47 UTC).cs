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

namespace scms.admin
{
	public partial class Admin : System.Web.UI.MasterPage
	{
		public enum ENavType
		{
			Pages,
			Templates,
			Masters,
			Files,
			Sites,
			Plugins,
            Security,
            Settings,
			None//,
		}
		protected ENavType navType = ENavType.None;
		public ENavType NavType
		{
			get { return navType; }
			set { navType = value; }
		}



        protected void Page_Init(object sender, EventArgs e)
        {
            bool? bSslEnabled;
            scms.Configuration.GetValue("ssl-enabled", false, out bSslEnabled);

            bool? bUseSslForAdmin;
            scms.Configuration.GetValue("use-ssl-for-admin", false, out bUseSslForAdmin);
            scms.modules.security.ProtocolHelper.RedirectIfRequired(Context, bSslEnabled, true, bUseSslForAdmin);
        }

		protected void Page_Load(object sender, EventArgs e)
		{
            Response.Expires = 0;
			if (!IsPostBack)
			{

				literalTitle.Text = this.Page.Title;

				switch (navType)
				{
					case ENavType.Pages:
						anchorPages.Attributes.Add("class", "active");
						break;

					case ENavType.Templates:
						anchorTemplates.Attributes.Add("class", "active");
						break;

					case ENavType.Masters:
						anchorMasters.Attributes.Add("class", "active");
						break;

					case ENavType.Files:
						anchorFiles.Attributes.Add("class", "active");
						break;

					case ENavType.Sites:
						anchorSites.Attributes.Add("class", "active");
						break;

					case ENavType.Plugins:
						anchorPlugins.Attributes.Add("class", "active");
						break;

                    case ENavType.Security:
                        anchorSecurity.Attributes.Add("class", "active");
                        break;

                    case ENavType.Settings:
                        anchorSettings.Attributes.Add("class", "active");
                        break;
				}
			}
		}

	}
}
