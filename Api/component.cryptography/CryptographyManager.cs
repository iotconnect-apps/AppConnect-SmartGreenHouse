using component.exception;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System;
using System.IO;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace component.cryptography
{
    public sealed class CryptographyManager
    {
        private static CryptographyManager instance = null;
        private readonly string _invalidDataMessage = "Invalid Data";
        private CryptographyManager()
        {
        }

        public static CryptographyManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new CryptographyManager();
                }
                return instance;
            }
        }

        private static string _dynamicEncPwd = "softweb.dynamic.encryption.pwd";
        private static byte[] _dynamicEncSalt = new byte[] { 0x45, 0xF1, 0x61, 0x6e, 0x20, 0x00, 0x65, 0x64, 0x76, 0x65, 0x64, 0x03, 0x76 };

        public async Task<string> EncryptAsync(string plainText)
        {
            if (string.IsNullOrEmpty(plainText))
            {
                return plainText;
            }

            byte[] encryptdata;
            try
            {
                byte[] clearBytes = System.Text.Encoding.Unicode.GetBytes(plainText);
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(_dynamicEncPwd, _dynamicEncSalt);
                encryptdata = _encrypt(clearBytes, pdb.GetBytes(32), pdb.GetBytes(16));
            }
            catch
            {
                throw new NotFoundCustomException(_invalidDataMessage);
            }
            return await Task.FromResult(Convert.ToBase64String(encryptdata));
        }

        public async Task<string> DecryptAsync(string hashText)
        {
            if (string.IsNullOrEmpty(hashText))
            {
                return hashText;

            }


            byte[] decryptedData;
            try
            {
                byte[] cipherBytes = Convert.FromBase64String(hashText);
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(_dynamicEncPwd, _dynamicEncSalt);
                decryptedData = _decrypt(cipherBytes, pdb.GetBytes(32), pdb.GetBytes(16));
            }
            catch
            {
                throw new BadRequestCustomException(_invalidDataMessage);
            }
            return await Task.FromResult(System.Text.Encoding.Unicode.GetString(decryptedData));
        }

        public async Task<string> CreateHashAsync(string plainText)
        {
            if (string.IsNullOrEmpty(plainText))
            {
                throw new BadRequestCustomException(_invalidDataMessage);
            }

            return await Task.FromResult(_getHashValue(plainText));
        }

        public async Task<bool> CompareHashAsync(string plainText, string hashText)
        {
            if (string.IsNullOrEmpty(plainText))
            {
                throw new BadRequestCustomException(_invalidDataMessage);
            }

            if (string.IsNullOrEmpty(hashText))
            {
                throw new BadRequestCustomException(_invalidDataMessage);
            }

            var hashResponse = _getHashValue(plainText);

            if (!string.IsNullOrEmpty(hashResponse) && hashResponse.Equals(hashText))
            {
                return await Task.FromResult(true);
            }

            return await Task.FromResult(false);
        }

        private byte[] _encrypt(byte[] clearData, byte[] key, byte[] initVector)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                Rijndael alg = Rijndael.Create();
                alg.Key = key;
                alg.IV = initVector;
                var cs = new CryptoStream(ms, alg.CreateEncryptor(), CryptoStreamMode.Write);
                cs.Write(clearData, 0, clearData.Length);
                cs.FlushFinalBlock();
                return ms.ToArray();
            }
        }

        private byte[] _decrypt(byte[] cipherdata, byte[] key, byte[] initVector)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                Rijndael alg = Rijndael.Create();
                alg.Key = key;
                alg.IV = initVector;
                var cs = new CryptoStream(ms, alg.CreateDecryptor(), CryptoStreamMode.Write);
                cs.Write(cipherdata, 0, cipherdata.Length);
                cs.FlushFinalBlock();
                return ms.ToArray();
            }
        }

        private string _getHashValue(string val)
        {
            using (SHA256.Create())
            {
                return Convert.ToBase64String(KeyDerivation.Pbkdf2(
                        password: val,
                        salt: _dynamicEncSalt,
                        prf: KeyDerivationPrf.HMACSHA256,
                        iterationCount: 10000,
                        numBytesRequested: 256 / 8));
            }
        }
    }
}
