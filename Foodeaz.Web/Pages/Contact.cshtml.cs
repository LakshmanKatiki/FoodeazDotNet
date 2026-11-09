using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Net.Mail;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
namespace Foodeaz.Web.Pages;
public class ContactModel : PageModel {
 [BindProperty] public ContactInput Input {get;set;} = new(); public string? StatusMessage {get;set;} public bool Success {get;set;}
 public void OnGet() {}
 public async Task<IActionResult> OnPostAsync() { if(!ModelState.IsValid) return Page(); var cfg=HttpContext.RequestServices.GetRequiredService<IConfiguration>().GetSection("Smtp"); var host=cfg["Host"]; var username=cfg["Username"]; var password=cfg["Password"]; var from=cfg["FromEmail"]; var to=cfg["ToEmail"]; if(new[]{host,username,password,from,to}.Any(string.IsNullOrWhiteSpace)){ StatusMessage="Contact form email is not configured yet. Please configure SMTP settings using environment variables; no message was sent."; return Page(); } try { using var mail=new MailMessage(); mail.From=new MailAddress(from!,"Foodeaz Contact Form"); mail.To.Add(to!); mail.ReplyToList.Add(new MailAddress(Input.Email,Input.Name)); mail.Subject="Foodeaz Contact Mail From "+Input.Name; mail.Body=$"Name: {Input.Name}\nEmail: {Input.Email}\nPhone: {Input.Phone}\nSubject: {Input.Subject}\nMessage:\n{Input.Message}"; using var smtp=new SmtpClient(host, int.TryParse(cfg["Port"],out var port)?port:587){EnableSsl=bool.TryParse(cfg["EnableSsl"],out var ssl)&&ssl,Credentials=new NetworkCredential(username,password)}; await smtp.SendMailAsync(mail); Success=true; StatusMessage="Thank you. Your message has been sent successfully."; Input=new(); } catch { StatusMessage="We could not send your message right now. Please try again later."; } return Page(); }
 public class ContactInput { [Required, StringLength(100)] public string Name {get;set;}=""; [Required,EmailAddress,StringLength(254)] public string Email {get;set;}=""; [StringLength(30)] public string? Phone {get;set;} [Required,StringLength(150)] public string Subject {get;set;}=""; [Required,StringLength(5000)] public string Message {get;set;}=""; }
}
