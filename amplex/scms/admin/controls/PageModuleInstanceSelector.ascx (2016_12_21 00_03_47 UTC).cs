using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace scms.admin
{
	[ValidationPropertyAttribute("PagePluginModuleInstanceId")]
  public partial class PageModuleInstanceSelector : System.Web.UI.UserControl
  {
    public delegate void ModuleSelectionChangedDelegate(int? nPagePluginModuleInstanceId);
    public ModuleSelectionChangedDelegate OnModuleSelectionChanged = null;

    public int? SiteId 
    {
      get;
      set;
    }

    // restrict modules available to this type
    public int? PluginApplicationId
    {
      get;
      set;
    }

    public int? PluginModuleId
    {
      get;
      set;
    }

    protected bool bDisableEvents = false;

		public int? PageId
		{
			get { return pageSelectorShare.PageId; }
		}

		// the selected module
		public int? PagePluginModuleInstanceId
		{
			get 
      {
				return (int?)ViewState["PagePluginModuleInstanceId"];
			}

			set 
      {
				bDisableEvents = true;
				SetPagePluginModuleInstanceId(value);
				ViewState["PagePluginModuleInstanceId"] = value;
				bDisableEvents = false;
			}
		}

		protected void SetPagePluginModuleInstanceId(int? nPagePluginModuleInstanceId)
		{
			bool bModuleSelected = false;

			if (nPagePluginModuleInstanceId.HasValue)
			{
				try
				{
					global::scms.data.ScmsDataContext dc = new global::scms.data.ScmsDataContext();

					var x = (from ppmi in dc.scms_page_plugin_modules
									 where ppmi.deleted == false
									 where ppmi.id == nPagePluginModuleInstanceId.Value
									 select ppmi).FirstOrDefault();

					// locate page containing module
					var instances = (from ppmi in dc.scms_page_plugin_modules
													 where ppmi.deleted == false
													 where ppmi.id == nPagePluginModuleInstanceId.Value
													 
													 join pmi in dc.scms_plugin_module_instances
													 on ppmi.instanceId equals pmi.id
													 where pmi.deleted == false

													 from pm in dc.scms_plugin_modules
													 where pm.pluginAppId == pmi.pluginAppId
													 where pm.id == pmi.pluginModuleId
													 
													 select new { ppmi, pmi, pm }).FirstOrDefault();
					if (instances != null)
					{
						if (PluginApplicationId.HasValue && PluginModuleId.HasValue)
						{
							if ((instances.pm.pluginAppId != PluginApplicationId.Value) || (instances.pm.id != PluginModuleId.Value))
							{
								instances = null;
							}
						}
					}

					if (instances != null)
					{
						pageSelectorShare.PageId = instances.ppmi.pageId;
						LoadShareModules();

						bModuleSelected = true;
					}
				}
				catch (Exception ex)
				{
					string strMessage = string.Format("Exception thrown while locating page plugin module instance '{0}'.", nPagePluginModuleInstanceId.Value);
					ScmsEvent.Raise(strMessage, this, ex);
				}

				if (!bModuleSelected)
				{
					// start over
					SetPagePluginModuleInstanceId(null);
				}
			}
		}

		protected void Page_Init(object sender, EventArgs e)
		{
			pageSelectorShare.OnPageSelectionChanged += OnSharePageSelectionChanged;
			// OnSharePageSelectionChanged
		}

		protected void Page_Load(object sender, EventArgs e)
    {
			pageSelectorShare.SiteId = SiteId;
			pageSelectorShare.Inititialize();
    }


    protected void OnSharePageSelectionChanged(int? nPageId)
    {
				ViewState["PagePluginModuleInstanceId"] = null;
        LoadShareModules();
    }

		protected void LoadShareModules()
    {
      try
      {
				scms.data.ScmsDataContext dc = new scms.data.ScmsDataContext();
				System.Collections.Generic.Dictionary<string, int> modules = new System.Collections.Generic.Dictionary<string, int>();
        int? nPageId = pageSelectorShare.PageId;
        if (nPageId.HasValue)
        {
          var pageModules = from ppmi in dc.scms_page_plugin_modules
                            where ppmi.pageId == nPageId.Value
                            where ppmi.deleted == false
                            
														join i in dc.scms_plugin_module_instances on ppmi.instanceId equals i.id
                            where i.deleted == false

                            select new { ppmi.name, ppmi.id, i.pluginAppId, i.pluginModuleId };

          if (PluginApplicationId.HasValue && PluginModuleId.HasValue)
          {
              pageModules = pageModules.Where(i => i.pluginAppId == PluginApplicationId.Value && i.pluginModuleId == PluginModuleId.Value);
          }

          ddlShareModule.DataSource = pageModules;
        }

        ddlShareModule.DataTextField = "name";
        ddlShareModule.DataValueField = "id";
        ddlShareModule.DataBind();

        ddlShareModule_SelectedIndexChanged(null, null);
      }
      catch (Exception ex)
      {
        string strMessage = string.Format("Failed loading share module");
        ScmsEvent.Raise(strMessage, this, ex);
      }
    }

    protected void ddlShareModule_SelectedIndexChanged(object sender, EventArgs args)
    {
      string strSelectedValue = ddlShareModule.SelectedValue;
      int? nSelectedModule = null;
      if (!string.IsNullOrEmpty(strSelectedValue))
      {
        nSelectedModule = int.Parse(strSelectedValue);
      }

			ViewState["PagePluginModuleInstanceId"] = nSelectedModule;

      if (!bDisableEvents)
      {
        if (OnModuleSelectionChanged != null)
        {
          OnModuleSelectionChanged(nSelectedModule);
        }
      }
    }
  }
}
 