using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;

namespace MoneyRetard.Common
{
    /// <summary>
    /// UploadHandler 的摘要说明
    /// </summary>
    public class UploadHandler : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";
            context.Response.Charset = "utf-8";

            HttpPostedFile file = context.Request.Files["Filedata"];
            string oldName = file.FileName;
            string type = oldName.Substring(oldName.LastIndexOf('.'));
            string uploadPath =
                HttpContext.Current.Server.MapPath("../RegisterUpload") + "\\";
            string strFileName = System.Web.Security.FormsAuthentication.HashPasswordForStoringInConfigFile(Guid.NewGuid() + DateTime.Now.Ticks.ToString(), "MD5") + type;
            if (file != null)
            {
                if (!Directory.Exists(uploadPath))
                {
                    Directory.CreateDirectory(uploadPath);
                }
                file.SaveAs(uploadPath + strFileName);
                //下面这句代码缺少的话，上传成功后上传队列的显示不会自动消失
                //context.Response.Write("1");
                context.Response.Write(strFileName);
            }
            else
            {
                context.Response.Write("0");
            }
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}