using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Bottleshop.Api.Lib;
using Bottleshop.Api.Models;
using MongoDB.Driver;

namespace Bottleshop.Api.Controllers
{
    public class MyController : BaseController
    {
        //
        // GET: /My/

        public ActionResult Info()
        {
            var filter = Builders<MemberPayInfo>.Filter.Eq("Uid", AUser().Uid);
            MemberPayInfo info = MongodbHelper.FindOne<MemberPayInfo>(filter, "MemberPayInfo");
            return View(info);
        }

        public ActionResult Order()
        {
            var filter = Builders<Order>.Filter.Eq("Uid", AUser().Uid);
            var result = MongodbHelper.Find<Order>(filter, "Order");
            return View(result);
        }

        public JsonResult GetMyInfo()
        {
            var filter = Builders<Member>.Filter.Eq("Uid", AUser().Uid);
            Member info = MongodbHelper.FindOne<Member>(filter, "Member");
            return Json(info);
        }
    }
}
