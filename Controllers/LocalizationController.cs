using Microsoft.AspNetCore.Mvc;
namespace AshkanCMS.Controllers;
public class LocalizationController : Controller
{
 [HttpPost,ValidateAntiForgeryToken]
 public IActionResult SetLanguage(string culture,string? returnUrl=null)
 {
   culture=culture=="fa"?"fa":"en";
   Response.Cookies.Append("ashkan.lang",culture,new CookieOptions{Expires=DateTimeOffset.UtcNow.AddYears(1),IsEssential=true,SameSite=SameSiteMode.Lax});
   return LocalRedirect(string.IsNullOrWhiteSpace(returnUrl)?"/":returnUrl);
 }
}
