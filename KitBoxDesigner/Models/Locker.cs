// filepath: b:\Ecam\BA3\Kitbox_app\KitBoxDesigner\Models\Locker.cs
using System.Collections.Generic;

namespace KitBoxDesigner.Models
{
    /// <summary>
    /// Represents a single locker in a cabinet configuration.
    /// Responsible for determining the parts it requires based on its specifications.
    /// </summary>
    public class Locker
    {
        /// <summary>
        /// Overall height of the locker in cm.
        /// This height is used to determine the height of vertical battens and side/back panels.
        /// Vertical batten height = LockerHeight - 4cm (for top and bottom crossbars).
        /// </summary>
        public int Height { get; set; }

        /// <summary>
        /// Width of the locker in cm.
        /// </summary>
        public int Width { get; set; }

        /// <summary>
        /// Depth of the locker in cm.
        /// </summary>
        public int Depth { get; set; }

        /// <summary>
        /// Color for the panels (Horizontal, Side, Back) of the locker.
        /// </summary>
        public string PanelColor { get; set; }

        /// <summary>
        /// Indicates whether the locker has doors.
        /// </summary>
        public bool HasDoors { get; set; }

        /// <summary>
        /// Color for the doors. Can be different from PanelColor.
        /// Null if HasDoors is false.
        /// </summary>
        public string? DoorColor { get; set; }

        /// <summary>
        /// Indicates if the doors are made of glass.
        /// Cup handles are not available for glass doors.
        /// </summary>
        public bool IsDoorGlass { get; set; }

        // Standard thickness of crossbars (2cm top, 2cm bottom) affecting internal component height.
        private const int CrossbarThickness = 2; // cm
        private const int TotalCrossbarHeightEffect = CrossbarThickness * 2; // cm

        public Locker(int height, int width, int depth, string panelColor, bool hasDoors = false, string? doorColor = null, bool isDoorGlass = false)
        {
            Height = height;
            Width = width;
            Depth = depth;
            PanelColor = panelColor;
            HasDoors = hasDoors;
            DoorColor = hasDoors ? doorColor : null;
            IsDoorGlass = hasDoors && isDoorGlass;

            if (hasDoors && string.IsNullOrEmpty(doorColor))
            {
                // Potentially throw an argument exception or default the door color
                // For now, we assume valid inputs or further validation elsewhere.
            }
        }

        /// <summary>
        /// Generates a list of PartSpecification objects required to build this locker.
        /// </summary>
        /// <returns>A list of PartSpecifications for the locker.</returns>
        public List<PartSpecification> GeneratePartSpecifications()
        {
            var specifications = new List<PartSpecification>();
            int componentHeight = Height - TotalCrossbarHeightEffect; // Height for vertical battens, side/back panels, doors

            // 1. Vertical Battens (4 units)
            specifications.Add(new PartSpecification(PartCategory.VerticalBatten, 4, height: componentHeight, color: PanelColor)); // Color might not apply to battens or be standard

            // 2. Crossbars
            //    - Front (2 units, 2 grooves for doors if HasDoors)
            //    - Back (2 units, 1 groove for panel)
            //    - Side (4 units, 1 groove for panel)
            //    TypeAttribute could be used to specify groove requirements if not implicit by category.
            specifications.Add(new PartSpecification(PartCategory.CrossbarFront, 2, width: Width, typeAttribute: HasDoors ? "2Grooves" : "NoGroovesForDoor")); // Color might be standard
            specifications.Add(new PartSpecification(PartCategory.CrossbarBack, 2, width: Width, typeAttribute: "1Groove"));  // Color might be standard
            specifications.Add(new PartSpecification(PartCategory.CrossbarSide, 4, depth: Depth, typeAttribute: "1Groove"));  // Color might be standard

            // 3. Horizontal Panels (2 units)
            specifications.Add(new PartSpecification(PartCategory.PanelHorizontal, 2, width: Width, depth: Depth, color: PanelColor));

            // 4. Side Panels (2 units)
            specifications.Add(new PartSpecification(PartCategory.PanelSide, 2, height: componentHeight, depth: Depth, color: PanelColor));

            // 5. Back Panel (1 unit)
            specifications.Add(new PartSpecification(PartCategory.PanelBack, 1, height: componentHeight, width: Width, color: PanelColor));

            // 6. Doors (Optional, 2 units)
            if (HasDoors)
            {
                // Door dimensions might need slight adjustments (e.g., slightly smaller than opening)
                // For now, using componentHeight and half width.
                // Door color can be different.
                // TypeAttribute could specify material if "Glass" is a type, or IsDoorGlass property is used by PartService.
                string? doorType = IsDoorGlass ? "Glass" : null;
                specifications.Add(new PartSpecification(PartCategory.Door, 2, height: componentHeight, width: Width / 2, color: DoorColor, typeAttribute: doorType));

                // 7. Cup Handles (Optional, 2 units, not for glass doors)
                if (!IsDoorGlass)
                {
                    specifications.Add(new PartSpecification(PartCategory.CupHandle, 2)); // Color/type might be standard or selectable
                }
            }

            return specifications;
        }
    }
}