//using FirebaseAdmin.Messaging;
//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Mvc;
//using PrimaryConnect.Data;
//using PrimaryConnect.Models;

//namespace PrimaryConnect.Controllers
//{
//       [Route("api/[controller]")]
//        [ApiController]
//        public class NotificationsController : ControllerBase
//        {
//            private readonly AppDbContext _context;

//            public NotificationsController(AppDbContext context)
//            {
//                _context = context;
//            }

//        [HttpPost("send")]
//        public async Task<IActionResult> SendNotification([FromBody] NotificationRequest request)
//        {
//            var senderId = HttpContext.Session.GetString("UserId");
//            var senderRole = HttpContext.Session.GetString("UserRole");

//            if (string.IsNullOrEmpty(senderId) || string.IsNullOrEmpty(senderRole))
//            {
//                return Unauthorized("User is not logged in.");
//            }

//            List<(string UserId, string FcmToken, string Role)> recipients = new List<(string, string, string)>();

//            if (request.SendToAll)
//            {
//                recipients.AddRange(_context.Administrators.Select(a => (a.Id, a.FcmToken, "admin")));
//                recipients.AddRange(_context.Teachers.Select(t => (t.Id, t.FcmToken, "teacher")));
//                recipients.AddRange(_context.Parents.Select(p => (p.Id, p.FcmToken, "parent")));
//            }
//            else
//            {
//                if (request.TargetRoles != null && request.TargetRoles.Any())
//                {
//                    if (request.TargetRoles.Contains("admin"))
//                        recipients.AddRange(_context.Admins.Select(a => (a.Id, a.FcmToken, "admin")));

//                    if (request.TargetRoles.Contains("teacher"))
//                        recipients.AddRange(_context.Teachers.Select(t => (t.Id, t.FcmToken, "teacher")));

//                    if (request.TargetRoles.Contains("parent"))
//                        recipients.AddRange(_context.Parents.Select(p => (p.Id, p.FcmToken, "parent")));
//                }

//                if (request.TargetClasses != null && request.TargetClasses.Any())
//                {
//                    recipients.AddRange(_context.Parents
//                        .Where(p => request.TargetClasses.Contains(p.ClassName))
//                        .Select(p => (p.Id, p.FcmToken, "parent")));

//                    recipients.AddRange(_context.Teachers
//                        .Where(t => request.TargetClasses.Contains(t.ClassName))
//                        .Select(t => (t.Id, t.FcmToken, "teacher")));
//                }

//                if (request.UserIds != null && request.UserIds.Any())
//                {
//                    recipients.AddRange(_context.Admins.Where(a => request.UserIds.Contains(a.Id)).Select(a => (a.Id, a.FcmToken, "admin")));
//                    recipients.AddRange(_context.Teachers.Where(t => request.UserIds.Contains(t.Id)).Select(t => (t.Id, t.FcmToken, "teacher")));
//                    recipients.AddRange(_context.Parents.Where(p => request.UserIds.Contains(p.Id)).Select(p => (p.Id, p.FcmToken, "parent")));
//                }
//            }

//            if (!recipients.Any())
//            {
//                return NotFound("No users found to send the notification.");
//            }

//            try
//            {
//                var messages = recipients.Select(r => new FirebaseAdmin.Messaging.Message()
//                {
//                    Token = r.FcmToken,
//                    Notification = new FirebaseAdmin.Messaging.Notification
//                    {
//                        Title = request.Title,
//                        Body = request.Message
//                    }
//                }).ToList();

//                var response = await FirebaseAdmin.Messaging.FirebaseMessaging.DefaultInstance.SendAllAsync(messages);

//                var notifications = recipients.Select(r => new Notification
//                {
//                    Title = request.Title,
//                    Message = request.Message,
//                    SenderId = senderId,
//                    RecipientId = r.UserId,
//                    RecipientType = r.Role,
//                    IsRead = false,
//                    SentAt = DateTime.UtcNow
//                }).ToList();

//                _context.Notifications.AddRange(notifications);
//                await _context.SaveChangesAsync();

//                return Ok(new
//                {
//                    Message = "Notification sent successfully!",
//                    SentCount = response.SuccessCount,
//                    FailedCount = response.FailureCount
//                });
//            }
//            catch (Exception ex)
//            {
//                return StatusCode(500, new { Message = "Error sending notification", Error = ex.Message });
//            }
//        }

//    }

//}

