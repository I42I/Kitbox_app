// filepath: b:\Ecam\BA3\Kitbox_app\KitBoxDesigner\Models\PartSpecification.cs
namespace KitBoxDesigner.Models
{
    /// <summary>
    /// Defines the characteristics of a part required for a cabinet,
    /// used to find a specific part from the stock.
    /// </summary>
    public class PartSpecification
    {
        /// <summary>
        /// Category of the part (e.g., VerticalBatten, PanelHorizontal).
        /// </summary>
        public PartCategory Category { get; set; }

        /// <summary>
        /// Required height of the part in cm. Null if not applicable.
        /// </summary>
        public int? Height { get; set; }

        /// <summary>
        /// Required width of the part in cm. Null if not applicable.
        /// </summary>
        public int? Width { get; set; }

        /// <summary>
        /// Required depth of the part in cm. Null if not applicable.
        /// </summary>
        public int? Depth { get; set; }

        /// <summary>
        /// Required color of the part (e.g., "White", "Brown"). Null if not applicable or any color is fine.
        /// </summary>
        public string? Color { get; set; }

        /// <summary>
        /// Optional: Specific type attribute, e.g., for angle irons "BL", "NR", "GL".
        /// Could also be used for door types if not covered by category/color.
        /// </summary>
        public string? TypeAttribute { get; set; }
        
        /// <summary>
        /// Quantity of this specific part needed.
        /// </summary>
        public int Quantity { get; set; }

        public PartSpecification(PartCategory category, int quantity, int? height = null, int? width = null, int? depth = null, string? color = null, string? typeAttribute = null)
        {
            Category = category;
            Quantity = quantity;
            Height = height;
            Width = width;
            Depth = depth;
            Color = color;
            TypeAttribute = typeAttribute;
        }
    }
}
