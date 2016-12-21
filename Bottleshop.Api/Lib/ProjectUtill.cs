using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Bottleshop.Api.Lib
{
    public class ProjectUtill
    {
        public static  string GetCategoryName(int idx)
        {
            string txt = "";
            switch (idx)
            {
                case 1:
                    txt = "WINE";
                    break;
                case 2:
                    txt = "BEER";
                    break;
                case 3:
                    txt = "WHISKY";
                    break;
                case 6:
                    txt = "SPIRIT";
                    break;
            }
            return txt;
        }
    }
}