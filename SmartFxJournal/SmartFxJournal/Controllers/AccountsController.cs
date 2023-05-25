using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartFxJournal.JournalDB.model;

namespace SmartFxJournal.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountsController : ControllerBase
    {
        private readonly JournalDbContext _context;

        public AccountsController(JournalDbContext context)
        {
            _context = context;
        }

        // GET: api/Accounts
        [HttpGet]
        public async Task<ActionResult<IEnumerable<FxAccount>>> GetAccounts()
        {
          if (_context.FxAccounts == null)
          {
              return NotFound();
          }
            return await _context.FxAccounts.ToListAsync();
        }

        // GET: api/Accounts/5
        [HttpGet("{id}")]
        public async Task<ActionResult<FxAccount>> GetAccount(string id)
        {
          if (_context.FxAccounts == null)
          {
              return NotFound();
          }
            var account = await _context.FxAccounts.FindAsync(id);

            if (account == null)
            {
                return NotFound();
            }

            return account;
        }

        // PUT: api/Accounts/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutAccount(long id, FxAccount account)
        {
            if (id != account.AccountNo)
            {
                return BadRequest();
            }

            _context.Entry(account).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AccountExists(id))
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

        // POST: api/Accounts
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<FxAccount>> PostAccount(FxAccount account)
        {
          if (_context.FxAccounts == null)
          {
              return Problem("Entity set 'JournalDbContext.Accounts'  is null.");
          }
            _context.FxAccounts.Add(account);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (AccountExists(account.AccountNo))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetAccount", new { id = account.AccountNo }, account);
        }

        // DELETE: api/Accounts/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAccount(string id)
        {
            if (_context.FxAccounts == null)
            {
                return NotFound();
            }
            var account = await _context.FxAccounts.FindAsync(id);
            if (account == null)
            {
                return NotFound();
            }

            _context.FxAccounts.Remove(account);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool AccountExists(long id)
        {
            return (_context.FxAccounts?.Any(e => e.AccountNo == id)).GetValueOrDefault();
        }
    }
}
