using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Bottleshop.Api.Lib;
using Bottleshop.Api.Models;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Bottleshop.Api.Controllers
{
    public class ProductController : BaseController
    {
        //
        // GET: /Product/

        public ActionResult Index(int bc_idx = 2, string pr_name = "", int page = 1)
        {
            int rows = 10;
            int sidx = ((page - 1) * rows) + 1;
            ViewBag.page = page;
            ViewBag.bc_idx = bc_idx;
            ViewBag.pr_name = pr_name;
            ViewBag.bc_name = ProjectUtill.GetCategoryName(bc_idx);

            var filter = FilterDefinition<Product>.Empty;
            if(pr_name != "")
            {
                filter = Builders<Product>.Filter.Eq("IsSale", true) & Builders<Product>.Filter.Eq("CategoryId", bc_idx) & Builders<Product>.Filter.Regex("ProductName", new BsonRegularExpression(pr_name, "i"));
            }
            else
            {
                filter = Builders<Product>.Filter.Eq("IsSale", true) & Builders<Product>.Filter.Eq("CategoryId", bc_idx);
            }

            var sort = Builders<Product>.Sort.Ascending("ProductName");
            MongoPagingResult<Product> result = MongodbHelper.FindPaging<Product>(filter, sort, "Product", sidx, rows);

            double dd = double.Parse(result.Count.ToString()) /  double.Parse(rows.ToString());
            ViewBag.Pages = Math.Ceiling(dd);
            return View(result);
        }



    }
}
