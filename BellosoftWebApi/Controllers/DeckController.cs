using BellosoftWebApi.Core;
using BellosoftWebApi.DeckOfCardsApi;
using BellosoftWebApi.Models;
using BellosoftWebApi.Responses;
using BellosoftWebApi.Services;
using BellosoftWebApi.Services.AuthenticatedUser;
using BellosoftWebApi.Services.Sqids;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace BellosoftWebApi.Controllers
{
    [Route("api/deck")]
    [ApiController]
    public class DeckController : ControllerBase
    {
        private readonly AppDbContext context;
        private readonly IAuthenticatedUser authenticatedUser;

        public DeckController(AppDbContext context, IAuthenticatedUser authenticatedUser)
        {
            this.context = context;
            this.authenticatedUser = authenticatedUser;
        }

        [Authorize]
        [HttpGet("list")]
        [ProducesResponseType(typeof(DeckListResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(MessageResponse), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> ListDecks([FromServices] ISqidsGenerator sqids)
        {
            AuthUserData? authUser = await authenticatedUser.GetActiveUser();
            if (authUser is null)
                return Unauthorized("Sessão expirada ou usuário não entrou");

            int userId = authUser.Id;
            var decks = await context.Decks
                .Where(deck => deck.UserId == userId && deck.DeletedAt == null)
                .Select(deck => new { deck.Id, deck.RemainingCards })
                .ToListAsync();

            List<DeckResponse> decksResponse = decks.ConvertAll(deck => new DeckResponse(sqids, deck.Id, deck.RemainingCards));
            DeckListResponse listResponse = new DeckListResponse(sqids, authUser.SelectedDeckId, decksResponse);
            return Ok(listResponse);
        }

        [Authorize]
        [HttpPost("select/{id:length(8)}")]
        [ProducesResponseType(typeof(DeckResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(MessageResponse), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(MessageResponse), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> SelectDeck([FromRoute]string id, [FromServices] ISqidsGenerator sqids)
        {
            AuthUserData? authUser = await authenticatedUser.GetActiveUser();
            if (authUser is null)
                return Unauthorized(new MessageResponse("Sessão expirada ou usuário não entrou"));

            int userId = authUser.Id;
            int deckId = sqids.Decode(id);
            var deck = await context.Decks.FindAsync(deckId);

            if (deck is null || deck.UserId != authUser.Id || deck.DeletedAt is not null)
                return NotFound(new MessageResponse("Baralho não encontrado"));

            User user = new User(authUser.Id);
            EntityEntry<User> userEntity = context.Entry(user);
            context.Attach(user);
            userEntity.SetProperty(user => user.SelectedDeckId, deck.Id);
            userEntity.SetProperty(u => u.UpdatedAt, DateTime.UtcNow);
            await context.SaveChangesAsync();
            
            return Ok(new DeckResponse(sqids, deck.Id, deck.RemainingCards));
        }

        [Authorize]
        [HttpPost("create")]
        [ProducesResponseType(typeof(DeckCreateResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(MessageResponse), StatusCodes.Status503ServiceUnavailable)]
        [ProducesResponseType(typeof(MessageResponse), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> CreateDeck([FromServices]IDeckOfCards api, [FromServices]ISqidsGenerator sqids)
        {
            AuthUserData? authUser = await authenticatedUser.GetActiveUser();
            if (authUser is null)
                return Unauthorized(new MessageResponse("Sessão expirada ou usuário não entrou"));

            ApiDeckCreateResponse? apiDeck = await api.CreateDeck(1);
            if (apiDeck is null)
                return StatusCode(503, new MessageResponse("Erro ao criar baralho. Tente novamente ou verifique a disponibilidade do serviço."));
            
            if (!apiDeck.Success)
                return StatusCode(503, new MessageResponse($"Erro ao criar baralho, tente novamente ou verifique a disponibilidade do serviço. Erro externo: {apiDeck.Error}"));

            Deck deck = new Deck(authUser.Id, apiDeck.DeckId, apiDeck.Remaining);
            context.Decks.Add(deck);
            await context.SaveChangesAsync();

            ChangeSelectedDeck(authUser.Id, deck.Id);
            await context.SaveChangesAsync();

            return Ok(new DeckCreateResponse(sqids, deck.Id, deck.RemainingCards));
        }

        [Authorize]
        [HttpPost("delete/{id:length(8)}")]
        [ProducesResponseType(typeof(MessageResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(MessageResponse), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(MessageResponse), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteDeck([FromRoute]string id, [FromServices]ISqidsGenerator sqids)
        {
            AuthUserData? authUser = await authenticatedUser.GetActiveUser();
            if (authUser is null)
                return Unauthorized(new MessageResponse("Sessão expirada ou usuário não entrou"));

            int deckId = sqids.Decode(id);
            Deck? deck = await context.Decks.FindAsync(deckId);
            if (deck is null || deck.DeletedAt is not null || deck.UserId != authUser.Id)
                return NotFound(new MessageResponse("Baralho não encontrado"));

            using (var transaction = await context.Database.BeginTransactionAsync())
            {
                deck.DeletedAt = DateTime.UtcNow;
                context.SaveChanges();

                if (authUser.SelectedDeckId == deck.Id)
                {
                    ChangeSelectedDeck(authUser.Id, null);
                    await context.SaveChangesAsync();
                }

                await context.SaveChangesAsync();
                await transaction.CommitAsync();
            }

            return Ok(new MessageResponse("Baralho excluído"));
        }

        [Authorize]
        [HttpPost("draw/{count:int}")]
        [ProducesResponseType(typeof(DrawCardResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(MessageResponse), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(MessageResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(MessageResponse), StatusCodes.Status503ServiceUnavailable)]
        public async Task<IActionResult> DrawCard([FromRoute]int count, [FromServices]ISqidsGenerator sqids, [FromServices]IDeckOfCards api)
        {
            AuthUserData? authUser = await authenticatedUser.GetActiveUser();
            if (authUser is null)
                return Unauthorized(new MessageResponse("Sessão expirada ou usuário não entrou"));

            if (authUser.SelectedDeckId is null)
                return BadRequest(new MessageResponse("Não há um baralho selecionado"));

            int userId = authUser.Id;
            int deckId = (int)authUser.SelectedDeckId;

            var deck = await context.Decks.FindAsync(deckId);
            if (deck is null || deck.UserId != authUser.Id || deck.DeletedAt is not null)
                return NotFound(new MessageResponse("Baralho não encontrado"));

            var apiResponse = await api.DrawCard(deck.ExternalName, count);

            if (apiResponse is null)
                return StatusCode(503, new MessageResponse("Erro ao sacar uma carta. Tente novamente ou verifique a disponibilidade do serviço."));

            if (!apiResponse.Success)
                return StatusCode(503, new MessageResponse($"Erro ao sacar uma carta, tente novamente ou verifique a disponibilidade do serviço. Erro externo: {apiResponse.Error}"));

            deck.RemainingCards = apiResponse.Remaining;
            deck.UpdatedAt = DateTime.UtcNow;
            await context.SaveChangesAsync();

            return Ok(new DrawCardResponse(sqids, deck.Id, apiResponse.Remaining, apiResponse.Cards));
        }

        private void ChangeSelectedDeck(int userId, int? deckId)
        {
            User user = new User(userId);
            EntityEntry<User> userEntity = context.Entry(user);
            context.Attach(user);
            userEntity.SetProperty(u => u.SelectedDeckId, deckId);
            userEntity.SetProperty(u => u.UpdatedAt, DateTime.UtcNow);
        }
    }
}
