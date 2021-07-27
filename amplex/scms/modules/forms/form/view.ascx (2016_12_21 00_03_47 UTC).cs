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
	public partial class view : scms.RootControl
	{
    protected global::scms.data.scms_form form = null;
		protected System.Collections.Generic.Dictionary<Control, global::scms.data.scms_form_field> controlFields = new System.Collections.Generic.Dictionary<Control, scms.data.scms_form_field>();
		protected System.Collections.Generic.Dictionary<CustomValidator, Control> customValidators = new System.Collections.Generic.Dictionary<CustomValidator, Control>();
		protected System.Collections.Generic.Dictionary<int,scms.data.scms_form_field> fieldsById = new System.Collections.Generic.Dictionary<int,scms.data.scms_form_field>();
		protected global::scms.data.scms_form_field[] aFields = null;

		

		protected void Page_Load(object sender, EventArgs e)
		{
			if (IsPostBack)
			{
				string strEventArgument = Request["__EVENTARGUMENT"];
				if (!string.IsNullOrEmpty(strEventArgument))
				{
					ManualSubmit();
				}

			}
			else
			{
				if (!IsPostBack)
				{
					System.Web.UI.HtmlControls.HtmlForm form = (System.Web.UI.HtmlControls.HtmlForm)this.Page.Master.FindControl("form");
					form.Attributes.Add("data-ajax", "false");
				}


				string strCrossPost = Request.Params["crosspost"];
				if (!string.IsNullOrEmpty(strCrossPost))
				{
					if (string.Compare(strCrossPost, "1") == 0)
					{
						ManualSubmit();
					}
				}
			}
		}

		protected void LoadRecaptchaSettings(scms.data.ScmsDataContext dc, scms.data.scms_form form)
		{
			try
			{
				if (form.generateForm)
				{
					if (form.validateReCaptcha)
					{
						var settings = (from s in dc.scms_configs
														select s);

						var publicKey = (from s in settings where s.name == "recaptcha-public-key" select s.value).FirstOrDefault();
						var privateKey = (from s in settings where s.name == "recaptcha-private-key" select s.value).FirstOrDefault();

						if (!string.IsNullOrEmpty(publicKey) && !string.IsNullOrEmpty(privateKey))
						{
							HtmlGenericControl div = new HtmlGenericControl("div");
							div.Attributes["class"] = "recaptcha_wrap";

							placeholderRecaptcha.Controls.Add(div);

							Recaptcha.RecaptchaControl control = new Recaptcha.RecaptchaControl();
							control.PublicKey = publicKey;
							control.PrivateKey = privateKey;
							div.Controls.Add(control);
							/*
							<recaptcha:RecaptchaControl 
			ID="recaptcha" 
			runat="server" 
			Visible="false"
			PublicKey="6LddNQkAAAAAAL_coAOfzy2lIcz9V3OyT_xxlh_U"
			PrivateKey="6LddNQkAAAAAAHJV1KiHqZR0laISivtOJIUSKzJp"
			/>

						recaptcha.Visible = true;
						 
						recaptcha.PublicKey = publicKey;
						recaptcha.PrivateKey = privateKey;
						 * * */
						}

					}

				}
			}
			catch
			{
				// TODO log this
			}
			//		<%@ Register TagPrefix="recaptcha" Namespace="Recaptcha" Assembly="Recaptcha" %>
		}

		protected void Page_PreInit(object sender, EventArgs args)
		{
		}

		protected void Page_Init(object sender, EventArgs args)
		{
			bool bShowForm = false;
			if (ModuleInstanceId.HasValue)
			{
				global::scms.data.ScmsDataContext dc = new scms.data.ScmsDataContext();
				form = (from fi in dc.scms_form_instances
										where fi.instanceId == ModuleInstanceId.Value
										join f in dc.scms_forms on fi.formid equals f.id
										where f.deleted == false
										select f).FirstOrDefault();
				if (form != null)
				{
					if( form.generateForm )
					{
						bShowForm = true;
						GenerateForm(dc, form);
					}

					if (!IsPostBack)
					{
						PrepareSpamProtections(form);
					}
				}
			}

			if( !bShowForm )
			{
				Visible = false;
			}
		}

		protected void GenerateForm(global::scms.data.ScmsDataContext dc, global::scms.data.scms_form form)
		{
			string strValidationGroup = string.Format("module-{0}", ModuleInstanceId.Value);

			btnSubmit.ValidationGroup = strValidationGroup;
			if (!string.IsNullOrEmpty(form.submitText))
			{
				btnSubmit.Text = form.submitText;
			}

			// load the fields
			var fields = from f in dc.scms_form_fields
									 where f.formid == form.id
									 where f.deleted == false
									 orderby f.ordinal
									 select f;
			aFields = fields.ToArray<global::scms.data.scms_form_field>();
			foreach( scms.data.scms_form_field field in aFields )
			{
				fieldsById[field.id] = field;
			}
			
			switch (form.generationType)
			{
				case "table":
					GenerateTableForm(strValidationGroup, dc, form, aFields);
					break;

				case "div":
					GenerateDivForm(strValidationGroup, dc, form, aFields);
					break;

				case "manual":
					GenerateManualForm(dc, form, aFields);
					break;
			}

			LoadRecaptchaSettings(dc, form);
		}

		protected void GenerateHiddenField(global::scms.data.scms_form_field field)
		{
		}

    protected void GenerateLiteralField(HtmlGenericControl divContainer, global::scms.data.scms_form_field field)
    {
      Literal literal = new Literal();
      literal.Text = field.defaultText;
      divContainer.Controls.Add(literal);
    }

    protected string GetFieldRowClass(global::scms.data.scms_form_field field)
    {
        System.Text.StringBuilder sbRowClass = new System.Text.StringBuilder();

        if (!string.IsNullOrEmpty(field.cssClassOverrideRow))
        {
            sbRowClass.Append(field.cssClassOverrideRow);
        }
        else
        {
            sbRowClass.Append(form.cssClassRow);
        }

        if (sbRowClass.Length > 0)
        {
            sbRowClass.Append(' ');
        }

        sbRowClass.AppendFormat("form-field-{0}", field.name);

        return sbRowClass.ToString();
    }

		protected void GenerateTableForm(string strValidationGroup, global::scms.data.ScmsDataContext dc, global::scms.data.scms_form form, global::scms.data.scms_form_field[] aFields)
		{
			multiView.SetActiveView(viewTable);

            HtmlGenericControl table = new HtmlGenericControl("table");
            placeholderTableGen.Controls.Add(table);

			if (!string.IsNullOrEmpty(form.cssClassContainer))
			{
				panelGenerated.CssClass = form.cssClassContainer;
			}

			if (!string.IsNullOrEmpty(form.cssClassTable))
			{
                table.Attributes.Add("class", form.cssClassTable);
			}

			foreach (global::scms.data.scms_form_field field in aFields)
			{
				// add if not an auto generated field
				if (string.Compare(field.type, 0, "auto:", 0, 5, true) != 0)
				{
					if (string.Compare(field.type, "hidden", true) == 0)
					{
						GenerateHiddenField(field);
					}

          if (string.Compare(field.type, "literal", true) == 0)
          {
						GenerateLiteralField(table, field);
          }
          else
          {
            // add a row
            System.Web.UI.HtmlControls.HtmlGenericControl row = new HtmlGenericControl("tr");
            table.Controls.Add(row);

            string strRowClass = GetFieldRowClass(field);
            row.Attributes.Add("class", strRowClass);


            // add the label cell
            System.Web.UI.HtmlControls.HtmlGenericControl cellLabel = new HtmlGenericControl("td");
            row.Controls.Add(cellLabel);
            string strLabelCellClass = form.cssClassCellLabel;
            if (!string.IsNullOrEmpty(field.cssClassOverrideCellLabel))
            {
                strLabelCellClass = field.cssClassOverrideCellLabel;
            }
            if (!string.IsNullOrEmpty(strLabelCellClass))
            {
                cellLabel.Attributes.Add("class", strLabelCellClass);
            }
            cellLabel.InnerText = field.label;

            // add the value cell
            System.Web.UI.HtmlControls.HtmlGenericControl cellValue = new HtmlGenericControl("td");
            row.Controls.Add(cellValue);
            string strValueCellClass = form.cssClassCellValue;
            if (!string.IsNullOrEmpty(field.cssClassOverrideCellValue))
            {
                strValueCellClass = field.cssClassOverrideCellValue;
            }
            if (!string.IsNullOrEmpty(strValueCellClass))
            {
                cellValue.Attributes.Add("class", strValueCellClass);
            }

            GenerateInputControl(strValidationGroup, dc, form, field, cellValue);
          }
				}
			}

			GenerateSummaryValidation(strValidationGroup, dc, form);
			GenerateSubmitRow(dc, form);
		}

		protected void GenerateInputControl(string strValidationGroup, global::scms.data.ScmsDataContext dc, global::scms.data.scms_form form, global::scms.data.scms_form_field field, Control controlParent)
		{
			Control control = null;
			bool bCustomRequired = false;

			switch (field.type)
			{
				case "text":
				{
					System.Web.UI.WebControls.TextBox txtControl = (TextBox) this.LoadControl(typeof(TextBox), null);
					control = txtControl;
					
					if (!string.IsNullOrEmpty(field.defaultText))
					{
						txtControl.Text = field.defaultText;
					}

					if (field.maxlength != null)
					{
						txtControl.MaxLength = field.maxlength.Value;
					}
				}
				break;
				
				case "textarea":
					System.Web.UI.WebControls.TextBox txtAreaControl = (TextBox)this.LoadControl(typeof(TextBox), null);
					control = txtAreaControl;

					txtAreaControl.TextMode = TextBoxMode.MultiLine;

					if (!string.IsNullOrEmpty(field.defaultText))
					{
						txtAreaControl.Text = field.defaultText;
					}

					if (field.cols.HasValue)
					{
						txtAreaControl.Columns = field.cols.Value;
					}

					if (field.rows.HasValue)
					{
						txtAreaControl.Rows = field.rows.Value;
					}
					break;

				case "checkbox":
					{
						CheckBox checkBox = new CheckBox();
						control = checkBox;

						if( field.defaultChecked.HasValue )
						{
							checkBox.Checked = field.defaultChecked.Value;
						}
					}
					break;

				case "checkboxlist":
					{
						CheckBoxList checkBoxList = new CheckBoxList();
						control = checkBoxList;
						AddOptions(checkBoxList.Items, field);
                        if (!string.IsNullOrEmpty(field.repeatDirection))
                        {
                            checkBoxList.RepeatDirection = (RepeatDirection)(Enum.Parse(typeof(RepeatDirection), field.repeatDirection, true));
                        }

                        if (field.repeatColumns.HasValue)
                        {
                            checkBoxList.RepeatColumns = field.repeatColumns.Value;
                        }

                        if (!string.IsNullOrEmpty(field.repeatLayout))
                        {
                            checkBoxList.RepeatLayout = (RepeatLayout)(Enum.Parse(typeof(RepeatLayout), field.repeatLayout, true));
                        }

						bCustomRequired = true;
					}
					break;

				case "dropdownlist":
					{
						DropDownList ddl = new DropDownList();
						control = ddl;
						AddOptions(ddl.Items, field);
						bCustomRequired = true;
					}
					break;

				case "radiobuttonlist":
					{
						RadioButtonList rbl = new RadioButtonList();
						control = rbl;
						AddOptions(rbl.Items, field);

                        if (!string.IsNullOrEmpty(field.repeatDirection))
                        {
                            rbl.RepeatDirection = (RepeatDirection)(Enum.Parse(typeof(RepeatDirection), field.repeatDirection, true));
                        }

                        if (field.repeatColumns.HasValue)
                        {
                            rbl.RepeatColumns = field.repeatColumns.Value;
                        }

                        if (!string.IsNullOrEmpty(field.repeatLayout))
                        {
                            rbl.RepeatLayout = (RepeatLayout)(Enum.Parse(typeof(RepeatLayout), field.repeatLayout, true));
                        }

						bCustomRequired = true;
					}
					break;

                case "fileupload":
                    {
                        FileUpload fileUpload = new FileUpload();
                        control = fileUpload;
                    }
                    break;

                case "literal":
                    {
                        Literal literal = new Literal();
                        literal.Text = field.defaultText;
                        control = literal;
                    }
                    break;

				default:
					throw new Exception("Unknown control type");
			}

			// generic webcontrol attributes
			// if web control, set width
			if( control is WebControl)
			{
				WebControl webControl = (WebControl)control;
				if (webControl != null)
				{
					if (field.width != null)
					{
						webControl.Width = field.width.Value;
					}
				}
			}

			control.ID = string.Format("scms_{0}", field.id);
			controlParent.Controls.Add(control);
			controlFields[control] = field;

			ValidatorDisplay validatorDisplay = ValidatorDisplay.Dynamic;
			if (!string.IsNullOrEmpty(field.validationDisplay))
			{
				validatorDisplay = (ValidatorDisplay)Enum.Parse(typeof(ValidatorDisplay), field.validationDisplay, true);
			}

			if (field.required)
			{
				if (bCustomRequired)
				{
					CustomValidator cv = (CustomValidator)this.LoadControl(typeof(CustomValidator), null);
					cv.ValidationGroup = strValidationGroup;
					cv.ValidateEmptyText = true;
//					cv.ControlToValidate = control.ID;
          if (!string.IsNullOrEmpty(field.requiredText))
          {
              cv.ErrorMessage = field.requiredText;
          }
          else
          {
              cv.ErrorMessage = "Required";
          }
					cv.Display = validatorDisplay;
					cv.ServerValidate += CustomRequired;
					controlParent.Controls.Add(cv);

					customValidators.Add(cv, control);
				}
				else
				{
					RequiredFieldValidator rfv = (RequiredFieldValidator)this.LoadControl(typeof(RequiredFieldValidator), null);
					rfv.ValidationGroup = strValidationGroup;
					rfv.ControlToValidate = control.ID;
          if (!string.IsNullOrEmpty(field.requiredText))
          {
						rfv.ErrorMessage = field.requiredText;
          }
          else
          {
						rfv.ErrorMessage = "Required";
          }

					rfv.Display = validatorDisplay;
					controlParent.Controls.Add(rfv);
				}
			}

      if (!string.IsNullOrEmpty(field.validationRegex))
      {
        RegularExpressionValidator rxv = (RegularExpressionValidator)this.LoadControl(typeof(RegularExpressionValidator), null);
        rxv.ValidationExpression = field.validationRegex;
        rxv.ValidationGroup = strValidationGroup;
        rxv.ControlToValidate = control.ID;
        rxv.ErrorMessage = field.validationErrorMessage;
				rxv.Display = validatorDisplay;
        
        controlParent.Controls.Add(rxv);
      }

      if (!string.IsNullOrEmpty(field.fileTypes))
      {
          CustomValidator cv = (CustomValidator)this.LoadControl(typeof(CustomValidator), null);
          cv.ServerValidate+= FileUploadServerValidate;
          cv.ControlToValidate = control.ID;
          cv.ValidationGroup = strValidationGroup;
          cv.ErrorMessage = "Invalid file format";
          cv.Display = validatorDisplay;
          controlParent.Controls.Add(cv);

          customValidators.Add(cv, control);
      }
		}

    protected void FileUploadServerValidate(object sender, ServerValidateEventArgs args)
    {
      if (sender.GetType() == typeof(CustomValidator))
      {
        CustomValidator cv = (CustomValidator)sender;

        Control controlToValidate = null;
        if (customValidators.TryGetValue(cv, out controlToValidate))
				{
          bool bValid = false;
          FileUpload fileUpload = controlToValidate as FileUpload;
          if (fileUpload != null)
	        {
            if (fileUpload.HasFile)
            {
              scms.data.scms_form_field field;
              if (!controlFields.TryGetValue(fileUpload, out field))
              {
								throw new Exception("file upload not found");
              }

              string strFileTypes = field.fileTypes;
              if (!string.IsNullOrEmpty(strFileTypes))
              {
                strFileTypes = strFileTypes.ToLower();
                string[] astrFileTypes = strFileTypes.Split(new char[] { ',', ';', ' ' }, StringSplitOptions.RemoveEmptyEntries);
                if (astrFileTypes.Length > 0)
                {
                  string strFileName = fileUpload.FileName.ToLower();
                  int nLastDot = strFileName.LastIndexOf('.');
                  if (nLastDot > 0)
                  {
                    string strExtension = strFileName.Substring(nLastDot + 1);
                    if (astrFileTypes.Contains(strExtension))
                    {
                      bValid = true;
                    }
                  }

                  if (!bValid)
                  {
                    cv.ErrorMessage = string.Format("<br />Uploaded file must be one of these types: '{0}'", strFileTypes);
                  }
                }
                else
                {
									bValid = true;
                }
              }
              else
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
				}
      }
    }

		protected void CustomRequired(object sender, ServerValidateEventArgs args)
		{
			if (sender.GetType() == typeof(CustomValidator))
			{
				CustomValidator cv = (CustomValidator)sender;
				Control controlToValidate = null;
				if (customValidators.TryGetValue(cv, out controlToValidate))
				{
					bool bValid = false;

					CheckBoxList checkBoxList = controlToValidate as CheckBoxList;
					if (checkBoxList != null)
					{
						foreach (ListItem item in checkBoxList.Items)
						{
							if (item.Selected)
							{
								bValid = true;
								break;
							}
						}
					}
					else
					{
						RadioButtonList radioButtonList = controlToValidate as RadioButtonList;
						if (radioButtonList != null)
						{
							if (!string.IsNullOrEmpty(radioButtonList.SelectedValue))
							{
								bValid = true;
							}
						}
						else
						{
							DropDownList dropDownList = controlToValidate as DropDownList;
							if (dropDownList != null)
							{
								if (!string.IsNullOrEmpty(dropDownList.SelectedValue))
								{
									bValid = true;
								}
							}
							else
							{
								bValid = true;
							}
						}
						

					}

					args.IsValid = bValid;
				}
			}
		}

		protected void AddOptions(ListItemCollection listItems, scms.data.scms_form_field field)
		{
			bool bAnySelected = false;

			foreach (scms.data.scms_form_field_option option in 
				field.scms_form_field_options.Where( o => o.deleted == false).OrderBy(o => o.ordinal ))
			{
				ListItem listItem = new ListItem(option.name, option.value);
				if (string.Compare(field.defaultText, option.value, true) == 0)
				{
					if (!bAnySelected)
					{
						listItem.Selected = true;
						bAnySelected = true;
					}
				}
				listItems.Add(listItem);
			}

		}

		protected void GenerateDivForm(string strValidationGroup, global::scms.data.ScmsDataContext dc, global::scms.data.scms_form form, global::scms.data.scms_form_field[] aFields)
		{
			multiView.SetActiveView(viewDiv);
			
			if (!string.IsNullOrEmpty(form.cssClassContainer))
			{
				panelGenerated.CssClass = form.cssClassContainer;
			}

			if (!string.IsNullOrEmpty(form.cssClassTable))
			{
				divGen.Attributes.Add("class", form.cssClassTable);
			}

			foreach (global::scms.data.scms_form_field field in aFields)
			{
				// add if not an auto generated field
				if (string.Compare(field.type, 0, "auto:", 0, 5, true) != 0)
				{
					if (string.Compare(field.type, "hidden", true) == 0)
					{
						GenerateHiddenField(field);
					}
                    else if(string.Compare(field.type, "literal", true) == 0)
                    {
                        GenerateLiteralField(divGen, field);
                    }
					else
					{
						System.Web.UI.HtmlControls.HtmlGenericControl divRow = new HtmlGenericControl("div");
						divGen.Controls.Add(divRow);

                        string strRowClass = GetFieldRowClass(field);

						if (!string.IsNullOrEmpty(strRowClass))
						{
							divRow.Attributes["class"] = strRowClass;
						}

						// add the div to hold the label
						System.Web.UI.HtmlControls.HtmlGenericControl divLabelCell = new HtmlGenericControl("div");
						divRow.Controls.Add(divLabelCell);
						string strLabelClass = form.cssClassCellLabel;
						if (!string.IsNullOrEmpty(field.cssClassOverrideCellLabel))
						{
							strLabelClass = field.cssClassOverrideCellLabel;
						}
						if( !string.IsNullOrEmpty(strLabelClass))
						{
							divLabelCell.Attributes["class"] = strLabelClass;
						}
						divLabelCell.InnerText = field.label;

						// add the value cell
						System.Web.UI.HtmlControls.HtmlGenericControl divValueCell = new HtmlGenericControl("div");
						divRow.Controls.Add(divValueCell);
						string strValueCellClass = form.cssClassCellValue;
						if (!string.IsNullOrEmpty(field.cssClassOverrideCellValue))
						{
							strValueCellClass = field.cssClassOverrideCellValue;
						}
						if (!string.IsNullOrEmpty(strValueCellClass))
						{
							divValueCell.Attributes.Add("class", strValueCellClass);
						}

						GenerateInputControl(strValidationGroup, dc, form, field, divValueCell);
					}
				}
			}

			GenerateSummaryValidation(strValidationGroup, dc, form);
			GenerateSubmitRow(dc, form);
		}

		protected void GenerateSummaryValidation(string strValidationGroup, global::scms.data.ScmsDataContext dc, global::scms.data.scms_form form)
		{
			if (form.summaryValidationEnabled)
			{
				validationSummary.Enabled = true;
				validationSummary.ValidationGroup = strValidationGroup;
				validationSummary.CssClass = form.summaryValidationCssClass;

				if( !string.IsNullOrEmpty(form.summaryValidationDisplayMode))
				{
					validationSummary.DisplayMode = (ValidationSummaryDisplayMode)Enum.Parse(typeof(ValidationSummaryDisplayMode), form.summaryValidationDisplayMode, true);
				}
				validationSummary.EnableClientScript = form.summaryValidationEnableClientScript.HasValue && form.summaryValidationEnableClientScript.Value;
				validationSummary.HeaderText = form.summaryValidationHeaderText;
				validationSummary.ShowMessageBox = form.summaryValidationShowMessageBox.HasValue && form.summaryValidationShowMessageBox.Value;
				validationSummary.ShowSummary = form.summaryValidationShowSummary.HasValue && form.summaryValidationShowSummary.Value;
			}
			else
			{
				validationSummary.Enabled = false;
			}
		}

		protected void GenerateSubmitRow(global::scms.data.ScmsDataContext dc, global::scms.data.scms_form form)
		{
			string strCssClassInputRow = form.cssClassInputRow;
			if( !string.IsNullOrEmpty(strCssClassInputRow))
			{
				divSubmit.Attributes["class"] = string.Format( "{0} form-submit", strCssClassInputRow );
			}
		}

		protected void GenerateManualForm(global::scms.data.ScmsDataContext dc, global::scms.data.scms_form form, global::scms.data.scms_form_field[] aFields)
		{
			panelGenerated.Visible = false;
			literalManualForm.Text = form.manualFormText;
			AddManualPostbackScript(form);
		}

		protected void SaveValues(scms.data.ScmsDataContext dc, scms.data.scms_form form, scms.data.scms_form_instance formInstance, out int ? nSubmissionId, out System.Collections.Generic.Dictionary<int,scms.data.scms_form_submission_fieldvalue> fieldValuesByFieldId )
		{
			nSubmissionId = null;
			fieldValuesByFieldId = new System.Collections.Generic.Dictionary<int, scms.data.scms_form_submission_fieldvalue>();

			try
			{
				System.Data.Common.DbTransaction trans = dc.Connection.BeginTransaction();
				dc.Transaction = trans;

				scms.data.scms_form_submission submission = new scms.data.scms_form_submission();
				submission.formid = form.id;
				submission.forminstanceid = formInstance.id;
				submission.submissionTime = DateTime.Now;
				dc.scms_form_submissions.InsertOnSubmit(submission);
				dc.SubmitChanges();
				nSubmissionId= submission.id;




				foreach (var kvp in controlFields)
				{
					scms.data.scms_form_field field = (scms.data.scms_form_field)kvp.Value;
					Control control = (Control)kvp.Key;

					string strValue;
					string[] astrValues;
					GetValueFromControl(field, control, out strValue, out astrValues);

					scms.data.scms_form_submission_fieldvalue fieldValue = new scms.data.scms_form_submission_fieldvalue();
					fieldValue.fieldid = field.id;
					fieldValue.formid = form.id;
					fieldValue.formsubmissionid = submission.id;
					fieldValue.value = strValue;
					dc.scms_form_submission_fieldvalues.InsertOnSubmit(fieldValue);

					fieldValuesByFieldId[field.id] = fieldValue;

					if (astrValues != null)
					{
						if (astrValues.Count() > 0)
						{
							foreach (string strOptionValue in astrValues)
							{
								scms.data.scms_form_submission_optionfieldvalue optionvalue = new scms.data.scms_form_submission_optionfieldvalue();
								optionvalue.fieldid = field.id;
								optionvalue.formid = form.id;
								optionvalue.formsubmissionid = submission.id;
								optionvalue.value = strOptionValue;
								dc.scms_form_submission_optionfieldvalues.InsertOnSubmit(optionvalue);
							}
						}
					}
				}

				InsertAutoAndHiddenFieldValues(dc, form, formInstance, submission, aFields, ref fieldValuesByFieldId);

				dc.SubmitChanges();
				dc.Transaction.Commit();
			}
			catch (Exception ex)
			{
                string strMessage = "Failed saving values";
                ScmsEvent.Raise(strMessage, this, ex);
			}
		}

		protected void SaveValuesManual(scms.data.ScmsDataContext dc, scms.data.scms_form form, scms.data.scms_form_instance formInstance, out int? nSubmissionId, out System.Collections.Generic.Dictionary<int, scms.data.scms_form_submission_fieldvalue> fieldValuesByFieldId)
		{
			nSubmissionId = null;
			fieldValuesByFieldId = new System.Collections.Generic.Dictionary<int, scms.data.scms_form_submission_fieldvalue>();

			try
			{
				System.Data.Common.DbTransaction trans = dc.Connection.BeginTransaction();
				dc.Transaction = trans;

				scms.data.scms_form_submission submission = new scms.data.scms_form_submission();
				submission.formid = form.id;
				submission.forminstanceid = formInstance.id;
				submission.submissionTime = DateTime.Now;
				dc.scms_form_submissions.InsertOnSubmit(submission);
				dc.SubmitChanges();
				nSubmissionId = submission.id;

				foreach (scms.data.scms_form_field field in aFields)
				{
					if (string.Compare(field.type, 0, "auto:", 0, "auto:".Length, true) != 0)
					{
						if (string.Compare(field.type, "hidden", true) != 0)
						{
							string strFieldName = field.name;
							string strFieldValue = Request.Params[strFieldName];
							if (!string.IsNullOrEmpty(strFieldValue))
							{
								scms.data.scms_form_submission_fieldvalue fieldValue = new scms.data.scms_form_submission_fieldvalue();
								fieldValue.fieldid = field.id;
								fieldValue.formid = form.id;
								fieldValue.formsubmissionid = submission.id;
								fieldValue.value = strFieldValue;
								dc.scms_form_submission_fieldvalues.InsertOnSubmit(fieldValue);

								fieldValuesByFieldId[field.id] = fieldValue;
							}
						}
					}
				}

				InsertAutoAndHiddenFieldValues(dc, form, formInstance, submission, aFields, ref fieldValuesByFieldId);

				dc.SubmitChanges();
				dc.Transaction.Commit();
			}
			catch (Exception ex)
			{
                string strMessage = "Failed saving values";
                ScmsEvent.Raise(strMessage, this, ex);
			}
		}

		protected void InsertAutoAndHiddenFieldValues(scms.data.ScmsDataContext dc, scms.data.scms_form form, scms.data.scms_form_instance formInstance, scms.data.scms_form_submission submission, scms.data.scms_form_field[] aFields, ref System.Collections.Generic.Dictionary<int, scms.data.scms_form_submission_fieldvalue> fieldValuesByFieldId)
		{
			// insert hidden + auto field values
			foreach (scms.data.scms_form_field field in aFields)
			{
				bool bInsert = false;
				string strValue = null;


				switch (field.type)
				{
					case "hidden":
						bInsert = true;
						strValue = field.defaultText;
						break;

					case "auto:ipaddress":
						bInsert = true;
						strValue = Request.UserHostAddress;
						break;

					case "auto:url":
						bInsert = true;
						strValue = Request.RawUrl;
						break;

					case "auto:referrer":
						bInsert = true;
						strValue = Request.UrlReferrer.ToString();
						break;

                    case "auto:userid":
                        bInsert = true;
                        strValue = null;
                        if (Page.User != null)
                        {
                            if (Page.User.Identity != null)
                            {
                                strValue = Page.User.Identity.Name;
                            }
                        }
                        break;
				}

				if (bInsert)
				{
					scms.data.scms_form_submission_fieldvalue fieldValue = new scms.data.scms_form_submission_fieldvalue();
					fieldValue.fieldid = field.id;
					fieldValue.formid = form.id;
					fieldValue.formsubmissionid = submission.id;
					fieldValue.value = strValue;
					dc.scms_form_submission_fieldvalues.InsertOnSubmit(fieldValue);

					fieldValuesByFieldId[field.id] = fieldValue;
				}
			}
		}

		protected void NotifyAdministrator(scms.data.ScmsDataContext dc, scms.data.scms_form form, scms.data.scms_form_instance formInstance, int nSubmissionId, System.Collections.Generic.Dictionary<int, scms.data.scms_form_submission_fieldvalue> fieldValuesByFieldId)
		{
			try
			{
				if (form.notify)
				{
					// get to email addresses
					string[] astrEmails = form.notifyEmail.Split(new char[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries);
					System.Collections.Generic.List<string> lstrEmails = new System.Collections.Generic.List<string>();
					lstrEmails.AddRange(astrEmails);

					var notifyByFieldIds = from n in dc.scms_form_notify_by_fields
																 where n.formId == form.id
																 orderby n.id
																 select n;
					foreach (var notifyByFieldId in notifyByFieldIds)
					{
						if (!string.IsNullOrEmpty(notifyByFieldId.value) && !string.IsNullOrEmpty(notifyByFieldId.email))
						{
							scms.data.scms_form_submission_fieldvalue value = null;
							if (fieldValuesByFieldId.TryGetValue(notifyByFieldId.fieldId, out value))
							{
								if (!string.IsNullOrEmpty(value.value))
								{
									if (value.value.IndexOf(notifyByFieldId.value, StringComparison.OrdinalIgnoreCase) >= 0)
									{
										if (!lstrEmails.Contains(notifyByFieldId.email))
										{
											lstrEmails.Add(notifyByFieldId.email);
										}
									}
								}
							}
						}
					}


					if (lstrEmails.Count > 0)
					{
						System.Text.StringBuilder sbBody = new System.Text.StringBuilder();
						sbBody.AppendFormat("form submission received for form '{0}', on {1}.<br /><br />", form.name, Request.RawUrl);
						sbBody.Append("<table>\r\n");

						var fieldOptionValues = from sfv in dc.scms_form_submission_fieldvalues
																		where sfv.formsubmissionid == nSubmissionId
																		select sfv;


						foreach( scms.data.scms_form_field field in aFields )
						{
							string strValue = null;
              if (field.redactInEmail.HasValue && field.redactInEmail.Value)
              {
                  strValue = "***REDACTED***";
              }
              else
              {
                  scms.data.scms_form_submission_fieldvalue value = null;
                  if (fieldValuesByFieldId.TryGetValue(field.id, out value))
                  {
										if (field.type == "fileupload")
										{
											if (!string.IsNullOrEmpty(value.value))
											{
												System.Text.StringBuilder sb = new System.Text.StringBuilder();
												sb.Append("http://");
												sb.Append(Request.Url.Host);
												if (Request.Url.Port != 80)
												{
													sb.Append(":");
													sb.Append(Request.Url.Port.ToString());
												}

												sb.Append(value.value);
												string strUrl = sb.ToString();
												strValue = string.Format("<a href=\"{0}\">{0}</a>", strUrl);
											}
										}
										else
										{
											strValue = value.value;
										}
                  }
              }
							sbBody.AppendFormat("<tr><td><strong>{0}</strong></td><td>{1}</td></tr>\r\n", field.name, strValue);
						}
						sbBody.Append("</table>\r\n");

						System.Net.Mail.SmtpClient client = new System.Net.Mail.SmtpClient();
						System.Net.Mail.MailMessage message = new System.Net.Mail.MailMessage();
						message.IsBodyHtml = true;
						message.Body = sbBody.ToString();
						message.Subject = "Form submission received";

						foreach (string strEmail in lstrEmails)
						{
							message.To.Clear();
							message.To.Add(strEmail);
							try
							{
								client.Send(message);
							}
							catch(Exception ex)
							{
								string strMessage = string.Format("Failed sending message to '{0}'", strEmail);
								ScmsEvent.Raise(strMessage, this, ex);
							}
						}
					}
				}
			}
			catch (Exception ex)
			{
                string strMessage = "Exception thrown while notifying administrator";
                ScmsEvent.Raise(strMessage, this, ex);
			}
		}

		protected void PrepareSpamProtections(scms.data.scms_form form)
		{
			if (form.validateSession)
			{
				string strKey = string.Format("spam-protection-{0}", form.id);
				Session[strKey] = true;
			}

			if (form.validateDummyFields)
			{
				panelInputs.Visible = true;
			}
		}

		protected bool ValidateDummyFields(scms.data.scms_form form)
		{
			bool bValid = true;

			bool bSkipEmail = false;
			bool bSkipUrl = false;
			bool bSkipPhone = false;

			if (!form.generateForm || (form.generateForm && (string.Compare(form.generationType, "manual", true) == 0 )))
			{
				foreach (scms.data.scms_form_field field in aFields)
				{
					if (string.Compare(field.name, "email", true) == 0)
					{
						bSkipEmail = true;
					}
					else
					{
						if (string.Compare(field.name, "url", true) == 0)
						{
							bSkipPhone = true;
						}
						else
						{
							if (string.Compare(field.name, "phone", true) == 0)
							{
								bSkipPhone = true;
							}
						}
					}

					
				}
			}

			if (!bSkipEmail)
			{
				string strRequestEmail = Request.Form["email"];
				if (!string.IsNullOrEmpty(strRequestEmail))
				{
					bValid = false;
				}
			}

			if (!bSkipUrl)
			{
				string strRequestUrl = Request.Form["url"];
				if (!string.IsNullOrEmpty(strRequestUrl))
				{
					bValid = false;
				}
			}

			if (!bSkipPhone)
			{
				string strRequestPhone = Request.Form["phone"];
				if (!string.IsNullOrEmpty(strRequestPhone))
				{
					bValid = false;
				}
			}
			

			return bValid;
		}
		

		protected bool ValidateSpamProtections(scms.data.scms_form form)
		{
			bool bVerifiedOrNotRequired = true;

			if (bVerifiedOrNotRequired)
			{
				if (form.validateSession)
				{
					if (!ValidateSessionKey(form))
					{
						bVerifiedOrNotRequired = false;
						// TODO log this
					}
				}
			}

			if (bVerifiedOrNotRequired)
			{
				if (form.validateReferrer)
				{
					if (!ValidateReferrer(form))
					{
						bVerifiedOrNotRequired = false;
						// TODO log this
					}
				}
			}

			if (bVerifiedOrNotRequired)
			{
				if (form.validateDummyFields)
				{
					if (!ValidateDummyFields(form))
					{
						bVerifiedOrNotRequired = false;
						// TODO log this
					}
				}
			}

			return bVerifiedOrNotRequired;
		}

		protected bool ValidateSessionKey(scms.data.scms_form form)
		{
			bool bVerified = false;

			string strKey = string.Format("spam-protection-{0}", form.id);
			object objValue = Session[strKey];
			if (objValue != null)
			{
				bVerified = true;
			}
			else
			{
				// TODO log this occurrence
			}

			return bVerified;
		}

		protected bool ValidateReferrer(scms.data.scms_form form)
		{
			bool bValid = false;

			// make sure it's this page
			scms.data.ScmsDataContext dc = new scms.data.ScmsDataContext();
			var page = (from p in dc.scms_pages
									where p.deleted == false
									where p.id == PageId
									select p).Single();
			string strUrl = page.url;
			string [] astrRequestPath = Request.RawUrl.Split(new char [] { '?' } );
			string strRequestPath = null;
			if (astrRequestPath.Length > 0)
			{
				strRequestPath = astrRequestPath[0];
				if (string.Compare(strRequestPath, "/default.aspx", true) == 0)
				{
					strRequestPath = "/";
				}
			}
			if (string.Compare(strUrl, strRequestPath, true) == 0)
			{
				bValid = true;
			}
			else
			{
				// TODO log this occurrence
			}

			return bValid;
		}

		protected void btnSubmit_Click(object sender, EventArgs args)
		{
            string strValidationGroup = string.Format("module-{0}", ModuleInstanceId.Value);
            Page.Validate(strValidationGroup);
			if (this.Page.IsValid)
			{
				try
				{
					// get values
					scms.data.ScmsDataContext dc = new scms.data.ScmsDataContext();
					dc.Connection.Open();


					var formDetails = (from fi in dc.scms_form_instances
														 where fi.instanceId == ModuleInstanceId.Value
														 join f in dc.scms_forms on fi.formid equals f.id
														 where f.deleted == false
														 select new { form = f, instance = fi }).Single();

					if( ValidateSpamProtections(formDetails.form))
					{
						int? nSubmissionId = null;
						System.Collections.Generic.Dictionary<int, scms.data.scms_form_submission_fieldvalue> fieldValuesByFieldId;
						SaveValues(dc, formDetails.form, formDetails.instance, out nSubmissionId, out fieldValuesByFieldId);
                        if (nSubmissionId.HasValue)
                        {
                            ProcessEvents(dc, form, nSubmissionId.Value, fieldValuesByFieldId);
                        }
						NotifyAdministrator(dc, formDetails.form, formDetails.instance, nSubmissionId.Value, fieldValuesByFieldId);
						RedirectToThankYouPage(dc, formDetails.form);
					}
				}
				catch (System.Threading.ThreadAbortException)
				{
				}
				catch (Exception ex)
				{
                    string strMessage = "Exception thrown during submit";
                    ScmsEvent.Raise(strMessage, this, ex);
				}
			}
		}

		protected void RedirectToThankYouPage(scms.data.ScmsDataContext dc, scms.data.scms_form form)
		{
			try
			{
				int ? nThankYouPageId = null;
				if (form.thankYouPageId.HasValue)
				{
					nThankYouPageId = form.thankYouPageId;
				}
				else
				{
					var site = (from s
											in dc.scms_sites
											where s.id == this.SiteId
											select s).Single();
					nThankYouPageId = site.homePageId.Value;
				}

				var page = (from p
										in dc.scms_pages
										where p.id == nThankYouPageId
										select p).FirstOrDefault();
				string strUrl = "/";
				if (page != null)
				{
					strUrl = page.url;
				}
				
				Response.Redirect(strUrl, false);
			}
			catch (Exception ex)
			{
                string strMessage = "Failed during redirect to thank you page";
                ScmsEvent.Raise(strMessage, this, ex);
			}
		}



		protected void GetValueFromControl(scms.data.scms_form_field field, Control control, out string strValue, out string[] astrValues)
		{
			strValue = null;
			astrValues = null;

			switch (field.type)
			{
				case "text":
				case "textarea":
					{
						TextBox textBox = (TextBox)control;
						strValue = textBox.Text.Trim();
					}
					break;

				case "checkbox":
					{
						CheckBox checkBox = (CheckBox)control;
						strValue = checkBox.Checked ? "true" : "false";
					}
					break;

				case "checkboxlist":
					{
						CheckBoxList checkBoxList = (CheckBoxList)control;
						System.Collections.Generic.List<string> lvalues = new System.Collections.Generic.List<string>();
						strValue = string.Empty;
						foreach( ListItem item in checkBoxList.Items )
						{
							if( item.Selected )
							{
								lvalues.Add(item.Value);
								if (!string.IsNullOrEmpty(strValue))
								{
									strValue += ", ";
								}
								strValue += item.Value;
							}
						}
						astrValues = lvalues.ToArray();
					}
					break;

				case "dropdownlist":
					{
						DropDownList ddl = (DropDownList)control;
						strValue = ddl.SelectedValue;
					}
					break;

				case "radiobuttonlist":
					{
						RadioButtonList rbl = (RadioButtonList)control;
						strValue = rbl.SelectedValue;
					}
					break;

      case "fileupload":
          {
              FileUpload fileUpload = (FileUpload)control;
              if (fileUpload.HasFile)
              {
                  string strUploadedFileName = fileUpload.FileName;
                  try
                  {
                      string strExtension = System.IO.Path.GetExtension(strUploadedFileName);
                      string strFileNameWithoutException = Guid.NewGuid().ToString();
                      string strFileName = string.Format("{0}{1}", strFileNameWithoutException, strExtension );
                      string strFileDirectory = GetFileDirectory();
                      EnsureDirectory(strFileDirectory);
                      string strFilePath = System.IO.Path.Combine(strFileDirectory, strFileName).ToLower();
                      fileUpload.SaveAs(strFilePath);

                      string strApplicationPath = HttpContext.Current.Server.MapPath("~").ToLower();
                      strValue = strFilePath.Replace(strApplicationPath, "/").Replace('\\','/');
                  }
                  catch (Exception ex)
                  {
                      string strMessage = string.Format( "Exception thrown while saving uploaded file '{0}' of size '{1}'", strUploadedFileName, fileUpload.FileBytes.Length );
                      ScmsEvent.Raise(strMessage, this, ex);
                      throw new Exception( "Failed saving file");
                  }
              }
          }
          break;
			}
		}

        protected bool EnsureDirectory(string strFileDirectory)
        {
            bool bPathfound = false;

            strFileDirectory = strFileDirectory.TrimEnd(new char[] { '\\' });

            try
            {
                if (System.IO.Directory.Exists(strFileDirectory))
                {
                    bPathfound = true;
                }
                else
                {
                    int nLastSlash = strFileDirectory.LastIndexOf('\\');
                    if (nLastSlash > 0)
                    {
                        string strParentDirectory = strFileDirectory.Substring(0, nLastSlash);
                        if (EnsureDirectory(strParentDirectory))
                        {
                            System.IO.Directory.CreateDirectory(strFileDirectory);
                            bPathfound = true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                string strMessage = string.Format( "Exception thrown while ensuring path '{0}'.", strFileDirectory );
                ScmsEvent.Raise(strFileDirectory, this, ex);
            }

            return bPathfound;
        }

        protected string GetFileDirectory()
        {
            string strFileDirectory = null;
            scms.ScmsSiteMapProvider provider = new ScmsSiteMapProvider();
            scms.ScmsSiteMapProvider.Site site;
            string strError;
            Exception exError;
            if (provider.GetSite(SiteId.Value, out site, out strError, out exError))
            {
                string strRootFilesLocation = site.site.filesLocation;
                string strRootPath = Server.MapPath(strRootFilesLocation);
                strFileDirectory = System.IO.Path.Combine(strRootPath, "forms");
                strFileDirectory = System.IO.Path.Combine(strFileDirectory, form.id.ToString());
            }

            return strFileDirectory;
        }

		// http://www.dotnetspider.com/resources/1521-How-call-Postback-from-Javascript.aspx
		protected void AddManualPostbackScript(global::scms.data.scms_form form)
		{
			string strScriptKey = string.Format("ManualPostback_{0}", form.id );

			string strManualSubmitScript = this.Page.ClientScript.GetPostBackEventReference(this, "ManualSubmit", false);

			string strScript = string.Format(@"
function ManualSubmit_{0}()
{{
	{1};
}}
", form.id, strManualSubmitScript);
			this.Page.ClientScript.RegisterStartupScript(typeof(string), strScriptKey, strScript, true);
		}

		protected void ManualSubmit()
		{
			try
			{
				// get values
				scms.data.ScmsDataContext dc = new scms.data.ScmsDataContext();
				dc.Connection.Open();


				var formDetails = (from fi in dc.scms_form_instances
													 where fi.instanceId == ModuleInstanceId.Value
													 join f in dc.scms_forms on fi.formid equals f.id
													 where f.deleted == false
													 select new { form = f, instance = fi }).Single();
				if (ValidateSpamProtections(formDetails.form))
				{
					int? nSubmissionId = null;
					System.Collections.Generic.Dictionary<int, scms.data.scms_form_submission_fieldvalue> fieldValuesByFieldId;
					SaveValuesManual(dc, formDetails.form, formDetails.instance, out nSubmissionId, out fieldValuesByFieldId);
                    if (nSubmissionId.HasValue)
                    {
                        ProcessEvents(dc, form, nSubmissionId.Value, fieldValuesByFieldId);
                    }
					NotifyAdministrator(dc, formDetails.form, formDetails.instance, nSubmissionId.Value, fieldValuesByFieldId);
					RedirectToThankYouPage(dc, formDetails.form);
				}
			}
			catch (System.Threading.ThreadAbortException)
			{
			}
			catch (Exception ex)
			{
                string strMessage = "Failed during manual submit";
                ScmsEvent.Raise(strMessage, this, ex);
			}
		}

        protected bool ProcessEvents(scms.data.ScmsDataContext dc, scms.data.scms_form form, int nSubmissionId, System.Collections.Generic.Dictionary<int, scms.data.scms_form_submission_fieldvalue> fieldValuesByFieldId)
        {
            bool bAnyErrors = false;

            try
            {
                var eventHandlers = from eh in dc.scms_form_eventhandlers
                             where eh.formid == form.id
                             where eh.deleted == false
                             orderby eh.ordinal
                             select eh;
                foreach (var eventHandler in eventHandlers)
                {
                    if (!ProcessEvent(form, nSubmissionId, eventHandler, fieldValuesByFieldId))
                    {
                        bAnyErrors = true;
                    }
                }
            }
            catch (Exception ex)
            {
                ScmsEvent.Raise("Failed looking up event handlers for form", this, ex);
                bAnyErrors = true;
            }

            return !bAnyErrors;
        }

        protected bool ProcessEvent(scms.data.scms_form form, int nSubmissionId, scms.data.scms_form_eventhandler eventHandler, System.Collections.Generic.Dictionary<int, scms.data.scms_form_submission_fieldvalue> fieldValuesByFieldId)
        {
            bool bSuccess = false;

            try
            {
                var eventHandlerTypes = global::scms.modules.forms.Forms.GetEventHandlers();
                var eventHandlerType = (from eht in eventHandlerTypes 
                                        where eht.UniqueName == eventHandler.eventName
                                        select eht).FirstOrDefault();
                if (eventHandlerType != null)
                {
                    bSuccess = eventHandlerType.FormSubmitted(eventHandler.id, ModuleInstanceId.Value, form.id, nSubmissionId, fieldValuesByFieldId);
                }
                else
                {
                    string strError = string.Format("Event handler of type '{0}' not found.", eventHandler.eventName);
                    ScmsEvent.Raise(strError, this, null);
                }
            }
            catch (Exception ex)
            {
                string strMessage = string.Format("Failed processing event '{0}'.", eventHandler.eventName);
                ScmsEvent.Raise(strMessage, this, ex);
            }

            return bSuccess;
        }

	}
}