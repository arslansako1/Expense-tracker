

using System.Security.Claims;

public interface INotificationsService
{
     Task<ServiceResult<List<Notification>>> GetAll(ClaimsPrincipal claimsPrincipal);
     Task<ServiceResult<bool>> Delete(int noficationId, ClaimsPrincipal claimsPrincipal);
     Task<ServiceResult<bool>> DeleteAll(ClaimsPrincipal claimsPrincipal);
     Task<ServiceResult<bool>> Mark(int nofiationId, MarkAsReadRequest request, ClaimsPrincipal claimsPrincipal);
     Task<ServiceResult<bool>> MarkAll(ClaimsPrincipal claimsPrincipal);
     Task<ServiceResult<bool>> MarkAllUnread(ClaimsPrincipal claimsPrincipal);
}