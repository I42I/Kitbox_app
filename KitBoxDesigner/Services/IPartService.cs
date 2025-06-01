using System.Collections.Generic;
using System.Threading.Tasks;
using KitBoxDesigner.Models;

namespace KitBoxDesigner.Services
{
    /// <summary>
    /// Service interface for managing parts and inventory
    /// </summary>
    public interface IPartService
    {
        /// <summary>
        /// Get all available parts
        /// </summary>
        Task<List<Part>> GetAllPartsAsync();

        /// <summary>
        /// Get part by unique code
        /// </summary>
        Task<Part?> GetPartByCodeAsync(string code);

        /// <summary>
        /// Get parts filtered by category
        /// </summary>
        Task<List<Part>> GetPartsByCategoryAsync(PartCategory category);

        /// <summary>
        /// Search parts by reference or code
        /// </summary>
        Task<List<Part>> SearchPartsAsync(string searchTerm);

        /// <summary>
        /// Get parts with low stock levels
        /// </summary>
        Task<List<Part>> GetLowStockPartsAsync();

        /// <summary>
        /// Get parts that are out of stock
        /// </summary>
        Task<List<Part>> GetOutOfStockPartsAsync();

        /// <summary>
        /// Get available dimensions for a specific part category
        /// </summary>
        Task<List<int>> GetAvailableDimensionsAsync(PartCategory category);

        /// <summary>
        /// Check if specific parts are available in required quantities
        /// </summary>
        Task<Dictionary<string, bool>> CheckPartsAvailabilityAsync(List<PartRequirement> requirements);

        /// <summary>
        /// Get parts required for a cabinet configuration
        /// </summary>
        Task<List<PartRequirement>> GetRequiredPartsAsync(CabinetConfiguration configuration);

        /// <summary>
        /// Update part stock quantity
        /// </summary>
        Task<bool> UpdatePartStockAsync(string partCode, int newQuantity);

        /// <summary>
        /// Reserve parts for an order
        /// </summary>
        Task<bool> ReservePartsAsync(List<PartRequirement> requirements);

        /// <summary>
        /// Release reserved parts
        /// </summary>
        Task<bool> ReleasePartsAsync(List<PartRequirement> requirements);

        /// <summary>
        /// Finds a specific part from stock that matches the given specification.
        /// </summary>
        /// <param name="specification">The characteristics of the part to find.</param>
        /// <returns>The matching Part object, or null if not found.</returns>
        Task<Part?> FindPartBySpecificationAsync(PartSpecification specification);
    }
}
