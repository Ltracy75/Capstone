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
    public class RequestLinesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public RequestLinesController(AppDbContext context)
        {
            _context = context;
        }


        private async Task<IActionResult> RecalcRequestTotal(int requestId)
            {
            var exp = await _context.Requests.FindAsync(requestId);
            if (exp is null)
                {
                throw new Exception("Recalc failed!");
                }
            exp.Total = (from el in _context.RequestLines
                         join i in _context.Products
                             on el.ProductId equals i.Id
                         where el.RequestId == requestId
                         select new
                             {
                             Subtotal = el.Quantity * i.Price
                             }).Sum(x => x.Subtotal);
            exp.Status = "Modified";
            await _context.SaveChangesAsync();
            return Ok();
            }

        // GET: api/RequestLines
        [HttpGet]
        public async Task<ActionResult<IEnumerable<RequestLine>>> GetRequestLines()
        {
          if (_context.RequestLines == null)
          {
              return NotFound();
          }
            return await _context.RequestLines.ToListAsync();
        }

        // GET: api/RequestLines/5
        [HttpGet("{id}")]
        public async Task<ActionResult<RequestLine>> GetRequestLine(int id)
        {
          if (_context.RequestLines == null)
          {
              return NotFound();
          }
            var requestLine = await _context.RequestLines.Include(x => x.Request).Include(x => x.Product).SingleOrDefaultAsync(x => x.Id == id);

            if (requestLine == null)
            {
                return NotFound();
            }

            return requestLine;
        }

        // PUT: api/RequestLines/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutRequestLine(int id, RequestLine requestline)
            {
            if (id != requestline.Id)
                {
                return BadRequest();
                }

            _context.Entry(requestline).State = EntityState.Modified;

            try
                {
                await _context.SaveChangesAsync();
                await RecalcRequestTotal(requestline.RequestId);
                }
            catch (DbUpdateConcurrencyException)
                {
                if (!RequestLineExists(id))
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

            // POST: api/RequestLines
            // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
            [HttpPost]
        public async Task<ActionResult<RequestLine>> PostRequestLine(RequestLine requestLine)
        {
          if (_context.RequestLines == null)
          {
              return Problem("Entity set 'AppDbContext.RequestLines'  is null.");
          }
            _context.RequestLines.Add(requestLine);
            await _context.SaveChangesAsync();
            await RecalcRequestTotal(requestLine.RequestId);

            return CreatedAtAction("GetRequestLine", new { id = requestLine.Id }, requestLine);
        }

        // DELETE: api/RequestLines/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRequestLine(int id)
        {
            if (_context.RequestLines == null)
            {
                return NotFound();
            }
            var requestLine = await _context.RequestLines.FindAsync(id);
            if (requestLine == null)
            {
                return NotFound();
            }

            _context.RequestLines.Remove(requestLine);
            await _context.SaveChangesAsync();
            await RecalcRequestTotal(requestLine.RequestId);
            return NoContent();
        }

        private bool RequestLineExists(int id)
        {
            return (_context.RequestLines?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
