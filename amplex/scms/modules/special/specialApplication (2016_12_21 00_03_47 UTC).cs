using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace scms.modules.special
{
	class specialApplication : IPluginApplication
	{
		protected int ? nApplicationId = null;
		public void Init(int ? nApplicationId)
		{
			specialEventHandler handler = new specialEventHandler();
			forms.Forms.RegisterFormSubmissionEventHandler(handler);
			this.nApplicationId = nApplicationId;
		}

		public int ? GetApplicationId()
		{
			return nApplicationId;
		}
	}
}

