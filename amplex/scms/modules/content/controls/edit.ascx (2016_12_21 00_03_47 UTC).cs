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

namespace scms.modules.content
{
    public partial class edit : global::scms.RootControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            LoadTinyMceScript();
            Session["content-edit-site-id"] = this.SiteId;
            if (!IsPostBack)
            {
                DataBind();
                if (ddlVersion.Items.Count > 0)
                {
                    ddlVersion.ClearSelection();
                    ddlVersion.SelectedIndex = 0;
                    ddlVersion_SelectedIndexChanged(null, null);
                }


                rblMode_SelectedIndexChanged(null, null);
            }
        }

        public override void DataBind()
        {
            base.DataBind();
            BindVersions();
        }

        protected void BindVersions()
        {
            ddlVersion.DataSource = null;
            if (ModuleInstanceId.HasValue)
            {
                scms.data.ScmsDataContext dc = new scms.data.ScmsDataContext();
                var contents = from c in dc.scms_contents
                               where c.instanceId == ModuleInstanceId.Value
                               orderby c.dtVersion descending
                               select c;

                ddlVersion.DataTextField = "dtVersion";
                ddlVersion.DataValueField = "id";
                ddlVersion.DataSource = contents;
            }

            ddlVersion.DataBind();
        }

        protected void ddlVersion_SelectedIndexChanged(object sender, EventArgs args)
        {
            int nContentId = int.Parse(ddlVersion.SelectedValue);

            scms.data.ScmsDataContext dc = new scms.data.ScmsDataContext();
            var content = (from c in dc.scms_contents
                           where c.instanceId == ModuleInstanceId.Value
                           where c.id == nContentId
                           select c).Single();

            if (content.literal)
            {
                rblMode.SelectedValue = "literal";
                txtLiteralContent.Value = content.content;
                txtWysiwygContent.Value = string.Empty;
                multiView.SetActiveView(viewLiteral);
            }
            else
            {
                rblMode.SelectedValue = "wysiwyg";
                txtWysiwygContent.Value = content.content;
                txtLiteralContent.Value = string.Empty;
                multiView.SetActiveView(viewWysiwyg);
            }
            rblMode_SelectedIndexChanged(null, null);
        }

        protected void rblMode_SelectedIndexChanged(object sender, EventArgs args)
        {
            switch (rblMode.SelectedValue)
            {
                case "literal":
                    txtWysiwygContent.Style["visibility"] = "none";
                    txtLiteralContent.Style["visibility"] = "visible";
                    if (sender != null)
                    {
                        txtLiteralContent.Value = txtWysiwygContent.Value;
                        txtWysiwygContent.Value = string.Empty;
                    }
                    multiView.SetActiveView(viewLiteral);
                    break;

                case "wysiwyg":
                    txtWysiwygContent.Style["visibility"] = "visible";
                    txtLiteralContent.Style["visibility"] = "none";
                    if (sender != null)
                    {
                        txtWysiwygContent.Value = txtLiteralContent.Value;
                        txtLiteralContent.Value = string.Empty;
                    }
                    multiView.SetActiveView(viewWysiwyg);
                    break;
            }
        }

				protected void btnSave_Click(object sender, EventArgs args)
				{
					if (ModuleInstanceId.HasValue )
					{
						scms.data.ScmsDataContext dc = new scms.data.ScmsDataContext();
						scms.data.scms_content content = new scms.data.scms_content();
						content.instanceId = ModuleInstanceId.Value;
						content.dtVersion = DateTime.Now;

						if (rblMode.SelectedValue == "wysiwyg")
						{
							content.content = txtWysiwygContent.Value;
							content.literal = false;
						}
						else
						{
							content.content = txtLiteralContent.Value;
							content.literal = true;
						}

						dc.scms_contents.InsertOnSubmit(content);
						dc.SubmitChanges();

						if (PageId.HasValue)
						{
							scms.search.search search = new scms.search.search();
							search.IndexModule(PageId.Value, content.instanceId, content.content, true);
						}

						scms.CacheManager.Clear();
						BindVersions();
						ddlVersion.ClearSelection();
						ddlVersion.SelectedIndex = 0;
						ddlVersion_SelectedIndexChanged(null, null);

						statusMessage.ShowSuccess("Content saved");
					}
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

";

            string strScript = string.Format(strScriptFormat, txtWysiwygContent.ClientID);

            this.Page.ClientScript.RegisterStartupScript(typeof(string), "xxtmce", strScript, true);
        }
    }
}