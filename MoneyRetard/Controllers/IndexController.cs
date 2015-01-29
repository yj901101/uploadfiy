using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MoneyRetard.Models;

namespace MoneyRetard.Controllers
{
    public class IndexController : Controller
    {
        //
        // GET: /Index/
        FlZlEntities fj = new FlZlEntities();
        public ActionResult Index()
        {
            if (Session["limit"].ToString() != "0") {
                return RedirectToAction("../");
            }
            List<FJ_BasicField> fb = fj.FJ_BasicField.Where(u=>u.PID==0).ToList();
            ViewData["fb"] = fb;
            return View();
        }
        [HttpPost]
        public string AddObj()
        {
            List<FJ_BasicField> fbd = fj.FJ_BasicField.Where(u=>u.PID==0).ToList();
            foreach (FJ_BasicField bf in fbd)
            {
                string s = "";
                try
                {
                    s = Request.Form["fb+" + bf.id].ToString();
                }
                catch {
                    s = "";
                }
                FJ_BasicData bd = new FJ_BasicData();
                bd.bid = bf.id;//FJ_BasicField对应的
                bd.Value = s;//填写的内容
                fj.FJ_BasicData.AddObject(bd);
                fj.SaveChanges();
            }
            return "";
        }

    }
}
