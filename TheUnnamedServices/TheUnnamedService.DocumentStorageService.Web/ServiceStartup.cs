using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.Extensions.DependencyInjection;

namespace TheUnnamedService.DocumentStorageService.Web;

public class ServiceStartup : IHostingStartup
{
    public void Configure(IWebHostBuilder builder)
    {
        builder.ConfigureServices(ConfigureServices);
    }

    private void ConfigureServices(IServiceCollection services)
    {
        services.AddControllers().PartManager.ApplicationParts.Add(new AssemblyPart(GetType().Assembly));
    }
}
