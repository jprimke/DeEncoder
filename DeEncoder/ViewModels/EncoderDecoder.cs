using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace DeEncoder.ViewModels;

public class EncoderDecoder : IEncodeDecoder
{
    private readonly Codecs codec;

    public EncoderDecoder(Codecs codec)
    {
        this.codec = codec;
    }

    public string Encode(string strToEncode, string salt = "")
    {
        var bytes = Encoding.UTF8.GetBytes(strToEncode);
        string result = null;

        switch (codec)
        {
            case Codecs.Base64:
                result = Convert.ToBase64String(bytes);
                break;
            case Codecs.SHA1:
            case Codecs.SHA256:
            case Codecs.SHA384:
            case Codecs.SHA512:
            case Codecs.MD5:
                var algorithm = EncodeDecoderHashAlgorithmFactory.Create(codec);
                var hash = algorithm.ComputeHash(bytes);
                result = hash.Aggregate("", (current, b) => current + b.ToString("x2"));
                break;
            default:
                throw new ArgumentOutOfRangeException($"The codec {codec} is not usable");
        }

        return result;
    }

    public string Decode(string strToDecode)
    {
        if (codec != Codecs.Base64)
        {
            throw new ArgumentException("Can't decode hashes");
        }

        var data = Convert.FromBase64String(strToDecode);
        var decodedString = Encoding.UTF8.GetString(data);

        return decodedString;
    }
}
