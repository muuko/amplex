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

namespace scms.modules.navigation.subnav
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
                    var subnavInstance = (from sni in dc.scms_navigation_subnavs
                                          where sni.instanceId == this.ModuleInstanceId.Value
                                          select sni).FirstOrDefault();
                    if (subnavInstance != null)
                    {
                        txtCssClassActive.Text = subnavInstance.cssClassActive;

                        if (subnavInstance.maxDepth.HasValue)
                        {
                            txtMaximumDepth.Text = subnavInstance.maxDepth.ToString();
                        }

                        checkShowChildren.Checked = subnavInstance.showChildren;
												if (checkShowChildren.Checked)
												{
													if (subnavInstance.maxChildrenPerNode.HasValue)
													{
														txtMaxChildrenPerNode.Text = subnavInstance.maxChildrenPerNode.ToString();
													}
												}

                        checkPinNavigationToHomePage.Checked = subnavInstance.pinNavigationToHomePage;
                        if (checkPinNavigationToHomePage.Checked)
                        {
                            if (subnavInstance.pinDepth.HasValue)
                            {
                                txtPinDepth.Text = subnavInstance.pinDepth.ToString();
                            }
                        }

                        checkShowSiblingsIfNoChildren.Checked = subnavInstance.showSiblingsIfNoChildren;
                    }
                    else
                    {
                        txtCssClassActive.Text = "active";
                        txtMaximumDepth.Text = "2";
                        checkShowChildren.Checked = true;
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

        protected void btnSave_Click(object sender, EventArgs args)
        {
            try
            {
                global::scms.data.ScmsDataContext dc = new scms.data.ScmsDataContext();
                var subnavInstance = (from sni in dc.scms_navigation_subnavs
                                      where sni.instanceId == this.ModuleInstanceId.Value
                                      select sni).FirstOrDefault();

                if (subnavInstance == null)
                {
                    subnavInstance = new scms.data.scms_navigation_subnav();
                    subnavInstance.instanceId = this.ModuleInstanceId.Value;
                    dc.scms_navigation_subnavs.InsertOnSubmit(subnavInstance);
                }

                subnavInstance.cssClassActive = txtCssClassActive.Text.Trim();

                string strMaximumDepth = txtMaximumDepth.Text.Trim();
                if (string.IsNullOrEmpty(strMaximumDepth))
                {
                    subnavInstance.maxDepth = null;
                }
                else
                {
                    subnavInstance.maxDepth = int.Parse(strMaximumDepth);
                }

                subnavInstance.showChildren = checkShowChildren.Checked;
								string strMaxChildrenPerNode = txtMaxChildrenPerNode.Text;
								if (!string.IsNullOrEmpty(strMaxChildrenPerNode))
								{
									subnavInstance.maxChildrenPerNode = int.Parse(strMaxChildrenPerNode);
								}
								else
								{
									subnavInstance.maxChildrenPerNode = null;
								}

                subnavInstance.pinNavigationToHomePage = checkPinNavigationToHomePage.Checked;
                subnavInstance.pinDepth = null;
                if (subnavInstance.pinNavigationToHomePage)
                {
                    if (!string.IsNullOrEmpty(txtPinDepth.Text))
                    {
                        subnavInstance.pinDepth = int.Parse(txtPinDepth.Text);
                    }
                }

                subnavInstance.showSiblingsIfNoChildren = checkShowSiblingsIfNoChildren.Checked;

                dc.SubmitChanges();
                statusMessage.ShowSuccess("Changes saved");

                ViewSubnavControl.ClearCache();

            }
            catch (Exception ex)
            {
                string strMessage = "Failed saving changes";
                ScmsEvent scmsEvent = new ScmsEvent(strMessage, this, ex);
                scmsEvent.Raise();
                statusMessage.ShowFailure(strMessage);

            }
        }

        protected void checkPinNavigationToHomePage_CheckedChanged(object sender, EventArgs args)
        {
            if (!checkPinNavigationToHomePage.Checked)
                txtPinDepth.Text = null;
            EnableControls();
        }

				protected void checkShowChildren_checkedChanged(object sender, EventArgs args)
				{
					if (!checkShowChildren.Checked)
						txtMaxChildrenPerNode.Text = null;
					EnableControls();
				}

        protected void EnableControls()
        {
            txtPinDepth.Enabled = checkPinNavigationToHomePage.Checked;
						txtMaxChildrenPerNode.Enabled = checkShowChildren.Checked;
        }
    }
}