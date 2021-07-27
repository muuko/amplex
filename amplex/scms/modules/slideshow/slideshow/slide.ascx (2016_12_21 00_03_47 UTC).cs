using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace scms.modules.slideshow
{
    public partial class slide : scms.RootControl
    {
        public int? SlideShowId
        {
            get;
            set;
        }

        public int? SlideId
        {
            get { return (int?)ViewState["SlideId"]; }
            set { ViewState["SlideId"] = value; }
        }

        public bool LoadSlide(out string strError)
        {
            bool bSuccess = false;
            strError = null;

            try
            {
                txtName.Text = null;
                txtHeading.Text = null;
                selectImage.Path = null;
                txtLinkUrl.Text = null;
                txtContent.Text = null;
                checkAvanced.Checked = false;
                txtCustomOverride.Text = null;


                if (SlideId.HasValue)
                {
                    scms.data.ScmsDataContext dc = new scms.data.ScmsDataContext();
                    var slide = (from s in dc.scms_slideshow_slides
                                 where s.id == SlideId.Value
                                 select s).Single();

                    txtName.Text = slide.name;
                    txtHeading.Text = slide.heading;
                    selectImage.Path = slide.imageUrl;
                    txtLinkUrl.Text = slide.linkUrl;
                    txtContent.Text = slide.content;
                    checkAvanced.Checked = false;
                    txtCustomOverride.Text = slide.customOverride;
                }

                EnableControls();
                bSuccess = true;
            }
            catch (Exception ex)
            {
                strError = string.Format("Failed loading slide id '{0}'.", SlideId);
                ScmsEvent.Raise(strError, this, ex);
            }

            return bSuccess;
        }

        public bool SaveSlide(out string strError)
        {
            bool bSuccess = false;
            strError = null;

            try
            {
                scms.data.ScmsDataContext dc = new scms.data.ScmsDataContext();
                scms.data.scms_slideshow_slide slide = null;

                if (SlideId.HasValue)
                {
                    slide = (from s in dc.scms_slideshow_slides
                             where s.id == SlideId.Value
                             select s).Single();
                }
                else
                {
                    slide = new scms.data.scms_slideshow_slide();
                    slide.slideShowId = SlideShowId.Value;
                    dc.scms_slideshow_slides.InsertOnSubmit(slide);

                    int nOrdinal = 1;
                    var maxOrdinal = (from s in dc.scms_slideshow_slides
                                      where s.slideShowId == SlideShowId.Value
                                      orderby s.ordinal descending
                                      select s.ordinal).FirstOrDefault();
                    if (maxOrdinal != null)
                    {
                        if (maxOrdinal > 0)
                        {
                            nOrdinal = maxOrdinal + 1;
                        }
                    }

                    slide.ordinal = nOrdinal;
                }

                slide.name = txtName.Text.Trim();
                slide.heading = txtHeading.Text.Trim();
                slide.imageUrl = selectImage.Path;
                slide.linkUrl = txtLinkUrl.Text.Trim();
                slide.content = txtContent.Text;
                slide.customOverride = txtCustomOverride.Text;

                dc.SubmitChanges();
                SlideId = slide.id;
                bSuccess = true;
            }
            catch (Exception ex)
            {
                strError = "Failed saving slide";
                ScmsEvent.Raise(strError, this, ex);
            }

            return bSuccess;
        }

        protected void PageSelectionChanged(int? nPageId)
        {
            ScmsSiteMapProvider provider = new ScmsSiteMapProvider();
            ScmsSiteMapProvider.PageNode pageNode;
            string strError;
            Exception exError;
            txtLinkUrl.Text = null;
            if (nPageId.HasValue)
            {
                if (provider.GetPageNode(nPageId.Value, out pageNode, out strError, out exError))
                {
                    txtLinkUrl.Text = pageNode.page.url;
                }
            }


        }

        protected void Page_Load(object sender, EventArgs e)
        {
            selectImage.SiteId = SiteId;

            pageSelector.SiteId = SiteId;
            pageSelector.OnPageSelectionChanged += PageSelectionChanged;

            if (!IsPostBack)
            {
                EnableControls();
            }
        }

        protected void checkAdvanced_Click(object sender, EventArgs args)
        {
            EnableControls();
        }

        protected void EnableControls()
        {
            bool bShowAdvanced = false;
            bool bShowHeading = false;
            bool bShowContent = false;

            try
            {
                scms.data.ScmsDataContext dc = new scms.data.ScmsDataContext();
                var slideshow = (from s in dc.scms_slideshows
                                 where s.id == SlideShowId.Value
                                 select s).FirstOrDefault();
                if (slideshow != null)
                {
                    switch (slideshow.type)
                    {
                        case "basic":
                            bShowAdvanced = checkAvanced.Checked;
                            break;

                        case "template":
                            bShowHeading = true;
                            bShowContent = true;
                            bShowAdvanced = checkAvanced.Checked;
                            break;

                        case "custom":
                            bShowAdvanced = checkAvanced.Checked;
                            bShowHeading = true;
                            bShowContent = true;
                            break;

                    }
                }

            }
            catch (Exception ex)
            {
                string strMessage = string.Format("Failed looking up slide '{0}", SlideId);
                ScmsEvent.Raise(strMessage, this, ex);
            }


            placeholderHeading.Visible = bShowHeading;
            placeholderContent.Visible = bShowContent;
            placeholderAdvanced.Visible = bShowAdvanced;

        }
    }
}