using SendGrid;
using SendGrid.Helpers.Mail;

namespace AmeliaRSVP.Core.Helpers;

public class SendgridHelper
{
    public static async Task<Response> SendEmail(string subject, string body)
    {
        var emailData = Config.Instance.Email;
        var client = new SendGridClient(emailData.SendgridApiKey);
        var emaiFrom = new EmailAddress(emailData.From, emailData.FromName);
        var toEmail = new EmailAddress(emailData.To);
        
        var msg = MailHelper.CreateSingleEmail(emaiFrom, toEmail, subject, body, null);
        return await client.SendEmailAsync(msg);
    }
}
