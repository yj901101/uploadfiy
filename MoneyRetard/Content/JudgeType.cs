using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MoneyRetard.Models;

namespace MoneyRetard.Content
{
    /// <summary>
    /// 判断所述的类型
    /// </summary>
    public class JudgeType : Controller
    {
        public static List<FJ_BasicField> retType(int? id)
        {
            FlZlEntities fje = new FlZlEntities();
            List<FJ_BasicField> fbf= fje.FJ_BasicField.Where(u => u.PID == id).ToList();
            if (fbf.Count >= 1) {
                return fbf;//返回子数据集
            }
            return null;
        }
    }
}