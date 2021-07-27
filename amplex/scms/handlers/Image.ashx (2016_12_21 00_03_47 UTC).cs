using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace scms.handlers
{
  // Image.ashx?src=/sites/amplex/images/badger.jpg&m=stretch&w=80&h=60

  /// <summary>
  /// Summary description for $codebehindclassname$
  /// </summary>
  public class Image : IHttpHandler
	{
    protected string strImagePath = null;
    protected int? nWidth = null;
    protected int? nHeight = null;
		protected int? nQuality = null;
    protected scms.ImageUtil.Mode mode = ImageUtil.Mode.stretch;
    protected int? nFillColor = null;
    protected byte[] abyImage = null;
    protected string strContentType = null;

    public void ProcessRequest(HttpContext context)
	  {
      HttpRequest request = context.Request;
      HttpResponse response = context.Response;

      bool bSuccess = false;
      GetParms(request);
      if (ValidateParms(context))
      {
        byte[] abyImage;
        string strContentType;
        scms.ImageUtil imageUtil = new ImageUtil();

        string strImageAbsoluePath = context.Server.MapPath(strImagePath);
        if (imageUtil.PrepareImage(strImageAbsoluePath, mode, nHeight, nWidth, nQuality, out abyImage, out strContentType))
        {
          response.ContentType = strContentType;
          response.BinaryWrite(abyImage);
          bSuccess = true;
        }
      }

      if (!bSuccess)
      {
				response.StatusCode = 500;
      }
		}


    bool DummyCallback()
    {
			return false;
    }

    protected void GetParms(HttpRequest request)
		{
      strImagePath = request.QueryString["src"];

      nWidth = null;
      string strWidth = request.QueryString["w"];
      if (!string.IsNullOrEmpty(strWidth))
      {
        int n;
        if (int.TryParse(strWidth, out n))
        {
					nWidth = n;
        }
        else
        {
          string strMessage = string.Format("Failed interpreting value '{0} for parm 'w' as int.", strWidth);
          ScmsEvent.Raise(strMessage, this, null);
        }
      }

      nHeight = null;
      string strHeight = request.QueryString["h"];
      if (!string.IsNullOrEmpty(strHeight))
      {
        int n;
        if (int.TryParse(strHeight, out n))
        {
					nHeight = n;
        }
        else
        {
          string strMessage = string.Format("Failed interpreting value '{0} for parm 'h' as int.", strHeight);
          ScmsEvent.Raise(strMessage, this, null);
        }
      }

			nQuality = null;
			string strQuality = request.QueryString["q"];
			if (!string.IsNullOrEmpty(strQuality))
			{
				int n;
				if (int.TryParse(strQuality, out n))
				{
					nQuality = n;
				}
				else
				{
					string strMessage = string.Format("Failed interpreting value '{0} for parm 'q' as int.", strQuality);
					ScmsEvent.Raise(strMessage, this, null);
				}
			}



      mode = ImageUtil.Mode.grow;
      string strMode = request.QueryString["m"];
      if (!string.IsNullOrEmpty(strMode))
      {
        try
        {
					mode = (ImageUtil.Mode)Enum.Parse(typeof(ImageUtil.Mode), strMode, true);
        }
        catch (Exception ex)
        {
          string strMessage = string.Format("Failed interpreting value '{0} for parm 'm' as Mode.", strMode);
          ScmsEvent.Raise(strMessage, this, ex);
        }
      }

      nFillColor = null;
      string strFillColor = request.QueryString["f"];
      if (!string.IsNullOrEmpty(strFillColor))
      {
        int n;
        if (int.TryParse(strFillColor, System.Globalization.NumberStyles.HexNumber, null, out n))
        {
					nFillColor = n;
        }
      }
		}

    protected bool ValidateParms(HttpContext context)
    {
      bool bAnyErrors = false;

      if (string.IsNullOrEmpty(strImagePath))
      {
        bAnyErrors = true;
        ScmsEvent.Raise("src parameter is missing", this, null);
      }

      if (!bAnyErrors)
      {
        switch (mode)
        {
					case ImageUtil.Mode.fill:
					case ImageUtil.Mode.crop:
						{
							if (!nWidth.HasValue)
							{
								bAnyErrors = true;
								string strMessage = string.Format("width is not set but mode is '{0}'", mode);
								ScmsEvent.Raise(strMessage, this, null);
							}

							if (!nHeight.HasValue)
							{
								bAnyErrors = true;
								string strMessage = string.Format("height is not set but mode is '{0}'", mode);
								ScmsEvent.Raise(strMessage, this, null);
							}
						}
						break;

					case ImageUtil.Mode.grow:
						{
							if (!nWidth.HasValue || !nHeight.HasValue)
							{
								bAnyErrors = true;
								ScmsEvent.Raise("either width or height missing but mode is grow", this, null);
							}
						}
						break;
        }
      }

      bool bSuccess = !bAnyErrors;

      return bSuccess;
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
