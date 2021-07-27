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
	public partial class PluginModuleInstances : System.Web.UI.UserControl
	{
		protected int? nSiteId = null;
		public int? SiteId
		{
			get { return nSiteId; }
			set { nSiteId = value; }
		}

		protected int? nTemplateId = null;
		public int? TemplateId
		{
			get { return nTemplateId; }
			set { nTemplateId = value; }
		}

		protected int? nPageId = null;
		public int? PageId
		{
			get { return nPageId; }
			set { nPageId = value; }
		}

		protected void Page_Load(object sender, EventArgs e)
		{
			if (!IsPostBack)
			{
				string strError;
				Exception exError;
				if (!LoadPageModuleInstances(out strError, out exError))
				{
					statusMessage.ShowFailure(string.Format("{0}<br /><br />{1}", strError, exError.ToString()));
				}
			}
		}

		protected bool LoadPageModuleInstances(out string strError, out Exception exError)
		{
			bool bSucces = false;
			strError = null;
			exError = null;

			try
			{
				global::scms.data.ScmsDataContext dc = new global::scms.data.ScmsDataContext();

				SPluginModuleInstance[] aPluginModuleInstances = null;

				// page module instances
				if (nPageId.HasValue)
				{
					SPagePluginModuleInstance[] aPagePluginModuleInstances =
						(from pmi in dc.scms_page_plugin_modules
						 where pmi.siteId == nSiteId.Value
						 where pmi.pageId == nPageId.Value
						 where pmi.deleted == false
						 orderby pmi.placeHolder, pmi.ordinal
						 select new SPagePluginModuleInstance
						 {
							 SiteId = pmi.siteId,
							 PageId = pmi.pageId,
							 Id = pmi.id,
							 PluginModuleInstanceId = pmi.instanceId,
							 Name = pmi.name,
							 PluginApplication = pmi.scms_plugin_module_instance.scms_plugin_application.name,
							 PluginModule = pmi.scms_plugin_module_instance.scms_plugin_module.name,
							 Ordinal = pmi.ordinal,
							 Owner = pmi.owner,
							 PlaceHolder = pmi.placeHolder,
							 OverrideTemplate = pmi.overrideTemplate
						 }
					).ToArray();

					aPluginModuleInstances = aPagePluginModuleInstances;
				}
				else
				{
					STemplatePluginModuleInstance[] aTemplatePluginModuleInstances =
						(from tpmi in dc.scms_template_plugin_modules
						 where tpmi.siteId == nSiteId.Value
						 where tpmi.templateId == nTemplateId.Value
						 where tpmi.deleted == false
						 orderby tpmi.placeHolder, tpmi.ordinal
						 select new STemplatePluginModuleInstance
						 {
							 SiteId = tpmi.siteId,
							 Id = tpmi.id,
							 PluginModuleInstanceId = tpmi.instanceId,
							 TemplateId = tpmi.templateId,
							 Name = tpmi.name,
							 PluginApplication = tpmi.scms_plugin_module_instance.scms_plugin_application.name,
							 PluginModule = tpmi.scms_plugin_module_instance.scms_plugin_module.name,
							 Ordinal = tpmi.ordinal,
							 Owner = tpmi.owner,
							 PlaceHolder = tpmi.placeHolder
						 }
					).ToArray();

					aPluginModuleInstances = aTemplatePluginModuleInstances;

					divCellTemplateOverride.Visible = false;

				}


				lvPluginModuleInstances.DataSource = aPluginModuleInstances;
				lvPluginModuleInstances.DataBind();

				bSucces = true;


			}
			catch (Exception ex)
			{
				strError = "Error occurred while loading plugin module instances.";
				exError = ex;
			}

			return bSucces;
		}

		protected void lvPluginModuleInstances_ItemDataBound(object sender, ListViewItemEventArgs args)
		{
			ListViewItem item = args.Item;
			switch (args.Item.ItemType)
			{
				case ListViewItemType.DataItem:
					{
						ListViewDataItem dataItem = (ListViewDataItem)args.Item;

						SPagePluginModuleInstance ppmi = dataItem.DataItem as SPagePluginModuleInstance;
						STemplatePluginModuleInstance tpmi = dataItem.DataItem as STemplatePluginModuleInstance;

						Control control = args.Item.FindControl("divCellTemplateOverride");
						if (control != null)
						{
							if (ppmi != null)
							{
								HtmlGenericControl div = (HtmlGenericControl)control;
								div.InnerText = ppmi.OverrideTemplate.ToString();
							}
							else
							{
								control.Visible = false;
							}
						}



					}
					break;
			}
		}

		protected void MoveModuleInstance(object objSender, CommandEventArgs args)
		{
			bool bUp = string.Compare(args.CommandName, "up", true) == 0;
			int nModuleInstanceId = int.Parse((string)args.CommandArgument);

			global::scms.data.ScmsDataContext dc = new global::scms.data.ScmsDataContext();

			if (nPageId.HasValue)
			{
				var modules = from pm in dc.scms_page_plugin_modules
													where pm.deleted == false
													where pm.pageId == nPageId.Value
													orderby pm.placeHolder, pm.ordinal
													select pm;

				var selectedModule = (from pm2 in modules
													where pm2.id == nModuleInstanceId
													select pm2).Single();

				var modulesThisPlaceholder = from pm3 in modules
																				 where pm3.placeHolder.ToLower() == selectedModule.placeHolder.ToLower()
																				 select pm3;

				global::scms.data.scms_page_plugin_module priorModule = null;
				global::scms.data.scms_page_plugin_module nextModule = null;


				int nOrdinal = 0;
				bool bFound = false;
				foreach (var module in modulesThisPlaceholder)
				{
					if (module.ordinal != nOrdinal)
					{
						module.ordinal = nOrdinal;
					}

					if (module.id == selectedModule.id)
					{
						bFound = true;
					}
					else
					{
						if (!bFound)
						{
							priorModule = module;
						}
						else
						{
							if (nextModule == null)
							{
								nextModule = module;
							}
						}
					}

					nOrdinal++;
				}

				if (bUp)
				{
					if (priorModule != null)
					{
						int nOrdinalTemp = selectedModule.ordinal;
						selectedModule.ordinal = priorModule.ordinal;
						priorModule.ordinal = nOrdinalTemp;
					}
				}
				else
				{
					if (nextModule != null)
					{
						int nOrdinalTemp = selectedModule.ordinal;
						selectedModule.ordinal = nextModule.ordinal;
						nextModule.ordinal = nOrdinalTemp;
					}
				}
			}
			else
			{
				if (nTemplateId.HasValue)
				{
					var modules = from tm in dc.scms_template_plugin_modules
																where tm.deleted == false
																where tm.templateId == nTemplateId.Value
																orderby tm.placeHolder, tm.ordinal
																select tm;

					var selectedModule = (from tm2 in modules
															  where tm2.id == nModuleInstanceId
															  select tm2).Single();

					var modulesThisPlaceholder = from tm3 in modules
																			 where tm3.placeHolder.ToLower() == selectedModule.placeHolder.ToLower()
																			 select tm3;

					global::scms.data.scms_template_plugin_module priorModule = null;
					global::scms.data.scms_template_plugin_module nextModule = null;


					int nOrdinal = 0;
					bool bFound = false;
					foreach (var module in modulesThisPlaceholder)
					{
						if (module.ordinal != nOrdinal)
						{
							module.ordinal = nOrdinal;
						}

						if (module.id == selectedModule.id)
						{
							bFound = true;
						}
						else
						{
							if (!bFound)
							{
								priorModule = module;
							}
							else
							{
								if (nextModule == null)
								{
									nextModule = module;
								}
							}
						}

						nOrdinal++;
					}

					if (bUp)
					{
						if (priorModule != null)
						{
							int nOrdinalTemp = selectedModule.ordinal;
							selectedModule.ordinal = priorModule.ordinal;
							priorModule.ordinal = nOrdinalTemp;
						}
					}
					else
					{
						if (nextModule != null)
						{
							int nOrdinalTemp = selectedModule.ordinal;
							selectedModule.ordinal = nextModule.ordinal;
							nextModule.ordinal = nOrdinalTemp;
						}
					}
				}
				else
				{
					throw new Exception("unexpected");
				}
			}

			dc.SubmitChanges();
			global::scms.CacheManager.Clear();
			string strError;
			Exception exError;
			if (!LoadPageModuleInstances(out strError, out exError))
			{
				throw new Exception(strError, exError);
			}
		}

		protected void Delete(object objSender, CommandEventArgs args)
		{
			int nModuleId = int.Parse((string)args.CommandArgument);

			global::scms.data.ScmsDataContext dc = new global::scms.data.ScmsDataContext();

			int? nModuleInstanceId = null;
			scms.data.scms_page_plugin_module pagePluginModule = null;
			if (nPageId.HasValue)
			{
				pagePluginModule = (from ppmi in dc.scms_page_plugin_modules
														where ppmi.id == nModuleId
														select ppmi).Single();
				nModuleInstanceId = pagePluginModule.instanceId;
			}

			scms.data.scms_template_plugin_module templatePluginModule = null;
			if (nTemplateId.HasValue)
			{
				templatePluginModule = (from tpmi in dc.scms_template_plugin_modules
																where tpmi.id == nModuleId
																select tpmi).Single();
				nModuleInstanceId = templatePluginModule.instanceId;
			}


			// find all uses of this module
			bool bInUse = false;
			var pageModulesInUse = from pm in dc.scms_page_plugin_modules
														 where pm.instanceId == nModuleInstanceId.Value
														 where pm.deleted == false
														 select pm;
			if (nPageId.HasValue)
			{
				pageModulesInUse = pageModulesInUse.Where(p => p.pageId != nPageId.Value);
			}
			if (pageModulesInUse.Count() > 0)
			{
				bInUse = true;
			}

			if (!bInUse)
			{
				var templateModulesInUse = from tm in dc.scms_template_plugin_modules
																	 where tm.instanceId == nModuleInstanceId.Value
																	 where tm.deleted == false
																	 select tm;
				if (nTemplateId.HasValue)
				{
					templateModulesInUse = templateModulesInUse.Where(t => t.templateId != nTemplateId.Value);
				}
				if (templateModulesInUse.Count() > 0)
				{
					bInUse = true;
				}
			}



			if (nPageId.HasValue)
			{
				var modules = (from pm in dc.scms_page_plugin_modules
											 join pmi in dc.scms_plugin_module_instances on pm.instanceId equals pmi.id
											 where pm.deleted == false
											 where pmi.deleted == false
											 where pm.id == nModuleId
											 select new { pm, pmi }).Single();


				if (modules.pm.owner)
				{
					if (bInUse)
					{
						statusMessage.ShowFailure("Unable to delete module, it is shared by other pages and/or templates");
					}
					else
					{
						modules.pmi.deleted = true;
						modules.pm.deleted = true;
					}
				}
				else
				{
					modules.pm.deleted = true;
				}
			}
			else
			{
				var modules = (from tm in dc.scms_template_plugin_modules
											 join pmi in dc.scms_plugin_module_instances on tm.instanceId equals pmi.id
											 where tm.deleted == false
											 where pmi.deleted == false
											 where tm.id == nModuleId
											 select new { tm, pmi }).Single();

				if (modules.tm.owner)
				{
					if (bInUse)
					{
						statusMessage.ShowFailure("Unable to delete module, it is shared by other pages and/or templates");
					}
					else
					{
						modules.pmi.deleted = true;
						modules.tm.deleted = true;
					}
				}
				else
				{
					modules.tm.deleted = true;
				}

			}

			dc.SubmitChanges();
			global::scms.CacheManager.Clear();

			string strError;
			Exception exError;
			if (!LoadPageModuleInstances(out strError, out exError))
			{
				throw new Exception(strError, exError);
			}
		}
	}

	public abstract class SPluginModuleInstance
	{
		protected int nSiteId;
		public int SiteId
		{
			get { return nSiteId; }
			set { nSiteId = value; }
		}
		protected int nId;
		public int Id
		{
			get { return nId; }
			set { nId = value; }
		}
		protected int nPluginModuleInstanceId;
		public int PluginModuleInstanceId
		{
			get { return nPluginModuleInstanceId; }
			set { nPluginModuleInstanceId = value; }
		}

		protected string name;
		public string Name
		{
			get { return name; }
			set { name = value; }
		}

		protected string pluginApplication;
		public string PluginApplication
		{
			get { return pluginApplication; }
			set { pluginApplication = value; }
		}

		protected string pluginModule;
		public string PluginModule
		{
			get { return pluginModule; }
			set { pluginModule = value; }
		}

		protected string placeHolder;
		public string PlaceHolder
		{
			get { return placeHolder; }
			set { placeHolder = value; }
		}

		protected int ordinal;
		public int Ordinal
		{
			get { return ordinal; }
			set { ordinal = value; }
		}

		protected bool owner = false;
		public bool Owner
		{
			get { return owner; }
			set { owner = value; }
		}

		abstract public string EditUrl
		{
			get;
		}
	}

	public class SPagePluginModuleInstance : SPluginModuleInstance
	{
		protected int? nPageId = null;
		public int? PageId
		{
			get { return nPageId; }
			set { nPageId = value; }
		}

		protected bool bOverrideTemplate = false;
		public bool OverrideTemplate
		{
			get { return bOverrideTemplate; }
			set { bOverrideTemplate = value; }
		}

		public override string EditUrl
		{
			get { return string.Format("/scms/admin/module.aspx?pmid={0}", nId); }
		}
	}

	public class STemplatePluginModuleInstance : SPluginModuleInstance
	{
		protected int? nTemplateId = null;
		public int? TemplateId
		{
			get { return nTemplateId; }
			set { nTemplateId = value; }
		}

		public override string EditUrl
		{
			get { return string.Format("/scms/admin/module.aspx?tmid={0}", nId); }
		}

	}


}
/*
bool bOwner = false;
                        if ((ppmi != null) && (ppmi.Owner))
                        {
                            bOwner = true;
                        }
                        else
                        {
                            if ((tpmi != null) && (tpmi.Owner))
                            {
                                bOwner = true;
                            }
                        }

                        if (!bOwner)
                        {
                            ImageButton btnDelete = args.Item.FindControl("btnDelete") as ImageButton;
                            if (btnDelete != null)
                            {
                                btnDelete.ImageUrl = "/scms/client/images/action_delete_disabled.gif";
                                btnDelete.Enabled = false;
                                btnDelete.ToolTip = "Cannot delete this module, not owner";
                            }
                        }
*/