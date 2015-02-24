using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Drawing;
using System.Web;



namespace MoneyRetard.Common
{
    /// <summary>
    /// 文件操作类
    /// </summary>
    public static class FileHelper
    {
        #region 1.0 创建 新文件名 +string NewFileName(string strOldFileName)
        /// <summary>
        /// 创建 新文件名
        /// </summary>
        /// <returns></returns>
        public static string NewFileName(string strOldFileName)
        {
            return Guid.NewGuid().ToString() + Path.GetExtension(strOldFileName);
        } 
        #endregion

        #region 2.0 获取图片对应 缩略图 名称 +string GetThumbName(string strImgName)
        /// <summary>
        /// 获取图片对应 缩略图 名称
        /// </summary>
        /// <param name="strImgName"></param>
        /// <returns></returns>
        public static string GetThumbName(string strImgName)
        {
            //1.jpg -> 1_thumb.jpg
            return strImgName.Replace(".", "_thumb.");
        } 
        #endregion

        #region 2.1根据缩略图 获取原图 名称 +string GetOriginalName(string strImgName)
        /// <summary>
        /// 根据缩略图 获取原图 名称
        /// </summary>
        /// <param name="strImgName"></param>
        /// <returns></returns>
        public static string GetOriginalName(string strThunmbImgName)
        {
            //1_thumb.jpg -> 1.jpg
            return strThunmbImgName.Replace("_thumb.", ".");
        }
        #endregion

        #region 3.0 保存图片 并 生成缩略图 +SaveImgAndThumb(System.Web.HttpPostedFile file, string strImgName, string savePath)
        /// <summary>
        /// 3.0 保存图片 并 生成缩略图
        /// </summary>
        /// <param name="file">图片文件对象</param>
        /// <param name="strImgName">图片名</param>
        /// <param name="savePath">保存的文件夹路径</param>
        public static void SaveImgAndThumb(System.Web.HttpPostedFile file, string strImgName, string savePath,float fThumbWidth, float fThumbHeight)
        {
            //3.1获取原图文件流
            using (Image img = Image.FromStream(file.InputStream))
            {
                //3.1.1保存原图
                img.Save(HttpContext.Current.Request.MapPath(savePath + "/" + strImgName));
            }
            //File.Delete(
            //3.2保存 缩略图
            MakeThumbImg(file, strImgName, savePath, fThumbWidth, fThumbHeight);
        } 
        #endregion

        #region 4.0 生成缩略图 +void MakeThumbImg(HttpPostedFile file, string strFileName, string savePath, float fThumbWidth, float fThumbHeight)
        /// <summary>
        /// 生成缩略图
        /// </summary>
        /// <param name="file">文件对象</param>
        /// <param name="strFileName">不带后缀的文件名</param>
        /// <param name="savePath">文件夹路径</param>
        /// <param name="intThumbWidth">缩略图宽</param>
        /// <param name="intThumbHeight">缩略图高</param>
        public static void MakeThumbImg(System.Web.HttpPostedFile file, string strFileName, string savePath, float fThumbWidth, float fThumbHeight)
        {
            //将文件流 转成 图片
            using (Image img = Image.FromStream(file.InputStream))
            {
                #region 计算 缩略图的 宽高
                float imgW = img.Width;//原图 宽
                float imgH = img.Height;//原图 高
                //根据 原图的 高宽比，算出 缩略图的 高（保证缩略图不变形）
                float thumbWidth;
                float thumbHeight;
                //获取第一张绘制图的大小,(比较 原图的宽/缩略图的宽 和 原图的高/缩略图的高) 
                thumbWidth = imgW;
                thumbHeight = imgH;
                //宽大于高或宽等于高（横向矩形或正方）
                if (imgW > imgH || imgW == imgH)
                {
                    //如果宽大于 缩略图 原定的宽度
                    if (imgW > fThumbWidth)
                    {
                        //宽按缩略图宽，高按比例缩放
                        thumbWidth = fThumbWidth;//将 原定的 缩略图宽度 设置为 图片要缩小的宽度
                        thumbHeight = imgH * (fThumbWidth / imgW);
                    }
                }
                //高大于宽（竖图）
                else
                {
                    //如果高大于模版
                    if (imgH > fThumbHeight)
                    {
                        //高按模版，宽按比例缩放
                        thumbHeight = fThumbHeight;
                        thumbWidth = imgW * (fThumbHeight / imgH);
                    }
                } 
                #endregion

                //按照高宽比算出的宽和高来生成缩略图，并保存 
                using (Image imgThumb = new Bitmap((int)thumbWidth, (int)thumbHeight))
                {
                    //创建“画家”对象，并告知在imgThumb上“作画”
                    using (Graphics g = Graphics.FromImage(imgThumb))
                    {
                        //设置缩略图的背景色为透明
                        g.Clear(Color.Transparent);
                        //设置一系列图片质量
                        g.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
                        g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                        g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                        //将大图 画到 小图上
                        g.DrawImage(img, new Rectangle(0, 0, imgThumb.Width, imgThumb.Height), new Rectangle(0, 0, img.Width, img.Height), GraphicsUnit.Pixel);
                    }
                    //保存缩略图
                    imgThumb.Save(HttpContext.Current.Server.MapPath(GetThumbName(savePath + "/" + strFileName)));
                }
            }
        }
        #endregion

        #region 5.0原图上传保存
        public static void SaveImg(System.Web.HttpPostedFile file, string strImgName, string savePath)
        {

            using (Image img = Image.FromStream(file.InputStream))
            {
                img.Save(HttpContext.Current.Request.MapPath(savePath + "/" + strImgName));
            }
        } 
        #endregion
    }
}
