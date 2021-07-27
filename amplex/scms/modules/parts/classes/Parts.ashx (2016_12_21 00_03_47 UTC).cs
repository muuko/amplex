using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;

namespace amplex.scms.modules.parts.classes
{
	/// <summary>
	/// Summary description for $codebehindclassname$
	/// </summary>
	[WebService(Namespace = "http://tempuri.org/")]
	[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
	public class Parts : IHttpHandler
	{

		public void ProcessRequest(HttpContext context)
		{
			context.Response.ContentType = "text/plain";

			bool bSuccess = false;
			string strError = null;
			if((context.Request.Files != null) && (context.Request.Files.Count == 1) )
			{
				HttpPostedFile file = context.Request.Files[0];
				if (file.ContentLength > 0)
				{
					bool bCompressed = false;
					string strFileName = file.FileName;
					if (!string.IsNullOrEmpty(strFileName))
					{
						if (strFileName.EndsWith(".gz", StringComparison.InvariantCultureIgnoreCase))
						{
							bCompressed = true;
						}
					}

					/*
					string strX = context.Server.MapPath("~/sites/amplex/files/parts/parts-dump-read.gz");
					System.IO.FileStream fs = System.IO.File.Create(strX);
					int nNumBytesReadx = 0;
					do
					{
						byte [] abyData = new byte[4096];
						nNumBytesReadx = file.InputStream.Read(abyData,0, abyData.Length);
						if( nNumBytesReadx > 0 )
						{
							fs.Write( abyData, 0, nNumBytesReadx);
						}
					} while (nNumBytesReadx > 0 );
					fs.Close();
					*/
					
					file.InputStream.Seek(0, System.IO.SeekOrigin.Begin);


					PartsLoader partsLoader = new PartsLoader();
					if (partsLoader.Init())
					{
						if (partsLoader.LoadParts(file.InputStream, bCompressed))
						{
							System.Threading.ThreadStart ts = new System.Threading.ThreadStart(partsLoader.ProcessParts);
							System.Threading.Thread threadLoader = new System.Threading.Thread(ts);
							threadLoader.Start();
							bSuccess = true;
						}
						else
						{
							strError = "parts loader failed loading parts file";
						}
					}
					else
					{
						strError = "failed initializing parts loader";
					}
				}
				else
				{
					strError = "parts file is zero length";
				}
			}
			else
			{
				context.Response.Write("error: no or multiple files");
			}

			if (bSuccess)
			{
				context.Response.Write("ok");
				context.Response.StatusCode = 200;
			}
			else
			{
				context.Response.Write("error: ");
				context.Response.Write(strError);
				context.Response.StatusCode = 400;
			}
		}


		public bool IsReusable
		{
			get
			{
				return false;
			}
		}
	}
}
