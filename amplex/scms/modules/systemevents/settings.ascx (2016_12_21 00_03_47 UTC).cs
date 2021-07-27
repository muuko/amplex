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
using global::scms.data;

namespace scms.modules.systemevents
{
    public partial class settings : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadEvents();
            }
        }

        protected void LoadEvents()
        {
            try
            {
                ScmsDataContext dc = new ScmsDataContext();
                var events = from e in dc.aspnet_WebEvent_Events
                             orderby e.EventTimeUtc descending
                             select e;
                gvEvents.DataSource = events;
                gvEvents.DataBind();
            }
            catch (Exception ex)
            {
                string strMessage = "Failed loading events";
                global::scms.ScmsEvent scmsEvent = new global::scms.ScmsEvent(strMessage, this, ex);
                scmsEvent.Raise();
            }
        }

        protected string Trim(object objText, int nMaxLength)
        {
            string strResult = string.Empty;

            if (objText != null)
            {
                string strText = (string)objText;


                if (strText.Length > nMaxLength)
                {
                    strResult = strText.Substring(0, nMaxLength) + "...";
                }
                else
                {
                    strResult = strText;
                }
            }

            return strResult;
        }

        protected string TrimUrl(object objText, int nMaxLength)
        {
            string strResult = string.Empty;

            if (objText != null)
            {
                string strText = (string)objText;

                if (!string.IsNullOrEmpty(strText))
                {
                    try
                    {
                        UriBuilder builder = new UriBuilder(strText);

                        if (string.Compare(Request.Url.Host, builder.Host, true) == 0)
                        {
                            strText = string.Format("{0}{1}", builder.Path, builder.Query);
                        }
                    }
                    catch
                    {
                    }
                }

                if (!string.IsNullOrEmpty(strText))
                {
                    if (strText.Length > nMaxLength)
                    {
                        strResult = strText.Substring(0, nMaxLength) + "...";
                    }
                    else
                    {
                        strResult = strText;
                    }
                }
            }

            return strResult;
        }

        protected void gvEvents_PageIndexChanging(object sender, GridViewPageEventArgs args)
        {
            int nIndex = args.NewPageIndex;
            gvEvents.PageIndex = nIndex;
            LoadEvents();
        }

        protected void gvEvents_RowDeleting(object sender, GridViewDeleteEventArgs args)
        {
            try
            {
                int nIndex = args.RowIndex;
                string strEventId = (string)gvEvents.DataKeys[nIndex].Value;

                ScmsDataContext dc = new ScmsDataContext();

                aspnet_WebEvent_Event ev = (from e in dc.aspnet_WebEvent_Events
                                            where e.EventId == strEventId
                                            select e).FirstOrDefault();
                dc.aspnet_WebEvent_Events.DeleteOnSubmit(ev);
                dc.SubmitChanges();

                LoadEvents();
            }
            catch (Exception ex)
            {
                string strMessage = "Failed deleting event";
                global::scms.ScmsEvent scmsEvent = new global::scms.ScmsEvent(strMessage, this, ex);
                scmsEvent.Raise();
            }
        }


        protected void btnView_Click(object sender, CommandEventArgs args)
        {
            string strEventId = (string)args.CommandArgument;

            try
            {
                ScmsDataContext dc = new ScmsDataContext();
                var ev = from e in dc.aspnet_WebEvent_Events
                         where e.EventId == strEventId
                         select e;
                dvEvent.DataSource = ev;
                dvEvent.DataBind();

                multiView.SetActiveView(viewDetailsView);
            }
            catch (Exception ex)
            {
                string strMessage = "Failed loading event";
                global::scms.ScmsEvent.Raise(strMessage, this, ex);
            }
        }

        protected void btnDelete_Command(object sender, CommandEventArgs args)
        {
            try
            {
                ScmsDataContext dc = new ScmsDataContext();

                string strEventId = (string)dvEvent.DataKey.Value;
                aspnet_WebEvent_Event ev = (from e in dc.aspnet_WebEvent_Events
                                            where e.EventId == strEventId
                                            select e).FirstOrDefault();
                dc.aspnet_WebEvent_Events.DeleteOnSubmit(ev);
                dc.SubmitChanges();

                LoadEvents();
                multiView.SetActiveView(viewGridView);
            }
            catch (Exception ex)
            {
                string strMessage = "Failed creating form field";
                global::scms.ScmsEvent.Raise(strMessage, this, ex);
            }
        }

        protected void btnReturnToGrid_Clicked(object sender, EventArgs args)
        {
            multiView.SetActiveView(viewGridView);
        }

        protected void btnDeleteAll_Click(object sender, EventArgs args)
        {
            try
            {
                ScmsDataContext dc = new ScmsDataContext();
                var events = from e in dc.aspnet_WebEvent_Events
                             select e;
                dc.aspnet_WebEvent_Events.DeleteAllOnSubmit(events);

                dc.SubmitChanges();

                LoadEvents();
                multiView.SetActiveView(viewGridView);
            }
            catch (Exception ex)
            {
                string strMessage = "Failed deleting all events";
                global::scms.ScmsEvent.Raise(strMessage, this, ex);
            }
        }

        protected string GetFormattedDetails(object objDetails)
        {
            string strFormattedDetails = string.Empty;

            if (objDetails != null)
            {
                string strDetails = objDetails.ToString();
                if (!string.IsNullOrEmpty(strDetails))
                {
                    strFormattedDetails = strDetails.Replace("\n", "<br />");
                }
            }

            return strFormattedDetails;
        }

    }
}