using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MoneyRetard.Models;

namespace MoneyRetard.Controllers
{
    public class WelcomeController : Controller
    {
        //
        // GET: /Welcome/
        FlZlEntities fjWel = new FlZlEntities();
        public ActionResult Index()
        {
            List<string> ls = new List<string>();
            ViewData["now"] = DateTime.Now.ToString("yyyy年MM月dd日");
            List<FJ_AffixUrl> lfa = (from n1 in fjWel.FJ_AffixUrl
                                    where n1.Status!=1
                                    orderby n1.CreateTime
                                        select n1).Take(10).ToList();
            string s = "";
            foreach (FJ_AffixUrl au in lfa) {
                s = au.FJ_ZlInfo.Zl_Name;
                switch (au.Status) {
                    case 0: s += "未通过审核"; break;
                    case 2: s += "通过审核"; break;
                }
                ls.Add(s);
            }
            ViewData["ls"] = ls;
            return View();
        }

    }
}
