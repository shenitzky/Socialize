using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Socialize.Startup))]
namespace Socialize
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
