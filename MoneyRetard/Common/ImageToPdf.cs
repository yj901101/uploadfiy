using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Drawing.Imaging;
using System.IO;
using System.Drawing;
using System.Drawing.Drawing2D;
using iTextSharp.text;
using iTextSharp.text.pdf;

namespace MoneyRetard.Common
{
    public class ImageToPdf
    {
        //固定高宽，便于后续处理
        const int WWidth = 600;
        const int HHeight = 800;
        List<System.Drawing.Image> AllName = new List<System.Drawing.Image>();
        //It is the construction function;
        public ImageToPdf()
        {
            //if (File.Exists(FileName))
            //{
            //    File.Delete(FileName);
            //}
        }
        public void TurnTheImageToPdf(ref List<MemoryStream> SourceImage,string FileName)
        {
            ChangeTheImageToS(ref SourceImage);
            Document document = new Document();
            document.SetPageSize(new iTextSharp.text.Rectangle(WWidth + 72f, HHeight + 72f));
            PdfWriter write = PdfWriter.GetInstance(document, new FileStream(FileName, FileMode.OpenOrCreate, FileAccess.Write));
            document.Open();
            iTextSharp.text.Image jpg;
            for (int i = 0; i < AllName.Count; ++i)
            {
                jpg = iTextSharp.text.Image.GetInstance(AllName[i], ImageFormat.Jpeg);
                document.NewPage();
                document.Add(jpg);
            }
            if (document != null && document.IsOpen())
            {
                document.Close();
            }
            if (write != null)
            {
                write.Close();
            }
        }
        //change all the image to standard
        private void ChangeTheImageToS(ref List<MemoryStream> ImageName)
        {
            for (int i = 0; i < ImageName.Count; ++i)
            {
                Bitmap src = new Bitmap(ImageName[i]);
                Bitmap bmImage = new Bitmap(WWidth, HHeight);
                Graphics g = Graphics.FromImage(bmImage);
                g.InterpolationMode = InterpolationMode.Low;
                g.DrawImage(src, new System.Drawing.Rectangle(0, 0, bmImage.Width, bmImage.Height), new System.Drawing.Rectangle(0, 0, src.Width, src.Height), GraphicsUnit.Pixel);
                g.Dispose();
                AllName.Add(bmImage);
            }
        }
    }
}