using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BottleShop
{
    public class ProductModel
    {
        public int PR_IDX { get; set; }
        public string PR_NAME { get; set; }
        public string PR_COUNTRY { get; set; }
        public string PR_GUBUN { get; set; }
        public int PR_QTY { get; set; }
        public int PR_LITER { get; set; }
        public int SALE_QTY { get; set; }
        public string PR_INCOME { get; set; }
        public float PR_IN_PRICE { get; set; }
        public float PR_PRICE { get; set; }
        public float PR_SALE { get; set; }
        public float PR_NOMEM_SALE { get; set; }
        public string ISSALE { get; set; }
        public float PR_WEIGHT { get; set; }
        public string PE_DESC { get; set; }
        public string CATE_NM { get; set; }
        public DateTime INDT { get; set; }
    }
}