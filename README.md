# AgriRotate 164: Crop Rotation & Soil Management System 🌱

A comprehensive sustainable agriculture mini-project developed using **ASP.NET Core MVC** and **MySQL**.

---

## 👨‍💻 Student Information
* **Name:** Patel Om
* **Enrollment No:** 20240310510164
* **Division:** F &bull; **Batch:** 4
* **Semester:** 5th Semester

---

## 📋 Project Overview
Continuous monoculture farming causes severe soil degradation, nutrient depletion (Nitrogen, Phosphorus, Potassium), and chronic pest infestation. **AgriRotate 164** digitizes farm plots and provides intelligent, multi-season crop succession planning.

### Key Features:
* **Executive KPI Dashboard:** Real-time metrics for registered farm plots, crop catalog, active rotation cycles, and average rotation health index.
* **Farm Fields (Plots) Management:** Complete CRUD operations tracking plot area in acres, soil classifications (Alluvial Loam, Black Clay Loam, etc.), and nutrient conditions.
* **Crops Master Catalog:** Categorization of crops across botanical families (*Legumes / Fabaceae*, *Cereals / Poaceae*, *Cruciferous / Brassicaceae*), growing durations, and nutrient demands.
* **Intelligent Rotation Scheduling:** 
  * Full CRUD on crop succession cycles.
  * Explicit agronomic validation (e.g. harvest date > planting date, minimum 30-day growth).
  * Automated rotation scoring: Rewards legume-cereal rotation (95% score), penalizes consecutive monoculture planting (60% score).
  * Quick status transitions (*Planned*, *In Progress*, *Harvested*).
* **Scientific Agronomy Advisory:** Educational guide explaining nitrogen fixation, taproot depth alternation, and biofumigation pest disruption.
* **State Management:**
  * **Session:** Saves active seasonal filter.
  * **TempData:** Flash notifications across HTTP 302 redirects.
  * **Cookies:** User view preferences.

---

## 🛠️ Technology Stack
* **Framework:** ASP.NET Core MVC (.NET 10)
* **Language:** C#
* **Database:** MySQL 8.0 / MariaDB
* **ORM:** Pomelo.EntityFrameworkCore.MySql 6.0.3
* **Front-End:** Razor Views, Bootstrap 5, Bootstrap Icons, HTML5, CSS3

---

## 🚀 How to Run the Project Locally

### 1. Prerequisites
* [.NET 10 SDK](https://dotnet.microsoft.com/)
* MySQL Server / XAMPP running on port `3306`

### 2. Database Setup
The database script `croprotation.sql` is included in the project root. Run it in MySQL Workbench or via command line:
```bash
mysql -u root < croprotation.sql
```
*(The application also automatically initializes and seeds the database via Entity Framework Core on first launch).*

### 3. Connection Configuration
Ensure your MySQL credentials match `appsettings.json`:
```json
"ConnectionStrings": {
  "DefaultConnection": "Server=localhost;Port=3306;Database=croprotation_db;User=root;Password=;"
}
```

### 4. Run Application
```bash
cd CropRotation_164
dotnet run
```
Open your browser and navigate to:
`http://localhost:5235` (or `https://localhost:7120`)

---

## 📄 License
Academic Mini-Project Submission.
