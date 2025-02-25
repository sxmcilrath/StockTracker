using Microsoft.AspNetCore.Mvc;
//NO LONGER USED
namespace StockTracker.BackgroundServices
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmailController : ControllerBase
    {
        private readonly IEmailService _emailService;

        //constructor inject, this automatically creates an instance of IEmailService when
        //the controller is created
        public EmailController(IEmailService emailService)
        {
            _emailService = emailService
                ?? throw new ArgumentNullException(nameof(emailService));
        }

        [HttpGet("singleemail")]
        public async Task<IActionResult> SendSingleEmail()
        {
            EmailMetadata emailMetadata = new("samuel.xavier.mac@gmail.com",
                "FluentEmail Test email",
                "This is a test email from FluentEmail.");

            await _emailService.Send(emailMetadata);

            return Ok();
        }
    }
}
