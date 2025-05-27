using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KitBoxDesigner.Data;
using KitBoxDesigner.Models;

namespace KitBoxDesigner.Services
{
    /// <summary>
    /// Implementation of part service for managing parts and inventory
    /// </summary>
    public class PartService : IPartService
    {
        private readonly List<Part> _parts;
        private readonly Dictionary<string, int> _reservations;

        public PartService()
        {
            _parts = PartsData.GetAllParts();
            _reservations = new Dictionary<string, int>();
        }

        /// <inheritdoc />
        public async Task<List<Part>> GetAllPartsAsync()
        {
            // Simulate async operation
            await Task.Delay(50);
            return _parts.ToList();
        }

        /// <inheritdoc />
        public async Task<Part?> GetPartByCodeAsync(string code)
        {
            await Task.Delay(10);
            return _parts.FirstOrDefault(p => p.Code.Equals(code, StringComparison.OrdinalIgnoreCase));
        }

        /// <inheritdoc />
        public async Task<List<Part>> GetPartsByCategoryAsync(PartCategory category)
        {
            await Task.Delay(30);
            return _parts.Where(p => p.Category == category).ToList();
        }

        /// <inheritdoc />
        public async Task<List<Part>> SearchPartsAsync(string searchTerm)
        {
            await Task.Delay(40);
            
            if (string.IsNullOrWhiteSpace(searchTerm))
                return _parts.ToList();

            var term = searchTerm.ToLowerInvariant();
            return _parts.Where(p => 
                p.Code.ToLowerInvariant().Contains(term) ||
                p.Reference.ToLowerInvariant().Contains(term) ||
                p.Dimensions.ToLowerInvariant().Contains(term)
            ).ToList();
        }

        /// <inheritdoc />
        public async Task<List<Part>> GetLowStockPartsAsync()
        {
            await Task.Delay(20);
            return _parts.Where(p => p.IsLowStock).ToList();
        }

        /// <inheritdoc />
        public async Task<List<Part>> GetOutOfStockPartsAsync()
        {
            await Task.Delay(20);
            return _parts.Where(p => p.IsOutOfStock).ToList();
        }

        /// <inheritdoc />
        public async Task<List<int>> GetAvailableDimensionsAsync(PartCategory category)
        {
            await Task.Delay(10);
            return PartsData.GetAvailableDimensions(category);
        }

        /// <inheritdoc />
        public async Task<Dictionary<string, bool>> CheckPartsAvailabilityAsync(List<PartRequirement> requirements)
        {
            await Task.Delay(100);
            
            var availability = new Dictionary<string, bool>();
            
            foreach (var requirement in requirements)
            {
                var part = await GetPartByCodeAsync(requirement.PartCode);
                if (part == null)
                {
                    availability[requirement.PartCode] = false;
                    continue;
                }

                var reservedQuantity = _reservations.GetValueOrDefault(requirement.PartCode, 0);
                var availableQuantity = part.StockQuantity - reservedQuantity;
                
                availability[requirement.PartCode] = availableQuantity >= requirement.Quantity;
            }

            return availability;
        }

        /// <inheritdoc />
        public async Task<List<PartRequirement>> GetRequiredPartsAsync(CabinetConfiguration configuration)
        {
            await Task.Delay(150);
            return configuration.GetRequiredParts();
        }

        /// <inheritdoc />
        public async Task<bool> UpdatePartStockAsync(string partCode, int newQuantity)
        {
            await Task.Delay(50);
            
            var part = _parts.FirstOrDefault(p => p.Code.Equals(partCode, StringComparison.OrdinalIgnoreCase));
            if (part == null)
                return false;

            part.StockQuantity = Math.Max(0, newQuantity);
            return true;
        }

        /// <inheritdoc />
        public async Task<bool> ReservePartsAsync(List<PartRequirement> requirements)
        {
            await Task.Delay(100);
            
            // First check if all parts are available
            var availability = await CheckPartsAvailabilityAsync(requirements);
            if (availability.Values.Any(available => !available))
                return false;

            // Reserve the parts
            foreach (var requirement in requirements)
            {
                if (_reservations.ContainsKey(requirement.PartCode))
                    _reservations[requirement.PartCode] += requirement.Quantity;
                else
                    _reservations[requirement.PartCode] = requirement.Quantity;
            }

            return true;
        }

        /// <inheritdoc />
        public async Task<bool> ReleasePartsAsync(List<PartRequirement> requirements)
        {
            await Task.Delay(50);
            
            foreach (var requirement in requirements)
            {
                if (_reservations.ContainsKey(requirement.PartCode))
                {
                    _reservations[requirement.PartCode] = Math.Max(0, 
                        _reservations[requirement.PartCode] - requirement.Quantity);
                    
                    if (_reservations[requirement.PartCode] == 0)
                        _reservations.Remove(requirement.PartCode);
                }
            }

            return true;
        }

        /// <summary>
        /// Get currently reserved quantity for a part
        /// </summary>
        public int GetReservedQuantity(string partCode)
        {
            return _reservations.GetValueOrDefault(partCode, 0);
        }

        /// <summary>
        /// Get available quantity (stock - reserved) for a part
        /// </summary>
        public async Task<int> GetAvailableQuantityAsync(string partCode)
        {
            var part = await GetPartByCodeAsync(partCode);
            if (part == null)
                return 0;

            var reserved = GetReservedQuantity(partCode);
            return Math.Max(0, part.StockQuantity - reserved);
        }

        /// <summary>
        /// Simulate restocking a part
        /// </summary>
        public async Task<bool> RestockPartAsync(string partCode, int quantity)
        {
            var part = await GetPartByCodeAsync(partCode);
            if (part == null)
                return false;

            part.StockQuantity += quantity;
            return true;
        }

        /// <summary>
        /// Get parts that need reordering based on minimum stock levels
        /// </summary>
        public async Task<List<Part>> GetPartsNeedingReorderAsync()
        {
            await Task.Delay(30);
            return _parts.Where(p => p.StockQuantity <= p.MinimumStock).ToList();
        }
    }
}
