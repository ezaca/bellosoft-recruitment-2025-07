using Sqids;

namespace BellosoftWebApi.Services.Sqids
{
    public class SqidsGeneratorService : ISqidsGenerator
    {
        private readonly SqidsEncoder<int> encoder;

        public SqidsGeneratorService()
        {
            var options = new SqidsOptions()
            {
                Alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789abcdefghijklmnopqrstuvwxyz",
                MinLength = 8,
            };
            encoder = new SqidsEncoder<int>(options);
        }

        public string Encode(int value)
            => encoder.Encode(value);

        public int Decode(string sqid)
            => encoder.Decode(sqid.AsSpan()).Single(); // Single requires exactly 1 number
    }
}
