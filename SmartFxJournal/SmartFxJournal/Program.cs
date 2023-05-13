using Microsoft.EntityFrameworkCore;
using SmartFxJournal.Common.Services;
using SmartFxJournal.Configuration.Services;
using SmartFxJournal.CTrader.Services;
using SmartFxJournal.JournalDB.model;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<JournalDbContext>(options => options.UseNpgsql(builder.Configuration.GetConnectionString("JournalDB")).UseSnakeCaseNamingConvention());
// Add services to the container.
builder.Services.AddScoped<IJournalSettingsService, JournalSettingsServiceImpl>();
builder.Services.AddSingleton<CTraderService, CTraderService>();

builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();


app.MapControllerRoute(
    name: "default",
    pattern: "{controller}/{action=Index}/{id?}");

app.MapFallbackToFile("index.html");

app.Run();