using EmisorDrones.CoreBusiness.Services;

var builder = WebApplication.CreateBuilder(args);

// agregar servicios
builder.Services.AddControllersWithViews();
builder.Services.AddScoped<ServicioDrones>();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
});

var app = builder.Build();

app.UseStaticFiles();
app.UseRouting();
app.UseSession();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
