using ePizzaHub.Core.Database.Entities;
using ePizzaHub.Repositories.Interfaces;
using ePizzaHub.Services.Interfaces;

namespace ePizzaHub.Services.Implementations
{
    public class ItemService: Service<Item>, IItemService
    {
        IRepository<Item> _itemRepo;
        public ItemService(IRepository<Item> itemRepo): base(itemRepo)
        {
            
        }
    }
}
