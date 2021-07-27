using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace scms.modules.parts
{
	class partsApplication : IPluginApplication, IPluginSearchApplication
	{
		protected amplex.scms.modules.parts.classes.cat_setting settings = null;
		protected bool bInitialized = false;
		protected int ? nApplicationId = null;

		protected System.Collections.Generic.Dictionary<int, global::scms.data.scms_plugin_module> dictModulesById = null;

		public partsApplication()
		{
		}

		public void Init(int ? nApplicationId)
    {
			this.nApplicationId = nApplicationId;
			
    }

		public int ? GetApplicationId()
		{
			return nApplicationId;
		}

		public void Init()
		{
			amplex.scms.modules.parts.classes.partsDataContext dcParts = new amplex.scms.modules.parts.classes.partsDataContext();
			settings = (from s in dcParts.cat_settings
									where s.searchResultsPageModuleInstanceId != null
									select s).FirstOrDefault();

			global::scms.data.ScmsDataContext dc = new scms.data.ScmsDataContext();
			var modules = (from m in dc.scms_plugin_modules
										 where m.pluginAppId == nApplicationId.Value
										 select m);
			dictModulesById = new Dictionary<int, scms.data.scms_plugin_module>();
			foreach (var module in modules)
			{
				dictModulesById[module.id] = module;
			}

			bInitialized = true;
		}

		public void RebuildIndex(scms.search.search search, scms.data.scms_page page, scms.data.scms_page_plugin_module pagePluginModuleInstance, scms.data.scms_plugin_module_instance pluginModuleInstance)
		{
			if (!bInitialized)
			{
				Init();
			}

			bool bReIndex = false;

			bool bFound = false;
			scms.data.scms_plugin_module pluginModule = null;
			if (pluginModuleInstance != null)
			{
				if (dictModulesById.TryGetValue(pluginModuleInstance.pluginModuleId, out pluginModule))
				{
					if (string.Compare(pluginModule.name, "part", true) == 0)
					{
						bReIndex = true;
					}
				}
			}
			else
			{
				bReIndex = true;
			}

			if (bReIndex)
			{
				// need to rebuild all, regardless of parms
				try
				{
					amplex.scms.modules.parts.classes.partsDataContext dc = new amplex.scms.modules.parts.classes.partsDataContext();
					var lParts = (from cp in dc.cat_parts
												select cp).ToList();
					IndexParts(search, lParts);
				}
				catch (Exception ex)
				{
					scms.ScmsEvent.Raise("failed reading parts", this, ex);
				}
			}
		}

		public void IndexPart(global::scms.search.search search, amplex.scms.modules.parts.classes.cat_part part)
		{
			System.Collections.Generic.List<amplex.scms.modules.parts.classes.cat_part> lParts = new List<amplex.scms.modules.parts.classes.cat_part>();
			lParts.Add(part);
			IndexParts(search, lParts);
		}

		public void IndexParts(global::scms.search.search search, System.Collections.Generic.List<amplex.scms.modules.parts.classes.cat_part> lParts)
		{
			scms.ScmsEvent.Raise("indexing parts", this, null);
			if( !bInitialized )
			{
				Init();
			}

			foreach (var part in lParts)
			{
				string strSearchText = string.Join(",",
					new string[] {
							part.sage_Description1,
							part.sage_Description2,
							part.sage_LongDescription });
				string strQueryString;
				string strTitleOverride;
				string strSummaryOverride;
				GetSearchOverrides(part, out strQueryString, out strTitleOverride, out strSummaryOverride);

				search.IndexModule(
					settings.searchResultsPageId.Value,
					settings.searchResultsPageModuleInstanceId,
					strSearchText,
					false,
					part.id,
					true,
					strQueryString,
					strTitleOverride,
					strSummaryOverride,
					part.imageUrl);
			}
			scms.ScmsEvent.Raise("indexing parts complete", this, null);
		}
	
		protected void GetSearchOverrides(amplex.scms.modules.parts.classes.cat_part part, out string strQueryString, out string strTitleOverride, out string strSummaryOverride)
		{
			strQueryString = string.Format("p={0}", System.Web.HttpUtility.UrlEncode(part.sage_ID));
			strTitleOverride = null;
			if (!string.IsNullOrEmpty(part.sage_Description1))
			{
				if (!string.IsNullOrEmpty(part.sage_Description2))
				{
					strTitleOverride = string.Format("{0} - [{1}]", part.sage_Description1, part.sage_Description2);
				}
				else
				{
					strTitleOverride = part.sage_Description1;
				}
			}
			else
			{
				strTitleOverride = part.sage_Description2;	
			}
			strSummaryOverride = part.sage_LongDescription;
		}
  }
}
