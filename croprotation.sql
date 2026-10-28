-- =========================================================================
-- Crop Rotation and Management System (Mini-Project: CropRotation_164)
-- Database: MySQL
-- =========================================================================

CREATE DATABASE IF NOT EXISTS croprotation_db;
USE croprotation_db;

-- 1. Create Fields Table
CREATE TABLE IF NOT EXISTS Fields (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    PlotName VARCHAR(60) NOT NULL,
    SoilType VARCHAR(50) NOT NULL,
    AreaInAcres DOUBLE NOT NULL,
    Location VARCHAR(100) NOT NULL,
    CurrentNutrientStatus VARCHAR(100) DEFAULT 'Balanced'
);

-- 2. Create Crops Table
CREATE TABLE IF NOT EXISTS Crops (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    CropName VARCHAR(60) NOT NULL,
    Family VARCHAR(60) NOT NULL,
    Season VARCHAR(50) NOT NULL,
    NutrientDemand VARCHAR(100) NOT NULL,
    DurationInDays INT NOT NULL,
    EstimatedYieldPerAcre VARCHAR(100) NOT NULL
);

-- 3. Create RotationPlans Table
CREATE TABLE IF NOT EXISTS RotationPlans (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    FieldId INT NOT NULL,
    CropId INT NOT NULL,
    SeasonYear VARCHAR(50) NOT NULL,
    PlantingDate DATETIME NOT NULL,
    ExpectedHarvestDate DATETIME NOT NULL,
    SoilConditionBefore VARCHAR(100) DEFAULT 'Good',
    RecommendedFertilizer VARCHAR(150) DEFAULT 'Vermicompost',
    Status VARCHAR(50) NOT NULL,
    RotationBenefitScore INT NOT NULL DEFAULT 85,
    CONSTRAINT FK_RotationPlans_Fields FOREIGN KEY (FieldId) REFERENCES Fields(Id) ON DELETE CASCADE,
    CONSTRAINT FK_RotationPlans_Crops FOREIGN KEY (CropId) REFERENCES Crops(Id) ON DELETE RESTRICT
);

-- 4. Seed Initial Fields
INSERT INTO Fields (Id, PlotName, SoilType, AreaInAcres, Location, CurrentNutrientStatus) VALUES
(1, 'North Valley Plot A', 'Alluvial Loam', 4.5, 'Sector 1 - North Perimeter', 'Nitrogen-Rich (Post-Legume)'),
(2, 'Riverside Plot B', 'Black Clay Loam', 6.0, 'Sector 2 - Canal Bank', 'Balanced Organic'),
(3, 'South Plateau Plot C', 'Sandy Loam', 3.2, 'Sector 3 - South Hillside', 'Slightly Nitrogen-Depleted'),
(4, 'East Meadow Plot D', 'Red Loamy Soil', 5.0, 'Sector 4 - East Boundary', 'High Phosphorus, Moderate Nitrogen')
ON DUPLICATE KEY UPDATE PlotName = VALUES(PlotName);

-- 5. Seed Initial Crops
INSERT INTO Crops (Id, CropName, Family, Season, NutrientDemand, DurationInDays, EstimatedYieldPerAcre) VALUES
(1, 'Soybean', 'Legumes (Fabaceae)', 'Kharif (Monsoon)', 'Nitrogen Fixing (Soil Enricher)', 95, '18 - 22 Quintals'),
(2, 'Wheat', 'Cereals (Poaceae)', 'Rabi (Winter)', 'High Nitrogen Consumer', 120, '24 - 28 Quintals'),
(3, 'Mustard', 'Cruciferous (Brassicaceae)', 'Rabi (Winter)', 'Biofumigant / Nematode Suppressor', 105, '8 - 12 Quintals'),
(4, 'Chickpea / Chana', 'Legumes (Fabaceae)', 'Rabi (Winter)', 'Deep Root Nitrogen Fixer', 110, '12 - 16 Quintals'),
(5, 'Corn / Maize', 'Cereals (Poaceae)', 'Kharif (Monsoon)', 'Heavy Nutrient Feeder', 100, '30 - 35 Quintals'),
(6, 'Green Gram / Mung', 'Legumes (Fabaceae)', 'Zaid (Summer)', 'Quick Green Manure & Nitrogen Fixer', 65, '6 - 9 Quintals')
ON DUPLICATE KEY UPDATE CropName = VALUES(CropName);

-- 6. Seed Initial Rotation Plans
INSERT INTO RotationPlans (Id, FieldId, CropId, SeasonYear, PlantingDate, ExpectedHarvestDate, SoilConditionBefore, RecommendedFertilizer, Status, RotationBenefitScore) VALUES
(1, 1, 2, '2026-2027 Rabi', '2026-11-05', '2027-03-15', 'Nitrogen-Enriched by prior Soybean', 'DAP & Bio-NPK Consortium', 'In Progress', 95),
(2, 2, 3, '2026-2027 Rabi', '2026-10-20', '2027-02-10', 'Good soil tilth', 'Sulphur & Farmyard Manure', 'In Progress', 90),
(3, 3, 4, '2026-2027 Rabi', '2026-11-01', '2027-02-28', 'Nitrogen-Depleted', 'Rhizobium Seed Inoculant & SSP', 'Planned', 92),
(4, 4, 1, '2026 Kharif', '2026-06-25', '2026-10-01', 'Organic mulch layer', 'Vermicompost & PSB', 'Harvested', 88)
ON DUPLICATE KEY UPDATE SeasonYear = VALUES(SeasonYear);

-- Display Verification Records
SELECT * FROM Fields;
SELECT * FROM Crops;
SELECT * FROM RotationPlans;
