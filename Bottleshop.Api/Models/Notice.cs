using System;
using MongoDB.Bson;

namespace Bottleshop.Api.Models
{
    public class Notice
    {
        public ObjectId Id { get; set; }
        public string Title { get; set; }
        public string Contents { get; set; }
        public DateTime Indate { get; set; }
    }
}