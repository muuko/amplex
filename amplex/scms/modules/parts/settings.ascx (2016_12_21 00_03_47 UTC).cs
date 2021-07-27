using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace scms.modules.parts
{
  public partial class settings : scms.RootControl
  {
		protected void Page_Load(object sender, EventArgs e)
    {
      if (!IsPostBack)
      {
				LoadSettings();
      }
    }

    protected void LoadSettings()
    {
			try
      {
				if (!SiteId.HasValue)
              throw new Exception("unexpected no value for siteid");

				scms.data.ScmsDataContext dc = new scms.data.ScmsDataContext();
				scms.data.scms_plugin_application app = (from a in dc.scms_plugin_applications
																								 where a.name == "parts"
																								 select a).First();

				scms.data.scms_plugin_module module = (from m in dc.scms_plugin_modules
																							 where m.pluginAppId == app.id
																							 where m.name == "part"
																							 select m).First();

				pagePluginModuleInstanceSelector.SiteId = SiteId;
				pagePluginModuleInstanceSelector.PluginApplicationId = app.id;
				pagePluginModuleInstanceSelector.PluginModuleId = module.id;

				amplex.scms.modules.parts.classes.partsDataContext partsDc = new amplex.scms.modules.parts.classes.partsDataContext();
				amplex.scms.modules.parts.classes.cat_setting settings = (from s in partsDc.cat_settings
																																 where s.siteId == SiteId.Value
																																 select s).FirstOrDefault();
				if (settings != null)
				{
					pagePluginModuleInstanceSelector.PagePluginModuleInstanceId = settings.searchResultsPageModuleInstanceId;
				}
				else
				{
					pagePluginModuleInstanceSelector.PagePluginModuleInstanceId = null;
				}
      }
      catch (Exception ex)
      {
          ScmsEvent.Raise("failed loading settings", this, ex);
          statusMessage.ShowFailure("Failed loading parts settings");
      }
    }

    protected void btnSave_Click(object sender, EventArgs args)
    {
      try
      {
				amplex.scms.modules.parts.classes.partsDataContext partsDc = new amplex.scms.modules.parts.classes.partsDataContext();
				amplex.scms.modules.parts.classes.cat_setting settings = (from s in partsDc.cat_settings
																																	where s.siteId == SiteId.Value
																																	select s).FirstOrDefault();
				if (settings == null)
				{
					settings = new amplex.scms.modules.parts.classes.cat_setting();
					partsDc.cat_settings.InsertOnSubmit(settings);
					settings.siteId = SiteId.Value;
				}
				settings.searchResultsPageId = pagePluginModuleInstanceSelector.PageId;
				settings.searchResultsPageModuleInstanceId = pagePluginModuleInstanceSelector.PagePluginModuleInstanceId;
				partsDc.SubmitChanges();

				statusMessage.ShowSuccess("Settings updated");
      }
      catch (Exception ex)
      {
          ScmsEvent.Raise("failed saving parts settings", this, ex);
          statusMessage.ShowFailure("Failed saving parts settings");
      }
    }

		protected void menu_Click(object sender, MenuEventArgs args)
		{
			switch (args.Item.Value)
			{
				case "general":
					mvSettings.SetActiveView(viewGeneral);
					break;

				case "sizes":
					mvSettings.SetActiveView(viewSizes);
					break;
			}
		}
	}

	
}