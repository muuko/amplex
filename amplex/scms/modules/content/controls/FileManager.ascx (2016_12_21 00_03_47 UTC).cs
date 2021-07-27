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

namespace scms.modules.content
{
	public partial class FileManager : System.Web.UI.UserControl
	{
		protected const string strDocumentImageUrl = "/scms/modules/content/images/document.jpg";
		protected string [] astrImageExtensions = new string [] { ".gif", ".jpg", ".bmp", ".png" };

		// protected string strFocusFile = null;
		protected string FocusFile
		{
			get
			{
				return (string)ViewState["FocusFile"];
			}
			set
			{
				ViewState["FocusFile"] = value;
			}
		}	

		public enum EMode
		{
			Manage,
			Select
		};

		protected EMode mode = EMode.Manage;
		public EMode Mode
		{
			get
			{
				mode = EMode.Manage;
				object objMode = ViewState["Mode"];
				if (objMode != null)
				{
					mode = (EMode)ViewState["Mode"];
				}
				return mode;
			}

			set
			{
				mode = value;
				ViewState["Mode"] = value;
			}
		}

		public enum ESelectType
		{
			Any,
			Image
		}

		protected ESelectType selectType = ESelectType.Image;
		public ESelectType SelectType
		{
			get
			{
				selectType = ESelectType.Image;
				object objSelectType = ViewState["SelectType"];
				if (objSelectType != null)
				{
					selectType = (ESelectType)objSelectType;
				}
				return selectType;
			}

			set
			{
				selectType = value;
				ViewState["SelectType"] = selectType;
			}
		}
		

		

		protected string strTarget = null;
		public string Target
		{
			get
			{
				strTarget = (string)ViewState["target"];
				return strTarget;
			}

			set
			{
				strTarget = value;
				ViewState["target"] = strTarget;
			}
		}

		protected string strOpenerScript = null;
		public string OpenerScript
		{
			get
			{
				strOpenerScript = (string)ViewState["OpenerScript"];
				return strOpenerScript;
			}

			set
			{
				strOpenerScript = value;
				ViewState["OpenerScript"] = strOpenerScript;
			}
		}


		protected string filesLocation = null;
		public string FilesLocation
		{
			get
			{
				filesLocation = (string)ViewState["FilesLocation"];
				return filesLocation;
			}

			set
			{
				filesLocation = value;
				ViewState["FilesLocation"] = value;
			}
		}


		protected bool showSystemFolders = false;
		public bool ShowSystemFolders
		{
			get
			{
				showSystemFolders = false;
				object obj = ViewState["ShowSystemFolders"];
				if (obj != null)
				{
					showSystemFolders = (bool)obj;
					return showSystemFolders;
				}
				return showSystemFolders;
			}

			set
			{
				showSystemFolders = value;
				ViewState["ShowSystemFolders"] = showSystemFolders;
			}
		}

		protected string preselect = null;
		public string Preselect
		{
			get
			{
				preselect = (string)ViewState["Preselect"];
				return preselect;
			}

			set
			{
				preselect = value;
				ViewState["Preselect"] = preselect;
			}
		}

		protected string preselectUrl = null;
		public string PreselectUrl
		{
			get
			{
				preselectUrl = (string)ViewState["PreselectUrl"];
				return preselectUrl;
			}

			set
			{
				preselectUrl = value;
				ViewState["PreselectUrl"] = preselectUrl;
			}
		}


		protected void Page_Load(object sender, EventArgs e)
		{
			if (!IsPostBack)
			{
				Target = Request.QueryString["target"];
				OpenerScript = Request.QueryString["openerscript"];
				checkShowSystemFolders.Checked = ShowSystemFolders;
				
				// if preselected by url, use that
				if (!string.IsNullOrEmpty(PreselectUrl))
				{
					try
					{
						Preselect = Server.MapPath(preselectUrl);
					}
					catch
					{
					}
				}

				LoadControls();
				PreselectFile(Preselect);

				

			}
		}

		protected void LoadControls()
		{
			try
			{
				treeViewFolders.Nodes.Clear();

				// get root files directory for this site

				string strPath = Server.MapPath(FilesLocation);
				string strDirectory = System.IO.Path.GetFileName(strPath);
				string strFolder = string.Empty;
				string[] astrFragments = strPath.Split(new char[] { '\\', '/' }, StringSplitOptions.RemoveEmptyEntries);
				if (astrFragments.Length > 0)
				{
					strFolder = astrFragments[astrFragments.Length - 1];
				}

				TreeNode treeNode = new TreeNode(strFolder, strPath);
				treeNode.PopulateOnDemand = true;
				treeNode.SelectAction = TreeNodeSelectAction.Select;
				treeViewFolders.Nodes.Add(treeNode);


				// Load();

				


			}
			catch (Exception ex)
			{
				string strMessage = string.Format("Exception thrown while loading folder/subfolders of '{0}': {1}.", FilesLocation, ex.ToString());
				statusMessage.ShowFailure(strMessage);
			}
		}

		protected void LoadFiles()
		{
			if (treeViewFolders.SelectedNode != null)
			{
				string strFolder = treeViewFolders.SelectedNode.Value;

				rptFiles.DataSource = null;
				try
				{
					string strSearchPattern = "*";
					string [] astrFiles = System.IO.Directory.GetFiles(strFolder, strSearchPattern, System.IO.SearchOption.TopDirectoryOnly);
					rptFiles.DataSource = astrFiles;
				}
				catch (Exception ex)
				{
                    string strMessage = string.Format("Failed loading files for folder '{0}'.", strFolder);
                    ScmsEvent.Raise(strMessage, this, ex);
					statusMessage.ShowFailure(strMessage);
				}

				mode = Mode;
				rptFiles.DataBind();
			}
		}



		protected void treeViewFolders_SelectedNodeChanged(object sender, EventArgs args)
		{
			FocusFile = null;
			ddlFolderAction_SelectedIndexChanged(null, null);
			LoadFiles();
			ddlFileAction.SelectedValue = "select";
			ddlFileAction_SelectedIndexChanged(null, null);
		}

		protected void treeViewFolders_TreeNodePopulate(object sender, System.Web.UI.WebControls.TreeNodeEventArgs args)
		{
			try
			{
				if (args.Node != null)
				{
					args.Node.ChildNodes.Clear();

					string strPath = args.Node.Value;
					string[] astrDirectories = System.IO.Directory.GetDirectories(strPath, "*", System.IO.SearchOption.TopDirectoryOnly);

					showSystemFolders = ShowSystemFolders;
					foreach (string strDirectory in astrDirectories)
					{
						bool bInclude = false;

						System.IO.DirectoryInfo directoryInfo = new System.IO.DirectoryInfo(strDirectory);
						string strFolder = directoryInfo.Name;
						if (showSystemFolders)
						{
							bInclude = true;
						}
						else
						{
							if (!strFolder.StartsWith("_"))
							{
								bInclude = true;
							}
						}

						if (bInclude)
						{
							TreeNode treeNode = new TreeNode(strFolder, strDirectory);
							treeNode.PopulateOnDemand = true;
							treeNode.SelectAction = TreeNodeSelectAction.Select;
							args.Node.ChildNodes.Add(treeNode);
						}
					}
				}
			}
			catch (Exception ex)
			{
				string strMessage = string.Format("Failed populating node, {0}.", ex.ToString());
				statusMessage.ShowFailure(strMessage);
			}
		}

		protected void btnSelect_Click(object sender, EventArgs args)
		{
			string strImageUrl = "http://www.coinbug.com/images/logo.gif";

			string strScriptFormat = @"
function SelectAndClose()
{{

 opener.{1} = '{0}';
 
 {2};

 close();
}}
SelectAndClose();";

			// opener.ImageDialog.showPreviewImage('{0}');

			string strScript = string.Format(strScriptFormat, strImageUrl, Target, OpenerScript);

			Page.ClientScript.RegisterClientScriptBlock(typeof(string), "SelectAndClose", strScript, true);
			// Page.RegisterStartupScript("SelectAndClose", strScript);
		}

		protected void ddlFolderAction_SelectedIndexChanged(object sender, EventArgs args)
		{
			bool bEnableText = false;
			string strText = string.Empty;
			string strScript = string.Empty;

			switch (ddlFolderAction.SelectedValue.ToLower())
			{
				case "new":
					bEnableText = true;
					break;

				case "rename":
					bEnableText = true;
					strText = treeViewFolders.SelectedNode.Text;
					break;

				case "delete":
					strScript = string.Format( "javascript: return confirm( \"delete '{0}' and all subfolders?\" ); ", treeViewFolders.SelectedNode.Text );
					strText = treeViewFolders.SelectedNode.Text;
					
					break;
			}

			txtNewFolder.Text = strText;
			txtNewFolder.Enabled = bEnableText;
			btnFolderAction.Attributes.Add("onclick", strScript);
		}

		protected void btnFolderAction_Click(object sender, EventArgs args)
		{
			switch (ddlFolderAction.SelectedValue.ToLower())
			{
				case "new":
					{
						string strNewFolderName = txtNewFolder.Text.Trim();

						if (!string.IsNullOrEmpty(strNewFolderName))
						{
							string strParentFolder = treeViewFolders.SelectedNode.Value;
							string strPath = System.IO.Path.Combine(strParentFolder, strNewFolderName);

							try
							{
								if (System.IO.Directory.Exists(strPath))
								{
									string strMessage = string.Format("Folder '{0}' already exists.", strNewFolderName);
									statusMessage.ShowFailure(strMessage);
								}
								else
								{
									System.IO.Directory.CreateDirectory(strPath);
									LoadControls();
									PreselectFile(strPath);
								}
							}
							catch (Exception ex)
							{
								string strMessage = string.Format("Failed creating folder '{0}'.", strNewFolderName);
								statusMessage.ShowFailure(strMessage);
                                ScmsEvent.Raise(strMessage, this, ex);
							}
						}
					}
					break;

				case "rename":
					{
						string strNewFolderName = txtNewFolder.Text.Trim();

						if (!string.IsNullOrEmpty(strNewFolderName))
						{
							TreeNode parentNode = treeViewFolders.SelectedNode.Parent;
							if (parentNode != null)
							{
								string strParentFolder = treeViewFolders.SelectedNode.Parent.Value;
								string strPath = System.IO.Path.Combine(strParentFolder, strNewFolderName);

								try
								{
									if (System.IO.Directory.Exists(strPath))
									{
										string strMessage = string.Format("Folder '{0}' already exists.", strNewFolderName);
										statusMessage.ShowFailure(strMessage);
									}
									else
									{
										string strOriginalPath = treeViewFolders.SelectedNode.Value;

										System.IO.Directory.Move(strOriginalPath, strPath);

										treeViewFolders.SelectedNode.Value = strPath;
										treeViewFolders.SelectedNode.Text = strNewFolderName;
										// LoadControls();
									}
								}
								catch (Exception ex)
								{
									string strMessage = string.Format("Failed creating folder '{0}'.", strNewFolderName);
									statusMessage.ShowFailure(strMessage);
                                    ScmsEvent.Raise(strMessage, this, ex);
								}
							}
						}
					}
					break;

				case "delete":
					{
						TreeNode parentNode = treeViewFolders.SelectedNode.Parent;
						if (parentNode != null)
						{
							string strPath = treeViewFolders.SelectedNode.Value;

							try
							{
								if (System.IO.Directory.Exists(strPath))
								{
									System.IO.Directory.Delete(strPath, true);
								}

								LoadControls();
								PreselectFile(parentNode.Value);

								FocusFile = null;
								LoadFiles();
								ddlFileAction.SelectedValue = "select";
								ddlFileAction_SelectedIndexChanged(null, null);
							}
							catch (Exception ex)
							{
								string strMessage = string.Format("Failed deleting folder '{0}'.", strPath);
								statusMessage.ShowFailure(strMessage);
                                ScmsEvent.Raise(strMessage, this, ex);
							}
						}
					}
					break;
			}
		}

		protected void checkShowSystemFolders_CheckedChanged(object sender, EventArgs args)
		{
			ShowSystemFolders = checkShowSystemFolders.Checked;
			LoadControls();
			PreselectFile(null);
			
		}

		protected bool PreselectFolder(TreeNode node, string strFolder)
		{
			bool bSelected = false;

			string strNodePath = node.Value;
			if (string.Compare(strNodePath, 0, strFolder, 0, strNodePath.Length, true) == 0)
			{
				if (strNodePath.Length == strFolder.Length)
				{
					node.Select();
					bSelected = true;
				}
				else
				{
					if (!(node.Expanded.HasValue && node.Expanded.Value))
					{
						node.Expand();
					}

					foreach (TreeNode childNode in node.ChildNodes)
					{
						if (PreselectFolder(childNode, strFolder))
						{
							bSelected = true;
							break;
						}
					}
				}
			}

			return bSelected;
		}

		protected void PreselectFile(string strPath)
		{
			string strFolder = System.IO.Path.GetDirectoryName(strPath);
			string strFile = System.IO.Path.GetFileName(strPath);

			if (!string.IsNullOrEmpty(strFile))
			{
				if (!strFile.Contains('.'))
				{
					strFolder = strPath;
					strFile = null;
				}
			}

			if (!string.IsNullOrEmpty(strFolder))
			{
				if (treeViewFolders.Nodes.Count > 0)
				{
					PreselectFolder(treeViewFolders.Nodes[0], strFolder);
				}
			}
			else
			{
				treeViewFolders.Nodes[0].Select();
			}

			FocusFile = strPath;
			treeViewFolders_SelectedNodeChanged(null, null);
		}

		protected void btnUpload_Click(object sender, EventArgs args)
		{
			Page.Validate("fileUpload");
			if (Page.IsValid)
			{
				bool bSuccess = false;
				string strPath = null;

				try
				{
					string strFolder = treeViewFolders.SelectedNode.Value;

					bool bUniqueNameFound = false;
					int nTry = 1;
					while (!bUniqueNameFound)
					{
						string strFileName = fileUpload.FileName;
						if (nTry > 1)
						{
							string strBaseFileName = string.Format("{0}-{1}", System.IO.Path.GetFileNameWithoutExtension(fileUpload.FileName), nTry);
							strFileName = string.Format("{0}{1}", strBaseFileName, System.IO.Path.GetExtension(fileUpload.FileName));
						}

						strPath = System.IO.Path.Combine(strFolder, strFileName);
						if (!System.IO.File.Exists(strPath))
						{
							bUniqueNameFound = true;
						}
						else
						{
							nTry++;
							if (nTry > 10000)
							{
								throw new Exception("no unique path found");
							}
						}
					}

					fileUpload.SaveAs(strPath);

					bSuccess = true;
				}
				catch (Exception ex)
				{
                    string strMessage = "Failed saving file";
					statusMessage.ShowFailure(strMessage);
                    ScmsEvent.Raise(strMessage, this, ex);
				}

				if (bSuccess)
				{
					FocusFile = strPath;
					LoadFiles();
					ddlFileAction.SelectedValue = "select";
					ddlFileAction_SelectedIndexChanged(null, null);
				}
			}
		}

		protected void rptFiles_ItemDataBound(object sender, RepeaterItemEventArgs args)
		{
			bool bShowFile = false;
			bool bImage = false;
			

			string strPath = (string)args.Item.DataItem;
			string strFileName = System.IO.Path.GetFileName(strPath);
			string strExension = System.IO.Path.GetExtension(strPath);
			foreach (string strImageExtension in astrImageExtensions)
			{
				if (string.Compare(strExension, strImageExtension, true) == 0)
				{
					bImage = true;
					break;
				}
			}

			if (mode == EMode.Select)
			{
				switch( SelectType )
				{
					case ESelectType.Image:
						bShowFile = bImage;
						break;

					case ESelectType.Any:
						bShowFile = true;
						break;
				}
			}
			else
			{
				bShowFile = true;
			}

			HtmlGenericControl divFile = (HtmlGenericControl)args.Item.FindControl("divFile");
			if (!bShowFile)
			{
				divFile.Visible = false;
			}
			else
			{
				HtmlAnchor anchorFile = (HtmlAnchor)args.Item.FindControl("anchorFile");


				ImageButton ibImageFile = (ImageButton)args.Item.FindControl("ibImageFile");
				/*
				HtmlImage image = (HtmlImage)args.Item.FindControl("imageFile");
				 * image.Src = strUrl;
				image.Width = 80;
				*/

				string strUrl = null;
				if (bImage)
				{
					string strFilesLocation = FilesLocation;
					string strRootPath = Server.MapPath(strFilesLocation);
					UrlFromPath(strRootPath, strFilesLocation, strPath, out strUrl);
				}
				else
				{
					strUrl = strDocumentImageUrl;
				}
                string strThumbnailUrl = string.Format("/Image.ashx?src={0}&m=stretch&w=80&h=80", HttpUtility.UrlEncode(strUrl));
				ibImageFile.ImageUrl = strThumbnailUrl;
				ibImageFile.Width = 80;
				ibImageFile.Attributes["title"] = strFileName;

				
				HtmlGenericControl spanFileName = (HtmlGenericControl)args.Item.FindControl("spanFileName");
				spanFileName.Attributes["title"] = strFileName;

				int nMaxFileName = 15;
				if (strFileName.Length > nMaxFileName)
				{
					spanFileName.InnerText = string.Format("{0}..", strFileName.Substring(0, 13));
				}
				else
				{
					spanFileName.InnerText = strFileName;
				}

				const string strInactiveClass = "file-manager-file file-manager-file-inactive";
				const string strActiveClass = "file-manager-file file-manager-file-active";
				bool bActive = false;
				if (string.IsNullOrEmpty(FocusFile))
				{
					bActive = true;
					FocusFile = strPath;
				}
				else
				{
					if (string.Compare(FocusFile, strPath, true) == 0)
					{
						bActive = true;
					}
				}
				divFile.Attributes["class"] = bActive ? strActiveClass : strInactiveClass;
			}

		}

		protected void UrlFromPath(string strRootPath, string strRootUrl, string strPath, out string strUrl)
		{
			strUrl = null;
			if (strPath.StartsWith(strRootPath, StringComparison.InvariantCultureIgnoreCase))
			{
				string strRemainder = strPath.Substring(strRootPath.Length);
				strUrl = string.Concat(strRootUrl, strRemainder.Replace('\\', '/'));
			}
		}

		protected void btnFileAction_Click(object sender, EventArgs args)
		{
			string strPath = FocusFile;
			switch( ddlFileAction.SelectedValue )
			{
				case "select":
					SelectAndClose(strPath);
					break;

				case "download":
					DownloadFile(strPath);
					break;

				case "delete":
					try
					{
						System.IO.File.Delete(strPath);
						FocusFile = null;
						LoadFiles();
						ddlFileAction.SelectedValue = "select";
						ddlFileAction_SelectedIndexChanged(null, null);
					}
					catch( Exception ex )
					{
						string strMessage = string.Format("Failed deleting '{0}'", strPath);
						statusMessage.ShowFailure(strMessage);
                        ScmsEvent.Raise(strMessage, this, ex);
					}
					break;

				case "rename":
					try
					{
						string strFileName = System.IO.Path.GetFileName(strPath);
						string strFolder = System.IO.Path.GetDirectoryName(strPath);
						string strNewFileName = txtFileAction.Text.Trim();

						string strNewPath = System.IO.Path.Combine(strFolder, strNewFileName);
						if (System.IO.File.Exists(strNewPath))
						{
							statusMessage.ShowFailure(string.Format("{0} already exists", strNewFileName));
						}
						else
						{
							System.IO.File.Move(strPath, strNewPath);
							FocusFile = strNewPath;
							LoadFiles();
							ddlFileAction.SelectedValue = "select";
							ddlFileAction_SelectedIndexChanged(null, null);
						}
					}
					catch (Exception ex)
					{
						string strMessage = string.Format("Failed renaming '{0}'", strPath);
						statusMessage.ShowFailure(strMessage);
                        ScmsEvent.Raise(strMessage, this, ex);
					}

					break;
			}
		}

		protected void SelectAndClose(string strPath)
		{
			string strImageUrl;
			string strFilesLocation = FilesLocation;
			string strRootPath = Server.MapPath(strFilesLocation);
			UrlFromPath(strRootPath, strFilesLocation, strPath, out strImageUrl);

			string strScriptFormat = @"
function SelectAndClose()
{{

opener.{1} = '{0}';


 {2};

 close();

}}
SelectAndClose();";

			// opener.ImageDialog.showPreviewImage('{0}');

			string strScript = string.Format(strScriptFormat, strImageUrl, Target, OpenerScript);

			Page.ClientScript.RegisterClientScriptBlock(typeof(string), "SelectAndClose", strScript, true);
			// Page.RegisterStartupScript("SelectAndClose", strScript);
		}

		protected void DownloadFile(string strPath)
		{
			string strScript = string.Format("window.open('/scms/modules/content/DownloadFile.aspx?file={0}');", HttpUtility.UrlEncode(strPath));
			Page.ClientScript.RegisterClientScriptBlock(typeof(string), "Download", strScript, true);
		}


		protected void ibImageFile_Command(object sender, CommandEventArgs args)
		{
			switch (args.CommandName)
			{
				case "focus":
					FocusFile = (string)args.CommandArgument;
					LoadFiles();
					ddlFileAction.SelectedValue = "select";
					ddlFileAction_SelectedIndexChanged(null, null);
					break;
			}
		}

		protected void ddlFileAction_SelectedIndexChanged(object sender, EventArgs args)
		{
			bool bEnableDropDown = false;
			bool bEnableText = false;
            bool bShowDownloadLink = false;
			string strPath = FocusFile;
			string strText = null;
			string strScript = null;


			if (!string.IsNullOrEmpty(strPath))
			{
				bEnableDropDown = true;
				strText = System.IO.Path.GetFileName(strPath);

				switch (ddlFileAction.SelectedValue.ToLower())
				{
					case "select":
						break;

					case "rename":
						bEnableText = true;
						break;

					case "delete":
						strScript = string.Format("javascript: return confirm( \"delete '{0}'?\" ); ", strText);
						break;

                    case "download":
                        bShowDownloadLink = true;
                        break;
				}
			}

			ddlFileAction.Enabled = bEnableDropDown;
			txtFileAction.Text = strText;
			txtFileAction.Enabled = bEnableText;
			btnFileAction.Attributes.Add("onclick", strScript);

            if (bShowDownloadLink)
            {
                string strUrl = string.Format("/scms/modules/content/DownloadFile.aspx?file={0}", FocusFile);
                anchorDownload.HRef = strUrl;
            }
            anchorDownload.Visible = bShowDownloadLink;
            btnFileAction.Visible = !bShowDownloadLink;
		}


	}
}