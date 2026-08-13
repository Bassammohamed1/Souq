using DomainLayer.Interfaces;
using DomainLayer.Models;
using InfrastructureLayer.Data;
using Microsoft.EntityFrameworkCore;

namespace InfrastructureLayer.Repository
{
    public class CommentsRepository : Repository<Comment>, ICommentsRepository
    {
        private readonly AppDbContext _context;

        public CommentsRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Comment>> GetAllComments()
        {
            return await _context.Comments.AsNoTracking()
                .Include(c => c.User).AsSingleQuery().ToListAsync();
        }
    }
}
