using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Bottleshop.Api.Models
{
    public class Notice
    {
        public ObjectId Id { get; set; }
        public string Title { get; set; }
        public string Contents { get; set; }
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime Indate { get; set; }
    }
}