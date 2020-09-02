using System;
using System.Net.Http;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Core;
using Serilog.Exceptions;

namespace BlazorWasmLogUnhandledExceptions.Client
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            SetupLogger();
            Log.Information("Hello, browser!");

            try
            {
                var builder = WebAssemblyHostBuilder.CreateDefault(args);
                builder.RootComponents.Add<App>("#app");
                builder.Services.AddSingleton(new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

                var unhandledExceptionSender = new UnhandledExceptionSender();
                var unhandledExceptionProvider = new UnhandledExceptionProvider(unhandledExceptionSender);
                builder.Logging.AddProvider(unhandledExceptionProvider);
                builder.Services.AddSingleton<IUnhandledExceptionSender>(unhandledExceptionSender);

                await builder.Build().RunAsync();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "An exception occurred while creating the WASM host");
                throw;
            }

        }

        private static void SetupLogger()
        {
            var levelSwitch = new LoggingLevelSwitch();
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.ControlledBy(levelSwitch)
                .Enrich.WithProperty("InstanceId", Guid.NewGuid().ToString("n"))
                .Enrich.WithProperty(nameof(AppState.User), AppState.User)
                .Enrich.WithProperty(nameof(AppState.UserCompany), AppState.UserCompany)
                .Enrich.WithExceptionDetails()
                .WriteTo.BrowserConsole()
                .WriteTo.BrowserHttp(controlLevelSwitch: levelSwitch)
                .CreateLogger();
        }
    }
}
