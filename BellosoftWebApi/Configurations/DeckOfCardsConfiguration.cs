namespace BellosoftWebApi.Configurations
{
    [Serializable]
    public class DeckOfCardsConfiguration
    {
        public string BaseUrl { get; set; }
        public string CreateDeckPath { get; set; }
        public string DrawCardPath { get; set; }
    }
}
