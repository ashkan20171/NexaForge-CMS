using AshkanCMS.Data; using AshkanCMS.Models; using AshkanCMS.Services; using Microsoft.AspNetCore.Authorization; using Microsoft.AspNetCore.Mvc; using Microsoft.EntityFrameworkCore;
namespace AshkanCMS.Areas.Admin.Controllers;
[Area("Admin"),Authorize]
public class ExperienceController(AppDbContext db,SlugService slug):Controller{
 public async Task<IActionResult> Tags()=>View(await db.Tags.OrderBy(x=>x.Name).ToListAsync());
 [HttpPost,ValidateAntiForgeryToken] public async Task<IActionResult> AddTag(string name){if(!string.IsNullOrWhiteSpace(name)){var s=slug.Make(name);if(!await db.Tags.AnyAsync(x=>x.Slug==s)){db.Tags.Add(new TagDefinition{Name=name.Trim(),Slug=s});await db.SaveChangesAsync();}}return RedirectToAction(nameof(Tags));}
 [HttpPost,ValidateAntiForgeryToken] public async Task<IActionResult> DeleteTag(int id){var x=await db.Tags.FindAsync(id);if(x!=null){db.Tags.Remove(x);await db.SaveChangesAsync();}return RedirectToAction(nameof(Tags));}
 public async Task<IActionResult> Widgets()=>View(await db.Widgets.OrderBy(x=>x.Zone).ThenBy(x=>x.SortOrder).ToListAsync());
 [HttpPost,ValidateAntiForgeryToken] public async Task<IActionResult> AddWidget(WidgetDefinition w){if(ModelState.IsValid){db.Widgets.Add(w);await db.SaveChangesAsync();}return RedirectToAction(nameof(Widgets));}
 [HttpPost,ValidateAntiForgeryToken] public async Task<IActionResult> ToggleWidget(int id){var x=await db.Widgets.FindAsync(id);if(x!=null){x.IsEnabled=!x.IsEnabled;await db.SaveChangesAsync();}return RedirectToAction(nameof(Widgets));}
 public async Task<IActionResult> Revisions()=>View(await db.Revisions.OrderByDescending(x=>x.CreatedAt).Take(100).ToListAsync());
 public async Task<IActionResult> Notifications()=>View(await db.Notifications.OrderByDescending(x=>x.CreatedAt).ToListAsync());
 [HttpPost,ValidateAntiForgeryToken] public async Task<IActionResult> ReadAll(){foreach(var n in await db.Notifications.Where(x=>!x.IsRead).ToListAsync())n.IsRead=true;await db.SaveChangesAsync();return RedirectToAction(nameof(Notifications));}
 public IActionResult PageBuilder()=>View();
}
