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

namespace scms.modules.rss.rssList
{
	public partial class edit : global::scms.RootControl
	{
		protected void Page_Load(object sender, EventArgs e)
		{
			if (!IsPostBack)
			{
        pageSelectorListReadMorePage.SiteId = this.SiteId;
				LoadFeeds();
        EnableControls(null, null);

				try
				{
					global::scms.data.ScmsDataContext dc = new scms.data.ScmsDataContext();
					var rssList = (from rl in dc.scms_rss_lists
												 where rl.instanceId == this.ModuleInstanceId.Value
												 select rl).FirstOrDefault();
          if (rssList != null) 
					{

						ddlFeeds.ClearSelection();
						ListItem liFeed = ddlFeeds.Items.FindByValue(rssList.rssId.ToString());
						if (liFeed != null)
						{
							liFeed.Selected = true;
						}

						checkTemplateEnabled.Checked = rssList.templateEnabled;

						if (checkTemplateEnabled.Checked)
						{
							txtHeaderTemplate.Text = rssList.templateHeaderHtml;
							txtItemTemplate.Text = rssList.templateHtml;
							txtSeparatorTemplate.Text = rssList.templateSeparatorHtml;
							txtFooterTemplate.Text = rssList.templateFooterHtml;
						}
						else
						{
							checkHeadingEnabled.Checked = rssList.headingEnabled;

							checkTitleEnabled.Checked = rssList.titleEnabled;
							if (checkTitleEnabled.Checked)
							{
								checkTitleAsLink.Checked = rssList.titleAsLink.Value;
							}
						}

            checkListLimitItems.Checked = rssList.listLimitItems;
            if (checkListLimitItems.Checked)
            {
                txtListMaxItems.Text = rssList.listMaxItems.ToString();
            }

						checkListReadMore.Checked = rssList.listReadMoreEnabled;
            if (checkListReadMore.Checked)
            {
							txtListReadMoreText.Text = rssList.listReadMoreText;
							pageSelectorListReadMorePage.PageId = rssList.listReadMorePageId;
            }

					}

					EnableControls(null, null);

				}
				catch (Exception ex)
				{
                    string strMessage = string.Format( "Exception thrown while loading feedlist for module id '{0}'.", this.ModuleInstanceId );
                    ScmsEvent.Raise(strMessage, this, ex);
					throw ex;
				}
			}
		}

		protected void checkChanged_Templated(object sender, EventArgs args)
		{
			EnableControls();
		}

		protected void EnableControls()
		{
		}

		protected void LoadFeeds()
		{
			try
			{
				ddlFeeds.DataTextField = "name";
				ddlFeeds.DataValueField = "id";

				global::scms.data.ScmsDataContext dc = new scms.data.ScmsDataContext();
				var rssFeeds = from rf in dc.scms_rsses
											 orderby rf.name
											 select rf;
				ddlFeeds.DataSource = rssFeeds;
				ddlFeeds.DataBind();
			}
			catch (Exception ex)
			{
				string strMessage = "Exception thrown while loading feeds ";
				ScmsEvent.Raise(strMessage, this, ex);
				throw ex;
			}
		}

    protected void EnableControls(object sender, EventArgs args)
    {
			bool bUseTemplate = checkTemplateEnabled.Checked;
			txtHeaderTemplate.Enabled = bUseTemplate;
			txtItemTemplate.Enabled = bUseTemplate;
			txtSeparatorTemplate.Enabled = bUseTemplate;
			txtFooterTemplate.Enabled = bUseTemplate;

			checkTitleEnabled.Enabled = !bUseTemplate;
			checkTitleAsLink.Enabled = !bUseTemplate &&  checkTitleEnabled.Checked;

      labelListMaxItems.Enabled = checkListLimitItems.Checked;
      txtListMaxItems.Enabled = checkListLimitItems.Checked;
      rfvListMaxItems.Enabled = checkListLimitItems.Checked;
      rvListMaxItems.Enabled = checkListLimitItems.Checked;

      checkListReadMore.Enabled = checkListLimitItems.Checked;
      labelListReadMoreText.Enabled = checkListLimitItems.Checked && checkListReadMore.Checked;
      txtListReadMoreText.Enabled = checkListLimitItems.Checked && checkListReadMore.Checked;
      rfvReadMoreText.Enabled = checkListLimitItems.Checked && checkListReadMore.Checked;

      labelListReadMorePage.Enabled = checkListLimitItems.Checked && checkListReadMore.Checked;
      pageSelectorListReadMorePage.Enabled = checkListLimitItems.Checked && checkListReadMore.Checked;
      rfvReadMorePage.Enabled = checkListLimitItems.Checked && checkListReadMore.Checked;
    }

		protected void btnSave_Click(object sender, EventArgs args)
		{
			try
			{
				global::scms.data.ScmsDataContext dc = new scms.data.ScmsDataContext();
				var rssList = (from rl in dc.scms_rss_lists
                       where rl.instanceId == this.ModuleInstanceId.Value
											 select rl).FirstOrDefault();
        if (rssList == null) 
				{
					rssList = new scms.data.scms_rss_list();
          rssList.instanceId = this.ModuleInstanceId.Value;
					dc.scms_rss_lists.InsertOnSubmit(rssList);
				}

				rssList.rssId = int.Parse(ddlFeeds.SelectedValue);


				rssList.templateEnabled = checkTemplateEnabled.Checked;
				if (rssList.templateEnabled)
				{
					rssList.templateHeaderHtml = txtHeaderTemplate.Text;
					rssList.templateHtml = txtItemTemplate.Text;
					rssList.templateSeparatorHtml = txtSeparatorTemplate.Text;
					rssList.templateFooterHtml = txtFooterTemplate.Text;
					rssList.headingEnabled = false;
					rssList.titleEnabled = false;
					rssList.titleAsLink = false;
				}
				else
				{
					rssList.templateHeaderHtml = null;
					rssList.templateHtml = null;
					rssList.templateSeparatorHtml = null;
					rssList.templateFooterHtml = null;

					rssList.headingEnabled = checkHeadingEnabled.Checked;

					rssList.titleEnabled = checkTitleEnabled.Checked;
					rssList.titleAsLink = null;
					if (rssList.titleEnabled)
					{
						rssList.titleAsLink = checkTitleAsLink.Checked;
					}
				}

        rssList.listLimitItems = checkListLimitItems.Checked;
        rssList.listMaxItems = null;
        if (rssList.listLimitItems)
        {
            rssList.listMaxItems = int.Parse(txtListMaxItems.Text);
        }

        rssList.listReadMoreEnabled = checkListReadMore.Checked;
        rssList.listReadMoreText = null;
        rssList.listReadMorePageId = null;
        if (rssList.listReadMoreEnabled)
        {
            rssList.listReadMoreText = txtListReadMoreText.Text;
            rssList.listReadMorePageId = pageSelectorListReadMorePage.PageId;
        }

				dc.SubmitChanges();
				statusMessage.ShowSuccess("Changes saved");
			}
			catch (Exception ex)
			{
                string strMessage = "Failed saving changes";
                ScmsEvent scmsEvent = new ScmsEvent(strMessage, this, ex);
                scmsEvent.Raise();
				statusMessage.ShowFailure(strMessage);
			}
		}

	}
}