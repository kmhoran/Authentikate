using System;
using System.Security.Cryptography;
using System.Text;

namespace Utils.Encrypt
{
    public static class Cryptic
    {
        // This encryption solution relies upon the Rijndael algorithm.
        // One benefit to this pattern: the same key is used to encrypt and decrypt.
        // For more infomation, see
        // https://docs.microsoft.com/en-us/dotnet/api/system.security.cryptography.rijndaelmanaged?view=netcore-3.0
        
        private static CipherMode cipherMode = CipherMode.CBC;
        private static PaddingMode paddingMode = PaddingMode.PKCS7;
        private static int keySize = 256;
        private static int blockSize = 128;
        private static int offset = 0;

        // For creating ininitialization vector (IV) --> BlockSize / 8
        private static byte[] GetIV(byte[] symetricKey){
            var smallerLen = symetricKey.Length;
            var iv = new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };
            if (smallerLen > iv.Length) smallerLen = iv.Length;

            Array.Copy(symetricKey, iv, smallerLen);
            return iv;
        }

        public static string Encrypt(string plainText, string key)
        {
            var rindy = new RijndaelManaged();
            rindy.Mode = cipherMode;
            rindy.Padding = paddingMode;
            rindy.KeySize = keySize;
            rindy.BlockSize = blockSize;
            
            var symetricKey = Encoding.UTF8.GetBytes(key);
            
            var iv = GetIV(symetricKey);
            rindy.Key = iv;
            rindy.IV = iv;
            
            var symetricAES = rindy.CreateEncryptor();
            var plainTextBytes = Encoding.UTF8.GetBytes(plainText);
            return Convert.ToBase64String(symetricAES.TransformFinalBlock(plainTextBytes, offset, plainTextBytes.Length));
        }

        public static string Decrypt(string encryptedText, string key)
        {
            var rindy = new RijndaelManaged();
            rindy.Mode = cipherMode;
            rindy.Padding = paddingMode;
            rindy.KeySize = keySize;
            rindy.BlockSize = blockSize;
            
            var symetricKey = Encoding.UTF8.GetBytes(key);

            var iv = GetIV(symetricKey);
            rindy.Key = iv;
            rindy.IV = iv;

            var symetricAES = rindy.CreateDecryptor();
            var cryptoBytes = Convert.FromBase64String(encryptedText);
            var plainTextBytes = symetricAES.TransformFinalBlock(cryptoBytes, offset, cryptoBytes.Length);
            return Encoding.UTF8.GetString(plainTextBytes);
        }
    }
}
