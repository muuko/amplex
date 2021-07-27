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

namespace scms.modules.forms.form
{
    public partial class edit : global::scms.RootControl
    {
        protected void OnFieldDeleted(int nFieldId)
        {
            panelEditField.Visible = false;
        }

        protected void OnFieldSelected(int nFieldId)
        {
            editField.FieldId = nFieldId;
            panelEditField.Visible = true;
        }

        protected void OnFieldSaved(int? nFieldId)
        {
            panelEditField.Visible = false;
            fields.LoadFields();
        }

        protected void OnFieldEditCancelled(int? nFieldId)
        {
            panelEditField.Visible = false;
        }

        protected void OnFormUpdated()
        {
            LoadFormsDropDown();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            editForm.OnFormUpdated += OnFormUpdated;
            fields.OnFieldSelected += OnFieldSelected;
            fields.OnFieldDeleted += OnFieldDeleted;
            editField.OnSaved += OnFieldSaved;
            editField.OnCancelled += OnFieldEditCancelled;

            if (!IsPostBack)
            {
                editForm.SiteId = SiteId;
                events.SiteId = SiteId;

                LoadFormsDropDown();

                scms.data.ScmsDataContext dc = new scms.data.ScmsDataContext();
                var form = (from fi in dc.scms_form_instances
                            join f in dc.scms_forms on fi.formid equals f.id
                            where fi.instanceId == ModuleInstanceId.Value
                            where f.deleted == false
                            orderby f.name
                            select f).FirstOrDefault();
                if (form != null)
                {
                    ddlForm.SelectedValue = form.id.ToString();
                }

                ddlForm_SelectedIndexChanged(null, null);
                panelEditField.Visible = false;

                ShowResponses();
            }
        }

        protected void LoadFormsDropDown()
        {
            string strSelectedValue = ddlForm.SelectedValue;
            ddlForm.Items.Clear();
            ddlForm.AppendDataBoundItems = true;
            ddlForm.Items.Add(new ListItem("(select)", "-1"));

            scms.data.ScmsDataContext dc = new scms.data.ScmsDataContext();
            var forms = from f in dc.scms_forms
                        where f.deleted == false
                        where f.siteid == SiteId.Value
                        orderby f.name
                        select new { f.name, f.id };
            ddlForm.DataTextField = "name";
            ddlForm.DataValueField = "id";
            ddlForm.DataSource = forms;
            ddlForm.DataBind();

            if (!string.IsNullOrEmpty(strSelectedValue))
            {
                ddlForm.SelectedValue = strSelectedValue;
            }
        }

        protected void ddlForm_SelectedIndexChanged(object sender, EventArgs e)
        {
            txtNewForm.Text = string.Empty;
            int nFormId = int.Parse(ddlForm.SelectedValue);
            if (sender != null)
            {

                if (nFormId >= 0)
                {
                    scms.data.ScmsDataContext dc = new scms.data.ScmsDataContext();
                    var formInstance = (from fi in dc.scms_form_instances
                                        where fi.instanceId == ModuleInstanceId
                                        select fi).FirstOrDefault();
                    if (formInstance == null)
                    {
                        formInstance = new scms.data.scms_form_instance();
                        formInstance.instanceId = ModuleInstanceId.Value;
                        dc.scms_form_instances.InsertOnSubmit(formInstance);
                    }
                    formInstance.formid = nFormId;
                    dc.SubmitChanges();


                }
            }
            editForm.FormId = nFormId > 0 ? (int?)nFormId : null;
            if (nFormId > 0)
            {
                panelForm.Visible = true;
                panelFields.Visible = true;
                fields.FormId = nFormId;
                responses.FormId = nFormId;
                events.FormId = nFormId;

            }
            else
            {
                panelForm.Visible = false;
                panelFields.Visible = false;
                fields.FormId = null;
                responses.FormId = null;
                events.FormId = null;
            }

            if (multiView.GetActiveView() == viewResponses)
            {
                bool bForceShowList = sender != null;
                responses.LoadResponses(bForceShowList);
            }
        }

        protected void cvNewFormUnique_OnServerValidate(object sender, ServerValidateEventArgs args)
        {
            string strName = txtNewForm.Text.Trim();
            if (!string.IsNullOrEmpty(strName))
            {
                scms.data.ScmsDataContext dc = new scms.data.ScmsDataContext();
                var existing = (from f in dc.scms_forms
                                where f.siteid == SiteId.Value
                                where f.deleted == false
                                where f.name == strName
                                select f).FirstOrDefault();
                if (existing != null)
                {
                    args.IsValid = false;
                }
            }
        }

        protected void btnNewForm_OnClick(object sender, EventArgs args)
        {
            if (Page.IsValid)
            {
                // insert form 
                scms.data.ScmsDataContext dc = new scms.data.ScmsDataContext();
                dc.Connection.Open();
                System.Data.Common.DbTransaction transaction = dc.Connection.BeginTransaction();
                dc.Transaction = transaction;

                scms.data.scms_form form = new scms.data.scms_form();
                form.siteid = SiteId.Value;
                form.thankYouPageId = null;
                form.name = txtNewForm.Text.Trim();
                form.generateForm = true;
                form.generationType = "table";
                form.validateDummyFields = true;
                form.validateReferrer = true;
                form.validateSession = true;
                form.validateReCaptcha = false;
                form.notify = false;
                form.post = false;
                dc.scms_forms.InsertOnSubmit(form);
                dc.SubmitChanges();

                scms.data.scms_form_instance formInstance = (from fi in dc.scms_form_instances
                                                             where fi.instanceId == ModuleInstanceId
                                                             select fi).FirstOrDefault();

                if (formInstance == null)
                {
                    formInstance = new scms.data.scms_form_instance();
                    formInstance.instanceId = ModuleInstanceId.Value;
                    dc.scms_form_instances.InsertOnSubmit(formInstance);
                }
                formInstance.formid = form.id;

                dc.SubmitChanges();

                transaction.Commit();

                LoadFormsDropDown();
                ddlForm.SelectedValue = form.id.ToString();
                ddlForm_SelectedIndexChanged(null, null);
            }
        }

        protected void btnNewField_Click(object sender, EventArgs args)
        {
            if (Page.IsValid)
            {
                try
                {
                    string strLabel = txtNewFieldLabel.Text.Trim();
                    string strOriginalName = string.Empty;
                    foreach (char ch in strLabel)
                    {
                        bool bValid = false;
                        if (char.IsLetterOrDigit(ch))
                        {
                            bValid = true;
                        }
                        else
                        {
                            if (ch == '_')
                            {
                                bValid = true;
                            }
                        }

                        if (bValid)
                        {
                            strOriginalName += ch;
                        }
                    }
                    if ((strOriginalName.Length == 0) || (!char.IsLetter(strOriginalName[0])))
                    {
                        strOriginalName = "I" + strOriginalName;
                    }

                    scms.data.ScmsDataContext dc = new scms.data.ScmsDataContext();

                    var form = (from fi in dc.scms_form_instances
                                join f in dc.scms_forms on fi.formid equals f.id
                                where fi.instanceId == ModuleInstanceId.Value
                                where f.deleted == false
                                orderby f.name
                                select f).FirstOrDefault();

                    if (form != null)
                    {
                        var maxOrdinalField = (from f in dc.scms_form_fields
                                               orderby f.ordinal descending
                                               select f).FirstOrDefault();
                        int nOrdinal = 0;
                        if (maxOrdinalField != null)
                        {
                            nOrdinal = maxOrdinalField.ordinal + 1;
                        }


                        bool bUniqueNameFound = false;
                        string strName = strOriginalName;
                        int nTry = 1;
                        while (!bUniqueNameFound)
                        {
                            var existingField = (from f in dc.scms_form_fields
                                                 where f.formid == form.id
                                                 where f.name == strName
                                                 where f.deleted == false
                                                 select f).FirstOrDefault();

                            if (existingField == null)
                            {
                                bUniqueNameFound = true;
                                break;
                            }
                            else
                            {
                                nTry++;
                                if (nTry > 1000)
                                    throw new Exception("unable to find unique name");
                                strName = string.Format("{0}_{1}", strOriginalName, nTry);
                            }
                        }

                        scms.data.scms_form_field field = new scms.data.scms_form_field();
                        field.formid = form.id;
                        field.name = strName;
                        field.label = strLabel;
                        field.type = "text";
                        field.required = false;
                        field.ordinal = nOrdinal;

                        if (form.post)
                        {
                            field.post = true;
                            field.postId = strOriginalName;
                        }

                        field.fileTypes = "gif, jpg, png, bmp";
                        field.deleted = false;

                        dc.scms_form_fields.InsertOnSubmit(field);
                        dc.SubmitChanges();
                        fields.LoadFields();

                        editField.FieldId = field.id;
                        panelEditField.Visible = true;
                        txtNewFieldLabel.Text = null;
                    }
                }
                catch (System.Threading.ThreadAbortException)
                {
                }
                catch (Exception ex)
                {
                    string strMessage = "Failed creating form field";
                    statusMessage.ShowFailure(strMessage);
                    ScmsEvent.Raise(strMessage, this, ex);
                }
            }
        }

        protected void menuTabs_Click(object sender, MenuEventArgs args)
        {
            switch (menuTabs.SelectedValue.ToLower())
            {
                case "responses":
                    ShowResponses();
                    responses.LoadResponses(true);
                    break;

                case "settings":
                    ShowSettings();
                    break;

                case "fields":
                    ShowFields();
                    break;

                case "events":
                    ShowEvents();
                    break;
            }
        }

        protected void ShowResponses()
        {
            MenuItem menuItem = menuTabs.FindItem("responses");
            menuItem.Selected = true;
            multiView.SetActiveView(viewResponses);
        }

        protected void ShowSettings()
        {
            MenuItem menuItem = menuTabs.FindItem("settings");
            menuItem.Selected = true;
            multiView.SetActiveView(viewSettings);
        }

        protected void ShowFields()
        {
            MenuItem menuItem = menuTabs.FindItem("fields");
            menuItem.Selected = true;
            multiView.SetActiveView(viewFields);
        }

        protected void ShowEvents()
        {
            MenuItem menuItem = menuTabs.FindItem("events");
            menuItem.Selected = true;
            multiView.SetActiveView(viewEvents);
        }




    }
}