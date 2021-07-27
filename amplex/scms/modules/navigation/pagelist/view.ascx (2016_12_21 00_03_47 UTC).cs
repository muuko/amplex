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
	public partial class view :  RootControl 
	{
        protected scms.data.scms_navigation_pagelist pageList = null;
		protected void Page_Load(object sender, EventArgs e)
		{
			if (!IsPostBack)
			{
				if (this.ModuleInstanceId.HasValue )
				{
					scms.data.ScmsDataContext dc = new scms.data.ScmsDataContext();
                    pageList = (from pl in dc.scms_navigation_pagelists
                                where pl.instanceId == this.ModuleInstanceId
                                select pl).SingleOrDefault();
                    if (pageList != null)
                    {
                        int ? nPageNumber = null;
                        int ? nPageSize = null;
                        int? nCount = null;
                        if (pageList.pagingEnabled)
                        {
                            nPageSize = pageList.pagingItemsPerPage;

                            string strPageNumberQueryParm = string.Format("plp{0}", pageList.instanceId);
                            pager.PageNumberParm = strPageNumberQueryParm;

                            string strPageNumber = Request.QueryString[strPageNumberQueryParm];
                            if( !string.IsNullOrEmpty(strPageNumber))
                            {
                                int n;
                                if( int.TryParse(strPageNumber, out n))
                                {
                                    nPageNumber = n;
                                }
                            }
                        }

                        System.Collections.Generic.List<scms.data.scms_page> lPages = null;
                        lPages = dc.scms_pages_get(this.SiteId,
                            pageList.rootPageId,
                            pageList.includeChildren,
                            pageList.hideParentNodes,
                            nPageNumber,
                            nPageSize,
                            pageList.ascending,
                            ref nCount).ToList();
                        if (pageList.listLimitItems)
                        {
                            lPages = lPages.Take(pageList.listMaxItems.Value).ToList();
                        }

                        if (pageList.templateEnabled)
                        {
                            SetupTemplate(lPages);
                        }
                        else
                        {
                            SetupNoTemplate(lPages, nPageNumber, nPageSize, nCount);
                        }
                    }
				}
			}
		}

        protected void SetupTemplate(System.Collections.Generic.List<scms.data.scms_page> lPages)
        {
            multiView.SetActiveView(viewTemplate);

            if( lPages.Count > 0 )
            {
                System.Text.StringBuilder sbPageList = new System.Text.StringBuilder();
                sbPageList.Append(pageList.templateHeaderHtml);


                int nPageCount = 0;
                foreach (scms.data.scms_page page in lPages)
                {
                    bool bNewGroup = false;
                    if (pageList.groupingEnabled.HasValue && pageList.groupingEnabled.Value)
                    {
                        if (nPageCount % pageList.groupingItemsPerGroup.Value == 0)
                        {
                            if (nPageCount > 0)
                            {
                                sbPageList.Append(pageList.templateGroupFooterHtml);
                            }

                            bNewGroup = true;
                            sbPageList.Append(pageList.templateGroupHeaderHtml);
                        }
                    }


                    if (!string.IsNullOrEmpty(pageList.templateSeparatorHtml))
                    {
                        if( nPageCount > 0 )
                        {
                            if (!bNewGroup)
                            {
                                sbPageList.Append(pageList.templateSeparatorHtml);
                            }
                        }
                    }



                    {
                        System.Text.StringBuilder sb = new System.Text.StringBuilder(pageList.templateHtml);
                        sb.Replace("#TITLE#", page.title);
                        sb.Replace("#LINKTEXT#", page.linktext);
                        sb.Replace("#URL#", page.url);

                        string strLongDate = null;
                        if (page.associatedDate.HasValue)
                        {
                            strLongDate = page.associatedDate.Value.ToLongDateString();
                        }
                        sb.Replace("#LONGDATE#", strLongDate);

                        string strShortDate = null;
                        if (page.associatedDate.HasValue)
                        {
                            strShortDate = page.associatedDate.Value.ToShortDateString();
                        }
                        sb.Replace("#SHORTDATE#", strShortDate);

                        sb.Replace("#DESCRIPTION#", page.summary);

                        string strImageUrl = null;
                        if (!string.IsNullOrEmpty(page.thumbnail))
                        {
                            strImageUrl = page.thumbnail;
                        }
                        sb.Replace("#IMAGEURL#", strImageUrl);

                        string strImageEncoded = null;
                        if (!string.IsNullOrEmpty(page.thumbnail))
                        {
                            strImageEncoded = HttpUtility.UrlEncode(page.thumbnail);
                        }
                        sb.Replace("#IMAGEURL_ENCODED#", strImageEncoded);

                        sbPageList.Append(sb);
                    }

                    nPageCount++;
                }

                if (pageList.groupingEnabled.HasValue && pageList.groupingEnabled.Value)
                {
                    sbPageList.Append(pageList.templateGroupFooterHtml);
                }

                sbPageList.Append(pageList.templateFooterHtml);

                literalTemplate.Text = sbPageList.ToString();
            }
        }

        protected void rptNoTemplatePageList_ItemDataBound(object sender, RepeaterItemEventArgs args)
        {
            switch (args.Item.ItemType)
            {
                case ListItemType.Item:
                case ListItemType.AlternatingItem:
                    {
                        scms.data.scms_page page = args.Item.DataItem as scms.data.scms_page;
                        if (page != null)
                        {
                            System.Text.StringBuilder sb = new System.Text.StringBuilder(pageList.templateHtml);
                            sb.Replace( "#TITLE#", page.title );
                            sb.Replace( "#LINKTEXT#", page.linktext );
                            sb.Replace( "#URL#", page.url );
                            
                            string strLongDate = null;
                            if( page.associatedDate.HasValue )
                            {
                                strLongDate = page.associatedDate.Value.ToLongDateString();
                            }
                            sb.Replace( "#LONGDATE#", strLongDate );
                            
                            string strShortDate = null;
                            if( page.associatedDate.HasValue )
                            {
                                strShortDate = page.associatedDate.Value.ToShortDateString();
                            }
                            sb.Replace( "#SHORTDATE#", strShortDate);

                            sb.Replace("#DESCRIPTION#", page.summary);

                            string strImageUrl = null;
                            if( !string.IsNullOrEmpty(page.thumbnail))
                            {
                                strImageUrl = page.thumbnail;    
                            }
                            sb.Replace( "#IMAGEURL#", strImageUrl );

                            string strImageEncoded = null;
                            if( !string.IsNullOrEmpty(page.thumbnail ))
                            {
                                strImageEncoded = HttpUtility.UrlEncode(page.thumbnail);
                            }
                            sb.Replace("#IMAGEURL_ENCODED#", strImageEncoded);

                            Literal literalItem = args.Item.FindControl("literalItem") as Literal;
                            if (literalItem != null)
                            {
                                literalItem.Text = sb.ToString();
                            }
                        }
                    }
                    break;

                case ListItemType.Separator:
                    {
                        Literal literalSeparator = args.Item.FindControl("literalSeparator") as Literal;
                        if (literalSeparator != null)
                        {
                            literalSeparator.Text = pageList.templateSeparatorHtml;
                        }
                    }
                    break;

                case ListItemType.Header:
                    {
                        Literal literalHeader = args.Item.FindControl("literalHeader") as Literal;
                        if (literalHeader != null)
                        {
                            literalHeader.Text = pageList.templateHeaderHtml;
                        }
                    }
                    break;

                case ListItemType.Footer:
                    {
                        Literal literalFooter = args.Item.FindControl("literalFooter") as Literal;
                        if (literalFooter != null)
                        {
                            literalFooter.Text = pageList.templateFooterHtml;
                        }
                    }
                    break;
            
            }
        }

        protected void SetupNoTemplate(System.Collections.Generic.List<scms.data.scms_page> lPages, int? nPageNumber, int? nPageSize, int? nCount)
        {
            multiView.SetActiveView(viewNoTemplate);
            rptNoTemplatePageListItem.DataSource = lPages;
            rptNoTemplatePageListItem.DataBind();

            if (pageList.listLimitItems && pageList.listReadMorePageId.HasValue)
            {
                string strReadMoreText = "Read More";
                if (!string.IsNullOrEmpty(pageList.listReadMoreText))
                {
                    strReadMoreText = pageList.listReadMoreText;
                }
                anchorReadMore.InnerHtml = strReadMoreText;

                ScmsSiteMapProvider provider = new ScmsSiteMapProvider();
                ScmsSiteMapProvider.PageNode pageNode;
                string strError;
                Exception exError;
                if (provider.GetPageNode(pageList.listReadMorePageId.Value, out pageNode, out strError, out exError))
                {
                    anchorReadMore.HRef = pageNode.page.url;
                }
            }
            else
            {
                divReadMore.Visible = false;
            }

            if (pageList.pagingEnabled)
            {
                pager.PageNumber = nPageNumber;
                pager.PageSize = nPageSize;
                pager.Count = nCount;
            }
            else
            {
                pager.Visible = false;
            }
        }


        protected void rptNoTemplatePageListItem_ItemDataBound(object sender, RepeaterItemEventArgs args)
        {
            switch (args.Item.ItemType)
            {
                case ListItemType.Item:
                case ListItemType.AlternatingItem:
                    {
                        scms.data.scms_page page = args.Item.DataItem as scms.data.scms_page;
                        if (page != null)
                        {
                            bool bNoImage = false;

                            if (pageList.thumbnailEnabled)
                            {
                                if (string.IsNullOrEmpty(page.thumbnail))
                                {
                                    bNoImage = true;
                                }
                                else
                                {
                                    string strThumbnailPath = Server.MapPath(page.thumbnail);
                                    bool bFileFound = false;
                                    try
                                    {
                                        bFileFound = System.IO.File.Exists(strThumbnailPath);
                                    }
                                    catch
                                    {
                                    }
                                    bNoImage = !bFileFound;

                                    if (bFileFound)
                                    {
                                        Literal literalThumbnail = args.Item.FindControl("literalThumbnail") as Literal;
                                        if (literalThumbnail != null)
                                        {
                                            System.Text.StringBuilder sbThumbnail = new System.Text.StringBuilder();
                                            if (pageList.thumbnailAsLink.HasValue && pageList.thumbnailAsLink.Value)
                                            {
                                                sbThumbnail.AppendFormat("<a href=\"{0}\">", page.url);
                                            }

                                            sbThumbnail.Append("<img ");

                                            string strThumbnailUrl = null;
                                            int? nWidth = pageList.thumbnailWidth;
                                            int? nHeight = pageList.thumbnailHeight;
                                            if (nWidth.HasValue || nHeight.HasValue)
                                            {
                                                string strImageWidthParm = null;
                                                string strImageHeightParm = null;
                                                if( nWidth.HasValue )
                                                {
                                                    strImageWidthParm = string.Format("&w={0}", nWidth.Value);
                                                }

                                                if (nHeight.HasValue)
                                                {
                                                    strImageHeightParm = string.Format("&h={0}", nHeight.Value);
                                                }

                                                strThumbnailUrl = string.Format("/Image.ashx?src={0}&m=grow{1}{2}", page.thumbnail, strImageWidthParm, strImageHeightParm);
                                            }
                                            else
                                            {
                                                strThumbnailUrl = page.thumbnail;
                                            }
                                            sbThumbnail.AppendFormat("src=\"{0}\" />", strThumbnailUrl);
                                            
                                            if (pageList.thumbnailAsLink.HasValue && pageList.thumbnailAsLink.Value)
                                            {
                                                sbThumbnail.Append("</a>");
                                            }
                                            literalThumbnail.Text = sbThumbnail.ToString();
                                        }
                                    }
                                }
                            }
                            else
                            {
                                bNoImage = true;
                            }

                            if (bNoImage)
                            {
                                if( pageList.thumbnailHideIfEmpty.HasValue && pageList.thumbnailHideIfEmpty.Value )
                                {
                                    HtmlGenericControl divThumbnail = args.Item.FindControl("divThumbnail") as HtmlGenericControl;
                                    if (divThumbnail != null)
                                    {
                                        divThumbnail.Visible = false;
                                    }
                                }
                            }

                            if (pageList.associatedDateEnabled)
                            {
                                Literal literalDate = args.Item.FindControl("literalDate") as Literal;
                                if (literalDate != null)
                                {
                                    string strDate = null;
                                    if (page.associatedDate != null)
                                    {
                                        string strDateFormat = null;
                                        if (string.IsNullOrEmpty(pageList.associatedDateFormat))
                                        {
                                            strDateFormat = "M/dd/yyyy";
                                        }
                                        else
                                        {
                                            strDateFormat = pageList.associatedDateFormat;
                                        }

                                        strDate = page.associatedDate.Value.ToString(strDateFormat);
                                    }
                                    literalDate.Text = strDate;
                                }
                            }
                            else
                            {
                                HtmlGenericControl divDate = args.Item.FindControl("divDate") as HtmlGenericControl;
                                if (divDate != null)
                                {
                                    divDate.Visible = false;
                                }
                            }


                            if (pageList.titleEnabled)
                            {
                                Literal literalTitle = args.Item.FindControl("literalTitle") as Literal;
                                if (literalTitle != null)
                                {
                                    if (!string.IsNullOrEmpty(page.title))
                                    {
                                        System.Text.StringBuilder sbTitle = new System.Text.StringBuilder();
                                        if (pageList.titleAsLink.HasValue && pageList.titleAsLink.Value)
                                        {
                                            sbTitle.AppendFormat("<a href=\"{0}\">", page.url);
                                        }

                                        sbTitle.Append(page.title);

                                        if (pageList.titleAsLink.HasValue && pageList.titleAsLink.Value)
                                        {
                                            sbTitle.Append("</a>");
                                        }
                                        literalTitle.Text = sbTitle.ToString();
                                    }
                                }
                            }
                            else
                            {
                                HtmlGenericControl divTitle = args.Item.FindControl("divTitle") as HtmlGenericControl;
                                if (divTitle != null)
                                {
                                    divTitle.Visible = false;
                                }
                            }

                            if (pageList.linkTextEnabled)
                            {
                                Literal literalLinkText = args.Item.FindControl("literalLinkText") as Literal;
                                if (literalLinkText != null)
                                {
                                    if (!string.IsNullOrEmpty(page.linktext))
                                    {
                                        System.Text.StringBuilder sbLinkText = new System.Text.StringBuilder();
                                        if (pageList.linkTextAsLink.HasValue && pageList.linkTextAsLink.Value)
                                        {
                                            sbLinkText.AppendFormat("<a href=\"{0}\">", page.url);
                                        }

                                        sbLinkText.Append(page.linktext);

                                        if (pageList.linkTextAsLink.HasValue && pageList.linkTextAsLink.Value)
                                        {
                                            sbLinkText.Append("</a>");
                                        }
                                        literalLinkText.Text = sbLinkText.ToString();
                                    }
                                }
                            }
                            else
                            {
                                HtmlGenericControl divLinkText = args.Item.FindControl("divLinkText") as HtmlGenericControl;
                                if (divLinkText != null)
                                {
                                    divLinkText.Visible = false;
                                }
                            }

                            HtmlGenericControl divDescription = args.Item.FindControl("divDescription") as HtmlGenericControl;
                            if (divDescription != null)
                            {
                                if (pageList.descriptionEnabled && !string.IsNullOrEmpty(page.summary))
                                {
                                    string strDescription = page.summary;
                                    if (pageList.descriptionTruncated.HasValue && pageList.descriptionTruncated.Value)
                                    {
                                        if (pageList.descriptionTruncateLength.HasValue)
                                        {
                                            if (strDescription.Length > pageList.descriptionTruncateLength.Value)
                                            {
                                                strDescription = strDescription.Substring(0, pageList.descriptionTruncateLength.Value) + " ...";
                                            }
                                        }
                                    }
                                    divDescription.InnerText = strDescription;
                                }
                                else
                                {
                                    divDescription.Visible = false;
                                }
                            }
                        }
                    }
                    break;
            }
        }
	}
}