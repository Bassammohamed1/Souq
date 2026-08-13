
namespace ApplicationLayer.Interfaces.ServicesInterfaces
{
    public interface IServicesInstanceProvider
    {
        public IItemsService GetItemsServiceInstance();
        public IWishingListService GetWishingListServiceInstance();
        public ICartService GetCartServiceInstance();
        public IOffersService GetOffersServiceInstance();
        public IOrdersService GetOrdersServiceInstance();
        public ICategoriesService GetCategoriesServiceInstance();
        public IDepartmentsService GetDepartmentsServiceInstance();
    }
}
