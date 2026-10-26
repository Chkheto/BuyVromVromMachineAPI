using System.Net;
using System.Security.Cryptography;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;

namespace MyVromVromMachineAPI.SMTP
{
    public class SMTPService
    {
        public const string SenderEmail = "chkhetianisandro@gmail.com";
        public const string AppPassword = "YOUR-NEW-APP-PASSWORD";

        public static string GenerateCode() => RandomNumberGenerator.GetInt32(0, 1_000_000).ToString("D6");

        public static async Task SendEmail(string email, string username, string code)
        {

            string safeName = WebUtility.HtmlEncode(username);

            string htmlContent = $"""
                <div style="font-family:Arial">
                    <h2>Hello, {safeName}!</h2>
                    <p>This is your verification code:</p>
                    <h1 style="letter-spacing:6px">{code}</h1>
                    <p>The code will work for 5 minutes.</p>
                </div>
                """;

            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("BuyVromVrom", SenderEmail));
            message.To.Add(MailboxAddress.Parse(email));
            message.Subject = "Your verification code";
            message.Body = new BodyBuilder { HtmlBody = htmlContent }.ToMessageBody();

            using var client = new SmtpClient();
            await client.ConnectAsync("smtp.gmail.com", 587, SecureSocketOptions.StartTls);
            await client.AuthenticateAsync(SenderEmail, AppPassword);
            await client.SendAsync(message);
            await client.DisconnectAsync(true);

        }
    }
}