using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text.RegularExpressions; // Added for Regex
using KitBoxDesigner.Models;

namespace KitBoxDesigner.Services
{
    /// <summary>
    /// Implementation of part service for managing parts and inventory
    /// </summary>
    public class PartService : IPartService
    {
        private readonly KitboxApiService _kitboxApiService;
        private List<Part> _parts; // This will be populated on first call or refreshed
        private readonly Dictionary<string, int> _reservations;

        public PartService(KitboxApiService kitboxApiService)
        {
            _kitboxApiService = kitboxApiService;
            _parts = new List<Part>();
            _reservations = new Dictionary<string, int>();
            // Initialize _parts by calling GetAllPartsAsync or a dedicated load method if needed at startup
            // For now, it will be loaded on the first call to a method that uses _parts.
        }

        // Helper method to parse dimensions string like "Hxx Wyy Dzz" or "Hxx Wyy" etc.
        private (int? Height, int? Width, int? Depth) ParseDimensions(string? dimensionsString)
        {
            if (string.IsNullOrWhiteSpace(dimensionsString))
            {
                return (null, null, null);
            }

            int? height = null;
            int? width = null;
            int? depth = null;

            var hMatch = Regex.Match(dimensionsString, @"H(\d+)");
            if (hMatch.Success) height = int.Parse(hMatch.Groups[1].Value);

            var wMatch = Regex.Match(dimensionsString, @"W(\d+)");
            if (wMatch.Success) width = int.Parse(wMatch.Groups[1].Value);

            var dMatch = Regex.Match(dimensionsString, @"D(\d+)");
            if (dMatch.Success) depth = int.Parse(dMatch.Groups[1].Value);

            return (height, width, depth);
        }

        // Helper method to parse delay string e.g., "3 days" into an integer
        private int ParseDelay(string? delayString)
        {
            if (string.IsNullOrWhiteSpace(delayString))
            {
                return int.MaxValue; // Assume longest delay if not specified
            }
            var match = Regex.Match(delayString, @"(\d+)");
            if (match.Success && int.TryParse(match.Groups[1].Value, out int days))
            {
                return days;
            }
            return int.MaxValue; // Or throw an exception for unparseable format
        }


        private Part MapStockItemToPart(StockItem s)
        {
            var (height, width, depth) = ParseDimensions(s.Part.Dimensions); // Already correct

            decimal price = 0;
            // Supplier selection logic
            bool supplier1Exists = s.Part.PriceSupplier1.HasValue;
            bool supplier2Exists = s.Part.PriceSupplier2.HasValue;

            if (supplier1Exists && supplier2Exists)
            {
                if (s.Part.PriceSupplier1!.Value < s.Part.PriceSupplier2!.Value)
                {
                    price = s.Part.PriceSupplier1.Value;
                }
                else if (s.Part.PriceSupplier2!.Value < s.Part.PriceSupplier1!.Value)
                {
                    price = s.Part.PriceSupplier2.Value;
                }
                else // Prices are identical, compare delays
                {
                    int delay1 = ParseDelay(s.Part.DelaySupplier1);
                    int delay2 = ParseDelay(s.Part.DelaySupplier2);
                    price = delay1 <= delay2 ? s.Part.PriceSupplier1.Value : s.Part.PriceSupplier2.Value;
                }
            }
            else if (supplier1Exists)
            {
                price = s.Part.PriceSupplier1!.Value;
            }
            else if (supplier2Exists)
            {
                price = s.Part.PriceSupplier2!.Value;
            }
            // else price remains 0 if no supplier info

            return new Part
            {
                Code = s.Part.Code, 
                Reference = s.Part.Reference, 
                Dimensions = s.Part.Dimensions, 
                Height = height,         // Assign parsed height
                Width = width,           // Assign parsed width
                Depth = depth,           // Assign parsed depth
                Price = price,
                StockQuantity = s.Part.StockQuantity, 
                MinimumStock = s.Part.MinimumStock, // Assuming MinimumStock is on Part
                Category = MapReferenceToCategory(s.Part.Reference), 
                Color = ExtractColorFromReference(s.Part.Reference, s.Part.Code),
                PriceSupplier1 = s.Part.PriceSupplier1, // Keep these for direct access if needed
                DelaySupplier1 = s.Part.DelaySupplier1,
                PriceSupplier2 = s.Part.PriceSupplier2,
                DelaySupplier2 = s.Part.DelaySupplier2
                // TypeAttribute might need specific logic if it's part of reference/code or another field
            };
        }

        /// <inheritdoc />
        public async Task<List<Part>> GetAllPartsAsync()
        {
            // Corrected to use the concrete type KitboxApiService and its method
            var stockItems = await _kitboxApiService.GetAllStockItemsAsync(); // Assuming this is the intended method
            _parts = stockItems.Select(MapStockItemToPart).ToList();
            return _parts;
        }

        /// <inheritdoc />
        public async Task<Part?> GetPartByCodeAsync(string code)
        {
            // Corrected to use the concrete type KitboxApiService and its method
            var stockItem = await _kitboxApiService.GetStockItemByCodeAsync(code); // Assuming this is the intended method
            if (stockItem == null)
            {
                return null;
            }
            return MapStockItemToPart(stockItem);
        }

        // Helper method to map reference to PartCategory
        private PartCategory MapReferenceToCategory(string reference)
        {
            if (string.IsNullOrWhiteSpace(reference)) return PartCategory.Unknown;

            // Adjusted to match PartCategory enum values more closely
            if (reference.StartsWith("TAS", StringComparison.OrdinalIgnoreCase)) return PartCategory.VerticalBatten;
            if (reference.StartsWith("COR", StringComparison.OrdinalIgnoreCase)) return PartCategory.AngleIron;
            
            if (reference.StartsWith("TR", StringComparison.OrdinalIgnoreCase)) 
            {
                if (reference.Contains("AV", StringComparison.OrdinalIgnoreCase) || reference.Contains("F", StringComparison.OrdinalIgnoreCase)) return PartCategory.CrossbarFront; 
                if (reference.Contains("AR", StringComparison.OrdinalIgnoreCase) || reference.Contains("B", StringComparison.OrdinalIgnoreCase)) return PartCategory.CrossbarBack;  
                if (reference.Contains("CO", StringComparison.OrdinalIgnoreCase) || reference.Contains("L", StringComparison.OrdinalIgnoreCase)) return PartCategory.CrossbarLeft;  
                if (reference.Contains("CO", StringComparison.OrdinalIgnoreCase) || reference.Contains("R", StringComparison.OrdinalIgnoreCase)) return PartCategory.CrossbarRight;
                // Fallback for generic crossbar if specific type not identified
                return PartCategory.CrossbarFront; // Or another default/general crossbar category
            }

            if (reference.StartsWith("PAN", StringComparison.OrdinalIgnoreCase)) 
            {
                if (reference.Contains("HB", StringComparison.OrdinalIgnoreCase) || reference.Contains("H", StringComparison.OrdinalIgnoreCase)) return PartCategory.PanelHorizontal; 
                if (reference.Contains("CO", StringComparison.OrdinalIgnoreCase) || reference.Contains("V", StringComparison.OrdinalIgnoreCase)) return PartCategory.PanelVertical;       
                if (reference.Contains("AR", StringComparison.OrdinalIgnoreCase)) return PartCategory.PanelBack;       
                return PartCategory.PanelHorizontal; // Or another default/general panel category
            }
            
            if (reference.StartsWith("POR", StringComparison.OrdinalIgnoreCase)) return PartCategory.Door;      
            if (reference.StartsWith("COU", StringComparison.OrdinalIgnoreCase)) return PartCategory.Coupelles; // Assuming Coupelles is correct
            
            return PartCategory.Unknown;
        }

        // Helper method to extract color from reference or code
        private string? ExtractColorFromReference(string reference, string code)
        {
            if (string.IsNullOrWhiteSpace(reference) && string.IsNullOrWhiteSpace(code)) return null;
            
            string combined = (reference ?? "") + (code ?? "");

            if (combined.Contains("BL")) return "White"; 
            if (combined.Contains("BR")) return "Brown"; 
            if (combined.Contains("NR")) return "Black"; 
            if (combined.Contains("WH")) return "White";
            if (combined.Contains("GL")) return "Glass"; // Example for glass doors
            
            return null; 
        }

        /// <inheritdoc />
        public async Task<List<Part>> GetPartsByCategoryAsync(PartCategory category)
        {
            if (!_parts.Any()) await GetAllPartsAsync(); // Ensure parts are loaded
            return _parts.Where(p => p.Category == category).ToList();
        }

        /// <inheritdoc />
        public async Task<List<Part>> SearchPartsAsync(string searchTerm)
        {
            if (!_parts.Any()) await GetAllPartsAsync(); // Ensure parts are loaded
            
            if (string.IsNullOrWhiteSpace(searchTerm))
                return _parts.ToList();

            var term = searchTerm.ToLowerInvariant();
            return _parts.Where(p => 
                (p.Code?.ToLowerInvariant().Contains(term) ?? false) ||
                (p.Reference?.ToLowerInvariant().Contains(term) ?? false) ||
                (p.Dimensions?.ToLowerInvariant().Contains(term) ?? false) ||
                (p.Color?.ToLowerInvariant().Contains(term) ?? false) ||
                (p.Category.ToString().ToLowerInvariant().Contains(term))
            ).ToList();
        }

        /// <inheritdoc />
        public async Task<List<Part>> GetLowStockPartsAsync()
        {
            if (!_parts.Any()) await GetAllPartsAsync(); // Ensure parts are loaded
            return _parts.Where(p => p.IsLowStock).ToList();
        }

        /// <inheritdoc />
        public async Task<List<Part>> GetOutOfStockPartsAsync()
        {
            if (!_parts.Any()) await GetAllPartsAsync(); // Ensure parts are loaded
            return _parts.Where(p => p.IsOutOfStock).ToList();
        }

        /// <inheritdoc />
        public async Task<List<int>> GetAvailableDimensionsAsync(PartCategory category)
        {
            await Task.Delay(10);
            return new List<int>(); // Return empty list
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

        /// <inheritdoc />
        public async Task<Part?> FindPartBySpecificationAsync(PartSpecification specification)
        {
            if (!_parts.Any()) await GetAllPartsAsync(); // Ensure parts are loaded

            return _parts.FirstOrDefault(p =>
                p.Category == specification.Category &&
                (specification.Height == null || p.Height == specification.Height) &&
                (specification.Width == null || p.Width == specification.Width) &&
                (specification.Depth == null || p.Depth == specification.Depth) &&
                (string.IsNullOrEmpty(specification.Color) || (p.Color != null && p.Color.Equals(specification.Color, StringComparison.OrdinalIgnoreCase))) &&
                (string.IsNullOrEmpty(specification.TypeAttribute) || 
                    (p.Reference != null && p.Reference.Contains(specification.TypeAttribute, StringComparison.OrdinalIgnoreCase)) || // Check reference
                    (p.Code != null && p.Code.Contains(specification.TypeAttribute, StringComparison.OrdinalIgnoreCase))) // Or check code for type
            );
        }
    }
}
