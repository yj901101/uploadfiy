using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MoneyRetard.Controllers
{
    public class JumpController : Controller
    {
        //
        // GET: /Jump/

        public ActionResult Index()
        {
            if (Session["limit"].ToString() == "0")
            {
                return RedirectToAction("../Index/Index");
            }
            else {
                return RedirectToAction("../ApplyForm/Index");   
            }
        }

    }
}
