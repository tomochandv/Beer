using System;
using MongoDB.Bson;

namespace Bottleshop.Api.Models
{
    public class Promotion
    {
        public ObjectId Id { get; set; }
        public string Code { get; set; }
        public bool Use { get; set; }
        public DateTime UseDate { get; set; }
        public string Uid { get; set; }
        public bool Send { get; set; }
    }
}