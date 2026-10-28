using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CropRotation_164.Data;
using CropRotation_164.Models;

namespace CropRotation_164.Controllers;

public class RotationPlansController : Controller
{
    private readonly CropDbContext _context;

    public RotationPlansController(CropDbContext context)
    {
        _context = context;
    }

    // =========================================================================
    // READ: List all crop rotation cycles (with filters)
    // =========================================================================
    [HttpGet]
    public async Task<IActionResult> Index(int? fieldId, string? status)
    {
        ViewBag.SelectedFieldId = fieldId;
        ViewBag.SelectedStatus = status;
        ViewBag.FieldsList = new SelectList(await _context.Fields.OrderBy(f => f.PlotName).ToListAsync(), "Id", "PlotName", fieldId);

        var query = _context.RotationPlans
            .Include(r => r.Field)
            .Include(r => r.Crop)
            .AsQueryable();

        if (fieldId.HasValue && fieldId.Value > 0)
        {
            query = query.Where(r => r.FieldId == fieldId.Value);
        }

        if (!string.IsNullOrWhiteSpace(status))
        {
            query = query.Where(r => r.Status == status);
        }

        var plans = await query.OrderByDescending(r => r.PlantingDate).ToListAsync();
        return View(plans);
    }

    // =========================================================================
    // READ: View rotation cycle details
    // =========================================================================
    [HttpGet]
    public async Task<IActionResult> Details(int id)
    {
        var plan = await _context.RotationPlans
            .Include(r => r.Field)
            .Include(r => r.Crop)
            .FirstOrDefaultAsync(m => m.Id == id);

        if (plan == null)
        {
            TempData["ErrorMessage"] = $"Rotation cycle #{id} not found.";
            return RedirectToAction(nameof(Index));
        }

        return View(plan);
    }

    // =========================================================================
    // CREATE: Display new rotation scheduling form
    // =========================================================================
    [HttpGet]
    public async Task<IActionResult> Create()
    {
        ViewBag.FieldId = new SelectList(await _context.Fields.OrderBy(f => f.PlotName).ToListAsync(), "Id", "PlotName");
        ViewBag.CropId = new SelectList(await _context.Crops.OrderBy(c => c.CropName).ToListAsync(), "Id", "CropName");
        return View(new RotationPlan());
    }

    // CREATE: Process submitted rotation with intelligent agricultural validation
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(RotationPlan plan)
    {
        // ---------------------------------------------------------------------
        // Explicit Agricultural Model Validation Rules:
        // ---------------------------------------------------------------------

        // Rule 1: Chronological validation
        if (plan.ExpectedHarvestDate <= plan.PlantingDate)
        {
            ModelState.AddModelError("ExpectedHarvestDate", "Explicit Error: Expected Harvest Date must be chronologically after the Planting Date.");
        }

        var durationDays = (plan.ExpectedHarvestDate - plan.PlantingDate).TotalDays;
        if (durationDays < 30)
        {
            ModelState.AddModelError("ExpectedHarvestDate", "Explicit Error: Minimum growing cycle duration must be at least 30 days.");
        }

        // Rule 2: Intelligent Rotation Check (Analyze previous crop on the same field)
        var priorPlan = await _context.RotationPlans
            .Include(r => r.Crop)
            .Where(r => r.FieldId == plan.FieldId && r.Id != plan.Id)
            .OrderByDescending(r => r.ExpectedHarvestDate)
            .FirstOrDefaultAsync();

        var selectedCrop = await _context.Crops.FindAsync(plan.CropId);

        if (priorPlan != null && selectedCrop != null && priorPlan.Crop != null)
        {
            // Same botanical family warning
            if (priorPlan.Crop.Family == selectedCrop.Family)
            {
                plan.RotationBenefitScore = 60; // Penalize score
                ViewBag.RotationWarning = $"Agronomic Advisory: Crop '{selectedCrop.CropName}' belongs to the same family ({selectedCrop.Family}) as prior crop '{priorPlan.Crop.CropName}'. Rotating different families is recommended to break pest cycles.";
            }
            // Legume followed by Cereal or vice versa is ideal!
            else if (priorPlan.Crop.Family.Contains("Legumes") || selectedCrop.Family.Contains("Legumes"))
            {
                plan.RotationBenefitScore = 95; // Excellent score
            }
            else
            {
                plan.RotationBenefitScore = 85;
            }
        }
        else
        {
            plan.RotationBenefitScore = 85;
        }

        if (!ModelState.IsValid)
        {
            ViewBag.FieldId = new SelectList(await _context.Fields.OrderBy(f => f.PlotName).ToListAsync(), "Id", "PlotName", plan.FieldId);
            ViewBag.CropId = new SelectList(await _context.Crops.OrderBy(c => c.CropName).ToListAsync(), "Id", "CropName", plan.CropId);
            return View(plan);
        }

        _context.Add(plan);
        await _context.SaveChangesAsync();
        TempData["SuccessMessage"] = $"Crop rotation schedule created for '{plan.SeasonYear}' (Health Score: {plan.RotationBenefitScore}%)!";
        return RedirectToAction(nameof(Index));
    }

    // =========================================================================
    // UPDATE: Display pre-filled edit form
    // =========================================================================
    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var plan = await _context.RotationPlans.FindAsync(id);
        if (plan == null)
        {
            TempData["ErrorMessage"] = $"Rotation cycle #{id} not found.";
            return RedirectToAction(nameof(Index));
        }

        ViewBag.FieldId = new SelectList(await _context.Fields.OrderBy(f => f.PlotName).ToListAsync(), "Id", "PlotName", plan.FieldId);
        ViewBag.CropId = new SelectList(await _context.Crops.OrderBy(c => c.CropName).ToListAsync(), "Id", "CropName", plan.CropId);
        return View(plan);
    }

    // UPDATE: Process submitted updates
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, RotationPlan plan)
    {
        if (id != plan.Id)
        {
            TempData["ErrorMessage"] = "Cycle ID mismatch.";
            return RedirectToAction(nameof(Index));
        }

        if (plan.ExpectedHarvestDate <= plan.PlantingDate)
        {
            ModelState.AddModelError("ExpectedHarvestDate", "Harvest Date must be after Planting Date.");
        }

        if (!ModelState.IsValid)
        {
            ViewBag.FieldId = new SelectList(await _context.Fields.OrderBy(f => f.PlotName).ToListAsync(), "Id", "PlotName", plan.FieldId);
            ViewBag.CropId = new SelectList(await _context.Crops.OrderBy(c => c.CropName).ToListAsync(), "Id", "CropName", plan.CropId);
            return View(plan);
        }

        try
        {
            _context.Update(plan);
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = $"Rotation cycle #{id} updated successfully!";
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!await _context.RotationPlans.AnyAsync(e => e.Id == plan.Id))
                return NotFound();
            else
                throw;
        }

        return RedirectToAction(nameof(Index));
    }

    // =========================================================================
    // QUICK STATUS TOGGLE
    // =========================================================================
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateStatus(int id, string newStatus)
    {
        var plan = await _context.RotationPlans.FindAsync(id);
        if (plan != null)
        {
            plan.Status = newStatus;
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = $"Cycle #{id} status changed to '{newStatus}'.";
        }
        return RedirectToAction(nameof(Index));
    }

    // =========================================================================
    // DELETE: Display confirmation screen
    // =========================================================================
    [HttpGet]
    public async Task<IActionResult> Delete(int id)
    {
        var plan = await _context.RotationPlans
            .Include(r => r.Field)
            .Include(r => r.Crop)
            .FirstOrDefaultAsync(m => m.Id == id);

        if (plan == null)
        {
            TempData["ErrorMessage"] = $"Rotation cycle #{id} not found.";
            return RedirectToAction(nameof(Index));
        }

        return View(plan);
    }

    // DELETE: Process delete
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var plan = await _context.RotationPlans.FindAsync(id);
        if (plan != null)
        {
            _context.RotationPlans.Remove(plan);
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = $"Rotation cycle #{id} has been removed.";
        }

        return RedirectToAction(nameof(Index));
    }
}
