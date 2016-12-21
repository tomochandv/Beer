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
    public class CartController : BaseController
    {
        public ActionResult Cart()
        {
            var filter = Builders<Cart>.Filter.Eq("Uid", AUser().Uid);
            var result = MongodbHelper.Find<Cart>(filter, "Cart");
            return View(result);
        }

    }
}
