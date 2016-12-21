using MongoDB.Bson;

namespace Bottleshop.Api.Models
{
    public class Cart
    {
        public ObjectId Id { get; set; }
        public Product Product { get; set; }
        public string Uid { get; set; }
        public int Qty { get; set; }
    }
}