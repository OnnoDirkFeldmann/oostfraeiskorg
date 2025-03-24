using DotVVM.Framework.Configuration;
using DotVVM.Framework.ResourceManagement;
using DotVVM.Framework.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace oostfraeiskorg;

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

        config.RouteTable.Add("about", "about", "Views/about.dothtml");
        config.RouteTable.Add("about.aspx", "about.aspx", "Views/about.dothtml");
        config.RouteTable.Add("about.dothtml", "about.dothtml", "Views/about.dothtml");

        config.RouteTable.Add("admin", "admin", "Views/admin.dothtml");
        config.RouteTable.Add("admin.aspx", "admin.aspx", "Views/admin.dothtml");
        config.RouteTable.Add("admin.dothtml", "admin.dothtml", "Views/admin.dothtml");

        config.RouteTable.Add("coursebook", "coursebook", "Views/coursebook.dothtml");
        config.RouteTable.Add("coursebook.aspx", "coursebook.aspx", "Views/coursebook.dothtml");
        config.RouteTable.Add("coursebook.dothtml", "coursebook.dothtml", "Views/coursebook.dothtml");

        config.RouteTable.Add("donateaword", "donateaword", "Views/donateaword.dothtml");
        config.RouteTable.Add("donateaword.aspx", "donateaword.aspx", "Views/donateaword.dothtml");
        config.RouteTable.Add("donateaword.dothtml", "donateaword.dothtml", "Views/donateaword.dothtml");

        config.RouteTable.Add("downloads", "downloads", "Views/downloads.dothtml");
        config.RouteTable.Add("downloads.aspx", "downloads.aspx", "Views/downloads.dothtml");
        config.RouteTable.Add("downloads.dothtml", "downloads.dothtml", "Views/downloads.dothtml");

        config.RouteTable.Add("dsgvo", "dsgvo", "Views/dsgvo.dothtml");
        config.RouteTable.Add("dsgvo.aspx", "dsgvo.aspx", "Views/dsgvo.dothtml");
        config.RouteTable.Add("dsgvo.dothtml", "dsgvo.dothtml", "Views/dsgvo.dothtml");

        config.RouteTable.Add("grammar", "grammar", "Views/grammar.dothtml");
        config.RouteTable.Add("grammar.aspx", "grammar.aspx", "Views/grammar.dothtml");
        config.RouteTable.Add("grammar.dothtml", "grammar.dothtml", "Views/grammar.dothtml");

        config.RouteTable.Add("translator", "translator", "Views/translator.dothtml");
        config.RouteTable.Add("translator.aspx", "translator.aspx", "Views/translator.dothtml");
        config.RouteTable.Add("translator.dothtml", "translator.dothtml", "Views/translator.dothtml");

        config.RouteTable.Add("keyboard", "keyboard", "Views/keyboard.dothtml");
        config.RouteTable.Add("keyboard.aspx", "keyboard.aspx", "Views/keyboard.dothtml");
        config.RouteTable.Add("keyboard.dothtml", "keyboard.dothtml", "Views/keyboard.dothtml");

        config.RouteTable.Add("leerbauk", "leerbauk", "Views/coursebook.dothtml");
        config.RouteTable.Add("leerbauk.aspx", "leerbauk.aspx", "Views/coursebook.dothtml");
        config.RouteTable.Add("leerbauk.dothtml", "leerbauk.dothtml", "Views/coursebook.dothtml");

        config.RouteTable.Add("lesson1", "lesson1", "Views/lessons/lesson1.dothtml");
        config.RouteTable.Add("lesson1.aspx", "lesson1.aspx", "Views/lessons/lesson1.dothtml");
        config.RouteTable.Add("lesson1.dothtml", "lesson1.dothtml", "Views/lessons/lesson1.dothtml");

        config.RouteTable.Add("lesson1en", "lesson1en", "Views/lessons/lesson1en.dothtml");
        config.RouteTable.Add("lesson1en.aspx", "lesson1en.aspx", "Views/lessons/lesson1en.dothtml");
        config.RouteTable.Add("lesson1en.dothtml", "lesson1en.dothtml", "Views/lessons/lesson1en.dothtml");

        config.RouteTable.Add("lesson2", "lesson2", "Views/lessons/lesson2.dothtml");
        config.RouteTable.Add("lesson2.aspx", "lesson2.aspx", "Views/lessons/lesson2.dothtml");
        config.RouteTable.Add("lesson2.dothtml", "lesson2.dothtml", "Views/lessons/lesson2.dothtml");

        config.RouteTable.Add("lesson2en", "lesson2en", "Views/lessons/lesson2en.dothtml");
        config.RouteTable.Add("lesson2en.aspx", "lesson2en.aspx", "Views/lessons/lesson2en.dothtml");
        config.RouteTable.Add("lesson2en.dothtml", "lesson2en.dothtml", "Views/lessons/lesson2en.dothtml");

        config.RouteTable.Add("lesson3", "lesson3", "Views/lessons/lesson3.dothtml");
        config.RouteTable.Add("lesson3.aspx", "lesson3.aspx", "Views/lessons/lesson3.dothtml");
        config.RouteTable.Add("lesson3.dothtml", "lesson3.dothtml", "Views/lessons/lesson3.dothtml");

        config.RouteTable.Add("lesson3en", "lesson3en", "Views/lessons/lesson3en.dothtml");
        config.RouteTable.Add("lesson3en.aspx", "lesson3en.aspx", "Views/lessons/lesson3en.dothtml");
        config.RouteTable.Add("lesson3en.dothtml", "lesson3en.dothtml", "Views/lessons/lesson3en.dothtml");

        config.RouteTable.Add("lesson4", "lesson4", "Views/lessons/lesson4.dothtml");
        config.RouteTable.Add("lesson4.aspx", "lesson4.aspx", "Views/lessons/lesson4.dothtml");
        config.RouteTable.Add("lesson4.dothtml", "lesson4.dothtml", "Views/lessons/lesson4.dothtml");

        config.RouteTable.Add("lesson4en", "lesson4en", "Views/lessons/lesson4en.dothtml");
        config.RouteTable.Add("lesson4en.aspx", "lesson4en.aspx", "Views/lessons/lesson4en.dothtml");
        config.RouteTable.Add("lesson4en.dothtml", "lesson4en.dothtml", "Views/lessons/lesson4en.dothtml");

        config.RouteTable.Add("links", "links", "Views/links.dothtml");
        config.RouteTable.Add("links.aspx", "links.aspx", "Views/links.dothtml");
        config.RouteTable.Add("links.dothtml", "links.dothtml", "Views/links.dothtml");

        config.RouteTable.Add("main", "main", "Views/main.dothtml");
        config.RouteTable.Add("main.aspx", "main.aspx", "Views/main.dothtml");
        config.RouteTable.Add("main.dothtml", "main.dothtml", "Views/main.dothtml");

        config.RouteTable.Add("onlinecourse", "onlinecourse", "Views/onlinecourse.dothtml");
        config.RouteTable.Add("onlinecourse.aspx", "onlinecourse.aspx", "Views/onlinecourse.dothtml");
        config.RouteTable.Add("onlinecourse.dothtml", "onlinecourse.dothtml", "Views/onlinecourse.dothtml");

        config.RouteTable.Add("onlinecourseen", "onlinecourseen", "Views/onlinecourseen.dothtml");
        config.RouteTable.Add("onlinecourseen.aspx", "onlinecourseen.aspx", "Views/onlinecourseen.dothtml");
        config.RouteTable.Add("onlinecourseen.dothtml", "onlinecourseen.dothtml", "Views/onlinecourseen.dothtml");

        config.RouteTable.Add("vocalsystem", "vocalsystem", "Views/lessons/vocalsystem.dothtml");
        config.RouteTable.Add("vocalsystem.aspx", "vocalsystem.aspx", "Views/lessons/vocalsystem.dothtml");
        config.RouteTable.Add("vocalsystem.dothtml", "vocalsystem.dothtml", "Views/lessons/vocalsystem.dothtml");

        config.RouteTable.Add("vocalsystemen", "vocalsystemen", "Views/lessons/vocalsystemen.dothtml");
        config.RouteTable.Add("vocalsystemen.aspx", "vocalsystemen.aspx", "Views/lessons/vocalsystemen.dothtml");
        config.RouteTable.Add("vocalsystemen.dothtml", "vocalsystemen.dothtml", "Views/lessons/vocalsystemen.dothtml");

        config.RouteTable.AutoDiscoverRoutes(new DefaultRouteStrategy(config));
    }

    private void ConfigureControls(DotvvmConfiguration config, string applicationPath)
    {
        // register code-only controls and markup controls
    }

    private void ConfigureResources(DotvvmConfiguration config, string applicationPath)
    {
        // css
        config.Resources.Register("bootstrap-css", new StylesheetResource
        {
            Location = new UrlResourceLocation("~/css/bootstrap.min.css")
        });
        config.Resources.Register("cookiealert-css", new StylesheetResource
        {
            Location = new UrlResourceLocation("~/css/cookiealert.css")
        });
        config.Resources.Register("custom-css", new StylesheetResource
        {
            Location = new UrlResourceLocation("~/css/custom.css")
        });
        config.Resources.Register("font-awesome-css", new StylesheetResource
        {
            Location = new UrlResourceLocation("~/css/font-awesome.min.css")
        });
        config.Resources.Register("styles-css", new StylesheetResource()
        {
            Location = new UrlResourceLocation("~/css/style.css")
        });

        //javascript
        config.Resources.Register("bootstrap", new ScriptResource
        {
            Location = new UrlResourceLocation("~/lib/js/bootstrap.min.js"),
            Dependencies = new[] { "bootstrap-css", "jquery", "popper" }
        });
        config.Resources.Register("jquery", new ScriptResource
        {
            Location = new UrlResourceLocation("~/lib/js/jquery-3.5.1.min.js")
        });
        config.Resources.Register("custom", new ScriptResource
        {
            Location = new UrlResourceLocation("~/lib/js/custom.js"),
            Dependencies = new[] { "custom-css" }
        });
        config.Resources.Register("popper", new ScriptResource
        {
            Location = new UrlResourceLocation("~/lib/js/popper.min.js")
        });
        config.Resources.Register("cookiealert", new ScriptResource
        {
            Location = new UrlResourceLocation("~/lib/js/cookiealert.js"),
            Dependencies = new[] { "cookiealert-css" }
        });

        //modules
        config.Resources.Register("dictionaryentry", new ScriptModuleResource(new UrlResourceLocation("~/lib/js/dictionaryentry.js"))
        {
        });
        config.Resources.Register("translator", new ScriptModuleResource(new UrlResourceLocation("~/lib/js/translator.js"))
        {
        });
    }

    public void ConfigureServices(IDotvvmServiceCollection options)
    {
        options.AddDefaultTempStorages("temp");
        options.AddHotReload();
    }
}
