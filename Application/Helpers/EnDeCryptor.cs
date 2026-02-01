using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Application.Helpers
{
    public static class EnDeCryptor
    {
        public static string DecryptStringAES(string cipherText)
        {
            var encrypted = Convert.FromBase64String(cipherText);
            var decriptedFromJavascript = DecryptStringFromBytes(encrypted, GetKey(), GetIv());
            return string.Format(decriptedFromJavascript);
        }
        public static byte[] EncryptStringAES(string cipherText)
        {
            var encriptedBytes = EncryptStringToBytes(cipherText, GetKey(), GetIv());
            return encriptedBytes;
        }
        private static byte[] GetKey()
        {
            return Encoding.UTF8.GetBytes("bde86dcd965a4b93");
        }
        private static byte[] GetIv()
        {
            return Encoding.UTF8.GetBytes("8b890dbb98743cfd");
        }

        private static string DecryptStringFromBytes(byte[] cipherText, byte[] key, byte[] iv)
        {
            // Check arguments.
            if (cipherText == null || cipherText.Length <= 0)
            {
                throw new ArgumentNullException(nameof(cipherText));
            }
            if (key == null || key.Length <= 0)
            {
                throw new ArgumentNullException(nameof(key));
            }
            if (iv == null || iv.Length <= 0)
            {
                throw new ArgumentNullException(nameof(key));
            }

            // Declare the string used to hold
            // the decrypted text.
            string plaintext;

            // Create an RijndaelManaged object
            // with the specified key and IV.
            var rijAlg = Aes.Create();
            // Create a decrytor to perform the stream transform.
            var decryptor = rijAlg.CreateDecryptor(key, iv);
            try
            {
                // Create the streams used for decryption.
                using var msDecrypt = new MemoryStream(cipherText);
                using var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read);
                using var srDecrypt = new StreamReader(csDecrypt);
                // Read the decrypted bytes from the decrypting stream and place them in a string.
                plaintext = srDecrypt.ReadToEnd();
            }
            catch
            {
                plaintext = "keyError";
            }

            return plaintext;
        }

        private static byte[] EncryptStringToBytes(string plainText, byte[] key, byte[] iv)
        {
            // Check arguments.
            if (plainText == null || plainText.Length <= 0)
            {
                throw new ArgumentNullException(nameof(plainText));
            }
            if (key == null || key.Length <= 0)
            {
                throw new ArgumentNullException(nameof(key));
            }
            if (iv == null || iv.Length <= 0)
            {
                throw new ArgumentNullException(nameof(key));
            }
            // Create a RijndaelManaged object
            // with the specified key and IV.
            var rijAlg = Aes.Create();

            // Create a decrytor to perform the stream transform.
            var encryptor = rijAlg.CreateEncryptor(key, iv);

            // Create the streams used for encryption.
            using var msEncrypt = new MemoryStream();
            using var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write);
            using (var swEncrypt = new StreamWriter(csEncrypt))
            {
                //Write all data to the stream.
                swEncrypt.Write(plainText);
            }
            byte[] encrypted = msEncrypt.ToArray();

            // Return the encrypted bytes from the memory stream.
            return encrypted;
        }

        public static String sha256_hash(string value)
        {
            StringBuilder Sb = new StringBuilder();

            using (var hash = SHA256.Create())
            {
                Encoding enc = Encoding.UTF8;
                byte[] result = hash.ComputeHash(enc.GetBytes(value));

                foreach (byte b in result)
                    Sb.Append(b.ToString("x2"));
            }

            return Sb.ToString();
        }

        public static string sha512hash(string inputs)
        {
            using (SHA512 sha512 = SHA512.Create())
            {
                byte[] bytes = Encoding.UTF8.GetBytes(inputs);
                byte[] hash = sha512.ComputeHash(bytes);

                // Convert the hash bytes to a hexadecimal string
                StringBuilder result = new StringBuilder();
                foreach (byte b in hash)
                {
                    result.Append(b.ToString("x2")); // Use lowercase hexadecimal for consistency
                }
                return result.ToString();
            }
        }
    }
}
