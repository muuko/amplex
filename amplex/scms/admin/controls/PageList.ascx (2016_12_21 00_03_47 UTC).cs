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

namespace scms.admin.controls
{
	public partial class PageListControl : System.Web.UI.UserControl
	{
		protected int? nSiteId = null;
		public int ? SiteId
		{
			get { return nSiteId; }
			set { nSiteId = value; }
		}

		protected int? nPageId = null;
		public int ? PageId
		{
			get { return nPageId; }
			set { nPageId = value; }
		}

		protected void Page_Load(object sender, EventArgs e)
		{
			if (!IsPostBack)
			{
				DataBind();
			}
		}

		public override void DataBind()
		{
			base.DataBind();

			string strError;
			Exception exError;
			if (!LoadChildren(out strError, out exError))
			{
				string strMessage = string.Format("{0}<br /><br />{1}", strError, exError.ToString());
				statusMessage.ShowFailure(strMessage);
			}
		}



		protected bool LoadChildren(out string strError, out Exception exError)
		{
			bool bSuccess = false;
			strError = null;
			exError = null;


			try
			{
				lvPages.DataSource = null;

				global::scms.data.ScmsDataContext dc = new global::scms.data.ScmsDataContext();
				if( nPageId.HasValue )
				{


					var pages = from p in dc.scms_pages
											where p.parentId == nPageId.Value
											where p.deleted == false
											orderby p.ordinal
											select new
											{
												id = p.id,
												linktext = p.linktext,
												type = TypeToString(p.type),
												visible = p.visible ? "Y" : "N"
											};
					lvPages.DataSource = pages;
				}


				lvPages.DataBind();

				bSuccess = true;
		
			}
			catch (Exception ex)
			{
				strError = "Error occurred while loading child pages.";
				exError = ex;
			}

			return bSuccess;
		}

		protected string TypeToString(char chType)
		{
			string strType = null;

			switch (chType)
			{
				case 'P':
					strType = "Content Page";
					break;

				case 'R':
					strType = "Redirect";
					break;

				case 'A':
					strType = "Alias";
					break;

				case 'I':
					strType = "Internal";
					break;

				default:
					strType = "Unknown";
					break;
			}

			return strType;
		}

		protected void Delete(object objSender, CommandEventArgs args)
		{
			global::scms.data.ScmsDataContext dc = new global::scms.data.ScmsDataContext();
			int nChildPageId = int.Parse(args.CommandArgument.ToString());

			DeleteRecurse(dc, nChildPageId);
			dc.SubmitChanges();
			global::scms.CacheManager.Clear();

			DataBind();
		}

		protected void DeleteRecurse(global::scms.data.ScmsDataContext dc, int nPageId)
		{

			// find children
			var childrenToDelete = (from pchild in dc.scms_pages
															where pchild.deleted == false
															where pchild.parentId == nPageId
															select pchild);
			foreach (var child in childrenToDelete)
			{
				DeleteRecurse(dc, child.id);
			}


            var itemsToDelete = (from p in dc.scms_pages
                                where p.id == nPageId
                                join ppm in dc.scms_page_plugin_modules on p.id equals ppm.pageId into g_ppm
                                select new
                                {
                                    p,
                                    g_ppm
                                }).Single();
            // mark instances as deleted
            foreach (var ppm in itemsToDelete.g_ppm)
            {
                ppm.deleted = true;
                if (ppm.owner)
                {
                    ppm.scms_plugin_module_instance.deleted = true;
                }
            }

            itemsToDelete.p.deleted = true;
		}



		protected void Move(object objSender, CommandEventArgs args)
		{
			bool bUp = string.Compare(args.CommandName, "up", true) == 0;
			int nPageIdChild = int.Parse((string)args.CommandArgument);

			global::scms.data.ScmsDataContext dc = new global::scms.data.ScmsDataContext();

			
			var pages = from p in dc.scms_pages
									where p.parentId == nPageId.Value
									where p.deleted == false
									orderby p.ordinal
									select p;

			global::scms.data.scms_page priorPage = null;
			global::scms.data.scms_page nextPage = null;
			

			var thisPage = pages.Where(p => (p.id == nPageIdChild)).Single();

			int nOrdinal = 0;
			bool bFound = false;
			foreach (var page in pages)
			{
				page.ordinal = nOrdinal;

				if (page.id == thisPage.id)
				{
					bFound = true;
				}
				else
				{
					if (bFound)
					{
						if (nextPage == null)
						{
							nextPage = page;
						}
					}
					else
					{
						priorPage = page;
					}
				}

				nOrdinal++;
			}

			if (bUp)
			{
				if (priorPage != null)
				{
					int nOrdinalTemp = thisPage.ordinal;
					thisPage.ordinal = priorPage.ordinal;
					priorPage.ordinal = nOrdinalTemp;
				}
			}
			else
			{
				if (nextPage != null)
				{
					int nOrdinalTemp = thisPage.ordinal;
					thisPage.ordinal = nextPage.ordinal;
					nextPage.ordinal = nOrdinalTemp;
				}
			}

			dc.SubmitChanges();
			global::scms.CacheManager.Clear();
			string strError;
			Exception exError;
			if( !LoadChildren(out strError, out exError))
			{
				throw new Exception(strError, exError);
			}
		}
		
	}

	






}