using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using PrimaryConnect;
using PrimaryConnect.Data;
using PrimaryConnect.Dto;
using PrimaryConnect.Models;
using System.Security.Claims;

namespace PrimaryConnect.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RealTimeController : ControllerBase
    {
        private readonly IHubContext<ChatHub> _hubContext;
        private readonly AppDbContext _context;
        public RealTimeController(IHubContext<ChatHub> hub,AppDbContext db)
        {
            _hubContext = hub;
            _context = db;
        }

        //[HttpPost("send")]
        //public async Task<IActionResult> SendMessage([FromBody] ChatMessageDto message)
        //{
        //    if (string.IsNullOrWhiteSpace(message.Text) || string.IsNullOrWhiteSpace(message.UserId.ToString()))
        //        return BadRequest("User and message text cannot be empty.");

        //    await _hubContext.Clients.All.SendAsync("NewMessage", message.Id, message.Text);
        //    return Ok(new { Status = "Message Sent" });
        //}

        //[HttpPost("send")]
        //public async Task<IActionResult> SendMessage([FromBody] ChatMessageDto message)
        //{
        //    //var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        //    //if (userIdClaim == null)
        //    //    return Unauthorized("يجب تسجيل الدخول أولًا.");
        //    try
        //    {
        //        //int userId = /*int.Parse(userIdClaim)*/  int.Parse(HttpContext.Session.GetString("UserId"));
        //        int userId = 3; 
        //        if (string.IsNullOrWhiteSpace(message.Text))
        //            return BadRequest("لا يمكن أن يكون محتوى الرسالة فارغًا.");

        //        // حفظ الرسالة في قاعدة البيانات
        //        var chatMessage = new ChatMessage
        //        {
        //            Text = message.Text,
        //            UserId = userId
        //        };

        //        _context.chatMessages.Add(chatMessage);
        //        await _context.SaveChangesAsync();

        //        // إرسال الرسالة لجميع المستخدمين عبر SignalR
        //        await _hubContext.Clients.All.SendAsync("NewMessage", new
        //        {
        //            Id = chatMessage.Id,
        //            Text = chatMessage.Text,
        //            UserId = chatMessage.UserId
        //        });

        //        return Ok(new { Status = "تم إرسال الرسالة وتخزينها." });
        //    }
        //    catch(Exception ex)
        //    {
        //        Console.WriteLine(ex.Message);
        //        Console.WriteLine("fuck");
        //        return StatusCode(500, new { error = ex.Message });
        //    }
        //    }

        [HttpPost("send")]
        public async Task<IActionResult> SendMessage([FromBody] ChatMessageDto message)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(message.Text))
                    return BadRequest("Message cannot be empty");

                var chatMessage = new ChatMessage
                {
                    Text = message.Text,
                    UserName = message.UserName,  // This is now required
                                                  // UserId = userId // Only if you have authentication
                };

                _context.chatMessages.Add(chatMessage);
                await _context.SaveChangesAsync();

                await _hubContext.Clients.All.SendAsync("NewMessage", new
                {
                    Id = chatMessage.Id,
                    Text = chatMessage.Text,
                    UserName = chatMessage.UserName,
                    Timestamp = chatMessage.Timestamp.ToString("HH:mm")
                });

                return Ok(new
                {
                    Status = "Success",
                    MessageId = chatMessage.Id
                });
            }
            catch (Exception ex)
            {
                // Add proper logging here
                Console.WriteLine($"Error: {ex.Message}\n{ex.StackTrace}");
                return StatusCode(500, new
                {
                    error = "Internal server error",
                    details = ex.Message  // Only in development!
                });
            }
        }



        //[Authorize]
        //[HttpPost("send")]
        //public async Task<IActionResult> SendMessage([FromBody] ChatMessageDto message)
        //{
        //    var userId = HttpContext.Session.GetInt32("UserId");
        //    if (userId == null)
        //        return Unauthorized("يجب تسجيل الدخول أولًا.");

        //    if (string.IsNullOrWhiteSpace(message.Text))
        //        return BadRequest("لا يمكن أن يكون محتوى الرسالة فارغًا.");

        //    // حفظ الرسالة في قاعدة البيانات
        //    var chatMessage = new ChatMessage
        //    {
        //        Text = message.Text,
        //        UserId = userId.Value
        //    };

        //    _context.chatMessages.Add(chatMessage);
        //    await _context.SaveChangesAsync();

        //    // إرسال الرسالة لجميع المستخدمين عبر SignalR
        //    await _hubContext.Clients.All.SendAsync("NewMessage", new
        //    {
        //        Id = chatMessage.Id,
        //        Text = chatMessage.Text,
        //        UserId = chatMessage.UserId
        //    });

        //    return Ok(new { Status = "تم إرسال الرسالة وتخزينها." });
        //}

        [HttpGet("history")]
        public async Task<IActionResult> GetChatHistory()
        {
            var messages = await _context.chatMessages
                .OrderBy(m => m.Id) // ترتيب الرسائل تصاعديًا
                .Select(m => new
                {
                    Id = m.Id,
                    Text = m.Text,
                   // UserId = m.UserId
                })
                .ToListAsync();

            return Ok(messages);
        }


    }

}













//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.AspNetCore.SignalR;

//namespace PrimaryConnect.Controllers
//{
//    [Route("api/[controller]")]
//    [ApiController]
//    public class RealTimeController : ControllerBase
//    {
//        private readonly IHubContext<ChatHub> _hubContext;

//        public RealTimeController(IHubContext<ChatHub> hubContext)
//        {
//            _hubContext = hubContext ?? throw new ArgumentNullException(nameof(hubContext));
//        }

//        [HttpPost("SendMessage")]
//        public async Task<IActionResult> SendMessage([FromBody] ChatMessage message)
//        {
//            if (string.IsNullOrWhiteSpace(message.UserName) || string.IsNullOrWhiteSpace(message.Text))
//                return BadRequest("User and message text cannot be empty.");

//            Console.WriteLine($"📩 Message reçu : {message.UserName} - {message.Text}"); // 🔥 Débogage

//            await _hubContext.Clients.All.SendAsync("NewMessage", message.UserName, message.Text);
//            return Ok(new { Status = "Message Sent" });
//        }
//    }

//    public class ChatMessage
//    {
//        public string Text { get; set; }
//        public string UserName { get; set; }
//    }
//}

