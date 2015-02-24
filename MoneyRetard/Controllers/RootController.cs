using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MoneyRetard.Content;
using MoneyRetard.Models;
using System.IO;
using Webdiyer.WebControls.Mvc;
using MoneyRetard.Common;
using System.Data;

namespace MoneyRetard.Controllers
{
    public class RootController : Controller
    {
        //
        // GET: /Root/
        FlZlEntities fjAppRoot = new FlZlEntities();
        public ActionResult Index()
        {
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
            const int pageSize = 10;
            int userid = Convert.ToInt32(Session["userID"]);
            //查看需要审核的专利FJ_ExportWord表中FileType=2
            var relationpeo = from n1 in fjAppRoot.personDatabaseByReals
                              orderby n1.id descending
                              select n1;
            YJPagedList<personDatabaseByReal> pagerList;
            try
            {
                pagerList = new YJPagedList<personDatabaseByReal>(relationpeo, pageIndex - 1, pageSize);
            }
            catch
            {
                pagerList = new YJPagedList<personDatabaseByReal>(relationpeo, 0, pageSize);
            }
            ViewData["Total"] = relationpeo.Count();
            ViewData["TotalPager"] = pagerList.TotalPages;
            ViewData["rpeo"] = pagerList.ToList();
            var user_id = Session["userID"];
            if (user_id!=null)
            {
                ViewData["user_id"] = user_id.ToString();
            }
            else 
            {
                ViewData["user_id"]="";
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
                    user_cookie.Expires=DateTime.Now.AddDays(-1);
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
        public ActionResult CheckedAlready() {
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
            const int pageSize = 10;
            int userid = Convert.ToInt32(Session["userID"]);
            //查看需要审核的专利FJ_ExportWord表中FileType=2
            var relationpeo = from n1 in fjAppRoot.Checkeds
                              orderby n1.id descending
                              select n1;
            YJPagedList<Checked> pagerList;
            try
            {
                pagerList = new YJPagedList<Checked>(relationpeo, pageIndex - 1, pageSize);
            }
            catch
            {
                pagerList = new YJPagedList<Checked>(relationpeo, 0, pageSize);
            }
            ViewData["Total"] = relationpeo.Count();
            ViewData["TotalPager"] = pagerList.TotalPages;
            ViewData["rpeo"] = pagerList.ToList();
            return View();
        }
        public ActionResult NotChecked()
        {
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
            const int pageSize = 10;
            int userid = Convert.ToInt32(Session["userID"]);
            //查看需要审核的专利FJ_ExportWord表中FileType=2
            var relationpeo = from n1 in fjAppRoot.NotCheckeds
                              orderby n1.id descending
                              select n1;
            YJPagedList<NotChecked> pagerList;
            try
            {
                pagerList = new YJPagedList<NotChecked>(relationpeo, pageIndex - 1, pageSize);
            }
            catch
            {
                pagerList = new YJPagedList<NotChecked>(relationpeo, 0, pageSize);
            }
            ViewData["Total"] = relationpeo.Count();
            ViewData["TotalPager"] = pagerList.TotalPages;
            ViewData["rpeo"] = pagerList.ToList();
            return View();
        }
        public ActionResult NotSumbit()
        {
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
            const int pageSize = 10;
            int userid = Convert.ToInt32(Session["userID"]);
            //查看需要审核的专利FJ_ExportWord表中FileType=2
            var relationpeo = from n1 in fjAppRoot.NotSumbits
                              orderby n1.id descending
                              select n1;
            YJPagedList<NotSumbit> pagerList;
            try
            {
                pagerList = new YJPagedList<NotSumbit>(relationpeo, pageIndex - 1, pageSize);
            }
            catch
            {
                pagerList = new YJPagedList<NotSumbit>(relationpeo, 0, pageSize);
            }
            ViewData["Total"] = relationpeo.Count();
            ViewData["TotalPager"] = pagerList.TotalPages;
            ViewData["rpeo"] = pagerList.ToList();
            return View();
        }
        public ActionResult allocationID() {//分配账号 
            List<FJ_Select> lfs = fjAppRoot.FJ_Select.Where(u=>u.type==1).ToList();//县区的下拉
            SelectList datatype_sellist = new SelectList(lfs, "id", "name");
            ViewData["lfs"] = datatype_sellist.AsEnumerable();
            return View();
        }
        public ActionResult Download(string downguid, int downzlID)
        {
            List<FJ_ExportWord> lfew = (from n in fjAppRoot.FJ_ExportWord
                                        where n.guid == downguid &&　n.FileType==2
                                        orderby n.creatTime descending
                                        select n).ToList();//提交的审核word表单,取最新的一个
            List<FJ_ExportWord> lfew_1 = (from n in fjAppRoot.FJ_ExportWord
                                        where n.guid == downguid && n.FileType == 3
                                        orderby n.creatTime descending
                                        select n).ToList();//代理机构证书，去最新的一个,时间从前往后排序
            DirectoryInfo theFolder = new DirectoryInfo(Server.MapPath("../zlWordFiles"));//遍历专利模板与文件的文件夹
            FileInfo[] fileInfo = theFolder.GetFiles();
            string filename = downguid + "_" + downzlID + ".doc";
            if (FilesContains(fileInfo, filename))//判断文件是否存在文件夹
            {
                ViewData["zlurl"] = "../zlWordFiles/" + filename;
            }
            else {
                createFileFromTempates(downguid, downzlID);
                ViewData["zlurl"] = "../zlWordFiles/" + filename;
            }
            List<string> ls_1 = new List<string>();
            foreach (FJ_ExportWord few in lfew) { 
                if(few.url.IndexOf(".pdf")!=-1){
                    ls_1.Add(few.url);
                }
            }
            try
            {
                ViewData["word"] = ls_1[0];
            }
            catch {
                ViewData["word"] = null;
            }//最新提交的数据
            try
            {
                ViewData["agen"] = lfew_1[0];//最新的代理证书证明
            }
            catch {
                ViewData["agen"] = null;
            }
            return View();
        }
        public ActionResult Upload(int downzlID)
        {
            ViewData["id"] = downzlID;
            return View();
        }
        private void createFileFromTempates(string guid,int id) {//根据模板创建文件
            string filename = guid + "_" + id + ".doc";
            Report report = new Report();
            string FilePath = Server.MapPath("../zlWordFiles/zlInfo.doc");//模板路径
            report.CreateNewDocument(FilePath);
            int[] idList = {1,5,13 };
            FJ_BasicData fbd1 = fjAppRoot.FJ_BasicData.Where(u => u.guid == guid && u.bid==1).FirstOrDefault();//获取批数据单位名称
            FJ_BasicData fbd5 = fjAppRoot.FJ_BasicData.Where(u => u.guid == guid && u.bid == 5).FirstOrDefault();//获取批数据单位性质
            FJ_BasicData fbd13 = fjAppRoot.FJ_BasicData.Where(u => u.guid == guid && u.bid == 13).FirstOrDefault();//获取批数据原因
            FJ_ZlInfo zl = fjAppRoot.FJ_ZlInfo.Where(u => u.id == id).FirstOrDefault();//获取当前专利对象
            report.InsertValue("fj1", fbd1.Value);//申请单位的名称
            report.InsertValue("fj" + fbd5.Value, "■");//替换checkbox
            report.InsertValue("fj13", fbd13.Value);//申请单位的理由
            report.InsertValue("zl", zl.Zl_Name);
            report.InsertValue("zl" + zl.Zl_Type, "■");//专利类型
            report.InsertValue("year",DateTime.Now.ToString("yyyy"));
            report.InsertValue("month", DateTime.Now.ToString("MM"));
            report.InsertValue("day",DateTime.Now.ToString("dd"));
            string SaveDocPath = Server.MapPath("../zlWordFiles/" + filename);
            report.SaveDocument(SaveDocPath);
        }
        public ActionResult ShowPicture(string pinum)
        {
            List<string> ls = new List<string>();
            List<FJ_ExportWord> lfew = fjAppRoot.FJ_ExportWord.Where(u => u.guid == pinum && u.FileType==2).ToList();//上传的费减审核表单
            foreach (FJ_ExportWord f in lfew) {
                if (f.url.LastIndexOf(".jpg") != -1 || f.url.IndexOf(".png") != -1 || f.url.IndexOf(".bmp") != -1 || f.url.IndexOf(".jpeg") != -1) {
                    ls.Add(f.url);
                }
            }
            ViewData["picList"] = ls;
            return View();
        }
        private bool FilesContains(FileInfo[] fileInfo, string fileName)//判断文件夹是否存在fileName文件
        { 
            foreach (FileInfo NextFile in fileInfo)  //遍历文件
            {
                if (NextFile.Name == fileName)
                {
                    return true;
                }
                continue;
            }
            return false;
        }
        public ActionResult RUserDetail(int id) 
        {
            int uid = id;
            Models.FJ_User usermodel= fjAppRoot.FJ_User.Where(u => u.id == uid).ToList().FirstOrDefault();
            ViewData["User"] = usermodel;
            return View();
        }
        public ActionResult RUserModify() 
        {
            int id = Convert.ToInt32(Request.Params["uid"]);
            Models.FJ_User oldusermodel = fjAppRoot.FJ_User.Where(u => u.id == id).ToList().FirstOrDefault();
            Models.FJ_UserInfo oldinfo = fjAppRoot.FJ_UserInfo.Where(i => i.UserID == id).ToList().FirstOrDefault();
            string username = Request.Params["uname"].Trim().ToString();
            string pwd = Request.Params["pwd"].Trim().ToString();
            oldusermodel.UserName = username;
            oldusermodel.Pwd = pwd;
            fjAppRoot.SaveChanges();
            MoneyRetard.Content.JsonModel jsmodel = new JsonModel() { msg = "修改成功!" };
            return Json(jsmodel);
        }
        public ActionResult ManageAccount(int id=1)
        {
            var kw = string.Empty;
            int usertype = 0;
            int type = 0;
            int area = 0;
            int start = 2;
            #region ef lamda 错误
            System.Linq.Expressions.Expression<Func<Models.Manage_UserDetail, bool>> wherelamda1 = null;
            System.Linq.Expressions.Expression<Func<Models.Manage_UserDetail, bool>> wherelamda2 = null;
            System.Linq.Expressions.Expression<Func<Models.Manage_UserDetail, bool>> wherelamda3 = null;
            System.Linq.Expressions.Expression<Func<Models.Manage_UserDetail, bool>> wherelamda4 = null;
            System.Linq.Expressions.Expression<Func<Models.Manage_UserDetail, bool>> wherelamda5 = null;
            if (!string.IsNullOrEmpty(Request.QueryString["kw"]))
            {
                kw = Request.QueryString["kw"].ToString(); ;
                wherelamda1 = i => i.UserRealName.Contains(kw);
            }
            if (!string.IsNullOrEmpty(Request.QueryString["usertype"]))
            {
                usertype = Convert.ToInt32(Request.QueryString["usertype"]);
            }
            if (!string.IsNullOrEmpty(Request.QueryString["type"]))
            {
                type = Convert.ToInt32(Request.QueryString["type"]);
            }
            if (!string.IsNullOrEmpty(Request.QueryString["area"]))
            {
                area = Convert.ToInt32(Request.QueryString["area"]);
            }
            if (!string.IsNullOrEmpty(Request.QueryString["start"]))
            {
                start = Convert.ToInt32(Request.QueryString["start"]);
            }
            if (usertype != 0)
            {
                wherelamda2 = u => u.UserType == usertype;
            }
            if (type != 0)
            {
                wherelamda3 = u => u.IsAgency == type;
            }
            if (area != 0)
            {
                wherelamda4 = u => u.Area == area;
            }
            if (start != 2)
            {
                wherelamda5 = u => u.IsStart == start;
            }
            wherelamda1 = wherelamda1 == null ? u => u.id > 0 : wherelamda1;
            wherelamda2 = wherelamda2 == null ? u => u.id > 0 : wherelamda2;
            wherelamda3 = wherelamda3 == null ? u => u.id > 0 : wherelamda3;
            wherelamda4 = wherelamda4 == null ? u => u.id > 0 : wherelamda4;
            wherelamda5 = wherelamda5 == null ? u => u.id > 0 : wherelamda5;
            PagedList<Models.Manage_UserDetail> pagedata = fjAppRoot.FJ_User.Join(fjAppRoot.FJ_UserInfo, u => u.id, i => i.UserID, (u, i) => new Models.Manage_UserDetail() { id = u.id, UserName = u.UserName, Pwd = u.Pwd, IsAgency = u.IsAgency, IsAllocation = u.IsAllocation, IsStart = u.IsStart, CreateTime = u.CreateTime, Limits = u.Limits, UInfoID = i.id, UserType = i.UserType, UserRealName = i.UserRealName, Area = i.Area, Adress = i.Adress, QQ = i.QQ, Phone = i.Phone, LegalPeo = i.LegalPeo, Email = i.Email, Icard_1 = i.Icard_1, Icard_1_Num = i.Icard_1_Num, Icard_2 = i.Icard_2, Icard_2_Num = i.Icard_2_Num }).Where(wherelamda1).Where(wherelamda2).Where(wherelamda3).Where(wherelamda4).Where(wherelamda5).OrderBy(u=>u.id).ToPagedList(id, 3);
            #endregion
            //string strwhere = "where 1=1";
            //if (!string.IsNullOrEmpty(Request.QueryString["kw"]))
            //{
            //    strwhere = strwhere + " AND UI.UserRealName LIKE '%" + Request.QueryString["kw"].ToString() + "%'";
            //}
            //if (!string.IsNullOrEmpty(Request.QueryString["usertype"]))
            //{
            //    usertype = Convert.ToInt32(Request.QueryString["usertype"]);
            //}
            //if (!string.IsNullOrEmpty(Request.QueryString["type"]))
            //{
            //    type = Convert.ToInt32(Request.QueryString["type"]);
            //}
            //if (!string.IsNullOrEmpty(Request.QueryString["area"]))
            //{
            //    area = Convert.ToInt32(Request.QueryString["area"]);
            //}
            //if (!string.IsNullOrEmpty(Request.QueryString["start"]))
            //{
            //    start = Convert.ToInt32(Request.QueryString["start"]);
            //}
            //if (type != 0)
            //{
            //    strwhere = strwhere + " And U.IsAgency=" + type;
            //}
            //if (usertype != 0)
            //{
            //    strwhere = strwhere + " And UI.UserType=" + usertype;
            //}
            //if (area != 0)
            //{
            //    strwhere = strwhere + " And UI.Area=" + area;
            //}
            //if (start != 2)
            //{
            //    strwhere = strwhere + " And U.IsStart=" + start;
            //}
            //List<Models.Manage_UserDetail> userlist = new List<Manage_UserDetail>();
            //DataTable dt = DbHelperSQL.GetDataTable("SELECT U.*,UI.id AS UInfoID,UI.Adress,UI.Area,UI.CreateTime,UI.UserType,UI.UserRealName,UI.Icard_1,UI.Icard_1_Num,UI.Icard_2,UI.Icard_2_Num,UI.LegalPeo,UI.Email,UI.Phone,UI.QQ FROM dbo.FJ_User AS U INNER JOIN  dbo.FJ_UserInfo AS UI ON UI.UserID=U.id " + strwhere);
            //Models.Manage_UserDetail usermodel = null;
            //for (int i = 0; i < dt.Rows.Count; i++)
            //{
            //    usermodel = new Manage_UserDetail() { };
            //    usermodel.id = Convert.ToInt32(dt.Rows[i]["id"]);
            //    usermodel.Pwd = dt.Rows[i]["Pwd"].ToString();
            //    usermodel.UserName = dt.Rows[i]["UserName"].ToString();
            //    usermodel.IsAllocation = Convert.ToBoolean(dt.Rows[i]["IsAllocation"]);
            //    usermodel.IsAgency = Convert.ToInt32(dt.Rows[i]["IsAgency"]);
            //    usermodel.IsStart = Convert.ToInt32(dt.Rows[i]["IsStart"]);
            //    usermodel.CreateTime = Convert.ToDateTime(dt.Rows[i]["CreateTime"]);
            //    usermodel.UInfoID = Convert.ToInt32(dt.Rows[i]["UInfoID"]);
            //    usermodel.UserRealName = dt.Rows[i]["UserRealName"].ToString();
            //    usermodel.UserType = Convert.ToInt32(dt.Rows[i]["UserType"]);
            //    usermodel.Limits = Convert.ToInt32(dt.Rows[i]["Limits"]);
            //    usermodel.LegalPeo = dt.Rows[i]["LegalPeo"] == null ? string.Empty : dt.Rows[i]["LegalPeo"].ToString();
            //    usermodel.Adress = dt.Rows[i]["Adress"].ToString();
            //    usermodel.Area = Convert.ToInt32(dt.Rows[i]["Area"]);
            //    usermodel.Icard_1 = Convert.ToInt32(dt.Rows[i]["Icard_1"]);
            //    usermodel.Icard_1_Num = dt.Rows[i]["Icard_1_Num"].ToString();
            //    usermodel.Icard_2 = Convert.ToInt32(dt.Rows[i]["Icard_2"]);
            //    usermodel.Icard_2_Num = dt.Rows[i]["Icard_2_Num"].ToString();
            //    usermodel.Phone = dt.Rows[i]["Phone"].ToString();
            //    usermodel.Email = dt.Rows[i]["Email"] == null ? string.Empty : dt.Rows[i]["Email"].ToString();
            //    usermodel.QQ = dt.Rows[i]["QQ"] == null ? string.Empty : dt.Rows[i]["QQ"].ToString();
            //    userlist.Add(usermodel);
            //}
            ////PagedList<Models.Manage_UserDetail> pagedata = null;
            ////if (string.IsNullOrEmpty(kw) && usertype == 0 && type == 0)
            ////{
            ////     pagedata = fjAppRoot.FJ_UserInfo.OrderBy(u => u.UserID).ToPagedList(id, 3);
                
            ////}
            ////else
            ////{
            ////    //fjAppRoot.FJ_User.Join(fjAppRoot.FJ_UserInfo, u => u.id, i => i.UserID, (u, i) => new Models.UserInfoDetail { id = u.id, UserName = u.UserName, Pwd = u.Pwd, UserType = i.UserType, UserRealName = i.UserRealName, Area = i.Area, Adress = i.Adress, Phone = i.Phone, Email = i.Email, QQ = i.QQ }).ToList();
            ////    //pagedata = fjAppRoot.FJ_UserInfo.Where(u => u.UserRealName.Contains(kw)&&u.UserType==6).OrderBy(u => u.UserID).ToPagedList(id, 3);
            ////     pagedata = fjAppRoot.FJ_UserInfo.Where(wherelamda).Where(wherelamda2).OrderBy(u => u.UserID).ToPagedList(id, 3);

            ////}
            //PagedList<Models.Manage_UserDetail> pagedata = userlist.ToPagedList(id, 3);
            ViewData["kw"] = string.IsNullOrEmpty(Request.QueryString["kw"]) ? string.Empty : Request.QueryString["kw"].ToString();
            ViewData["usertype"] = string.IsNullOrEmpty(Request.QueryString["usertype"]) ? 0 : Convert.ToInt32(Request.QueryString["usertype"].ToString());
            ViewData["type"] = string.IsNullOrEmpty(Request.QueryString["type"]) ? 0 : Convert.ToInt32(Request.QueryString["type"].ToString());
            ViewData["start"] = string.IsNullOrEmpty(Request.QueryString["start"]) ? 2 : Convert.ToInt32(Request.QueryString["start"].ToString());
            ViewData["area"] = string.IsNullOrEmpty(Request.QueryString["area"]) ? 0 : Convert.ToInt32(Request.QueryString["area"].ToString());
            return View(pagedata);
        }
        public ActionResult GetUserInfo(int id) 
        {
            int uid = id;
            Models.Manage_UserDetail usermodel=fjAppRoot.FJ_User.Where(u=>u.id==uid).Join(fjAppRoot.FJ_UserInfo, u => u.id, i => i.UserID, (u, i) => new Models.Manage_UserDetail() { 
             id=u.id,UserName=u.UserName,Pwd=u.Pwd,IsAgency=u.IsAgency,IsAllocation=u.IsAllocation,IsStart=u.IsStart,UInfoID=i.id, UserRealName=i.UserRealName, UserType=i.UserType, LegalPeo=i.LegalPeo, Area=i.Area, Adress=i.Adress, CreateTime=u.CreateTime, Limits=u.Limits, Email= i.Email, Phone=i.Phone,Icard_1=i.Icard_1, Icard_1_Num=i.Icard_1_Num, Icard_2=i.Icard_2, Icard_2_Num=i.Icard_2_Num,QQ=i.QQ
            }).ToList().FirstOrDefault();
            return Json(usermodel, JsonRequestBehavior.AllowGet);
        }
        public ActionResult DoModify() 
        {
            int id= Convert.ToInt32(Request.Params["uid"].ToString());
            Models.FJ_User oldmodel = fjAppRoot.FJ_User.Where(u => u.id == id).ToList().FirstOrDefault();
            oldmodel.UserName = Request.Params["uname"].Trim().ToString();
            oldmodel.Pwd = Request.Params["upwd"].Trim().ToString();
            oldmodel.IsAgency = Convert.ToInt32(Request.Params["uisagency"]);
            oldmodel.IsStart = Convert.ToInt32(Request.Params["uisstart"]);
            oldmodel.IsAllocation =Request.Params["uisallcation"].ToString()=="0"?false:true;
            oldmodel.Limits = Convert.ToInt32(Request.Params["ulimits"]);
            fjAppRoot.SaveChanges();
            Models.FJ_UserInfo oldinfo = fjAppRoot.FJ_UserInfo.Where(u => u.UserID == id).ToList().FirstOrDefault();
            oldinfo.UserRealName = Request.Params["ucompanyname"].Trim().ToString();
            oldinfo.UserType = Convert.ToInt32(Request.Params["utype"].ToString());
            oldinfo.Phone = Request.Params["ucallnum"].Trim().ToString();
            oldinfo.Area = Convert.ToInt32(Request.Params["uarea"].ToString());
            oldinfo.Adress = Request.Params["uadd"].Trim().ToString();
            string email = string.Empty;
            string qq = string.Empty;
            if (!string.IsNullOrEmpty(Request.Params["uemail"]))
            {
                email = Request.Params["uemail"].Trim().ToString();
            }
            if (!string.IsNullOrEmpty(Request.Params["uqq"]))
            {
                qq = Request.Params["uqq"].Trim().ToString();
            }
            oldinfo.Email = email;
            oldinfo.QQ = qq;
            fjAppRoot.SaveChanges();
            MoneyRetard.Content.JsonModel jsmodel = new JsonModel() {
             msg="修改成功", statu="ok"
            };
            return Json(jsmodel);
        }
        public ActionResult DelUser(int id) 
        {
            Models.FJ_User user = fjAppRoot.FJ_User.Where(u => u.id == id).ToList().FirstOrDefault();
            Models.FJ_UserInfo userinfo = fjAppRoot.FJ_UserInfo.Where(u => u.UserID == id).ToList().FirstOrDefault();
            //删除上传的图片
            if (System.IO.File.Exists(HttpContext.Server.MapPath("/RegisterUpload/" + userinfo.Icard_1_Num)))
            {
                System.IO.File.Delete(HttpContext.Server.MapPath("/RegisterUpload/" + userinfo.Icard_1_Num));
            }
            if (System.IO.File.Exists(HttpContext.Server.MapPath("/RegisterUpload/" + userinfo.Icard_2_Num)))
            {
                System.IO.File.Delete(HttpContext.Server.MapPath("/RegisterUpload/" + userinfo.Icard_2_Num));
            }
            //删除用户信息
            fjAppRoot.FJ_User.Attach(user);
            fjAppRoot.ObjectStateManager.ChangeObjectState(user, EntityState.Deleted);
            fjAppRoot.FJ_UserInfo.Attach(userinfo);
            fjAppRoot.ObjectStateManager.ChangeObjectState(userinfo, EntityState.Deleted);
            fjAppRoot.SaveChanges();
            MoneyRetard.Content.JsonModel jsomodel = new JsonModel() { 
             statu="ok", msg="删除成功"
            };
            return Json(jsomodel);
        }
    }
}
