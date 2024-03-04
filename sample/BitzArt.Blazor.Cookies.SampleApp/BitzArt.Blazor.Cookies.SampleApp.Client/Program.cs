using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

namespace BitzArt.Blazor.Cookies.SampleApp.Client
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);

            builder.AddBlazorCookies();

            await builder.Build().RunAsync();
        }
    }
}
