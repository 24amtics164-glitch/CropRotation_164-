namespace CropRotation_164.Models;

public class DashboardViewModel
{
    public int TotalFields { get; set; }
    public int TotalCrops { get; set; }
    public int ActiveRotations { get; set; }
    public int HarvestedCycles { get; set; }
    public double TotalFarmAcres { get; set; }
    public double AverageSoilHealthScore { get; set; }

    public List<RotationPlan> UpcomingRotations { get; set; } = new List<RotationPlan>();
    public List<Field> FieldsSummary { get; set; } = new List<Field>();
    public List<Crop> TopCrops { get; set; } = new List<Crop>();

    public string CurrentSeasonFilter { get; set; } = "All Seasons";
    public string ActiveFarmTheme { get; set; } = "EcoGreen";
}
