using AshkanCMS.Data; using AshkanCMS.Models;
namespace AshkanCMS.Services;
public class AuditService(AppDbContext db, IHttpContextAccessor http){public async Task LogAsync(string action,string entity,string details=""){db.AuditEntries.Add(new AuditEntry{UserName=http.HttpContext?.User?.Identity?.Name??"system",Action=action,Entity=entity,Details=details});await db.SaveChangesAsync();}}
