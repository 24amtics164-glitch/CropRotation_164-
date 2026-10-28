using System.ComponentModel.DataAnnotations;

namespace CropRotation_164.Models;

public class Crop
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Crop Name is required.")]
    [StringLength(60, MinimumLength = 2, ErrorMessage = "Crop Name must be between 2 and 60 characters.")]
    [Display(Name = "Crop Name")]
    public string CropName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Botanical Family is required for rotation logic.")]
    [Display(Name = "Botanical Family")]
    public string Family { get; set; } = string.Empty;

    [Required(ErrorMessage = "Farming Season is required.")]
    [Display(Name = "Season")]
    public string Season { get; set; } = string.Empty;

    [Required(ErrorMessage = "Nutrient Demand is required.")]
    [Display(Name = "Nutrient Demand")]
    public string NutrientDemand { get; set; } = string.Empty;

    [Required(ErrorMessage = "Duration is required.")]
    [Range(30, 365, ErrorMessage = "Growing duration must be between 30 and 365 days.")]
    [Display(Name = "Duration (Days)")]
    public int DurationInDays { get; set; }

    [Display(Name = "Estimated Yield / Acre")]
    public string EstimatedYieldPerAcre { get; set; } = string.Empty;

    // Navigation property
    public virtual ICollection<RotationPlan> RotationPlans { get; set; } = new List<RotationPlan>();
}
