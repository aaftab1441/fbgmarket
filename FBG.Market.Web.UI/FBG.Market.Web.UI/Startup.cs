using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(FBG.Market.Web.Identity.Startup))]
namespace FBG.Market.Web.Identity
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
