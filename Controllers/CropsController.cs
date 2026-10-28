using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CropRotation_164.Data;
using CropRotation_164.Models;

namespace CropRotation_164.Controllers;

public class CropsController : Controller
{
    private readonly CropDbContext _context;

    public CropsController(CropDbContext context)
    {
        _context = context;
    }

    // =========================================================================
    // READ: List all crops with optional filtering by Family / Season
    // =========================================================================
    [HttpGet]
    public async Task<IActionResult> Index(string? family, string? season)
    {
        ViewBag.SelectedFamily = family;
        ViewBag.SelectedSeason = season;

        var query = _context.Crops.Include(c => c.RotationPlans).AsQueryable();

        if (!string.IsNullOrWhiteSpace(family))
        {
            query = query.Where(c => c.Family.Contains(family));
        }

        if (!string.IsNullOrWhiteSpace(season))
        {
            query = query.Where(c => c.Season.Contains(season));
        }

        var crops = await query.OrderBy(c => c.CropName).ToListAsync();
        return View(crops);
    }

    // =========================================================================
    // READ: View detailed agronomic profile of a crop
    // =========================================================================
    [HttpGet]
    public async Task<IActionResult> Details(int id)
    {
        var crop = await _context.Crops
            .Include(c => c.RotationPlans)
                .ThenInclude(r => r.Field)
            .FirstOrDefaultAsync(m => m.Id == id);

        if (crop == null)
        {
            TempData["ErrorMessage"] = $"Crop with ID {id} was not found.";
            return RedirectToAction(nameof(Index));
        }

        return View(crop);
    }

    // =========================================================================
    // CREATE: Display new crop form
    // =========================================================================
    [HttpGet]
    public IActionResult Create()
    {
        return View(new Crop());
    }

    // CREATE: Process submitted new crop
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Crop crop)
    {
        // Explicit Validation: Check for duplicate crop name
        if (await _context.Crops.AnyAsync(c => c.CropName.ToLower() == crop.CropName.Trim().ToLower()))
        {
            ModelState.AddModelError("CropName", "A crop with this name is already registered.");
        }

        if (!ModelState.IsValid)
        {
            return View(crop);
        }

        _context.Add(crop);
        await _context.SaveChangesAsync();
        TempData["SuccessMessage"] = $"Crop '{crop.CropName}' ({crop.Family}) registered successfully!";
        return RedirectToAction(nameof(Index));
    }

    // =========================================================================
    // UPDATE: Display pre-filled crop form
    // =========================================================================
    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var crop = await _context.Crops.FindAsync(id);
        if (crop == null)
        {
            TempData["ErrorMessage"] = $"Crop with ID {id} was not found.";
            return RedirectToAction(nameof(Index));
        }

        return View(crop);
    }

    // UPDATE: Process submitted updates
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Crop crop)
    {
        if (id != crop.Id)
        {
            TempData["ErrorMessage"] = "Crop ID mismatch.";
            return RedirectToAction(nameof(Index));
        }

        if (await _context.Crops.AnyAsync(c => c.CropName.ToLower() == crop.CropName.Trim().ToLower() && c.Id != id))
        {
            ModelState.AddModelError("CropName", "Another crop is already registered with this name.");
        }

        if (!ModelState.IsValid)
        {
            return View(crop);
        }

        try
        {
            _context.Update(crop);
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = $"Crop '{crop.CropName}' updated successfully!";
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!await _context.Crops.AnyAsync(e => e.Id == crop.Id))
                return NotFound();
            else
                throw;
        }

        return RedirectToAction(nameof(Index));
    }

    // =========================================================================
    // DELETE: Display confirmation screen
    // =========================================================================
    [HttpGet]
    public async Task<IActionResult> Delete(int id)
    {
        var crop = await _context.Crops
            .Include(c => c.RotationPlans)
            .FirstOrDefaultAsync(m => m.Id == id);

        if (crop == null)
        {
            TempData["ErrorMessage"] = $"Crop with ID {id} was not found.";
            return RedirectToAction(nameof(Index));
        }

        return View(crop);
    }

    // DELETE: Process confirmed delete
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var crop = await _context.Crops.FindAsync(id);
        if (crop != null)
        {
            // Explicit Validation: Check if crop is currently locked in rotation plans
            bool inUse = await _context.RotationPlans.AnyAsync(r => r.CropId == id && r.Status != "Harvested");
            if (inUse)
            {
                TempData["ErrorMessage"] = $"Cannot delete crop '{crop.CropName}' because it has active or planned rotations!";
                return RedirectToAction(nameof(Index));
            }

            _context.Crops.Remove(crop);
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = $"Crop '{crop.CropName}' was removed successfully.";
        }

        return RedirectToAction(nameof(Index));
    }
}
