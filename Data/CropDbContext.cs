using Microsoft.EntityFrameworkCore;
using CropRotation_164.Models;

namespace CropRotation_164.Data;

public class CropDbContext : DbContext
{
    public CropDbContext(DbContextOptions<CropDbContext> options)
        : base(options)
    {
    }

    public DbSet<Field> Fields => Set<Field>();
    public DbSet<Crop> Crops => Set<Crop>();
    public DbSet<RotationPlan> RotationPlans => Set<RotationPlan>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure Field-RotationPlan 1-to-many
        modelBuilder.Entity<RotationPlan>()
            .HasOne(r => r.Field)
            .WithMany(f => f.RotationPlans)
            .HasForeignKey(r => r.FieldId)
            .OnDelete(DeleteBehavior.Cascade);

        // Configure Crop-RotationPlan 1-to-many
        modelBuilder.Entity<RotationPlan>()
            .HasOne(r => r.Crop)
            .WithMany(c => c.RotationPlans)
            .HasForeignKey(r => r.CropId)
            .OnDelete(DeleteBehavior.Restrict);

        // Seed initial Field data
        modelBuilder.Entity<Field>().HasData(
            new Field
            {
                Id = 1,
                PlotName = "North Valley Plot A",
                SoilType = "Alluvial Loam",
                AreaInAcres = 4.5,
                Location = "Sector 1 - North Perimeter",
                CurrentNutrientStatus = "Nitrogen-Rich (Post-Legume)"
            },
            new Field
            {
                Id = 2,
                PlotName = "Riverside Plot B",
                SoilType = "Black Clay Loam",
                AreaInAcres = 6.0,
                Location = "Sector 2 - Canal Bank",
                CurrentNutrientStatus = "Balanced Organic"
            },
            new Field
            {
                Id = 3,
                PlotName = "South Plateau Plot C",
                SoilType = "Sandy Loam",
                AreaInAcres = 3.2,
                Location = "Sector 3 - South Hillside",
                CurrentNutrientStatus = "Slightly Nitrogen-Depleted"
            },
            new Field
            {
                Id = 4,
                PlotName = "East Meadow Plot D",
                SoilType = "Red Loamy Soil",
                AreaInAcres = 5.0,
                Location = "Sector 4 - East Boundary",
                CurrentNutrientStatus = "High Phosphorus, Moderate Nitrogen"
            }
        );

        // Seed initial Crop data with rotation benefits
        modelBuilder.Entity<Crop>().HasData(
            new Crop
            {
                Id = 1,
                CropName = "Soybean",
                Family = "Legumes (Fabaceae)",
                Season = "Kharif (Monsoon)",
                NutrientDemand = "Nitrogen Fixing (Soil Enricher)",
                DurationInDays = 95,
                EstimatedYieldPerAcre = "18 - 22 Quintals"
            },
            new Crop
            {
                Id = 2,
                CropName = "Wheat",
                Family = "Cereals (Poaceae)",
                Season = "Rabi (Winter)",
                NutrientDemand = "High Nitrogen Consumer",
                DurationInDays = 120,
                EstimatedYieldPerAcre = "24 - 28 Quintals"
            },
            new Crop
            {
                Id = 3,
                CropName = "Mustard",
                Family = "Cruciferous (Brassicaceae)",
                Season = "Rabi (Winter)",
                NutrientDemand = "Biofumigant / Nematode Suppressor",
                DurationInDays = 105,
                EstimatedYieldPerAcre = "8 - 12 Quintals"
            },
            new Crop
            {
                Id = 4,
                CropName = "Chickpea / Chana",
                Family = "Legumes (Fabaceae)",
                Season = "Rabi (Winter)",
                NutrientDemand = "Deep Root Nitrogen Fixer",
                DurationInDays = 110,
                EstimatedYieldPerAcre = "12 - 16 Quintals"
            },
            new Crop
            {
                Id = 5,
                CropName = "Corn / Maize",
                Family = "Cereals (Poaceae)",
                Season = "Kharif (Monsoon)",
                NutrientDemand = "Heavy Nutrient Feeder",
                DurationInDays = 100,
                EstimatedYieldPerAcre = "30 - 35 Quintals"
            },
            new Crop
            {
                Id = 6,
                CropName = "Green Gram / Mung",
                Family = "Legumes (Fabaceae)",
                Season = "Zaid (Summer)",
                NutrientDemand = "Quick Green Manure & Nitrogen Fixer",
                DurationInDays = 65,
                EstimatedYieldPerAcre = "6 - 9 Quintals"
            }
        );

        // Seed initial Crop Rotation plans
        modelBuilder.Entity<RotationPlan>().HasData(
            new RotationPlan
            {
                Id = 1,
                FieldId = 1,
                CropId = 2, // Wheat following Soybean
                SeasonYear = "2026-2027 Rabi",
                PlantingDate = new DateTime(2026, 11, 5),
                ExpectedHarvestDate = new DateTime(2027, 3, 15),
                SoilConditionBefore = "Nitrogen-Enriched by prior Soybean",
                RecommendedFertilizer = "DAP & Bio-NPK Consortium",
                Status = "In Progress",
                RotationBenefitScore = 95
            },
            new RotationPlan
            {
                Id = 2,
                FieldId = 2,
                CropId = 3, // Mustard to break pest cycle
                SeasonYear = "2026-2027 Rabi",
                PlantingDate = new DateTime(2026, 10, 20),
                ExpectedHarvestDate = new DateTime(2027, 2, 10),
                SoilConditionBefore = "Good soil tilth",
                RecommendedFertilizer = "Sulphur & Farmyard Manure",
                Status = "In Progress",
                RotationBenefitScore = 90
            },
            new RotationPlan
            {
                Id = 3,
                FieldId = 3,
                CropId = 4, // Chickpea to fix nitrogen in depleted soil
                SeasonYear = "2026-2027 Rabi",
                PlantingDate = new DateTime(2026, 11, 1),
                ExpectedHarvestDate = new DateTime(2027, 2, 28),
                SoilConditionBefore = "Nitrogen-Depleted",
                RecommendedFertilizer = "Rhizobium Seed Inoculant & Single Super Phosphate",
                Status = "Planned",
                RotationBenefitScore = 92
            },
            new RotationPlan
            {
                Id = 4,
                FieldId = 4,
                CropId = 1, // Soybean
                SeasonYear = "2026 Kharif",
                PlantingDate = new DateTime(2026, 6, 25),
                ExpectedHarvestDate = new DateTime(2026, 10, 1),
                SoilConditionBefore = "Organic mulch layer",
                RecommendedFertilizer = "Vermicompost & PSB",
                Status = "Harvested",
                RotationBenefitScore = 88
            }
        );
    }
}
