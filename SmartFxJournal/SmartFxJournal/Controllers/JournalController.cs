using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartFxJournal.JournalDB.model;

namespace SmartFxJournal.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class JournalController : ControllerBase
    {
        private readonly JournalDbContext _context;

        public JournalController(JournalDbContext context)
        {
            _context = context;
        }

        // GET: api/Journal
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PositionJournalEntry>>> GetPositionJournalEntries()
        {
          if (_context.PositionJournalEntries == null)
          {
              return NotFound();
          }
            return await _context.PositionJournalEntries.ToListAsync();
        }

        // GET: api/Journal/5
        [HttpGet("{id}")]
        public async Task<ActionResult<PositionJournalEntry>> GetPositionJournalEntry(long id)
        {
          if (_context.PositionJournalEntries == null)
          {
              return NotFound();
          }
            var positionJournalEntry = await _context.PositionJournalEntries.Where(j => j.ParentId == id).FirstOrDefaultAsync();

            if (positionJournalEntry == null)
            {
                return NotFound();
            }

            return positionJournalEntry;
        }

        // PUT: api/Journal/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPositionJournalEntry(long id, PositionJournalEntry positionJournalEntry)
        {
            if (id != positionJournalEntry.JournalId)
            {
                return BadRequest();
            }

            _context.Entry(positionJournalEntry).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PositionJournalEntryExists(id))
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

        // POST: api/Journal
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<PositionJournalEntry>> PostPositionJournalEntry(PositionJournalEntry positionJournalEntry)
        {
          if (_context.PositionJournalEntries == null)
          {
              return Problem("Entity set 'JournalDbContext.PositionJournalEntries'  is null.");
          }
            _context.PositionJournalEntries.Add(positionJournalEntry);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetPositionJournalEntry", new { id = positionJournalEntry.JournalId }, positionJournalEntry);
        }

        // DELETE: api/Journal/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePositionJournalEntry(long id)
        {
            if (_context.PositionJournalEntries == null)
            {
                return NotFound();
            }
            var positionJournalEntry = await _context.PositionJournalEntries.FindAsync(id);
            if (positionJournalEntry == null)
            {
                return NotFound();
            }

            _context.PositionJournalEntries.Remove(positionJournalEntry);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool PositionJournalEntryExists(long id)
        {
            return (_context.PositionJournalEntries?.Any(e => e.JournalId == id)).GetValueOrDefault();
        }
    }
}
