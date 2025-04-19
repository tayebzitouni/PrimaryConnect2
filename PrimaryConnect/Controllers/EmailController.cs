using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Outlook = Microsoft.Office.Interop.Outlook;
namespace PrimaryConnect.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmailController : ControllerBase
    {
        private readonly IEmailService _emailService;

        public EmailController(IEmailService emailService)
        {
            _emailService = emailService;
        }

        //[HttpPost("send-otp")]
        //public async Task<IActionResult> SendOtp([FromBody] ForgotPasswordRequest request)
        //{
        //    // You might want to check if user with this email exists first
        //    // Generate a 6-digit OTP
        //    var otp = new Random().Next(100000, 999999).ToString();

        //    // TODO: Store OTP temporarily (in DB or in-memory cache like Redis)

        //    await _emailService.SendOtpEmailAsync(request.Email, otp);

        //    return Ok(new { message = "OTP sent to email." });
        //}
        [HttpPost("send-otp")]
        public void SendOtp([FromBody] ForgotPasswordRequest request)
        {
            try
            {
                Outlook.Application outlookApp = new Outlook.Application();
                Outlook.MailItem mailItem = (Outlook.MailItem)outlookApp.CreateItem(Outlook.OlItemType.olMailItem);
                mailItem.Subject = "Test Email from C#";
                mailItem.To = "tayebzitouni1122111@gmail.com";  // Change this to the actual recipient's email address
                mailItem.Body = "Hello, this is a test email sent from a C# application using Outlook Interop.";
                mailItem.Importance = Outlook.OlImportance.olImportanceHigh;
                mailItem.Display(false);  // Set to true to display the email before sending
                mailItem.Send();
                Console.WriteLine("Email sent successfully!");
                Console.ReadKey();

            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
        }
    }

    public class ForgotPasswordRequest
    {
        public string Email { get; set; }
    }
}

