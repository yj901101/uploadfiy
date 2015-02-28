using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MoneyRetard.Models
{
    public class printmodel
    {
        public int id { get; set; }
        public string UserRealName { get; set; }
        public int? IsAgency { get; set; }
        public string LegalPeo { get; set; }
        public string Adress { get; set; }
        public int? Zl_Type { get; set; }
        public string Zl_Name { get; set; }
        public string ZL_Bnum { get; set; }
    }
}