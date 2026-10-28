using System.ComponentModel.DataAnnotations;

namespace CropRotation_164.Models;

public class RotationPlan
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Please select a Field / Plot.")]
    [Display(Name = "Field / Plot")]
    public int FieldId { get; set; }
    public virtual Field? Field { get; set; }

    [Required(ErrorMessage = "Please select a Crop.")]
    [Display(Name = "Crop to Plant")]
    public int CropId { get; set; }
    public virtual Crop? Crop { get; set; }

    [Required(ErrorMessage = "Season & Year is required (e.g., 2026-2027 Rabi).")]
    [Display(Name = "Rotation Cycle / Season")]
    public string SeasonYear { get; set; } = string.Empty;

    [Required(ErrorMessage = "Planting Date is required.")]
    [DataType(DataType.Date)]
    [Display(Name = "Planting Date")]
    public DateTime PlantingDate { get; set; } = DateTime.Today;

    [Required(ErrorMessage = "Expected Harvest Date is required.")]
    [DataType(DataType.Date)]
    [Display(Name = "Expected Harvest Date")]
    public DateTime ExpectedHarvestDate { get; set; } = DateTime.Today.AddDays(120);

    [Display(Name = "Soil Condition Prior")]
    public string SoilConditionBefore { get; set; } = "Good";

    [Display(Name = "Recommended Bio-Fertilizer / Compost")]
    public string RecommendedFertilizer { get; set; } = "Vermicompost & Rhizobium";

    [Required(ErrorMessage = "Status is required.")]
    [Display(Name = "Cycle Status")]
    public string Status { get; set; } = "Planned"; // Planned, In Progress, Harvested, Rotated

    [Range(0, 100, ErrorMessage = "Rotation Benefit Score must be between 0 and 100.")]
    [Display(Name = "Rotation Health Score (%)")]
    public int RotationBenefitScore { get; set; } = 85;
}
