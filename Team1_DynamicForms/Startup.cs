using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Team1_DynamicForms.Startup))]
namespace Team1_DynamicForms
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
