using System.IO.Compression;
using System.Text.Json;
using AshkanCMS.Data;
using AshkanCMS.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AshkanCMS.Areas.Admin.Controllers;

[Area("Admin"), Authorize(Roles="Administrator")]
public class EngineeringController(AppDbContext db, IWebHostEnvironment env) : Controller
{
    public async Task<IActionResult> Index()
    {
        ViewBag.Tasks = await db.BackgroundTasks.OrderByDescending(x=>x.CreatedAt).Take(8).ToListAsync();
        ViewBag.Backups = await db.BackupSnapshots.OrderByDescending(x=>x.CreatedAt).Take(6).ToListAsync();
        ViewBag.Deliveries = await db.WebhookDeliveries.OrderByDescending(x=>x.CreatedAt).Take(6).ToListAsync();
        ViewBag.Extensions = await db.ExtensionPackages.CountAsync(x=>x.IsEnabled);
        ViewBag.Queued = await db.BackgroundTasks.CountAsync(x=>x.Status=="Queued");
        ViewBag.Errors = await db.ErrorLogs.CountAsync(x=>x.CreatedAt>=DateTime.UtcNow.AddHours(-24));
        var uploads=Path.Combine(env.WebRootPath,"uploads"); ViewBag.MediaBytes = Directory.Exists(uploads) ? Directory.EnumerateFiles(uploads,"*",SearchOption.AllDirectories).Sum(f=>new FileInfo(f).Length) : 0L;
        return View();
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> QueueMaintenance(string taskType="Cache refresh")
    {
        db.BackgroundTasks.Add(new BackgroundTaskItem{Name=taskType,TaskType="Operations",Status="Queued",Details="Queued from Production Engineering Center"});
        await db.SaveChangesAsync(); TempData["Success"]="Maintenance task queued."; return RedirectToAction(nameof(Index));
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateBackup()
    {
        var dir=Path.Combine(env.ContentRootPath,"App_Data","backups"); Directory.CreateDirectory(dir);
        var stamp=DateTime.UtcNow.ToString("yyyyMMdd-HHmmss"); var fileName=$"nexaforge-backup-{stamp}.zip"; var path=Path.Combine(dir,fileName);
        var payload=new { generatedAtUtc=DateTime.UtcNow, posts=await db.Posts.AsNoTracking().ToListAsync(), pages=await db.Pages.AsNoTracking().ToListAsync(), categories=await db.Categories.AsNoTracking().ToListAsync(), settings=await db.Settings.AsNoTracking().ToListAsync(), menus=await db.MenuItems.AsNoTracking().ToListAsync() };
        await using(var fs=System.IO.File.Create(path)) using(var zip=new ZipArchive(fs,ZipArchiveMode.Create)){var e=zip.CreateEntry("content.json",CompressionLevel.Optimal); await using var es=e.Open(); await JsonSerializer.SerializeAsync(es,payload,new JsonSerializerOptions{WriteIndented=true});}
        var info=new FileInfo(path); db.BackupSnapshots.Add(new BackupSnapshot{Name=$"Content snapshot {stamp}",FileName=fileName,SizeBytes=info.Length,CreatedBy=User.Identity?.Name??"system"}); await db.SaveChangesAsync();
        TempData["Success"]="Portable content backup created."; return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> DownloadBackup(int id)
    {
        var b=await db.BackupSnapshots.FindAsync(id); if(b==null)return NotFound(); var path=Path.Combine(env.ContentRootPath,"App_Data","backups",Path.GetFileName(b.FileName)); if(!System.IO.File.Exists(path))return NotFound(); return PhysicalFile(path,"application/zip",b.FileName);
    }

    public IActionResult ApiExplorer()=>View();
}
