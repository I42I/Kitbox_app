// New file: KitBoxDesigner/Services/IKitboxApiService.cs
using KitBoxDesigner.Models; // For Part, StockItem etc. if they are params/returns in the inherited interfaces
using System.Collections.Generic;
using System.Threading.Tasks;

namespace KitBoxDesigner.Services
{
    /// <summary>
    /// Combined service interface for Kitbox operations,
    /// aggregating part and stock management functionalities.
    /// </summary>
    public interface IKitboxApiService : IPartService, IStockService
    {
        // This interface can also include methods that are specific to KitboxApiService
        // and not covered by IPartService or IStockService, if any.
        // For example, authentication methods if they interact with the API:
        // Task<User?> LoginAsync(string username, string password);
        // Task<bool> RegisterUserAsync(UserRegistrationDetails details);
    }
}
