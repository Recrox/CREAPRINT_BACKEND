using System.Net;
using System.Net.Mail;
using CreaPrintCore.Interfaces;
namespace CreaPrintApi.Services;

public class EmailService : IEmailService
{
    private readonly Serilog.ILogger _logger;
    private readonly IConfiguration _config;

    public EmailService(Serilog.ILogger logger, IConfiguration config)
    {
        _logger = logger;
        _config = config;
    }

    public async Task SendEmailAsync(string to, string subject, string body)
    {
        // Read SMTP configuration from app configuration
        var host = _config["Smtp:Host"];
        if (string.IsNullOrWhiteSpace(host))
        {
            // No SMTP configured - just log the email for development
            _logger.Information("[EmailService] (No SMTP) Sending email to {To}: {Subject}\n{Body}", to, subject, body);
            return;
        }

        int port = 25;
        if (!int.TryParse(_config["Smtp:Port"], out port))
        {
            port = 25;
        }

        var enableSsl = false;
        if (!bool.TryParse(_config["Smtp:EnableSsl"], out enableSsl))
        {
            enableSsl = false;
        }

        var username = _config["Smtp:Username"];
        var password = _config["Smtp:Password"];
        var from = _config["Smtp:From"] ?? username ?? "no-reply@local";

        try
        {
            using var message = new MailMessage();
            message.From = new MailAddress(from);
            message.To.Add(new MailAddress(to));
            message.Subject = subject;
            message.Body = body;
            message.IsBodyHtml = false;

            using var client = new SmtpClient(host, port)
            {
                EnableSsl = enableSsl,
            };

            if (!string.IsNullOrEmpty(username))
            {
                client.Credentials = new NetworkCredential(username, password);
            }

            _logger.Information("[EmailService] Sending email to {To} via SMTP {Host}:{Port}", to, host, port);
            await client.SendMailAsync(message);
            _logger.Information("[EmailService] Email sent to {To}", to);
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "[EmailService] Failed to send email to {To}. Falling back to log. Subject: {Subject}", to, subject);
            // fallback logging so no caller exception for email failures
            _logger.Information("[EmailService] (Fallback) Email to {To}: {Subject}\n{Body}", to, subject, body);
        }
    }
}
