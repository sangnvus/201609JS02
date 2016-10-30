using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(BSNCapstone.Startup))]
namespace BSNCapstone
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
