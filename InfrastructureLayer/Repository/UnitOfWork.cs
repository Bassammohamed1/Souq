using DomainLayer.Interfaces;
using DomainLayer.Models;
using InfrastructureLayer.Data;

namespace InfrastructureLayer.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _context;
        private readonly IUserService _userService;

        public UnitOfWork(AppDbContext context, IUserService userService)
        {
            _context = context;
            _userService = userService;
            Departments = new DepartmentsRepository(_context);
            Categories = new CategoriesRepository(_context);
            CategoryDepartments = new Repository<CategoryDepartments>(_context);
            Items = new ItemsRepository(_context);
            AirConditioners = new Repository<AirConditioner>(_context);
            Cookers = new Repository<Cooker>(_context);
            Fridges = new Repository<Fridge>(_context);
            HeadPhones = new Repository<HeadPhone>(_context);
            Laptops = new Repository<Laptop>(_context);
            TVs = new Repository<TV>(_context);
            VideoGames = new Repository<VideoGame>(_context);
            WashingMachines = new Repository<WashingMachine>(_context);
            Comments = new Repository<Comment>(_context);
            Rates = new Repository<Rate>(_context);
            MobilePhones = new MobilePhonesRepository(_context);
            WishLists = new WishListRepository(_context, _userService);
            Orders = new OrdersRepository(_context, userService);
            Offers = new OffersRepository(_context, Departments, Categories, Items);
            Carts = new CartRepository(_context, userService, Orders, Offers, Items);
            Chats = new ChatsRepository(_context);
        }

        public IDepartmentsRepository Departments { get; private set; }
        public ICategoriesRepository Categories { get; private set; }
        public IRepository<CategoryDepartments> CategoryDepartments { get; private set; }
        public IItemsRepository Items { get; private set; }
        public IRepository<AirConditioner> AirConditioners { get; private set; }
        public IRepository<Cooker> Cookers { get; private set; }
        public IRepository<Fridge> Fridges { get; private set; }
        public IRepository<HeadPhone> HeadPhones { get; private set; }
        public IRepository<Laptop> Laptops { get; private set; }
        public IRepository<TV> TVs { get; private set; }
        public IRepository<VideoGame> VideoGames { get; private set; }
        public IRepository<WashingMachine> WashingMachines { get; private set; }
        public IRepository<Comment> Comments { get; private set; }
        public IRepository<Rate> Rates { get; private set; }
        public IMobilePhonesRepository MobilePhones { get; private set; }
        public IWishListRepository WishLists { get; private set; }
        public ICartRepository Carts { get; private set; }
        public IOrdersRepository Orders { get; private set; }
        public IOffersRepository Offers { get; private set; }
        public IChatsRepository Chats { get; private set; }

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
