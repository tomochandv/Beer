using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Bottleshop.Api.Lib;
using Bottleshop.Api.Models;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;

namespace Bottleshop.Api.Controllers
{
    public class HomeController : BaseController
    {
        public ActionResult Index()
        {
            var filter = new BsonDocument();
            Popup result = MongodbHelper.FindOne<Popup>(filter, "Popup");
            return View(result);
        }

        public ActionResult Popup()
        {
            var filter = new BsonDocument();
            Popup result = MongodbHelper.FindOne<Popup>(filter, "Popup");
            return View(result);
        }

        public ActionResult About()
        {
            return View();
        }
        public ActionResult Mart()
        {
            return View();
        }

    }
}
