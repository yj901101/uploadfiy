using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MoneyRetard.Models;
using MoneyRetard.Content;

namespace MoneyRetard.Controllers
{
    public class ApplyFormController : Controller
    {
        //
        // GET: /ApplyForm/
        FlZlEntities fjApp = new FlZlEntities();
        public ActionResult Index()
        {
            ViewData["now"] = DateTime.Now.ToString("yyyy年MM月dd日");
            return View();
        }
        public ActionResult Person_Zl() {
            int pageIndex = 1;
            try
            {
                pageIndex = int.Parse(Request.QueryString["pager"]);
            }
            catch
            {
                pageIndex = 1;
            }
            ViewData["pagerIndex"] = pageIndex;
            const int pageSize = 1;
            int userid = Convert.ToInt32(Session["userID"]);
            //查看专利
            var relationpeo = from n1 in fjApp.personDatabases
                              where n1.userid == userid
                              orderby n1.id descending
                              select n1;
            PagedList<personDatabase> pagerList;
            try
            {
                pagerList = new PagedList<personDatabase>(relationpeo, pageIndex - 1, pageSize);
            }
            catch
            {
                pagerList = new PagedList<personDatabase>(relationpeo, 0, pageSize);
            }
            ViewData["Total"] = relationpeo.Count();
            ViewData["TotalPager"] = pagerList.TotalPages;
            ViewData["rpeo"] = pagerList.ToList();
            return View();
        }
    }
}
