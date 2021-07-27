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
	public partial class view :  RootControl 
	{
		protected void Page_Load(object sender, EventArgs e)
		{
			selectImage.SiteId = this.SiteId;
			
			if (!IsPostBack)
			{
				string strSearchResults = Request.QueryString["searchResults"];
				if (!string.IsNullOrEmpty(strSearchResults))
				{
					anchorReturnToCatalog.HRef = strSearchResults;
				}

				string strSageId = Request.QueryString["p"];
				if (!string.IsNullOrEmpty(strSageId))
				{
					amplex.scms.modules.parts.classes.partsDataContext dcParts = new amplex.scms.modules.parts.classes.partsDataContext();
					var partPlusSize =
										 (from p in dcParts.cat_parts
											where p.sage_ID == strSageId
											join s in dcParts.cat_sizes on p.sage_ProductCategoryDesc2 equals s.id into gs
											from subSize in gs.DefaultIfEmpty()
											select new { p, s = subSize }).FirstOrDefault();
					if (partPlusSize  != null)
					{
						var part = partPlusSize.p;
						if (Page.User.IsInRole("administrator"))
						{
							placeholderEditImge.Visible = true;
							selectImage.Path = part.imageUrl;
						}
						else
						{
							placeholderEditImge.Visible = false;
						}

						title.InnerText = part.sage_Description1;
						if (!string.IsNullOrEmpty(part.sage_Description2))
						{
							literalBotanicalName.Text = part.sage_Description2;
						}
						else
						{
							divBotanicalName.Visible = false;
						}

						if (partPlusSize.s != null)
						{
							literalSize.Text = partPlusSize.s.name ?? partPlusSize.s.id;
						}
						else
						{
							divSize.Visible = false;
						}
						

						SetupImage(part);

						if (!string.IsNullOrEmpty(part.sage_LongDescription))
						{
							divPartSummary.InnerText = part.sage_LongDescription;
						}
						else
						{
							divPartSummary.Visible = false;
						}

						if (Page.User.Identity.IsAuthenticated)
						{
							placeholderActualPrice.Visible = true;
							placeholderLoginToViewPrice.Visible = false;
							if (part.sage_price.HasValue && (part.sage_price.Value > 0))
							{
								literalPrice.Text = part.sage_price.Value.ToString("c");
							}
						}
						else
						{
							placeholderLoginToViewPrice.Visible = true;
							placeholderActualPrice.Visible = false;

							anchorLogin.HRef = string.Format("/login?returnUrl={0}", HttpUtility.UrlEncode(Request.RawUrl));
						}

					}
					else
					{
						Visible = false;
					}
				}


				if (this.ModuleInstanceId.HasValue )
				{
					/*
					scms.data.ScmsDataContext dc = new scms.data.ScmsDataContext();
					
                    pageList = (from pl in dc.scms_navigation_pagelists
                                where pl.instanceId == this.ModuleInstanceId
                                select pl).SingleOrDefault();
					 */
				}
			}
		}

		protected void SetupImage(amplex.scms.modules.parts.classes.cat_part part)
		{
			bool bShowImage = false;

			if (part != null)
			{
				if (!string.IsNullOrEmpty(part.imageUrl))
				{
					imgPart.Src = part.imageUrl;
					imgPart.Alt = string.Format("{0} [{1}]", part.sage_Description1, part.sage_Description2);
					bShowImage = true;
				}
			}

			imgPart.Visible = bShowImage;
		}

		protected void btnUpdateImage_Click(object sender, EventArgs args)
		{
			string strImageUrl = selectImage.Path;
			
			string strSageId = Request.QueryString["p"];
			if (!string.IsNullOrEmpty(strSageId))
			{

				amplex.scms.modules.parts.classes.partsDataContext dcParts = new amplex.scms.modules.parts.classes.partsDataContext();
				var part = (from p in dcParts.cat_parts
										where p.sage_ID == strSageId
										select p).FirstOrDefault();
				if (part != null)
				{
					part.imageUrl = strImageUrl;
					dcParts.SubmitChanges();

					global::scms.search.search search = new scms.search.search();
					search.Init();

					global::scms.IPluginApplication iPluginApplication = null;
					global::scms.PluginApplications.GetApplication(typeof(global::scms.modules.parts.partsApplication), out iPluginApplication);
					global::scms.modules.parts.partsApplication partsApplication = (global::scms.modules.parts.partsApplication)iPluginApplication;

					partsApplication.IndexPart(search, part);

				}
				else
				{
					throw new Exception("Unexpected part not found");
				}

				SetupImage(part);
			}
			else
			{
				throw new Exception("Unexpected query parameter p missing");
			}
		}
	}
}