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
	public partial class SelectImagePage : System.Web.UI.Page
	{

		protected void Page_Load(object sender, EventArgs args)
		{
			if (!IsPostBack)
			{
				int? siteId = null;
				string strSiteId = Request.QueryString["sid"];
				if (!string.IsNullOrEmpty(strSiteId))
				{
					int n;
					if (int.TryParse(strSiteId, out n))
					{
						siteId = n;
					}
				}
				if (!siteId.HasValue)
				{
					siteId = (int?)Session["content-edit-site-id"];
					if (!siteId.HasValue)
						throw new Exception("site id is missing");
				}


				global::scms.modules.content.FileManager.ESelectType eSelectType = global::scms.modules.content.FileManager.ESelectType.Image;
				string strSelecType = Request.QueryString["type"];
				if (!string.IsNullOrEmpty(strSelecType))
				{
					try
					{
						eSelectType = (global::scms.modules.content.FileManager.ESelectType)Enum.Parse(typeof(global::scms.modules.content.FileManager.ESelectType), strSelecType, true);
					}
					catch (Exception ex)
					{
                        string strMessage = string.Format("Failed parsing '{0}' as select type.", strSelecType);
                        global::scms.ScmsEvent scmsEvent = new global::scms.ScmsEvent(strMessage, this, ex);
                        scmsEvent.Raise();
					}
				}
				fileManager.SelectType = eSelectType;

				string strTitle = null;
				switch (eSelectType)
				{
					case global::scms.modules.content.FileManager.ESelectType.Image:
						strTitle = "Select Image";
						break;

					case global::scms.modules.content.FileManager.ESelectType.Any:
						strTitle = "Select File";
						break;
				}
				literalTitle.Text = strTitle;
				literalTitle2.Text = strTitle;


				// get root files directory for this site
				global::scms.data.ScmsDataContext dc = new global::scms.data.ScmsDataContext();
				var site = (from s in dc.scms_sites
										where s.id == siteId.Value
										select s).Single();
				string strFilesLocation = site.filesLocation;
				fileManager.FilesLocation = strFilesLocation;
				fileManager.SelectType = eSelectType;

				string strPreselectUrl = Request.QueryString["preselectUrl"];
				if (string.IsNullOrEmpty(strPreselectUrl))
				{
					fileManager.PreselectUrl = string.Format("{0}{1}", strFilesLocation, eSelectType == FileManager.ESelectType.Image ? "images" : string.Empty);
				}
				else
				{
					fileManager.PreselectUrl = strPreselectUrl;
				}
			}
		}
	}
}
