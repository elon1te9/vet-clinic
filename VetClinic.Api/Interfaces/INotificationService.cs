using System.Security.Claims;
using VetClinic.Shared.Enums;
using VetClinic.Shared.Responses;

namespace VetClinic.Api.Interfaces;

public interface INotificationService
{
    Task<List<NotificationResponse>> GetMyAsync(ClaimsPrincipal user);
    Task<bool> MarkAsReadAsync(int id, ClaimsPrincipal user);
    Task<bool> MarkAllAsReadAsync(ClaimsPrincipal user);
    Task CreateAsync(string userId, string title, string message, NotificationType type, string? eventName = null);
}
