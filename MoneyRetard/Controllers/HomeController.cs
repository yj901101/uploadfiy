using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MoneyRetard.Content;
using Microsoft.Office.Interop.Word;
using MoneyRetard.Models;

namespace MoneyRetard.Controllers
{
    public class HomeController : Controller
    {
        //
        // GET: /Home/
        FlZlEntities fj = new FlZlEntities();
        public ActionResult Index()
        {
            WriteWord();
            return View();
        }
        public void WriteWord() {
            Report report = new Report();
            string s = Server.MapPath("/wordFiles/f7.dot");
            report.CreateNewDocument(s); //模板路径
            //report.InsertValue("Bookmark_value", "世界杯");//在书签“Bookmark_value”处插入值
            List<FJ_BasicField> fb = fj.FJ_BasicField.ToList();
            Table table = report.InsertTable("boottable", 2, 4, 0);//在书签“Bookmark_table”处插入2行3列行宽最大的表 
            report.AddRow(1, 5); //给模板中第一个表格插入2行
            report.MergeCell(table, 1, 1, 1, 4);//表名,开始行号,开始列号,结束行号,结束列号,
            report.AddRow(table); //表名
            report.InsertCell(table, 1, 1, "马鞍山知识产权局");//表名,行号,列号,值
            report.InsertCell(table, 2, 1, "R2C1");//表名,行号,列号,值
            report.InsertCell(table, 2, 2, "hehe");//表名,行号,列号,值
            report.InsertCell(table, 3, 1, "程序员");//表名,行号,列号,值
            report.MergeCell(table, 3, 2, 3, 4);
            report.SetParagraph_Table(table, 0, 0);//水平方向左对齐，垂直方向居中对齐
            report.SetFont_Table(table, "宋体", 12);//宋体9磅
            report.AddRow(1); //给模板中第一个表格添加一行
            report.UseBorder(1, true); //模板中第一个表格使用实线边框
            
            string[] values = { "英超", "意甲", "德甲", "西甲", "法甲" };
            //report.InsertCell(1, 2, 5, values); //给模板中第一个表格的第二行的5列分别插入数据
            //string picturePath = @"C:\Documents and Settings\Administrator\桌面\1.jpg"; 
            //report.InsertPicture("Bookmark_picture", picturePath, 150, 150); //书签位置，图片路径，图片宽度，
            string text = "长期从事电脑操作者，应多吃一些新鲜的蔬菜和水果，同时增加维生素A、B1、C、E的摄入。为预防角膜干燥、眼干涩、视力下降、甚至出现夜盲等，电 脑操作者应多吃富含维生素A的食物，如豆制品、鱼、牛奶、核桃、青菜、大白菜、空心菜、西红柿及新鲜水果等。"; 
            //report.InsertText("Bookmark_text", text);
            report.InsertCell(table, 3, 2, text);
            report.SaveDocument(Server.MapPath("/wordFiles/f7end.doc")); //文档路径
        }
    }
}
