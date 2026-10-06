using AshkanCMS.Data;
using AshkanCMS.Models;
using AshkanCMS.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
var builder=WebApplication.CreateBuilder(args);
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<AppDbContext>(o=>o.UseSqlite(builder.Configuration.GetConnectionString("Default")));
builder.Services.AddScoped<SlugService>(); builder.Services.AddScoped<AuditService>(); builder.Services.AddHttpClient(); builder.Services.AddScoped<AiAssistantService>();
builder.Services.AddScoped<PackageInstallerService>(); builder.Services.AddScoped<HookRuntime>(); builder.Services.AddHttpContextAccessor(); builder.Services.AddSession();
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(o=>{o.LoginPath="/Account/Login";o.AccessDeniedPath="/Account/Denied";o.ExpireTimeSpan=TimeSpan.FromHours(8);});
builder.Services.AddAuthorization(); builder.Services.AddScoped<IPasswordHasher<CmsUser>,PasswordHasher<CmsUser>>();
var app=builder.Build();
using(var s=app.Services.CreateScope()) { var db=s.ServiceProvider.GetRequiredService<AppDbContext>(); var logger=s.ServiceProvider.GetRequiredService<ILoggerFactory>().CreateLogger("DatabaseBootstrap"); DatabaseBootstrapper.Initialize(db, logger); SeedData.Initialize(db); if(!db.Users.Any()){var h=s.ServiceProvider.GetRequiredService<IPasswordHasher<CmsUser>>();var u=new CmsUser{UserName="admin",DisplayName="Ashkan Administrator",Role="Administrator"};u.PasswordHash=h.HashPassword(u,"Ashkan@123");db.Users.Add(u);db.SaveChanges();}}
if(!app.Environment.IsDevelopment()) app.UseExceptionHandler("/Home/Error");
app.Use(async (context,next)=>{
 context.Response.Headers["X-Content-Type-Options"]="nosniff";
 context.Response.Headers["X-Frame-Options"]="SAMEORIGIN";
 context.Response.Headers["Referrer-Policy"]="strict-origin-when-cross-origin";
 context.Response.Headers["Permissions-Policy"]="camera=(), microphone=(), geolocation=()";
 await next();
});
app.UseStaticFiles(); app.UseRouting(); app.UseSession(); app.UseAuthentication(); app.UseAuthorization();
app.MapControllerRoute(name:"areas",pattern:"{area:exists}/{controller=Dashboard}/{action=Index}/{id?}");
app.MapGet("/health", () => Results.Ok(new { status="healthy", product="NexaForge CMS", utc=DateTime.UtcNow }));
app.MapControllerRoute(name:"default",pattern:"{controller=Home}/{action=Index}/{id?}"); app.Run();
