using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MongoDB.Bson;

namespace Bottleshop.Api.Models
{
    public class TodayOrder
    {
        public TodayOrder()
        { }
        public TodayOrder(Product pr, int qty, List<string> orderId)
        {
            Product = pr;
            Qty = qty;
            OrderId = orderId;
        }
        public Product Product { get; set; }
        public int Qty { get; set; }
        public List<string> OrderId { get; set; }
    }
}