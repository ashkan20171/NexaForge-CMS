using AshkanCMS.Data; using AshkanCMS.Services; using Microsoft.AspNetCore.Authorization; using Microsoft.AspNetCore.Mvc; using Microsoft.EntityFrameworkCore;
namespace AshkanCMS.Areas.Admin.Controllers;
[Area("Admin"),Authorize(Roles="Administrator")]
public class ExtensionsController(AppDbContext db,PackageInstallerService installer,AuditService audit):Controller{
 public async Task<IActionResult> Index()=>View(await db.ExtensionPackages.OrderByDescending(x=>x.InstalledAt).ToListAsync());
 [HttpPost,ValidateAntiForgeryToken,RequestSizeLimit(11_000_000)] public async Task<IActionResult> Install(IFormFile package){if(package is null){TempData["Error"]="Choose a package ZIP first.";return RedirectToAction(nameof(Index));}var r=await installer.InstallAsync(package);TempData[r.ok?"Success":"Error"]=r.message;if(r.ok)await audit.LogAsync("Installed","Extension package",package.FileName);return RedirectToAction(nameof(Index));}
 [HttpPost,ValidateAntiForgeryToken] public async Task<IActionResult> Toggle(int id){var x=await db.ExtensionPackages.FindAsync(id);if(x!=null){x.IsEnabled=!x.IsEnabled;await db.SaveChangesAsync();await audit.LogAsync(x.IsEnabled?"Enabled":"Disabled","Extension",x.Name);}return RedirectToAction(nameof(Index));}
 [HttpPost,ValidateAntiForgeryToken] public async Task<IActionResult> Uninstall(int id){var r=await installer.UninstallAsync(id);TempData[r.ok?"Success":"Error"]=r.message;return RedirectToAction(nameof(Index));}
 public IActionResult DeveloperKit()=>View();
}
