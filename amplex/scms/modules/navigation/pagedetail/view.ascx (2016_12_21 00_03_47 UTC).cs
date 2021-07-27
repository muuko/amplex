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

namespace scms.modules.navigation.pagedetail
{
    public partial class view : RootControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (this.ModuleInstanceId.HasValue && this.PageId.HasValue)
                {
                    scms.data.ScmsDataContext dc = new scms.data.ScmsDataContext();
                    var pagedetailInstance = (from pdi in dc.scms_navigation_pagedetails
                                              where pdi.instanceId == this.ModuleInstanceId.Value
                                              select pdi).FirstOrDefault();
                    if (pagedetailInstance != null)
                    {

                        scms.ScmsSiteMapProvider.PageNode pageNode = null;
                        if (GetPageNode(out pageNode))
                        {
                            string strText = null;
                            switch (pagedetailInstance.detailType.ToLower())
                            {
                                case "title":
                                    strText = pageNode.page.title;
                                    break;

                                case "linktext":
                                    strText = pageNode.page.linktext;
                                    break;

                                case "description":
                                    strText = pageNode.page.summary;
                                    break;

                                case "thumbnail":
                                    if (pageNode.page.thumbnail != null)
                                    {
                                        strText = string.Format("<img src=\"{0}\" />", pageNode.page.thumbnail);
                                    }
                                    break;

                                case "dateshort":
                                    if (pageNode.page.associatedDate.HasValue)
                                    {
                                        strText = pageNode.page.associatedDate.Value.ToShortDateString();
                                    }
                                    break;

                                case "datelong":
                                    if (pageNode.page.associatedDate.HasValue)
                                    {
                                        strText = pageNode.page.associatedDate.Value.ToLongDateString();
                                    }
                                    break;
                            }

                            string strOutput = strText;
                            if (pagedetailInstance.wrapDetailInHtmlElement)
                            {
                                string strClassNameValue = null;
                                if (!string.IsNullOrEmpty(pagedetailInstance.cssClassWrap))
                                {
                                    strClassNameValue = string.Format(" class=\"{0}\" ", pagedetailInstance.cssClassWrap);
                                }

                                strOutput = string.Format("<{0}{1}>{2}</{0}>", pagedetailInstance.wrapElementType, strClassNameValue, strText);
                            }
                            literalPageDetail.Text = strOutput;
                        }
                        else
                        {
                            // TODO log error
                        }
                    }
                }
            }
        }
    }
}