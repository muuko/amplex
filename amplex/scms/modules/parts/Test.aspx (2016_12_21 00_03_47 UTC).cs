using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace amplex.scms.modules.parts
{
	public partial class test : System.Web.UI.Page
	{
		protected void Page_Load(object sender, EventArgs e)
		{

		}

		protected void btn_Test(object sender, EventArgs args)
		{
			parts.classes.PartsLoader partsLoader = new amplex.scms.modules.parts.classes.PartsLoader();
			
			string strError;

			if (partsLoader.Init())
			{
				string strPath = @"C:\projects\amplex\cmdSageExport\bin\Debug\parts-dump.xml";

				// if (partsLoader.LoadPartsFile(Server.MapPath("~/sites/amplex/files/parts/parts-dump.gz"), true))
				if (partsLoader.LoadPartsFile(strPath, false))
				{
					partsLoader.ProcessParts();
				}
			}
		}
	}
}
