using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace scms.modules.forms.autoresponder
{
    public partial class formsettings : System.Web.UI.UserControl, scms.modules.forms.IFormSubmissionEventHandlerSettings
    {
        /*
        public delegate void SettingsSavedDelegate();
        public delegate void SettingsCancelledDelegate();
        public SettingsSavedDelegate OnSettingsSaved = null;
        public SettingsCancelledDelegate OnSettingsCancelled = null;
        */


        public int? SiteId
        {
            get;
            set;
        }

        public int? FormId
        {
            get;
            set;
        }

        public int? EventHandlerId
        {
            get;
            set;
        }

        scms.modules.forms.SettingsSavedDelegate settingsSavedDelegate = null;
        scms.modules.forms.SettingsCancelledDelegate settingsCancelledDelegate = null;

        public void SetDelegates(scms.modules.forms.SettingsSavedDelegate settingsSavedDelegate, scms.modules.forms.SettingsCancelledDelegate settingsCancelledDelegate)
        {
            this.settingsSavedDelegate = settingsSavedDelegate;
            this.settingsCancelledDelegate = settingsCancelledDelegate;
        }
/*
        protected int? SpecialApplicationId
        {
            get { return (int?)ViewState["SpecialApplicationId"]; }
            set { ViewState["SpecialApplicationId"] = value; }
        }


        protected void Page_Init(object sender, EventArgs args)
        {
            pageSelector.SiteId = SiteId;
            pageSelector.Inititialize();

            int? nSpecialApplicationId = SpecialApplicationId;

            if (!nSpecialApplicationId.HasValue)
            {
                try
                {
                    scms.data.ScmsDataContext dc = new scms.data.ScmsDataContext();
                    LoadTemplates(dc);
                    nSpecialApplicationId = (from a in dc.scms_plugin_applications
                                                where a.name == "special"
                                                select a.id).Single();

                    ViewState["SpecialApplicationId"] = nSpecialApplicationId;

                }
                catch (Exception ex)
                {
                    string strMessage = "Exception thrown while looking up special application module";
                    ScmsEvent.Raise(strMessage, this, ex);
                }
            }

            // moduleSelector.PluginApplicationId = SpecialApplicationId;
            // moduleSelector.PluginModuleId = SpecialModuleId;
        }
*/
        protected void Page_Load(object sender, EventArgs e)
        {
            LoadTinyMceScript();
        }

        protected void GetFields(out scms.data.scms_form_field[] aFields)
        {
            aFields = null;
            try
            {
                scms.data.ScmsDataContext dc = new scms.data.ScmsDataContext();
                var fields = from f in dc.scms_form_fields
                             where f.formid == FormId.Value
                             where f.deleted == false
                             orderby f.name
                             select f;
                aFields = fields.ToArray();
            }
            catch (Exception ex)
            {
                string strMessage = "Failed getting form fields.";
                ScmsEvent.Raise(strMessage, this, ex);
            }
        }

        protected void PopulateFormFields()
        {
            try
            {
                scms.data.scms_form_field[] aFields;
                GetFields(out aFields);
                PopulateFormFields(ddlEmailAddress, aFields);
            }
            catch (Exception ex)
            {
                string strMessage = "Failed populating form fields.";
                ScmsEvent.Raise(strMessage, this, ex);
            }
        }


        protected void PopulateFormFields(DropDownList ddlFields, scms.data.scms_form_field[] aFields)
        {
            ddlFields.Items.Clear();
            ddlFields.Items.Add("");
            ddlFields.AppendDataBoundItems = true;
            ddlFields.DataTextField = "name";
            ddlFields.DataValueField = "id";
            ddlFields.DataSource = aFields;
            ddlFields.DataBind();
        }


        public bool LoadSettings()
        {
            bool bSuccess = false;
            
            try
            {
                PopulateFormFields();

                if (EventHandlerId.HasValue)
                {
                    scms.data.ScmsDataContext dcScms = new scms.data.ScmsDataContext();
                    scms.data.scms_form_eventhandler eventHandler =
                        (from eh in dcScms.scms_form_eventhandlers
                         where eh.id == EventHandlerId.Value
                         select eh).FirstOrDefault();

                    scms.data.scms_forms_autoresponder_eventhandler arEventHandler =
                        (from areh in dcScms.scms_forms_autoresponder_eventhandlers
                         where areh.eventHandlerId == EventHandlerId.Value
                         select areh).FirstOrDefault();

                    scms.data.scms_form_field[] aFields;
                    GetFields(out aFields);
                    if (arEventHandler != null)
                    {
                        ddlEmailAddress.SelectedValue = arEventHandler.emailAddressFieldId.ToString();
                        txtFromEmailAddress.Text = arEventHandler.from;
                        txtCc.Text = arEventHandler.cc;
                        txtBcc.Text = arEventHandler.bcc;
                        txtSubject.Text = UpdateTextWithFieldNames(arEventHandler.subject, aFields);

                        string strText = UpdateTextWithFieldNames(arEventHandler.body, aFields);
                        txtBodyHtml.Value = strText;
                        txtBodyText.Value = strText;
                        rblMode.SelectedValue = arEventHandler.bodyIsHtml ? "html" : "text";
                    }
                    else
                    {
                        ddlEmailAddress.ClearSelection();
                        txtFromEmailAddress.Text = null;
                        txtCc.Text = null;
                        txtBcc.Text = null;
                        txtSubject.Text = null;
                        txtBodyHtml.Value = null;
                        txtBodyText.Value = null;
                        rblMode.SelectedValue = "html";
                    }

                    bSuccess = true;
                }
            }
            catch (Exception ex)
            {
                ScmsEvent.Raise("Exception thrown while loading settings", this, ex);
            }
            

            rblMode_SelectedIndexChanged(null, null);

            return bSuccess;
        }

        protected string UpdateTextWithFieldIds(string strText, scms.data.scms_form_field[] aFields)
        {
            string strResponse = strText;
            if (!string.IsNullOrEmpty(strResponse))
            {
                foreach (scms.data.scms_form_field field in aFields)
                {
                    string strFieldByName = string.Format("##{0}##", field.name);
                    string strFieldByNameUpper = string.Format("##{0}##", field.name.ToUpper());
                    string strFieldByNameLower = string.Format("##{0}##", field.name.ToLower());
                    string strFieldById = string.Format("##{0}##", field.id);

                    strResponse = strResponse.Replace(strFieldByName, strFieldById);
                    strResponse = strResponse.Replace(strFieldByNameUpper, strFieldById);
                    strResponse = strResponse.Replace(strFieldByNameLower, strFieldById);
                }
            }

            return strResponse;
        }

        protected string UpdateTextWithFieldNames(string strText, scms.data.scms_form_field[] aFields)
        {
            string strResponse = strText;

            if (!string.IsNullOrEmpty(strResponse))
            {
                foreach (scms.data.scms_form_field field in aFields)
                {
                    string strFieldByName = string.Format("##{0}##", field.name);
                    string strFieldById = string.Format("##{0}##", field.id);

                    strResponse = strResponse.Replace(strFieldById, strFieldByName);
                }
            }

            return strResponse;
        }

        protected void btnSaveSettings_Clicked(object sender, EventArgs args)
        {
            Page.Validate("autoresponder");
            if (Page.IsValid)
            {
                try
                {
                    scms.data.ScmsDataContext dcScms = new scms.data.ScmsDataContext();
                    
                    if (EventHandlerId.HasValue)
                    {
                        scms.data.scms_form_eventhandler eventHandler =
                            (from eh in dcScms.scms_form_eventhandlers
                             where eh.id == EventHandlerId.Value
                             select eh).FirstOrDefault();

                        scms.data.scms_forms_autoresponder_eventhandler arEventHandler =
                            (from areh in dcScms.scms_forms_autoresponder_eventhandlers
                             where areh.eventHandlerId == EventHandlerId.Value
                             select areh).FirstOrDefault();

                        if (arEventHandler == null)
                        {
                            arEventHandler = new scms.data.scms_forms_autoresponder_eventhandler();
                            arEventHandler.eventHandlerId = EventHandlerId.Value;
                            arEventHandler.formid = FormId.Value;
                            dcScms.scms_forms_autoresponder_eventhandlers.InsertOnSubmit(arEventHandler);
                        }

                        arEventHandler.emailAddressFieldId = int.Parse(ddlEmailAddress.SelectedValue);

                        scms.data.scms_form_field[] aFields = null;
                        GetFields(out aFields);
                        arEventHandler.from = txtFromEmailAddress.Text.Trim();
                        arEventHandler.cc = UpdateTextWithFieldIds( txtCc.Text.Trim(), aFields );
                        arEventHandler.bcc = UpdateTextWithFieldIds( txtBcc.Text.Trim(), aFields );
                        arEventHandler.subject = UpdateTextWithFieldIds(txtSubject.Text.Trim(), aFields );

                        if (rblMode.SelectedValue == "html")
                        {
                            arEventHandler.bodyIsHtml = true;
                            arEventHandler.body = UpdateTextWithFieldIds(txtBodyHtml.Value, aFields);
                        }
                        else
                        {
                            arEventHandler.bodyIsHtml = false;
                            arEventHandler.body = UpdateTextWithFieldIds(txtBodyText.Value, aFields);
                        }

                        dcScms.SubmitChanges();



                        if (settingsSavedDelegate != null)
                        {
                            settingsSavedDelegate();
                        }
                     
                    }
                }
                catch (Exception ex)
                {
                    ScmsEvent.Raise("Exception thrown while loading settings", this, ex);
                    statusMessage.ShowFailure("Failed saving settings");
                }
            }
         }

        protected void btnCancel_Clicked(object sender, EventArgs args)
        {
            if (settingsCancelledDelegate != null)
            {
                settingsCancelledDelegate();
            }
        }

        protected void rblMode_SelectedIndexChanged(object sender, EventArgs args)
        {
            switch (rblMode.SelectedValue)
            {
                case "html":
                    if (sender != null)
                    {
                        txtBodyHtml.Value = txtBodyText.Value;
                    }
                    mvBody.SetActiveView(viewBodyHtml);
                    break;

                case "text":
                    if (sender != null)
                    {
                        txtBodyText.Value = txtBodyHtml.Value;
                    }
                    mvBody.SetActiveView(viewBodyText);
                    break;
            }
        }

        protected void cvBody_ServerValidate(object sender, ServerValidateEventArgs args)
        {
            bool bValid = false;
            switch (rblMode.SelectedValue)
            {
                case "html":
                    bValid = !string.IsNullOrEmpty(txtBodyHtml.Value);
                    break;

                case "text":
                    bValid = !string.IsNullOrEmpty(txtBodyText.Value);
                    break;
            }
            args.IsValid = bValid;
        }

        protected void LoadTinyMceScript()
        {
            // var ed = tinyMCE.Editor('txtContent', { 
            /*
                        string strScriptFormatworkds = @"
                        tinyMCE.init({{ 
		

                        // http://tinymce.moxiecode.com/punbb/viewtopic.php?id=642
                        remove_script_host : false,
                        convert_urls : false,

		 
                        // General options
                        width : ""800"",
                        mode : ""textareas"",
                        theme : ""advanced"",
                        plugins : ""safari,pagebreak,style,layer,table,save,advhr,advimage,advlink,emotions,iespell,inlinepopups,insertdatetime,preview,media,searchreplace,print,contextmenu,paste,directionality,fullscreen,noneditable,visualchars,nonbreaking,xhtmlxtras,template,wordcount"",

                        // Theme options
                        theme_advanced_buttons1 : ""save,newdocument,|,bold,italic,underline,strikethrough,|,justifyleft,justifycenter,justifyright,justifyfull,styleselect,formatselect,fontselect,fontsizeselect"",
                        theme_advanced_buttons2 : ""cut,copy,paste,pastetext,pasteword,|,search,replace,|,bullist,numlist,|,outdent,indent,blockquote,|,undo,redo,|,link,unlink,anchor,image,cleanup,help,code,|,insertdate,inserttime,preview,|,forecolor,backcolor"",
                        theme_advanced_buttons3 : ""tablecontrols,|,hr,removeformat,visualaid,|,sub,sup,|,charmap,emotions,iespell,media,advhr,|,print,|,ltr,rtl,|,fullscreen"",
                        theme_advanced_buttons4 : ""insertlayer,moveforward,movebackward,absolute,|,styleprops,|,cite,abbr,acronym,del,ins,attribs,|,visualchars,nonbreaking,template,pagebreak"",
                        theme_advanced_toolbar_location : ""top"",
                        theme_advanced_toolbar_align : ""left"",
                        theme_advanced_statusbar_location : ""bottom"",
                        theme_advanced_resizing : true,

                        // Example content CSS (should be your site CSS)
                        content_css : ""css/content.css"",

                        // Drop lists for link/image/media/template dialogs
                        template_external_list_url : ""lists/template_list.js"",
                        external_link_list_url : ""lists/link_list.js"",
                        external_image_list_url : ""lists/image_list.js"",
                        media_external_list_url : ""lists/media_list.js"",

                        // Replace values for the template plugin
                        template_replace_values : {{
                            username : ""Some User"",
                            staffid : ""991234""
                        }}
			
			
                }});";
            */

            // var ed = tinyMCE.Editor('txtContent', { 
            string strScriptFormat = @"
            $(document).ready( function() {{

			if(typeof(tinyMCE) !== 'undefined') 
			{{
				tinyMCE.init({{ 
			

				// http://tinymce.moxiecode.com/punbb/viewtopic.php?id=642
				remove_script_host : false,
				convert_urls : false,

			 
				// General options
				width : ""800"",

				// http://wiki.moxiecode.com/index.php/TinyMCE:Configuration/mode
				mode : ""exact"",
				elements : ""{0}"",

				theme : ""advanced"",
				plugins : ""safari,pagebreak,style,layer,table,save,advhr,advimage,advlink,emotions,iespell,inlinepopups,insertdatetime,preview,media,searchreplace,print,contextmenu,paste,directionality,fullscreen,noneditable,visualchars,nonbreaking,xhtmlxtras,template,wordcount"",

				// Theme options
				theme_advanced_buttons1 : ""save,newdocument,|,bold,italic,underline,strikethrough,|,justifyleft,justifycenter,justifyright,justifyfull,styleselect,formatselect,fontselect,fontsizeselect"",
				theme_advanced_buttons2 : ""cut,copy,paste,pastetext,pasteword,|,search,replace,|,bullist,numlist,|,outdent,indent,blockquote,|,undo,redo,|,link,unlink,anchor,image,cleanup,help,code,|,insertdate,inserttime,preview,|,forecolor,backcolor"",
				theme_advanced_buttons3 : ""tablecontrols,|,hr,removeformat,visualaid,|,sub,sup,|,charmap,emotions,iespell,media,advhr,|,print,|,ltr,rtl,|,fullscreen"",
				theme_advanced_buttons4 : ""insertlayer,moveforward,movebackward,absolute,|,styleprops,|,cite,abbr,acronym,del,ins,attribs,|,visualchars,nonbreaking,template,pagebreak"",
				theme_advanced_toolbar_location : ""top"",
				theme_advanced_toolbar_align : ""left"",
				theme_advanced_statusbar_location : ""bottom"",
				theme_advanced_resizing : true,

				// Example content CSS (should be your site CSS)
				content_css : ""css/content.css"",

				// Drop lists for link/image/media/template dialogs
				template_external_list_url : ""lists/template_list.js"",
				external_link_list_url : ""lists/link_list.js"",
				external_image_list_url : ""lists/image_list.js"",
				media_external_list_url : ""lists/media_list.js"",

				// Replace values for the template plugin
				template_replace_values : {{
					username : ""Some User"",
					staffid : ""991234""
				}}
					
			}});
		}}

}}
);

";

            string strScript = string.Format(strScriptFormat, txtBodyHtml.ClientID);

            string strScriptName = string.Format("forms_ar_tmce_{0}", txtBodyHtml.ClientID);
            this.Page.ClientScript.RegisterStartupScript(typeof(string), strScriptName, strScript, true);
        }
    }

}