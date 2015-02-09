using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MoneyRetard.Models;
using MoneyRetard.Content;

namespace MoneyRetard.Controllers
{
    public class FAgencyRegisterController : Controller
    {
        //
        // GET: /FAgencyRegister/
        FlZlEntities fjFrge = new FlZlEntities();
        public ActionResult Index()
        {
            List<FJ_Select> lfs = fjFrge.FJ_Select.Where(u => u.type == 1).ToList();//县区的下拉
            SelectList datatype_sellist = new SelectList(lfs, "id", "name");
            ViewData["lfs"] = datatype_sellist.AsEnumerable();
            return View();
        }
        public ActionResult Register() {
            JsonModel jsm = new JsonModel();
            try
            {
                FJ_User fu = new FJ_User();
                fu.IsAgency = 2;
                fu.Limits = 1;//用户权限
                fu.CreateTime = DateTime.Now;
                fu.IsStart = 0;//是否被启用
                fu.UserName = Request.Form["userID"];
                fu.Pwd = Request.Form["userPwd"].ToString();
                fjFrge.FJ_User.AddObject(fu);
                fjFrge.SaveChanges();
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
                fui.IsAgency = 2;
                fjFrge.FJ_UserInfo.AddObject(fui);
                fjFrge.SaveChanges();
                jsm.statu = "success";
                jsm.msg = "添加成功,请等待管理员审核！";
            }
            catch
            {
                jsm.statu = "error";
                jsm.msg = "添加账号出错，请重新添加";
            }
            return RedirectToAction("../");
        }
    }
}
