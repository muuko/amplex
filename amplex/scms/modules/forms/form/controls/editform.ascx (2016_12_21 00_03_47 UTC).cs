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

namespace scms.modules.forms.form.controls
{
	public partial class editform : System.Web.UI.UserControl
  {
		public delegate void FormUpdated();
		public FormUpdated OnFormUpdated = null;

    public int? SiteId
    {
      get
      {
				return (int?)ViewState["SiteId"];
      }

      set
      {
        ViewState["SiteId"] = value;
        pageSelectorThankYouPage.SiteId = SiteId;
      }
    }

		public int? FormId
		{
      get
      {
				return (int?)ViewState["FormId"];
      }

      set
      {
        ViewState["FormId"] = value;
        LoadForm();
      }

		}

		public string NotifyBeingEdited
		{
			get { return (string)ViewState["NotifyBeingEdited"]; }
			set { ViewState["NotifyBeingEdited"] = value; }
		}
		
		[Serializable]
		public class NotifyListItem
		{
			public int? id { get; set; }
			public int fieldId { get; set; }
			public string name { get; set; }
			public string value { get; set; }
			public string email { get; set; }
			public override string ToString()
			{
				string strResult = string.Format("id:{0},fieldId:{1},name:{2},value:{3},email:{4}", id, fieldId, name, value, email);
				return strResult;
			}
		}

		protected System.Collections.Generic.List<NotifyListItem> lNotifyByField
		{
			get
			{
				System.Collections.Generic.List<NotifyListItem> l = (System.Collections.Generic.List<NotifyListItem>)ViewState["lNotifyByField"];

				if( l == null )
				{
					l = new System.Collections.Generic.List<NotifyListItem>();
					lNotifyByField = l;
				}
				return l;
			}

			set
			{
				ViewState["lNotifyByField"] = value;
			}
		}

    protected void Page_Load(object sender, EventArgs e)
    {
      if (!IsPostBack)
      {
      }
    }

    protected void LoadForm()
    {
			if (FormId.HasValue)
			{
        scms.data.ScmsDataContext dc = new scms.data.ScmsDataContext();
        var form = (from f in dc.scms_forms
                    where f.deleted == false
                    where f.id == FormId.Value
                    select f).Single();

        txtFormName.Text = form.name;
        pageSelectorThankYouPage.PageId = form.thankYouPageId;
        checkNotify.Checked = form.notify;
        txtNotifyEmailAddress.Text = form.notifyEmail;
        checkGenerate.Checked = form.generateForm;
        ddlGenerationType.SelectedValue = form.generationType;
        txtSubmitText.Text = form.submitText;
        txtCssClassContainer.Text = form.cssClassContainer;
        txtCssClassTable.Text = form.cssClassTable;
        txtCssClassRow.Text = form.cssClassRow;
        txtCssClassLabel.Text = form.cssClassCellLabel;
        txtCssClassValue.Text = form.cssClassCellValue;
        txtCssClassInputRow.Text = form.cssClassInputRow;
        txtManualForm.Text = form.manualFormText;
        checkDummyFields.Checked = form.validateDummyFields;
        checkSession.Checked = form.validateSession;
        checkReferrer.Checked = form.validateReferrer;
        checkRecaptcha.Checked = form.validateReCaptcha;
        checkPost.Checked = form.post;
        txtPostUrl.Text = form.postUrl;

				checkSummaryValidationEnabled.Checked = form.summaryValidationEnabled;
				if( checkSummaryValidationEnabled.Checked )
				{
					txtSummaryValidationCssClass.Text = form.summaryValidationCssClass;
					txtSummaryValidationHeaderText.Text = form.summaryValidationHeaderText;
					ddlSummaryValidationDisplayMode.Text = form.summaryValidationDisplayMode;
					checkSummaryValidationEnableClientScript.Checked = form.summaryValidationEnableClientScript.HasValue && form.summaryValidationEnableClientScript.Value;
					checkSummaryValidationShowSummary.Checked = form.summaryValidationShowSummary.HasValue && form.summaryValidationShowSummary.Value;
					checkSummaryValidationShowMessageBox.Checked = form.summaryValidationShowMessageBox.HasValue && form.summaryValidationShowMessageBox.Value;
				}

				var fields = from f in dc.scms_form_fields
										 where f.formid == form.id
										 orderby f.name
										 select f;
				ddlNotifyByFieldField.DataTextField = "name";
				ddlNotifyByFieldField.DataValueField = "id";
				ddlNotifyByFieldField.DataSource = fields;
				ddlNotifyByFieldField.DataBind();


				lNotifyByField.Clear();
				var notifies = from n in dc.scms_form_notify_by_fields
											 where n.formId == form.id
											 orderby n.id
											 select n;
				foreach (var notify in notifies)
				{
					NotifyListItem notifyListItem = new NotifyListItem();
					notifyListItem.id = notify.id;
					notifyListItem.fieldId = notify.fieldId;
					notifyListItem.name = notify.scms_form_field.name;
					notifyListItem.value = notify.value;
					notifyListItem.email = notify.email;
					lNotifyByField.Add(notifyListItem);
				}

				ReloadNotifications();
				
			}
      else
      {
        txtFormName.Text = null;
        pageSelectorThankYouPage.PageId = null;
        checkNotify.Checked = false;
        txtNotifyEmailAddress.Text = null;
        checkPost.Checked = false;
        txtPostUrl.Text = null;
      }


      EnableControls();
      ddlGenerationType_SelectedIndexChanged(null, null);
    }

		

    protected void checkPost_CheckedChanged(object sender, EventArgs args)
    {
			EnableControls();
    }

    protected void checkGenerate_CheckedChanged(object sender, EventArgs args)
    {
			EnableControls();
    }

    protected void checkNotify_CheckedChanged(object sender, EventArgs args)
    {
			EnableControls();
    }

    protected void EnableControls()
    {
      if (FormId.HasValue)
			{
        txtFormName.Enabled = true;
        pageSelectorThankYouPage.Enabled = true;
        checkNotify.Enabled = true;
        txtNotifyEmailAddress.Enabled = true;
        checkPost.Enabled = true;
        ddlGenerationType.Enabled = true;
        txtSubmitText.Enabled = true;
        txtPostUrl.Enabled = true;

				btnNewNotify.Enabled = true;
				btnSaveNotify.Enabled = true;
				btnCancelNotify.Enabled = true;
				ddlNotifyByFieldField.Enabled = true;
				txtNotifyByFieldEmail.Enabled = true;
				txtNotifyByFieldValue.Enabled = true;

        if (!checkNotify.Checked)
        {
          txtNotifyEmailAddress.Text = string.Empty;
          txtNotifyEmailAddress.Enabled = false;
					btnNewNotify.Enabled = false;
					btnSaveNotify.Enabled = false;
					btnCancelNotify.Enabled = false;
					ddlNotifyByFieldField.Enabled = false;
					txtNotifyByFieldEmail.Enabled = false;
					txtNotifyByFieldValue.Enabled = false;
        }

        if (!checkPost.Checked)
        {
          txtPostUrl.Text = string.Empty;
          txtPostUrl.Enabled = false;
        }

        if (!checkGenerate.Checked)
        {
          ddlGenerationType.Enabled = false;
          txtSubmitText.Enabled = false;
          txtCssClassContainer.Enabled = false;
          txtCssClassTable.Enabled = false;
          txtCssClassRow.Enabled = false;
          txtCssClassLabel.Enabled = false;
          txtCssClassValue.Enabled = false;
          txtCssClassInputRow.Enabled = false;
          txtManualForm.Enabled = false;
        }

				labelSummaryValidationCssClass.Enabled = checkSummaryValidationEnabled.Checked;
				txtSummaryValidationCssClass.Enabled = checkSummaryValidationEnabled.Checked;

				labelSummaryValidationHeaderText.Enabled = checkSummaryValidationEnabled.Checked; 
				txtSummaryValidationHeaderText.Enabled = checkSummaryValidationEnabled.Checked;

				labelSummaryValidationDisplayMode.Enabled = checkSummaryValidationEnabled.Checked;
				ddlSummaryValidationDisplayMode.Enabled = checkSummaryValidationEnabled.Checked;

				labelSummaryValidationEnableClientScript.Enabled = checkSummaryValidationEnabled.Checked;
				checkSummaryValidationEnableClientScript.Enabled = checkSummaryValidationEnabled.Checked;

				labelSummaryValidationShowSummary.Enabled = checkSummaryValidationEnabled.Checked;
				checkSummaryValidationShowSummary.Enabled = checkSummaryValidationEnabled.Checked;

				labelSummaryValidationShowMessageBox.Enabled = checkSummaryValidationEnabled.Checked;
				checkSummaryValidationShowMessageBox.Enabled = checkSummaryValidationEnabled.Checked;
    }
    else
		{
      txtFormName.Enabled = false;
      pageSelectorThankYouPage.Enabled = false;
      checkNotify.Enabled = false;
      txtNotifyEmailAddress.Enabled = false;
      checkPost.Enabled = false;

      txtPostUrl.Enabled = false;

			labelSummaryValidationCssClass.Enabled = false;
			txtSummaryValidationCssClass.Enabled = false;

			labelSummaryValidationHeaderText.Enabled = false;
			txtSummaryValidationHeaderText.Enabled = false;

			labelSummaryValidationDisplayMode.Enabled = false;
			ddlSummaryValidationDisplayMode.Enabled = false;

			labelSummaryValidationEnableClientScript.Enabled = false;
			checkSummaryValidationEnableClientScript.Enabled = false;

			labelSummaryValidationShowSummary.Enabled = false;
			checkSummaryValidationShowSummary.Enabled = false;

			labelSummaryValidationShowMessageBox.Enabled = false;
			checkSummaryValidationShowMessageBox.Enabled = false;
		}
	}

  protected void btnSaveForm_Clicked(object sender, EventArgs args)
	{
    if (Page.IsValid)
		{
	    try
			{
        scms.data.ScmsDataContext dc = new scms.data.ScmsDataContext();
        scms.data.scms_form form = null;
        if (FormId.HasValue)
        {
          form = (from f in dc.scms_forms
                  where f.deleted == false
                  where f.id == FormId.Value
                  select f).Single();
        }
        else
        {
          form = new scms.data.scms_form();
          dc.scms_forms.InsertOnSubmit(form);
        }

        form.name = txtFormName.Text.Trim();
        form.thankYouPageId = pageSelectorThankYouPage.PageId;
        form.notify = checkNotify.Checked;
        if (form.notify)
        {
					form.notifyEmail = txtNotifyEmailAddress.Text.Trim();
        }
        else
        {
					form.notify = false;
        }
        form.post = checkPost.Checked;
        if (form.post)
        {
					form.postUrl = txtPostUrl.Text.Trim();
        }
        else
        {
					form.postUrl = null;
        }

        form.generateForm = checkGenerate.Checked;
        if (form.generateForm)
        {
          form.generationType = ddlGenerationType.SelectedValue;
          form.submitText = txtSubmitText.Text;
          form.cssClassContainer = txtCssClassContainer.Text.Trim();
          form.cssClassTable = txtCssClassTable.Text.Trim();
          form.cssClassRow = txtCssClassRow.Text.Trim();
          form.cssClassCellLabel = txtCssClassLabel.Text.Trim();
          form.cssClassCellValue = txtCssClassValue.Text.Trim();
          form.cssClassInputRow = txtCssClassInputRow.Text.Trim();
          form.manualFormText = txtManualForm.Text.Trim();
          form.validateDummyFields = checkDummyFields.Checked;
          form.validateReferrer = checkReferrer.Checked;
          form.validateSession = checkSession.Checked;
          form.validateReCaptcha = checkRecaptcha.Checked;


					form.summaryValidationEnabled = checkSummaryValidationEnabled.Checked;
					if (form.summaryValidationEnabled)
					{
						form.summaryValidationCssClass = txtSummaryValidationCssClass.Text;
						form.summaryValidationHeaderText = txtSummaryValidationHeaderText.Text;
						form.summaryValidationDisplayMode = ddlSummaryValidationDisplayMode.SelectedValue;
						form.summaryValidationEnableClientScript = checkSummaryValidationEnableClientScript.Checked;
						form.summaryValidationShowSummary = checkSummaryValidationShowSummary.Checked;
						form.summaryValidationShowMessageBox = checkSummaryValidationShowMessageBox.Checked;
					}
					else
					{
						form.summaryValidationCssClass = null;
						form.summaryValidationHeaderText = null;
						form.summaryValidationDisplayMode = null;
						form.summaryValidationEnableClientScript = false;
						form.summaryValidationShowSummary = false;
						form.summaryValidationShowMessageBox = false;
					}
        }
        else
        {
          form.cssClassContainer = null;
          form.cssClassTable = null;
          form.cssClassRow = null;
          form.cssClassCellLabel = null;
          form.cssClassCellValue = null;
          form.cssClassInputRow = null;
          form.validateDummyFields = true;
          form.validateReferrer = true;
          form.validateSession = true;
          form.validateReCaptcha = false;
        }

				System.Collections.Generic.List<NotifyListItem> lRemainingListItems = new System.Collections.Generic.List<NotifyListItem>();
				lRemainingListItems.AddRange(lNotifyByField);

				var notifies = from n in dc.scms_form_notify_by_fields
											 where n.formId == form.id
											 orderby n.id
											 select n;
				foreach (var notify in notifies)
				{
					NotifyListItem notifyListItem = lRemainingListItems.Where(nli => nli.id == notify.id).FirstOrDefault();
					if (notifyListItem != null)
					{
						if (notify.fieldId != notifyListItem.fieldId)
						{
							notify.fieldId = notifyListItem.fieldId;
						}

						if (string.Compare(notify.value, notifyListItem.value, false) != 0)
						{
							notify.value = notifyListItem.value;
						}

						if (string.Compare(notify.email, notifyListItem.value, false) != 0)
						{
							notify.email = notifyListItem.email;
						}

						lRemainingListItems.Remove(notifyListItem);
					}
					else
					{
						dc.scms_form_notify_by_fields.DeleteOnSubmit(notify);
					}
				}

				foreach (NotifyListItem notifyListItem in lRemainingListItems)
				{
					scms.data.scms_form_notify_by_field notify = new scms.data.scms_form_notify_by_field();
					dc.scms_form_notify_by_fields.InsertOnSubmit(notify);
					
					notify.formId = FormId.Value;
					notify.fieldId = notifyListItem.fieldId;
					notify.value = notifyListItem.value;
					notify.email = notifyListItem.email;
				}

        dc.SubmitChanges();
        FormId = form.id;
        statusMessage.ShowSuccess("Form updated");

        if (OnFormUpdated != null)
        {
					OnFormUpdated();
        }
			}
			catch (Exception ex)
			{
        string strMessage = "Failed updating form";
        statusMessage.ShowFailure(strMessage);
        global::scms.ScmsEvent.Raise(strMessage, this, ex);
			}
		}
	}

  protected void cvNotify_ServerValidate(object sender, ServerValidateEventArgs args)
  {
    bool bValid = false;

    if (checkNotify.Checked)
    {
      string strNotificationEmailAddress = txtNotifyEmailAddress.Text.Trim();
      if (!string.IsNullOrEmpty(strNotificationEmailAddress))
      {
				bValid = true;
      }
    }
    else
    {
			bValid = true;
    }

    args.IsValid = bValid;
  }

  protected void cvPost_ServerValidate(object sender, ServerValidateEventArgs args)
  {
    bool bValid = false;

    if (checkPost.Checked)
    {
      string strPostUrl = txtPostUrl.Text.Trim();
      if (!string.IsNullOrEmpty(strPostUrl))
      {
          bValid = true;
      }
    }
    else
    {
			bValid = true;
    }

    args.IsValid = bValid;
  }

  protected void ddlGenerationType_SelectedIndexChanged(object sender, EventArgs args)
  {
		bool bAuto = true;
		if (string.Compare(ddlGenerationType.SelectedValue, "manual", true) == 0)
		{
			bAuto = false;
		}

		if (bAuto)
		{
			multiviewGenSettings.SetActiveView(viewAutoGenSettings);
		}
		else
		{
			multiviewGenSettings.SetActiveView(viewManualGenSettings);
		}
  }

		protected void checkSummaryValidationEnabled_CheckedChanged(object sender, EventArgs args)
		{
			EnableControls();
		}

		protected void ReloadNotifications()
		{
			rptNotifyByField.DataSource = lNotifyByField;
			rptNotifyByField.DataBind();
		}

		protected void ClearNotificationInputs()
		{
			ddlNotifyByFieldField.ClearSelection();
			txtNotifyByFieldValue.Text = null;
			txtNotifyByFieldEmail.Text = null;
		}

		protected void btnNewNotifyByField_Click(object sender, EventArgs args)
		{
			NotifyListItem notifyListItem = new NotifyListItem();
			notifyListItem.id = 0;
			notifyListItem.name = ddlNotifyByFieldField.SelectedItem.Text;
			notifyListItem.fieldId = int.Parse(ddlNotifyByFieldField.SelectedValue);
			notifyListItem.value = txtNotifyByFieldValue.Text.Trim();
			notifyListItem.email = txtNotifyByFieldEmail.Text.Trim();
			lNotifyByField.Add(notifyListItem);
			ReloadNotifications();
			ClearNotificationInputs();
		}

		protected NotifyListItem GetNotifyListItem(string strStringValue)
		{
			NotifyListItem notifyListItem = null;

			foreach (var listItem in lNotifyByField)
			{
				if (string.Compare(listItem.ToString(), strStringValue) == 0)
				{
					notifyListItem = listItem;
					break;
				}
			}

			return notifyListItem;
		}

		protected void btnSaveNotifyByField_Click(object sender, EventArgs args)
		{
			NotifyListItem notifyListItem = GetNotifyListItem(NotifyBeingEdited);
			if (notifyListItem != null)
			{
				notifyListItem.fieldId = int.Parse(ddlNotifyByFieldField.SelectedValue);
				notifyListItem.name = ddlNotifyByFieldField.SelectedItem.Text;
				notifyListItem.value = txtNotifyByFieldValue.Text.Trim();
				notifyListItem.email = txtNotifyByFieldEmail.Text.Trim();
				
				NotifyBeingEdited = null;
				ClearNotificationInputs();

				btnNewNotify.Visible = true;
				btnSaveNotify.Visible = false;
				btnCancelNotify.Visible = false;
			}

			ReloadNotifications();
		}

		protected void btnCancelNotifyByField_Click(object sender, EventArgs args)
		{
			NotifyBeingEdited = null;

			ClearNotificationInputs();
			btnNewNotify.Visible = true;
			btnSaveNotify.Visible = false;
			btnCancelNotify.Visible = false;

			ReloadNotifications();
		}

		protected void EditNotify(object sender, CommandEventArgs args)
		{
			ClearNotificationInputs();

			string strNotifyKey = (string)args.CommandArgument;
			NotifyListItem  notifyListItem = GetNotifyListItem(strNotifyKey);
			if (notifyListItem != null)
			{
				ddlNotifyByFieldField.SelectedValue = notifyListItem.fieldId.ToString();
				txtNotifyByFieldValue.Text = notifyListItem.value;
				txtNotifyByFieldEmail.Text = notifyListItem.email;

				btnNewNotify.Visible = false;
				btnSaveNotify.Visible = true;
				btnCancelNotify.Visible = true;

				NotifyBeingEdited = strNotifyKey;
			}


		}

		protected void DeleteNotify(object sender, CommandEventArgs args)
		{
			NotifyListItem notifyListItem = GetNotifyListItem((string)args.CommandArgument);
			if (notifyListItem != null)
			{
				lNotifyByField.Remove(notifyListItem);
			}
			
			ReloadNotifications();
		}
  }
}