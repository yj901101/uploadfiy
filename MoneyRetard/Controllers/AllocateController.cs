using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MoneyRetard.Controllers
{
    public class AllocateController : Controller
    {
        //
        // GET: /Allocate/
        Models.FlZlEntities db = new Models.FlZlEntities();
        public ActionResult Index()
        {
          //  db.FJ_User.Include(
            return View();
        }

    }
}
