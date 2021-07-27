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

namespace scms.modules.navigation.pagedetail
{
    public partial class edit : global::scms.RootControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                try
                {
                    global::scms.data.ScmsDataContext dc = new scms.data.ScmsDataContext();
                    var pagedetailInstance = (from pdi in dc.scms_navigation_pagedetails
                                              where pdi.instanceId == this.ModuleInstanceId.Value
                                              select pdi).FirstOrDefault();
                    if (pagedetailInstance != null)
                    {
                        ddlType.SelectedValue = pagedetailInstance.detailType;
                        checkWrapInHtmlElement.Checked = pagedetailInstance.wrapDetailInHtmlElement;
                        if (checkWrapInHtmlElement.Checked)
                        {
                            txtHtmlElementType.Text = pagedetailInstance.wrapElementType;
                            txtHtmlCssClass.Text = pagedetailInstance.cssClassWrap;
                        }
                    }

                    EnableControls();

                }
                catch (Exception ex)
                {
                    // TODO log error
                    throw ex;
                }
            }

        }

        protected void checkWrapInHtmlElement_checkChanged(object sender, EventArgs args)
        {
            EnableControls();
        }

        protected void EnableControls()
        {
            bool bEnabled = checkWrapInHtmlElement.Checked;
            txtHtmlElementType.Enabled = bEnabled;
            txtHtmlCssClass.Enabled = bEnabled;
        }

        protected void cvHtmlElementType_ServerValidate(object sender, ServerValidateEventArgs args)
        {
            bool bValid = false;

            if (checkWrapInHtmlElement.Checked)
            {
                string strText = txtHtmlElementType.Text;
                if (!string.IsNullOrEmpty(strText))
                {
                    bool bAllLetters = true;
                    foreach (char ch in strText)
                    {
                        if (!char.IsLetterOrDigit(ch))
                        {
                            bAllLetters = false;
                        }
                    }
                    if (bAllLetters)
                    {
                        bValid = true;
                    }
                }
            }
            else
            {
                bValid = true;
            }


            args.IsValid = bValid;
        }

        protected void btnSave_Click(object sender, EventArgs args)
        {
            try
            {
                global::scms.data.ScmsDataContext dc = new scms.data.ScmsDataContext();
                var pagedetailInstance = (from pdi in dc.scms_navigation_pagedetails
                                          where pdi.instanceId == this.ModuleInstanceId.Value
                                          select pdi).FirstOrDefault();
                if (pagedetailInstance == null)
                {
                    pagedetailInstance = new scms.data.scms_navigation_pagedetail();
                    pagedetailInstance.instanceId = this.ModuleInstanceId.Value;
                    dc.scms_navigation_pagedetails.InsertOnSubmit(pagedetailInstance);
                }
                pagedetailInstance.detailType = ddlType.SelectedValue;
                pagedetailInstance.wrapDetailInHtmlElement = checkWrapInHtmlElement.Checked;

                pagedetailInstance.wrapElementType = null;
                pagedetailInstance.cssClassWrap = null;
                if (pagedetailInstance.wrapDetailInHtmlElement)
                {
                    pagedetailInstance.wrapElementType = txtHtmlElementType.Text.Trim();
                    pagedetailInstance.cssClassWrap = txtHtmlCssClass.Text.Trim();
                }

                dc.SubmitChanges();
                statusMessage.ShowSuccess("Changes saved");
            }
            catch (Exception ex)
            {
                string strMessage = "Failed saving changes";
                ScmsEvent scmsEvent = new ScmsEvent(strMessage, this, ex);
                scmsEvent.Raise();
                statusMessage.ShowFailure(strMessage);
            }
        }

    }
}