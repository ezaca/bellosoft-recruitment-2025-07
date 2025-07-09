using Sqids;

namespace BellosoftWebApi.Services.Sqids
{
    public class SqidsGeneratorService : ISqidsGenerator
    {
        private readonly SqidsEncoder<int> encoder;

        public SqidsGeneratorService(IConfiguration configuration)
        {
            string alphabet = configuration["Sqids:Alphabet"]
                ?? throw new InvalidDataException("Key not configured for ID hashing algorithn");

            var options = new SqidsOptions()
            {
                Alphabet = alphabet,
                MinLength = 8, // Hard-coded because it is hard-coded in the routes too, no meaning in being configurable
            };
            encoder = new SqidsEncoder<int>(options);
        }

        public string Encode(int value)
            => encoder.Encode(value);

        public int Decode(string sqid)
            => encoder.Decode(sqid.AsSpan()).Single(); // Single requires exactly 1 number
    }
}
