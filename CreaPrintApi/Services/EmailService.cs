using System.Threading.Tasks;
using CreaPrintCore.Interfaces;
using Serilog;

namespace CreaPrintApi.Services
{
 public class EmailService : IEmailService
 {
 private readonly Serilog.ILogger _logger;
 public EmailService(Serilog.ILogger logger)
 {
 _logger = logger;
 }

 public Task SendEmailAsync(string to, string subject, string body)
 {
 // For now just log the email. In production replace with SMTP or third-party provider.
 _logger.Information("Sending email to {To}: {Subject}\n{Body}", to, subject, body);
 return Task.CompletedTask;
 }
 }
}
