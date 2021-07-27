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

namespace scms.modules.navigation.pagelist
{
	public partial class edit : global::scms.RootControl
	{
		protected void Page_Load(object sender, EventArgs e)
		{
			if (!IsPostBack)
			{
                pageSelectorRootPage.SiteId = this.SiteId;
                pageSelectorListReadMorePage.SiteId = this.SiteId;
                EnableControls(null, null);


				try
				{
					global::scms.data.ScmsDataContext dc = new scms.data.ScmsDataContext();

                    var pageList = (from pl in dc.scms_navigation_pagelists
                                    where pl.instanceId == this.ModuleInstanceId.Value
                                    select pl).FirstOrDefault();

                    if (pageList != null) 
					{
                        pageSelectorRootPage.PageId = pageList.rootPageId;

                        checkIncludeChildren.Checked = pageList.includeChildren;
                        if (checkIncludeChildren.Checked)
                        {
                            checkHideParentNodes.Checked = pageList.hideParentNodes;
                        }

                        checkTemplateEnabled.Checked = pageList.templateEnabled;
                        if (checkTemplateEnabled.Checked)
                        {
                            txtHeaderTemplate.Text = pageList.templateHeaderHtml;
                            txtItemTemplate.Text = pageList.templateHtml;
                            txtSeparatorTemplate.Text = pageList.templateSeparatorHtml;
                            txtFooterTemplate.Text = pageList.templateFooterHtml;

                            checkGroupTemplateEnabled.Checked = pageList.groupingEnabled.HasValue && pageList.groupingEnabled.Value;
                            txtItemsPerGroup.Text = pageList.groupingItemsPerGroup.ToString();
                            txtGroupHeaderTemplate.Text = pageList.templateGroupHeaderHtml;
                            txtGroupFooterTemplate.Text = pageList.templateGroupFooterHtml;
                        }
                        else
                        {
                            checkGroupTemplateEnabled.Checked = false;
                            txtItemsPerGroup.Text = null;
                            txtGroupHeaderTemplate.Text = null;
                            txtGroupFooterTemplate.Text = null;

                            checkTitleEnabled.Checked = pageList.titleEnabled;
                            if (checkTitleEnabled.Checked)
                            {
                                checkTitleAsLink.Checked = pageList.titleAsLink.Value;
                            }

                            checkLinkEnabled.Checked = pageList.linkTextEnabled;
                            if (checkLinkEnabled.Checked)
                            {
                                checkLinkAsLink.Checked = pageList.linkTextAsLink.Value;
                            }

                            checkAssociatedDateEnabled.Checked = pageList.associatedDateEnabled;
                            if (checkAssociatedDateEnabled.Checked)
                            {
                                txtAssociatedDateFormat.Text = pageList.associatedDateFormat;
                            }

                            checkThumbnailEnabled.Checked = pageList.thumbnailEnabled;
                            if (checkThumbnailEnabled.Checked)
                            {
                                checkThumbnailAsLink.Checked = pageList.thumbnailAsLink.HasValue && pageList.thumbnailAsLink.Value;
                                txtThumbnailWidth.Text = pageList.thumbnailWidth.ToString();
                                txtThumbnailHeight.Text = pageList.thumbnailHeight.ToString();
                            }

                            checkDescriptionEnabled.Checked = pageList.descriptionEnabled;
                            if (checkDescriptionEnabled.Checked)
                            {
                                checkDescriptionTruncated.Checked = pageList.descriptionTruncated.Value;
                                if (checkDescriptionTruncated.Checked)
                                {
                                    txtDescriptionTruncationLength.Text = pageList.descriptionTruncateLength.ToString();
                                }
                            }

                            checkItemReadMoreEnabled.Checked = pageList.itemReadMoreEnabled;
                            if (checkItemReadMoreEnabled.Checked)
                            {
                                txtItemReadMoreText.Text = pageList.itemReadMoreText;
                            }
                        }

                        checkListLimitItems.Checked = pageList.listLimitItems;
                        if (checkListLimitItems.Checked)
                        {
                            txtListMaxItems.Text = pageList.listMaxItems.ToString();
                        }

                        checkListReadMore.Checked = pageList.listReadMoreEnabled;
                        if (checkListReadMore.Checked)
                        {
                            txtListReadMoreText.Text = pageList.listReadMoreText;
                            pageSelectorListReadMorePage.PageId = pageList.listReadMorePageId;
                        }

                        checkPagingEnabled.Checked = pageList.pagingEnabled;
                        if (checkPagingEnabled.Checked)
                        {
                            txtPageSize.Text = pageList.pagingItemsPerPage.ToString();
                        }
						
                        checkSortAscending.Checked = pageList.ascending;

					}

					EnableControls(null, null);

				}
				catch (Exception ex)
				{
                    string strMessage = string.Format( "Exception thrown while loading pagelist for module id '{0}'.", this.ModuleInstanceId );
                    ScmsEvent.Raise(strMessage, this, ex);
					throw ex;
				}
			}

		}

        protected void EnableControls(object sender, EventArgs args)
        {
            if (checkTemplateEnabled.Checked)
            {
                multiView.SetActiveView(viewTemplate);

                labelItemsPerGroup.Enabled = checkGroupTemplateEnabled.Checked;
                txtItemsPerGroup.Enabled = checkGroupTemplateEnabled.Checked;
                rfvItemsPerGroup.Enabled = checkGroupTemplateEnabled.Checked;
                rvItemsPerGroup.Enabled = checkGroupTemplateEnabled.Checked;
                txtGroupHeaderTemplate.Enabled = checkGroupTemplateEnabled.Checked;
                txtGroupFooterTemplate.Enabled = checkGroupTemplateEnabled.Checked;
            }
            else
            {
                multiView.SetActiveView(viewNoTemplate);

                checkHideParentNodes.Enabled = checkIncludeChildren.Checked;
                checkTitleAsLink.Enabled = checkTitleEnabled.Checked;
                checkLinkAsLink.Enabled = checkLinkEnabled.Checked;

                labelAssociatedDateFormat.Enabled = checkAssociatedDateEnabled.Enabled;
                txtAssociatedDateFormat.Enabled = checkAssociatedDateEnabled.Checked;

                checkThumbnailAsLink.Enabled = checkThumbnailEnabled.Checked;
                txtThumbnailWidth.Enabled = checkThumbnailEnabled.Checked;
                txtThumbnailHeight.Enabled = checkThumbnailEnabled.Checked;
                labelThumbnailWidth.Enabled = checkThumbnailEnabled.Checked;
                labelThumbnailHeight.Enabled = checkThumbnailEnabled.Checked;
                rvThumbnailWidth.Enabled = checkThumbnailEnabled.Checked;
                rvThumbnailHeight.Enabled = checkThumbnailEnabled.Checked;

                checkDescriptionTruncated.Enabled = checkDescriptionEnabled.Checked;
                labelDescriptionTruncationLength.Enabled = checkDescriptionEnabled.Checked;
                txtDescriptionTruncationLength.Enabled = checkDescriptionEnabled.Checked && checkDescriptionTruncated.Checked;
                rvDescriptionTruncationLength.Enabled = checkDescriptionEnabled.Checked && checkDescriptionTruncated.Checked;
                rfvDescriptionTruncationLength.Enabled = checkDescriptionEnabled.Checked && checkDescriptionTruncated.Checked;

                labelItemReadMoreText.Enabled = checkItemReadMoreEnabled.Checked;
                txtItemReadMoreText.Enabled = checkItemReadMoreEnabled.Checked;
                rfvReadItemMoreText.Enabled = checkItemReadMoreEnabled.Checked;

                rfvItemsPerGroup.Enabled = false;
                rvItemsPerGroup.Enabled = false;
            }



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

            labelPageSize.Enabled = checkPagingEnabled.Checked;
            txtPageSize.Enabled = checkPagingEnabled.Checked;
            rfvPageSize.Enabled = checkPagingEnabled.Checked;
            rvPageSize.Enabled = checkPagingEnabled.Checked;
        }

		protected void btnSave_Click(object sender, EventArgs args)
		{
			try
			{
				global::scms.data.ScmsDataContext dc = new scms.data.ScmsDataContext();
                var pageList = (from pl in dc.scms_navigation_pagelists
                                where pl.instanceId == this.ModuleInstanceId.Value
                                select pl).FirstOrDefault();

                if (pageList == null) 
				{
                    pageList = new scms.data.scms_navigation_pagelist();
                    pageList.instanceId = this.ModuleInstanceId.Value;
					dc.scms_navigation_pagelists.InsertOnSubmit(pageList);
				}
                pageList.rootPageId = pageSelectorRootPage.PageId;

                pageList.includeChildren = checkIncludeChildren.Checked;
                pageList.hideParentNodes = false;
                if (checkIncludeChildren.Checked)
                {
                    pageList.hideParentNodes = checkHideParentNodes.Checked;
                }

                pageList.templateEnabled = checkTemplateEnabled.Checked;
                if (checkTemplateEnabled.Checked)
                {
                    pageList.templateHeaderHtml = txtHeaderTemplate.Text;
                    pageList.templateHtml = txtItemTemplate.Text;
                    pageList.templateSeparatorHtml = txtSeparatorTemplate.Text;
                    pageList.templateFooterHtml = txtFooterTemplate.Text;

                    pageList.groupingEnabled = checkGroupTemplateEnabled.Checked;

                    if (pageList.groupingEnabled.Value)
                    {
                        pageList.groupingItemsPerGroup = int.Parse(txtItemsPerGroup.Text);
                        pageList.templateGroupHeaderHtml = txtGroupHeaderTemplate.Text;
                        pageList.templateGroupFooterHtml = txtGroupFooterTemplate.Text;
                    }
                    else
                    {
                        pageList.groupingItemsPerGroup = null;
                        pageList.templateGroupHeaderHtml = null;
                        pageList.templateGroupFooterHtml = null;
                    }

                    pageList.titleEnabled = false;
                    pageList.titleAsLink = null;

                    pageList.linkTextEnabled = false;
                    pageList.linkTextAsLink = null;

                    pageList.associatedDateEnabled = false;
                    pageList.associatedDateFormat = null;

                    pageList.thumbnailEnabled = false;
                    pageList.thumbnailWidth = null;
                    pageList.thumbnailHeight = null;
                    
                    pageList.descriptionEnabled = false;
                    pageList.descriptionTruncated = null;
                    pageList.descriptionTruncateLength = null;

                    pageList.itemReadMoreEnabled = false;
                    pageList.itemReadMoreText = null;

                    
                }
                else
                {
                    pageList.templateHtml = null;
                    pageList.templateSeparatorHtml = null;

                    pageList.groupingEnabled = false;
                    pageList.groupingItemsPerGroup = null;
                    pageList.templateGroupHeaderHtml = null;
                    pageList.templateGroupFooterHtml = null;

                    pageList.titleEnabled = checkTitleEnabled.Checked;
                    pageList.titleAsLink = null;
                    if (pageList.titleEnabled)
                    {
                        pageList.titleAsLink = checkTitleAsLink.Checked;
                    }


                    pageList.linkTextEnabled = checkLinkEnabled.Checked;
                    pageList.linkTextAsLink = null;
                    if (pageList.linkTextEnabled)
                    {
                        pageList.linkTextAsLink = checkLinkAsLink.Checked;
                    }

                    pageList.associatedDateEnabled = checkAssociatedDateEnabled.Checked;
                    pageList.associatedDateFormat = null;
                    if (pageList.associatedDateEnabled)
                    {
                        pageList.associatedDateFormat = txtAssociatedDateFormat.Text;
                    }

                    pageList.thumbnailEnabled = checkThumbnailEnabled.Checked;
                    pageList.thumbnailAsLink = null;
                    pageList.thumbnailWidth = null;
                    pageList.thumbnailHeight = null;
                    if (pageList.thumbnailEnabled)
                    {
                        pageList.thumbnailAsLink = checkThumbnailAsLink.Checked;

                        if (!string.IsNullOrEmpty(txtThumbnailWidth.Text))
                        {
                            pageList.thumbnailWidth = int.Parse(txtThumbnailWidth.Text);
                        }

                        if (!string.IsNullOrEmpty(txtThumbnailHeight.Text))
                        {
                            pageList.thumbnailHeight = int.Parse(txtThumbnailHeight.Text);
                        }
                    }

                    pageList.descriptionEnabled = checkDescriptionEnabled.Checked;
                    pageList.descriptionTruncated = null;
                    pageList.descriptionTruncateLength = null;
                    if (pageList.descriptionEnabled)
                    {
                        pageList.descriptionTruncated = checkDescriptionTruncated.Checked;
                        if (checkDescriptionTruncated.Checked)
                        {
                            pageList.descriptionTruncateLength = int.Parse(txtDescriptionTruncationLength.Text);
                        }
                    }

                    pageList.itemReadMoreEnabled = checkItemReadMoreEnabled.Checked;
                    pageList.itemReadMoreText = null;
                    if (pageList.itemReadMoreEnabled)
                    {
                        pageList.itemReadMoreText = txtItemReadMoreText.Text;
                    }
                }

                pageList.listLimitItems = checkListLimitItems.Checked;
                pageList.listMaxItems = null;
                if (pageList.listLimitItems)
                {
                    pageList.listMaxItems = int.Parse(txtListMaxItems.Text);
                }

                pageList.listReadMoreEnabled = checkListReadMore.Checked;
                pageList.listReadMoreText = null;
                pageList.listReadMorePageId = null;
                if (pageList.listReadMoreEnabled)
                {
                    pageList.listReadMoreText = txtListReadMoreText.Text;
                    pageList.listReadMorePageId = pageSelectorListReadMorePage.PageId;
                }

                pageList.pagingEnabled = checkPagingEnabled.Checked;
                pageList.pagingItemsPerPage = null;
                if (pageList.pagingEnabled)
                {
                    pageList.pagingItemsPerPage = int.Parse(txtPageSize.Text);
                }

                pageList.ascending = checkSortAscending.Checked;

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