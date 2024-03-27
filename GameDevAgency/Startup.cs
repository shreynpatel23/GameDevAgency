using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(GameDevAgency.Startup))]
namespace GameDevAgency
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
