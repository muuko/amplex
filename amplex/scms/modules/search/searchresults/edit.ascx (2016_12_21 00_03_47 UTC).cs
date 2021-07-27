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

namespace scms.modules.search.searchresults
{
    public partial class edit : global::scms.RootControl
    {
        public void Page_Load(object sender, EventArgs args)
        {
            if (!IsPostBack)
            {
                LoadModule();
                chkShowThumbnail_CheckedChanged(null, null);
            }
        }

        protected void LoadModule()
        {
            try
            {
                scms.data.ScmsDataContext dc = new scms.data.ScmsDataContext();
                var module = (from m in dc.scms_search_results_modules
                              where m.instanceId == ModuleInstanceId.Value
                              select m).FirstOrDefault();
                if (module != null)
                {
                    txtMaxResultCount.Text = module.maxResultCount.ToString();
                    txtPageSize.Text = module.pageSize.ToString();
                    txtMaxKeywords.Text = module.maxKeywords.ToString();
                    chkShowThumbnail.Checked = module.showThumbnail;
                    txtThumbnailHeight.Text = module.thumbnailHeight.ToString();
                    txtThumbnailWidth.Text = module.thumbnailWidth.ToString();
                    chkShowUrl.Checked = module.showUrl;
                    chkReadMore.Checked = module.showReadMore;
                    chkPrevNext.Checked = module.showPrevNext;
                }
            }
            catch (Exception ex)
            {
                string strMessage = string.Format("Failed loading search results module with instance id '{0}'.", ModuleInstanceId);
                ScmsEvent.Raise(strMessage, this, ex);
                statusMessage.ShowFailure("Failed loading module settings.");
            }
        }

        protected void chkShowThumbnail_CheckedChanged(object sender, EventArgs args)
        {
            if (chkShowThumbnail.Checked)
            {
                lblThumbnailHeight.Enabled = true;
                txtThumbnailHeight.Enabled = true;
                lblThumbnailWidth.Enabled = true;
                lblThumbnailWidth.Enabled = true;
            }
            else
            {
                lblThumbnailHeight.Enabled = false;
                txtThumbnailHeight.Enabled = false;
                lblThumbnailWidth.Enabled = false;
                lblThumbnailWidth.Enabled = false;
            }
        }

        protected void btnSave_Click(object sender, EventArgs args)
        {
            try
            {
                scms.data.ScmsDataContext dc = new scms.data.ScmsDataContext();
                scms.data.scms_search_results_module module = null;
                module = (from m in dc.scms_search_results_modules
                          where m.instanceId == ModuleInstanceId.Value
                          select m).FirstOrDefault();
                if (module == null)
                {
                    module = new scms.data.scms_search_results_module();
                    module.instanceId = ModuleInstanceId.Value;
                    dc.scms_search_results_modules.InsertOnSubmit(module);
                }

                int? nMaxResult = null;
                int n;
                if (int.TryParse(txtMaxResultCount.Text, out n))
                {
                    nMaxResult = n;
                }
                module.maxResultCount = nMaxResult;

                int? nPageSize = null;
                if (int.TryParse(txtPageSize.Text, out n))
                {
                    nPageSize = n;
                }
                module.pageSize = nPageSize;

                int? nMaxKeywords = null;
                if (int.TryParse(txtMaxKeywords.Text, out n))
                {
                    nMaxKeywords = n;
                }
                module.maxKeywords = nMaxKeywords;

                module.showThumbnail = chkShowThumbnail.Checked;

                int? nThumbnailHeight = null;
                if (int.TryParse(txtThumbnailHeight.Text, out n))
                {
                    nThumbnailHeight = n;
                }
                module.thumbnailHeight = nThumbnailHeight;

                int? nThumbnailWidth = null;
                if (int.TryParse(txtThumbnailWidth.Text, out n))
                {
                    nThumbnailWidth = n;
                }
                module.thumbnailWidth = nThumbnailWidth;

                module.showUrl = chkShowUrl.Checked;
                module.showReadMore = chkReadMore.Checked;
                module.showPrevNext = chkPrevNext.Checked;
                dc.SubmitChanges();

                statusMessage.ShowSuccess("Settings updated");
            }
            catch (Exception ex)
            {
                string strMessage = string.Format("Failed loading search results module with instance id '{0}'.", ModuleInstanceId);
                ScmsEvent.Raise(strMessage, this, ex);
                statusMessage.ShowFailure("Failed loading module settings.");
            }
        }


    }


}

