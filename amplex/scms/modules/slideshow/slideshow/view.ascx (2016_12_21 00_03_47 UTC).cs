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
    public partial class view : scms.modules.content.ViewContentControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                scms.data.scms_slideshow slideshow = null;
                scms.data.scms_slideshow_slide[] aSlides = null;
                if (Load(out slideshow, out aSlides))
                {
                    RenderSlideShow(slideshow, aSlides);
                }
            }
        }

        protected bool Load(out scms.data.scms_slideshow slideshow, out scms.data.scms_slideshow_slide[] aSlides)
        {
            bool bSuccess = false;

            slideshow = null;
            aSlides = null;

            try
            {
                if (!ModuleInstanceId.HasValue)
                {
                    throw new Exception("Unexpected module has no instance id");
                }

                scms.data.ScmsDataContext dc = new scms.data.ScmsDataContext();
                slideshow = (from s in dc.scms_slideshows
                             where s.instanceId == ModuleInstanceId.Value
                             select s).FirstOrDefault();
                if (slideshow != null)
                {
                    if (slideshow.random)
                    {
                        int nTotalSlides = slideshow.scms_slideshow_slides.Count;
                        aSlides = new scms.data.scms_slideshow_slide[nTotalSlides];

                        System.Collections.Generic.List<scms.data.scms_slideshow_slide> lSlides = slideshow.scms_slideshow_slides.ToList();
                        for (int nIndex = 0; nIndex < nTotalSlides; nIndex++)
                        {
                            Random r = new Random();
                            int nSelectedIndex = r.Next(lSlides.Count);
                            aSlides[nIndex] = lSlides[nSelectedIndex];
                            lSlides.RemoveAt(nSelectedIndex);
                        }
                    }
                    else
                    {
                        aSlides = slideshow.scms_slideshow_slides.OrderBy(s => s.ordinal).ToArray();
                    }

                    if (aSlides.Length > 0)
                    {
                        bSuccess = true;
                    }
                    else
                    {
                        throw new Exception("Slideshow has no slides");
                    }
                }
                else
                {
                    throw new Exception("No slideshow settings found");
                }
            }
            catch (Exception ex)
            {
                string strMessage = "Failed loading slides";
                ScmsEvent.Raise(strMessage, this, ex);
            }

            return bSuccess;
        }

        protected void RenderSlideShow(scms.data.scms_slideshow slideshow, scms.data.scms_slideshow_slide[] aSlides)
        {
            try
            {
                switch (slideshow.type.ToLower())
                {
                    case "basic":
                        RenderBasic(slideshow, aSlides);
                        break;

                    case "template":
                        RenderTemplate(slideshow, aSlides);
                        break;

                    case "custom":
                        RenderCustom(slideshow, aSlides);
                        break;

                    default:
                        throw new Exception(string.Format("Unknown slide type '{0}'.", slideshow.type));
                }
            }
            catch (Exception ex)
            {
                string strMessage = "Failed rendering slideshow";
                ScmsEvent.Raise(strMessage, this, ex);
            }
        }

        protected void RegisterCycle()
        {
            Page.ClientScript.RegisterClientScriptInclude("scms-cycle", "/scms/modules/slideshow/public/js/jquery.cycle.all.min.js");
        }

        protected void RenderCycleInit(string strSlideshowClass, scms.data.scms_slideshow slideshow, scms.data.scms_slideshow_slide[] aSlides)
        {
            System.Collections.Generic.Dictionary<string, string> lAttributes = new System.Collections.Generic.Dictionary<string, string>();

            string strPagerClass = string.Format("scms-slideshow-pager-{0}", slideshow.id);
            if (slideshow.showPager)
            {
                if (!string.IsNullOrEmpty(slideshow.pagerClass))
                {
                    strPagerClass = slideshow.pagerClass;
                }

                lAttributes.Add("pager", string.Format("'.{0}'", strPagerClass));
            }

            if (slideshow.hoverPause)
            {
                lAttributes.Add("pause", "1");
            }

            if (slideshow.clickAdvance)
            {
                lAttributes.Add("next", string.Format("'.{0}'", strSlideshowClass));
            }

            string strTransitionType = slideshow.transitionType;
            if (!string.IsNullOrEmpty(strTransitionType))
            {
                lAttributes.Add("fx", string.Format("'{0}'", strTransitionType));
            }

            if (slideshow.pauseTimeMs.HasValue)
            {
                lAttributes.Add("timeout", slideshow.pauseTimeMs.ToString());
            }

            if (slideshow.transitionTimeMs.HasValue)
            {
                lAttributes.Add("speed", slideshow.transitionTimeMs.ToString());
            }


            // generate the startup script
            System.Text.StringBuilder sbReady = new System.Text.StringBuilder();
            sbReady.AppendLine("$(document).ready(function() {");
            sbReady.AppendFormat("$('.{0}')", strSlideshowClass);

            if (slideshow.showPager)
            {
                string strPageControl = null;
                bool bAddAnchorBuilder = false;

                switch (slideshow.pagerType)
                {
                    case "text":
                        strPageControl = string.Format("<div class=\"{0} scms-slideshow-pager\" />", strPagerClass);
                        break;

                    case "thumbnail":
                        {
                            System.Text.StringBuilder sbPagerControl = new System.Text.StringBuilder();
                            sbPagerControl.AppendFormat("<ul class=\"{0} scms-slideshow-thumbnail-pager\">", strPagerClass);
                            foreach (var slide in aSlides)
                            {
                                if (!string.IsNullOrEmpty(slide.imageUrl))
                                {
                                    int nThumbnailWidth = 50;
                                    string strWidthNameValue = null;
                                    if (slideshow.pageThumbnailWidth.HasValue)
                                    {
                                        strWidthNameValue = string.Format(" width=\"{0}\" ", slideshow.pageThumbnailWidth.Value);
                                        nThumbnailWidth = slideshow.pageThumbnailWidth.Value;
                                    }

                                    int nThumbnailHeight = 50;
                                    string strHeightNameValue = null;
                                    if (slideshow.pagerThumbnailHeight.HasValue)
                                    {
                                        strHeightNameValue = string.Format(" Height=\"{0}\" ", slideshow.pagerThumbnailHeight.Value);
                                        nThumbnailHeight = slideshow.pagerThumbnailHeight.Value;
                                    }


                                    sbPagerControl.Append("<li><a>");
                                    string strThumbnailUrl = string.Format("/Image.ashx?src={0}&m=stretch&w={1}&h={2}", HttpUtility.UrlEncode(slide.imageUrl), nThumbnailWidth, nThumbnailHeight);
                                    sbPagerControl.AppendFormat("<img src=\"{0}\" {1} {2} />", strThumbnailUrl, strWidthNameValue, strHeightNameValue);
                                    sbPagerControl.Append("</a></li>");
                                }
                            }
                            sbPagerControl.Append("</ul>");

                            strPageControl = sbPagerControl.ToString();
                            bAddAnchorBuilder = true;
                        }
                        break;

                    case "custom":
                        bAddAnchorBuilder = true;
                        break;

                    default:
                        throw new Exception("unknown pager type");
                }


                switch (slideshow.pagerLocation)
                {
                    case "before":
                    case "after":
                        if (!string.IsNullOrEmpty(strPageControl))
                        {
                            sbReady.AppendFormat(".{0}('{1}')\r\n", slideshow.pagerLocation, strPageControl);
                        }
                        break;

                    case "custom":
                        bAddAnchorBuilder = true;
                        break;
                }

                if (bAddAnchorBuilder)
                {
                    string strAnchorBuilder = string.Format(@"function(idx, slide) {{ 
                        // return selector string for existing anchor 
                        return '.{0} > *:eq(' + idx + ') a'; 
                        }} ", strPagerClass);
                    lAttributes.Add("pagerAnchorBuilder", strAnchorBuilder);
                }
            }

            sbReady.AppendLine(".cycle({\r\n");

            bool bFirst = true;
            foreach (var attribute in lAttributes)
            {
                if (bFirst)
                {
                    bFirst = false;
                }
                else
                {
                    sbReady.Append(",");
                }
                sbReady.AppendLine();
                sbReady.AppendFormat("{0}: {1}", attribute.Key, attribute.Value);
            }

            sbReady.AppendLine("});");
            sbReady.AppendLine("});");

            Page.ClientScript.RegisterClientScriptBlock(typeof(string), strSlideshowClass, sbReady.ToString(), true);
        }

        protected void RenderBasicSlides(string strSlideshowClass, Control controlContainer, scms.data.scms_slideshow slideshow, scms.data.scms_slideshow_slide[] aSlides)
        {
            foreach (scms.data.scms_slideshow_slide slide in aSlides)
            {
                HtmlImage img = new HtmlImage();
                img.Src = string.Format(slide.imageUrl);
                if (slideshow.width.HasValue)
                {
                    img.Width = slideshow.width.Value;
                }

                if (slideshow.height.HasValue)
                {
                    img.Height = slideshow.height.Value;
                }

                if (string.IsNullOrEmpty(slide.linkUrl))
                {
                    controlContainer.Controls.Add(img);
                }
                else
                {
                    HtmlAnchor anchor = new HtmlAnchor();
                    anchor.HRef = slide.linkUrl;
                    controlContainer.Controls.Add(anchor);
                    anchor.Controls.Add(img);
                }
            }
        }


        protected void RenderBasic(scms.data.scms_slideshow slideshow, scms.data.scms_slideshow_slide[] aSlides)
        {

            /*
                $(document).ready(function() {
                $('.slideshow').cycle({
                fx: 'fade',
                timeout: 10000,
                speed: 1000
                });
            });
                         
            <div class="slideshow">
                <a href="http://www.google.com/"><img src="/sites/t1/images/jellyfish.jpg" width="200" height="200" /></a>
                <a href="http://www.amazon.com/"><img src="/sites/t1/images/penguins.jpg" width="200" height="200" /></a>
            </div>

            */

            // register the cycle script

            RegisterCycle();

            string strSlideshowClass = string.Format("scms-slideshow-{0}", slideshow.id);
            RenderCycleInit(strSlideshowClass, slideshow, aSlides);



            // generate the control
            System.Web.UI.HtmlControls.HtmlGenericControl div = new HtmlGenericControl("div");
            Controls.Add(div);
            div.Attributes["class"] = string.Format("{0} scms-slideshow", strSlideshowClass);

            RenderBasicSlides(strSlideshowClass, div, slideshow, aSlides);
        }

        protected void RenderTemplateSlides(string strSlideshowClass, Control controlContainer, scms.data.scms_slideshow slideshow, scms.data.scms_slideshow_slide[] aSlides)
        {
            foreach (scms.data.scms_slideshow_slide slide in aSlides)
            {
                string strHtml = null;
                if (!string.IsNullOrEmpty(slide.customOverride))
                {
                    strHtml = slide.customOverride;
                }
                else
                {
                    strHtml = slideshow.itemTemplate;
                }

                if (!string.IsNullOrEmpty(strHtml))
                {
                    strHtml = strHtml.Replace("$SLIDESHOW-CLASS$", strSlideshowClass);
                    strHtml = strHtml.Replace("$WIDTH$", slideshow.width.ToString());
                    strHtml = strHtml.Replace("$HEIGHT$", slideshow.height.ToString());
                    strHtml = strHtml.Replace("$TRANSITION-TYPE$", slideshow.transitionType);
                    strHtml = strHtml.Replace("$IMAGE$", slide.imageUrl);
                    strHtml = strHtml.Replace("$LINK$", slide.linkUrl);
                    strHtml = strHtml.Replace("$HEADING$", slide.heading);
                    strHtml = strHtml.Replace("$CONTENT$", slide.content);
                }

                Literal literal = new Literal();
                controlContainer.Controls.Add(literal);
                literal.Text = strHtml;
            }
        }

        protected void RenderTemplate(scms.data.scms_slideshow slideshow, scms.data.scms_slideshow_slide[] aSlides)
        {
            // register the cycle script
            RegisterCycle();

            string strSlideshowClass = string.Format("scms-slideshow-{0}", slideshow.id);
            RenderCycleInit(strSlideshowClass, slideshow, aSlides);

            System.Web.UI.HtmlControls.HtmlGenericControl div = new HtmlGenericControl("div");
            Controls.Add(div);
            div.Attributes["class"] = string.Format("{0} scms-slideshow", strSlideshowClass);

            RenderTemplateSlides(strSlideshowClass, div, slideshow, aSlides);
        }

        protected void RenderCustom(scms.data.scms_slideshow slideshow, scms.data.scms_slideshow_slide[] aSlides)
        {
        }


        protected void LoadSlides()
        {
            try
            {
                if (ModuleInstanceId.HasValue)
                {
                    scms.data.ScmsDataContext dc = new scms.data.ScmsDataContext();

                    var slideshow = (from s in dc.scms_slideshows
                                     where s.instanceId == ModuleInstanceId.Value
                                     select s).FirstOrDefault();

                    if (slideshow != null)
                    {
                        var slides = from s in dc.scms_slideshow_slides
                                     where s.slideShowId == slideshow.id
                                     orderby s.ordinal
                                     select s;
                    }
                    else
                    {
                        throw new Exception("slideshow is not configured");
                    }
                }
                else
                {
                    throw new Exception("module instance id is null");
                }
            }
            catch (Exception ex)
            {
                string strMessage = "Failed loading slide show.";
                ScmsEvent.Raise(strMessage, this, ex);
            }
        }

    }
}