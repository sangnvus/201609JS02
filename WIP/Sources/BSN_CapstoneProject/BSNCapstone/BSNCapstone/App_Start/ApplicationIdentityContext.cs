using AspNet.Identity.MongoDB;
using BSNCapstone.Models;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace BSNCapstone.App_Start
{
    public class ApplicationIdentityContext : IDisposable
    {
        public static ApplicationIdentityContext Create()
        {

            // todo add settings where appropriate to switch server & database in your own application
            // Khởi tạo Context để sử dụng bên Controller hoặc bất cứ đâu để tương tác vs DB
            var client = new MongoClient("mongodb://localhost:27017");

            var database = client.GetDatabase("mydb");

            var users = database.GetCollection<ApplicationUser>("users");

            var roles = database.GetCollection<IdentityRole>("roles");

            var categories = database.GetCollection<Category>("categories");

            var books = database.GetCollection<Book>("books");

            //HuyenPT. Create. Start
            var photos = database.GetCollection<MongoPhoto>("photos");
            //HuyenPT. Create. End

            //HuyenPT. Update. Start
            //return new ApplicationIdentityContext(users, roles, categories, books);
            return new ApplicationIdentityContext(users, roles, categories, books, photos);
            //HuyenPT. Update. End

        }

        // Khai báo Context cho từng Object 
        //HuyenPT. Update. Start
        /* private ApplicationIdentityContext(IMongoCollection<ApplicationUser> users,
            IMongoCollection<IdentityRole> roles,
            IMongoCollection<Category> categories,
            IMongoCollection<Book> books) */
        private ApplicationIdentityContext(IMongoCollection<ApplicationUser> users,
            IMongoCollection<IdentityRole> roles,
            IMongoCollection<Category> categories,
            IMongoCollection<Book> books,
            IMongoCollection<MongoPhoto> photos)
        //HuyenPT. Update. End
        {
            Users = users;
            Roles = roles;
            Categories = categories;
            Books = books;
            //HuyenPT. Create. Start
            Photos = photos;
            //HuyenPT. Create. End
        }

        // Bổ sung IMongoCollection cho 1 Object mới
        public IMongoCollection<IdentityRole> Roles { get; set; }
        public IMongoCollection<ApplicationUser> Users { get; set; }
        public IMongoCollection<Category> Categories { get; set; }
        public IMongoCollection<Book> Books { get; set; }
        //HuyenPT. Create. Start
        public IMongoCollection<MongoPhoto> Photos { get; set; }
        //HuyenPT. Create. End

        public Task<List<IdentityRole>> AllRolesAsync()
        {
            return Roles.Find(r => true).ToListAsync();
        }

        public void Dispose()
        {

        }

    }
}