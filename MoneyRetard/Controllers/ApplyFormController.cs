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
            ViewData["deptname"] = fu.UserRealName;//申请单位
            ViewData["LinkPeo"] = fu.LegalPeo;//联系人
            ViewData["Adress"] = fu.Adress;//地址
            ViewData["UserType"] = fu.UserType;//企业性质
            ViewData["Phone"] = fu.Phone;//电话
            return View();
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
            const int pageSize = 10;
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
        private string getType(int num) {
            switch (num) {
                case 1: return "发明专利";
                case 2: return "外观专利";
                default: return "实用新型";
            }
        } 
    }
}
