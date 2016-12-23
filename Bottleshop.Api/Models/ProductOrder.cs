using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MongoDB.Bson;

namespace Bottleshop.Api.Models
{
    public class ProductOrder
    {
        public ObjectId Id { get; set; }
        public ObjectId OrderId { get; set; }
        public Product Product { get; set; }
        public int Qty { get; set; }
        public float Price { get; set; }
    }
}