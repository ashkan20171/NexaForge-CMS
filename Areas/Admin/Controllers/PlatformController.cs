using AshkanCMS.Data; using AshkanCMS.Models; using Microsoft.AspNetCore.Authorization; using Microsoft.AspNetCore.Mvc; using Microsoft.EntityFrameworkCore;
namespace AshkanCMS.Areas.Admin.Controllers;
[Area("Admin"),Authorize]
public class PlatformController(AppDbContext db):Controller{
 public async Task<IActionResult> Forms()=>View(await db.Forms.OrderBy(x=>x.Name).ToListAsync());
 [HttpPost,ValidateAntiForgeryToken] public async Task<IActionResult> AddForm(string name,string slug,string fieldsJson){if(!string.IsNullOrWhiteSpace(name)){db.Forms.Add(new FormDefinition{Name=name.Trim(),Slug=slug.Trim().ToLowerInvariant(),FieldsJson=fieldsJson});await db.SaveChangesAsync();TempData["Success"]="Form created.";}return RedirectToAction(nameof(Forms));}
 public async Task<IActionResult> Submissions()=>View(await db.FormSubmissions.OrderByDescending(x=>x.CreatedAt).Take(200).ToListAsync());
 public async Task<IActionResult> Redirects()=>View(await db.RedirectRules.OrderBy(x=>x.FromPath).ToListAsync());
 [HttpPost,ValidateAntiForgeryToken] public async Task<IActionResult> AddRedirect(string fromPath,string toPath,int statusCode=301){db.RedirectRules.Add(new RedirectRule{FromPath=fromPath,ToPath=toPath,StatusCode=statusCode});await db.SaveChangesAsync();return RedirectToAction(nameof(Redirects));}
 [HttpPost,ValidateAntiForgeryToken] public async Task<IActionResult> ToggleRedirect(int id){var x=await db.RedirectRules.FindAsync(id);if(x!=null){x.IsEnabled=!x.IsEnabled;await db.SaveChangesAsync();}return RedirectToAction(nameof(Redirects));}
 public async Task<IActionResult> Languages()=>View(await db.Languages.OrderByDescending(x=>x.IsDefault).ToListAsync());
 [HttpPost,ValidateAntiForgeryToken] public async Task<IActionResult> AddLanguage(string code,string name,bool isRtl=false){if(!await db.Languages.AnyAsync(x=>x.Code==code)){db.Languages.Add(new LanguageDefinition{Code=code.Trim().ToLowerInvariant(),Name=name,IsRtl=isRtl});await db.SaveChangesAsync();}return RedirectToAction(nameof(Languages));}
 public async Task<IActionResult> Scheduler()=>View(await db.ScheduledJobs.OrderBy(x=>x.RunAt).ToListAsync());
 [HttpPost,ValidateAntiForgeryToken] public async Task<IActionResult> AddJob(string name,string jobType,DateTime runAt,string payload){db.ScheduledJobs.Add(new ScheduledJob{Name=name,JobType=jobType,RunAt=runAt,Payload=payload});await db.SaveChangesAsync();return RedirectToAction(nameof(Scheduler));}
 public async Task<IActionResult> Customizer()=>View(await db.CustomizerSettings.FirstAsync());
 [HttpPost,ValidateAntiForgeryToken] public async Task<IActionResult> Customizer(CustomizerSetting model){var x=await db.CustomizerSettings.FirstAsync();x.BrandName=model.BrandName;x.PrimaryColor=model.PrimaryColor;x.SurfaceStyle=model.SurfaceStyle;x.FontScale=model.FontScale;x.CompactSidebar=model.CompactSidebar;await db.SaveChangesAsync();TempData["Success"]="Appearance saved.";return RedirectToAction(nameof(Customizer));}
 public async Task<IActionResult> Health(){ViewBag.Posts=await db.Posts.CountAsync();ViewBag.Pages=await db.Pages.CountAsync();ViewBag.Users=await db.Users.CountAsync();ViewBag.Media=await db.Media.CountAsync();ViewBag.PendingComments=await db.Comments.CountAsync(x=>!x.Approved);ViewBag.Unread=await db.Notifications.CountAsync(x=>!x.IsRead);return View();}
}
