namespace BellosoftWebApi.Models
{
    public class Deck
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
        public int ExternalName { get; set; }
        public int RemainingCards { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }

        public Deck() { }

        public Deck(int userId, int externalName, int remainingCards)
        {
            UserId = userId;
            ExternalName = externalName;
            RemainingCards = remainingCards;
            CreatedAt = DateTime.UtcNow;
            UpdatedAt = DateTime.UtcNow;
        }
    }
}
