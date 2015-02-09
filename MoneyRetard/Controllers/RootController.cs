using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MoneyRetard.Content;
using MoneyRetard.Models;
using System.IO;

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
            PagedList<personDatabaseByReal> pagerList;
            try
            {
                pagerList = new PagedList<personDatabaseByReal>(relationpeo, pageIndex - 1, pageSize);
            }
            catch
            {
                pagerList = new PagedList<personDatabaseByReal>(relationpeo, 0, pageSize);
            }
            ViewData["Total"] = relationpeo.Count();
            ViewData["TotalPager"] = pagerList.TotalPages;
            ViewData["rpeo"] = pagerList.ToList();
            return View();
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
            PagedList<Checked> pagerList;
            try
            {
                pagerList = new PagedList<Checked>(relationpeo, pageIndex - 1, pageSize);
            }
            catch
            {
                pagerList = new PagedList<Checked>(relationpeo, 0, pageSize);
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
            PagedList<NotChecked> pagerList;
            try
            {
                pagerList = new PagedList<NotChecked>(relationpeo, pageIndex - 1, pageSize);
            }
            catch
            {
                pagerList = new PagedList<NotChecked>(relationpeo, 0, pageSize);
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
            PagedList<NotSumbit> pagerList;
            try
            {
                pagerList = new PagedList<NotSumbit>(relationpeo, pageIndex - 1, pageSize);
            }
            catch
            {
                pagerList = new PagedList<NotSumbit>(relationpeo, 0, pageSize);
            }
            ViewData["Total"] = relationpeo.Count();
            ViewData["TotalPager"] = pagerList.TotalPages;
            ViewData["rpeo"] = pagerList.ToList();
            return View();
        }
        public ActionResult updatePwa() {//修改密码

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
            ViewData["word"] = lfew[0].url;//最新提交的数据
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
    }
}
