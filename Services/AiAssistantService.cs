using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using AshkanCMS.Models;
namespace AshkanCMS.Services;
public class AiAssistantService(IHttpClientFactory factory){
 public async Task<string> GenerateAsync(AiSetting cfg,string tool,string prompt,CancellationToken ct=default){
  if(!string.IsNullOrWhiteSpace(cfg.Endpoint) && !string.IsNullOrWhiteSpace(cfg.ApiKey)){
   try{using var req=new HttpRequestMessage(HttpMethod.Post,cfg.Endpoint);req.Headers.Authorization=new AuthenticationHeaderValue("Bearer",cfg.ApiKey);var payload=new{model=cfg.Model,messages=new[]{new{role="system",content="You are Ashkan CMS Editorial Copilot. Return concise, publication-ready content."},new{role="user",content=$"Tool: {tool}\n{prompt}"}},temperature=cfg.Temperature};req.Content=new StringContent(JsonSerializer.Serialize(payload),Encoding.UTF8,"application/json");var res=await factory.CreateClient().SendAsync(req,ct);res.EnsureSuccessStatusCode();using var doc=JsonDocument.Parse(await res.Content.ReadAsStringAsync(ct));return doc.RootElement.GetProperty("choices")[0].GetProperty("message").GetProperty("content").GetString()??"No response.";}catch(Exception ex){return $"AI provider error: {ex.Message}";}
  }
  var clean=(prompt??"").Trim(); if(clean.Length==0)return "Add some source text or a topic first.";
  return tool switch{
   "SEO"=>$"SEO title: {Trim(clean,58)}\nMeta description: {Trim(clean,150)}\nSuggested keywords: ashkan cms, content, publishing, {Keywords(clean)}",
   "Summary"=>$"Summary: {Trim(clean,220)}",
   "Ideas"=>$"1. A practical guide to {Trim(clean,55)}\n2. Common mistakes and better patterns\n3. A checklist readers can use today\n4. Case study and measurable outcomes\n5. FAQ and next steps",
   "Rewrite"=>$"Refined draft:\n{clean}\n\nEditorial note: tighten the opening, use descriptive headings, short paragraphs and a clear call to action.",
   "TranslateFA"=>$"ترجمه پیشنهادی فارسی (برای ترجمه دقیق‌تر، یک ارائه‌دهنده AI را در تنظیمات متصل کنید):\n{clean}",
   "TranslateEN"=>$"Suggested English translation (connect an AI provider for full translation):\n{clean}",
   "Analyze"=>$"Editorial analysis\n• Clarity: keep one main idea per paragraph.\n• Structure: add descriptive H2/H3 headings.\n• SEO: place the primary topic naturally in the title and introduction.\n• Accessibility: use meaningful link text and image alt text.\n• Readability: shorten long sentences and end with a clear next action.",
   _=>$"Ashkan AI suggests focusing this content around: {Trim(clean,180)}\n\nRecommended next step: define the audience, desired action, and one primary keyword before publishing."
  };
 }
 static string Trim(string s,int n)=>s.Length<=n?s:s[..n].Trim()+"…";
 static string Keywords(string s)=>string.Join(", ",s.Split(' ',StringSplitOptions.RemoveEmptyEntries).Where(x=>x.Length>4).Distinct(StringComparer.OrdinalIgnoreCase).Take(5));
}
