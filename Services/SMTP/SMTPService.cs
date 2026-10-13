using System.Net;
using System.Net.Mail;
using System.Security.Cryptography;

namespace MyVromVromMachineAPI.SMTP
{
    public class SMTPService
    {
        public const string SenderEmail = "gegagego68@gmail.com"; //this is my personal Email so yeah. don't use it for anything else lol.
        public const string AppPassword = "rtiu hlat ktdl vuuf";

        public static string GenerateCode() => RandomNumberGenerator.GetInt32(0, 1_000_000).ToString("D6");
        public static async Task<string> SendEmail(string email, string username)
        {
            string code = GenerateCode();

            string htmlContent = $"""
                <div style="font-family:Arial">
                    <h2>Hello dear {username}!</h2>
                    <p>this is your verification code:</p>
                    <h1 style="letter-spacing:6px">{code}</h1>
                    <p>code will be valid for only 5 minutes.</p>
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
                Port = 4221,
                EnableSsl = true,
                Credentials = new NetworkCredential(SenderEmail, AppPassword)
            };
            await SMTPClient.SendMailAsync(mail);
            return htmlContent;
        }
    }
}
