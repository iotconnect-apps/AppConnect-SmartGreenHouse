using iot.solution.dataacess.Properties;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Cryptography;
using System.Text;

namespace iot.solution.data.Utilities
{
    public static class Encryption
    {
        public static string EncryptionKey = "41a3f131dd91";
        public static int EncryptionKeySize = 24;
        #region Two-way Encryption
        public static byte[] EncryptBytes(byte[] inputBytes, string encryptionKey)
        {
            if (encryptionKey == null)
                encryptionKey = Encryption.EncryptionKey;

            return EncryptBytes(inputBytes, Encoding.UTF8.GetBytes(encryptionKey));
        }
        public static byte[] EncryptBytes(byte[] inputBytes, SecureString encryptionKey)
        {
            if (encryptionKey == null)
                throw new ArgumentException(Resources.MissingEncryptionKey);

            return EncryptBytes(inputBytes, Encoding.UTF8.GetBytes(encryptionKey.GetString()));
        }
        public static byte[] EncryptBytes(byte[] inputBytes, byte[] encryptionKey, CipherMode cipherMode = CipherMode.ECB)
        {
            var des = TripleDES.Create(); //new TripleDESCryptoServiceProvider();
            des.Mode = cipherMode;


            if (EncryptionKeySize == 16)
            {
                MD5CryptoServiceProvider hash = new MD5CryptoServiceProvider();
                des.Key = hash.ComputeHash(encryptionKey);
            }
            else
            {
                SHA256CryptoServiceProvider hash = new SHA256CryptoServiceProvider();
                des.Key = hash.ComputeHash(encryptionKey)
                              .Take(EncryptionKeySize)
                              .ToArray();
            }

            ICryptoTransform Transform = des.CreateEncryptor();

            byte[] Buffer = inputBytes;
            return Transform.TransformFinalBlock(Buffer, 0, Buffer.Length);
        }
        public static byte[] EncryptBytes(string inputString, string encryptionKey)
        {
            return EncryptBytes(Encoding.UTF8.GetBytes(inputString), encryptionKey);
        }
        public static string EncryptString(string inputString, byte[] encryptionKey, bool useBinHex = false)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(inputString);

            if (useBinHex)
                return BinaryToBinHex(EncryptBytes(bytes, encryptionKey));

            return Convert.ToBase64String(EncryptBytes(bytes, encryptionKey));
        }
        public static string EncryptString(string inputString, string encryptionKey, bool useBinHex = false)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(inputString);

            if (useBinHex)
                return BinaryToBinHex(EncryptBytes(bytes, encryptionKey));

            return Convert.ToBase64String(EncryptBytes(bytes, encryptionKey));
        }
        public static string EncryptString(string inputString, SecureString encryptionKey, bool useBinHex = false)
        {
            if (encryptionKey == null)
                throw new ArgumentException(Resources.MissingEncryptionKey);

            byte[] bytes = Encoding.UTF8.GetBytes(inputString);

            if (useBinHex)
                return BinaryToBinHex(EncryptBytes(bytes, encryptionKey.GetString()));

            return Convert.ToBase64String(EncryptBytes(bytes, encryptionKey.GetString()));
        }
        public static byte[] DecryptBytes(byte[] decryptBuffer, string encryptionKey)
        {
            if (decryptBuffer == null || decryptBuffer.Length == 0)
                return null;

            if (encryptionKey == null)
                encryptionKey = Encryption.EncryptionKey;

            return DecryptBytes(decryptBuffer, Encoding.UTF8.GetBytes(encryptionKey));
        }
        public static byte[] DecryptBytes(byte[] decryptBuffer, SecureString encryptionKey)
        {
            if (decryptBuffer == null || decryptBuffer.Length == 0)
                return null;

            return DecryptBytes(decryptBuffer, Encoding.UTF8.GetBytes(encryptionKey.GetString()));
        }
        public static byte[] DecryptBytes(byte[] decryptBuffer, byte[] encryptionKey, CipherMode cipherMode = CipherMode.ECB)
        {
            if (decryptBuffer == null || decryptBuffer.Length == 0)
                return null;

            var des = TripleDES.Create();
            des.Mode = cipherMode;

            if (EncryptionKeySize == 16)
            {
                MD5CryptoServiceProvider hash = new MD5CryptoServiceProvider();
                des.Key = hash.ComputeHash(encryptionKey);
            }
            else
            {
                SHA256CryptoServiceProvider hash = new SHA256CryptoServiceProvider();
                des.Key = hash.ComputeHash(encryptionKey)
                    .Take(EncryptionKeySize)
                    .ToArray();
            }

            ICryptoTransform Transform = des.CreateDecryptor();

            return Transform.TransformFinalBlock(decryptBuffer, 0, decryptBuffer.Length);
        }
        public static byte[] DecryptBytes(string decryptString, string encryptionKey, bool useBinHex = false)
        {
            if (useBinHex)
                return DecryptBytes(BinHexToBinary(decryptString), encryptionKey);

            return DecryptBytes(Convert.FromBase64String(decryptString), encryptionKey);
        }
        public static byte[] DecryptBytes(string decryptString, SecureString encryptionKey, bool useBinHex = false)
        {
            if (encryptionKey == null)
                throw new ArgumentException(Resources.MissingEncryptionKey);

            if (useBinHex)
                return DecryptBytes(BinHexToBinary(decryptString), encryptionKey.GetString());

            return DecryptBytes(Convert.FromBase64String(decryptString), encryptionKey.GetString());
        }
        public static string DecryptString(string decryptString, string encryptionKey, bool useBinHex = false)
        {
            try
            {
                var data = useBinHex ? BinHexToBinary(decryptString) : Convert.FromBase64String(decryptString);

                byte[] decrypted = DecryptBytes(data, encryptionKey);
                return Encoding.UTF8.GetString(decrypted);
            }
            catch
            {
                // undecryptable data
                return string.Empty;
            }
        }
        public static string DecryptString(string decryptString, SecureString encryptionKey, bool useBinHex = false)
        {
            if (encryptionKey == null)
                throw new ArgumentException(Resources.MissingEncryptionKey);

            var data = useBinHex ? BinHexToBinary(decryptString) : Convert.FromBase64String(decryptString);

            try
            {
                byte[] decrypted = DecryptBytes(data, encryptionKey.GetString());
                return Encoding.UTF8.GetString(decrypted);
            }
            catch
            {
                return string.Empty;
            }
        }
        public static string DecryptString(string decryptString, byte[] encryptionKey, bool useBinHex = false)
        {
            var data = useBinHex ? BinHexToBinary(decryptString) : Convert.FromBase64String(decryptString);

            try
            {
                byte[] decrypted = DecryptBytes(data, encryptionKey);
                return Encoding.UTF8.GetString(decrypted);
            }
            catch
            {
                return string.Empty;
            }
        }
        #endregion


        #region Hashes
        public static string ComputeHash(string plainText,
                                         string hashAlgorithm,
                                         byte[] saltBytes,
                                         bool useBinHex = false)
        {
            if (string.IsNullOrEmpty(plainText))
                return plainText;

            return ComputeHash(Encoding.UTF8.GetBytes(plainText), hashAlgorithm, saltBytes, useBinHex);
        }

        public static string ComputeHash(string plainText,
                                         string hashAlgorithm,
                                         string salt,
                                         bool useBinHex = false)
        {
            if (string.IsNullOrEmpty(plainText))
                return plainText;

            return ComputeHash(Encoding.UTF8.GetBytes(plainText), hashAlgorithm, Encoding.UTF8.GetBytes(salt), useBinHex);
        }

        public static string ComputeHash(byte[] byteData,
                                         string hashAlgorithm,
                                         byte[] saltBytes,
                                         bool useBinHex = false)
        {
            if (byteData == null)
                return null;

            // Convert plain text into a byte array.            
            byte[] plainTextWithSaltBytes;

            if (saltBytes != null && !hashAlgorithm.StartsWith("HMAC"))
            {
                // Allocate array, which will hold plain text and salt.
                plainTextWithSaltBytes =
                    new byte[byteData.Length + saltBytes.Length];

                // Copy plain text bytes into resulting array.
                for (int i = 0; i < byteData.Length; i++)
                    plainTextWithSaltBytes[i] = byteData[i];

                // Append salt bytes to the resulting array.
                for (int i = 0; i < saltBytes.Length; i++)
                    plainTextWithSaltBytes[byteData.Length + i] = saltBytes[i];
            }
            else
                plainTextWithSaltBytes = byteData;

            HashAlgorithm hash;

            // Make sure hashing algorithm name is specified.
            if (hashAlgorithm == null)
                hashAlgorithm = "";

            // Initialize appropriate hashing algorithm class.
            switch (hashAlgorithm.ToUpper())
            {
                case "SHA1":
                    hash = new SHA1Managed();
                    break;
                case "SHA256":
                    hash = new SHA256Managed();
                    break;
                case "SHA384":
                    hash = new SHA384Managed();
                    break;
                case "SHA512":
                    hash = new SHA512Managed();
                    break;
                case "HMACMD5":
                    hash = new HMACMD5(saltBytes);
                    break;
                case "HMACSHA1":
                    hash = new HMACSHA1(saltBytes);
                    break;
                case "HMACSHA256":
                    hash = new HMACSHA256(saltBytes);
                    break;
                case "HMACSHA512":
                    hash = new HMACSHA512(saltBytes);
                    break;
                default:
                    // default to MD5
                    hash = new MD5CryptoServiceProvider();
                    break;
            }

            byte[] hashBytes = hash.ComputeHash(plainTextWithSaltBytes);


            hash.Dispose();

            if (useBinHex)
                return BinaryToBinHex(hashBytes);

            return Convert.ToBase64String(hashBytes);
        }
        #endregion

        #region Gzip
        public static byte[] GZipMemory(byte[] buffer)
        {
            MemoryStream ms = new MemoryStream();

            GZipStream gZip = new GZipStream(ms, CompressionMode.Compress);

            gZip.Write(buffer, 0, buffer.Length);
            gZip.Close();

            byte[] result = ms.ToArray();
            ms.Close();

            return result;
        }
        public static byte[] GZipMemory(string Input)
        {
            return GZipMemory(Encoding.UTF8.GetBytes(Input));
        }
        public static byte[] GZipMemory(string Filename, bool IsFile)
        {
            string InputFile = Filename;
            byte[] Buffer = File.ReadAllBytes(Filename);
            return GZipMemory(Buffer);
        }
        public static bool GZipFile(string Filename, string OutputFile)
        {
            string InputFile = Filename;
            byte[] Buffer = File.ReadAllBytes(Filename);
            FileStream fs = new FileStream(OutputFile, FileMode.OpenOrCreate, FileAccess.Write);
            GZipStream GZip = new GZipStream(fs, CompressionMode.Compress);
            GZip.Write(Buffer, 0, Buffer.Length);
            GZip.Close();
            fs.Close();

            return true;
        }
        #endregion

        #region CheckSum
        public static string GetChecksumFromFile(string file, string mode = "SHA256")
        {
            if (!File.Exists(file))
                return null;

            try
            {
                using (FileStream stream = File.OpenRead(file))
                {
                    if (mode == "SHA256")
                    {
                        var sha = new SHA256Managed();
                        byte[] checksum = sha.ComputeHash(stream);
                        return BinaryToBinHex(checksum);
                    }
                    if (mode == "SHA512")
                    {
                        var sha = new SHA512Managed();
                        byte[] checksum = sha.ComputeHash(stream);
                        return BinaryToBinHex(checksum);
                    }
                    if (mode == "MD5")
                    {
                        var md = new MD5CryptoServiceProvider();
                        byte[] checkSum = md.ComputeHash(stream);

                        return BinaryToBinHex(checkSum);
                    }
                }
            }
            catch
            {
                return null;
            }

            return null;
        }
        public static string GetChecksumFromBytes(byte[] fileData, string mode = "SHA256")
        {
            using (MemoryStream stream = new MemoryStream(fileData))
            {
                if (mode == "SHA256")
                {
                    var sha = new SHA256Managed();
                    byte[] checksum = sha.ComputeHash(stream);
                    return BinaryToBinHex(checksum);
                }
                if (mode == "SHA512")
                {
                    var sha = new SHA512Managed();
                    byte[] checksum = sha.ComputeHash(stream);
                    return BinaryToBinHex(checksum);
                }
                if (mode == "MD5")
                {
                    var md = new MD5CryptoServiceProvider();
                    byte[] checkSum = md.ComputeHash(stream);

                    return BinaryToBinHex(checkSum);
                }
            }

            return null;
        }

        #endregion

        #region BinHex Helpers
        public static string BinaryToBinHex(byte[] data)
        {
            if (data == null)
                return null;

            StringBuilder sb = new StringBuilder(data.Length * 2);
            foreach (byte val in data)
            {
                sb.AppendFormat("{0:x2}", val);
            }
            return sb.ToString().ToUpper();
        }
        public static byte[] BinHexToBinary(string hex)
        {
            int offset = hex.StartsWith("0x") ? 2 : 0;
            if ((hex.Length % 2) != 0)
                throw new ArgumentException("Invalid String Length");

            byte[] ret = new byte[(hex.Length - offset) / 2];

            for (int i = 0; i < ret.Length; i++)
            {
                ret[i] = (byte)((ParseHexChar(hex[offset]) << 4)
                                 | ParseHexChar(hex[offset + 1]));
                offset += 2;
            }
            return ret;
        }
        static int ParseHexChar(char c)
        {
            if (c >= '0' && c <= '9')
                return c - '0';
            if (c >= 'A' && c <= 'F')
                return c - 'A' + 10;
            if (c >= 'a' && c <= 'f')
                return c - 'a' + 10;

            throw new ArgumentException("Invalid character");
        }

        #endregion
    }

    internal static class SecureStringExtensions
    {
        public static string GetString(this SecureString source)
        {
            string result = null;
            int length = source.Length;
            IntPtr pointer = IntPtr.Zero;
            char[] chars = new char[length];

            try
            {
                pointer = Marshal.SecureStringToBSTR(source);
                Marshal.Copy(pointer, chars, 0, length);

                result = string.Join("", chars);
            }
            finally
            {
                if (pointer != IntPtr.Zero)
                {
                    Marshal.ZeroFreeBSTR(pointer);
                }
            }

            return result;
        }
    }

}
