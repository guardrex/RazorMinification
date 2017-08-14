using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Internal;
//using Microsoft.AspNetCore.Razor.Language;
//using Microsoft.AspNetCore.Routing;
//using Microsoft.Extensions.DependencyInjection;
//using Microsoft.Extensions.Logging;

namespace RazorMinification
{
    public class Program
    {
        public static void Main(string[] args)
        {
            BuildWebHost(args).Run();
        }

        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .ConfigureServices(services =>
                {
                    services.AddMvc();
                    services.AddSingleton<RazorTemplateEngine, CustomRazorTemplateEngine>();
                })
                .Configure(app =>
                {
                    app.UseDeveloperExceptionPage();
                    app.UseMvcWithDefaultRoute();
                    app.UseStaticFiles();
                })
                .Build();
    }
}