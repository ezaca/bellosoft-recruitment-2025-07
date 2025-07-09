namespace BellosoftWebApi.DeckOfCardsApi
{
    public class DeckCreateApiResponse
    {
        public bool Success { get; set; }
        public string DeckId { get; set; }
        public bool Suffled { get; set; }
        public int Remaining { get; set; }
        public string? Error { get; set; }
    }
}
