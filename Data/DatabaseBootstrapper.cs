using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AshkanCMS.Data;

/// <summary>
/// Keeps SQLite installations created by older portfolio stages usable when a later
/// stage introduces new entity tables. EnsureCreated() intentionally does not update
/// an existing database, so we replay EF Core's idempotent-safe CREATE statements.
/// This preserves existing data while creating tables/indexes that do not yet exist.
/// </summary>
public static class DatabaseBootstrapper
{
    public static void Initialize(AppDbContext db, ILogger logger)
    {
        db.Database.EnsureCreated();

        if (!db.Database.IsSqlite())
            return;

        var createScript = db.Database.GenerateCreateScript();
        var safeScript = MakeSqliteCreateScriptIdempotent(createScript);

        try
        {
            db.Database.ExecuteSqlRaw(safeScript);
            logger.LogInformation("SQLite schema compatibility check completed successfully.");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "SQLite schema compatibility check failed.");
            throw;
        }
    }

    private static string MakeSqliteCreateScriptIdempotent(string script)
    {
        return script
            .Replace("CREATE TABLE \"", "CREATE TABLE IF NOT EXISTS \"")
            .Replace("CREATE UNIQUE INDEX \"", "CREATE UNIQUE INDEX IF NOT EXISTS \"")
            .Replace("CREATE INDEX \"", "CREATE INDEX IF NOT EXISTS \"");
    }
}
