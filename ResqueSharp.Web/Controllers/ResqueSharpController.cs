using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;


namespace ResqueSharp.Web.Controllers
{
    public abstract class ResqueSharpController : Controller
    {
        public Resque ResqueClient { get; set; }

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            this.ResqueClient = (Resque)HttpContext.Items["CurrentRequestResqueClient"];
        }
    }
}
