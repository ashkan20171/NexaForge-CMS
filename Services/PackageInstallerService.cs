using System.IO.Compression;
using System.Text.Json;
using AshkanCMS.Data;
using AshkanCMS.Models;
using Microsoft.EntityFrameworkCore;

namespace AshkanCMS.Services;
public class PackageInstallerService(IWebHostEnvironment env, AppDbContext db)
{
    private static readonly HashSet<string> Allowed = new(StringComparer.OrdinalIgnoreCase){".json",".css",".js",".html",".htm",".md",".txt",".png",".jpg",".jpeg",".webp",".svg",".woff",".woff2"};
    public async Task<(bool ok,string message)> InstallAsync(IFormFile file)
    {
        if(file.Length==0 || file.Length>10*1024*1024) return (false,"Package must be a ZIP smaller than 10 MB.");
        if(!string.Equals(Path.GetExtension(file.FileName),".zip",StringComparison.OrdinalIgnoreCase)) return (false,"Only .zip packages are accepted.");
        var temp=Path.Combine(Path.GetTempPath(),Guid.NewGuid()+".zip"); await using(var fs=File.Create(temp)) await file.CopyToAsync(fs);
        try{
            using var zip=ZipFile.OpenRead(temp);
            var manifestEntry=zip.Entries.FirstOrDefault(e=>e.FullName.Equals("ashkan-package.json",StringComparison.OrdinalIgnoreCase));
            if(manifestEntry is null) return (false,"ashkan-package.json was not found at the package root.");
            PackageManifest? m; await using(var st=manifestEntry.Open()) m=await JsonSerializer.DeserializeAsync<PackageManifest>(st,new JsonSerializerOptions{PropertyNameCaseInsensitive=true});
            if(m is null || string.IsNullOrWhiteSpace(m.Id)||string.IsNullOrWhiteSpace(m.Name)) return (false,"Package manifest is invalid.");
            var type=(m.Type??"").Trim(); if(type is not ("Plugin" or "Theme" or "Module")) return (false,"Type must be Plugin, Theme or Module.");
            var safeId=new string(m.Id.Where(c=>char.IsLetterOrDigit(c)||c is '-' or '_').ToArray()); if(safeId.Length<2) return (false,"Package id is invalid.");
            if(await db.ExtensionPackages.AnyAsync(x=>x.PackageId==safeId)) return (false,"A package with this id is already installed. Uninstall it first.");
            foreach(var e in zip.Entries){ if(string.IsNullOrEmpty(e.Name))continue; if(e.Length>5*1024*1024)return(false,"A package file exceeds the 5 MB per-file limit."); if(!Allowed.Contains(Path.GetExtension(e.Name)))return(false,$"File type not allowed: {e.Name}"); }
            var root=Path.Combine(env.ContentRootPath,"App_Data","Extensions",type.ToLowerInvariant()+"s",safeId); Directory.CreateDirectory(root);
            foreach(var e in zip.Entries){ if(string.IsNullOrEmpty(e.Name))continue; var dest=Path.GetFullPath(Path.Combine(root,e.FullName)); if(!dest.StartsWith(Path.GetFullPath(root)+Path.DirectorySeparatorChar,StringComparison.Ordinal))return(false,"Unsafe package path detected."); Directory.CreateDirectory(Path.GetDirectoryName(dest)!); await using var input=e.Open(); await using var output=File.Create(dest); await input.CopyToAsync(output); }
            db.ExtensionPackages.Add(new ExtensionPackage{PackageId=safeId,Name=m.Name,Type=type,Version=m.Version??"1.0.0",Author=m.Author??"",Description=m.Description??"",InstallPath=root,IsEnabled=true});
            if(type=="Plugin")db.Plugins.Add(new PluginDefinition{Name=m.Name,Description=m.Description??"Installed package",Version=m.Version??"1.0.0",IsEnabled=true});
            if(type=="Theme")db.Themes.Add(new ThemeDefinition{Name=m.Name,Description=m.Description??"Installed theme",PreviewClass="theme-"+safeId,IsActive=false});
            await db.SaveChangesAsync(); return(true,$"{type} ‘{m.Name}’ installed successfully.");
        }catch(InvalidDataException){return(false,"The ZIP package is damaged or invalid.");}finally{try{File.Delete(temp);}catch{}}
    }
    public async Task<(bool ok,string message)> UninstallAsync(int id){var x=await db.ExtensionPackages.FindAsync(id);if(x is null)return(false,"Package not found."); try{if(Directory.Exists(x.InstallPath))Directory.Delete(x.InstallPath,true);}catch{return(false,"Package files are currently locked.");} db.ExtensionPackages.Remove(x); await db.SaveChangesAsync(); return(true,$"{x.Name} uninstalled.");}
    private sealed class PackageManifest{public string Id{get;set;}="";public string Name{get;set;}="";public string? Type{get;set;}public string? Version{get;set;}public string? Author{get;set;}public string? Description{get;set;}}
}
