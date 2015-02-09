using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MoneyRetard.Models;
using MoneyRetard.Content;

namespace MoneyRetard.Controllers
{
    public class RegisterController : Controller
    {
        //
        // GET: /Register/
        FlZlEntities fjReg = new FlZlEntities();
        public ActionResult Index()
        {
            return View();
        }
        //管理员分配账号
        public ActionResult AllotID() {
            JsonModel jsm = new JsonModel();
            try
            {
                FJ_User fu = new FJ_User();
                int agency = Convert.ToInt32(Request.Form["userType"]);//用户类别
                fu.IsAgency = agency;
                fu.Limits = Convert.ToInt32(Request.Form["userLimit"]);//用户权限
                fu.CreateTime = DateTime.Now;
                fu.IsStart = 1;
                fu.UserName = Request.Form["userID"];
                try
                {
                    if (Request.Form["userPwd"].ToString() != "")
                    {
                        fu.Pwd = Request.Form["userPwd"].ToString();
                    }
                    else {
                        fu.Pwd = "123456";
                    }
                }
                catch
                {
                    fu.Pwd = "123456";
                }
                fjReg.FJ_User.AddObject(fu);
                fjReg.SaveChanges();
                FJ_UserInfo fui = new FJ_UserInfo();
                fui.UserRealName = Request.Form["userRealName"];
                fui.UserID = fu.id;
                fui.UserType = Convert.ToInt32(Request.Form["busType"]);
                fui.Icard_1 = Convert.ToInt32(Request.Form["IC1"]);
                fui.Icard_1_Num = Request.Form["IC1Num"];
                fui.Icard_2 = Convert.ToInt32(Request.Form["IC3"]);
                fui.Icard_2_Num = Request.Form["IC3Num"];
                fui.MustNum = Request.Form["IC2Num"];//代理机构注册证
                fui.Area = Convert.ToInt32(Request.Form["Area"]);
                fui.LegalPeo = Request.Form["faren"];
                fui.Email = Request.Form["emailNum"];
                fui.Adress = Request.Form["linkAdree"];
                fui.Phone = Request.Form["linkNum"];
                fui.QQ = Request.Form["linkQQ"];
                fui.CreateTime = DateTime.Now;
                fui.IsAgency = agency;
                fjReg.FJ_UserInfo.AddObject(fui);
                fjReg.SaveChanges();
                jsm.statu = "success";
                jsm.msg = "添加成功";
            }
            catch {
                jsm.statu = "error";
                jsm.msg = "添加账号出错，请重新添加";
            }
            return Json(jsm);
        }
    }
}
