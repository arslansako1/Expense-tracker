


using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MyApiProject.Migrations;

public class NotificationsService(UserManager<ApplicationUser> _userManager, AppDbContext _context) : INotificationsService
{
    public async Task<ServiceResult<List<Notification>>> GetAll(ClaimsPrincipal claimsPrincipal)
    {
        var user = await _userManager.GetUserAsync(claimsPrincipal);
        if (user is null)
        {
            return ServiceResult<List<Notification>>.Fail("User not found");
        }

        var allNofications = await _context.Notifications
        .Where(n => n.UserId == user.Id && n.IsDeleted ==false)
        .OrderByDescending(n => n.CreatedAt).ToListAsync();

        if (allNofications is null)
        {
            return ServiceResult<List<Notification>>.Fail("No nofications are generated yet");
        }

        return ServiceResult<List<Notification>>.Ok(allNofications);
        
    }

    public async Task<ServiceResult<bool>> Delete(int noficationId, ClaimsPrincipal claimsPrincipal)
    {
    var user = await _userManager.GetUserAsync(claimsPrincipal);
        if (user is null)
        {
            return ServiceResult<bool>.Fail("User not found");
        }

        var nofication = await _context.Notifications.FirstOrDefaultAsync(n => n.Id == noficationId && n.IsDeleted == false);
        if (nofication is null)
        {
            return ServiceResult<bool>.Fail("Nofication not found");
        }

        nofication.IsDeleted = true; 

        
         var auditLog = new AuditLog{
            EntityType = "Nofications",
            EntityId = nofication.Id,
            UserId = user.Id,
            Action = "Deleted",
            OldValue = $"Message: {nofication.Message}, isRead: {nofication.IsRead}",
            NewValue = "N/A",
            Timestamp = DateTime.UtcNow
        };

         _context.AuditLogs.Add(auditLog);
         await _context.SaveChangesAsync();

         return ServiceResult<bool>.NoContent("Deleted the notification");
    }

    public async Task<ServiceResult<bool>> DeleteAll(ClaimsPrincipal claimsPrincipal)
    {
   var user = await _userManager.GetUserAsync(claimsPrincipal);
        if (user is null)
        {
            return ServiceResult<bool>.Fail("User not found");
        }

        var allNofications = await _context.Notifications.Where(n => n.UserId == user.Id).ToListAsync();
        if (allNofications is null)
        {
            return ServiceResult<bool>.Fail("No Nofications to delete");
        }

        foreach(var nofiation in allNofications)
        {
            nofiation.IsDeleted = true;
            
        } 
        
         var auditLog = new AuditLog{
            EntityType = "Nofications",
            EntityId = 0,
            UserId = user.Id,
            Action = "Deleted",
            OldValue = $"Deleted {allNofications.Count} notifications",
            NewValue = "N/A",
            Timestamp = DateTime.UtcNow
        };

         _context.AuditLogs.Add(auditLog);
         await _context.SaveChangesAsync();

         return ServiceResult<bool>.NoContent("Deleted all notifications");
    }

     public async Task<ServiceResult<bool>> Mark(int noficationId, MarkAsReadRequest request, ClaimsPrincipal claimsPrincipal)
    {
        var user = await _userManager.GetUserAsync(claimsPrincipal);
        if (user is null)
        {
            return ServiceResult<bool>.Fail("User not found");
        }

        var nofication = await _context.Notifications.FirstOrDefaultAsync(n => n.Id == noficationId && n.IsDeleted == false);
        if (nofication is null)
        {
            return ServiceResult<bool>.Fail("Nofication not found");
        }

        nofication.IsRead = request.Mark; 
        
         var auditLog = new AuditLog{
            EntityType = "Nofications",
            EntityId = nofication.Id,
            UserId = user.Id,
            Action = request.Mark ? "MarkAsRead" : "MarkAsUnread",
            OldValue = $"IsRead: {!request.Mark}",
            NewValue = $"isRead: {request.Mark}",
            Timestamp = DateTime.UtcNow
        };

         _context.AuditLogs.Add(auditLog);
         await _context.SaveChangesAsync();

         return ServiceResult<bool>.NoContent("Marked the Notification");

    }

     public async Task<ServiceResult<bool>> MarkAll(ClaimsPrincipal claimsPrincipal)
    {
    var user = await _userManager.GetUserAsync(claimsPrincipal);
        if (user is null)
        {
            return ServiceResult<bool>.Fail("User not found");
        }

        var allNofications = await _context.Notifications.Where(n => n.UserId == user.Id).ToListAsync();
        if (allNofications is null)
        {
            return ServiceResult<bool>.Fail("No Nofications to read");
        }

        foreach(var nofiation in allNofications)
        {
            nofiation.IsRead = true;
            
        } 
        
         var auditLog = new AuditLog{
            EntityType = "Nofications",
            EntityId = 0,
            UserId = user.Id,
            Action = "Readed",
            OldValue = "All marked as read",
            NewValue = "N/A",
            Timestamp = DateTime.UtcNow
        };

         _context.AuditLogs.Add(auditLog);
         await _context.SaveChangesAsync();

         return ServiceResult<bool>.NoContent("Marked all notifications");

    }

     public async Task<ServiceResult<bool>> MarkAllUnread(ClaimsPrincipal claimsPrincipal)
    {
    var user = await _userManager.GetUserAsync(claimsPrincipal);
        if (user is null)
        {
            return ServiceResult<bool>.Fail("User not found");
        }

        var allNofications = await _context.Notifications.Where(n => n.UserId == user.Id).ToListAsync();
        if (allNofications is null)
        {
            return ServiceResult<bool>.Fail("No Nofications to read");
        }

        foreach(var nofiation in allNofications)
        {
            nofiation.IsRead = false;
            
        } 
        
         var auditLog = new AuditLog{
            EntityType = "Nofications",
            EntityId = 0,
            UserId = user.Id,
            Action = "Unreaded",
            OldValue = "All marked as Unread",
            NewValue = "N/A",
            Timestamp = DateTime.UtcNow
        };

         _context.AuditLogs.Add(auditLog);
         await _context.SaveChangesAsync();

         return ServiceResult<bool>.NoContent("Marked all as unread");
    }
}