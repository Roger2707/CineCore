namespace Shared.Services.IServices
{
    public interface ICurrentUserService
    {
        string UserId { get; }
        string UserName { get; }
        string Email { get; }
        string Role { get; }
        Guid? CinemaId { get; }
        bool IsSuperAdmin { get; }
        bool IsAdmin { get; }
        bool IsCustomer { get; }
        bool IsAuthenticated { get; }
        T GetClaimValue<T>(string claimType);
    }
}
