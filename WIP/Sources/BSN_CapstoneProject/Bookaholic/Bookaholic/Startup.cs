using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Bookaholic.Startup))]
namespace Bookaholic
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
