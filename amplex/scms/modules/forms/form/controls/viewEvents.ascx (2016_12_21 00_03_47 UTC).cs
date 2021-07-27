using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace scms.modules.forms.form.controls
{
    public partial class viewEvents : System.Web.UI.UserControl
    {
        public int? SiteId
        {
            get { return (int?)ViewState["SiteId"]; }
            set { ViewState["SiteId"] = value; }
        }

        public int? FormId
        {
            get { return (int?)ViewState["FormId"]; }
            set { ViewState["FormId"] = value; }
        }

        public int? EditEventHandlerId
        {
            get { return (int?)ViewState["EditEventHandlerId"]; }
            set { ViewState["EditEventHandlerId"] = value; }
        }

        protected void Page_Init(object sender, EventArgs e)
        {

        }

        protected void Page_Load(object sender, EventArgs e)
        {
            panelEditEvent.Visible = true;
            if (!IsPostBack)
            {
                var eventHandlers = global::scms.modules.forms.Forms.GetEventHandlers();
                ddlEvents.DataTextField = "Name";
                ddlEvents.DataValueField = "UniqueName";
                ddlEvents.DataSource = eventHandlers;
                ddlEvents.DataBind();

                

                // panelEditEvent.Visible = false;
            }

            int? nEditEventHandlerId = EditEventHandlerId;
            if (nEditEventHandlerId.HasValue)
            {
                placeholderEventHandlerSettings.Controls.Clear();
                EditEventHandler(nEditEventHandlerId.Value, false);
            }
            else
            {
                panelEditEvent.Visible = false;
            }

						EnableControls();
        }

        public void LoadEventHandlers(bool bForceShowEvents)
        {
            try
            {
                if (FormId.HasValue)
                {
                    lvEvents.DataSource = null;

                    var eventHandlerTypes = global::scms.modules.forms.Forms.GetEventHandlers();


                    global::scms.data.ScmsDataContext dc = new global::scms.data.ScmsDataContext();
                    var eventHandlers = (from eh in dc.scms_form_eventhandlers
                                         where eh.formid == FormId.Value
                                         where eh.deleted == false
                                         select eh).ToArray();

                    var namedEventHandlers = (from eh2 in eventHandlers
                                              join eht in eventHandlerTypes on eh2.eventName equals eht.UniqueName
                                              orderby eh2.ordinal
                                              select new { id = eh2.id, name = eht.Name });

                    lvEvents.DataSource = namedEventHandlers;
                    lvEvents.DataBind();
                }
            }
            catch (Exception ex)
            {
                string strMessage = string.Format("Failed loading events for form id '{0}'.", FormId.Value);
                global::scms.ScmsEvent.Raise(strMessage, this, ex);
            }
        }

        protected void btnNewEvent_Click(object sender, EventArgs args)
        {
            if (FormId.HasValue)
            {
                int? nEventHandlerId = null;

                // create the event
                try
                {
                    global::scms.data.ScmsDataContext dc = new global::scms.data.ScmsDataContext();
                    int maxOrdinal = (from e in dc.scms_form_eventhandlers
                                      where e.formid == FormId.Value
                                      where e.deleted == false
                                      orderby e.ordinal descending
                                      select e.ordinal).FirstOrDefault();
                    int nOrdinal = maxOrdinal + 1;

                    global::scms.data.scms_form_eventhandler eventHandler = new global::scms.data.scms_form_eventhandler();
                    dc.scms_form_eventhandlers.InsertOnSubmit(eventHandler);
                    eventHandler.ordinal = nOrdinal;
                    eventHandler.formid = FormId.Value;
                    eventHandler.eventName = ddlEvents.SelectedValue;
                    dc.SubmitChanges();
                    nEventHandlerId = eventHandler.id;
                }
                catch (Exception ex)
                {
                    string strMessage = string.Format("Failed creating new event of type '{0}'.", ddlEvents.SelectedValue);
                    global::scms.ScmsEvent.Raise(strMessage, this, ex);
                    statusMessage.ShowFailure(strMessage);
                }

                if (nEventHandlerId.HasValue)
                {
                    placeholderEventHandlerSettings.Controls.Clear();
                    EditEventHandler(nEventHandlerId.Value, true);
                }

								EnableControls();
            }
        }

        protected void Event_Command(object sender, CommandEventArgs args)
        {
        }

        protected void EditEventHandler(int nEventHandlerId, bool bLoadSettings)
        {
            //placeholderEventHandlerSettings.Controls.Clear();
            panelEditEvent.Visible = true;

            if (FormId.HasValue)
            {

                try
                {
                    global::scms.data.ScmsDataContext dc = new global::scms.data.ScmsDataContext();
                    global::scms.data.scms_form_eventhandler ev =
                        (from eh in dc.scms_form_eventhandlers
                         where eh.id == nEventHandlerId
                         where eh.formid == FormId.Value
                         select eh).Single();

                    var eventHandlers = global::scms.modules.forms.Forms.GetEventHandlers();
                    var eventHandler = (from eh in eventHandlers
                                        where eh.UniqueName == ev.eventName
                                        select eh).FirstOrDefault();

                    if (eventHandler == null)
                    {
                        throw new Exception(string.Format("Event handler type '{0}' not found.", ev.eventName));
                    }

                    EditEventHandlerId = nEventHandlerId;


                    Control eventHandlerSettings = this.Page.LoadControl(eventHandler.SettingsControlPath);
                    global::scms.modules.forms.IFormSubmissionEventHandlerSettings iSettings = eventHandlerSettings as global::scms.modules.forms.IFormSubmissionEventHandlerSettings;
                    if (iSettings != null)
                    {
                        iSettings.SiteId = SiteId;
                        iSettings.FormId = FormId;
                        iSettings.EventHandlerId = nEventHandlerId;
                    }
                    placeholderEventHandlerSettings.Controls.Add(eventHandlerSettings);

                    if (bLoadSettings)
                    {
                        iSettings.LoadSettings();
                    }

                    iSettings.SetDelegates(new global::scms.modules.forms.SettingsSavedDelegate(OnEditSaved), new global::scms.modules.forms.SettingsCancelledDelegate(OnEditCancelled));

                    placeholderEventHandlerSettings.Visible = true;

                    //panelEditEvent.Visible = true;
                }
                catch (Exception ex)
                {
                    string strMessage = string.Format("Failed loading event handler '{0}'.", nEventHandlerId);
                    global::scms.ScmsEvent.Raise(strMessage, this, ex);
                    statusMessage.ShowFailure(strMessage);
                }

								EnableControls();
            }
        }


        protected void Move(object sender, CommandEventArgs args)
        {
            bool bUp = string.Compare(args.CommandName, "up", true) == 0;
            int nEventHandlerId = int.Parse((string)args.CommandArgument);

            global::scms.data.ScmsDataContext dc = new global::scms.data.ScmsDataContext();

            var eventHandlers = from ev in dc.scms_form_eventhandlers
                                where ev.formid == FormId.Value
                                where ev.deleted == false
                                orderby ev.ordinal
                                select ev;

            global::scms.data.scms_form_eventhandler prior = null;
            global::scms.data.scms_form_eventhandler next = null;

            var thisEventHandler = eventHandlers.Where(eventHandler => (eventHandler.id == nEventHandlerId)).Single();
            int nThisOrdinal = thisEventHandler.ordinal;

            foreach (var eventHandler in eventHandlers)
            {
                if (eventHandler.id != thisEventHandler.id)
                {
                    if (eventHandler.ordinal < nThisOrdinal)
                    {
                        prior = eventHandler;
                    }
                    else
                    {
                        if (next == null)
                        {
                            if (eventHandler.ordinal > nThisOrdinal)
                            {
                                next = eventHandler;
                                break;
                            }
                        }
                    }
                }
            }

            if (bUp)
            {
                if (prior != null)
                {
                    thisEventHandler.ordinal = prior.ordinal;
                    prior.ordinal = nThisOrdinal;
                }
            }
            else
            {
                if (next != null)
                {
                    thisEventHandler.ordinal = next.ordinal;
                    next.ordinal = nThisOrdinal;
                }
            }


            dc.SubmitChanges();
						EnableControls();
        }

        protected void Delete(object sender, CommandEventArgs args)
        {
            int nEventHandlerId = int.Parse((string)args.CommandArgument);

            global::scms.data.ScmsDataContext dc = new global::scms.data.ScmsDataContext();

            var eventHandler = (from eh in dc.scms_form_eventhandlers
                                where eh.id == nEventHandlerId
                                where eh.deleted == false
                                select eh).FirstOrDefault();
            if (eventHandler != null)
            {
                eventHandler.deleted = true;
                dc.SubmitChanges();
            }
						EnableControls();

            if (EditEventHandlerId.HasValue)
            {
                if (nEventHandlerId == EditEventHandlerId.Value)
                {
                    EditEventHandlerId = null;
                    panelEditEvent.Visible = false;
                }
            }
        }

        protected void Edit(object sender, CommandEventArgs args)
        {
            placeholderEventHandlerSettings.Controls.Clear();
            int nEventHandlerId = int.Parse((string)args.CommandArgument);
            EditEventHandler(nEventHandlerId, true);
						EnableControls();
        }

        protected void OnEditSaved()
        {
            EditEventHandlerId = null;
            panelEditEvent.Visible = false;
            // panelEditEvent.Visible = false;
            // Response.Redirect(Request.RawUrl);
						EnableControls();
        }

        protected void OnEditCancelled()
        {
            EditEventHandlerId = null;
            panelEditEvent.Visible = false;

            // panelEditEvent.Visible = false;
            // Response.Redirect(Request.RawUrl);
						EnableControls();
        }

				protected void EnableControls()
				{
					bool bEnabled = !EditEventHandlerId.HasValue;

					ddlEvents.Enabled = bEnabled;
					btnNewEvent.Enabled = bEnabled;
					LoadEventHandlers(false);
				}

				protected void lvEvents_ItemDataBound(object sender, ListViewItemEventArgs args)
				{
					switch (args.Item.ItemType)
					{
						case ListViewItemType.DataItem:
							{
								ListViewDataItem lvDataItem = (ListViewDataItem)args.Item;
								bool bEnabled = !EditEventHandlerId.HasValue;

								foreach (var control in args.Item.Controls)
								{
									var linkButton = control as LinkButton;
									if (linkButton != null)
									{
										linkButton.Enabled = bEnabled;
									}
									else
									{
										var imageButton = control as ImageButton;
										if (imageButton != null)
										{
											imageButton.Enabled = bEnabled;
										}
									}

									
								}

								
							}
							break;
					}
				}

    }
}