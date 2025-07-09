using BellosoftWebApi.Responses;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Runtime.CompilerServices;

namespace BellosoftWebApi.Controllers
{
    [Route("api/reports")]
    [ApiController]
    public class ReportsController : ControllerBase
    {
        private readonly AppDbContext context;

        public ReportsController(AppDbContext context) : base()
        {
            this.context = context;
        }

        [ProducesResponseType(typeof(IEnumerable<ActiveDeckResponse>), 200)]
        [HttpGet("active-decks-linq")]
        public async Task<ActionResult<IEnumerable<ActiveDeckResponse>>> ActiveDecksLinq()
        {
            var query = context.Decks
                .Where(deck => deck.DeletedAt == null && deck.User.DeletedAt == null)
                .Select(deck => new ActiveDeckResponse(
                    deck.ExternalName,
                    deck.CreatedAt,
                    deck.RemainingCards,
                    deck.User.Email,
                    deck.User.SelectedDeckId == deck.Id
                ));

            var result = await query.ToListAsync();
            return Ok(result);
        }

        [HttpGet("active-decks-sql")]
        [ProducesResponseType(typeof(IEnumerable<ActiveDeckResponse>), 200)]
        public async Task<ActionResult<IEnumerable<ActiveDeckResponse>>> ActiveDecksSql()
        {
            string sql = """
                SELECT 
                    d.ExternalName,
                    d.CreatedAt AS DeckCreatedAt,
                    d.RemainingCards,
                    u.Email AS OwnerEmail,
                    CAST(CASE WHEN u.SelectedDeckId = d.Id THEN 1 ELSE 0 END AS BIT) AS IsSelected
                FROM Decks d
                INNER JOIN Users u ON u.Id = d.UserId
                WHERE d.DeletedAt IS NULL AND u.DeletedAt IS NULL
            """;

            var result = await context.Database
                .SqlQuery<ActiveDeckResponse>(FormattableStringFactory.Create(sql))
                .ToListAsync();


            return Ok(result);
        }

    }
}
