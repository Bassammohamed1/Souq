using ApplicationLayer.Interfaces.ServicesInterfaces;
using Microsoft.Extensions.DependencyInjection;

namespace ApplicationLayer.Services
{
    public class ServicesInstanceProvider : IServicesInstanceProvider
    {
        private readonly IServiceProvider _serviceProvider;

        public ServicesInstanceProvider(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public IItemsService GetItemsServiceInstance()
        {
            return _serviceProvider.GetRequiredService<IItemsService>();
        }

        public IWishingListService GetWishingListServiceInstance()
        {
            return _serviceProvider.GetRequiredService<IWishingListService>();
        }

        public ICartService GetCartServiceInstance()
        {
            return _serviceProvider.GetRequiredService<ICartService>();
        }

        public IOffersService GetOffersServiceInstance()
        {
            return _serviceProvider.GetRequiredService<IOffersService>();
        }

        public IOrdersService GetOrdersServiceInstance()
        {
            return _serviceProvider.GetRequiredService<IOrdersService>();
        }

        public ICategoriesService GetCategoriesServiceInstance()
        {
            return _serviceProvider.GetRequiredService<ICategoriesService>();
        }

        public IDepartmentsService GetDepartmentsServiceInstance()
        {
            return _serviceProvider.GetRequiredService<IDepartmentsService>();
        }
    }
}