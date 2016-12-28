using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Bottleshop.Api.Models
{
    public class Promotion
    {
        public ObjectId Id { get; set; }
        public string PromotionCode { get; set; }
        public bool Use { get; set; }
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime UseDate { get; set; }
        public string Uid { get; set; }
        public bool Send { get; set; }
    }
}