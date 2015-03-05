using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Drawing;
using System.IO;
namespace MoneyRetard.Controllers
{
    public class UserRegisterController : Controller
    {
        
        // GET: /UserRegister/
        Models.FlZlEntities db = new Models.FlZlEntities();
        #region 注册页面初始化
        public ActionResult Index()
        {
            List<Models.FJ_Select> lfs = db.FJ_Select.Where(u => u.type == 1).ToList();//县区的下拉
            SelectList datatype_sellist = new SelectList(lfs, "id", "name");
            ViewData["lfs"] = datatype_sellist.AsEnumerable();
            return View();
        } 
        #endregion
        #region 非代理机构用户注册
        public ActionResult Register()
        {   
            //判断用户名是否存在
            string request_uname = Request.Params["uname"].Trim().ToString();
            List<Models.FJ_User> userlist = db.FJ_User.Where(u => u.UserName == request_uname).ToList();
            if (userlist.Count > 0)
            {
                string outmsg = ("<script> alert(\"注册失败,该用户名已经存在\");window.location=\"/UserRegister/Index\";</script>").Replace("\r", "").Replace("\n", "");
                return Content(outmsg);
            }
            else
            {
                //处理上传文件，如果文件类型不对或者文件类型大小超出范围，则删除上传的图片
                //System.Text.StringBuilder sbmsg = new System.Text.StringBuilder();
                //List<string> img_list = new List<string>();
                //HttpFileCollectionBase files = Request.Files;
                //for (int i = 0; i < files.Count; i++)
                //{
                //    int index = i + 1;
                //    if (files[i].ContentLength > 0)
                //    {
                //        if (files[i].ContentType.Contains("image/"))
                //        {
                //            if (files[i].ContentLength > 5242880)
                //            {
                //                sbmsg.AppendLine("附件" + index + "大于5MB");
                //            }
                //            else
                //            {
                //                string imgname = MoneyRetard.Common.FileHelper.NewFileName(files[i].FileName);
                //                using (Image img = Image.FromStream(files[i].InputStream))
                //                {
                //                    img.Save(HttpContext.Server.MapPath("/RegisterUpload/" + imgname));
                //                }
                //                img_list.Add(imgname);
                //            }
                //        }
                //        else
                //        {
                //            sbmsg.AppendLine("附件" + index + "请上传图片类型文件");
                //        }
                //    }
                //}
                ////提示信息长度大于0表明上传附件有大小或类型错误，所以将上传文件删除
                //if (sbmsg.Length > 0)
                //{
                //    foreach (string filename in img_list)
                //    {
                //        if (System.IO.File.Exists(HttpContext.Server.MapPath("/RegisterUpload/" + filename)))
                //        {
                //            System.IO.File.Delete(HttpContext.Server.MapPath("/RegisterUpload/" + filename));
                //        }
                //    }
                //    string outmsg = ("<script> alert(\"注册失败," + sbmsg.ToString() + "\");window.location=\"/UserRegister/Index\";</script>").Replace("\r", "").Replace("\n", "");
                //    return Content(outmsg);
                //}
                //else
                //{
                    Models.FJ_User modeluser = new Models.FJ_User();
                    modeluser.UserName = Request.Params["uname"].Trim().ToString();
                    modeluser.Pwd = Request.Params["upwd"].Trim().ToString();
                    modeluser.IsAllocation = false;
                    modeluser.IsAgency = 1;
                    modeluser.IsStart = 0;
                    modeluser.Limits = 1;
                    modeluser.CreateTime = DateTime.Now;
                    db.FJ_User.AddObject(modeluser);
                    db.SaveChanges();
                    try
                    {
                        Models.FJ_UserInfo modeluserinfo = new Models.FJ_UserInfo();
                        modeluserinfo.UserID = modeluser.id;
                        modeluserinfo.UserRealName = Request.Params["companyname"].Trim().ToString();
                        modeluserinfo.UserType = Convert.ToInt32(Request.Params["busType"].ToString());
                        modeluserinfo.Icard_1 = Convert.ToInt32(Request.Params["IC1"].ToString());
                        modeluserinfo.Icard_1_Num = Request.Form["file1"].ToString();
                        modeluserinfo.Icard_2 = Convert.ToInt32(Request.Params["IC2"].ToString());
                        modeluserinfo.Icard_2_Num = Request.Form["file2"].ToString();
                        modeluserinfo.Area = Convert.ToInt32(Request.Params["Area"].ToString());
                        modeluserinfo.Adress = Request.Params["add"].Trim().ToString();
                        modeluserinfo.Phone = Request.Params["callnum"].Trim().ToString();
                        modeluserinfo.LinkName = Request.Params["linkname"].Trim().ToString();
                        modeluserinfo.IsAgency = 1;
                        modeluserinfo.CreateTime = modeluser.CreateTime;
                        if (!string.IsNullOrEmpty(Request.Params["email"]))
                        {
                            modeluserinfo.Email = Request.Params["email"].Trim().ToString();
                        }
                        if (!string.IsNullOrEmpty(Request.Params["qq"]))
                        {
                            modeluserinfo.QQ = Request.Params["qq"].Trim().ToString();
                        }
                        db.FJ_UserInfo.AddObject(modeluserinfo);
                        db.SaveChanges();
                        return Content("<script>alert(\"注册成功,请联系管理员开通账户\");window.location=\"/Login/Index?ty=2\";</script>");
                    }
                    catch {
                        db.DeleteObject(modeluser);
                        db.SaveChanges();
                        return Content("<script>alert(\"注册失败,请确认信息填写\");window.location=\"/UserRegister/Index\";</script>");
                    }
                }
            //}
        } 
        #endregion

    }
}
