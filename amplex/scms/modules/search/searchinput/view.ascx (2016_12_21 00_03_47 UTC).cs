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

// note that this control is intended to be used both as a module and as a
// stand alone control, hence the settings accessed through properties
namespace scms.modules.search.search
{
    public partial class view : scms.RootControl
    {

        public int ScriptId
        {
            get { return (int)ViewState["ScriptId"]; }
            set { ViewState["ScriptId"] = value; }
        }

        public string CssClass
        {
            get { return (string)ViewState["CssClass"]; }
            set { ViewState["CssClass"] = value; }
        }

        public string CssClassInputActive
        {
            get { return (string)ViewState["CssClassInputActive"]; }
            set { ViewState["CssClassInputActive"] = value; }
        }

        public string CssClassInputInActive
        {
            get { return (string)ViewState["CssClassInputInActive"]; }
            set { ViewState["CssClassInputInActive"] = value; }
        }

        public string DefaultText
        {
            get { return (string)ViewState["DefaultText"]; }
            set { ViewState["DefaultText"] = value; }
        }

        public string ValidationErrorMessage
        {
            get { return (string)ViewState["ValidationErrorMessage"]; }
            set { ViewState["ValidationErrorMessage"] = value; }
        }

        public string CssClassButtonInput
        {
            get { return (string)ViewState["CssClassButtonInput"]; }
            set { ViewState["CssClassButtonInput"] = value; }
        }

        public string ButtonText
        {
            get { return (string)ViewState["ButtonText"]; }
            set { ViewState["ButtonText"] = value; }
        }

        public bool UseImageButton
        {
            get
            {
                bool? bUseImageButton = (bool?)ViewState["UseImageButton"];
                return bUseImageButton.HasValue && bUseImageButton.Value;
            }
            set { ViewState["UseImageButton"] = value; }
        }

        public string ButtonImagePath
        {
            get { return (string)ViewState["ButtonImagePath"]; }
            set { ViewState["ButtonImagePath"] = value; }
        }

        protected int? ResultsPageId
        {
            get { return (int?)ViewState["ResultsPageId"]; }
            set { ViewState["ResultsPageId"] = value; }
        }

        public view()
        {
            ScriptId = 0;
            CssClass = "search";
            CssClassInputActive = "search-text search-text-active";
            CssClassInputInActive = "search-text search-text-inactive";
            DefaultText = "Enter Keyword(s)";
            ValidationErrorMessage = "Please enter search keyword(s).";
            CssClassButtonInput = "search-button";
            ButtonText = "Search";
            UseImageButton = false;
            ButtonImagePath = "/sites/amplex/images/go.jpg";
            ResultsPageId = null;
        }

        protected void LoadModule()
        {
            if (ModuleInstanceId.HasValue)
            {
                ScriptId = ModuleInstanceId.Value;

                scms.data.ScmsDataContext dc = new scms.data.ScmsDataContext();
                var module = (from sim in dc.scms_search_input_modules
                              where sim.instanceId == ModuleInstanceId.Value
                              select sim).FirstOrDefault();
                if (module != null)
                {
                    CssClass = module.CssClass;
                    CssClassInputActive = module.CssClassTextInputActive;
                    CssClassInputInActive = module.CssClassTextInputInactive;
                    DefaultText = module.DefaultText;
                    ValidationErrorMessage = module.ValidationErrorMessage;
                    CssClassButtonInput = module.CssClassButtonInput;
                    ButtonText = module.ButtonText;
                    UseImageButton = module.UseImageButton;
                    ButtonImagePath = module.ButtonImagePath;
                    ResultsPageId = module.resultsPageId;
                }
            }
        }


        protected void Page_Init(object sender, EventArgs e)
        {
            LoadModule();
            PrepareScripts();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {

                divSearch.Attributes["class"] = CssClass;
                txtKeywords.Attributes["class"] = CssClassInputInActive;

                txtKeywords.Attributes["onfocus"] = string.Format("inputFocus_{0}(this);", ScriptId);
                txtKeywords.Attributes["onblur"] = string.Format("inputBlur_{0}(this);", ScriptId);
                txtKeywords.Attributes["onkeypress"] = string.Format("return searchIfEnter_{0}(event);", ScriptId);

                btnInput.Attributes["class"] = CssClassButtonInput;
                btnInput.Attributes["onclick"] = string.Format("searchClick_{0}(); return false;", ScriptId);


                string strButtonText = ButtonText;
                if (!string.IsNullOrEmpty(strButtonText))
                {
                    btnInput.Value = strButtonText;
                }

                if (UseImageButton)
                {
                    btnInput.Attributes["type"] = "image";
                    btnInput.Attributes["src"] = ButtonImagePath;
                }
            }
            else
            {
                string strEventArg = Request["__EVENTARGUMENT"];
                string strEventId = string.Format("scms_search_go_{0}", ScriptId);
                if (string.Compare(strEventArg, strEventId, true) == 0)
                {
                    search(txtKeywords.Value);
                }
            }
        }

        protected void PrepareScripts()
        {
            string strDefaultText = DefaultText;
            if (!string.IsNullOrEmpty(strDefaultText))
            {
                txtKeywords.Value = strDefaultText;
            }

            string strEventId = string.Format("scms_search_go_{0}", ScriptId);
            string strPostbackEventReference = Page.GetPostBackClientEvent(btnInput, strEventId);

            string strSearchClickScript = string.Format(@"
    function searchClick_{4}() {{
        var input = document.getElementById('{0}');
        if ((input.value == '') || (input.value == '{2}')) {{
            alert('{3}');
        }}
        else {{
            {1};
        }}
    }}

    function searchIfEnter_{4}(event) {{
        var keynum;
        if (window.event) {{
            keynum = event.keyCode;
        }}
        else if (event.which) {{
            keynum = event.which;
        }}

        if (keynum == 13) {{
            searchClick_{4}();
            return false;
        }}
        else {{
            return true;
        }}
    }}
", txtKeywords.ClientID, strPostbackEventReference, DefaultText, ValidationErrorMessage, ScriptId);
            string strScriptKey = string.Format("scms-search-searchClick-{0}", ScriptId);
            this.Page.ClientScript.RegisterClientScriptBlock(typeof(string), strScriptKey, strSearchClickScript, true);

            // btnInput.Attributes["onclick"] = "javascript: searchClick(); return false;";

            string strFocusScripts = string.Format(@"
    function inputFocus_{3}(input) {{
        input.setAttribute(""class"", ""{1}"");
        if (input.value == '{0}') {{
            input.value = '';
        }}
    }}

    function inputBlur_{3}(input) {{
        input.setAttribute(""class"", ""{2}"");
        if (input.value == '') {{
            input.value = '{0}';
        }}
    }}
", DefaultText, CssClassInputActive, CssClassInputInActive, ScriptId);

            string strFocusScriptKey = string.Format("scms-search-focusscripts-{0}", ScriptId);
            this.Page.ClientScript.RegisterClientScriptBlock(typeof(string), strFocusScriptKey, strFocusScripts, true);
        }

        protected void search(string strKeywords)
        {
            try
            {

                scms.data.ScmsDataContext dc = new scms.data.ScmsDataContext();

                int? nResultsPageId = ResultsPageId;
                if (!nResultsPageId.HasValue)
                {
                    var settings = (from s in dc.scms_search_settings
                                    select s).FirstOrDefault();
                    nResultsPageId = settings.searchResultsPageId;
                }
                if (nResultsPageId.HasValue)
                {
                    var page = (from p in dc.scms_pages
                                where p.id == nResultsPageId
                                select p).Single();
                    string strKeywordsEncoded = HttpUtility.UrlEncode(strKeywords);
                    string strResultsPath = string.Format("{0}?q={1}", page.url, strKeywordsEncoded);
                    Response.Redirect(strResultsPath, true);
                }
                else
                {
                    throw new Exception("Search results page not set, override not set");
                }
            }
            catch (System.Threading.ThreadAbortException)
            {
            }
            catch (Exception ex)
            {
            }
        }
    }


}