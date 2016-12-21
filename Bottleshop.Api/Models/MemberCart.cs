using System.Collections.Generic;
using MongoDB.Bson;

namespace Bottleshop.Api.Models
{
    public class MemberCart
    {
        public ObjectId Id { get; set; }
        public string Uid { get; set; }
        public List<Product> products { get; set; }
    }
}