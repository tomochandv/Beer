using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Bottleshop.Api.Lib;
using Bottleshop.Api.Models;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Bottleshop.Api.Views
{
    public class NoticeController : BaseController
    {
        public ActionResult Index(int page = 1)
        {
            int rows = 20;
            int sidx = ((page - 1) * rows) + 1;
            ViewBag.page = page;
            var filter = new BsonDocument();
            var sort = Builders<Notice>.Sort.Descending("Indate");
            MongoPagingResult<Notice> result = MongodbHelper.FindPaging<Notice>(filter, sort, "Notice", sidx, rows);
            double dd = double.Parse(result.Count.ToString()) / double.Parse(rows.ToString());
            ViewBag.Pages = Math.Ceiling(dd);
            return View(result);
        }

        public ActionResult Detail(string idx = "")
        {
            var filter = Builders<Notice>.Filter.Eq("Id", ObjectId.Parse(idx));
            Notice result = MongodbHelper.FindOne<Notice>(filter, "Notice");
            return View(result);
        }

    }
}
