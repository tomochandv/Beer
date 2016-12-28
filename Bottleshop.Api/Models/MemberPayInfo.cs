using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Bottleshop.Api.Models
{
    public class MemberPayInfo
    {
        public ObjectId Id { get; set; }
        public string Uid { get; set; }
        public string Orderid { get; set; }
        public string InicisId { get; set; }
        public string BillingKey { get; set; }
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime InDate { get; set; }
        public bool IsAuth { get; set; }
        public bool BillingUse { get; set; }

        public List<MemberBillingInfo> billList { get; set; }

    }
}