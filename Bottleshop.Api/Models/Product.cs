using System;
using MongoDB.Bson;

namespace Bottleshop.Api.Models
{
    public class Product
    {
        public ObjectId Id { get; set; }
        public int CategoryId { get; set; }
        public string ProductName { get; set; }
        public string ProductCountry { get; set; }
        public string ProductGubun { get; set; }
        public int ProductQty { get; set; }
        public int ProductLiter { get; set; }
        public string ProductCompany { get; set; }
        public float ProductPurchsePrice { get; set; }
        public float ProductSaleMemberPrice { get; set; }
        public float ProductSaleNormalPrice { get; set; }
        public int ProductSaleQty { get; set; }
        public bool IsSale { get; set; }
        public DateTime InDate { get; set; }
        public int MemberQty { get; set; }

    }
}
