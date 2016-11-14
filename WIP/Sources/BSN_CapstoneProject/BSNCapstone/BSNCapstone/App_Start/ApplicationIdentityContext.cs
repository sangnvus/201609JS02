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

            //HuyenPT. Create. Start. 30-10-2016
            var sliders = database.GetCollection<Slider>("slides");
            //HuyenPT. Create. End. 30-10-2016

            //HuyenPT. Create. Start. 02-11-2016
            var publishers = database.GetCollection<Publisher>("publishers");
            //HuyenPT. Create. End. 02-11-2016

            //HuyenPT. Create. Start. 08-11-2016
            var posts = database.GetCollection<Post>("posts");
            //HuyenPT. Create. End. 08-11-2016
            //HuyenPT. Create. Start. 10-11-2016
            var comments = database.GetCollection<Comment>("comments");
            var userprofiles = database.GetCollection<UserProfile>("userprofiles");
            //HuyenPT. Create. End. 10-11-2016

            //DangVH. Create. Start (08/11/2016)
            var groups = database.GetCollection<Group>("groups");
            //DangVH. Create. End (08/11/2016)

            //HuyenPT. Update. Start. 14-11-2016
            //return new ApplicationIdentityContext(users, roles, categories, books, sliders, publishers, posts, groups);
            return new ApplicationIdentityContext(users, roles, categories, books, sliders, publishers, groups, posts, comments, userprofiles);
            //HuyenPT. Update. End. 14-11-2016

        }

        // Khai báo Context cho từng Object 
        //HuyenPT. Update. Start. 14-11-2016
        /*private ApplicationIdentityContext(IMongoCollection<ApplicationUser> users,
           IMongoCollection<IdentityRole> roles,
           IMongoCollection<Category> categories,
           IMongoCollection<Book> books,
           IMongoCollection<Slider> sliders,
           IMongoCollection<Publisher> publishers,
           IMongoCollection<Post> posts,
           IMongoCollection<Group> groups
        )*/
        private ApplicationIdentityContext(IMongoCollection<ApplicationUser> users,
           IMongoCollection<IdentityRole> roles,
           IMongoCollection<Category> categories,
           IMongoCollection<Book> books,
           IMongoCollection<Slider> sliders,
           IMongoCollection<Publisher> publishers,
           IMongoCollection<Group> groups,
           IMongoCollection<Post> posts,
           IMongoCollection<Comment> comments,
           IMongoCollection<UserProfile> userprofiles
        )
        //HuyenPT. Update. End. 14-11-2016
        {
            Users = users;
            Roles = roles;
            Categories = categories;
            Books = books;
            //HuyenPT. Create. Start. 30-10-2016
            Sliders = sliders;
            //HuyenPT. Create. End. 30-10-2016
            //HuyenPT. Create. Start02-11-2016
            Publishers = publishers;
            //HuyenPT. Create. End. 02-11-2016
            //HuyenPT. Create. Start. 08-11-2016
            Posts = posts;
            //HuyenPT. Create. End. 08-11-2016
            //HuyenPT. Create. Start. 14-11-2016
            Comments = comments;
            UserProfiles = userprofiles;
            //HuyenPT. Create. End. 14-11-2016
            //DangVH. Create. Start (08/11/2016)
            Groups = groups;
            //DangVH. Create. End (08/11/2016)
        }

        // Bổ sung IMongoCollection cho 1 Object mới
        public IMongoCollection<IdentityRole> Roles { get; set; }
        public IMongoCollection<ApplicationUser> Users { get; set; }
        public IMongoCollection<Category> Categories { get; set; }
        public IMongoCollection<Book> Books { get; set; }
        //HuyenPT. Create. Start. 30-10-2016
        public IMongoCollection<Slider> Sliders { get; set; }
        //HuyenPT. Create. End. 30-10-2016
        //HuyenPT. Create. Start. 02-11-2016
        public IMongoCollection<Publisher> Publishers { get; set; }
        //HuyenPT. Create. End. 02-11-2016
        //HuyenPT. Create. Start. 08-11-2016
        public IMongoCollection<Post> Posts { get; set; }
        //HuyenPT. Create. End. 08-11-2016
        //HuyenPT. Create. Start. 14-11-2016      
        public IMongoCollection<Comment> Comments { get; set; }
        public IMongoCollection<UserProfile> UserProfiles { get; set; }
        //HuyenPT. Create. End. 14-11-2016 
        //DangVH. Create. Start (08/11/2016)
        public IMongoCollection<Group> Groups { get; set; }
        //DangVH. Create. End (08/11/2016)

        public Task<List<IdentityRole>> AllRolesAsync()
        {
            return Roles.Find(r => true).ToListAsync();
        }

        public void Dispose()
        {

        }

    }
}