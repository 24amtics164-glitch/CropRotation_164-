using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CropRotation_164.Data;
using CropRotation_164.Models;

namespace CropRotation_164.Controllers;

public class HomeController : Controller
{
    private readonly CropDbContext _context;

    public HomeController(CropDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        // -------------------------------------------------------------
        // State Management Demonstration:
        // 1. Session: Retrieve active seasonal filter (or default to "All Seasons")
        // 2. Cookie: Retrieve or set user theme preference
        // 3. ViewData & ViewBag: Transmit real-time metrics and alerts
        // -------------------------------------------------------------
        string currentSeason = HttpContext.Session.GetString("ActiveSeasonFilter") ?? "All Seasons";

        if (!Request.Cookies.ContainsKey("FarmViewMode"))
        {
            Response.Cookies.Append("FarmViewMode", "EcoCardView", new CookieOptions
            {
                Expires = DateTime.Now.AddDays(7)
            });
        }
        string viewMode = Request.Cookies["FarmViewMode"] ?? "EcoCardView";

        ViewData["ActiveSeason"] = currentSeason;
        ViewBag.ViewMode = viewMode;
        ViewBag.SystemDate = DateTime.Now.ToString("dd MMMM yyyy");

        // Query database metrics via EF Core
        var fields = await _context.Fields.Include(f => f.RotationPlans).ToListAsync();
        var crops = await _context.Crops.ToListAsync();
        
        var rotationQuery = _context.RotationPlans
            .Include(r => r.Field)
            .Include(r => r.Crop)
            .AsQueryable();

        if (currentSeason != "All Seasons")
        {
            rotationQuery = rotationQuery.Where(r => r.SeasonYear.Contains(currentSeason));
        }

        var allRotations = await rotationQuery.OrderByDescending(r => r.PlantingDate).ToListAsync();

        var viewModel = new DashboardViewModel
        {
            TotalFields = fields.Count,
            TotalCrops = crops.Count,
            ActiveRotations = allRotations.Count(r => r.Status == "In Progress"),
            HarvestedCycles = allRotations.Count(r => r.Status == "Harvested"),
            TotalFarmAcres = fields.Sum(f => f.AreaInAcres),
            AverageSoilHealthScore = allRotations.Any() ? Math.Round(allRotations.Average(r => r.RotationBenefitScore), 1) : 85.0,
            UpcomingRotations = allRotations.Take(5).ToList(),
            FieldsSummary = fields,
            TopCrops = crops.Take(6).ToList(),
            CurrentSeasonFilter = currentSeason,
            ActiveFarmTheme = viewMode
        };

        return View(viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult SetSeasonFilter(string seasonFilter)
    {
        // State Management: Save selected season filter into Session
        HttpContext.Session.SetString("ActiveSeasonFilter", seasonFilter);
        TempData["SuccessMessage"] = $"Dashboard filter updated to: {seasonFilter}";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public IActionResult Advisory()
    {
        ViewData["Title"] = "Scientific Crop Rotation Principles & Guidelines";
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
