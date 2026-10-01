using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;

namespace Application.Extensions
{
    public static class StringExtension
    {
        public static string GetFirst(this string source, int length)
        {
            return length >= source.Length ? source.ToUpper() : source.Substring(0, length).ToUpper();
        }
        public static string GetLast(this string source, int length)
        {
            return length >= source.Length ? source.ToUpper() : source.Substring(source.Length - length).ToUpper();
        }
        public static string RemoveSpace(this string source)
        {
            return source.Contains(" ") ? source.Replace(" ", "") : source;
        }
        public static string GetUptoFirstSpace(this string source)
        {
            return source.Contains(" ") ? source.Split(" ")[0].ToUpper() : source.ToUpper();
        }
        public static string GetSubstringByString(this string source, string a, string b)
        {
            return source.Substring(source.IndexOf(a, StringComparison.Ordinal) + a.Length, source.IndexOf(b, StringComparison.Ordinal) - source.IndexOf(a, StringComparison.Ordinal) - a.Length);
        }
        public static string GetValidFileName(this string source)
        {
            var regexSearch = new string(Path.GetInvalidFileNameChars());
            var regex = new Regex($"[{Regex.Escape(regexSearch)}]");
            return regex.Replace(source, "");
        }
        public static string Encrypt(this string text, string keyString)
        {
            var key = Encoding.UTF8.GetBytes(keyString);

            using var aesAlg = Aes.Create();
            using var encryptor = aesAlg.CreateEncryptor(key, aesAlg.IV);
            using var msEncrypt = new MemoryStream();
            using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
            using (var swEncrypt = new StreamWriter(csEncrypt))
            {
                swEncrypt.Write(text);
            }

            var iv = aesAlg.IV;

            var decryptedContent = msEncrypt.ToArray();

            var result = new byte[iv.Length + decryptedContent.Length];

            Buffer.BlockCopy(iv, 0, result, 0, iv.Length);
            Buffer.BlockCopy(decryptedContent, 0, result, iv.Length, decryptedContent.Length);

            return Convert.ToBase64String(result);
        }
        public static string Decrypt(this string cipherText, string keyString)
        {
            var fullCipher = Convert.FromBase64String(cipherText);

            var iv = new byte[16];
            var cipher = new byte[16];

            Buffer.BlockCopy(fullCipher, 0, iv, 0, iv.Length);
            Buffer.BlockCopy(fullCipher, iv.Length, cipher, 0, iv.Length);
            var key = Encoding.UTF8.GetBytes(keyString);

            using var aesAlg = Aes.Create();
            using var decryptor = aesAlg.CreateDecryptor(key, iv);
            using var msDecrypt = new MemoryStream(cipher);
            using var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read);
            using var srDecrypt = new StreamReader(csDecrypt);
            var result = srDecrypt.ReadToEnd();
            return result;
        }
        public static string FormatXml(XmlNode xmlNode)
        {
            var stringBuilder = new StringBuilder();
            using var stringWriter = new StringWriter(stringBuilder);
            using var xmlTextWriter = new XmlTextWriter(stringWriter) { Formatting = Formatting.Indented };
            xmlNode.WriteTo(xmlTextWriter);
            return stringBuilder.ToString();
        }
    }
}
