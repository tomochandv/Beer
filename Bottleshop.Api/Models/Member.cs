using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Bottleshop.Api.Models
{
    public class Member
    {
        public ObjectId Id { get; set; }
        public string Uid { get; set; }
        public string Name { get; set; }
        public string Pwd { get; set; }
        public string Email { get; set; }
        public string Birth { get; set; }
        public string Tell { get; set; }
        /// <summary>
        /// A : 어드민, U : 일반사용자
        /// </summary>
        public string  MemberType { get; set; }
        public DateTime CreateDate { get; set; }

    }
}