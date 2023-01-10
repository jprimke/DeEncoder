using System.Security.Cryptography;

namespace DeEncoder.ViewModels;

public static class EncodeDecoderHashAlgorithmFactory
{
    public static HashAlgorithm Create(Codecs codec)
    {
        HashAlgorithm result = null;
            
        switch (codec)
        {
            case Codecs.SHA1:
                result = SHA1.Create();
                break;
            case Codecs.SHA256:
                result = SHA256.Create();
                break;
            case Codecs.SHA384:
                result = SHA384.Create();
                break;
            case Codecs.SHA512:
                result = SHA512.Create();
                break;
            case Codecs.MD5:
                result = MD5.Create();
                break;
            // case Codecs.RIPEMD160:
            //     result = new RIPEMD160Managed();
            //     break;
        }

        return result;
    }
}
