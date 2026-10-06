using AshkanCMS.Data;
using Microsoft.EntityFrameworkCore;
namespace AshkanCMS.Services;
public class HookRuntime(AppDbContext db)
{
    public async Task<IReadOnlyList<string>> GetSubscribersAsync(string hook, CancellationToken ct=default) =>
        await db.ExtensionHooks.AsNoTracking().Where(x=>x.IsEnabled && x.HookName==hook).OrderBy(x=>x.Priority).Select(x=>x.PackageId).ToListAsync(ct);
    public async Task RecordAsync(string hook,string details,CancellationToken ct=default)
    {
        var packages=await GetSubscribersAsync(hook,ct);
        db.AuditEntries.Add(new Models.AuditEntry{Action=$"Hook:{hook}",Entity="ExtensionRuntime",Details=$"Subscribers: {string.Join(", ",packages)} · {details}"});
        await db.SaveChangesAsync(ct);
    }
}
