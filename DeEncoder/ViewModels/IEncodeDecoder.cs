namespace DeEncoder.ViewModels;

public interface IEncodeDecoder
{
    string Encode(string strToEncode, string salt = "");

    string Decode(string setToDecode);
}
