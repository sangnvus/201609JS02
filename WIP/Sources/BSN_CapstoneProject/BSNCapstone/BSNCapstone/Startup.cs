using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(BSNCapstone.Startup))]
namespace BSNCapstone
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            // HuyenPT. Create. Start. 14-11-2016
            app.MapSignalR();
            // HuyenPT. Create. End. 14-11-2016
            ConfigureAuth(app);
        }
    }
}
