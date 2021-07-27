using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Xml.Linq;

namespace scms.admin
{
    public partial class masters : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Page.Title = "Admin - Masters";
            siteDdl.OnSiteSelected += OnSiteSelected;

            scms.admin.Admin master = (scms.admin.Admin)this.Master;
            master.NavType = Admin.ENavType.Masters;

            anchorSource.HRef = string.Format("javascript:ShowSelectWindowWithSource({0},'{1}');", siteDdl.SiteId, txtPath.ClientID);
            DataBind();
        }

        public override void DataBind()
        {
            base.DataBind();

            try
            {
                int nSiteId = siteDdl.SiteId.Value;

                global::scms.data.ScmsDataContext dc = new global::scms.data.ScmsDataContext();
                var masters = from m in dc.scms_masters
                              where m.siteId == nSiteId
                              where m.deleted == false
                              orderby m.id
                              select m;

                lvMasters.DataSource = masters;
                lvMasters.DataBind();

            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("Exception thrown in DataBind: '{0}'.", ex.ToString(), ex));
            }
        }

        protected void OnSiteSelected(int? nSiteId)
        {
            DataBind();
        }

        protected void Delete(object sender, System.Web.UI.WebControls.CommandEventArgs args)
        {
            global::scms.data.ScmsDataContext dc = new global::scms.data.ScmsDataContext();

            string strMasterId = (string)args.CommandArgument;
            int nMasterId = int.Parse(strMasterId);

            // determine if in use
            var templates = from t in dc.scms_templates
                            where t.masterId == nMasterId
                            where t.deleted == false
                            orderby t.name
                            select t;

            if (templates.Count() == 0)
            {
                try
                {
                    var master = (from m in dc.scms_masters
                                  where m.id == nMasterId
                                  where m.deleted == false
                                  select m).Single();

                    master.deleted = true;
                    dc.SubmitChanges();
                    global::scms.CacheManager.Clear();
                    DataBind();
                    Response.Redirect(Request.RawUrl, false);
                }
                catch (Exception ex)
                {
                    string strMessage = "Failed deleting master";
                    statusMessage.ShowFailure(strMessage);
                    ScmsEvent scmsEvent = new ScmsEvent(strMessage, this, ex);
                    scmsEvent.Raise();
                }
            }
            else
            {
                System.Text.StringBuilder sbMessage = new System.Text.StringBuilder();
                sbMessage.Append("Unable to delete master, because it is being used by the following templates:<br />");

                foreach (var template in templates)
                {
                    sbMessage.AppendFormat("{0}<br />", template.name);
                }

                statusMessage.ShowFailure(sbMessage.ToString());
            }
        }

        protected void lvMasters_ItemDataBound(object sender, System.Web.UI.WebControls.ListViewItemEventArgs args)
        {
            /*
            if (args.Item.ItemType == ListViewItemType.DataItem)
            {
                ListViewDataItem dataItem = (ListViewDataItem)args.Item;
                TemplateItem templateItem = (TemplateItem)dataItem.DataItem;
                string strTemplateName = templateItem.name;
                ImageButton imageButton = (ImageButton)args.Item.FindControl("btnDelete");
                if (imageButton != null)
                {
                    imageButton.OnClientClick = string.Format("javascript: return confirm(\"Delete '{0}' template?\");", strTemplateName);
                }

            }
            */
        }

        protected void btnNew_Click(object sender, EventArgs args)
        {
            Page.Validate("new");
            if (Page.IsValid)
            {
                int nSiteId = siteDdl.SiteId.Value;

                string strBaseName = txtnewMasterName.Text.Trim();

                global::scms.data.ScmsDataContext dc = new global::scms.data.ScmsDataContext();

                int nTry = 0;
                bool bUniqueNameFound = false;
                string strName = null;
                do
                {
                    strName = strBaseName;
                    if (nTry > 0)
                    {
                        strName = string.Format("{0}-{1}", strBaseName, nTry);
                    }

                    var existing = (from m in dc.scms_masters
                                    where m.name == strName
                                    where m.deleted == false
                                    where m.siteId == nSiteId
                                    select m).FirstOrDefault();

                    if (existing == null)
                    {
                        bUniqueNameFound = true;
                    }
                    else
                    {
                        nTry++;
                    }
                }
                while (!bUniqueNameFound);

                global::scms.data.scms_master master = new scms.data.scms_master();
                master.siteId = nSiteId;
                master.name = strName;
                master.path = txtPath.Text;

                dc.scms_masters.InsertOnSubmit(master);
                dc.SubmitChanges();
                global::scms.CacheManager.Clear();

                //				int nMasterId = master.id;
                //				string strMasterUrl = string.Format("/scms/admin/template.aspx?sid={0}&mid={1}", nSiteId, nMasterId);
                Response.Redirect(Request.RawUrl, false);

            }
        }
    }
}
