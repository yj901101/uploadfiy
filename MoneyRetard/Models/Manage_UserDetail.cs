using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MoneyRetard.Models
{
    public class Manage_UserDetail
    {
        public int id { get; set; }
        public int UInfoID { get; set; }
        public string UserName { get; set; }
        public string Pwd { get; set; }
        public bool? IsAllocation { get; set; }
        public int? IsAgency { get; set; }
        public int? Limits { get; set; }
        public DateTime? CreateTime { get; set; }
        public int? IsStart { get; set; }
        public int? UserType { get; set; }
        public string UserRealName { get; set; }
        public string Icard_1 { get; set; }
        public string Icard_1_Num { get; set; }
        public string Icard_2 { get; set; }
        public string Icard_2_Num { get; set; }
        public int? Area { get; set; }
        public string LegalPeo { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Adress { get; set; }
        public string QQ { get; set; }
        public string mustNum { get; set; }
    }
}