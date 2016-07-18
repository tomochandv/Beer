using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BottleShop
{
    public class OrderInfoModel
    {
        public int OR_IDX { get; set; }
        public DateTime INDT { get; set; }
        public DateTime END_DATE { get; set; }
        public string OR_STATUS { get; set; }
        public string USERID { get; set; }
        public float TOTAL_PRICE { get; set; }
        public string NAME { get; set; }
        public string TELL { get; set; }
        public string EMAIL { get; set; }
        public List<OrderProductModel> ProductList { get; set; }
    }
}