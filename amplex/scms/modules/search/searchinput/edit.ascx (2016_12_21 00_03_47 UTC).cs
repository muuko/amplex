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

namespace scms.modules.search.search
{
    public partial class edit : global::scms.RootControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadModule();
                ddlType_SelectedIndexChanged(null, null);
                selectImage.SiteId = this.SiteId;
                pageSearchResultsOverride.SiteId = this.SiteId;
            }
        }

        protected void LoadModule()
        {
            try
            {
                scms.data.ScmsDataContext dc = new scms.data.ScmsDataContext();
                var searchInputModule = (from si in dc.scms_search_input_modules
                                         where si.instanceId == this.ModuleInstanceId.Value
                                         select si).FirstOrDefault();
                if (searchInputModule != null)
                {
                    txtCssClass.Text = searchInputModule.CssClass;
                    txtDefaultText.Text = searchInputModule.DefaultText;
                    txtCssClassTextActive.Text = searchInputModule.CssClassTextInputActive;
                    txtCssClassTextInactive.Text = searchInputModule.CssClassTextInputInactive;
                    txtValidationErrorMessage.Text = searchInputModule.ValidationErrorMessage;
                    txtCssClasButton.Text = searchInputModule.CssClassButtonInput;

                    ddlType.SelectedValue = searchInputModule.UseImageButton ? "image" : "button";
                    if (searchInputModule.UseImageButton)
                    {
                        selectImage.Path = searchInputModule.ButtonImagePath;
                    }
                    else
                    {
                        txtButtonText.Text = searchInputModule.ButtonText;
                    }
                    pageSearchResultsOverride.PageId = searchInputModule.resultsPageId;
                }

            }
            catch (Exception ex)
            {
                string strMessage = "Failed loading settings";
                ScmsEvent.Raise(strMessage, this, ex);
                statusMessage.ShowFailure(strMessage);
            }
        }

        protected void ddlType_SelectedIndexChanged(object sender, EventArgs args)
        {
            switch (ddlType.SelectedValue)
            {
                case "button":
                    mvDetails.SetActiveView(viewButton);
                    break;

                case "image":
                    mvDetails.SetActiveView(viewImage);
                    break;
            }
        }

        protected void btnSave_Click(object sender, EventArgs args)
        {
            try
            {
                scms.data.ScmsDataContext dc = new scms.data.ScmsDataContext();
                scms.data.scms_search_input_module searchInputModule = null;
                searchInputModule = (from sim in dc.scms_search_input_modules
                                     where sim.instanceId == ModuleInstanceId.Value
                                     select sim).FirstOrDefault();
                if (searchInputModule == null)
                {
                    searchInputModule = new scms.data.scms_search_input_module();
                    searchInputModule.instanceId = ModuleInstanceId.Value;
                    dc.scms_search_input_modules.InsertOnSubmit(searchInputModule);
                }
                searchInputModule.CssClass = txtCssClass.Text.Trim();
                searchInputModule.DefaultText = txtDefaultText.Text;
                searchInputModule.CssClassTextInputActive = txtCssClassTextActive.Text.Trim();
                searchInputModule.CssClassTextInputInactive = txtCssClassTextInactive.Text.Trim();
                searchInputModule.ValidationErrorMessage = txtValidationErrorMessage.Text.Trim();
                searchInputModule.CssClassButtonInput = txtCssClasButton.Text.Trim();
                switch (ddlType.SelectedValue)
                {
                    case "button":
                        searchInputModule.UseImageButton = false;
                        searchInputModule.ButtonText = txtButtonText.Text.Trim();
                        searchInputModule.ButtonImagePath = null;
                        break;

                    case "image":
                        searchInputModule.UseImageButton = true;
                        searchInputModule.ButtonText = null;
                        searchInputModule.ButtonImagePath = selectImage.Path;
                        break;
                }
                searchInputModule.resultsPageId = pageSearchResultsOverride.PageId;
                dc.SubmitChanges();
                statusMessage.ShowSuccess("Settings saved");
            }
            catch (Exception ex)
            {
                string strMessage = "Failed saving settings";
                ScmsEvent.Raise(strMessage, this, ex);
                statusMessage.ShowFailure(strMessage);
            }
        }
    }
}