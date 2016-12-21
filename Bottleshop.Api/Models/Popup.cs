using System;
using MongoDB.Bson;


namespace Bottleshop.Api.Models
{
    public class Popup
    {
        public ObjectId Id { get; set; }
        public string Title { get; set; }
        public string Contents { get; set; }
        public bool UseYn { get; set; }
    }
}