using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace scms.modules.special.data
{
    public partial class specialDataContext
    {
        public specialDataContext()
				: base(System.Configuration.ConfigurationManager.ConnectionStrings["amplexConnectionString"].ConnectionString)
        {
            
        }
    }
}
