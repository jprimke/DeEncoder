using System.Security.Cryptography;

namespace DeEncoder.ViewModels
{
    public class EncodeDecoderHashAlgorithmFactory
    {
        public static HashAlgorithm Create(Codecs codec)
        {
            HashAlgorithm result = null;

            switch (codec)
            {
                case Codecs.SHA1:
                    result = new SHA1Managed();
                    break;
                case Codecs.SHA256:
                    result = new SHA256Managed();
                    break;
                case Codecs.SHA384:
                    result = new SHA384Managed();
                    break;
                case Codecs.SHA512:
                    result = new SHA512Managed();
                    break;
                case Codecs.MD5:
                    result = new MD5Cng();
                    break;
                case Codecs.RIPEMD160:
                    result = new RIPEMD160Managed();
                    break;
            }

            return result;
        }
    }
}