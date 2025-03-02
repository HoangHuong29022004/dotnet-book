using Microsoft.EntityFrameworkCore;
using BlazorApp.Data; // 💡 Đảm bảo bạn đã import namespace
using Microsoft.AspNetCore.Http.Features;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddSingleton<WeatherForecastService>();

// 💡 SỬA ApplicationDbContext -> AppDbContext
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql("server=localhost;database=dev_dotnet;user=dev_khaizinam;password=It@@246357",
        new MySqlServerVersion(new Version(8, 0, 34))));

builder.Services.Configure<FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = 10 * 1024 * 1024; // 10 MB
});

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri("http://127.0.0.1:5076") });
builder.Services.AddScoped<ProductService>();

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddHttpClient();

// Register the Blazor services
builder.Services.AddServerSideBlazor();

var app = builder.Build();

// Enable static file serving
app.UseStaticFiles();  // This will serve files from wwwroot

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();
