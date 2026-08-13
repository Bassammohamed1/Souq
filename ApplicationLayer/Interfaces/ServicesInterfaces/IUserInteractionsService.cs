using ApplicationLayer.Helpers;
using DomainLayer.Models;

namespace ApplicationLayer.Interfaces.ServicesInterfaces
{
    public interface IUserInteractionsService
    {
        Task<Result> AddComment(Comment comment);
        Task<Result> AddRate(Rate rate);
    }
}