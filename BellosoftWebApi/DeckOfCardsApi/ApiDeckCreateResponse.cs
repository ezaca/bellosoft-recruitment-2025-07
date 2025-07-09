using System.Text.Json.Serialization;

namespace BellosoftWebApi.DeckOfCardsApi
{
    public class ApiDeckCreateResponse
    {
        [JsonPropertyName("success")]
        public bool Success { get; set; }

        [JsonPropertyName("deck_id")]
        public string DeckId { get; set; }
        
        [JsonPropertyName("shuffled")]
        public bool Shuffled { get; set; }

        [JsonPropertyName("remaining")]
        public int Remaining { get; set; }

        [JsonPropertyName("error")]
        public string? Error { get; set; }
    }
}
