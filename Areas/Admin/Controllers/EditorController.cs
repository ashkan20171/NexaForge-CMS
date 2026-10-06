using AshkanCMS.Data;
using AshkanCMS.Models;
using AshkanCMS.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AshkanCMS.Areas.Admin.Controllers;

[Area("Admin"), Authorize]
public class EditorController(AppDbContext db, AiAssistantService ai) : Controller
{
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Ai(string tool, string text, CancellationToken ct)
    {
        var cfg = await db.AiSettings.FirstOrDefaultAsync(ct) ?? new AiSetting();
        var result = await ai.GenerateAsync(cfg, tool, text ?? "", ct);
        db.AiHistory.Add(new AiHistoryItem { Tool = $"Editor:{tool}", Prompt = text ?? "", Result = result, UserName = User.Identity?.Name ?? "editor" });
        await db.SaveChangesAsync(ct);
        return Json(new { ok = true, result });
    }

    [HttpGet]
    public async Task<IActionResult> Revisions(int postId)
    {
        var post = await db.Posts.AsNoTracking().FirstOrDefaultAsync(x => x.Id == postId);
        if (post == null) return NotFound();
        ViewBag.Post = post;
        return View(await db.Revisions.Where(x => x.PostId == postId).OrderByDescending(x => x.CreatedAt).ToListAsync());
    }

    [HttpGet]
    public async Task<IActionResult> Revision(int id)
    {
        var rev = await db.Revisions.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
        if (rev == null) return NotFound();
        return Json(new { rev.Id, rev.Title, rev.Body, rev.CreatedAt, rev.CreatedBy });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> RestoreRevision(int id)
    {
        var rev = await db.Revisions.FindAsync(id);
        if (rev == null) return NotFound();
        var post = await db.Posts.FindAsync(rev.PostId);
        if (post == null) return NotFound();
        db.Revisions.Add(new ContentRevision { PostId = post.Id, Title = post.Title, Body = post.Body, CreatedBy = User.Identity?.Name ?? "editor" });
        post.Title = rev.Title; post.Body = rev.Body;
        await db.SaveChangesAsync();
        TempData["Success"] = "Revision restored. The previous current version was preserved.";
        return RedirectToAction("Edit", "Posts", new { id = post.Id });
    }
}
