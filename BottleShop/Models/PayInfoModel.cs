using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BottleShop
{
    public class PayInfoModel
    {
        public int IDX { get; set; }
        public string USERID { get; set; }

        public string PTYPE { get; set; }

        public float PRICE { get; set; }

        public DateTime SDATE { get; set; }

        public DateTime EDATE { get; set; }

        public string ISUSE { get; set; }

        public DateTime INDT { get; set; }
    }
}