using Microsoft.EntityFrameworkCore;
using Negocio;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages(options =>
{
    options.Conventions.AuthorizeFolder("/"); // Protege todas las páginas
    options.Conventions.AllowAnonymousToPage("/Login"); // Exceptúa la página de Login
    options.Conventions.AllowAnonymousToPage("/EnviosEzeiza");
});
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Login";
    });
builder.Services.AddAuthorization();
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(
        builder.Configuration.GetConnectionString("MySqlConnection"),
        ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("MySqlConnection"))
    )
);
builder.Services.AddHttpContextAccessor();
builder.Services.AddSession();

// También registrar las clases de negocio:
builder.Services.AddScoped<RemitoNegocio>();
builder.Services.AddScoped<ItemRemitoNegocio>();
builder.Services.AddScoped<GenerarReportes>();
builder.Services.AddScoped<GenerarPDFSalida>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseSession();
app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();

app.Run();
