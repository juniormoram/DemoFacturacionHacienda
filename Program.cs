using DemoFacturacionHacienda.Data;
using DemoFacturacionHacienda.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite("Data Source=facturacion.db"));

builder.Services.AddScoped<ClaveService>();
builder.Services.AddScoped<XmlService>();
builder.Services.AddScoped<FirmaDigitalService>();
builder.Services.AddHttpClient();
builder.Services.AddScoped<HaciendaService>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();
app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();