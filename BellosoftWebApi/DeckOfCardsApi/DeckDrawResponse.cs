namespace BellosoftWebApi.DeckOfCardsApi
{
    public class DeckDrawResponse
    {
        public bool Success { get; set; }
        public string DeckId { get; set; }
        public int Remaining { get; set; }
        public List<CardData> Cards { get; set; }
        public string? Error { get; set; }
    }
}
