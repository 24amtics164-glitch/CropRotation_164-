using System.ComponentModel.DataAnnotations;

namespace CropRotation_164.Models;

public class Field
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Plot / Field Name is required.")]
    [StringLength(60, MinimumLength = 3, ErrorMessage = "Field Name must be between 3 and 60 characters.")]
    [Display(Name = "Field / Plot Name")]
    public string PlotName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Soil Type is required.")]
    [Display(Name = "Soil Type")]
    public string SoilType { get; set; } = string.Empty;

    [Required(ErrorMessage = "Area in acres is required.")]
    [Range(0.1, 1000.0, ErrorMessage = "Area must be between 0.1 and 1000 acres.")]
    [Display(Name = "Area (Acres)")]
    public double AreaInAcres { get; set; }

    [Required(ErrorMessage = "Location is required.")]
    [StringLength(100, ErrorMessage = "Location cannot exceed 100 characters.")]
    [Display(Name = "Farm Location / Sector")]
    public string Location { get; set; } = string.Empty;

    [Display(Name = "Current Nutrient Status")]
    public string CurrentNutrientStatus { get; set; } = "Balanced";

    // Navigation property
    public virtual ICollection<RotationPlan> RotationPlans { get; set; } = new List<RotationPlan>();
}
