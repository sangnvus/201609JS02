using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MongoDB;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Demo_Cloudinary_Mongo.Models
{
    public class DBContext
    {
        public String connectionString = "mongodb://localhost";
        public String databaseName = "PictureDB";
        public MongoDatabase database;

        public DBContext()
        {
            MongoServerSettings settings = new MongoServerSettings();
            settings.Server = new MongoServerAddress("localhost", 27017);
            MongoServer server = new MongoServer(settings);
            database = server.GetDatabase(databaseName);
        }

        public MongoCollection<MongoPhoto> Photos      //KO HIEU
        {
            get
            {
                var photos = database.GetCollection<MongoPhoto>("Photos");
                return photos;
            }
        }
    }
}