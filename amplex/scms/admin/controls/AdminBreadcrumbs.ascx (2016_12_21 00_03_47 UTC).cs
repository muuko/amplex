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

namespace scms.admin.controls
{
    public partial class AdminBreadcrumbs : System.Web.UI.UserControl
    {
        protected int? nSiteId = null;
        protected int? nPageId = null;
        protected int? nTemplateId = null;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                string strSiteId = Request.QueryString["sid"];
                if (!string.IsNullOrEmpty(strSiteId))
                {
                    int n;
                    if (int.TryParse(strSiteId, out n))
                    {
                        nSiteId = n;
                    }
                }

                string strPageId = Request.QueryString["pid"];
                if (!string.IsNullOrEmpty(strPageId))
                {
                    int n;
                    if (int.TryParse(strPageId, out n))
                    {
                        nPageId = n;
                    }
                }

                string strTemplateId = Request.QueryString["tid"];
                if (!string.IsNullOrEmpty(strTemplateId))
                {
                    int n;
                    if (int.TryParse(strTemplateId, out n))
                    {
                        nTemplateId = n;
                    }
                }

                if (nSiteId.HasValue)
                {
                    if (nPageId.HasValue)
                    {
                        pagesBreadcrumbs.SiteId = nSiteId;
                        pagesBreadcrumbs.PageId = nPageId;
                        multiViewHeader.SetActiveView(viewPageModule);
                    }
                    else
                    {
                        if (nTemplateId.HasValue)
                        {
                            global::scms.Template template;
                            string strError;
                            Exception exError;
                            if (global::scms.Templates.GetTemplate(nSiteId.Value, nTemplateId.Value, out template, out strError, out exError))
                            {
                                literalTemplateName.Text = template.ScmsTemplate.name;
                                multiViewHeader.SetActiveView(viewTemplateModule);
                            }
                        }
                    }
                }
            }
        }
    }
}