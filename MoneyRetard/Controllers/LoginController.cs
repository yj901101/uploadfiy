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
        //登陆账号判断
        // GET: /Login/
        FlZlEntities fjLgon = new FlZlEntities();
        [HttpGet]
        public ActionResult Index()
        {
            Session["ty"] = Request.QueryString["ty"];
            int ty = 1;
            if (!string.IsNullOrEmpty(Request.QueryString["ty"]))
            {
                ty = Convert.ToInt32(Request.QueryString["ty"].ToString());
            }
            string title = string.Empty;
            if (ty == 2)
            {
                ViewData["title"] = "代理机构用户登录";
            }
            else if (ty == 1)
            {
                ViewData["title"] = "非代理机构用户登录";
            }
            else 
            {
                ViewData["title"] = "管理员登录";
            }
            if (ty== 1)
            {
                ViewData["IsDisplay"] = 1;
            }
            else 
            {
                ViewData["IsDisplay"] = 2;
            }
            return View();
        }
        [HttpPost]
        public ActionResult Index(string user, string pwd,string vcode)
        {
            JsonModel jm = new JsonModel();
            List<FJ_User> lfu = fjLgon.FJ_User.Where(u => u.UserName == user ).ToList();
            int bty = 0;
            try
            {
                bty = Convert.ToInt32(Session["ty"]);
            }
            catch {
                bty = 0;
            }
            if (lfu[0].IsAgency == 2)
            {
                DateTime nowTime = DateTime.Now;
                DateTime newTime = Convert.ToDateTime(lfu[0].CreateTime).AddYears(1);
                if (newTime <= nowTime)
                {
                    lfu[0].IsStart = 0;
                    UpdateModel(lfu[0]);
                    fjLgon.SaveChanges();                                         
                }
            }
            string code = Session["vcode"].ToString();
            if (lfu.Count <= 0)
            {
                jm.statu = "error";
                jm.msg = "账号不存在!";
            }
            else if (code != vcode)
            {
                jm.statu = "error";
                jm.msg = "验证码错误!";
            }
            else if (lfu[0].Pwd == pwd) {
                if (lfu[0].IsStart == 0) {//判断用户的账号是否激活
                    jm.statu = "error";
                    jm.msg = "请联系管理员激活此账号!";
                    return Json(jm);
                }
                //if (lfu[0].IsAgency != 0 && lfu[0].IsAgency!=3)
              //  {//判断是否为代理机构 3是管理员，0为默认分配账号，还未选择类型
                    if (lfu[0].IsAgency != bty) {
                        if (bty == 0) {
                            return RedirectToAction("../");
                        }
                        jm.statu = "error";
                        jm.msg = "用户类别出错!";
                        return Json(jm);
                    }
              //  }
                if (!string.IsNullOrEmpty(Request.Form["always"]))
                {
                    HttpCookie cookie = new HttpCookie("ucookie", lfu[0].id.ToString());
                    cookie.Expires = DateTime.Now.AddDays(1);
                    Response.Cookies.Add(cookie);
                }
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
