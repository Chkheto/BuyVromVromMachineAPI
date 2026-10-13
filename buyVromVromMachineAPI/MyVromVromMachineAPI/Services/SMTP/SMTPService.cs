using System.Net;
using System.Net.Mail;
using System.Security.Cryptography;

namespace MyVromVromMachineAPI.SMTP
{
    public class SMTPService
    {
        public const string SenderEmail = "";
        public const string AppPassword = "";

        public static string GenerateCode() => RandomNumberGenerator.GetInt32(0, 1_000_000).ToString("D6");
        public static async Task<string> SendEmail(string email, string username)
        {
            string code = GenerateCode();

            string htmlContent = $"""
                <div style="font-family:Arial">
                    <h2>Hallo, {username}!</h2>
                    <p>this is your verafication code:</p>
                    <h1 style="letter-spacing:6px">{code}</h1>
                    <p>code will work for 5 minutes.</p>
                </div>
                """;

            using var mail = new MailMessage();
            mail.From = new MailAddress(SenderEmail, username);
            mail.To.Add(email);
            mail.Subject = "your code is here";
            mail.Body = htmlContent;
            mail.IsBodyHtml = true;

            using var SMTPClient = new SmtpClient("smtp.google.com")
            {
                Port = 587,
                EnableSsl = true,
                Credentials = new NetworkCredential(SenderEmail, AppPassword)
            };
            await SMTPClient.SendMailAsync(mail);
            return htmlContent;
        }
    }
}
