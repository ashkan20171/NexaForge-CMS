using System.ComponentModel.DataAnnotations;
namespace AshkanCMS.Models;
public class CmsUser { public int Id {get;set;} [Required,MaxLength(80)] public string UserName {get;set;}=""; [Required] public string PasswordHash {get;set;}=""; [Required] public string Role {get;set;}="Editor"; public string DisplayName {get;set;}=""; public bool IsActive {get;set;}=true; }
public class LoginVm { [Required] public string UserName {get;set;}=""; [Required,DataType(DataType.Password)] public string Password {get;set;}=""; public bool RememberMe {get;set;} }
public class MenuItem { public int Id {get;set;} [Required] public string Label {get;set;}=""; [Required] public string Url {get;set;}="/"; public int SortOrder {get;set;} public bool IsVisible {get;set;}=true; }
