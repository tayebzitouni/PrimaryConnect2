//using FirebaseAdmin.Messaging;
//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Mvc;
//using PrimaryConnect.Data;
//using PrimaryConnect.Models;

//namespace PrimaryConnect.Controllers
//{
//    [Route("api/[controller]")]
//    [ApiController]
//    public class NotificationsController : ControllerBase
//    {
//        private readonly AppDbContext _context;

//        public NotificationsController(AppDbContext context)
//        {
//            _context = context;
//        }

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
//                        recipients.AddRange(_context.Administrators.Select(a => (a.Id, a.FcmToken, "admin")));

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
//                    recipients.AddRange(_context.Administrators.Where(a => request.UserIds.Contains(a.Id)).Select(a => (a.Id, a.FcmToken, "admin")));
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

//using Microsoft.AspNetCore.Mvc;
//using Microsoft.EntityFrameworkCore;
//using FirebaseAdmin.Messaging;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Linq.Expressions;
//using System.Threading.Tasks;
//using PrimaryConnect.Data;
//using PrimaryConnect.Dto;

//[ApiController]
//[Route("api/notifications")]
//public class NotificationController : ControllerBase
//{
//    private readonly AppDbContext _context;

//    public NotificationController(AppDbContext context)
//    {
//        _context = context;
//    }

//    // ✅ Send Notification
//    [HttpPost("send")]
//    public async Task<IActionResult> Send([FromBody] NotificationDto dto)
//    {
//        var sender = await _context.person.FindAsync(dto.SenderId);
//        if (sender == null) return BadRequest("Invalid sender");

//        var query = _context.person.AsQueryable();

//        // Exclude sender
//        query = query.Where(u => u.Id != dto.SenderId);

//        // Apply filters only if SendToAll is false
//        if (!dto.SendToAll)
//        {
//            if (dto.Roles?.Any() == true)
//                query = query.Where(u => dto.Roles.Contains(u.Role));

//            if (dto.Classes?.Any() == true)
//                query = query.Where(u => dto.Classes.Contains(u.Class));

//            if (dto.UserIds?.Any() == true)
//                query = query.Where(u => dto.UserIds.Contains(u.Id));
//        }

//        // Only users with FCM tokens
//        var recipients = await query
//            .Where(u => !string.IsNullOrEmpty(u.FcmToken))
//            .ToListAsync();

//        if (!recipients.Any())
//            return Ok(new { message = "No recipients matched the filters" });

//        // Send FCM
//        var tokens = recipients.Select(r => r.FcmToken).ToList();

//        var message = new MulticastMessage
//        {
//            Notification = new FirebaseAdmin.Messaging.Notification
//            {
//                Title = dto.Title,
//                Body = dto.Body
//            },
//            Tokens = tokens
//        };

//        await FirebaseMessaging.DefaultInstance.SendMulticastAsync(message);

//        // Store notifications in DB
//        var notificationEntities = recipients.Select(r => new Notification
//        {
//            Id = Guid.NewGuid(),
//            Title = dto.Title,
//            Body = dto.Body,
//            SenderId = dto.SenderId,
//            ReceiverId = r.Id,
//            SentAt = DateTime.UtcNow
//        });

//        _context.Notifications.AddRange(notificationEntities);
//        await _context.SaveChangesAsync();

//        return Ok(new { sent = recipients.Count });
//    }

//    // ✅ Get Notifications for a specific user
//    [HttpGet("user/{userId}")]
//    public async Task<IActionResult> GetUserNotifications(Guid userId)
//    {
//        var notifications = await _context.Notifications
//            .Where(n => n.ReceiverId == userId)
//            .OrderByDescending(n => n.SentAt)
//            .ToListAsync();

//        return Ok(notifications);
//    }
//}







