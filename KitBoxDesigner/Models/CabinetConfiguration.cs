using System;
using System.Collections.Generic;
using System.Linq;

namespace KitBoxDesigner.Models
{
    /// <summary>
    /// Represents a complete cabinet configuration with all compartments and options
    /// </summary>
    public class CabinetConfiguration
    {
        /// <summary>
        /// Unique identifier for this configuration
        /// </summary>
        public Guid Id { get; set; } = Guid.NewGuid();

        /// <summary>
        /// List of compartments in this cabinet (max 7)
        /// </summary>
        public List<Compartment> Compartments { get; set; } = new();        /// <summary>
        /// Primary color scheme for the cabinet
        /// </summary>
        public CabinetColor PrimaryColor { get; set; } = CabinetColor.White;

        /// <summary>
        /// Overall cabinet width in cm
        /// </summary>
        public int Width { get; set; }

        /// <summary>
        /// Overall cabinet depth in cm
        /// </summary>
        public int Depth { get; set; }

        /// <summary>
        /// Overall cabinet height in cm
        /// </summary>
        public int Height { get; set; }

        /// <summary>
        /// Cabinet color (alias for PrimaryColor for compatibility)
        /// </summary>
        public CabinetColor Color
        {
            get => PrimaryColor;
            set => PrimaryColor = value;
        }

        /// <summary>
        /// Type of door finish
        /// </summary>
        public DoorType DoorType { get; set; } = DoorType.None;

        /// <summary>
        /// Door color (if doors are selected)
        /// </summary>
        public DoorColor DoorColor { get; set; } = DoorColor.White;

        /// <summary>
        /// Angle iron color/material
        /// </summary>
        public AngleIronType AngleIronType { get; set; } = AngleIronType.White;

        /// <summary>
        /// Whether to include assembly service
        /// </summary>
        public bool IncludeAssembly { get; set; } = false;

        /// <summary>
        /// Customer notes for this configuration
        /// </summary>
        public string CustomerNotes { get; set; } = string.Empty;

        /// <summary>
        /// Formatted dimensions for display
        /// </summary>
        public string FormattedDimensions => $"{Width} × {Depth} × {Height} cm";

        /// <summary>
        /// Date when configuration was created
        /// </summary>
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        /// <summary>
        /// Date when configuration was last modified
        /// </summary>
        public DateTime LastModified { get; set; } = DateTime.Now;

        /// <summary>
        /// Total height of the cabinet (sum of all compartments + 4cm for angle irons)
        /// </summary>
        public int TotalHeight => Compartments.Sum(c => c.Height) + 4;

        /// <summary>
        /// Maximum width of all compartments
        /// </summary>
        public int MaxWidth => Compartments.Any() ? Compartments.Max(c => c.Width) : 0;

        /// <summary>
        /// Maximum depth of all compartments
        /// </summary>
        public int MaxDepth => Compartments.Any() ? Compartments.Max(c => c.Depth) : 0;

        /// <summary>
        /// Number of compartments in this cabinet
        /// </summary>
        public int CompartmentCount => Compartments.Count;

        /// <summary>
        /// Check if configuration is valid (max 7 compartments, at least 1)
        /// </summary>
        public bool IsValid => CompartmentCount > 0 && CompartmentCount <= 7;

        /// <summary>
        /// Get all required parts for this configuration
        /// </summary>
        public List<PartRequirement> GetRequiredParts()
        {
            var requirements = new List<PartRequirement>();

            foreach (var compartment in Compartments)
            {
                requirements.AddRange(compartment.GetRequiredParts());
            }

            // Add angle irons (4 pieces, length based on total height)
            var angleIronLength = CalculateAngleIronLength();
            var angleIronCode = GetAngleIronCode(angleIronLength, AngleIronType);
            requirements.Add(new PartRequirement { PartCode = angleIronCode, Quantity = 4 });

            // Add coupelles (4 pieces)
            requirements.Add(new PartRequirement { PartCode = "COUPEL", Quantity = 4 });

            return requirements.GroupBy(r => r.PartCode)
                .Select(g => new PartRequirement { PartCode = g.Key, Quantity = g.Sum(r => r.Quantity) })
                .ToList();
        }

        /// <summary>
        /// Calculate angle iron length based on total cabinet height
        /// </summary>
        private int CalculateAngleIronLength()
        {
            return TotalHeight;
        }

        /// <summary>
        /// Get appropriate angle iron code based on length and type
        /// </summary>
        private string GetAngleIronCode(int length, AngleIronType type)
        {
            var suffix = type switch
            {
                AngleIronType.White => "BL",
                AngleIronType.Black => "NR",
                AngleIronType.Galvanized => "GL",
                _ => "BL"
            };

            // Simplified logic - in real app would need more complex calculation
            return length <= 32 ? $"COR35{suffix}" : $"COR66{suffix}";
        }

        public void UpdateLastModified()
        {
            LastModified = DateTime.Now;
        }
    }

    /// <summary>
    /// Represents a single compartment within a cabinet
    /// </summary>
    public class Compartment
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public int Height { get; set; } = 32; // Default height in cm
        public int Width { get; set; } = 32; // Default width in cm  
        public int Depth { get; set; } = 32; // Default depth in cm
        public bool HasDoor { get; set; } = false;
        public DoorColor DoorColor { get; set; } = DoorColor.White;
        public int Position { get; set; } // Position in the cabinet (0-based from bottom)

        /// <summary>
        /// Get all parts required for this compartment
        /// </summary>
        public List<PartRequirement> GetRequiredParts()
        {
            var requirements = new List<PartRequirement>();

            // 4 vertical battens
            var verticalBattenCode = GetVerticalBattenCode(Height);
            requirements.Add(new PartRequirement { PartCode = verticalBattenCode, Quantity = 4 });

            // 6 crossbars (2 front, 2 back, 2 lateral)
            var lateralCrossbarCode = GetLateralCrossbarCode(Depth);
            requirements.Add(new PartRequirement { PartCode = lateralCrossbarCode, Quantity = 2 });

            var frontCrossbarCode = GetFrontCrossbarCode(Width);
            requirements.Add(new PartRequirement { PartCode = frontCrossbarCode, Quantity = 2 });

            var backCrossbarCode = GetBackCrossbarCode(Width);
            requirements.Add(new PartRequirement { PartCode = backCrossbarCode, Quantity = 2 });

            // 5 panels (2 horizontal, 2 lateral, 1 back)
            var backPanelCode = GetBackPanelCode(Height, Width);
            requirements.Add(new PartRequirement { PartCode = backPanelCode, Quantity = 1 });

            // Add horizontal and lateral panels (simplified for now)
            // In real implementation, would need specific panel codes

            // Door if selected
            if (HasDoor)
            {
                var doorCode = GetDoorCode(Height, Width, DoorColor);
                requirements.Add(new PartRequirement { PartCode = doorCode, Quantity = 1 });
            }

            return requirements;
        }

        private string GetVerticalBattenCode(int height)
        {
            return height switch
            {
                32 => "TAS27",
                42 => "TAS37",
                52 => "TAS47",
                _ => "TAS27" // Default fallback
            };
        }

        private string GetLateralCrossbarCode(int depth)
        {
            return depth switch
            {
                32 => "TRG32",
                42 => "TRG42",
                52 => "TRG52",
                62 => "TRG62",
                _ => "TRG32" // Default fallback
            };
        }

        private string GetFrontCrossbarCode(int width)
        {
            return width switch
            {
                32 => "TRF32",
                42 => "TRF42",
                52 => "TRF52",
                62 => "TRF62",
                80 => "TRF80",
                100 => "TRF100",
                120 => "TRF120",
                _ => "TRF32" // Default fallback
            };
        }

        private string GetBackCrossbarCode(int width)
        {
            return width switch
            {
                32 => "TRR32",
                42 => "TRR42",
                52 => "TRR52",
                62 => "TRR62",
                80 => "TRR80",
                100 => "TRR100",
                120 => "TRR120",
                _ => "TRR32" // Default fallback
            };
        }

        private string GetBackPanelCode(int height, int width)
        {
            // Simplified logic - in real app would have complete matrix
            return $"PAR{height}{width}BL"; // White panels
        }

        private string GetDoorCode(int height, int width, DoorColor color)
        {
            var colorSuffix = color switch
            {
                DoorColor.White => "BL",
                DoorColor.Brown => "BR",
                DoorColor.Glass => "VE",
                _ => "BL"
            };
            return $"POR{height}{width}{colorSuffix}";
        }
    }

    /// <summary>
    /// Represents a requirement for a specific part and quantity
    /// </summary>
    public class PartRequirement
    {
        public string PartCode { get; set; } = string.Empty;
        public int Quantity { get; set; }
    }

    /// <summary>
    /// Available cabinet colors
    /// </summary>
    public enum CabinetColor
    {
        White,
        Black,
        Natural
    }

    /// <summary>
    /// Available door types
    /// </summary>
    public enum DoorType
    {
        None,
        Standard,
        Glass
    }

    /// <summary>
    /// Available door colors
    /// </summary>
    public enum DoorColor
    {
        White,
        Brown,
        Glass
    }

    /// <summary>
    /// Available angle iron types
    /// </summary>
    public enum AngleIronType
    {
        White,
        Black,
        Galvanized
    }
}
