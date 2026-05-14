using GLMS.Data;
using GLMS.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);


// DATABASE CONNECTION
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")));


// MVC

builder.Services.AddControllersWithViews();
builder.Services.AddHttpClient();

builder.Services.AddScoped<CurrencyService>();


var app = builder.Build();


// ERROR HANDLING
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}


// MIDDLEWARE
app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();


// ROUTING
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");


app.Run();