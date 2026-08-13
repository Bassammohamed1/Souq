using DomainLayer.Models;

namespace DomainLayer.Interfaces
{
    public interface ICommentsRepository : IRepository<Comment>
    {
        Task<IEnumerable<Comment>> GetAllComments();
    }
}