using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using VetClinic.Api.Data;
using VetClinic.Api.Hubs;
using VetClinic.Api.Interfaces;
using VetClinic.Api.Models;
using VetClinic.Shared.Enums;
using VetClinic.Shared.Responses;

namespace VetClinic.Api.Services;

public class NotificationService : INotificationService
{
    private readonly AppDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IHubContext<NotificationHub> _hubContext;

    public NotificationService(
        AppDbContext context,
        UserManager<ApplicationUser> userManager,
        IHubContext<NotificationHub> hubContext)
    {
        _context = context;
        _userManager = userManager;
        _hubContext = hubContext;
    }

    public async Task<List<NotificationResponse>> GetMyAsync(ClaimsPrincipal user)
    {
        var userId = _userManager.GetUserId(user);
        if (string.IsNullOrWhiteSpace(userId))
        {
            return [];
        }

        var notifications = await _context.Notifications
            .AsNoTracking()
            .Where(n => n.UserId == userId)
            .OrderByDescending(n => n.CreatedAt)
            .ToListAsync();

        return notifications.Select(MapNotification).ToList();
    }

    public async Task<bool> MarkAsReadAsync(int id, ClaimsPrincipal user)
    {
        var userId = _userManager.GetUserId(user);
        if (string.IsNullOrWhiteSpace(userId))
        {
            return false;
        }

        var notification = await _context.Notifications
            .FirstOrDefaultAsync(n => n.Id == id && n.UserId == userId);

        if (notification is null)
        {
            return false;
        }

        notification.IsRead = true;
        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<bool> MarkAllAsReadAsync(ClaimsPrincipal user)
    {
        var userId = _userManager.GetUserId(user);
        if (string.IsNullOrWhiteSpace(userId))
        {
            return false;
        }

        var currentUser = await _userManager.FindByIdAsync(userId);
        if (currentUser is null)
        {
            return false;
        }

        var notifications = await _context.Notifications
            .Where(n => n.UserId == userId && !n.IsRead)
            .ToListAsync();

        foreach (var notification in notifications)
        {
            notification.IsRead = true;
        }

        await _context.SaveChangesAsync();

        return true;
    }

    public async Task CreateAsync(string userId, string title, string message, NotificationType type, string? eventName = null)
    {
        if (string.IsNullOrWhiteSpace(userId))
        {
            return;
        }

        var notification = new Notification
        {
            UserId = userId,
            Title = title,
            Message = message,
            Type = type
        };

        _context.Notifications.Add(notification);
        await _context.SaveChangesAsync();

        var response = MapNotification(notification);
        await _hubContext.Clients.Group(userId).SendAsync("ReceiveNotification", response);

        if (!string.IsNullOrWhiteSpace(eventName))
        {
            await _hubContext.Clients.Group(userId).SendAsync(eventName, response);
        }
    }

    private static NotificationResponse MapNotification(Notification notification)
    {
        return new NotificationResponse
        {
            Id = notification.Id,
            UserId = notification.UserId,
            Title = notification.Title,
            Message = notification.Message,
            Type = notification.Type,
            IsRead = notification.IsRead,
            CreatedAt = notification.CreatedAt
        };
    }
}
