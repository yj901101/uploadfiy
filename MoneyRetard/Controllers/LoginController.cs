using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MoneyRetard.Models;
using MoneyRetard.Content;

namespace MoneyRetard.Controllers
{
    public class LoginController : Controller
    {
        //
        // GET: /Login/
        FlZlEntities fjLgon = new FlZlEntities();
        public ActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Index(string user, string pwd)
        {
            JsonModel jm = new JsonModel();
            List<FJ_User> lfu = fjLgon.FJ_User.Where(u => u.UserName == user ).ToList();
            if (lfu.Count <= 0)
            {
                jm.statu = "error";
                jm.msg = "账号不存在!";
            }
            else if (lfu[0].Pwd == pwd) {
                Session["userID"] = lfu[0].id;
                Session["limit"] = lfu[0].Limits;
                jm.statu = "success";
                jm.msg = "登陆成功!";
            }else{
                jm.statu = "error";
                jm.msg = "密码错误!";
            }
            return Json(jm);
        }

    }
}
