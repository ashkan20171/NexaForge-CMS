using System.Text.Json;
using AshkanCMS.Data; using AshkanCMS.Models; using AshkanCMS.Services;
using Microsoft.AspNetCore.Authorization; using Microsoft.AspNetCore.Mvc; using Microsoft.EntityFrameworkCore;
namespace AshkanCMS.Areas.Admin.Controllers;
[Area("Admin"),Authorize]
public class StudioController(AppDbContext db,AiAssistantService ai):Controller{
 public async Task<IActionResult> Index(){ViewBag.Trash=await db.TrashItems.OrderByDescending(x=>x.DeletedAt).Take(20).ToListAsync();ViewBag.Permissions=await db.PermissionGrants.OrderBy(x=>x.Role).ToListAsync();ViewBag.Hooks=await db.ExtensionHooks.OrderBy(x=>x.HookName).ThenBy(x=>x.Priority).ToListAsync();return View();}
 [HttpPost,ValidateAntiForgeryToken] public async Task<IActionResult> AiAssist(string tool,string prompt){var cfg=await db.AiSettings.FirstOrDefaultAsync()??new AiSetting();var result=await ai.GenerateAsync(cfg,tool,prompt);db.AiHistory.Add(new AiHistoryItem{Tool=tool,Prompt=prompt,Result=result,UserName=User.Identity?.Name??"system"});await db.SaveChangesAsync();return Json(new{result});}
 [HttpPost,ValidateAntiForgeryToken] public async Task<IActionResult> Restore(int id){var t=await db.TrashItems.FindAsync(id);if(t==null)return NotFound();if(t.EntityType=="Post"){var p=JsonSerializer.Deserialize<Post>(t.PayloadJson);if(p!=null){p.Id=0;p.Slug=p.Slug+"-restored-"+DateTime.UtcNow.ToString("HHmmss");db.Posts.Add(p);}}db.TrashItems.Remove(t);await db.SaveChangesAsync();TempData["Success"]="Content restored.";return RedirectToAction(nameof(Index));}
 [Authorize(Roles="Administrator"),HttpPost,ValidateAntiForgeryToken] public async Task<IActionResult> SetPermission(string role,string permission,bool allowed){var x=await db.PermissionGrants.FirstOrDefaultAsync(a=>a.Role==role&&a.Permission==permission);if(x==null)db.PermissionGrants.Add(new PermissionGrant{Role=role,Permission=permission,Allowed=allowed});else x.Allowed=allowed;await db.SaveChangesAsync();return RedirectToAction(nameof(Index));}
 [Authorize(Roles="Administrator"),HttpPost,ValidateAntiForgeryToken] public async Task<IActionResult> AddHook(string hookName,string packageId,int priority=10){db.ExtensionHooks.Add(new ExtensionHook{HookName=hookName,PackageId=packageId,Priority=priority});await db.SaveChangesAsync();return RedirectToAction(nameof(Index));}
}
