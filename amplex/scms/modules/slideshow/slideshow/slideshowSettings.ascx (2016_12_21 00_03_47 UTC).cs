using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace scms.modules.slideshow
{
    public partial class slideshowSettings : global::scms.RootControl
    {
        public int? SlideShowId
        {
            get;
            set;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadSettings();
                EnableControls();
            }
        }

        protected void LoadSettings()
        {
            if (SlideShowId.HasValue)
            {
                global::scms.data.ScmsDataContext dc = new global::scms.data.ScmsDataContext();
                scms.data.scms_slideshow slideshow = (from s in dc.scms_slideshows
                                                      where s.id == SlideShowId.Value
                                                      select s).FirstOrDefault();
                if (slideshow != null)
                {
                    ddlType.SelectedValue = slideshow.type;

                    txtWidth.Text = null;
                    if (slideshow.width.HasValue)
                    {
                        txtWidth.Text = slideshow.width.ToString();
                    }

                    txtHeight.Text = null;
                    if (slideshow.height.HasValue)
                    {
                        txtHeight.Text = slideshow.height.ToString();
                    }

                    ddlTransitionType.SelectedValue = slideshow.transitionType;

                    txtTransitionSpeed.Text = null;
                    if (slideshow.transitionTimeMs.HasValue)
                    {
                        txtTransitionSpeed.Text = slideshow.transitionTimeMs.Value.ToString();
                    }

                    txtPauseTime.Text = null;
                    if (slideshow.pauseTimeMs.HasValue)
                    {
                        txtPauseTime.Text = slideshow.pauseTimeMs.Value.ToString();
                    }

                    checkRandom.Checked = slideshow.random;
                    checkPauseOnHover.Checked = slideshow.hoverPause;
                    checkAdvanceOnClick.Checked = slideshow.clickAdvance;

                    checkHasSelectorButtons.Checked = slideshow.hasSelectorButtons.HasValue && slideshow.hasSelectorButtons.Value;

                    txtHeaderTemplate.Text = slideshow.headerTemplate;
                    txtFooterTemplate.Text = slideshow.footerTemplate;
                    txtItemTemplate.Text = slideshow.itemTemplate;
                    txtHeaderScript.Text = slideshow.headerScript;

                    checkUsePager.Checked = slideshow.showPager;
                    txtPagerCssClass.Text = null;
                    if (checkUsePager.Checked)
                    {
                        if (!string.IsNullOrEmpty(slideshow.pagerLocation))
                        {
                            ListItem liPagerLocation = ddlPagerLocation.Items.FindByValue(slideshow.pagerLocation);
                            if (liPagerLocation != null)
                            {
                                ddlPagerLocation.ClearSelection();
                                liPagerLocation.Selected = true;
                            }
                        }

                        if (!string.IsNullOrEmpty(slideshow.pagerClass))
                        {
                            txtPagerCssClass.Text = slideshow.pagerClass;
                        }

                        if (!string.IsNullOrEmpty(slideshow.pagerType))
                        {
                            ListItem liPagerType = ddlPagerType.Items.FindByValue(slideshow.pagerType);
                            if (liPagerType != null)
                            {
                                ddlPagerType.ClearSelection();
                                liPagerType.Selected = true;
                            }

                            if (string.Compare(slideshow.pagerType, "thumbnail", true) == 0)
                            {
                                txtPagerWidth.Text = slideshow.pageThumbnailWidth.ToString();
                                txtPagerHeight.Text = slideshow.pagerThumbnailHeight.ToString();
                            }
                        }

                    }
                }
            }
            else
            {
                statusMessage.ShowFailure("Error, slideshow id is missing");
            }
        }

        protected void checkAdvanced_CheckedChanged(object sender, EventArgs args)
        {
            EnableControls();
        }

        protected void ddlType_SelectedIndexChanged(object sender, EventArgs args)
        {
            EnableControls();
        }

        protected void checkUsePager_CheckedChanged(object sender, EventArgs args)
        {
            EnableControls();
        }

        protected void EnableControls()
        {
            bool bShowSize = false;
            bool bShowAdvancedCheckbox = false;
            bool bShowTransition = false;
            bool bShowItemTemplate = false;
            bool bShowTemplates = false;
            bool bShowSelectorButtons = false;
            bool bShowHeaderScript = false;
            bool bShowPager = false;

            string strType = ddlType.SelectedValue.ToLower();
            switch (strType)
            {
                case "basic":
                    bShowSize = true;
                    bShowTransition = true;
                    bShowPager = true;
                    break;

                case "template":
                    bShowSize = true;
                    bShowTransition = true;
                    bShowAdvancedCheckbox = true;
                    bShowItemTemplate = checkAdvanced.Checked;
                    bShowSelectorButtons = checkAdvanced.Checked;
                    bShowPager = true;
                    break;

                case "custom":
                    bShowSize = false;
                    bShowTransition = true;
                    bShowAdvancedCheckbox = true;
                    bShowItemTemplate = checkAdvanced.Checked;
                    bShowTemplates = checkAdvanced.Checked;
                    bShowHeaderScript = checkAdvanced.Checked;
                    break;
            }

            placeholderSize.Visible = bShowSize;
            placeholderSelectorButtons.Visible = bShowSize;
            placeholderTransition.Visible = bShowTransition;
            placeholderAdvanced.Visible = bShowAdvancedCheckbox;
            placeholderItemTemplate.Visible = bShowItemTemplate;
            placeholderTemplates.Visible = bShowTemplates;
            placeholderSelectorButtons.Visible = bShowSelectorButtons;
            placeholderHeaderScript.Visible = bShowHeaderScript;
            placeholderPagerSettings.Visible = bShowPager;
        }

        protected void btnSave_Click(object sender, EventArgs args)
        {
            if (Page.IsValid)
            {
                try
                {
                    scms.data.ScmsDataContext dc = new scms.data.ScmsDataContext();

                    scms.data.scms_slideshow slideshow = null;
                    slideshow = (from s in dc.scms_slideshows
                                 where s.id == SlideShowId.Value
                                 select s).Single();
                    slideshow.type = ddlType.SelectedValue;
                    slideshow.headerTemplate = txtHeaderTemplate.Text;
                    slideshow.footerTemplate = txtFooterTemplate.Text;
                    slideshow.itemTemplate = txtItemTemplate.Text;
                    slideshow.headerScript = txtHeaderScript.Text;

                    slideshow.width = null;
                    int n;
                    if (int.TryParse(txtWidth.Text, out n))
                    {
                        slideshow.width = n;
                    }

                    slideshow.height = null;
                    if (int.TryParse(txtHeight.Text, out n))
                    {
                        slideshow.height = n;
                    }

                    slideshow.transitionType = ddlTransitionType.SelectedValue;
                    slideshow.transitionTimeMs = null;
                    if (int.TryParse(txtTransitionSpeed.Text, out n))
                    {
                        slideshow.transitionTimeMs = n;
                    }

                    if (int.TryParse(txtPauseTime.Text, out n))
                    {
                        slideshow.pauseTimeMs = n;
                    }

                    slideshow.random = checkRandom.Checked;
                    slideshow.hoverPause = checkPauseOnHover.Checked;
                    slideshow.clickAdvance = checkAdvanceOnClick.Checked;

                    slideshow.showPager = checkUsePager.Checked;
                    slideshow.pagerLocation = null;
                    slideshow.pagerType = null;
                    slideshow.pagerClass = null;

                    slideshow.pageThumbnailWidth = null;
                    slideshow.pagerThumbnailHeight = null;
                    if (slideshow.showPager)
                    {
                        slideshow.pagerLocation = ddlPagerLocation.SelectedValue;
                        slideshow.pagerType = ddlPagerType.SelectedValue;
                        slideshow.pagerClass = txtPagerCssClass.Text;

                        if (string.Compare(slideshow.pagerType, "thumbnail", true) == 0)
                        {
                            string strThumbnailWidth = txtPagerWidth.Text.Trim();
                            if (int.TryParse(strThumbnailWidth, out n))
                            {
                                slideshow.pageThumbnailWidth = n;
                            }

                            string strThumbnailHeight = txtPagerHeight.Text.Trim();
                            if (int.TryParse(strThumbnailHeight, out n))
                            {
                                slideshow.pagerThumbnailHeight = n;
                            }
                        }


                    }

                    slideshow.hasSelectorButtons = checkHasSelectorButtons.Checked;
                    dc.SubmitChanges();

                    statusMessage.ShowSuccess("Settings updated");
                }
                catch (Exception ex)
                {
                    string strMessage = "Failed saving settings";
                    statusMessage.ShowFailure(strMessage);
                    ScmsEvent.Raise(strMessage, this, ex);
                }
            }
        }

        protected void btnLoadTemplate_Click(object sender, EventArgs args)
        {
            string strImageTitleContent = @"
<div style=""background-color:#000;width:$WIDTH$px;height:$HEIGHT$px""> 
 <div style=""padding:10px;"" >
  <a href=""$LINK$""><img src=""$IMAGE$""/></a>
  <h2 style=""color:#ccc;margin-top:10px;"">$HEADING$</h2>
  <div style=""color:#bbb;"">$CONTENT$</div>
 </div>
</div>
";
            txtItemTemplate.Text = strImageTitleContent;

        }

        protected void cvPagerType_ServerValidate(object sender, ServerValidateEventArgs args)
        {
            if (checkUsePager.Checked)
            {
                if (string.Compare(ddlPagerLocation.SelectedValue, "content", true) == 0)
                {
                    if (string.Compare(ddlPagerType.SelectedValue, "custom", true) != 0)
                    {
                        cvPagerType.ErrorMessage = "Content managed pager must be of type 'Custom'";
                        args.IsValid = false;
                    }
                }
            }
        }
    }
}