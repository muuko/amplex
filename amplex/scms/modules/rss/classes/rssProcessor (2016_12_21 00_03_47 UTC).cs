using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml;
using System.Xml.Linq;

using global::scms;
using global::scms.data;


namespace scms.modules.rss.classes
{
	public class rssProcessor
	{
		protected const string strCacheKeyFormat = "rssProcessor[{0}]";

		protected class CacheItem
		{
			public scms_rss rss;
			public IEnumerable<scms_rss_item> iItems;
		}

		public bool GetFeed(int nFeedId, out scms_rss rss, out IEnumerable<scms_rss_item> iItems)
		{
			bool bSuccess = false;
			rss = null;
			iItems = null;

			string strCacheKey = string.Format(strCacheKeyFormat, nFeedId);

			CacheItem cacheItem = null;

			try
			{
				cacheItem = (CacheItem)System.Web.HttpRuntime.Cache[strCacheKey];
			}
			catch (Exception ex)
			{
				ScmsEvent.Raise("exception thrown while loading feed from cache", this, ex);
			}

			if (cacheItem == null)
			{
				cacheItem = new CacheItem();

				try
				{
					ScmsDataContext dc = new ScmsDataContext();
					rss = (from r in dc.scms_rsses
								 where r.id == nFeedId
								 select r).Single();


				}
				catch (Exception ex)
				{
					ScmsEvent.Raise(string.Format("exception thrown while loading feed id {0}", nFeedId), this, ex);
				}

				if (GetItemsFromLiveFeed(rss, out iItems))
				{
					// retain old items if configured
					// if (rss.retainDropOff)
					// {
					// }

					cacheItem.rss = rss;
					cacheItem.iItems = iItems;

					System.Web.HttpRuntime.Cache.Add(
						strCacheKey,
						cacheItem,
						null,
						DateTime.Now.AddSeconds(rss.expiresSeconds),
						System.Web.Caching.Cache.NoSlidingExpiration,
						System.Web.Caching.CacheItemPriority.Normal,
						null);
				}
			}

			if (cacheItem != null)
			{
				rss = cacheItem.rss;
				iItems = cacheItem.iItems;

				if ((rss != null) && (iItems != null))
				{
					bSuccess = true;
				}
			}

			return bSuccess;
		}

		protected bool GetItemsFromLiveFeed(scms_rss rss, out IEnumerable<scms_rss_item> iLiveItems)
		{
			bool bSuccess = false;
			iLiveItems = null;

			// deteremine required categories 
			HashSet<string> hsCategories = null;

			if (rss.categories != null)
			{
				hsCategories = new HashSet<string>();
				string[] astrCategories = rss.categories.Split(new char[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries);
				foreach (string strCategory in astrCategories)
				{
					hsCategories.Add(strCategory.ToLower());
				}
			}


			try
			{
				System.Net.HttpWebRequest webRequest = (System.Net.HttpWebRequest)System.Net.HttpWebRequest.Create(rss.feedUrl);
				System.Net.HttpWebResponse response = (System.Net.HttpWebResponse)webRequest.GetResponse();
				switch( response.StatusCode )
				{
					case System.Net.HttpStatusCode.OK:
					{
						
					}
					break;

					default:
						throw new Exception( string.Format("received unsuccesful status code from web request '{0}'", response.StatusCode) );
				}

				System.Collections.Generic.List<scms_rss_item> lItems = new List<scms_rss_item>();
				System.IO.Stream streamResponse = response.GetResponseStream();
				XmlReader xrResponse =  XmlReader.Create(streamResponse);
				XDocument xd = XDocument.Load(xrResponse);
				IEnumerable<XElement> elItems = xd.Descendants("item");
				foreach( XElement elItem in elItems )
				{
					string strTitle = null;
					XElement elTitle = elItem.Element("title");
					if( elTitle != null)
					{
						strTitle = elTitle.Value;
					}
					if( string.IsNullOrEmpty(strTitle))
					{
						continue;
					}

					string strLink = null;
					XElement elLink = elItem.Element("link");
					if( elLink != null)
					{
						strLink = elLink.Value;
					}
					if( string.IsNullOrEmpty(strLink))
					{
						continue;
					}

					string strDescription = null;
					XElement elDescription = elItem.Element("description");
					if (elDescription != null)
					{
						strDescription = elDescription.Value;
					}


					List<string> lstrCategories = new List<string>();
					IEnumerable<XElement> elsCategory = elItem.Elements("category");
					foreach (XElement elCategory in elsCategory)
					{
						lstrCategories.Add(elCategory.Value);
					}
					string strCategories = string.Join(",", lstrCategories.ToArray());

					bool bFilterIn = false;
					if( (hsCategories != null) && (hsCategories.Count > 0))
					{
						foreach( string strCategory in lstrCategories )
						{
							if( hsCategories.Contains(strCategory.ToLower()))
							{
								bFilterIn = true;
							}
						}

					}
					else
					{
						bFilterIn = true;
					}

					if( !bFilterIn )
					{
						continue;
					}

					

					// item requires title and link at minimum for this project
					scms_rss_item item = new scms_rss_item();
					item.title = strTitle;
					item.link = strLink;
					item.description = strDescription;
					item.categories = strCategories;
					lItems.Add(item);
				
					/*
					item.content;
					item.dc_creator;
					item.description;
					item.guid;
					item.pubDate;
					
					 * */
				}

				iLiveItems = lItems.AsEnumerable();
				bSuccess = true;
			}
			catch (Exception ex)
			{
				ScmsEvent.Raise("failed updating items from live feed", this, ex);
			}

			return bSuccess;
		}
	}
}
