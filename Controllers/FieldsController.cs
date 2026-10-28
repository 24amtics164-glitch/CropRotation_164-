using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CropRotation_164.Data;
using CropRotation_164.Models;

namespace CropRotation_164.Controllers;

public class FieldsController : Controller
{
    private readonly CropDbContext _context;

    public FieldsController(CropDbContext context)
    {
        _context = context;
    }

    // =========================================================================
    // READ: List all fields / plots (with optional search)
    // =========================================================================
    [HttpGet]
    public async Task<IActionResult> Index(string? search)
    {
        ViewBag.CurrentSearch = search;
        var query = _context.Fields.Include(f => f.RotationPlans).AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(f => 
                f.PlotName.Contains(search) || 
                f.SoilType.Contains(search) || 
                f.Location.Contains(search));
        }

        var fields = await query.OrderBy(f => f.PlotName).ToListAsync();
        return View(fields);
    }

    // =========================================================================
    // READ: View detailed profile of a field
    // =========================================================================
    [HttpGet]
    public async Task<IActionResult> Details(int id)
    {
        var field = await _context.Fields
            .Include(f => f.RotationPlans)
                .ThenInclude(r => r.Crop)
            .FirstOrDefaultAsync(f => f.Id == id);

        if (field == null)
        {
            TempData["ErrorMessage"] = $"Field with ID {id} was not found.";
            return RedirectToAction(nameof(Index));
        }

        return View(field);
    }

    // =========================================================================
    // CREATE: Display new field form
    // =========================================================================
    [HttpGet]
    public IActionResult Create()
    {
        return View(new Field());
    }

    // CREATE: Process submitted new field with validation
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Field field)
    {
        // Explicit Validation: Check for duplicate field/plot name
        if (await _context.Fields.AnyAsync(f => f.PlotName.ToLower() == field.PlotName.Trim().ToLower()))
        {
            ModelState.AddModelError("PlotName", "A field / plot with this name already exists.");
        }

        if (!ModelState.IsValid)
        {
            return View(field);
        }

        _context.Add(field);
        await _context.SaveChangesAsync();
        TempData["SuccessMessage"] = $"Field '{field.PlotName}' ({field.AreaInAcres} Acres) registered successfully!";
        return RedirectToAction(nameof(Index));
    }

    // =========================================================================
    // UPDATE: Display pre-filled edit form
    // =========================================================================
    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var field = await _context.Fields.FindAsync(id);
        if (field == null)
        {
            TempData["ErrorMessage"] = $"Field with ID {id} was not found.";
            return RedirectToAction(nameof(Index));
        }

        return View(field);
    }

    // UPDATE: Process submitted field updates
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Field field)
    {
        if (id != field.Id)
        {
            TempData["ErrorMessage"] = "Field ID mismatch.";
            return RedirectToAction(nameof(Index));
        }

        // Explicit Validation: Check duplicate name excluding current record
        if (await _context.Fields.AnyAsync(f => f.PlotName.ToLower() == field.PlotName.Trim().ToLower() && f.Id != id))
        {
            ModelState.AddModelError("PlotName", "Another field / plot already uses this name.");
        }

        if (!ModelState.IsValid)
        {
            return View(field);
        }

        try
        {
            _context.Update(field);
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = $"Field '{field.PlotName}' updated successfully!";
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!await _context.Fields.AnyAsync(e => e.Id == field.Id))
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
        var field = await _context.Fields
            .Include(f => f.RotationPlans)
            .FirstOrDefaultAsync(m => m.Id == id);

        if (field == null)
        {
            TempData["ErrorMessage"] = $"Field with ID {id} was not found.";
            return RedirectToAction(nameof(Index));
        }

        return View(field);
    }

    // DELETE: Confirm deletion
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var field = await _context.Fields.FindAsync(id);
        if (field != null)
        {
            _context.Fields.Remove(field);
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = $"Field '{field.PlotName}' and associated rotation plans were removed.";
        }
        else
        {
            TempData["ErrorMessage"] = $"Field with ID {id} not found.";
        }

        return RedirectToAction(nameof(Index));
    }
}
