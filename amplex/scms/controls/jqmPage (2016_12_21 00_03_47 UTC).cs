using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;

namespace scms.controls
{
	public class jqmPage : System.Web.UI.HtmlControls.HtmlGenericControl 
	{
		protected string strPageId = null;
		public string PageId
		{
			get { return strPageId; }
			set { strPageId = value; }
		}

		public jqmPage(string strTagName):base(strTagName)
		{
		}

		protected override void RenderBeginTag(HtmlTextWriter writer)
		{
			//base.RenderBeginTag(writer);
			writer.Write("<div data-role=\"page\" id=\"");
			writer.Write(strPageId);
			writer.Write("\">");
		}

		protected override void RenderEndTag(HtmlTextWriter writer)
		{
			//base.RenderEndTag(writer);
			writer.Write("</div>");
		}

	}
}
