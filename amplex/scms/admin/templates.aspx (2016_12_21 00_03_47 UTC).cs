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
    public partial class templates : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Page.Title = "Admin - Templates";
            siteDdl.OnSiteSelected += OnSiteSelected;

            scms.admin.Admin master = (scms.admin.Admin)this.Master;
            master.NavType = Admin.ENavType.Templates;

            if (!IsPostBack)
            {
                DataBind();
            }
        }

        struct TemplateItem
        {
            public TemplateItem(int siteId, int templateId, string name, string masterName)
            {
                this.siteId = siteId;
                this.templateId = templateId;
                this.name = name;
                this.masterName = masterName;
            }

            public int siteId;
            public int SiteId
            {
                get { return siteId; }
            }

            public int templateId;
            public int TemplateId
            {
                get { return templateId; }
            }

            public string name;
            public string Name
            {
                get { return name; }
            }

            public string masterName;
            public string MasterName
            {
                get { return masterName; }
            }
        }

        public override void DataBind()
        {
            base.DataBind();

            try
            {
                int nSiteId = siteDdl.SiteId.Value;

                global::scms.data.ScmsDataContext dc = new global::scms.data.ScmsDataContext();
                var templates = from t in dc.scms_templates
                                where t.siteId == nSiteId
                                where t.deleted == false
                                orderby t.id
                                select new TemplateItem
                                {
                                    siteId = t.siteId,
                                    templateId = t.id,
                                    name = t.name,
                                    masterName = t.scms_master.name
                                };
                lvTemplates.DataSource = templates;
                lvTemplates.DataBind();

                var masterPages = from m in dc.scms_masters
                                  where m.siteId == nSiteId
                                  where m.deleted == false
                                  orderby m.name
                                  select m;
                ddlMasterPage.DataSource = masterPages;
                ddlMasterPage.DataTextField = "name";
                ddlMasterPage.DataValueField = "id";
                ddlMasterPage.DataBind();
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

            string strTemplateId = (string)args.CommandArgument;
            int nTemplateId = int.Parse(strTemplateId);

            // determine if in use
            var pages = from p in dc.scms_pages
                        where p.templateId == nTemplateId
                        orderby p.url
                        select p;

            if (pages.Count() == 0)
            {
                try
                {
                    var template = (from t in dc.scms_templates
                                    where t.id == nTemplateId
                                    where t.deleted == false
                                    select t).Single();

                    template.deleted = true;
                    dc.SubmitChanges();
                    global::scms.CacheManager.Clear();
                    DataBind();

                }
                catch (Exception ex)
                {
                    string strMessage = "Failed deleting template";
                    ScmsEvent scmsEvent = new ScmsEvent(strMessage, this, ex);
                    scmsEvent.Raise();
                    statusMessage.ShowFailure(strMessage);
                }
            }
            else
            {
                System.Text.StringBuilder sbMessage = new System.Text.StringBuilder();
                sbMessage.Append("Unable to delete template, because it is being used by the following pages:<br />");

                foreach (var page in pages)
                {
                    sbMessage.AppendFormat("{0}<br />", page.url);
                }

                statusMessage.ShowFailure(sbMessage.ToString());
            }
        }

        protected void lvTemplates_ItemDataBound(object sender, System.Web.UI.WebControls.ListViewItemEventArgs args)
        {
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
        }

        protected void btnNew_Click(object sender, EventArgs args)
        {
            Page.Validate("new");
            if (Page.IsValid)
            {
                int nSiteId = siteDdl.SiteId.Value;

                string strBaseName = txtnewTemplateName.Text.Trim();

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

                    var existingTemplate = (from t in dc.scms_templates
                                            where t.name == strName
                                            where t.deleted == false
                                            where t.siteId == nSiteId
                                            select t).FirstOrDefault();

                    if (existingTemplate == null)
                    {
                        bUniqueNameFound = true;
                    }
                    else
                    {
                        nTry++;
                    }
                }
                while (!bUniqueNameFound);

                global::scms.data.scms_template template = new global::scms.data.scms_template();
                template.siteId = nSiteId;
                template.name = strName;
                template.masterId = int.Parse(ddlMasterPage.SelectedValue);

                dc.scms_templates.InsertOnSubmit(template);
                dc.SubmitChanges();
                global::scms.CacheManager.Clear();

                int nTemplateId = template.id;
                string strTemplateUrl = string.Format("/scms/admin/template.aspx?sid={0}&tid={1}", nSiteId, template.id);
                Response.Redirect(strTemplateUrl, false);
            }
        }
    }
}
