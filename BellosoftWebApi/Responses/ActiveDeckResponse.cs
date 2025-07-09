namespace BellosoftWebApi.Responses
{
    public class ActiveDeckResponse
    {
        public string ExternalName { get; set; }
        public DateTime DeckCreatedAt { get; set; }
        public int RemainingCards { get; set; }
        public string OwnerEmail { get; set; }
        public bool IsSelected { get; set; }

        public ActiveDeckResponse(string externalName, DateTime deckCreatedAt, int remainingCards, string ownerEmail, bool isSelected)
        {
            ExternalName = externalName;
            DeckCreatedAt = deckCreatedAt;
            RemainingCards = remainingCards;
            OwnerEmail = ownerEmail;
            IsSelected = isSelected;
        }
    }
}
