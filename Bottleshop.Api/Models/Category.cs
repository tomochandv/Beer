using MongoDB.Bson;

namespace Bottleshop.Api.Models
{
    public class Category
    {
        public ObjectId Id { get; set; }
        public int CategoryId { get; set; }
        public string Name { get; set; }
    }
}