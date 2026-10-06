using System.Text.RegularExpressions; namespace AshkanCMS.Services;
public class SlugService{public string Make(string value){var s=value.Trim().ToLowerInvariant(); s=Regex.Replace(s,@"[^a-z0-9؀-ۿ]+","-"); return s.Trim('-');}}
