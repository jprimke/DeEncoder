namespace DeEncoder.ViewModels
{
    public interface IEncodeDecoder
    {
        string Encode(string strToEncode);

        string Decode(string setToDecode);
    }
}