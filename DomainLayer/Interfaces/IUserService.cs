namespace DomainLayer.Interfaces
{
    public interface IUserService
    {
        Task<string> GetUserId();
    }
}
