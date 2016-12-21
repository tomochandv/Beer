using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;

namespace Bottleshop.Api.Lib
{
    public class MongodbHelper
    {
        private static MongoClient client = new MongoClient("mongodb://localhost:27017");
        private static IMongoDatabase database = client.GetDatabase("thebottleshop");
        /// <summary>
        /// Insert One
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="instance"></param>
        /// <param name="collectionName"></param>
        public static void InsertOneModel<T>(T instance, string collectionName) where T : class
        {
            try
            {
                var collection = database.GetCollection<T>(collectionName);
                collection.InsertOne(instance);
            }
            catch(Exception ex)
            {
                new Log().Error(ex);
            }
        }

        public static void InsertManyModel<T>(List<T> instance, string collectionName) where T : class
        {
            try
            {
                var collection = database.GetCollection<T>(collectionName);
                collection.InsertMany(instance);
            }
            catch (Exception ex)
            {
                new Log().Error(ex);
            }
        }

        /// <summary>
        /// Update 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="filter"></param>
        /// <param name="update"></param>
        /// <param name="collectionName"></param>
        /// <returns></returns>
        public static long Update<T>(FilterDefinition<T> filter, UpdateDefinition<T> update, string collectionName) where T : class
        {
            long count = 0;
            var collection = database.GetCollection<T>(collectionName);
            var result = collection.UpdateMany(filter, update);
            if (result.IsModifiedCountAvailable)
            {
                count = result.ModifiedCount;
            }
            return count;
        }

        /// <summary>
        /// Delete
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="filter"></param>
        /// <param name="collectionName"></param>
        /// <returns></returns>
        public static long Delete<T>(FilterDefinition<T> filter, string collectionName) where T : class
        {
            long count = 0;
            var collection = database.GetCollection<T>(collectionName);
            var result = collection.DeleteMany(filter);
            count = result.DeletedCount;
            return count;
        }

        public static T FindOne<T>(FilterDefinition<T> filter, string collectionName) where T : class
        {
            var collection = database.GetCollection<T>(collectionName);
            var document = collection.Find(filter);
            if(document.ToList().Count > 0)
            {
               return  collection.Find(filter).First();
            }
            else
            {
                return null;
            }
        }

        public static List<T> Find<T>(FilterDefinition<T> filter, string collectionName) where T : class
        {
           
            var collection = database.GetCollection<T>(collectionName);
            var document = collection.Find<T>(filter).ToList();
            return document;
        }

        public static List<T> FindSort<T>(FilterDefinition<T> filter, SortDefinition<T> sort, string collectionName) where T : class
        {
            var collection = database.GetCollection<T>(collectionName);
            var document = collection.Find<T>(filter).Sort(sort).ToList();
            return document;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="filter"></param>
        /// <param name="sort"></param>
        /// <param name="collectionName"></param>
        /// <param name="start"></param>
        /// <param name="end">Rows</param>
        /// <returns></returns>
        public static MongoPagingResult<T> FindPaging<T>(FilterDefinition<T> filter, SortDefinition<T> sort, string collectionName, int start, int rows) where T : class
        {
            MongoPagingResult<T> result = new MongoPagingResult<T>();

            var collection = database.GetCollection<T>(collectionName);
            var document = collection.Find<T>(filter).Sort(sort).ToList();
            result.Count = document.Count();

            result.Result = collection.Find<T>(filter).Sort(sort)
                             .Skip(start)
                             .Limit(rows)
                             .ToList();

            return result;
        }
    }

    public class MongoPagingResult<T>
    {
        public long Count { get; set; }
        public List<T> Result { get; set; }
    }
}