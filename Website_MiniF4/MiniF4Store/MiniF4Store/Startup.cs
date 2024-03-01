using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(MiniF4Store.Startup))]
namespace MiniF4Store
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
