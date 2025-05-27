using System;
using System.Collections.Generic;
using System.Linq;
using KitBoxDesigner.Models;

namespace KitBoxDesigner.Data
{
    /// <summary>
    /// Static data repository for all parts used in KitBox cabinet construction
    /// Contains exact data from the TypeScript project specification
    /// </summary>
    public static class PartsData
    {
        private static readonly Random _random = new();

        /// <summary>
        /// Get all available parts with simulated stock levels
        /// </summary>
        public static List<Part> GetAllParts() => new List<Part>
        {
            // TASSEUX VERTICAUX (Vertical Battens)
            new Part 
            { 
                Reference = "Vertical batten", 
                Code = "TAS27", 
                Dimensions = "27(h32)", 
                Price = 0.17m, 
                DeliveryDelay = 10,
                Category = PartCategory.VerticalBatten,
                StockQuantity = SimulateStock(),
                MinimumStock = 20
            },
            new Part 
            { 
                Reference = "Vertical batten", 
                Code = "TAS37", 
                Dimensions = "37(h42)", 
                Price = 0.25m, 
                DeliveryDelay = 4,
                Category = PartCategory.VerticalBatten,
                StockQuantity = SimulateStock(),
                MinimumStock = 20
            },
            new Part 
            { 
                Reference = "Vertical batten", 
                Code = "TAS47", 
                Dimensions = "47(h52)", 
                Price = 0.34m, 
                DeliveryDelay = 10,
                Category = PartCategory.VerticalBatten,
                StockQuantity = SimulateStock(),
                MinimumStock = 20
            },

            // TRAVERSES LATÉRALES (Side Crossbars)
            new Part 
            { 
                Reference = "Crossbar left or right", 
                Code = "TRG32", 
                Dimensions = "32(p)", 
                Price = 0.90m, 
                DeliveryDelay = 9,
                Category = PartCategory.CrossbarLeft,
                StockQuantity = SimulateStock(),
                MinimumStock = 15
            },
            new Part 
            { 
                Reference = "Crossbar left or right", 
                Code = "TRG42", 
                Dimensions = "42(p)", 
                Price = 1.01m, 
                DeliveryDelay = 4,
                Category = PartCategory.CrossbarLeft,
                StockQuantity = SimulateStock(),
                MinimumStock = 15
            },
            new Part 
            { 
                Reference = "Crossbar left or right", 
                Code = "TRG52", 
                Dimensions = "52(p)", 
                Price = 1.21m, 
                DeliveryDelay = 12,
                Category = PartCategory.CrossbarLeft,
                StockQuantity = SimulateStock(),
                MinimumStock = 15
            },
            new Part 
            { 
                Reference = "Crossbar left or right", 
                Code = "TRG62", 
                Dimensions = "62(p)", 
                Price = 1.43m, 
                DeliveryDelay = 8,
                Category = PartCategory.CrossbarLeft,
                StockQuantity = SimulateStock(),
                MinimumStock = 15
            },

            // TRAVERSES ARRIÈRE (Back Crossbars)
            new Part 
            { 
                Reference = "Crossbar back", 
                Code = "TRR32", 
                Dimensions = "32(L)", 
                Price = 0.89m, 
                DeliveryDelay = 8,
                Category = PartCategory.CrossbarBack,
                StockQuantity = SimulateStock(),
                MinimumStock = 12
            },
            new Part 
            { 
                Reference = "Crossbar back", 
                Code = "TRR42", 
                Dimensions = "42(L)", 
                Price = 0.98m, 
                DeliveryDelay = 10,
                Category = PartCategory.CrossbarBack,
                StockQuantity = SimulateStock(),
                MinimumStock = 12
            },
            new Part 
            { 
                Reference = "Crossbar back", 
                Code = "TRR52", 
                Dimensions = "52(L)", 
                Price = 1.20m, 
                DeliveryDelay = 6,
                Category = PartCategory.CrossbarBack,
                StockQuantity = SimulateStock(),
                MinimumStock = 12
            },
            new Part 
            { 
                Reference = "Crossbar back", 
                Code = "TRR62", 
                Dimensions = "62(L)", 
                Price = 1.35m, 
                DeliveryDelay = 12,
                Category = PartCategory.CrossbarBack,
                StockQuantity = SimulateStock(),
                MinimumStock = 12
            },
            new Part 
            { 
                Reference = "Crossbar back", 
                Code = "TRR80", 
                Dimensions = "80(L)", 
                Price = 1.52m, 
                DeliveryDelay = 3,
                Category = PartCategory.CrossbarBack,
                StockQuantity = SimulateStock(),
                MinimumStock = 10
            },
            new Part 
            { 
                Reference = "Crossbar back", 
                Code = "TRR100", 
                Dimensions = "100(L)", 
                Price = 1.67m, 
                DeliveryDelay = 10,
                Category = PartCategory.CrossbarBack,
                StockQuantity = SimulateStock(),
                MinimumStock = 8
            },
            new Part 
            { 
                Reference = "Crossbar back", 
                Code = "TRR120", 
                Dimensions = "120(L)", 
                Price = 1.88m, 
                DeliveryDelay = 4,
                Category = PartCategory.CrossbarBack,
                StockQuantity = SimulateStock(),
                MinimumStock = 8
            },

            // TRAVERSES AVANT (Front Crossbars)
            new Part 
            { 
                Reference = "Crossbar front", 
                Code = "TRF32", 
                Dimensions = "32(L)", 
                Price = 1.32m, 
                DeliveryDelay = 8,
                Category = PartCategory.CrossbarFront,
                StockQuantity = SimulateStock(),
                MinimumStock = 12
            },
            new Part 
            { 
                Reference = "Crossbar front", 
                Code = "TRF42", 
                Dimensions = "42(L)", 
                Price = 1.49m, 
                DeliveryDelay = 8,
                Category = PartCategory.CrossbarFront,
                StockQuantity = SimulateStock(),
                MinimumStock = 12
            },
            new Part 
            { 
                Reference = "Crossbar front", 
                Code = "TRF52", 
                Dimensions = "52(L)", 
                Price = 1.53m, 
                DeliveryDelay = 6,
                Category = PartCategory.CrossbarFront,
                StockQuantity = SimulateStock(),
                MinimumStock = 12
            },
            new Part 
            { 
                Reference = "Crossbar front", 
                Code = "TRF62", 
                Dimensions = "62(L)", 
                Price = 1.63m, 
                DeliveryDelay = 8,
                Category = PartCategory.CrossbarFront,
                StockQuantity = SimulateStock(),
                MinimumStock = 12
            },
            new Part 
            { 
                Reference = "Crossbar front", 
                Code = "TRF80", 
                Dimensions = "80(L)", 
                Price = 1.61m, 
                DeliveryDelay = 4,
                Category = PartCategory.CrossbarFront,
                StockQuantity = SimulateStock(),
                MinimumStock = 10
            },
            new Part 
            { 
                Reference = "Crossbar front", 
                Code = "TRF100", 
                Dimensions = "100(L)", 
                Price = 1.93m, 
                DeliveryDelay = 5,
                Category = PartCategory.CrossbarFront,
                StockQuantity = SimulateStock(),
                MinimumStock = 8
            },
            new Part 
            { 
                Reference = "Crossbar front", 
                Code = "TRF120", 
                Dimensions = "120(L)", 
                Price = 1.98m, 
                DeliveryDelay = 5,
                Category = PartCategory.CrossbarFront,
                StockQuantity = SimulateStock(),
                MinimumStock = 8
            },

            // PANNEAUX ARRIÈRE BLANCS (White Back Panels) - Sample
            new Part 
            { 
                Reference = "Panel back White", 
                Code = "PAR3232BL", 
                Dimensions = "32(h)x32(L)", 
                Price = 4.24m, 
                DeliveryDelay = 10,
                Category = PartCategory.PanelBack,
                Color = "White",
                StockQuantity = SimulateStock(),
                MinimumStock = 10
            },
            new Part 
            { 
                Reference = "Panel back White", 
                Code = "PAR3242BL", 
                Dimensions = "32(h)x42(L)", 
                Price = 6.00m, 
                DeliveryDelay = 7,
                Category = PartCategory.PanelBack,
                Color = "White",
                StockQuantity = SimulateStock(),
                MinimumStock = 8
            },
            new Part 
            { 
                Reference = "Panel back White", 
                Code = "PAR3252BL", 
                Dimensions = "32(h)x52(L)", 
                Price = 7.36m, 
                DeliveryDelay = 7,
                Category = PartCategory.PanelBack,
                Color = "White",
                StockQuantity = SimulateStock(),
                MinimumStock = 8
            },
            new Part 
            { 
                Reference = "Panel back White", 
                Code = "PAR4242BL", 
                Dimensions = "42(h)x42(L)", 
                Price = 9.82m, 
                DeliveryDelay = 4,
                Category = PartCategory.PanelBack,
                Color = "White",
                StockQuantity = SimulateStock(),
                MinimumStock = 6
            },

            // PORTES (Doors)
            new Part 
            { 
                Reference = "Door glass", 
                Code = "POR5262VE", 
                Dimensions = "52(h)x62(L)", 
                Price = 27.45m, 
                DeliveryDelay = 8,
                Category = PartCategory.Door,
                Color = "Glass",
                StockQuantity = SimulateStock(5, 25),
                MinimumStock = 3
            },

            // CORNIÈRES (Angle Irons)
            new Part 
            { 
                Reference = "Angle iron White", 
                Code = "COR35BL", 
                Dimensions = "1x32(h)", 
                Price = 0.30m, 
                DeliveryDelay = 3,
                Category = PartCategory.AngleIron,
                Color = "White",
                StockQuantity = SimulateStock(),
                MinimumStock = 20
            },
            new Part 
            { 
                Reference = "Angle iron White", 
                Code = "COR66BL", 
                Dimensions = "2x32(h)", 
                Price = 0.54m, 
                DeliveryDelay = 12,
                Category = PartCategory.AngleIron,
                Color = "White",
                StockQuantity = SimulateStock(),
                MinimumStock = 15
            },
            new Part 
            { 
                Reference = "Angle iron black", 
                Code = "COR35NR", 
                Dimensions = "1x32(h)", 
                Price = 0.21m, 
                DeliveryDelay = 7,
                Category = PartCategory.AngleIron,
                Color = "Black",
                StockQuantity = SimulateStock(),
                MinimumStock = 20
            },
            new Part 
            { 
                Reference = "Angle iron (Galva)", 
                Code = "COR35GL", 
                Dimensions = "1x32(h)", 
                Price = 0.30m, 
                DeliveryDelay = 7,
                Category = PartCategory.AngleIron,
                Color = "Galvanized",
                StockQuantity = SimulateStock(),
                MinimumStock = 15
            },

            // PIEDS (Coupelles/Feet)
            new Part 
            { 
                Reference = "Coupelles", 
                Code = "COUPEL", 
                Dimensions = "6cmDiam", 
                Price = 0.004m, 
                DeliveryDelay = 13,
                Category = PartCategory.Coupelles,
                StockQuantity = SimulateStock(50, 500),
                MinimumStock = 100
            }
        };

        /// <summary>
        /// Get parts filtered by category
        /// </summary>
        public static List<Part> GetPartsByCategory(PartCategory category)
        {
            return GetAllParts().Where(p => p.Category == category).ToList();
        }

        /// <summary>
        /// Get part by code
        /// </summary>
        public static Part? GetPartByCode(string code)
        {
            return GetAllParts().FirstOrDefault(p => p.Code.Equals(code, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Get parts with low stock
        /// </summary>
        public static List<Part> GetLowStockParts()
        {
            return GetAllParts().Where(p => p.IsLowStock).ToList();
        }

        /// <summary>
        /// Get parts out of stock
        /// </summary>
        public static List<Part> GetOutOfStockParts()
        {
            return GetAllParts().Where(p => p.IsOutOfStock).ToList();
        }

        /// <summary>
        /// Get available dimensions for a specific part type
        /// </summary>
        public static List<int> GetAvailableDimensions(PartCategory category)
        {
            return category switch
            {
                PartCategory.VerticalBatten => new List<int> { 32, 42, 52 },
                PartCategory.CrossbarLeft or PartCategory.CrossbarRight => new List<int> { 32, 42, 52, 62 },
                PartCategory.CrossbarFront or PartCategory.CrossbarBack => new List<int> { 32, 42, 52, 62, 80, 100, 120 },
                _ => new List<int>()
            };
        }

        /// <summary>
        /// Get stock items with complete inventory information
        /// </summary>
        public static List<StockItem> GetStockItems()
        {
            var parts = GetAllParts();
            var stockItems = new List<StockItem>();

            foreach (var part in parts)
            {
                var stockItem = new StockItem
                {
                    Part = part,
                    CurrentStock = part.StockQuantity,
                    ReservedStock = _random.Next(0, Math.Max(1, part.StockQuantity / 4)),
                    MinimumStockLevel = part.MinimumStock,
                    MaximumStockLevel = part.MinimumStock * 8,
                    ReorderPoint = part.MinimumStock + 5,
                    ReorderQuantity = part.MinimumStock * 3,
                    Supplier = GetRandomSupplier(),
                    WarehouseLocation = GetRandomLocation(),
                    ExpectedRestockDate = part.IsLowStock ? DateTime.Now.AddDays(part.DeliveryDelay) : null
                };

                stockItems.Add(stockItem);
            }

            return stockItems;
        }

        /// <summary>
        /// Simulate realistic stock levels
        /// 70% in stock, 20% low stock, 10% out of stock
        /// </summary>
        private static int SimulateStock(int min = 0, int max = 100)
        {
            var probability = _random.NextDouble();
            
            if (probability <= 0.1) // 10% out of stock
                return 0;
            else if (probability <= 0.3) // 20% low stock
                return _random.Next(1, Math.Max(2, min + 8));
            else // 70% in stock
                return _random.Next(Math.Max(15, min), max);
        }

        /// <summary>
        /// Get random supplier for simulation
        /// </summary>
        private static string GetRandomSupplier()
        {
            var suppliers = new[] { "MetalWorks Ltd", "WoodCraft Solutions", "Premium Parts Co", "EuroComponents", "FastTrack Supplies" };
            return suppliers[_random.Next(suppliers.Length)];
        }

        /// <summary>
        /// Get random warehouse location for simulation
        /// </summary>
        private static string GetRandomLocation()
        {
            var locations = new[] { "A1-B3", "B2-C4", "C1-D2", "D3-E1", "E2-F4", "F1-A2" };
            return locations[_random.Next(locations.Length)];
        }

        /// <summary>
        /// Generate sample cabinet configurations for testing
        /// </summary>
        public static List<CabinetConfiguration> GetSampleConfigurations()
        {
            return new List<CabinetConfiguration>
            {
                new CabinetConfiguration
                {
                    Compartments = new List<Compartment>
                    {
                        new Compartment { Height = 32, Width = 32, Depth = 32, Position = 0 },
                        new Compartment { Height = 42, Width = 32, Depth = 32, Position = 1 }
                    },                    PrimaryColor = CabinetColor.White,
                    DoorType = DoorType.Standard,
                    DoorColor = DoorColor.White,
                    CustomerNotes = "Sample 2-compartment cabinet"
                },
                new CabinetConfiguration
                {
                    Compartments = new List<Compartment>
                    {
                        new Compartment { Height = 52, Width = 42, Depth = 32, Position = 0, HasDoor = true },
                        new Compartment { Height = 52, Width = 42, Depth = 32, Position = 1, HasDoor = true },
                        new Compartment { Height = 52, Width = 42, Depth = 32, Position = 2, HasDoor = true }
                    },                    PrimaryColor = CabinetColor.White,
                    DoorType = DoorType.Glass,
                    DoorColor = DoorColor.Glass,
                    IncludeAssembly = true,
                    CustomerNotes = "Premium 3-compartment cabinet with glass doors"
                }
            };
        }
    }
}
