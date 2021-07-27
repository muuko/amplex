using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace scms.modules.slideshow
{
    public partial class slides : scms.RootControl
    {
        public int? SlideShowId
        {
            get;
            set;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            slide.SiteId = SiteId;
            slide.SlideShowId = SlideShowId;
            slide.ModuleInstanceId = ModuleInstanceId;
            if (!IsPostBack)
            {
                LoadSlides();
            }
        }

        public void ShowSlides()
        {
            mv.SetActiveView(viewSlides);
        }

        public void LoadSlides()
        {
            try
            {
                scms.data.ScmsDataContext dc = new scms.data.ScmsDataContext();
                var slides = from s in dc.scms_slideshow_slides
                             where s.slideShowId == SlideShowId.Value
                             orderby s.ordinal
                             select s;
                rptSlides.DataSource = slides;
                rptSlides.DataBind();
            }
            catch (Exception ex)
            {
                string strMessage = "Failed loading slides";
                statusMessage.ShowFailure(strMessage);
                ScmsEvent.Raise(strMessage, this, ex);
            }
        }

        protected void btnReturnToSlides_Click(object sender, EventArgs args)
        {
            mv.SetActiveView(viewSlides);
        }

        protected void btnNew_Click(object sender, EventArgs args)
        {
            EditSlide(null);
        }

        protected void btnSave_Click(object sender, EventArgs args)
        {
            string strError = null;
            if (Page.IsValid)
            {
                if (slide.SaveSlide(out strError))
                {
                    LoadSlides();
                    mv.SetActiveView(viewSlides);
                }
                else
                {
                    statusMessage.ShowFailure(string.Format("Failed saving slide: '{0}'.", strError));
                }
            }
        }

        protected void rptSlides_ItemDataBound(object sender, RepeaterItemEventArgs args)
        {
            scms.data.scms_slideshow_slide slide = (scms.data.scms_slideshow_slide)args.Item.DataItem;

            ImageButton ibThumbnail = args.Item.FindControl("ibThumbnail") as ImageButton;
            if (ibThumbnail != null)
            {
                if (string.IsNullOrEmpty(slide.imageUrl))
                {
                    ibThumbnail.Visible = false;
                }
                else
                {
                    string strThumbnailUrl = string.Format("/Image.ashx?src={0}&m=stretch&w=80&h=80", HttpUtility.UrlEncode(slide.imageUrl));
                    ibThumbnail.ImageUrl = strThumbnailUrl;
                    ibThumbnail.Width = 80;
                    ibThumbnail.Attributes["title"] = slide.name;
                }
            }
        }

        protected void ibThumbnail_Command(object sender, CommandEventArgs args)
        {
            int nSlideId = int.Parse(args.CommandArgument.ToString());
            EditSlide(nSlideId);
        }

        protected void EditSlide(int? nSlideId)
        {
            slide.SlideId = nSlideId;
            string strError;
            if (slide.LoadSlide(out strError))
            {
                mv.SetActiveView(viewDetail);
            }
            else
            {
                statusMessage.ShowFailure(strError);
            }
        }

        protected void Edit_Command(object sender, CommandEventArgs args)
        {
            int nSlideId = int.Parse(args.CommandArgument.ToString());
            EditSlide(nSlideId);
        }

        protected void Move_Command(object sender, CommandEventArgs args)
        {
            try
            {
                bool bUp = string.Compare(args.CommandName, "up", true) == 0;
                int nSlideId = int.Parse((string)args.CommandArgument);

                global::scms.data.ScmsDataContext dc = new global::scms.data.ScmsDataContext();


                var slides = from s in dc.scms_slideshow_slides
                             where s.slideShowId == SlideShowId.Value
                             orderby s.ordinal
                             select s;

                global::scms.data.scms_slideshow_slide priorSlide = null;
                global::scms.data.scms_slideshow_slide nextSlide = null;


                var thisSlide = slides.Where(s => (s.id == nSlideId)).Single();
                int nThisOrdinal = thisSlide.ordinal;

                foreach (var Slide in slides)
                {
                    if (Slide.id != thisSlide.id)
                    {
                        if (Slide.ordinal < nThisOrdinal)
                        {
                            priorSlide = Slide;
                        }
                        else
                        {
                            if (nextSlide == null)
                            {
                                if (Slide.ordinal > nThisOrdinal)
                                {
                                    nextSlide = Slide;
                                    break;
                                }
                            }
                        }
                    }
                }

                if (bUp)
                {
                    if (priorSlide != null)
                    {
                        thisSlide.ordinal = priorSlide.ordinal;
                        priorSlide.ordinal = nThisOrdinal;
                    }
                }
                else
                {
                    if (nextSlide != null)
                    {
                        thisSlide.ordinal = nextSlide.ordinal;
                        nextSlide.ordinal = nThisOrdinal;
                    }
                }

                dc.SubmitChanges();
                global::scms.CacheManager.Clear();
                LoadSlides();
            }
            catch (Exception ex)
            {
                string strMessage = "Failed moving slide.";
                ScmsEvent.Raise(strMessage, this, ex);
                statusMessage.ShowFailure(strMessage);

            }
        }

        protected void Delete_Command(object sender, CommandEventArgs args)
        {
            try
            {
                int nSlideId = int.Parse(args.CommandArgument.ToString());
                scms.data.ScmsDataContext dc = new scms.data.ScmsDataContext();
                scms.data.scms_slideshow_slide slide = (from s in dc.scms_slideshow_slides
                                                        where s.id == nSlideId
                                                        select s).FirstOrDefault();
                if (slide != null)
                {
                    dc.scms_slideshow_slides.DeleteOnSubmit(slide);
                    dc.SubmitChanges();
                }
                statusMessage.ShowSuccess("Slide deleted");
                LoadSlides();
                mv.SetActiveView(viewSlides);
            }
            catch (Exception ex)
            {
                string strMessage = "Failed deleting slide.";
                ScmsEvent.Raise(strMessage, this, ex);
                statusMessage.ShowFailure(strMessage);
            }
        }
    }
}