using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


namespace amplex.scms.modules.parts.classes
{
	public class PartsLoader 
	{

		// on initial load, this list represents all current sage ids and their corresponding hashs
		// as parts are processed from input file, they are removed from the dictionary
		// the remaining ones should be deleted
		protected System.Collections.Generic.Dictionary<int, string> dictHashesBySageSKIC = null;
		protected System.Collections.Generic.Dictionary<int,ICPart> dictICPartsToCreate = null;
		protected System.Collections.Generic.Dictionary<int,ICPart> dictICPartsToUpdate = null;
		protected amplex.scms.modules.parts.classes.cat_setting settings = null;
		protected bool bInitialized = false;

		public bool bProcessSuccess = false;

		public PartsLoader()
		{
			
		}

		public bool Init()
		{
			try
			{
				// just one entry in settings
				amplex.scms.modules.parts.classes.partsDataContext dcParts = new partsDataContext();
				settings = (from s in dcParts.cat_settings
										where s.searchResultsPageModuleInstanceId != null
										select s).FirstOrDefault();
				if (settings != null)
				{
					bInitialized = LoadCurrentPartsHashes();
				}
			}
			catch (Exception ex)
			{
				global::scms.ScmsEvent.Raise("failed loading parts settings", this, ex);
			}
			
			return bInitialized;
		}

		protected bool LoadCurrentPartsHashes()
		{
			bool bSuccess = false;

			try
			{
				scms.modules.parts.classes.partsDataContext dc = new partsDataContext();
				var ieAllHashessBySageSKICs = from p in dc.cat_parts
																				select new { p.sage_SKICPart, p.hash };
				dictHashesBySageSKIC = new Dictionary<int, string>();
				foreach (var hashBySageId in ieAllHashessBySageSKICs)
				{
					dictHashesBySageSKIC[hashBySageId.sage_SKICPart] = hashBySageId.hash;
				}
				bSuccess = true;
			}
			catch( Exception ex )
			{
				global::scms.ScmsEvent.Raise("failed loading parts hashs", this, ex);
			}

			return bSuccess;
		}

		public bool LoadPartsFile(string strPath, bool bCompressed)
		{
			bool bSuccess = false;

			try
			{
				System.IO.Stream streamFileInput = new System.IO.FileStream(strPath, 
					System.IO.FileMode.Open, 
					System.IO.FileAccess.Read, 
					System.IO.FileShare.Read);

				bSuccess = LoadParts(streamFileInput, bCompressed);
				if (!bSuccess)
				{
					string strMessage = string.Format("failed loading parts file '{0}'", strPath);
					global::scms.ScmsEvent.Raise(strMessage, this, null);
				}
				streamFileInput.Close();
			}
			catch (Exception ex)
			{
				string strMessage = string.Format("failed loading parts file '{0}'", strPath);
				global::scms.ScmsEvent.Raise(strMessage, this, ex);
			}

			return bSuccess;
		}

		public bool LoadParts(System.IO.Stream streamInput, bool bCompressed)
		{
			bool bSuccess = false;

			if (!bInitialized)
				throw new Exception("parts loader not initialized");

			dictICPartsToCreate = new Dictionary<int, ICPart>();
			dictICPartsToUpdate = new Dictionary<int, ICPart>();

			try
			{
				System.IO.Stream streamToProcess = null;

				if (bCompressed)
				{
					if (!Uncompress(streamInput, out streamToProcess))
					{
						throw new Exception("Failed to decompress input");
					}
				}
				else
				{
					streamToProcess = streamInput;
				}

				streamToProcess.Seek(0, System.IO.SeekOrigin.Begin);
				System.Xml.XPath.XPathDocument xpd = new System.Xml.XPath.XPathDocument(streamToProcess);

				System.Xml.XPath.XPathNavigator xpn = xpd.CreateNavigator();
				System.Xml.XPath.XPathNodeIterator xpni = xpn.Select("root/ICParts/ICPart");
				while (xpni.MoveNext())
				{
					bool bCreate = false;
					bool bUpdate = false;
					
					bool bKeep = false;
					int ? nSKICPart = null;

					string strSKICPart = xpni.Current.GetAttribute("SKICPart", string.Empty);
					string strID = xpni.Current.GetAttribute("ID", string.Empty);
					string strHash = xpni.Current.GetAttribute("Hash", string.Empty);
					string strPrice = xpni.Current.GetAttribute("BasePrice", string.Empty);
					if (!string.IsNullOrEmpty(strSKICPart))
					{
						int n;
						if (int.TryParse(strSKICPart, out n ))
						{
							nSKICPart = n;
							string strExistingHash;
							if (dictHashesBySageSKIC.TryGetValue(nSKICPart.Value, out strExistingHash))
							{
								if (string.Compare(strHash, strExistingHash) != 0)
								{
									bUpdate = true;
								}
								else
								{
									bKeep = true;
								}
							}
							else
							{
								bCreate = true;
							}

							if (bCreate || bUpdate)
							{
								string strDescription1 = null;
								string strDescription2 = null;
								string strDescription3 = null;
								string strLongDescription = null;
								string strProductCategoryDesc1 = null;
								string strProductCategoryDesc2 = null;
								string strProductCategoryDesc3 = null;
								string strProductCategoryDesc4 = null;
								string strProductCategoryDesc5 = null;
								if (xpni.Current.MoveToFirstChild())
								{
									do
									{
										switch (xpni.Current.Name)
										{
											case "Description1":
												strDescription1 = xpni.Current.Value;
												break;

											case "Description2":
												strDescription2 = xpni.Current.Value;
												break;

											case "Description3":
												strDescription3 = xpni.Current.Value;
												break;

											case "LongDescription":
												strLongDescription = xpni.Current.Value;
												break;

											case "ProductCategoryDesc1":
												if (!string.IsNullOrEmpty(xpni.Current.Value))
												{
													strProductCategoryDesc1 = xpni.Current.Value;
												}
												break;

											case "ProductCategoryDesc2":
												if (!string.IsNullOrEmpty(xpni.Current.Value))
												{
													strProductCategoryDesc2 = xpni.Current.Value;
												}
												break;

											case "ProductCategoryDesc3":
												if (!string.IsNullOrEmpty(xpni.Current.Value))
												{
													strProductCategoryDesc3 = xpni.Current.Value;
												}
												break;

											case "ProductCategoryDesc4":
												if (!string.IsNullOrEmpty(xpni.Current.Value))
												{
													strProductCategoryDesc4 = xpni.Current.Value;
												}
												break;

											case "ProductCategoryDesc5":
												if (!string.IsNullOrEmpty(xpni.Current.Value))
												{
													strProductCategoryDesc5 = xpni.Current.Value;
												}
												break;

										}
									}
									while (xpni.Current.MoveToNext());

									xpni.Current.MoveToParent();
								}

								if (!ShouldPartBeSkipped(strDescription1, strDescription2, strDescription3, strLongDescription))
								{
									decimal? dPrice = null;
									decimal d;
									if (decimal.TryParse(strPrice, out d))
									{
										dPrice = d;
									}

									ICPart part = new ICPart
									{
										SKICPart = nSKICPart.Value,
										dPrice = dPrice,
										ID = strID,
										Hash = strHash,
										Description1 = strDescription1,
										Description2 = strDescription2,
										Description3 = strDescription3,
										LongDescription = strLongDescription,
										ProductCategoryDesc1 = strProductCategoryDesc1,
										ProductCategoryDesc2 = strProductCategoryDesc2,
										ProductCategoryDesc3 = strProductCategoryDesc3,
										ProductCategoryDesc4 = strProductCategoryDesc4,
										ProductCategoryDesc5 = strProductCategoryDesc5
									};

									if (bCreate)
									{
										dictICPartsToCreate.Add(nSKICPart.Value, part);
									}
									else if (bUpdate)
									{
										dictICPartsToUpdate.Add(nSKICPart.Value, part);
										bKeep = true;
									}
									else
									{
										throw new Exception("unexpected");
									}
								}
							}
						}
					}

					if (bKeep)
					{
						// unchanged, remove now from dictionary to keep it from being deleted
						if (dictHashesBySageSKIC.ContainsKey(nSKICPart.Value))
						{
							dictHashesBySageSKIC.Remove(nSKICPart.Value);
						}
					}
				}

				bSuccess = true;
			}
			catch (Exception ex)
			{
				string strMessage = "failed loading parts";
				global::scms.ScmsEvent.Raise(strMessage, this, ex);
			}

			return bSuccess;
		}

		protected bool ShouldPartBeSkipped(string strDescription1, string strDescription2, string strDescription3, string strLongDescription)
		{
			bool bSkip = false;

			string[] astrFields = new string[] { 
				strDescription1,
				strDescription2,
				strDescription3 };

			foreach (string strField in astrFields)
			{
				if (!string.IsNullOrEmpty(strField))
				{
					if( (strField.IndexOf("do not use", StringComparison.OrdinalIgnoreCase) >= 0) 
						|| (strField.IndexOf("do not sell", StringComparison.OrdinalIgnoreCase) >= 0))
					{
						bSkip = true;
						break;
					}
				}
			}

			return bSkip;
		}

		protected bool Uncompress(System.IO.Stream streamInput, out System.IO.Stream streamOutput)
		{
			bool bSuccess = false;
			streamOutput = null;

			try
			{
				streamOutput = new System.IO.MemoryStream();

				streamInput.Seek(0, System.IO.SeekOrigin.Begin);

				System.IO.Compression.DeflateStream deflateStream = new System.IO.Compression.DeflateStream(streamInput, System.IO.Compression.CompressionMode.Decompress);

				int nBytesRead = 0;
				byte[] abyData = new byte[4096];
				do
				{
					nBytesRead = deflateStream.Read(abyData, 0, abyData.Length);
					if (nBytesRead > 0)
					{
						streamOutput.Write(abyData, 0, nBytesRead);
					}
				} 
				while (nBytesRead > 0);

				streamOutput.Flush();

				bSuccess = true;
			}
			catch (Exception ex)
			{
				string strMessage = string.Format("failed decompressing parts file");
				global::scms.ScmsEvent.Raise(strMessage, this, ex);
			}

			return bSuccess;
		}

		protected void GetSearchOverrides(amplex.scms.modules.parts.classes.cat_part part, out string strQueryString, out string strTitleOverride, out string strSummaryOverride)
		{
			strQueryString = string.Format("p={0}", part.id);
			strTitleOverride = null;
			strTitleOverride = string.Format("{0} - [{1}]", part.sage_Description1, part.sage_Description2);
			strSummaryOverride = part.sage_LongDescription;
		}

		public void ProcessParts()
		{
			bool bSuccess = false;

			global::scms.ScmsEvent.Raise("PartsLoader.ProcessParts() started", this, null);

			global::scms.IPluginApplication iPluginApplication = null;
			global::scms.PluginApplications.GetApplication(typeof(global::scms.modules.parts.partsApplication), out iPluginApplication);
			global::scms.modules.parts.partsApplication partsApplication = (global::scms.modules.parts.partsApplication)iPluginApplication;

			try
			{
				global::scms.search.search search = new global::scms.search.search();
				if (!search.Init())
					throw new Exception("failed initializing search");

				scms.modules.parts.classes.partsDataContext dc = new partsDataContext();
				
				// mark any part to delete that still remains after processing
				var partsToDelete = from pCurrent in dc.cat_parts
														where dictHashesBySageSKIC.Keys.Contains(pCurrent.sage_SKICPart)
														select pCurrent;

				// remove from searchf
				global::scms.data.ScmsDataContext dcScms = new global::scms.data.ScmsDataContext();
				foreach (var partToDelete in partsToDelete)
				{
					dcScms.scms_search_clear(settings.searchResultsPageId, settings.searchResultsPageModuleInstanceId, partToDelete.id);
				}

				// delete the parts
				dc.cat_parts.DeleteAllOnSubmit(partsToDelete);
				dc.SubmitChanges();


				// group parts to update by 1000 rows to prevent error, limit is 2100fs
				int nCopied = 0;
				int nCopyGroupSize = 1000;
				List<List<int>> llPartsToUpdate = new List<List<int>>();
				while (nCopied < dictICPartsToUpdate.Keys.Count)
				{
					List<int> lPartsToUpdate = new List<int>();
					lPartsToUpdate.AddRange(dictICPartsToUpdate.Keys.Skip(nCopied).Take(nCopyGroupSize));
					llPartsToUpdate.Add(lPartsToUpdate);
					nCopied += lPartsToUpdate.Count;
				}

				foreach (var lPartsToUpdate in llPartsToUpdate)
				{

					// update any parts
					System.Collections.Generic.List<cat_part> lPartsUpdated = new List<cat_part>();
					var partsToUpdate = from pCurrentToUpdate in dc.cat_parts
															where lPartsToUpdate.Contains(pCurrentToUpdate.sage_SKICPart)
															select pCurrentToUpdate;
					foreach (var partToUpdate in partsToUpdate)
					{
						ICPart icPart = dictICPartsToUpdate[partToUpdate.sage_SKICPart];
						partToUpdate.hash = icPart.Hash;
						partToUpdate.lastUpdated = DateTime.Now;
						partToUpdate.sage_price = icPart.dPrice;
						partToUpdate.sage_ID = icPart.ID;
						partToUpdate.sage_Description1 = icPart.Description1;
						partToUpdate.sage_Description2 = icPart.Description2;
						partToUpdate.sage_Description3 = icPart.Description3;
						partToUpdate.sage_LongDescription = icPart.LongDescription;
						if (string.Compare(partToUpdate.sage_ProductCategoryDesc1, icPart.ProductCategoryDesc1) != 0)
						{
							partToUpdate.sage_ProductCategoryDesc1 = icPart.ProductCategoryDesc1;
						}

						if (string.Compare(partToUpdate.sage_ProductCategoryDesc2, icPart.ProductCategoryDesc2) != 0)
						{
							EnsureSize(dc, icPart.ProductCategoryDesc2);
							partToUpdate.sage_ProductCategoryDesc2 = icPart.ProductCategoryDesc2;
						}

						if (string.Compare(partToUpdate.sage_ProductCategoryDesc3, icPart.ProductCategoryDesc3) != 0)
						{
							partToUpdate.sage_ProductCategoryDesc3 = icPart.ProductCategoryDesc3;
						}

						if (string.Compare(partToUpdate.sage_ProductCategoryDesc4, icPart.ProductCategoryDesc4) != 0)
						{
							partToUpdate.sage_ProductCategoryDesc4 = icPart.ProductCategoryDesc4;
						}

						if (string.Compare(partToUpdate.sage_ProductCategoryDesc5, icPart.ProductCategoryDesc5) != 0)
						{
							partToUpdate.sage_ProductCategoryDesc5 = icPart.ProductCategoryDesc5;
						}

						if (partToUpdate.sage_SKICPart != icPart.SKICPart)
							throw new Exception("Unexpected part mismatch");
					}

					dc.SubmitChanges();
					partsApplication.IndexParts(search, partsToUpdate.ToList());
				}

				// insert new parts
				System.Collections.Generic.List<cat_part> lPartsInserted = new List<cat_part>();
				foreach (var icPartToCreate in dictICPartsToCreate.Values)
				{
					cat_part part = new cat_part
					{
						sage_SKICPart = icPartToCreate.SKICPart,
						sage_ID = icPartToCreate.ID,
						hash = icPartToCreate.Hash,
						sage_price = icPartToCreate.dPrice,
						sage_Description1 = icPartToCreate.Description1,
						sage_Description2 = icPartToCreate.Description2,
						sage_Description3 = icPartToCreate.Description3,
						sage_LongDescription = icPartToCreate.LongDescription,
						sage_ProductCategoryDesc1 = icPartToCreate.ProductCategoryDesc1,
						sage_ProductCategoryDesc2 = icPartToCreate.ProductCategoryDesc2,
						sage_ProductCategoryDesc3 = icPartToCreate.ProductCategoryDesc3,
						sage_ProductCategoryDesc4 = icPartToCreate.ProductCategoryDesc4,
						sage_ProductCategoryDesc5 = icPartToCreate.ProductCategoryDesc5,
						lastUpdated = DateTime.Now
					};

					EnsureSize(dc, icPartToCreate.ProductCategoryDesc2);

					dc.cat_parts.InsertOnSubmit(part);
					lPartsInserted.Add(part);
				}
				dc.SubmitChanges();
				partsApplication.IndexParts(search, lPartsInserted);

				bSuccess = true;

				/*
				var partsToUpdate = from pCurrentToUpdate in dc.cat_parts
														join pToUpdate in lICPartsToUpdate.
				*/


			}
			catch (Exception ex)
			{
				string strMessage = string.Format("failed processing parts");
				global::scms.ScmsEvent.Raise(strMessage, this, ex);
			}

			this.bProcessSuccess = bSuccess;

			global::scms.ScmsEvent.Raise("PartsLoader.ProcessParts() complete", this, null);
		}

		HashSet<string> hsSizes = new HashSet<string>();
		protected void EnsureSize(scms.modules.parts.classes.partsDataContext dc, string strSizeId)
		{
			if (!string.IsNullOrEmpty(strSizeId))
			{
				string strSizeUpper = strSizeId.ToUpper();
				if (!hsSizes.Contains(strSizeUpper))
				{

					try
					{
						bool bFound = false;

						int nMaxOrdinal = 0;

						var searchCatSizes = from s in dc.cat_sizes
																 select s;

						foreach (var searchCatSize in searchCatSizes)
						{
							nMaxOrdinal = Math.Max(nMaxOrdinal, searchCatSize.ordinal);

							string strSearchCatSizeUpper = searchCatSize.id.ToUpper();
							if (!hsSizes.Contains(strSearchCatSizeUpper))
							{
								hsSizes.Add(strSearchCatSizeUpper);
								if (string.Compare(strSearchCatSizeUpper, strSizeUpper, true) == 0)
								{
									bFound = true;
								}
							}
						}

						if (!bFound)
						{
							cat_size cs = new cat_size();
							cs.id = strSizeUpper;
							cs.ordinal = nMaxOrdinal + 1;
							dc.cat_sizes.InsertOnSubmit(cs);
							dc.SubmitChanges();
						}

					}
					catch (Exception ex)
					{
						string strMessage = string.Format("Failed ensuring cat size '{0}'.", strSizeId);
						global::scms.ScmsEvent.Raise(strMessage, this, ex);
					}
				}
			}
		}

		/*
		protected void IndexParts(global::scms.search.search search, System.Collections.Generic.List<scms.modules.parts.classes.cat_part> lParts)
		{
			foreach (var part in lParts)
			{
				string strSearchText = string.Join(",",
					new string[] {
							part.sage_Description1,
							part.sage_Description2,
							part.sage_LongDescription });
				string strQueryString;
				string strTitleOverride;
				string strSummaryOverride;
				GetSearchOverrides(part, out strQueryString, out strTitleOverride, out strSummaryOverride);

				search.IndexModule(
					settings.searchResultsPageId.Value,
					settings.searchResultsPageModuleInstanceId,
					strSearchText,
					false,
					part.id,
					true,
					strQueryString,
					strTitleOverride,
					strSummaryOverride,
					part.imageUrl);
			}
		}
		*/

		public bool UnCompressPartsFile(string strPath, out string strPartsFileData, out string strError)
		{
			bool bSuccess = false;
			strPartsFileData = null;
			strError = null;

			return bSuccess;
		}



	}
}
