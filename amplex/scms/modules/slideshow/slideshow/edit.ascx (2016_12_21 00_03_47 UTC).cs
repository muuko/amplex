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

namespace scms.modules.slideshow
{
    public partial class edit : global::scms.RootControl
    {
        public int? SlideShowId
        {
            get { return (int?)ViewState["slideShowId"]; }
            set { ViewState["slideShowId"] = value; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                EnsureSlideShow();
            }

            slideshowSettings.SiteId = SiteId;
            slideshowSettings.SlideShowId = SlideShowId;
            slideshowSettings.ModuleInstanceId = ModuleInstanceId;

            slides.SiteId = SiteId;
            slides.SlideShowId = SlideShowId;
            slides.ModuleInstanceId = ModuleInstanceId;

            if (!IsPostBack)
            {
                menuTabs_Click(null, null);
            }


        }

        protected void EnsureSlideShow()
        {
            try
            {
                scms.data.ScmsDataContext dc = new scms.data.ScmsDataContext();
                scms.data.scms_slideshow slideShow = (from s in dc.scms_slideshows
                                                      where s.instanceId == ModuleInstanceId.Value
                                                      select s).FirstOrDefault();
                if (slideShow == null)
                {
                    slideShow = new scms.data.scms_slideshow();
                    slideShow.type = "basic";
                    slideShow.width = 400;
                    slideShow.height = 200;
                    slideShow.instanceId = ModuleInstanceId.Value;
                    slideShow.transitionTimeMs = 500;
                    slideShow.pauseTimeMs = 500;
                    slideShow.transitionType = "fade";
                    dc.scms_slideshows.InsertOnSubmit(slideShow);
                    dc.SubmitChanges();
                }

                SlideShowId = slideShow.id;
            }
            catch (Exception ex)
            {
                string strMessage = string.Format("Failed creating / loading slide show.");
                ScmsEvent.Raise(strMessage, this, ex);
                statusMessage.ShowFailure(strMessage);
            }
        }

        protected void menuTabs_Click(object sender, EventArgs args)
        {
            switch (menuTabs.SelectedValue.ToLower())
            {
                case "general":
                    multiView.SetActiveView(viewGeneral);
                    break;

                case "slides":
                    slides.LoadSlides();
                    slides.ShowSlides();
                    multiView.SetActiveView(viewSlides);
                    break;
            }
        }
    }
}