using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace cmdSageExport
{
	class Program
	{
		static void Main(string[] args)
		{
			Console.Write("{0} start\r\n", DateTime.Now.ToLongTimeString());

			Program program = new Program(args);

			bool bShowHelp = false;

			program.Run(out bShowHelp);
			if (bShowHelp)
			{
				Console.WriteLine();
				Console.WriteLine("usage:");
				Console.WriteLine("cmdSageExport.exe -dsn=AMPLEX_ODBC -user=MANAGER -password= -compress=true -log=true -parts=\"ICPart\" -url=\"http://localhost/scms/modules/parts/classes/Parts.ashx\" -timeout=300 -max=10000");
				Console.WriteLine();
			}

			Console.Write("{0} end\r\n", DateTime.Now.ToLongTimeString());

			if (bShowHelp || program.dictArgs.ContainsKey("pause"))
			{
				Console.ReadKey();
			}

		}

		public System.Collections.Generic.Dictionary<string, string> dictArgs = new Dictionary<string, string>();
		

		public Program(string [] args)
		{
			foreach (var strArg in args)
			{
				string[] astrNameValue = strArg.Split(new char[] { '=' }, StringSplitOptions.RemoveEmptyEntries);
				if (astrNameValue.Length > 0)
				{
					string strName = astrNameValue[0].Trim().ToLower();
					strName = strName.TrimStart(new char[] { '-', '/' });
					if (!string.IsNullOrEmpty(strName))
					{
						string strValue = string.Empty;
						if (astrNameValue.Length > 1)
						{
							strValue = astrNameValue[1].Trim();
							dictArgs[strName] = strValue;
						}
					}
				}
			}
		}

		public bool Run(out bool bShowHelp)
		{
			bool bSuccess = false;


			bShowHelp = false;

			string strDsn = null;
			string strUser = null;
			string strPassword = null;
			bool bCompress = false;
			
			bool bLog = false;
			string strParts = null;
			string strUrl = null;
			int? nTimeout = null;
			int? nMax = null;

			if (dictArgs.ContainsKey("?"))
			{
				bShowHelp = true;
			}
			else
			{
				dictArgs.TryGetValue("dsn", out strDsn);
				dictArgs.TryGetValue("user", out strUser);
				dictArgs.TryGetValue("password", out strPassword);

				
				string strCompress = null;
				if (dictArgs.TryGetValue("compress", out strCompress))
				{
					bCompress = false;
					bool.TryParse(strCompress, out bCompress);
				}

				string strLog = null;
				if (dictArgs.TryGetValue("log", out strLog))
				{
					bLog = false;
					bool.TryParse(strLog, out bLog);
				}

				dictArgs.TryGetValue("parts", out strParts);
				if (!string.IsNullOrEmpty(strParts))
				{
					strParts = strParts.Trim(new char[] { '\'', '\"' });
				}

				dictArgs.TryGetValue("url", out strUrl);
				if (!string.IsNullOrEmpty(strUrl))
				{
					strUrl = strUrl.Trim(new char[] { '\'', '\"' });
				}

				string strTimeout = null;
				dictArgs.TryGetValue("timeout", out strTimeout);
				if( !string.IsNullOrEmpty(strTimeout))
				{
					int n;
					if (int.TryParse(strTimeout, out n))
					{
						nTimeout = n;
					}
				}

				string strMax = null;
				dictArgs.TryGetValue("max", out strMax);
				if (!string.IsNullOrEmpty(strMax))
				{
					int n;
					if (int.TryParse(strMax, out n))
					{
						nMax = n;
					}
				}

				Console.WriteLine("cmdSageExport");
				Console.WriteLine("dsn: {0}", strDsn);
				Console.WriteLine("user: {0}", strUser);
				Console.WriteLine("password: {0}", strPassword);
				Console.WriteLine("compress: {0}", bCompress);
				Console.WriteLine("log: {0}", bLog);
				Console.WriteLine("parts: {0}", strParts);
				Console.WriteLine("url: {0}", strUrl);
				Console.WriteLine("timeout: {0}", nTimeout);
				Console.WriteLine("max: {0}", nMax);


				if (string.IsNullOrEmpty(strDsn))
				{
					Console.WriteLine("Error: 'dsn' is missing");
					bShowHelp = true;
				}
				if (string.IsNullOrEmpty(strUser))
				{
					Console.Write("Warning: 'user' is missing");
				}
				if (string.IsNullOrEmpty(strPassword))
				{
					Console.WriteLine("Warning: 'password' is missing");
				}
				if (string.IsNullOrEmpty(strParts))
				{
					Console.WriteLine("error: 'parts' is missing");
					bShowHelp = true;
				}
				if (string.IsNullOrEmpty(strUrl))
				{
					Console.WriteLine("warning: 'url' is missing, data will not be uploaded to website");
				}
				if (!nTimeout.HasValue)
				{
					Console.WriteLine("warning: 'timeout' is missing, using default");
				}
				if (nMax.HasValue)
				{
					Console.WriteLine("warning: 'max' has been set and will limit output");
				}
			}

			if (!bShowHelp)
			{
				/* SageExportLib.SageExportProcessor processor = new SageExportLib.SageExportProcessor( "SageTest", "[ICPart$]", "[ICPartPrice$]", "http://localhost/scms/modules/parts/classes/Parts.ashx");*/
				SageExportLib.SageExportProcessor processor = new SageExportLib.SageExportProcessor(
					strDsn,
					strUser,
					strPassword,
					strParts,
					strUrl,
					nTimeout,
					nMax);

				bool bAnyErrors = false;
				if (!bAnyErrors)
				{
					if (processor.LoadData())
					{
						if (bLog)
						{
							Console.WriteLine("dumping xml output");
							processor.streamUncompressed.Seek(0, System.IO.SeekOrigin.Begin);
							byte[] abyOutput = new byte[processor.streamUncompressed.Length];
							processor.streamUncompressed.Read(abyOutput, 0, (int)processor.streamUncompressed.Length);
							System.IO.File.WriteAllBytes(@"parts-dump.xml", abyOutput);
						}
					}
					else
					{
						bAnyErrors = true;
					}
				}

				if (!bAnyErrors)
				{
					if (bCompress)
					{
						if (processor.CompressOutput())
						{
							Console.WriteLine("dumping xml output compressed");
							processor.streamCompressed.Seek(0, System.IO.SeekOrigin.Begin);
							byte[] abyOutputCompressed = new byte[processor.streamCompressed.Length];
							processor.streamCompressed.Read(abyOutputCompressed, 0, (int)processor.streamCompressed.Length);
							System.IO.File.WriteAllBytes(@"parts-dump.xml.gz", abyOutputCompressed);
						}
						else
						{
							bAnyErrors = true;
						}
					}
				}

				if (!bAnyErrors)
				{
					if (!string.IsNullOrEmpty(strUrl))
					{
						if (processor.SendData(true))
						{
							Console.WriteLine("send ok");
						}
						else
						{
							Console.WriteLine("send failed");
						}
					}
				}

				if (bAnyErrors)
				{
					Console.WriteLine(processor.sbErrors.ToString());
				}
			}

			return bSuccess;
		}
	}
}
