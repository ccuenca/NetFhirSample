using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using FHIRCamp.Schedule.GUI;
using FHIRCamp.Schedule.BL.Services;
using System.Reflection;
using AutoMapper;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

//Services
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri("https://server.fire.ly") });
builder.Services.AddScoped<IScheduleService, ScheduleService>();
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

await builder.Build().RunAsync();

