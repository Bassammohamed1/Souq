using DomainLayer.Interfaces;
using DomainLayer.Models;
using DomainLayer.Models.Chat;
using InfrastructureLayer.Data;

namespace InfrastructureLayer.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _context;

        public UnitOfWork(AppDbContext context)
        {
            _context = context;
            Categories = new CategoriesRepository(_context);
            CategoryDepartments = new CategoryDepartmentsRepository(_context);
            Departments = new Repository<Department>(_context);
            Items = new ItemsRepository(_context);
            AirConditioners = new Repository<AirConditioner>(_context);
            Cookers = new Repository<Cooker>(_context);
            Fridges = new Repository<Fridge>(_context);
            HeadPhones = new Repository<HeadPhone>(_context);
            Laptops = new Repository<Laptop>(_context);
            TVs = new Repository<TV>(_context);
            VideoGames = new Repository<VideoGame>(_context);
            MobilePhones = new Repository<MobilePhone>(_context);
            WashingMachines = new Repository<WashingMachine>(_context);
            Comments = new CommentsRepository(_context);
            Rates = new Repository<Rate>(_context);
            WishLists = new WishListRepository(_context);
            Orders = new OrdersRepository(_context);
            Offers = new OffersRepository(_context);
            Carts = new CartRepository(_context);
            Chats = new Repository<ChatMessage>(_context);
        }

        public ICategoriesRepository Categories { get; private set; }
        public ICategoryDepartmentsRepository CategoryDepartments { get; private set; }
        public IRepository<Department> Departments { get; private set; }
        public IItemsRepository Items { get; private set; }
        public IRepository<AirConditioner> AirConditioners { get; private set; }
        public IRepository<Cooker> Cookers { get; private set; }
        public IRepository<Fridge> Fridges { get; private set; }
        public IRepository<HeadPhone> HeadPhones { get; private set; }
        public IRepository<Laptop> Laptops { get; private set; }
        public IRepository<TV> TVs { get; private set; }
        public IRepository<VideoGame> VideoGames { get; private set; }
        public IRepository<MobilePhone> MobilePhones { get; private set; }
        public IRepository<WashingMachine> WashingMachines { get; private set; }
        public ICommentsRepository Comments { get; private set; }
        public IRepository<Rate> Rates { get; private set; }
        public IWishListRepository WishLists { get; private set; }
        public ICartRepository Carts { get; private set; }
        public IOrdersRepository Orders { get; private set; }
        public IOffersRepository Offers { get; private set; }
        public IRepository<ChatMessage> Chats { get; private set; }

        public async Task Commit()
        {
            await _context.SaveChangesAsync();
        }

        public async Task Dispose()
        {
            await _context.DisposeAsync();
        }
    }
}
