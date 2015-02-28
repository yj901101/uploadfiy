using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MoneyRetard.Common
{
    public static class PageHelper
    {
        public static string Zllx(int? n) 
        {
            string zllx = string.Empty;
            switch(n)
            {
                case 1: zllx="发明专利"; break;
                case 2: zllx = "外观专利"; break;
                case 3: zllx = "实用新型"; break;
            }
            return zllx;
        }
        public static string IsAgency(int n,string username)
        {

            if (n == 1)
            {
                return "无";
            }
            else 
            {
                return username;
            }
        }
       public static string  IsAllocation(bool n)
       {
           if (n==true)
           {
               return "有分配权";
           }
           else 
           {
               return "无分配权";
           }
       }
       public static string IsAgency(int n)
       {
           if (n == 1)
           {
               return "非代理机构";
           }
           else 
           {
               return "代理机构";
           }
       }
       public static string IsStart(int n) 
       {
           if (n == 0)
           {
               return "未启用";
           }
           else 
           {
               return "启用";
           }
       }
       public static string Limits(int n) 
       {
           if (n == 0)
           {
               return "超级管理员";
           }
           else 
           {
               return "普通用户";
           }
       }
       public static string Area(int n) 
       {
           string area = string.Empty;
           switch (n) 
           {
               case 8: area="花山区"; break;
               case 9: area = "雨山区"; break;
               case 10: area = "慈湖高新区"; break;
               case 11: area = "博望区"; break;
               case 12: area = "当涂县"; break;
               case 13: area = "含山县"; break;
               case 14: area = "和县"; break;
                   
           }
           return area;
       }
       public static string UserType(int n) 
       {
           string usertype = string.Empty;
           switch (n)
           {
               case 6: usertype = "企业"; break;
               case 7: usertype = "事业"; break;
               case 8: usertype = "高校"; break;
               case 9: usertype = "科研机构"; break;
               case 10: usertype = "机关团体"; break;
               case 11: usertype = "个人"; break;
           }
           return usertype;
       }
    }
}