using System.Security.Cryptography;
using System.Text;

namespace evasafe.API.utils
{
    public class AppConstants
    {
        public const string AppName = "EvaSafe";
        public static HashAlgorithmName KEY_ALGORITHM = HashAlgorithmName.SHA256;
        public static int NUMBER_OF_ITERATIONS = 100000;
        public static int KEY_LENGTH = 256;
        public static byte[] SALT_ARRAY = Encoding.UTF8.GetBytes("//NaCl^;;£7%%=jj");
    }
}
