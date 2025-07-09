namespace BellosoftWebApi.Services.Sqids
{
    public interface ISqidsGenerator
    {
        string Encode(int value);
        int Decode(string sqid);
    }
}
