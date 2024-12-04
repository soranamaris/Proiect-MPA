using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Proiect_MPA.Data;
using Proiect_MPA.Models;

namespace Proiect_MPA.Controllers
{
    [Authorize]
    public class ReservationsController : Controller
    {
        private readonly RestaurantContext _context;
        private readonly UserManager<IdentityUser> _userManager;


        public ReservationsController(RestaurantContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Reservations

        public async Task<IActionResult> Index(string sortOrder, string searchString)
        {
            // Configurează parametrii pentru sortare și căutare
            ViewData["DateSortParm"] = String.IsNullOrEmpty(sortOrder) ? "date_desc" : "";
            ViewData["CurrentFilter"] = searchString;

            // Obține adresa de e-mail a utilizatorului conectat
            var userEmail = _userManager.GetUserName(User);

            IQueryable<Reservation> reservations;

            if (User.IsInRole("Manager"))
            {
                // Manager vede toate rezervările
                reservations = _context.Reservation.Include(r => r.Client);
            }
            else
            {
                // Utilizatorii normali văd doar rezervările proprii
                reservations = _context.Reservation
                    .Include(r => r.Client)
                    .Where(r => r.Client.Email == userEmail);
            }

            // Adu toate datele din baza de date
            var reservationList = await reservations.ToListAsync();

            // Aplică filtrarea în memorie
            if (!String.IsNullOrEmpty(searchString))
            {
                reservationList = reservationList
                    .Where(r => r.Client != null &&
                                !string.IsNullOrEmpty(r.Client.FullName) &&
                                r.Client.FullName.Contains(searchString, StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }

            // Aplică sortarea
            reservationList = sortOrder switch
            {
                "date_desc" => reservationList.OrderByDescending(r => r.ReservationDate).ToList(),
                _ => reservationList.OrderBy(r => r.ReservationDate).ToList()
            };

            return View(reservationList);
        }

        // GET: Reservations/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var reservation = await _context.Reservation
                .Include(r => r.Client)
                .FirstOrDefaultAsync(m => m.ID == id);
            if (reservation == null)
            {
                return NotFound();
            }

            return View(reservation);
        }

        // GET: Reservations/Create

        public IActionResult Create()
        {
            ViewBag.ClientID = new SelectList(_context.Client.Select(c => new
            {
                c.ID,
                Display = c.FullName + " - " + c.Email
            }), "ID", "Display");
            return View();
        }

        // POST: Reservations/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,ClientID,TableID,ReservationDate,ReservationTime,ReservationDuration")] Reservation reservation)
        {
            if (ModelState.IsValid)
            {
                _context.Add(reservation);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ClientID"] = new SelectList(_context.Client, "ID", "ID", reservation.ClientID);
            return View(reservation);
        }

        // GET: Reservations/Edit/5
        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var reservation = await _context.Reservation.FindAsync(id);
            if (reservation == null)
            {
                return NotFound();
            }
            ViewData["ClientID"] = new SelectList(_context.Client, "ID", "ID", reservation.ClientID);
            return View(reservation);
        }

        // POST: Reservations/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> Edit(int id, [Bind("ID,ClientID,TableID,ReservationDate,ReservationTime,ReservationDuration")] Reservation reservation)
        {
            if (id != reservation.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(reservation);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ReservationExists(reservation.ID))
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
            ViewData["ClientID"] = new SelectList(_context.Client, "ID", "ID", reservation.ClientID);
            return View(reservation);
        }

        // GET: Reservations/Delete/5
        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var reservation = await _context.Reservation
                .Include(r => r.Client)
                .FirstOrDefaultAsync(m => m.ID == id);
            if (reservation == null)
            {
                return NotFound();
            }

            return View(reservation);
        }

        // POST: Reservations/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var reservation = await _context.Reservation.FindAsync(id);
            if (reservation != null)
            {
                _context.Reservation.Remove(reservation);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ReservationExists(int id)
        {
            return _context.Reservation.Any(e => e.ID == id);
        }
    }
}
