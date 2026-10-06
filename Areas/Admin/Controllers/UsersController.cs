using AshkanCMS.Data; using AshkanCMS.Models; using AshkanCMS.Services; using Microsoft.AspNetCore.Authorization; using Microsoft.AspNetCore.Identity; using Microsoft.AspNetCore.Mvc; using Microsoft.EntityFrameworkCore;
namespace AshkanCMS.Areas.Admin.Controllers;
[Area("Admin"),Authorize(Roles="Administrator")]
public class UsersController(AppDbContext db,IPasswordHasher<CmsUser> hasher,AuditService audit):Controller{
 public async Task<IActionResult> Index()=>View(await db.Users.OrderBy(x=>x.UserName).ToListAsync());
 [HttpPost,ValidateAntiForgeryToken] public async Task<IActionResult> Create(UserCreateVm vm){if(!ModelState.IsValid){TempData["Error"]="Please complete all user fields.";return RedirectToAction(nameof(Index));}if(await db.Users.AnyAsync(x=>x.UserName==vm.UserName)){TempData["Error"]="Username already exists.";return RedirectToAction(nameof(Index));}var u=new CmsUser{UserName=vm.UserName.Trim(),DisplayName=vm.DisplayName.Trim(),Role=vm.Role,IsActive=true};u.PasswordHash=hasher.HashPassword(u,vm.Password);db.Users.Add(u);await db.SaveChangesAsync();await audit.LogAsync("Created","User",u.UserName);TempData["Success"]="User created.";return RedirectToAction(nameof(Index));}
 [HttpPost,ValidateAntiForgeryToken] public async Task<IActionResult> Toggle(int id){var u=await db.Users.FindAsync(id);if(u!=null&&u.UserName!="admin"){u.IsActive=!u.IsActive;await db.SaveChangesAsync();await audit.LogAsync("Toggled","User",u.UserName);}return RedirectToAction(nameof(Index));}
}
