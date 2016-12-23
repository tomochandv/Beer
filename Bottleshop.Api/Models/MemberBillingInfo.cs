using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MongoDB.Bson;

namespace Bottleshop.Api.Models
{
    public class MemberBillingInfo
    {
        public ObjectId Id { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Orderid { get; set; }
        public string InicisId { get; set; }
        public string BillingKey { get; set; }
        public DateTime InDate { get; set; }
        public string ResultCode { get; set; }
        public string ResultMsg { get; set; }
        public string PgauthDate { get; set; }
        public string PgauthTime { get; set; }
        /// <summary>
        /// 빌링타입 S:결제, C: 취소, P:프로모션, A: 현장결제
        /// </summary>
        public string BillType { get; set; }
        public bool Use { get; set; }
    }
}