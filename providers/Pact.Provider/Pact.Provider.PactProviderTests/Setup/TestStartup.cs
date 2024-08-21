using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Pact.Provider.Api.Services;

namespace Pact.Provider.PactProviderTests.Setup
{
    public class TestStartup
    {
        public TestStartup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<INhtsaHttpClient, HttpClientMock>(); //--> MOCKS !!!!
            services.AddSingleton<INhtsaService, NhtsaService>();
            var controllersAssembly = typeof(NhtsaService).Assembly;
            services.AddControllers().AddApplicationPart(controllersAssembly).AddControllersAsServices();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseMiddleware<ProviderStateMiddleware>();
            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseEndpoints(endpoints => endpoints.MapControllers());

        }
    }
}