using DomainLayer.Models;
using DomainLayer.Models.Chat;

namespace DomainLayer.Interfaces
{
    public interface IUnitOfWork
    {
        public ICategoriesRepository Categories { get; }
        public ICategoryDepartmentsRepository CategoryDepartments { get; }
        public IRepository<Department> Departments { get; }
        public IItemsRepository Items { get; }
        public IRepository<AirConditioner> AirConditioners { get; }
        public IRepository<Cooker> Cookers { get; }
        public IRepository<Fridge> Fridges { get; }
        public IRepository<HeadPhone> HeadPhones { get; }
        public IRepository<Laptop> Laptops { get; }
        public IRepository<TV> TVs { get; }
        public IRepository<VideoGame> VideoGames { get; }
        public IRepository<MobilePhone> MobilePhones { get; }
        public IRepository<WashingMachine> WashingMachines { get; }
        public ICommentsRepository Comments { get; }
        public IRepository<Rate> Rates { get; }
        public IWishListRepository WishLists { get; }
        public ICartRepository Carts { get; }
        public IOrdersRepository Orders { get; }
        public IOffersRepository Offers { get; }
        public IRepository<ChatMessage> Chats { get; }
        Task Commit();
    }
}
