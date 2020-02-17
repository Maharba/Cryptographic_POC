using System;
using System.IO;
using System.Security.Cryptography;
using LazyCache;
using Xamarin.Essentials;

namespace Cryptography_POC.Services
{
    public class CryptoFileService : ICryptoFileService
    {
        private ICryptoConfiguration _cryptoConfiguration;
        private ICryptoBuilder _cryptoBuilder;

        public CryptoFileService()
        {
        }

        private ICryptoConfiguration CreateCryptoConfiguration(string password, int blockSize, int iterations, int saltLength, int keySize)
        {
            return new RijndaelCryptoConfiguration(password, keySize, iterations, blockSize, saltLength);
        }
        
        public void EncryptFile(
            string filePath,
            string password,
            int keySize,
            int blockSize,
            int saltLength,
            int iterations)
        {
            if (File.Exists(filePath))
            {
                _cryptoConfiguration = CreateCryptoConfiguration(password, blockSize, iterations, saltLength, keySize);
                _cryptoBuilder = new CryptoBuilder(new RijndaelCryptoPlatform());
                
                var result = _cryptoBuilder.SetCryptoConfiguration(_cryptoConfiguration)
                                                  .SetCryptoResponse(CryptoResponse)
                                                  .Build()
                                                  .Encrypt(File.ReadAllBytes(filePath));
                File.WriteAllBytes(filePath, result);
            }
        }

        private void CryptoResponse(CryptoStatus obj)
        {
            Console.WriteLine($"Status: {obj.Response}");
        }

        public void DecryptFile(string filePath)
        {
            if (File.Exists(filePath))
            {
                var result = _cryptoBuilder.SetCryptoConfiguration(_cryptoConfiguration)
                                                  .SetCryptoResponse(CryptoResponse)
                                                  .Build()
                                                  .Decrypt(File.ReadAllBytes(filePath));
                
                File.WriteAllBytes(filePath, result);
            }
        }
    }
}