using ApplicationLayer.Helpers;
using ApplicationLayer.Interfaces.ServicesInterfaces;
using DomainLayer.Interfaces;
using DomainLayer.Models;

namespace ApplicationLayer.Services
{
    public class UserInteractionsService : IUserInteractionsService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IServicesInstanceProvider _servicesInstanceProvider;

        public UserInteractionsService(IUnitOfWork unitOfWork, IServicesInstanceProvider servicesInstanceProvider)
        {
            _unitOfWork = unitOfWork;
            _servicesInstanceProvider = servicesInstanceProvider;
        }

        public async Task<Result> AddComment(Comment comment)
        {
            var result = await _unitOfWork.Comments.Add(comment);

            await _unitOfWork.Commit();

            return result is not null ? new Result() { Success = true } :
                new Result() { Success = false, Error = "An Error occured while making comment." };
        }

        public async Task<Result> AddRate(Rate rate)
        {
            var rates = (await _unitOfWork.Rates.GetAll())
                .Where(r => r.UserId == rate.UserId && r.ItemId == rate.ItemId && r.ItemType == rate.ItemType);

            if (rates.Any())
            {
                _unitOfWork.Rates.Delete(rates.First());
                await _unitOfWork.Commit();
            }

            await _unitOfWork.Rates.Add(rate);
            await _unitOfWork.Commit();

            var result = await _servicesInstanceProvider.GetItemsServiceInstance().SetRate(rate);

            return result ? new Result() { Success = true } :
                new Result() { Success = false, Error = "An error occured while setting rate." };
        }
    }
}
