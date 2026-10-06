using AshkanCMS.Data; using AshkanCMS.Services; using Microsoft.AspNetCore.Authorization; using Microsoft.AspNetCore.Mvc; using Microsoft.EntityFrameworkCore;
namespace AshkanCMS.Areas.Admin.Controllers;
[Area("Admin"),Authorize(Roles="Administrator")]
public class ThemesController(AppDbContext db,AuditService audit):Controller{public async Task<IActionResult> Index()=>View(await db.Themes.ToListAsync());[HttpPost,ValidateAntiForgeryToken]public async Task<IActionResult> Activate(int id){foreach(var t in db.Themes)t.IsActive=t.Id==id;await db.SaveChangesAsync();var active=await db.Themes.FindAsync(id);await audit.LogAsync("Activated","Theme",active?.Name??id.ToString());return RedirectToAction(nameof(Index));}}
