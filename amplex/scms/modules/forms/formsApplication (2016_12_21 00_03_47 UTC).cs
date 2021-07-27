using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace scms.modules.forms
{
  class formsApplication : IPluginApplication
  {
		protected int ? nApplicationId = null;
		public void Init(int ? nApplicationId)
		{
			this.nApplicationId = nApplicationId;
			formsAutoResponderEventHandler handler = new formsAutoResponderEventHandler();
			forms.Forms.RegisterFormSubmissionEventHandler(handler);
		}

		public int ? GetApplicationId()
		{
			return nApplicationId;
		}
	}
}
