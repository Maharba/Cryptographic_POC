using System;
using System.IO;
using System.Security.Cryptography;
using LazyCache;
using Xamarin.Essentials;

namespace Cryptography_POC.Services
{
    /// <summary>
    /// Essentially, Aes is a subset of Rijndael with the only difference of having the block size set to 128 bits. Rijndael can be set to any multiples of 32 bits.
    /// </summary>
    public enum SymmetricAlgorithms
    {
        Aes,
        Rijndael
    }

    public enum BitSizes
    {
        Bit128 = 128,
        Bit160 = 160,
        Bit192 = 192,
        Bit224 = 224,
        Bit256 = 256
    }

    public enum CryptographyAction
    {
        Encrypt = 0,
        Decrypt = 1
    }

    public class CryptoFileService : ICryptoFileService
    {
        private const int SALT_LENGTH_LIMIT = 32;

        private BitSizes _keySize;
        private IAppCache _cache;
        private BitSizes _blockSize;

        public CryptoFileService()
        {
            _cache = new CachingService();
        }

        public SymmetricAlgorithms SymmetricAlgorithmType { get; set; } = SymmetricAlgorithms.Aes;
        
        public void EncryptFile(
            string filePath,
            string password,
            BitSizes keySize,
            BitSizes blockSize,
            int saltLength,
            int iterations)
        {
            if (SymmetricAlgorithmType == SymmetricAlgorithms.Aes && blockSize != BitSizes.Bit128)
            {
                throw new NotSupportedException("Block size is not supported for AES");
            }
            try
            {
                if (File.Exists(filePath))
                {
                    _blockSize = blockSize;
                    var key = GenerateEncryptionKey(password, keySize, iterations);
                    _cache.Add<byte[]>("Key", key);
                    var encrypted = Encrypt(
                        File.ReadAllBytes(filePath), 
                        SymmetricAlgorithmType, 
                        blockSize,
                        key);
                    File.WriteAllBytes(filePath, encrypted);
                }
                else
                {
                    Console.WriteLine("No file found."); 
                    throw new FileNotFoundException();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERROR:"+ ex.Message);
            }
        }

        public void DecryptFile(string filePath, string password)
        {
            try
            {
                if (File.Exists(filePath))
                {
                    var key = _cache.Get<byte[]>("Key");
                    
                    var decrypted = Decrypt(
                        File.ReadAllBytes(filePath), 
                        SymmetricAlgorithmType, 
                        _blockSize,
                        key);
                    File.WriteAllBytes(filePath, decrypted);
                }
                else
                {
                    Console.WriteLine("No file found."); 
                    throw new FileNotFoundException();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERROR:"+ ex.Message);
            }
        }

        public byte[] Decrypt(
            byte[] data, 
            SymmetricAlgorithms algorithm, 
            BitSizes blockSize,
            byte[] key)
        {
            return PerformEncryption(data, algorithm, blockSize, key, CryptographyAction.Decrypt);
        }
        
        public byte[] Encrypt(
            byte[] data, 
            SymmetricAlgorithms algorithm, 
            BitSizes blockSize,
            byte[] key)
        {
            return PerformEncryption(data, algorithm, blockSize, key, CryptographyAction.Encrypt);
        }

        private byte[] PerformEncryption(byte[] data, SymmetricAlgorithms algorithm, BitSizes blockSize, byte[] key, 
            CryptographyAction action)
        {
            SymmetricAlgorithm aes;
            
            byte[] cryptoData = new byte[32];
            switch (algorithm)
            {
                case SymmetricAlgorithms.Aes:
                    using (aes = new AesManaged())
                    {
                        aes.BlockSize = Convert.ToInt32(blockSize);
                        aes.Padding = PaddingMode.PKCS7;

                        aes.Key = key;

                        switch (action)
                        {
                            case CryptographyAction.Encrypt:
                                aes.GenerateIV();
                                _cache.Add<byte[]>("IV", aes.IV);
                                using (var encryptor = aes.CreateEncryptor(aes.Key, aes.IV))
                                {
                                    cryptoData = PerformCryptography(data, encryptor);
                                }
                                break;
                            case CryptographyAction.Decrypt:
                                aes.IV = _cache.Get<byte[]>("IV");
                                using (var decryptor = aes.CreateDecryptor(aes.Key, aes.IV))
                                {
                                    cryptoData = PerformCryptography(data, decryptor);
                                }
                                break;
                        }
                    }
                    break;
                case SymmetricAlgorithms.Rijndael:
                    using (aes = new RijndaelManaged())
                    {
                        aes.BlockSize = Convert.ToInt32(blockSize);
                        aes.Padding = PaddingMode.PKCS7;

                        aes.Key = key;
                        switch (action)
                        {
                            case CryptographyAction.Encrypt:
                                aes.GenerateIV();
                                _cache.Add<byte[]>("IV", aes.IV);
                                using (var encryptor = aes.CreateEncryptor(aes.Key, aes.IV))
                                {
                                    cryptoData = PerformCryptography(data, encryptor);
                                }
                                break;
                            case CryptographyAction.Decrypt:
                                aes.IV = _cache.Get<byte[]>("IV");
                                using (var decryptor = aes.CreateDecryptor(aes.Key, aes.IV))
                                {
                                    cryptoData = PerformCryptography(data, decryptor);
                                }
                                break;
                        }
                    }
                    break;
            }
            return cryptoData;
        }

        private byte[] GetSalt()
        {
            return GetSalt(SALT_LENGTH_LIMIT);
        }
        private byte[] GetSalt(int maximumSaltLength)
        {
            var salt = new byte[maximumSaltLength];
            using (var random = new RNGCryptoServiceProvider())
            {
                random.GetNonZeroBytes(salt);
            }
            return salt;
        }

        public byte[] GenerateEncryptionKey(string password, BitSizes keySize, int iterations = 1000)
        {
            byte[] salt = GetSalt();
            using (RNGCryptoServiceProvider rngCsp = new 
                RNGCryptoServiceProvider())
            {
                // Fill the array with a random value.
                rngCsp.GetBytes(salt);
            }
            var keyGen = new Rfc2898DeriveBytes(password, salt, iterations);
            int keyLength = 0;
            switch (keySize)
            {
                case BitSizes.Bit128:
                    keyLength = 128;
                    break;
                case BitSizes.Bit160:
                    keyLength = 160;
                    break;
                case BitSizes.Bit192:
                    keyLength = 192;
                    break;
                case BitSizes.Bit224:
                    keyLength = 224;
                    break;
                case BitSizes.Bit256:
                    keyLength = 256;
                    break;
            }
            return keyGen.GetBytes(keyLength / 8);
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