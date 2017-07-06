using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(NunesHR.Startup))]
namespace NunesHR
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
