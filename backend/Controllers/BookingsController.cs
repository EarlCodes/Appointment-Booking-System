using Microsoft.AspNetCore.Mvc;
using BookingApi.Data;
using BookingApi.Models;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace BookingApi.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class BookingsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public BookingsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/bookings - Get all bookings for the authenticated user
        [HttpGet]
        public IActionResult GetAll()
        {
            var userId = GetCurrentUserId();
            if (userId == null) return Unauthorized();

            var userBookings = _context.Bookings
                .Where(b => b.UserId == userId)
                .OrderByDescending(b => b.Date)
                .ThenBy(b => b.Time)
                .ToList();

            return Ok(userBookings);
        }

        // GET: api/bookings/{id} - Get a single booking by ID 
        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var userId = GetCurrentUserId();
            if (userId == null) return Unauthorized();

            var booking = _context.Bookings
                .FirstOrDefault(b => b.Id == id && b.UserId == userId);

            if (booking == null) return NotFound();

            return Ok(booking);
        }

        // POST: api/bookings - Create a new booking for the authenticated user
        [HttpPost]
        public IActionResult Create([FromBody] Booking booking)
        {
            var userId = GetCurrentUserId();
            if (userId == null) return Unauthorized();

            booking.UserId = userId.Value;
            _context.Bookings.Add(booking);
            _context.SaveChanges();
            return Ok(booking);
        }

        // PUT: api/bookings/{id} - Update a booking (must belong to user)
        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody] Booking updated)
        {
            var userId = GetCurrentUserId();
            if (userId == null) return Unauthorized();

            var booking = _context.Bookings
                .FirstOrDefault(b => b.Id == id && b.UserId == userId);

            if (booking == null) return NotFound();

            booking.Date = updated.Date;
            booking.Time = updated.Time;
            booking.Description = updated.Description;

            _context.SaveChanges();
            return NoContent();
        }

        // DELETE: api/bookings/{id} - Delete a booking (must belong to user)
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var userId = GetCurrentUserId();
            if (userId == null) return Unauthorized();

            var booking = _context.Bookings
                .FirstOrDefault(b => b.Id == id && b.UserId == userId);

            if (booking == null) return NotFound();

            _context.Bookings.Remove(booking);
            _context.SaveChanges();
            return NoContent();
        }

        private int? GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
                return null;

            return userId;
        }
    }
}
