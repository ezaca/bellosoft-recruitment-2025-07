using BellosoftWebApi.Configurations;
using BellosoftWebApi.DeckOfCardsApi;
using Microsoft.Extensions.Options;
using System.Text.RegularExpressions;

namespace BellosoftWebApi.Services
{
    public class DeckOfCardsService : IDeckOfCards
    {
        public Regex ValidIdRegex = new Regex(@"^\w{1,50}$", RegexOptions.Compiled);

        private readonly string baseUrl;
        private readonly string createDeckPath;
        private readonly string drawCardPath;
        private readonly HttpClient httpClient;

        public DeckOfCardsService(IOptions<DeckOfCardsConfiguration> configuration, IHttpClientFactory httpClientFactory)
        {
            DeckOfCardsConfiguration config = configuration.Value;
            baseUrl = config.BaseUrl;
            createDeckPath = config.CreateDeckPath;
            drawCardPath = config.DrawCardPath;
            httpClient = httpClientFactory.CreateClient();
        }

        public async Task<DeckCreateApiResponse?> CreateDeck(int amount)
        {
            string url = baseUrl + string.Format(createDeckPath, amount);
            DeckCreateApiResponse? response = await httpClient.GetFromJsonAsync<DeckCreateApiResponse>(url);
            return response;
        }

        public async Task<DeckDrawResponse?> DrawCard(string id, int amount)
        {
            id = Uri.EscapeDataString(id);

            if (!ValidIdRegex.IsMatch(id))
                return new DeckDrawResponse() { Success = false, Error = "O ID não está em um formato válido." };

            string url = baseUrl + string.Format(drawCardPath, id, amount);
            DeckDrawResponse? response = await httpClient.GetFromJsonAsync<DeckDrawResponse>(url);
            return response;
        }
    }
}
