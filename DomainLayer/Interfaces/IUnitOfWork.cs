using DomainLayer.Models;

namespace DomainLayer.Interfaces
{
    public interface IUnitOfWork
    {
        public IDepartmentsRepository Departments { get; }
        public ICategoriesRepository Categories { get; }
        public IRepository<CategoryDepartments> CategoryDepartments { get; }
        public IItemsRepository Items { get; }
        public IRepository<AirConditioner> AirConditioners { get; }
        public IRepository<Cooker> Cookers { get; }
        public IRepository<Fridge> Fridges { get; }
        public IRepository<HeadPhone> HeadPhones { get; }
        public IRepository<Laptop> Laptops { get; }
        public IRepository<TV> TVs { get; }
        public IRepository<VideoGame> VideoGames { get; }
        public IRepository<WashingMachine> WashingMachines { get; }
        public IRepository<Comment> Comments { get; }
        public IRepository<Rate> Rates { get; }
        public IMobilePhonesRepository MobilePhones { get; }
        public IWishListRepository WishLists { get; }
        public ICartRepository Carts { get; }
        public IOrdersRepository Orders { get; }
        public IOffersRepository Offers { get; }
        public IChatsRepository Chats { get; }
        Task Commit();
    }
}
