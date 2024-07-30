using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Pact.Consumer.MVC;

namespace Pact.Provider.Api
{
    public static class Program
    {
        public const string NhtsaPublicApiHttpClientName = "somePublicApi";

        public static void Main(string[] args)
        {

            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddControllersWithViews();

            var startup = new Startup(builder.Configuration);

            startup.ConfigureServices(builder.Services);

            var app = builder.Build();
            app.UseRouting();
            //app.UseEndpoints(endpoints =>
            //{
            //    endpoints.MapControllers();
            //});

            app.MapControllerRoute(name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");

            startup.Configure(app, app.Environment);

            app.Run();

        }
    }
}