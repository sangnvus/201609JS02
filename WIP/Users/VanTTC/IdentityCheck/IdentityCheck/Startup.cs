using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(IdentityCheck.Startup))]
namespace IdentityCheck
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
