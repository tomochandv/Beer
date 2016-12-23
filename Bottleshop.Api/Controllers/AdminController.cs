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
    public class AdminController : AdminBaseController
    {
        //
        // GET: /Admin/

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Product(int bc_idx = 0, string pr_name = "", int page = 1, string isSale = "")
        {
            int rows = 20;
            int sidx = ((page - 1) * rows);
            ViewBag.page = page;
            ViewBag.bc_idx = bc_idx;
            ViewBag.pr_name = pr_name;
            ViewBag.isSale = isSale;
            ViewBag.bc_name = ProjectUtill.GetCategoryName(bc_idx);

            var filter = FilterDefinition<Product>.Empty;
            if (bc_idx != 0)
            {
                filter = filter & Builders<Product>.Filter.Eq("bc_idx", DataType.Int(bc_idx));
            }
            if (pr_name != "")
            {
                filter = filter & Builders<Product>.Filter.Eq("ProductName", pr_name);
            }
            if (isSale != "")
            {
                filter = filter & Builders<Product>.Filter.Eq("IsSale", isSale == "Y" ? true: false);
            }

            var sort = Builders<Product>.Sort.Ascending("ProductName");
            MongoPagingResult<Product> result = MongodbHelper.FindPaging<Product>(filter, sort, "Product", sidx, rows);

            double dd = double.Parse(result.Count.ToString()) / double.Parse(rows.ToString());
            ViewBag.Pages = Math.Ceiling(dd);
            return View(result);
        }

    }
}
