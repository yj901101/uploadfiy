using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MoneyRetard.Models
{
    public class UserInfoDetail
    {
        public int id { get; set; }
        public string UserName { get; set; }
        public string Pwd { get; set; }
        public int? UserType { get; set; }
        public string UserRealName { get; set; }
        public int? Area { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Adress { get; set; }
        public string QQ { get; set; }
        public string LinkName { get; set; }
    }
}