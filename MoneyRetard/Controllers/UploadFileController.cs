using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using MoneyRetard.Models;
using MoneyRetard.Content;

namespace MoneyRetard.Controllers
{
    public class UploadFileController : Controller
    {
        //
        // GET: /UploadFile/
        FlZlEntities fjApp1 = new FlZlEntities();
        public ActionResult Index(string uploadguid)
        {
            int userid = Convert.ToInt32(Session["userID"]);//获取当前用户
            FJ_User fu = fjApp1.FJ_User.Where(u => u.id == userid).FirstOrDefault();
            ViewData["ag"] = fu.IsAgency;//判断使用是否为代理机构
            ViewData["guid"] = uploadguid;
            return View();
        }
        public void Uploadfiy(HttpPostedFileBase Filedata)
        {
            string forminnerval = Request.QueryString["uploadType"];   
            if (Filedata != null && Filedata.ContentLength > 0)
            {
                if (Filedata != null && Filedata.ContentLength > 0)
                {
                    var fileName = Path.GetFileName(Filedata.FileName);
                    string fileExt = Path.GetExtension(fileName);//接收文件的扩展名
                    if (fileExt == ".pdf" || fileExt == ".PDF")
                    {
                        string timequne = Request.QueryString["timequeue"].ToString();
                        string imageDir = "/zlPDFFiles/";
                        string newFileName ="审批扫描件_" + timequne + "" + fileExt;
                        var path = Path.Combine(Server.MapPath(imageDir), newFileName);
                        Filedata.SaveAs(path);
                        FJ_ExportWord fewu = new FJ_ExportWord();//将提交文件地址添加到表中
                        fewu.creatTime = DateTime.Now;
                        fewu.FileType = 2;
                        fewu.guid = forminnerval;
                        fewu.timeQueue = timequne;
                        fewu.url = ".." + imageDir + newFileName;
                        fjApp1.FJ_ExportWord.AddObject(fewu);
                        fjApp1.SaveChanges();
                    }
                    else
                    {
                        string imageDir = "/imgFile/" + DateTime.Now.Year + "/" + DateTime.Now.Month + "/" + DateTime.Now.Day + "/";
                        Directory.CreateDirectory(Path.GetDirectoryName(Server.MapPath(imageDir)));//创建存储图片的目录;
                        string newFileName = Guid.NewGuid().ToString() + fileExt;
                        var path = Path.Combine(Server.MapPath(imageDir), newFileName);
                        Filedata.SaveAs(path);

                        FJ_ExportWord fewu = new FJ_ExportWord();//将提交文件地址添加到表中
                        fewu.creatTime = DateTime.Now;
                        fewu.FileType = 2;
                        fewu.guid = forminnerval;
                        fewu.timeQueue = Request.QueryString["timequeue"].ToString();
                        fewu.url = ".." + imageDir + newFileName;
                        fjApp1.FJ_ExportWord.AddObject(fewu);
                        fjApp1.SaveChanges();
                    }
                }
            }
        }
        public void Uploadfiy_1(HttpPostedFileBase Filedata)
        {
            string forminnerval = Request.QueryString["uploadType"];
            if (Filedata != null && Filedata.ContentLength > 0)
            {
                var fileName = Path.GetFileName(Filedata.FileName);
                string fileExt = Path.GetExtension(fileName);//接收文件的扩展名
                if (fileExt == ".pdf" || fileExt == ".PDF")
                {
                    string timequne = Request.QueryString["timequeue"].ToString();
                    string imageDir = "/zlBussPDF/";
                    string newFileName = "代理企业营业执照_" + timequne + "" + fileExt;
                    var path = Path.Combine(Server.MapPath(imageDir), newFileName);
                    Filedata.SaveAs(path);
                    FJ_ExportWord fewu = new FJ_ExportWord();//将提交文件地址添加到表中
                    fewu.creatTime = DateTime.Now;
                    fewu.FileType = 3;
                    fewu.guid = forminnerval;
                    fewu.timeQueue = timequne;
                    fewu.url = ".." + imageDir + newFileName;
                    fjApp1.FJ_ExportWord.AddObject(fewu);
                    fjApp1.SaveChanges();
                }
                else
                {
                    string imageDir = "/imgFile/" + DateTime.Now.Year + "/" + DateTime.Now.Month + "/" + DateTime.Now.Day + "/";
                    Directory.CreateDirectory(Path.GetDirectoryName(Server.MapPath(imageDir)));//创建存储图片的目录;
                    string newFileName = Guid.NewGuid().ToString() + fileExt;
                    var path = Path.Combine(Server.MapPath(imageDir), newFileName);
                    Filedata.SaveAs(path);

                    FJ_ExportWord fewu = new FJ_ExportWord();//将提交文件地址添加到表中
                    fewu.creatTime = DateTime.Now;
                    fewu.FileType = 3;
                    fewu.timeQueue = Request.QueryString["timequeue"].ToString();
                    fewu.guid = forminnerval;
                    fewu.url = ".." + imageDir + newFileName;
                    fjApp1.FJ_ExportWord.AddObject(fewu);
                    fjApp1.SaveChanges();
                }
            }
        }
        [HttpPost]
        public ActionResult Upload(HttpPostedFileBase fileField, HttpPostedFileBase fileFielddaili, string forminnerval)
        {
            JsonModel jmupload = new JsonModel();
            if (fileField != null && fileField.ContentLength > 0)
            {
                var fileName = Path.GetFileName(fileField.FileName);
                string fileExt = Path.GetExtension(fileName);//接收文件的扩展名
                if (fileExt == ".doc" || fileExt == ".docx")//限制文件为word
                {
                    string imageDir = "/imgFile/" + DateTime.Now.Year + "/" + DateTime.Now.Month + "/" + DateTime.Now.Day + "/";
                    Directory.CreateDirectory(Path.GetDirectoryName(Server.MapPath(imageDir)));//创建存储图片的目录;
                    string newFileName = Guid.NewGuid().ToString() + fileExt;
                    var path = Path.Combine(Server.MapPath(imageDir), newFileName);
                    fileField.SaveAs(path);

                    FJ_ExportWord fewu = new FJ_ExportWord();//将提交文件地址添加到表中
                    fewu.creatTime = DateTime.Now;
                    fewu.FileType = 2;
                    fewu.guid = forminnerval;
                    fewu.url = ".." + imageDir + newFileName;
                    fjApp1.FJ_ExportWord.AddObject(fewu);
                    fjApp1.SaveChanges();
                    jmupload.statu = "success";
                    jmupload.msg = "上传成功";
                }
                else
                {
                    jmupload.statu = "error";
                    jmupload.msg = "文件格式不符合";
                }
            }
            if (fileFielddaili != null && fileFielddaili.ContentLength > 0)//代理机构证书
            {
                var fileName = Path.GetFileName(fileFielddaili.FileName);
                string fileExt = Path.GetExtension(fileName);//接收文件的扩展名
                if (fileExt == ".doc" || fileExt == ".docx")//限制文件为word
                {
                    string imageDir = "/imgFile/" + DateTime.Now.Year + "/" + DateTime.Now.Month + "/" + DateTime.Now.Day + "/";
                    Directory.CreateDirectory(Path.GetDirectoryName(Server.MapPath(imageDir)));//创建存储图片的目录;
                    string newFileName = Guid.NewGuid().ToString() + fileExt;
                    var path = Path.Combine(Server.MapPath(imageDir), newFileName);
                    fileFielddaili.SaveAs(path);

                    FJ_ExportWord fewu = new FJ_ExportWord();//将提交文件地址添加到表中
                    fewu.creatTime = DateTime.Now;
                    fewu.FileType = 3;//代理机构证书
                    fewu.guid = forminnerval;
                    fewu.url = ".." + imageDir + newFileName;
                    fjApp1.FJ_ExportWord.AddObject(fewu);
                    fjApp1.SaveChanges();
                    jmupload.statu = "success";
                    jmupload.msg = "上传成功";
                }
                else
                {
                    jmupload.statu = "error";
                    jmupload.msg = "文件格式不符合";
                }
            }
            if (jmupload.statu == "error")
            {
                return Json(jmupload);
            }
            else
            {
                return RedirectToAction("../ApplyForm/Person_Zl");
            }
        }
        //专利文件的上传
        [HttpPost]
        public ActionResult UploadFile(HttpPostedFileBase fileField, int forminnerval,int status)
        {
            JsonModel jmzl = new JsonModel();
            jmzl.statu="error";
            jmzl.msg = "数据异常联系管理员";
            if (fileField != null && fileField.ContentLength > 0)
            {
                var fileName = Path.GetFileName(fileField.FileName);
                string fileExt = Path.GetExtension(fileName);//接收文件的扩展名
                if (fileExt == ".doc" || fileExt == ".docx")//限制文件为word
                {
                    string imageDir = "/zlImgFile/" + DateTime.Now.Year + "/" + DateTime.Now.Month + "/" + DateTime.Now.Day + "/";
                    Directory.CreateDirectory(Path.GetDirectoryName(Server.MapPath(imageDir)));//创建存储图片的目录;
                    string newFileName = Guid.NewGuid().ToString() + fileExt;
                    var path = Path.Combine(Server.MapPath(imageDir), newFileName);
                    fileField.SaveAs(path);
                    try
                    {
                        FJ_AffixUrl fau = new FJ_AffixUrl();
                        fau.Zid = forminnerval;
                        fau.url = ".." + imageDir + newFileName;
                        fau.CreateTime = DateTime.Now;
                        fau.Status = status;
                        fjApp1.FJ_AffixUrl.AddObject(fau);
                        fjApp1.SaveChanges();
                    }
                    catch {
                        return Json(jmzl);
                    }
                }
                
            }
            return RedirectToAction("../Root/NotChecked");
        }
    }
}
