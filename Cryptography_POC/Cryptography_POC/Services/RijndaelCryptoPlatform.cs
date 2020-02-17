using System;
using System.IO;
using System.Security.Cryptography;

namespace Cryptography_POC.Services
{
    public class RijndaelCryptoPlatform : ICryptoPlatform
    {
        private ICryptoConfiguration _configuration;

        public RijndaelCryptoPlatform()
        {
        }
        
        public byte[] Encrypt(byte[] data, ICryptoConfiguration configuration)
        {
            try
            {
                byte[] cryptoData;
            
                _configuration = configuration;
                using (var aes = new RijndaelManaged())
                {
                    aes.BlockSize = _configuration.BlockSize;
                    aes.Padding = PaddingMode.PKCS7;

                    aes.Key = _configuration.Key;
                    aes.IV = _configuration.IV;

                    using (var encryptor = aes.CreateEncryptor(aes.Key, aes.IV))
                    {
                        cryptoData = PerformCryptography(data, encryptor);
                    }
                }

                CryptoPerformer.Instance.CryptoResponse?.Invoke(new CryptoStatus() { Response = ResponseType.Success});
                return cryptoData;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                CryptoPerformer.Instance.CryptoResponse?.Invoke(new CryptoStatus() { Response = ResponseType.InvalidPassword});
                return null;
            }
        }

        public byte[] Decrypt(byte[] data, ICryptoConfiguration configuration)
        {
            try
            {
                byte[] rawData;
                
                _configuration = configuration;
                using (var aes = new RijndaelManaged())
                {
                    aes.BlockSize = _configuration.BlockSize;
                    aes.Padding = PaddingMode.PKCS7;
                    aes.Key = _configuration.Key;
                    aes.IV = _configuration.IV;

                    using (var decryptor = aes.CreateDecryptor(aes.Key, aes.IV))
                    {
                        rawData = PerformCryptography(data, decryptor);
                    }
                    CryptoPerformer.Instance.CryptoResponse?.Invoke(new CryptoStatus() { Response = ResponseType.Success});
                    return rawData;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                CryptoPerformer.Instance.CryptoResponse?.Invoke(new CryptoStatus() { Response = ResponseType.InvalidPassword});
                return null;
            }
        }

        private byte[] PerformCryptography(byte[] data, ICryptoTransform cryptoTransform)
        {
            using (var ms = new MemoryStream())
            using (var cryptoStream = new CryptoStream(ms, cryptoTransform, CryptoStreamMode.Write))
            {
                cryptoStream.Write(data, 0, data.Length);
                cryptoStream.FlushFinalBlock();

                return ms.ToArray();
            }
        }
    }
}