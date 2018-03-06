using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using AXCEX_ONLINE.Data;
using AXCEX_ONLINE.Models;

namespace AXCEX_ONLINE.Controllers
{
    public class ProjectAssignmentsController : Controller
    {
        private readonly ProjectDbContext _context;

        public ProjectAssignmentsController(ProjectDbContext context)
        {
            _context = context;
        }

        // GET: ProjectAssignments
        public async Task<IActionResult> Index()
        {
            return View(await _context.ProjectAssignments.ToListAsync());
        }

        // GET: ProjectAssignments/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var projectAssignment = await _context.ProjectAssignments
                .SingleOrDefaultAsync(m => m.ID == id);
            if (projectAssignment == null)
            {
                return NotFound();
            }

            return View(projectAssignment);
        }

        // GET: ProjectAssignments/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: ProjectAssignments/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,EmpKey,ProjKey,authorized_assignment")] ProjectAssignment projectAssignment)
        {
            if (ModelState.IsValid)
            {
                _context.Add(projectAssignment);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(projectAssignment);
        }

        // GET: ProjectAssignments/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var projectAssignment = await _context.ProjectAssignments.SingleOrDefaultAsync(m => m.ID == id);
            if (projectAssignment == null)
            {
                return NotFound();
            }
            return View(projectAssignment);
        }

        // POST: ProjectAssignments/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,EmpKey,ProjKey,authorized_assignment")] ProjectAssignment projectAssignment)
        {
            if (id != projectAssignment.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(projectAssignment);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProjectAssignmentExists(projectAssignment.ID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(projectAssignment);
        }

        // GET: ProjectAssignments/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var projectAssignment = await _context.ProjectAssignments
                .SingleOrDefaultAsync(m => m.ID == id);
            if (projectAssignment == null)
            {
                return NotFound();
            }

            return View(projectAssignment);
        }

        // POST: ProjectAssignments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var projectAssignment = await _context.ProjectAssignments.SingleOrDefaultAsync(m => m.ID == id);
            _context.ProjectAssignments.Remove(projectAssignment);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProjectAssignmentExists(int id)
        {
            return _context.ProjectAssignments.Any(e => e.ID == id);
        }
    }
}
