using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace scms.modules.submission.controls
{
    public partial class voting : System.Web.UI.UserControl
    {
        public delegate void UpDownVotingEvent(int? nId, bool bUp);
        public UpDownVotingEvent OnUpDownVote = null;

        public delegate void FiveUpVotingEvent(int? nId, int nIndex);
        public FiveUpVotingEvent OnFiveUpVote = null;

        public int? Id
        {
            get { return (int?)ViewState["Id"]; }
            set { ViewState["Id"] = value; }
        }

        public enum EMode
        {
            UpDown,
            FiveUp
        }

        public EMode Mode
        {
            get
            {
                EMode mode = EMode.UpDown;

                object objMode = ViewState["Mode"];
                if (objMode != null)
                {
                    mode = (EMode)objMode;
                }
                return mode;
            }

            set
            {
                ViewState["Mode"] = value;
            }
        }


        // settings for UpDown
        public string UpImagePath
        {
            get
            {
                return (string)ViewState["UpImagePath"];
            }
            set
            {
                ViewState["UpImagePath"] = value;
            }
        }

        public string DownImagePath
        {
            get
            {
                return (string)ViewState["DownImagePath"];
            }
            set
            {
                ViewState["DownImagePath"] = value;
            }
        }

        public string UpText
        {
            get
            {
                return (string)ViewState["UpText"];
            }
            set
            {
                ViewState["UpText"] = value;
            }
        }

        public string DownText
        {
            get
            {
                return (string)ViewState["DownText"];
            }
            set
            {
                ViewState["DownText"] = value;
            }
        }

        public int VotesUp
        {
            get
            {
                int nVotesUp = 0;
                object objVotesUp = ViewState["VotesUp"];
                if (objVotesUp != null)
                {
                    nVotesUp = (int)objVotesUp;
                }

                return nVotesUp;
            }

            set
            {
                ViewState["VotesUp"] = value;
            }
        }

        public int VotesDown
        {
            get
            {
                int nVotesDown = 0;
                object objVotesDown = ViewState["VotesDown"];
                if (objVotesDown != null)
                {
                    nVotesDown = (int)objVotesDown;
                }

                return nVotesDown;
            }

            set
            {
                ViewState["VotesDown"] = value;
            }
        }


        public int NumberOfVotes
        {
            get
            {
                int nNumberOfVotes = 0;
                object objNumberOfVotes = ViewState["NumberOfVotes"];
                if (objNumberOfVotes != null)
                {
                    nNumberOfVotes = (int)objNumberOfVotes;
                }

                return NumberOfVotes;
            }

            set { ViewState["NumberOfVotes"] = value; }
        }


        // settings for FiveUp
        public decimal? FiveUp_Vote
        {
            get
            {
                return (decimal?)ViewState["FiveUp_Vote"];
            }
            set
            {
                ViewState["FiveUp_Vote"] = value;
            }
        }

        public string FiveUp_ActiveImagePath
        {
            get
            {
                return (string)ViewState["FiveUp_ActiveImagePath"];
            }
            set
            {
                ViewState["FiveUp_ActiveImagePath"] = value;
            }
        }

        public string FiveUp_InActiveImagePath
        {
            get
            {
                return (string)ViewState["FiveUp_InActiveImagePath"];
            }
            set
            {
                ViewState["FiveUp_InActiveImagePath"] = value;
            }
        }


        public string FiveUp_EvenImagePath
        {
            get
            {
                return (string)ViewState["FiveUp_EvenImagePath"];
            }
            set
            {
                ViewState["FiveUp_EvenImagePath"] = value;
            }
        }

        public string FiveUp_VotesText
        {
            get
            {
                return (string)ViewState["FiveUp_VotesText"];
            }
            set
            {
                ViewState["FiveUp_VotesText"] = value;
            }
        }

        public string FiveUp_UpText
        {
            get
            {
                return (string)ViewState["FiveUp_UpText"];
            }
            set
            {
                ViewState["FiveUp_UpText"] = value;
            }
        }



        protected void Page_Load(object sender, EventArgs e)
        {

        }

        public void LoadVoting()
        {
            switch (Mode)
            {
                case EMode.UpDown:
                    SetupUpDown();
                    break;

                case EMode.FiveUp:
                    SetupFiveUp();
                    break;
            }
        }

        protected void SetupUpDown()
        {
            string strUpImage = UpImagePath;
            if (!string.IsNullOrEmpty(strUpImage))
            {
                imgUp.Src = strUpImage;
            }

            string strDownImage = DownImagePath;
            if (!string.IsNullOrEmpty(strDownImage))
            {
                imgDown.Src = strDownImage;
            }

            literalVotesUp.Text = VotesUp.ToString();
            literalVotesDown.Text = VotesDown.ToString();

            string strHoverScriptFormat = @"
    $(document).ready(function() {{

        $('#{0}').hover(
        function() {{
            $('#{1}').html('{2}');
        }},
        function() {{
            $('#{1}').html('');
        }}
      );

        $('#{3}').hover(
        function() {{
            $('#{1}').html('{4}');
        }},
        function() {{
            $('#{1}').html('');
        }}
      );


    }});
";
            string strUpText = UpText;
            if (string.IsNullOrEmpty(strUpText))
            {
                strUpText = "Like This";
            }

            string strDownText = DownText;
            if (string.IsNullOrEmpty(strDownText))
            {
                strDownText = "Don't Like This";
            }

            strUpText = strUpText.Replace("'", "\\'");
            strDownText = strDownText.Replace("'", "\\'");


            string strHoverScript = string.Format(strHoverScriptFormat,
                imgUp.ClientID,
                divVotingText.ClientID,
                strUpText,
                imgDown.ClientID,
                strDownText);

            string strScriptKey = string.Format("votingscr-{0}", this.ClientID);
            this.Page.ClientScript.RegisterStartupScript(typeof(string), strScriptKey, strHoverScript, true);

            multiview.SetActiveView(viewUpDown);
        }

        protected void AppendAttributeIfSet(System.Text.StringBuilder sb, string strAttribute, object objValue)
        {
            if (objValue != null)
            {
                string strValue = objValue.ToString();
                if (!string.IsNullOrEmpty(strValue))
                {
                    if (sb.Length > 0)
                    {
                        sb.AppendLine(",");
                    }
                    sb.AppendFormat("{0}: '{1}' ", strAttribute, strValue);
                }
            }
        }

        protected void SetupFiveUp()
        {

            System.Text.StringBuilder sbAttributes = new System.Text.StringBuilder();

            AppendAttributeIfSet(sbAttributes, "vote", FiveUp_Vote);
            AppendAttributeIfSet(sbAttributes, "votesText", FiveUp_VotesText);
            AppendAttributeIfSet(sbAttributes, "activeImage", FiveUp_ActiveImagePath);
            AppendAttributeIfSet(sbAttributes, "inactiveImage", FiveUp_InActiveImagePath);
            AppendAttributeIfSet(sbAttributes, "evenImage", FiveUp_EvenImagePath);

            string strUpText = FiveUp_UpText;
            if (!string.IsNullOrEmpty(strUpText))
            {
                string[] astrUpText = strUpText.Split(new char[] { ',' });

                System.Text.StringBuilder sbUpText = new System.Text.StringBuilder();
                foreach (string strUpTextSetting in astrUpText)
                {
                    if (sbUpText.Length > 0)
                    {
                        sbUpText.Append(",");
                    }
                    sbUpText.AppendFormat("'{0}'", strUpTextSetting);
                }

                if (sbAttributes.Length > 0)
                {
                    sbAttributes.AppendLine(",");
                }
                sbAttributes.AppendFormat("aSelectText: [{0}]\r\n",
                    sbUpText.ToString());
            }


            string strScriptFormat = @"
$(document).ready(function() {{
        $('#{0}').voting(
            {{
                {1}
        }});
    }});
";

            /* vote: 1.5,
                votesText: '125 Votes' */


            string strScript = string.Format(strScriptFormat, fivebuttons.ClientID, sbAttributes.ToString());
            string strScriptKey = string.Format("votingscr-multi-{0}", this.ClientID);
            this.Page.ClientScript.RegisterStartupScript(typeof(string), strScriptKey, strScript, true);

            this.Page.ClientScript.RegisterClientScriptInclude("voting", "/scms/modules/submission/public/js/voting.js");

            /*
                        string strUpImage = UpImagePath;
                        if (!string.IsNullOrEmpty(strUpImage))
                        {
                            imgUp1.Src = strUpImage;
                            imgUp2.Src = strUpImage;
                            imgUp3.Src = strUpImage;
                            imgUp4.Src = strUpImage;
                            imgUp5.Src = strUpImage;
                        }



                        string strHoverScriptFormat = @"
                $(document).ready(function() {{

                    $('#{0}').hover(
                    function() {{
                        $('#{1}').html('{2}');
                    }},
                    function() {{
                        $('#{1}').html('');
                    }}
                  );

                    $('#{3}').hover(
                    function() {{
                        $('#{1}').html('{4}');
                    }},
                    function() {{
                        $('#{1}').html('');
                    }}
                  );


                }});
            ";
                        string strUpText = UpText;
                        if( string.IsNullOrEmpty(strUpText))
                        {
                            strUpText = "Like This";
                        }

                        string strDownText = DownText;
                        if (string.IsNullOrEmpty(strDownText))
                        {
                            strDownText = "Don't Like This";
                        }

                        strUpText = strUpText.Replace("'", "\\'");
                        strDownText = strDownText.Replace("'", "\\'");


                        string strHoverScript = string.Format( strHoverScriptFormat, 
                            imgUp.ClientID, 
                            divVotingText.ClientID, 
                            strUpText,
                            imgDown.ClientID,
                            strDownText);

                        string strScriptKey = string.Format("votingscr-{0}", this.ClientID);
                        this.Page.ClientScript.RegisterStartupScript(typeof(string), strScriptKey, strHoverScript, true);
            */
            multiview.SetActiveView(viewFiveUp);
        }

        protected void btnUp_Click(object sender, EventArgs args)
        {
            if (OnUpDownVote != null)
            {
                OnUpDownVote(Id, true);
            }
        }

        protected void btnDown_Click(object sender, EventArgs args)
        {
            if (OnUpDownVote != null)
            {
                OnUpDownVote(Id, false);
            }
        }

        protected void btnUpMultiple_Click(object sender, EventArgs args)
        {
            if (OnFiveUpVote != null)
            {
                int nVoteUp = 0;
                if (sender == btnUp1)
                {
                    nVoteUp = 0;
                }
                else if (sender == btnUp2)
                {
                    nVoteUp = 1;
                }
                else if (sender == btnUp3)
                {
                    nVoteUp = 2;
                }
                else if (sender == btnUp4)
                {
                    nVoteUp = 3;
                }
                else if (sender == btnUp5)
                {
                    nVoteUp = 4;
                }

                OnFiveUpVote(Id, nVoteUp);
            }
        }
    }
}