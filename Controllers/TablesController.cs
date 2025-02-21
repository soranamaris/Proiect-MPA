using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Proiect_MPA.Data;
using Proiect_MPA.Models;

namespace Proiect_MPA.Controllers
{
    [Authorize]
    public class TablesController : Controller
    {
        private readonly RestaurantContext _context;
       

        public TablesController(RestaurantContext context)
        {
            _context = context;
            

        }

        
        // GET: Tables
        public async Task<IActionResult> Index()
        {
            var restaurantContext = _context.Table
        .Include(t => t.Waiter)
        .Include(t => t.Zone)
        .Include(t => t.BookingSchedules) // Include BookingSchedules
            .ThenInclude(bs => bs.Schedule); // Include Schedule asociat
            return View(await restaurantContext.ToListAsync());
        }

        // GET: Tables/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var table = await _context.Table
                .Include(t => t.Waiter)
                .Include(t => t.Zone)
                .FirstOrDefaultAsync(m => m.ID == id);
            if (table == null)
            {
                return NotFound();
            }

            return View(table);
        }

        // GET: Tables/Create
        [Authorize(Roles = "Manager")]
        public IActionResult Create()
        {
            ViewBag.ZoneID = new SelectList(_context.Zone, "ID", "Name");
            ViewBag.WaiterID = new SelectList(_context.Waiter, "ID", "Name");
            ViewBag.ReservationID = new SelectList(_context.Schedule, "ID", "ScheduleName");
            return View();
        }

        // POST: Tables/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> Create([Bind("ID,NumberTable,Seats,WaiterID,ZoneID,ReservationID")] Table table)
        {
            if (ModelState.IsValid)
            {
                _context.Add(table);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["WaiterID"] = new SelectList(_context.Waiter, "ID", "Name", table.WaiterID);
            ViewData["ZoneID"] = new SelectList(_context.Zone, "ID", "Name", table.ZoneID);
            ViewData["ReservationID"] = new SelectList(_context.Schedule, "ID", "ScheduleName", table.ReservationID);

            return View(table);
        }

        // GET: Tables/Edit/5
        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var table = await _context.Table
         .Include(t => t.BookingSchedules)
         .ThenInclude(bs => bs.Schedule) // Include programările asociate
         .FirstOrDefaultAsync(t => t.ID == id);

            if (table == null)
            {
                return NotFound();
            }

            // Populează ViewBag și datele de programare
            var pageModel = new TableSchedulesPageModel();
            pageModel.PopulateAssignedScheduleData(_context, table);
            table.AssignedScheduleDataList = pageModel.AssignedScheduleDataList ?? new List<AssignedScheduleData>(); // Populează datele programărilor
            ViewData["WaiterID"] = new SelectList(_context.Waiter, "ID", "ID", table.WaiterID);
            ViewData["ZoneID"] = new SelectList(_context.Zone, "ID", "ID", table.ZoneID);
            ViewData["ReservationID"] = new SelectList(_context.Schedule, "ID", "ScheduleName", table.ReservationID);

            return View(table);
        }

        // POST: Tables/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> Edit(int id, [Bind("ID,NumberTable,Seats,WaiterID,ZoneID,ReservationID")] Table table , string[] selectedSchedules)
        {
            if (id != table.ID)
            {
                return NotFound();
            }

            var tableToUpdate = await _context.Table
                .Include(t => t.BookingSchedules) // Include programările existente
                .FirstOrDefaultAsync(t => t.ID == id);

            if (tableToUpdate == null)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Actualizează programările tabelului
                    UpdateTableSchedules(selectedSchedules, tableToUpdate);

                    tableToUpdate.Seats = table.Seats;
                    tableToUpdate.WaiterID = table.WaiterID;
                    tableToUpdate.ZoneID = table.ZoneID;
                    tableToUpdate.ReservationID = table.ReservationID;

                    _context.Update(tableToUpdate);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TableExists(table.ID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }

            ViewData["WaiterID"] = new SelectList(_context.Waiter, "ID", "Name", table.WaiterID);
            ViewData["ZoneID"] = new SelectList(_context.Zone, "ID", "Name", table.ZoneID);
            ViewData["ReservationID"] = new SelectList(_context.Schedule, "ID", "ScheduleName", table.ReservationID);

            return View(table);
        }
        private void UpdateTableSchedules(string[] selectedSchedules, Table tableToUpdate)
        {
            if (selectedSchedules == null)
            {
                tableToUpdate.BookingSchedules = new List<BookingSchedule>();
                return;
            }

            var selectedSchedulesHS = new HashSet<string>(selectedSchedules);
            var tableSchedules = new HashSet<int>(tableToUpdate.BookingSchedules.Select(bs => bs.ScheduleID));

            foreach (var schedule in _context.Schedule)
            {
                if (selectedSchedulesHS.Contains(schedule.ID.ToString()))
                {
                    if (!tableSchedules.Contains(schedule.ID))
                    {
                        tableToUpdate.BookingSchedules.Add(new BookingSchedule
                        {
                            TableID = tableToUpdate.ID,
                            ScheduleID = schedule.ID
                        });
                    }
                }
                else
                {
                    if (tableSchedules.Contains(schedule.ID))
                    {
                        var scheduleToRemove = tableToUpdate.BookingSchedules.FirstOrDefault(bs => bs.ScheduleID == schedule.ID);
                        if (scheduleToRemove != null)
                        {
                            _context.BookingSchedule.Remove(scheduleToRemove);
                        }
                    }
                }
            }
        }
        // GET: Tables/Delete/5
        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var table = await _context.Table
                .Include(t => t.Waiter)
                .Include(t => t.Zone)
                .FirstOrDefaultAsync(m => m.ID == id);
            if (table == null)
            {
                return NotFound();
            }

            return View(table);
        }

        // POST: Tables/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var table = await _context.Table.FindAsync(id);
            if (table != null)
            {
                _context.Table.Remove(table);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TableExists(int id)
        {
            return _context.Table.Any(e => e.ID == id);
        }

    }
}
