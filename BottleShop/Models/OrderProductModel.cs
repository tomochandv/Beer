using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BottleShop
{
    public class OrderProductModel
    {
        public int OR_IDX { get; set; }
        public int PR_IDX { get; set; }
        public int QTY { get; set; }
        public string MESSAGE { get; set; }
        public string PR_NAME { get; set; }
        public string CATE_NAME { get; set; }
        public float PRICE { get; set; }
    }
}