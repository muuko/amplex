using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SageExportLib
{
	public class SageExportProcessor
	{
		protected string strDataSourceName = null;
		protected string strUser = null;
		protected string strPassword = null;
		protected string strTableName = null;
		protected string strUrl = null;
		protected int? nTimeout = null;
		protected int? nMax = null;

		public System.IO.Stream streamUncompressed = null;
		public System.IO.Stream streamCompressed = null;
		public System.Text.StringBuilder sbErrors = new StringBuilder();


		public SageExportProcessor(string strDataSourceName, string strUser, string strPassword, string strTableName, string strUrl, int ? nTimeout, int ? nMax)
		{
			this.strDataSourceName = strDataSourceName;
			this.strUser = strUser;
			this.strPassword = strPassword;
			this.strTableName = strTableName;
			this.strUrl = strUrl;
			this.nTimeout = nTimeout;
			this.nMax = nMax;
		}
		
		
		
		public bool LoadData()
		{
			bool bSuccess = false;

			sbErrors.AppendLine("LoadData() begin");

			try
			{
				streamUncompressed = new System.IO.MemoryStream(0x100000);
				System.Text.StringBuilder sb = new StringBuilder();
				System.Xml.XmlWriterSettings settings = new System.Xml.XmlWriterSettings();
				settings.Encoding = Encoding.ASCII;
				settings.Indent = true;
				settings.OmitXmlDeclaration = false;
				System.Xml.XmlWriter xw = System.Xml.XmlWriter.Create(streamUncompressed, settings);
				xw.WriteStartElement("root");

				xw.WriteStartElement("ICParts");


				string strConnection = string.Format( "DSN={0};Uid={1};Pwd={2};", strDataSourceName, strUser, strPassword );

				sbErrors.AppendLine("Opening connection");
				using (System.Data.Odbc.OdbcConnection conn = new System.Data.Odbc.OdbcConnection(strConnection))
				{
					conn.Open();

					string strCommand = string.Format("SELECT * FROM {0}", strTableName);
					System.Data.Odbc.OdbcCommand cmd = new System.Data.Odbc.OdbcCommand(strCommand, conn);

					sbErrors.AppendLine("Executing parts reader");
					System.Data.Odbc.OdbcDataReader dr = cmd.ExecuteReader();

					System.Security.Cryptography.SHA256Managed sha256 = new System.Security.Cryptography.SHA256Managed();

					int nPart = 0;
					sbErrors.AppendLine("Reading");
					while (dr.Read())
					{
						
						string strSKICPart = dr["SKICPart"].ToString();

						sbErrors.AppendFormat("Loading part '{0}'", strSKICPart);

						string strID = dr["ID"].ToString();
						sbErrors.AppendFormat(", '{0}'\r\n", strID);

						string strDescription1 = dr["Description1"].ToString();
						string strDescription2 = dr["Description2"].ToString();
						string strDescription3 = dr["Description3"].ToString();
						string strLongDescription = dr["LongDescription"].ToString();
						string strBasePrice = dr["BasePrice"].ToString();
						string strProductCategoryDesc1 = dr["ProductCategoryDesc1"].ToString();
						string strProductCategoryDesc2 = dr["ProductCategoryDesc2"].ToString();
						string strProductCategoryDesc3 = dr["ProductCategoryDesc3"].ToString();
						string strProductCategoryDesc4 = dr["ProductCategoryDesc4"].ToString();
						string strProductCategoryDesc5 = dr["ProductCategoryDesc5"].ToString();


						if (!string.IsNullOrEmpty(strID))
						{
							System.Text.StringBuilder sbToBeHashed = new StringBuilder();
							sbToBeHashed.Append(strSKICPart);
							sbToBeHashed.Append(";");
							sbToBeHashed.Append(strID);
							sbToBeHashed.Append(";");
							sbToBeHashed.Append(strDescription1);
							sbToBeHashed.Append(";");
							sbToBeHashed.Append(strDescription2);
							sbToBeHashed.Append(";");
							sbToBeHashed.Append(strDescription3);
							sbToBeHashed.Append(";");
							sbToBeHashed.Append(strLongDescription);
							sbToBeHashed.Append(";");
							sbToBeHashed.Append(strBasePrice);
							sbToBeHashed.Append(";");
							sbToBeHashed.Append(strProductCategoryDesc1);
							sbToBeHashed.Append(";");
							sbToBeHashed.Append(strProductCategoryDesc2);
							sbToBeHashed.Append(";");
							sbToBeHashed.Append(strProductCategoryDesc3);
							sbToBeHashed.Append(";");
							sbToBeHashed.Append(strProductCategoryDesc4);
							sbToBeHashed.Append(";");
							sbToBeHashed.Append(strProductCategoryDesc5);
							sbToBeHashed.Append(";");

							string strToBeHashed = sbToBeHashed.ToString();
							System.Text.ASCIIEncoding encoding = new ASCIIEncoding();
							byte[] abyToBeHashed = encoding.GetBytes(strToBeHashed);
							byte[] abyResult = sha256.ComputeHash(abyToBeHashed);
							string strHash = Convert.ToBase64String(abyResult);

							xw.WriteStartElement("ICPart");
							xw.WriteAttributeString("SKICPart", strSKICPart);
							xw.WriteAttributeString("ID", strID);
							xw.WriteAttributeString("Hash", strHash);
							xw.WriteAttributeString("BasePrice", strBasePrice);
							xw.WriteElementString("Description1", strDescription1);
							xw.WriteElementString("Description2", strDescription2);
							xw.WriteElementString("Description3", strDescription3);
							xw.WriteElementString("ProductCategoryDesc1", strProductCategoryDesc1);
							xw.WriteElementString("ProductCategoryDesc2", strProductCategoryDesc2);
							xw.WriteElementString("ProductCategoryDesc3", strProductCategoryDesc3);
							xw.WriteElementString("ProductCategoryDesc4", strProductCategoryDesc4);
							xw.WriteElementString("ProductCategoryDesc5", strProductCategoryDesc5);
							xw.WriteStartElement("LongDescription");
							/* xw.WriteCData(strLongDescription); */
							xw.WriteEndElement();

							xw.WriteEndElement();
						}

						nPart++;
						if (nMax.HasValue)
						{
							if (nPart >= nMax.Value)
							{
								break;
							}
						}
					}
					xw.WriteEndElement(); // ICParts
					xw.WriteEndElement(); // root

					bSuccess = true;
				}
					
				xw.Close();

			}
			catch (Exception ex)
			{
				string strMessage = string.Format("Failed processing data '{0}'.", ex.ToString());
				sbErrors.AppendLine(strMessage);
			}

			sbErrors.AppendLine("LoadData() end");

			return bSuccess;
		}

		public bool CompressOutput()
		{
			bool bSuccess = false;
			try
			{
				byte[] abyData = new byte[streamUncompressed.Length];
				streamUncompressed.Seek(0, System.IO.SeekOrigin.Begin);
				streamUncompressed.Read(abyData, 0, abyData.Length);


				streamCompressed = new System.IO.MemoryStream();
				System.IO.Compression.DeflateStream deflateStream = new System.IO.Compression.DeflateStream(streamCompressed, System.IO.Compression.CompressionMode.Compress, true);
				deflateStream.Write(abyData, 0, (int)abyData.Length);
				deflateStream.Flush();
				deflateStream.Close();

				bSuccess = true;
			}
			catch (Exception ex)
			{
				string strMessage = string.Format("Failed compressing output stream '{0}'.", ex.ToString());
				sbErrors.AppendLine(strMessage);
			}

			return bSuccess;
		}

		public bool SendData(bool bCompressed)
		{
			bool bSuccess = false;

			try
			{
				// prep cat parts data
				System.IO.Stream streamCatPartsData = null;
				string strFileName = null;
				if (bCompressed)
				{
					streamCatPartsData = streamCompressed;
					strFileName = "cat-parts.xml.gz";
				}
				else
				{
					streamCatPartsData = streamUncompressed;
					strFileName = "cata-parts.xml";
				}

				// may to need fix this, ascii encoding may be unnecessary
				streamCatPartsData.Seek(0, System.IO.SeekOrigin.Begin);
				byte[] abyOutput = new byte[streamCatPartsData.Length];
				streamCatPartsData.Read(abyOutput, 0, (int)streamCatPartsData.Length);
				

				// prep request
				System.Net.HttpWebRequest request = (System.Net.HttpWebRequest)System.Net.HttpWebRequest.Create(strUrl);
				request.Method = "POST";
				request.AllowAutoRedirect = false;
				request.KeepAlive = false;
				if (nTimeout.HasValue)
				{
					request.Timeout = nTimeout.Value * 1000;
				}
				string boundary = "----------------------------" + DateTime.Now.Ticks.ToString("x");
				request.ContentType = "multipart/form-data; boundary=" + boundary;
				request.Credentials = System.Net.CredentialCache.DefaultCredentials;

				System.IO.MemoryStream msOutput = new System.IO.MemoryStream();
				byte[] abyBoundaryBytes = System.Text.Encoding.ASCII.GetBytes("\r\n--" + boundary);

				// prep header
				string headerTemplate = "\r\n--" + boundary + "\r\nContent-Disposition: form-data; name=\"{0}\"; filename=\"{1}\"\r\nContent-Type: application/xml\r\n\r\n";
				string header = string.Format(headerTemplate, "userfile", strFileName);
				byte[] abyHeaderBytes = System.Text.Encoding.ASCII.GetBytes(header);
				msOutput.Write(abyHeaderBytes, 0, abyHeaderBytes.Length);

				// prep file file
				msOutput.Write(abyOutput, 0, abyOutput.Length);
				msOutput.Write(abyBoundaryBytes, 0, abyBoundaryBytes.Length);

				// get to byte array
				msOutput.Position = 0;
				byte[] abyTempBuffer = new byte[msOutput.Length];
				msOutput.Read(abyTempBuffer, 0, abyTempBuffer.Length);
				msOutput.Close();

				request.ContentLength = abyTempBuffer.Length;

				// get request stream
				System.IO.Stream streamRequest = request.GetRequestStream();
				streamRequest.Write(abyTempBuffer, 0, abyTempBuffer.Length);
				streamRequest.Close();

				System.Net.HttpWebResponse response = (System.Net.HttpWebResponse)request.GetResponse();
				System.IO.Stream streamResponse = response.GetResponseStream();
				System.IO.StreamReader srRepsonse = new System.IO.StreamReader(streamResponse);
				string strResponse = srRepsonse.ReadToEnd();
				bSuccess = true;
				
			}
			catch (Exception ex)
			{
				sbErrors.AppendFormat("error occurred during send: '{0}'", ex.ToString());
			}

			return bSuccess;
		}






	}
}
