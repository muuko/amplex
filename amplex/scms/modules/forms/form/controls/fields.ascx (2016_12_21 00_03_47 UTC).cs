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

namespace scms.modules.forms.form.controls
{
    public partial class fields : System.Web.UI.UserControl
    {
        public delegate void FieldSelected(int nFieldId);
        public FieldSelected OnFieldSelected = null;
        public delegate void FieldDeleted(int nFieldId);
        public FieldDeleted OnFieldDeleted = null;

        public int? FormId
        {
            get { return (int?)ViewState["FormId"]; }
            set
            {
                ViewState["FormId"] = value;
                LoadFields();
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
            }
        }


        public void LoadFields()
        {

            try
            {
                lvFields.DataSource = null;

                int? nFormId = FormId;
                if (nFormId.HasValue)
                {
                    global::scms.data.ScmsDataContext dc = new global::scms.data.ScmsDataContext();
                    var fields = from ff in dc.scms_form_fields
                                 where ff.formid == nFormId.Value
                                 where ff.deleted == false
                                 orderby ff.ordinal, ff.name
                                 select ff;

                    lvFields.DataSource = fields;
                }

                lvFields.DataBind();

            }
            catch (Exception ex)
            {
                string strMessage = "Error occurred while loading fields.";
                ScmsEvent scmsEvent = new ScmsEvent(strMessage, this, ex);
                scmsEvent.Raise();
                statusMessage.ShowFailure(strMessage);
            }

        }



        protected void lvFields_ItemDataBound(object sender, ListViewItemEventArgs args)
        {
            ListViewItem item = args.Item;
            switch (args.Item.ItemType)
            {
                case ListViewItemType.DataItem:
                    {
                        ListViewDataItem dataItem = (ListViewDataItem)args.Item;
                        HtmlAnchor anchor = (HtmlAnchor)args.Item.FindControl("anchorEdit");
                        if (anchor != null)
                        {

                        }
                    }
                    break;
            }
        }

        protected void Select(object objSender, CommandEventArgs args)
        {
            int nFieldId = int.Parse((string)args.CommandArgument);
            if (OnFieldSelected != null)
            {
                OnFieldSelected(nFieldId);
            }
        }

        protected void Move(object objSender, CommandEventArgs args)
        {
            bool bUp = string.Compare(args.CommandName, "up", true) == 0;
            int nFieldId = int.Parse((string)args.CommandArgument);

            global::scms.data.ScmsDataContext dc = new global::scms.data.ScmsDataContext();

            if (FormId.HasValue)
            {
                var fields = from field in dc.scms_form_fields
                             where field.formid == FormId.Value
                             where field.deleted == false
                             orderby field.ordinal
                             select field;

                global::scms.data.scms_form_field prior = null;
                global::scms.data.scms_form_field next = null;

                var thisField = fields.Where(field => (field.id == nFieldId)).Single();
                int nThisOrdinal = thisField.ordinal;

                foreach (var field in fields)
                {
                    if (field.id != thisField.id)
                    {
                        if (field.ordinal < nThisOrdinal)
                        {
                            prior = field;
                        }
                        else
                        {
                            if (next == null)
                            {
                                if (field.ordinal > nThisOrdinal)
                                {
                                    next = field;
                                    break;
                                }
                            }
                        }
                    }
                }

                if (bUp)
                {
                    if (prior != null)
                    {
                        thisField.ordinal = prior.ordinal;
                        prior.ordinal = nThisOrdinal;
                    }
                }
                else
                {
                    if (next != null)
                    {
                        thisField.ordinal = next.ordinal;
                        next.ordinal = nThisOrdinal;
                    }
                }
            }


            dc.SubmitChanges();
            LoadFields();
        }



        protected void Delete(object objSender, CommandEventArgs args)
        {

            int nFieldId = int.Parse((string)args.CommandArgument);

            global::scms.data.ScmsDataContext dc = new global::scms.data.ScmsDataContext();

            var field = (from f in dc.scms_form_fields
                         where f.id == nFieldId
                         where f.deleted == false
                         select f).FirstOrDefault();
            if (field != null)
            {
                field.deleted = true;
                dc.SubmitChanges();

                if (OnFieldDeleted != null)
                {
                    OnFieldDeleted(nFieldId);
                }
            }
            LoadFields();
        }
    }


}