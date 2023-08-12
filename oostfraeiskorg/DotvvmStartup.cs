using DotVVM.Framework.Configuration;
using DotVVM.Framework.ResourceManagement;
using DotVVM.Framework.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace oostfraeiskorg
{
    public class DotvvmStartup : IDotvvmStartup, IDotvvmServiceConfigurator
    {
        // For more information about this class, visit https://dotvvm.com/docs/tutorials/basics-project-structure
        public void Configure(DotvvmConfiguration config, string applicationPath)
        {

            ConfigureRoutes(config, applicationPath);
            ConfigureControls(config, applicationPath);
            ConfigureResources(config, applicationPath);
            
        }

        private void ConfigureRoutes(DotvvmConfiguration config, string applicationPath)
        {
            config.RouteTable.Add("index", "", "Views/main.dothtml");
            config.RouteTable.Add("main", "main", "Views/main.dothtml");
            config.RouteTable.Add("main.aspx", "main.aspx", "Views/main.dothtml");
            config.RouteTable.Add("main.dothtml", "main.dothtml", "Views/main.dothtml");

            config.RouteTable.Add("about", "about", "Views/about.dothtml");
            config.RouteTable.Add("about.aspx", "about.aspx", "Views/about.dothtml");
            config.RouteTable.Add("about.dothtml", "about.dothtml", "Views/about.dothtml");

            config.RouteTable.Add("grammar", "grammar", "Views/grammar.dothtml");
            config.RouteTable.Add("grammar.aspx", "grammar.aspx", "Views/grammar.dothtml");
            config.RouteTable.Add("grammar.dothtml", "grammar.dothtml", "Views/grammar.dothtml");

            config.RouteTable.Add("dsgvo", "dsgvo", "Views/dsgvo.dothtml");
            config.RouteTable.Add("dsgvo.aspx", "dsgvo.aspx", "Views/dsgvo.dothtml");
            config.RouteTable.Add("dsgvo.dothtml", "dsgvo.dothtml", "Views/dsgvo.dothtml");

            config.RouteTable.AutoDiscoverRoutes(new DefaultRouteStrategy(config));    
        }

        private void ConfigureControls(DotvvmConfiguration config, string applicationPath)
        {
            // register code-only controls and markup controls
        }

        private void ConfigureResources(DotvvmConfiguration config, string applicationPath)
        {
            // register custom resources and adjust paths to the built-in resources
            config.Resources.Register("bootstrap-css", new StylesheetResource
            {
                Location = new UrlResourceLocation("~/lib/bootstrap/css/bootstrap.min.css")
            });
            config.Resources.Register("bootstrap", new ScriptResource
            {
                Location = new UrlResourceLocation("~/lib/bootstrap/js/bootstrap.min.js"),
                Dependencies = new[] { "bootstrap-css" , "jquery", "popper" }
            });
            config.Resources.Register("jquery", new ScriptResource
            {
                Location = new UrlResourceLocation("~/lib/jquery/jquery.min.js")
            });
            config.Resources.Register("custom", new ScriptResource
            {
                Location = new UrlResourceLocation("~/lib/js/custom.js")
            });
            config.Resources.Register("popper", new ScriptResource
            {
                Location = new UrlResourceLocation("~/lib/js/popper.min.js")
            });
            config.Resources.Register("cookiealert", new ScriptResource
            {
                Location = new UrlResourceLocation("~/lib/js/cookiealert.js")
            });
            config.Resources.Register("Styles", new StylesheetResource()
            {
                Location = new UrlResourceLocation("~/css/style.css")
            });
            config.Resources.Register("Styles", new StylesheetResource()
            {
                Location = new UrlResourceLocation("~/css/style.css")
            });
            config.Resources.Register("module", new ScriptModuleResource(new UrlResourceLocation("~/lib/js/entry.js"))
            {
                //Dependencies = new[] { "bootstrap-css", "jquery" }
            });
        }

		public void ConfigureServices(IDotvvmServiceCollection options)
        {
            options.AddDefaultTempStorages("temp");
            options.AddHotReload();
		}
    }
}
