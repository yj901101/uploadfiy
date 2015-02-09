using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.Office.Interop.Word;

namespace WordMark.Content
{
    public class WriteIntoWord
    {
         ApplicationClass app = null;   //定义应用程序对象 
         Document doc = null;   //定义 word 文档对象
         Object missing = System.Reflection.Missing.Value; //定义空变量
         Object isReadOnly = false;
        // 向 word 文档写入数据
        public void OpenDocument(string FilePath) 
        {   
            object filePath =  FilePath;//文档路径
            app = new ApplicationClass(); //打开文档 
            doc = app.Documents.Open(ref filePath, ref missing, ref missing, ref missing, 
            ref missing, ref missing, ref missing, ref missing); 
            doc.Activate();//激活文档
        } 
        /// <summary> 
        /// </summary> 
        ///<param name="parLableName">域标签</param> 
        /// <param name="parFillName">写入域中的内容</param> 
        /// 
        //打开word，将对应数据写入word里对应书签域
        public void WriteIntoDocument(string BookmarkName, string FillName)
        {
             object bookmarkName = BookmarkName;
             Bookmark bm = doc.Bookmarks.get_Item(ref bookmarkName);//返回书签
             //try
             //{
             //    int i = int.Parse(FillName);
             //    object start = 65;
             //    object end = 66;
             //    Microsoft.Office.Interop.Word.Range range = doc.Range(ref start, ref end);
             //    Microsoft.Office.Interop.Word.WdFieldType type = Microsoft.Office.Interop.Word.WdFieldType.wdFieldFormCheckBox;
             //    doc.FormFields.Add(range, type).CheckBox.Value = true;
             //}
             //catch
             //{
                 bm.Range.Text = FillName;//设置书签域的内容}
             //}
        } 
        /// <summary> 
         /// 保存并关闭 
        /// </summary> 
        /// <param name="parSaveDocPath">文档另存为的路径</param>
         /// 
        public void  Save_CloseDocument(string  SaveDocPath) 
        { 
            object  savePath = SaveDocPath;  //文档另存为的路径 
            Object saveChanges = app.Options.BackgroundSave;//文档另存为 
            doc.SaveAs(ref savePath, ref missing, ref missing, ref missing, ref missing,
            ref missing, ref missing, ref missing);
            doc.Close(ref saveChanges, ref missing, ref missing);//关闭文档
            app.Quit(ref missing, ref missing, ref missing);//关闭应用程序       
        } 
    }
}