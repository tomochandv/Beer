using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Bottleshop.Api.Models
{
    public class Order
    {
        public ObjectId Id { get; set; }
        public List<ProductOrder> Product { get; set; }
        public string Uid { get; set; }
        public float TotalAmount { get; set; }
        /// <summary>
        /// 0:주문완료 1: 준비중 2: 준비완료 3:판매완료 4: 재고 없음
        /// </summary>
        public int OrderStatus { get; set; }
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime OrderDate { get; set; }
    }
}