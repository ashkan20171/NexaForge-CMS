using AshkanCMS.Data; using Microsoft.AspNetCore.Authorization; using Microsoft.AspNetCore.Mvc; using Microsoft.EntityFrameworkCore;
namespace AshkanCMS.Areas.Admin.Controllers;
[Area("Admin"),Authorize(Roles="Administrator")]
public class AuditController(AppDbContext db):Controller{public async Task<IActionResult> Index()=>View(await db.AuditEntries.OrderByDescending(x=>x.CreatedAt).Take(150).ToListAsync());}
