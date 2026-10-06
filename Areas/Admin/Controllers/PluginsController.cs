using AshkanCMS.Data; using AshkanCMS.Services; using Microsoft.AspNetCore.Authorization; using Microsoft.AspNetCore.Mvc; using Microsoft.EntityFrameworkCore;
namespace AshkanCMS.Areas.Admin.Controllers;
[Area("Admin"),Authorize(Roles="Administrator")]
public class PluginsController(AppDbContext db,AuditService audit):Controller{public async Task<IActionResult> Index()=>View(await db.Plugins.ToListAsync());[HttpPost,ValidateAntiForgeryToken]public async Task<IActionResult> Toggle(int id){var p=await db.Plugins.FindAsync(id);if(p!=null){p.IsEnabled=!p.IsEnabled;await db.SaveChangesAsync();await audit.LogAsync(p.IsEnabled?"Enabled":"Disabled","Plugin",p.Name);}return RedirectToAction(nameof(Index));}}
