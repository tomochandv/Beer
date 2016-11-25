using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BottleShop
{
    public class CurrentPrModel
    {
        public int QTY { get; set; }
        public string PR_NAME { get; set; }
        public List<int> OR_IDX { get; set; }
    }
}