using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MoneyRetard.Models;

namespace MoneyRetard.Content
{
    public class CheckAudit : Controller
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="guid">批号</param>
        /// <param name="id">专利编号</param>
        /// <returns></returns>
        public static string check(string guid, int id)
        {
            FlZlEntities fjcheck = new FlZlEntities();
            List<FJ_ExportWord> lfw = (from n1 in fjcheck.FJ_ExportWord
                                       where n1.guid == guid && n1.FileType == 2
                                       select n1).ToList();
            if (lfw.Count <= 0)
            {
                return "<font color=red>未提交</font>";
            }
            else
            {
                List<FJ_AffixUrl> lzi = fjcheck.FJ_AffixUrl.Where(u => u.Zid == id).ToList();//查看审核状态
                if (lzi.Count > 0)
                {
                    return audit(lzi[0].Status);
                }
                else {
                    return audit(1);
                }
            }
        }
        private static string audit(int? i) {
            switch (i) {
                case 0: return "<img src='../images/btn-failed.png'/>";
                case 1: return "<font color=#ff6600>审核中</font>";
                case 2: return "<font color=green>已通过</font>";
                default: return "请联系管理员";
            }
        }
        public static string imgSrc(string guid, int id)//下载按钮的切换
        {
            FlZlEntities fjimg = new FlZlEntities();
            List<FJ_ExportWord> lfw1 = (from n1 in fjimg.FJ_ExportWord
                                       where n1.guid == guid && n1.FileType == 2
                                       select n1).ToList();
            if (lfw1.Count <= 0)
            {
                return "<img src=../images/btn-download-error.png />";
            }
            else {
                List<FJ_AffixUrl> lzi = fjimg.FJ_AffixUrl.Where(u => u.Zid == id).OrderByDescending(u=>u.CreateTime).ToList();//查看专利的审核表单是否可以下载
                if (lzi.Count > 0)
                {
                    switch (lzi[0].Status)
                    {
                        case 2: return "<a href=" + lzi[0].url + "><img src=../images/btn-download.png></a>";
                        default: return "<img src=../images/btn-download-error.png>";
                    }
                }
                else {
                    return "<img src=../images/btn-download-error.png>"; 
                }
            }
        }
        public static string rtnTd(string guid ,int id) { //合并td,去最小id来生成合并td
            FlZlEntities fjTd = new FlZlEntities();
            List<int> li = new List<int>();
            List<FJ_ZlInfo> lfz = fjTd.FJ_ZlInfo.Where(u => u.guid == guid).ToList();
            foreach (FJ_ZlInfo zf in lfz) {
                li.Add(zf.id);
            }
            if (id == isMin(li)) {
                return "<td rowspan=" + lfz.Count + "><img src='../images/btn-upload.png' onclick=uploadFile('" + guid + "')></td>";
            }
            return "";
        }
        private static int isMin(List<int> li) { //id是否数组中的最小值
            int temp = li[0];
            for(int i=1;i<li.Count;i++){
                if (li[i] >= temp) {
                    temp = li[i];
                }
            }
            return temp;
        } 
    }
}