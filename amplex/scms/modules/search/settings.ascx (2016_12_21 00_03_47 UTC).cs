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

namespace scms.modules.search
{
    public partial class settings : scms.RootControl
    {
        protected const string strSearchResultsPageKey = "search-results-page";
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                pageSelector.SiteId = this.SiteId;

                try
                {
                    scms.data.ScmsDataContext dc = new scms.data.ScmsDataContext();
                    var settings = (from s in dc.scms_search_settings
                                    where s.siteId == this.SiteId
                                    select s).FirstOrDefault();
                    if (settings != null)
                    {
                        pageSelector.PageId = settings.searchResultsPageId;
                    }
                }
                catch (Exception ex)
                {
                    ScmsEvent.Raise("Failed loading settings", this, ex);
                    statusMessage.ShowFailure("Failed loading settings");
                }
            }
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {

            try
            {
                scms.data.ScmsDataContext dc = new scms.data.ScmsDataContext();
                scms.data.scms_search_setting settings = (from s in dc.scms_search_settings
                                                          where s.siteId == this.SiteId
                                                          select s).FirstOrDefault();
                if (settings == null)
                {
                    settings = new scms.data.scms_search_setting();
                    settings.siteId = this.SiteId.Value;
                    dc.scms_search_settings.InsertOnSubmit(settings);

                }
                settings.searchResultsPageId = pageSelector.PageId;
                dc.SubmitChanges();

                statusMessage.ShowSuccess("Settings updated");
            }
            catch (Exception ex)
            {
                ScmsEvent.Raise("Failed saving settings", this, ex);
                statusMessage.ShowFailure("Failed saving settings");
            }
        }

        protected void btnRebuildIndex_Click(object sender, EventArgs args)
        {
            ScmsEvent.Raise("Rebuilding search index - Start", this, null);

            System.Threading.ThreadStart threadStart = new System.Threading.ThreadStart(RebuildSearchIndex);
            System.Threading.Thread thread = new System.Threading.Thread(threadStart);
            thread.Start();

            statusMessage.ShowSuccess("Search rebuild launched, view the error status log for completion status.");

            // RebuildSearchIndex();
        }

        protected void RebuildSearchIndex()
	      {
          try
          {
						scms.search.search search = new scms.search.search();
						search.RebuildSearchIndex();

            scms.CacheManager.Clear();
          }
          catch (Exception ex)
          {
              ScmsEvent.Raise("An error occurred during search indexing", this, ex);
          }

          ScmsEvent.Raise("Rebuilding search index - Complete", this, null);
				}
    }
}