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
    public class CartController : BaseController
    {
        public ActionResult Cart()
        {
            var filter = Builders<Cart>.Filter.Eq("Uid", AUser().Uid);
            var result = MongodbHelper.Find<Cart>(filter, "Cart");
            return View(result);
        }

        public JsonResult Add(string pr_idx)
        {
            int count = 0;
            try
            {
                var filter = Builders<Product>.Filter.Eq("Id", ObjectId.Parse(pr_idx));
                var result = MongodbHelper.FindOne<Product>(filter, "Product");

                Cart cart = new Cart();
                cart.Uid = AUser().Uid;
                cart.Product = result;
                cart.Qty = 1;

                MongodbHelper.InsertOneModel<Cart>(cart, "Cart");
                count = 1;
            }
            catch(Exception ex)
            {
                new Log().Error(ex);
            }
            return Json(count);
        }

        public JsonResult Delete(string pr_idx)
        {
            int count = 0;
            try
            {
                var filter = Builders<Cart>.Filter.Eq("Id", ObjectId.Parse(pr_idx)) & Builders<Cart>.Filter.Eq("Uid", AUser().Uid);
                MongodbHelper.Delete<Cart>(filter, "Cart");
                count = 1;
            }
            catch (Exception ex)
            {
                new Log().Error(ex);
            }
            return Json(count);
        }

        public RedirectResult Order()
        {
            string[] arrQty = Request["qty"].Split(',');
            string[] arrIdx = Request["pr_idx"].Split(',');
            
            if (arrIdx.Length  == arrQty.Length)
            {
                ObjectId[] arrObjectId = new ObjectId[arrIdx.Length];
                for(int i=0; i < arrIdx.Length; i++)
                {
                    arrObjectId[i] = ObjectId.Parse(arrIdx[i]);
                }
                var builder = Builders<Product>.Filter.In("Id", arrObjectId);
                var producList = MongodbHelper.Find<Product>(builder, "Product");

                List<ProductOrder> listProductOrder = new List<ProductOrder>();
                float totalAmount = 0;
                ObjectId orderId = ObjectId.GenerateNewId();

                for (int i = 0; i < arrIdx.Length; i++)
                {
                    ProductOrder data = new ProductOrder();
                    data.OrderId = orderId;
                    data.Product = producList.Where(x => x.Id == ObjectId.Parse(arrIdx[i])).ToList()[0];
                    data.Price = producList.Where(x => x.Id == ObjectId.Parse(arrIdx[i])).ToList()[0].ProductSaleMemberPrice;
                    data.Qty = DataType.Int(arrQty[i]);
                    totalAmount += data.Price * data.Qty;
                    listProductOrder.Add(data);
                }

                Order order = new Order();
                order.Id = orderId;
                order.OrderStatus = 0;
                order.Product = listProductOrder;
                order.TotalAmount = totalAmount;
                order.Uid = AUser().Uid;
                order.OrderDate = DateTime.Now.ToLocalTime();
                

                MongodbHelper.InsertOneModel<Order>(order, "Order");
                MongodbHelper.Delete<Cart>(Builders<Cart>.Filter.Eq("Uid", AUser().Uid), "Cart");
            }
            return Redirect(Url.Action("Order", "My"));
        }

    }
}
