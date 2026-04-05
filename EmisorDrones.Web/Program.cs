using EmisorDrones.CoreBusiness.Services;

var builder = WebApplication.CreateBuilder(args);

// agregar servicios al contenedor DI
builder.Services.AddControllersWithViews();
builder.Services.AddSingleton<ServicioDrones>();
builder.Services.AddScoped<MotorOptimizacionService>();
builder.Services.AddScoped<XmlReaderService>();
builder.Services.AddScoped<XmlWriterService>();
builder.Services.AddScoped<GraphvizService>();
builder.Services.AddSingleton<EstadoSistemaService>();
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
