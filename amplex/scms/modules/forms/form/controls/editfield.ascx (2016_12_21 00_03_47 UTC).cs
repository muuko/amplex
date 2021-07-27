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
	public partial class editfield : System.Web.UI.UserControl
	{
		public delegate void SavedHandler(int? nFieldId);
		public delegate void CanceledHandler(int? nFieldId);

		public SavedHandler OnSaved = null;
		public CanceledHandler OnCancelled = null;

		public int? FieldId
		{
			get { return (int?)ViewState["FieldId"]; }

			set
			{
				int? nFieldId = value;
				ViewState["FieldId"] = value;
				LoadField();
			}
		}

		protected void Page_Load(object sender, EventArgs e)
		{
		}

		protected void LoadField()
		{
			Visible = false;

      if (FieldId.HasValue)
      {
          global::scms.data.ScmsDataContext dc = new global::scms.data.ScmsDataContext();
          var field = (from f in dc.scms_form_fields
                       where f.id == FieldId.Value
                       where f.deleted == false
                       select f).FirstOrDefault();
          if (field != null)
          {
              txtName.Text = field.name;
              txtLabel.Text = field.label;

              ddlType.ClearSelection();
              ListItem listItem = ddlType.Items.FindByValue(field.type);
              if (listItem == null)
              {
                  listItem = new ListItem(field.type, field.type);
                  ddlType.Items.Add(listItem);
              }
              listItem.Selected = true;

              if (field.width != null)
              {
                  txtWidth.Text = field.width.ToString();
              }
              else
              {
                  txtWidth.Text = null;
              }

              if (field.maxlength != null)
              {
                  txtMaxLength.Text = field.maxlength.ToString();
              }
              else
              {
                  txtMaxLength.Text = null;
              }

              txtColumns.Text = field.cols.ToString();
              txtRows.Text = field.rows.ToString();

              checkRequired.Checked = field.required;
              txtRequiredText.Text = field.requiredText;

							ddlValidationDisplay.SelectedValue = field.validationDisplay;

              txtCssClassOverrideRow.Text = field.cssClassOverrideRow;
              txtCssClassOverrideCellLabel.Text = field.cssClassOverrideCellLabel;
              txtCssClassOverrideCellValue.Text = field.cssClassOverrideCellValue;

              txtFileTypes.Text = field.fileTypes;

              txtDefault.Text = field.defaultText;
              txtDefaultMultiline.Text = field.defaultText;

              txtValidationRegex.Text = field.validationRegex;
              txtValidationErrorMessage.Text = field.validationErrorMessage;

              if (!string.IsNullOrEmpty(field.repeatDirection))
              {
                  ddlRepeatDirection.ClearSelection();
                  ListItem liRepeatDirection = ddlRepeatDirection.Items.FindByValue(field.repeatDirection);
                  if (liRepeatDirection == null)
                  {
                      liRepeatDirection = new ListItem(field.repeatDirection, field.repeatDirection);
                  }
                  liRepeatDirection.Selected = true;
              }

              if (field.repeatColumns != null)
              {
                  txtRepeatColumns.Text = field.repeatColumns.ToString();
              }

              if (!string.IsNullOrEmpty(field.repeatLayout))
              {
                  ddlRepeatLayout.ClearSelection();
                  ListItem liRepeatLayout = ddlRepeatLayout.Items.FindByValue(field.repeatLayout);
                  if (liRepeatLayout == null)
                  {
                      liRepeatLayout = new ListItem(field.repeatLayout, field.repeatLayout);
                  }
                  liRepeatLayout.Selected = true;
              }

              checkRedactInNotification.Checked = field.redactInEmail.HasValue && field.redactInEmail.Value;
              

              Visible = true;

              EnableControls();
              txtName.Focus();
          }
      }

			LoadOptions();
		}

		protected void GetRelevantAttributes(string strType, 
			out bool bDefaultText,
            out bool bDefaultTextMultiline,
			out bool bDefaultOption,
			out bool bDefaultCheck,
			out bool bTextAreaSettings,
			out bool bWidth,
			out bool bMaxLength,
			out bool bRequired,
			out bool bValidation,
			out bool bOptions,
            out bool bFileTypes,
            out bool bShowClassOverrides,
            out bool bShowRepeatOptions,
            out bool bShowRedact
			)
		{
			bDefaultText = false;
			bDefaultOption = false;
			bDefaultCheck = false;
      bDefaultTextMultiline = false;
			bTextAreaSettings = false;
			bWidth = false;
			bMaxLength = false;
			bRequired = false;
			bValidation = false;
			bOptions = false;
      bFileTypes = false;
      bShowClassOverrides = false;
      bShowRepeatOptions = false;
      bShowRedact = false;

			// defaults
			switch (strType)
			{
				case "text":
					bDefaultText = true;
					bWidth = true;
					bMaxLength = true;
					bRequired = true;
					bValidation = true;
          bShowClassOverrides = true;
          bShowRedact = true;
					break;

				case "textarea":
					bDefaultText = true;
					bTextAreaSettings = true;
					bWidth = true;
					bRequired = true;
					bValidation = true;
          bShowClassOverrides = true;
          bShowRedact = true;
					break;

				case "checkbox":
					bDefaultCheck = true;
          bShowClassOverrides = true;
          bShowRedact = true;
					break;

				case "checkboxlist":
					bDefaultOption = true;
					bWidth = true;
					bOptions = true;
					bRequired = true;
          bShowClassOverrides = true;
          bShowRepeatOptions = true;
          bShowRedact = true;
					break;

				case "dropdownlist":
					bDefaultOption = true;
					bWidth = true;
					bOptions = true;
					bRequired = true;
          bShowClassOverrides = true;
          bShowRedact = true;
					break;

				case "radiobuttonlist":
					bDefaultOption = true;
					bWidth = true;
					bOptions = true;
					bRequired = true;
          bShowClassOverrides = true;
          bShowRepeatOptions = true;
          bShowRedact = true;
					break;

        case "fileupload":
            bWidth = true;
            bFileTypes = true;
            bShowClassOverrides = true;
            break;

        case "literal":
            bDefaultTextMultiline = true;
            break;

				case "hidden":
					bDefaultText = true;
					break;

				case "auto:ipaddress":
				case "auto:url":
				case "auto:referrer":
                case "auto:userid":
					break;
			}
		}


		protected void EnableControls()
		{
			bool bShowDefaultText = false;
            bool bShowDefaultTextMultiline = false;
			bool bShowDefaultOption = false;
			bool bShowDefaultCheck = false;
			bool bShowTextAreaSettings = false;
			bool bShowWidth = false;
			bool bShowMaxLength = false;
			bool bShowRequired = false;
			bool bShowValidation = false;
			bool bShowOptions = false;
      bool bShowFileTypes = false;
      bool bShowClassOverrides = false;
      bool bShowRepeatOptions = false;
      bool bShowRedact = false;

			GetRelevantAttributes(ddlType.SelectedValue,
				out bShowDefaultText,
                out bShowDefaultTextMultiline,
				out bShowDefaultOption,
				out bShowDefaultCheck,
				out bShowTextAreaSettings,
				out bShowWidth,
				out bShowMaxLength,
				out bShowRequired,
				out bShowValidation,
				out bShowOptions,
                out bShowFileTypes,
                out bShowClassOverrides,
                out bShowRepeatOptions,
                out bShowRedact);


			txtDefault.Visible = bShowDefaultText;
			txtDefaultMultiline.Visible = bShowDefaultTextMultiline;
			ddlDefault.Visible = bShowDefaultOption;
			checkDefault.Visible = bShowDefaultCheck;
			trOptions.Visible = bShowOptions;
			trDefaultValue.Visible = bShowDefaultText || bShowDefaultTextMultiline || bShowDefaultCheck || bShowDefaultOption;
      placeholderRequired.Visible = bShowRequired;
			placeholderValidation.Visible = bShowValidation;

			trWidth.Visible = bShowWidth;
			trMaxLength.Visible = bShowMaxLength;
			placeholderTextAreaSettings.Visible = bShowTextAreaSettings;

			trFileTypes.Visible = bShowFileTypes;


			txtPostId.Enabled = checkPost.Checked;

      placeholderClassOverrides.Visible = bShowClassOverrides;
      placeholderRepeatOptions.Visible = bShowRepeatOptions;
      placeholderRedact.Visible = bShowRedact;
		}


		protected void checkPost_CheckedChanged(object sender, EventArgs args )
		{
			EnableControls();
		}

		protected void ddlType_SelectedIndexChanged(object sender, EventArgs args)
		{
			EnableControls();
			LoadOptions();
		}

		protected void LoadOptions()
		{
			rptOptions.DataSource = null;
			if (FieldId.HasValue)
			{
				global::scms.data.ScmsDataContext dc = new global::scms.data.ScmsDataContext();
				var options = from o in dc.scms_form_field_options
											 where o.fieldid == FieldId.Value
											 where o.deleted == false
											 orderby o.ordinal
											 select o;
				rptOptions.DataSource = options;
			}

			rptOptions.DataBind();
			LoadDefaultOptionValues();
		}

		protected void LoadDefaultOptionValues()
		{
			string strCurrent = ddlDefault.SelectedValue;
			ddlDefault.Items.Clear();

			bool bShowDefaultText = false;
            bool bShowDefaultTextMultiline = false;
			bool bShowDefaultOption = false;
			bool bShowDefaultCheck = false;
			bool bShowTextAreaSettings = false;
			bool bShowWidth = false;
			bool bShowMaxLength = false;
			bool bShowRequired = false;
			bool bShowValidation = false;
			bool bShowOptions = false;
      bool bShowFileTypes = false;
      bool bShowClassOverrides = false;
      bool bShowRepeatOptions = false;
      bool bShowRedact = false;

			GetRelevantAttributes(ddlType.SelectedValue,
				out bShowDefaultText,
                out bShowDefaultTextMultiline,
				out bShowDefaultOption,
				out bShowDefaultCheck,
				out bShowTextAreaSettings,
				out bShowWidth,
				out bShowMaxLength,
				out bShowRequired,
				out bShowValidation,
				out bShowOptions,
        out bShowFileTypes,
        out bShowClassOverrides,
        out bShowRepeatOptions,
        out bShowRedact);

			ddlDefault.DataSource = null;
			if (bShowOptions)
			{
				global::scms.data.ScmsDataContext dc = new global::scms.data.ScmsDataContext();
				var options = from o in dc.scms_form_field_options
											where o.fieldid == FieldId.Value
											where o.deleted == false
											orderby o.ordinal
											select o;

				ListItem itemNone = new ListItem("", "");
				ddlDefault.Items.Add(itemNone);

				ddlDefault.DataSource = options;
				ddlDefault.DataTextField = "name";
				ddlDefault.DataValueField = "value";
				ddlDefault.AppendDataBoundItems = true;

				ddlDefault.DataBind();

				ListItem itemToSelect = itemNone;
				if (!string.IsNullOrEmpty(strCurrent))
				{
					ListItem itemCurrent = ddlDefault.Items.FindByValue(strCurrent);
					if (itemCurrent != null)
					{
						itemToSelect = itemCurrent;
					}
				}
				itemToSelect.Selected = true;
			
			}
			else
			{
				ddlDefault.DataBind();
			}

			
		}

		protected void DeleteOption(object sender, CommandEventArgs args)
		{
			int nFieldOptionId = int.Parse(args.CommandArgument.ToString());
			global::scms.data.ScmsDataContext dc = new global::scms.data.ScmsDataContext();
			var option = (from o in dc.scms_form_field_options
										where o.id == nFieldOptionId
										where o.deleted == false
										where o.fieldid == FieldId.Value
										select o).FirstOrDefault();
			if (option != null)
			{
				option.deleted = true;
				dc.SubmitChanges();
			}

			LoadOptions();
		}

		protected void MoveOption(object sender, CommandEventArgs args)
		{
			bool bUp = string.Compare(args.CommandName, "up", true) == 0;
			int nFieldOptionId = int.Parse((string)args.CommandArgument);

			global::scms.data.ScmsDataContext dc = new global::scms.data.ScmsDataContext();

			if (FieldId.HasValue)
			{
				var fieldOptions = from o in dc.scms_form_field_options
										 where o.fieldid == FieldId.Value
										 where o.deleted == false
										 orderby o.ordinal
										 select o;

				global::scms.data.scms_form_field_option prior = null;
				global::scms.data.scms_form_field_option next = null;

				var thisFieldOption = fieldOptions.Where(fieldOption => (fieldOption.id == nFieldOptionId)).Single();
				int nThisOrdinal = thisFieldOption.ordinal;

				foreach (var fieldOption in fieldOptions)
				{
					if (fieldOption.id != thisFieldOption.id)
					{
						if (fieldOption.ordinal < nThisOrdinal)
						{
							prior = fieldOption;
						}
						else
						{
							if (next == null)
							{
								if (fieldOption.ordinal > nThisOrdinal)
								{
									next = fieldOption;
									break;
								}
							}
						}
					}
				}

				if (bUp)
				{
					if (prior != null)
					{
						thisFieldOption.ordinal = prior.ordinal;
						prior.ordinal = nThisOrdinal;
					}
				}
				else
				{
					if (next != null)
					{
						thisFieldOption.ordinal = next.ordinal;
						next.ordinal = nThisOrdinal;
					}
				}
			}


			dc.SubmitChanges();
			LoadOptions();
		}

		protected void btnNewOption_Click(object sender, EventArgs args)
		{
			if (Page.IsValid)
			{
				string strOptionName = txtOptionName.Text.Trim();
				string strOptionValue = txtOptionValue.Text.Trim();

				global::scms.data.ScmsDataContext dc = new global::scms.data.ScmsDataContext();

				int nOrdinal = 0;
				var maxFieldOption = (from o in dc.scms_form_field_options
															where o.fieldid == FieldId.Value
															where o.deleted == false
															orderby o.ordinal descending
															select o).FirstOrDefault();
				if (maxFieldOption != null)
				{
					nOrdinal = maxFieldOption.ordinal + 1;
				}

				global::scms.data.scms_form_field_option option = new global::scms.data.scms_form_field_option();
				option.fieldid = FieldId.Value;
				option.name = strOptionName;
				option.value = strOptionValue;
				option.ordinal = nOrdinal;
				option.deleted = false;
				dc.scms_form_field_options.InsertOnSubmit(option);
				dc.SubmitChanges();
				txtOptionName.Text = null;
				txtOptionValue.Text = null;
                txtFileTypes.Text = "gif, jpg, png, bmp";

				LoadOptions();
			}
		}

		protected void cvName_ServerValidate(object sender, ServerValidateEventArgs args)
		{
			bool bValid = false;

			if (FieldId.HasValue)
			{
				global::scms.data.ScmsDataContext dc = new global::scms.data.ScmsDataContext();
				var field = (from f in dc.scms_form_fields
										 where f.deleted == false
										 where f.id == FieldId.Value
										 select f).Single();

				var fieldsWithSameName = from f in dc.scms_form_fields
																 where f.formid == field.formid
																 where f.deleted == false
																 where f.id != FieldId.Value
																 where f.name == txtName.Text.Trim()
																 select f;
				if (fieldsWithSameName.Count()== 0)
				{
					bValid = true;
				}
			}

			args.IsValid = bValid;
		}

		protected void cvPostId_ServerValidate(object sender, ServerValidateEventArgs args)
		{
			bool bValid = false;
			if (checkPost.Checked)
			{
				bValid = !string.IsNullOrEmpty(txtPostId.Text.Trim());
			}
			else
			{
				bValid = true;
			}
			args.IsValid = bValid;
		}

		protected void btnSave_Click(object sender, EventArgs args)
		{
			Page.Validate("EditField");
			if (Page.IsValid)
			{
				if (FieldId.HasValue)
				{
					global::scms.data.ScmsDataContext dc = new global::scms.data.ScmsDataContext();
					var field = (from f in dc.scms_form_fields
											 where f.id == FieldId.Value
											 where f.deleted == false
											 select f).FirstOrDefault();
					if (field != null)
					{
						field.name = txtName.Text.Trim();
						field.label = txtLabel.Text.Trim();
						field.type = ddlType.SelectedValue;


						bool bDefaultText = false;
                        bool bDefaultTextMultiline = false;
						bool bDefaultOption = false;
						bool bDefaultCheck = false;
						bool bTextAreaSettings = false;
						bool bWidth = false;
						bool bMaxLength = false;
						bool bRequired = false;
						bool bValidation = false;
						bool bOptions = false;
            bool bFileTypes = false;
            bool bShowClassOverrides = false;
            bool bShowRepeatOptions = false;
            bool bShowRedact = false;

						GetRelevantAttributes(field.type,
							out bDefaultText,
                            out bDefaultTextMultiline,
							out bDefaultOption,
							out bDefaultCheck,
							out bTextAreaSettings,
							out bWidth,
							out bMaxLength,
							out bRequired,
							out bValidation,
							out bOptions,
                            out bFileTypes,
                            out bShowClassOverrides,
                            out bShowRepeatOptions,
                            out bShowRedact);

						field.defaultChecked = null;
						field.defaultText = null;
						if (bDefaultText)
						{
							field.defaultText = txtDefault.Text.Trim();
						}
            if (bDefaultTextMultiline)
            {
                field.defaultText = txtDefaultMultiline.Text.Trim();
            }

						if (bDefaultOption)
						{
							field.defaultText = ddlDefault.SelectedValue;
						}
						if (bDefaultCheck)
						{
							field.defaultChecked = checkDefault.Checked;
						}

            field.validationErrorMessage = null;
            field.validationRegex = null;
            if (bValidation)
            {
                field.validationErrorMessage = txtValidationErrorMessage.Text;
                field.validationRegex = txtValidationRegex.Text;
            }

						field.width = null;
						if (bWidth)
						{
							string strWidth = txtWidth.Text.Trim();
							if (!string.IsNullOrEmpty(strWidth))
							{
								field.width = int.Parse(strWidth);
							}
						}

						field.maxlength = null;
						if (bMaxLength)
						{
							string strMaxLength = txtMaxLength.Text.Trim();
							if (!string.IsNullOrEmpty(strMaxLength))
							{
								field.maxlength = int.Parse(strMaxLength);
							}
						}

						field.cols = null;
						field.rows = null;
						if (bTextAreaSettings)
						{
							string strColumns = txtColumns.Text.Trim();
							if (!string.IsNullOrEmpty(strColumns))
							{
								field.cols = int.Parse(strColumns);
							}

							string strRows = txtRows.Text.Trim();
							if (!string.IsNullOrEmpty(strRows))
							{
								field.rows = int.Parse(strRows);
							}
						}

						field.required = false;
						if (bRequired)
						{
							field.required = checkRequired.Checked;
							field.requiredText = txtRequiredText.Text;
						}
						field.validationDisplay = ddlValidationDisplay.SelectedValue;

						field.cssClassOverrideRow = null;
						if( !string.IsNullOrEmpty(txtCssClassOverrideRow.Text.Trim()))
						{
							field.cssClassOverrideRow = txtCssClassOverrideRow.Text.Trim();
						}

						field.cssClassOverrideCellLabel = null;
						if( !string.IsNullOrEmpty(txtCssClassOverrideCellLabel.Text.Trim()))
						{
							field.cssClassOverrideCellLabel = txtCssClassOverrideCellLabel.Text.Trim();
						}

						field.cssClassOverrideCellValue = null;
						if( !string.IsNullOrEmpty(txtCssClassOverrideCellValue.Text.Trim()))
						{
							field.cssClassOverrideCellValue = txtCssClassOverrideCellValue.Text.Trim();
						}

            field.fileTypes = null;
            if (bFileTypes)
            {
                field.fileTypes = txtFileTypes.Text.Trim();
            }

            field.repeatDirection = null;
            field.repeatColumns = null;
            field.repeatLayout = null;
            if (bShowRepeatOptions)
            {
                if (!string.IsNullOrEmpty(ddlRepeatDirection.SelectedValue))
                {
                    field.repeatDirection = ddlRepeatDirection.SelectedValue;
                }

                if (!string.IsNullOrEmpty(txtRepeatColumns.Text))
                {
                    field.repeatColumns = int.Parse(txtRepeatColumns.Text);
                }

                if (!string.IsNullOrEmpty(ddlRepeatLayout.SelectedValue))
                {
                    field.repeatLayout = ddlRepeatLayout.SelectedValue;
                }
            }

            field.redactInEmail = false;
            if (bShowRedact)
            {
                field.redactInEmail = checkRedactInNotification.Checked;
            }

						dc.SubmitChanges();

						if (OnSaved != null)
						{
							OnSaved(FieldId);
						}
					}
				}

			}
			else
			{
				panel.Focus();
			}
		}

		protected void btnCancel_Click(object sender, EventArgs args)
		{
			if (OnCancelled != null)
			{
				OnCancelled(FieldId);
			}
		}
	}
}