using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Capstone.Models;

namespace Capstone.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RequestsController : ControllerBase
    {
        private readonly AppDbContext _context;
        private const string APPROVED = "Approved";
        private const string REJECTED = "Rejected";
        private const string REVIEW = "Review";
        private const string PAID = "Paid";

        public RequestsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/Requests
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Request>>> GetRequests()
        {
          if (_context.Requests == null)
          {
              return NotFound();
          }
            return await _context.Requests.Include(x => x.User).ToListAsync();
        }

        //*me Get: api/Requests/Review/UserId
        [HttpGet("reviewlst/{userId}")]
        public async Task<ActionResult<IEnumerable<Request>>> GetRequestsInReview(int userId)
            {
            var requests = await _context.Requests
                                    .Where(x => x.Status == "REVIEW"
                                            && x.UserId != userId)
                                    .ToListAsync();
            return requests;
            }

        // GET: api/Requests/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Request>> GetRequest(int id)
        {
          if (_context.Requests == null)
          {
              return NotFound();
          }
            var request = await _context.Requests.Include(x => x.User).Include(x => x.RequestLines)
                                                .ThenInclude(x => x.Product)
                                            .SingleOrDefaultAsync(x => x.Id == id);

            if (request == null)
            {
                return NotFound();
            }

            return request;
        }
        [HttpGet("reviewed")]
        public async Task<ActionResult<IEnumerable<Request>>> GetApprovedRequests()
            {
            return await _context.Requests.Where(x => x.Status == REVIEW).Include(x => x.User).ToListAsync();
            }


        [HttpPut("review/{id}")]
        public async Task<IActionResult> ReviewExpense(int id, Request request)
            {
            var prevStatus = request.Status;
            request.Status = (request.Total <= 50) ? APPROVED : REVIEW;
            var rc = await PutRequest(id, request);
            if ((prevStatus == APPROVED && request.Status != APPROVED)
                || (prevStatus != APPROVED && request.Status == APPROVED)) { }

            return rc;
            }

        // PUT: api/Requests/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutRequest(int id, Request request)
        {
            if (id != request.Id)
            {
                return BadRequest();
            }

            _context.Entry(request).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!RequestExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        [HttpPut("approve/{id}")]
        public async Task<IActionResult> ApproveRequest(int id, Request request)
            {
            if (request.Status == APPROVED)
                {
                return BadRequest();
                }
            request.Status = APPROVED;
            var rc = await PutRequest(id, request);
            
            return rc;
            }

        [HttpPut("reject/{id}")]
        public async Task<IActionResult> RejectRequest(int id, Request request)
            {
            request.Status = REJECTED;
            var rc = await PutRequest(id, request);
            return rc;

            }
            // POST: api/Requests
            // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
            [HttpPost]
        public async Task<ActionResult<Request>> PostRequest(Request request)
        {
          if (_context.Requests == null)
          {
              return Problem("Entity set 'AppDbContext.Requests'  is null.");
          }
            _context.Requests.Add(request);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetRequest", new { id = request.Id }, request);
        }

        // DELETE: api/Requests/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRequest(int id)
        {
            if (_context.Requests == null)
            {
                return NotFound();
            }
            var request = await _context.Requests.FindAsync(id);
            if (request == null)
            {
                return NotFound();
            }

            _context.Requests.Remove(request);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool RequestExists(int id)
        {
            return (_context.Requests?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
