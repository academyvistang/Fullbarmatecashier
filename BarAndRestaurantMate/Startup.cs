using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(BarAndRestaurantMate.Startup))]
namespace BarAndRestaurantMate
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
            app.MapSignalR();
        }
    }
}
