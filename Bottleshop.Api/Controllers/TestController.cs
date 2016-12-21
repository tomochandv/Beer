using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Bottleshop.Api.Lib;
using Bottleshop.Api.Models;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Bottleshop.Api.Controllers
{
    public class TestController : Controller
    {
        //
        // GET: /Test/

        public JsonResult Insert()
        {
            Category ca = new Category();


            ca.CategoryId = 6;
            ca.Name = "SPIRIT";
            MongodbHelper.InsertOneModel<Category>(ca, "Category");
           
           return Json("", JsonRequestBehavior.AllowGet);
        }


        public JsonResult Select()
        {
            //var filter = Builders<Member>.Filter.Eq("Name", "test");
            var filter = new BsonDocument();
            var document = MongodbHelper.Find<Member>(filter, "Member");//MongodbHelper.FindOne<Member>(filter, "Member"); //collection.Find<Member>(filter).ToList().ToArray();
            return Json(document, JsonRequestBehavior.AllowGet);
        }

        public JsonResult SelectPage()
        {
            var filter = new BsonDocument();
            var sort = Builders<Member>.Sort.Descending("Name");

            MongoPagingResult<Member> result = MongodbHelper.FindPaging<Member>(filter, sort, "Member", 1, 10);

            return Json(result.Result.ToList(), JsonRequestBehavior.AllowGet);
        }

        public JsonResult Update()
        {
            long count = 0;
            List<int> list = new List<int>();
            list.Add(3);
            var filter = Builders<Member>.Filter.Eq("Name", "0");
            var update = Builders<Member>.Update.Set("num", list);
            count = MongodbHelper.Update<Member>(filter, update, "Member");
            return Json(count, JsonRequestBehavior.AllowGet);
        }

        public JsonResult Delete()
        {
            long count = 0;
            var filter = new BsonDocument(); //Builders<Member>.Filter.Eq("Pwd", "1111");
            count = MongodbHelper.Delete<Member>(filter, "Member");

            return Json(count, JsonRequestBehavior.AllowGet);
        }

        

    }
}
