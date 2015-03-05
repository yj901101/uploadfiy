using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MoneyRetard.Models;
using MoneyRetard.Content;
using WordMark.Content;
using Microsoft.Office.Interop.Word;

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
            ViewData["year"] = DateTime.Now.ToString("yyyy");
            FJ_ZlInfo fz = (from n1 in fjApp.FJ_ZlInfo
                            orderby n1.id descending
                            select n1).FirstOrDefault();
            try
            {
                ViewData["lastId"] = DateTime.Now.ToString("yyyy") + "00" + (fz.id+1);//获取最大的一个id
            }
            catch {
                ViewData["lastId"] = DateTime.Now.ToString("yyyy") + "001";
            }
            //下拉数据专利类型
            List<FJ_Select> datatype_list = fjApp.FJ_Select.Where(u=>u.type==2).ToList();
            SelectList datatype_sellist = new SelectList(datatype_list, "id", "name");
            ViewData["datatype_sellist"] = datatype_sellist.AsEnumerable();
            int userId = Convert.ToInt32(Session["userID"]);
            FJ_UserInfo fu = fjApp.FJ_UserInfo.Where(u => u.UserID == userId).FirstOrDefault();
            try
            {
                ViewData["deptname"] = fu.UserRealName;//申请单位
                ViewData["LinkPeo"] = fu.LinkName;//联系人
                ViewData["Adress"] = fu.Adress;//地址
                ViewData["UserType"] = fu.UserType;//企业性质
                ViewData["Phone"] = fu.Phone;//电话
            }
            catch {
                ViewData["deptname"] = "";//申请单位
                ViewData["LinkPeo"] = "";//联系人
                ViewData["Adress"] = "";//地址
                ViewData["UserType"] = "";//企业性质
                ViewData["Phone"] = "";//电话
            }
            var user_id = Session["userID"];
            if (user_id != null)
            {
                ViewData["user_id"] = user_id.ToString();
            }
            else
            {
                ViewData["user_id"] = "";
            }
            return View();
        }
        public ActionResult LoginOut()
        {
            string user_id = Request.QueryString["user_id"];
            JsonModel jsmodel = new JsonModel();
            if (!string.IsNullOrEmpty(user_id))
            {
                HttpCookie user_cookie = Request.Cookies["ucookie"];
                if (user_cookie != null && (user_cookie.Value.ToString() == user_id))
                {
                    user_cookie.Expires = DateTime.Now.AddDays(-1);
                    Response.Cookies.Add(user_cookie);
                }
                jsmodel.statu = "ok";
                jsmodel.msg = "退出成功";
            }
            else
            {
                jsmodel.statu = "fail";
                jsmodel.msg = "退出失败，用户信息错误，请联系网站管理员";
            }
            return Json(jsmodel);
        }
        [HttpPost]
        public ActionResult FromSubmit() {
            JsonModel jmform = new JsonModel();
            Random ran = new Random();
            int RandKey = ran.Next(100000, 999999);//随即生成一个六位数
            string newid = DateTime.Now.ToString("yyyyMMddmmss"+RandKey);//生成的批号
            List<FJ_BasicField> lfbf = fjApp.FJ_BasicField.ToList();//表单取名
            int userId = Convert.ToInt32(Session["userID"]);
            string FilePath = Server.MapPath("../wordFiles/table1.docx");//模板路径
            Report report = new Report();//生成表格
            report.CreateNewDocument(FilePath);
            foreach (FJ_BasicField fbf in lfbf) {
                string s = "";
                try
                {
                    s = Request.Form["fj" + fbf.id].ToString();
                }
                catch
                {
                    s = "";
                }
                FJ_BasicData bd = new FJ_BasicData();
                bd.bid = fbf.id;//FJ_BasicField对应的
                bd.Value = s;//填写的内容
                bd.UserId = userId;
                bd.guid = newid;
                fjApp.FJ_BasicData.AddObject(bd);
                fjApp.SaveChanges();
                if (s != "") { //判断当前标签是否被选中
                    report.InsertValue("fj" + fbf.id, s);
                    if (fbf.id == 5) {
                        report.InsertValue("fj" + s, "■");//替换chckbox
                    }
                }
            }
            FJ_UserInfo fu = fjApp.FJ_UserInfo.Where(u => u.UserID == userId).FirstOrDefault();//判断当前用户是否为代理机构
            if (fu.IsAgency == 1) { //是代理机构的话，将当前用户的名称填入word代理机构一栏中
                report.InsertValue("fj12", fu.UserRealName);//代理机构在word书签编号为12
            }
            Table table = report.InsertTable("fj14", 1, 3, 0);//生成一个一列3行的表格
            report.InsertCell(table, 1, 1, "专利申请类型");//表名,行号,列号,值
            report.InsertCell(table, 1, 2, "专利申请名称");//表名,行号,列号,值
            report.InsertCell(table, 1, 3, DateTime.Now.ToString("yyyy")+"编号");//表名,行号,列号,值
            for (int i = 0; i < 100; i++) {
                int zltype = 0;
                string zlname = "";
                string zlnum = "";
                try
                {
                    zltype = Convert.ToInt32(Request.Form["zltype" + i]);
                    zlname = Request.Form["zlname" + i].ToString();
                    zlnum = Request.Form["zlnum" + i].ToString();
                }
                catch {
                    break;//专利数量不到10条
                }
                if (zlname != "")
                {
                    FJ_ZlInfo fzi = new FJ_ZlInfo();
                    fzi.guid = newid;
                    fzi.Zl_Name = zlname;
                    fzi.ZL_Bnum = zlnum;
                    fzi.Zl_Type = zltype;
                    fzi.userid = userId;
                    fjApp.FJ_ZlInfo.AddObject(fzi);
                    fjApp.SaveChanges();
                    report.AddRow(table);
                    report.InsertCell(table, i + 2, 1, getType(zltype));//表名,行号,列号,值
                    report.InsertCell(table, i + 2, 2, zlname);//表名,行号,列号,值
                    report.InsertCell(table, i + 2, 3, zlnum);//表名,行号,列号,值
                }
            }
            string SaveDocPath = Server.MapPath("../wordFiles/" + newid + ".doc");//保存的word地址
            FJ_ExportWord few = new FJ_ExportWord();//将系统自动文件地址添加到表中
            few.guid = newid;
            few.FileType = 1;
            few.url = "/wordFiles/" + newid + ".doc";
            few.creatTime = DateTime.Now;
            fjApp.FJ_ExportWord.AddObject(few);
            fjApp.SaveChanges();//将文件的地址上传了
            report.SaveDocument(SaveDocPath);
            jmform.statu = "success";
            jmform.msg = "word生成成功";
            jmform.guid = "/wordFiles/" + newid + ".doc";
            return Json(jmform);
        }
        public ActionResult updatePwa()
        {//修改密码

            return View();
        }
        public ActionResult Person_Zl() {
            int pageIndex = 1;
            string filtername = Request.QueryString["filtername"];
            string filternum = Request.QueryString["filternum"];
            try
            {
                pageIndex = int.Parse(Request.QueryString["pager"]);
            }
            catch
            {
                pageIndex = 1;
            }
            ViewData["pagerIndex"] = pageIndex;
            const int pageSize = 10;
            int userid = Convert.ToInt32(Session["userID"]);
            //查看专利
            var relationpeo = from n1 in fjApp.personDatabases
                              where n1.userid == userid
                              //orderby n1.id descending
                              select n1;
            if (!string.IsNullOrEmpty(filtername))
            {
                relationpeo = relationpeo.Where(u => u.Zl_Name.Contains(filtername));
            }
            if (!string.IsNullOrEmpty(filternum))
            {
                relationpeo = relationpeo.Where(u => u.ZL_Bnum.Contains(filternum));
            }
            relationpeo = relationpeo.OrderByDescending(u => u.id);
            YJPagedList<personDatabase> pagerList;
            try
            {
                pagerList = new YJPagedList<personDatabase>(relationpeo, pageIndex - 1, pageSize);
            }
            catch
            {
                pagerList = new YJPagedList<personDatabase>(relationpeo, 0, pageSize);
            }
            ViewData["Total"] = relationpeo.Count();
            ViewData["TotalPager"] = pagerList.TotalPages;
            ViewData["rpeo"] = pagerList.ToList();
            return View();
        }
        private string getType(int num) {
            switch (num) {
                case 1: return "发明专利";
                case 2: return "外观专利";
                default: return "实用新型";
            }
        }
        public ActionResult AUserDetail(int id)
        {
            int uid = id;
            Models.UserInfoDetail userinfo = fjApp.FJ_User.Where(u=>u.id==uid).Join(fjApp.FJ_UserInfo, u => u.id, i => i.UserID, (u, i) => new Models.UserInfoDetail { id = u.id, UserName = u.UserName, Pwd = u.Pwd, UserType = i.UserType, UserRealName = i.UserRealName, Area = i.Area, Adress = i.Adress, Phone = i.Phone, Email = i.Email, QQ = i.QQ,LinkName = i.LinkName }).ToList().FirstOrDefault();
            ViewData["User"] = userinfo;
            List<Models.FJ_Select> lfs = fjApp.FJ_Select.Where(u => u.type == 1).ToList();//县区的下拉
            SelectList datatype_sellist = new SelectList(lfs, "id", "name");
            ViewData["lfs"] = datatype_sellist.AsEnumerable();
            FJ_User fj = fjApp.FJ_User.Where(u => u.id == uid).FirstOrDefault();
            List<SelectListItem> items = new List<SelectListItem>();
            List<SelectListItem> items2 = new List<SelectListItem>();
            if (fj.IsAgency == 1) {
                items.Add(new SelectListItem { Text = "电子申请号批文", Value = "6", Selected = true });
                items.Add(new SelectListItem { Text = "营业执照副本", Value = "7" });
                items2.Add(new SelectListItem { Text = "电子申请号批文", Value = "6" });
                items2.Add(new SelectListItem { Text = "营业执照副本", Value = "7", Selected = true });
            }
            else if (fj.IsAgency == 2) {
                items.Add(new SelectListItem { Text = "职业人证1", Value = "5", Selected = true });
                items.Add(new SelectListItem { Text = "职业人证2", Value = "15" });
                items2.Add(new SelectListItem { Text = "职业人证2", Value = "15", Selected = true });
                items2.Add(new SelectListItem { Text = "职业人证1", Value = "5" });
            }
            ViewData["item"] = items;
            ViewData["item2"] = items2;
            ViewData["agent"] = fj.IsAgency;
            return View();
        }
        public ActionResult AUserModify()
        {
            int id = Convert.ToInt32(Request.Params["uid"]);
            Models.FJ_User oldusermodel = fjApp.FJ_User.Where(u => u.id == id).ToList().FirstOrDefault();
            Models.FJ_UserInfo oldinfo = fjApp.FJ_UserInfo.Where(i => i.UserID == id).ToList().FirstOrDefault();
            string username = Request.Params["uname"].Trim().ToString();
            string pwd = Request.Params["pwd"].Trim().ToString();
            oldusermodel.UserName = username;
            oldusermodel.Pwd = pwd;
            fjApp.SaveChanges();
            string userrealname = Request.Params["companyname"].Trim().ToString();
            int usertype = Convert.ToInt32(Request.Params["busType"].ToString());
            int area = Convert.ToInt32(Request.Params["Area"].ToString());
            string address = Request.Params["add"].Trim().ToString();
            string phone = Request.Params["callnum"].Trim().ToString();
            if (Request.Params["file1"] != null && Request.Params["file1"].ToString() != "") {
                oldinfo.Icard_1_Num = Request.Params["file1"].ToString(); 
            }
            if (Request.Params["file2"] != null && Request.Params["file2"].ToString() != "")
            {
                oldinfo.Icard_2_Num = Request.Params["file2"].ToString();
            }
            if (Request.Params["file8"] != null && Request.Params["file8"].ToString() != "")
            {
                oldinfo.MustNum = Request.Params["file8"].ToString();
            }
            oldinfo.Icard_1 = Convert.ToInt32(Request.Params["IC1"]);
            oldinfo.Icard_2 = Convert.ToInt32(Request.Params["IC2"]);
            string email = string.Empty;
            string qq = string.Empty;
            if (!string.IsNullOrEmpty(Request.Params["email"]))
            {
                email = Request.Params["email"].Trim().ToString();
            }
            if (!string.IsNullOrEmpty(Request.Params["qq"]))
            {
                qq = Request.Params["qq"].Trim().ToString();
            }
            oldinfo.UserRealName = userrealname;
            oldinfo.UserType = usertype;
            oldinfo.Adress = address;
            oldinfo.Area = area;
            oldinfo.Phone = phone;
            oldinfo.Email = email;
            oldinfo.QQ = qq;
            oldinfo.LinkName = Request.Params["linkname"];
            fjApp.SaveChanges();
            MoneyRetard.Content.JsonModel jsmodel = new JsonModel() { msg = "修改成功!" };
            return Json(jsmodel);
        }
        public string lastId() {
            FJ_ZlInfo fz = (from n1 in fjApp.FJ_ZlInfo
                            orderby n1.id descending
                            select n1).FirstOrDefault();
            try
            {
                return DateTime.Now.ToString("yyyy") + "00" + (fz.id + 1);//获取最大的一个id
            }
            catch
            {
                return DateTime.Now.ToString("yyyy") + "001";
            }
        }
    }
}
