using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MoneyRetard.Content
{
    public class StyleControl
    {
        public static string changeBackage(int id) {
            if (id % 2 == 1)
            {
                return "";
            }
            else {
                return "bgcolor=#E5E5E5";
            }
        }
    }
}