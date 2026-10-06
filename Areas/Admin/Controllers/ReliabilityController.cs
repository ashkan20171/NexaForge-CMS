using System.Text.Json;
using AshkanCMS.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AshkanCMS.Areas.Admin.Controllers;

[Area("Admin"), Authorize(Roles = "Administrator")]
public class ReliabilityController(AppDbContext db, IWebHostEnvironment env) : Controller
{
    public async Task<IActionResult> Index()
    {
        ViewBag.Posts = await db.Posts.CountAsync();
        ViewBag.Media = await db.Media.CountAsync();
        ViewBag.Users = await db.Users.CountAsync();
        ViewBag.Errors24h = await db.ErrorLogs.CountAsync(x => x.CreatedAt >= DateTime.UtcNow.AddHours(-24));
        ViewBag.FailedLogins24h = await db.LoginEvents.CountAsync(x => !x.Success && x.CreatedAt >= DateTime.UtcNow.AddHours(-24));
        ViewBag.PendingJobs = await db.ScheduledJobs.CountAsync(x => x.Status == "Queued");
        ViewBag.EnabledExtensions = await db.ExtensionPackages.CountAsync(x => x.IsEnabled);
        ViewBag.LastErrors = await db.ErrorLogs.OrderByDescending(x => x.CreatedAt).Take(8).ToListAsync();
        ViewBag.LastLogins = await db.LoginEvents.OrderByDescending(x => x.CreatedAt).Take(8).ToListAsync();
        ViewBag.EnvironmentName = env.EnvironmentName;
        return View();
    }

    [HttpGet]
    public async Task<IActionResult> Diagnostics()
    {
        var report = new
        {
            product = "NexaForge CMS",
            generatedAtUtc = DateTime.UtcNow,
            environment = env.EnvironmentName,
            framework = System.Runtime.InteropServices.RuntimeInformation.FrameworkDescription,
            os = System.Runtime.InteropServices.RuntimeInformation.OSDescription,
            content = new { posts = await db.Posts.CountAsync(), pages = await db.Pages.CountAsync(), media = await db.Media.CountAsync() },
            platform = new { users = await db.Users.CountAsync(), extensions = await db.ExtensionPackages.CountAsync(), queuedJobs = await db.ScheduledJobs.CountAsync(x => x.Status == "Queued") },
            reliability = new { errors24h = await db.ErrorLogs.CountAsync(x => x.CreatedAt >= DateTime.UtcNow.AddHours(-24)), failedLogins24h = await db.LoginEvents.CountAsync(x => !x.Success && x.CreatedAt >= DateTime.UtcNow.AddHours(-24)) }
        };
        var bytes = JsonSerializer.SerializeToUtf8Bytes(report, new JsonSerializerOptions { WriteIndented = true });
        return File(bytes, "application/json", $"nexaforge-diagnostics-{DateTime.UtcNow:yyyyMMdd-HHmm}.json");
    }
}
