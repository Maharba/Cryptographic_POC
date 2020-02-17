using System;
using System.Security.Cryptography;

namespace Cryptography_POC.Services
{
    public class RijndaelCryptoConfiguration : ICryptoConfiguration
    {
        public RijndaelCryptoConfiguration(string password, int keySize, int iterations, int blockSize, int saltLength)
        {
            using (var aes = new AesManaged())
            {
                aes.GenerateIV();
                IV = aes.IV;
                
                Password = password;
                KeySize = keySize;
                Iterations = iterations;
                BlockSize = blockSize;
                SaltLength = saltLength;
                
                Key = GenerateEncryptionKey(Password, KeySize, Iterations);
            }
        }
        
        public string Password { get; private set; }
        public int BlockSize { get; private set; }
        public int KeySize { get; private set; }
        public int SaltLength { get; private set; }
        public int Iterations { get; private set; }
        public byte[] IV { get; private set; }
        public byte[] Key { get; private set; }

        private byte[] GetSalt()
        {
            return GetSalt(SaltLength);
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
        
        private byte[] GenerateEncryptionKey(string password, int keySize, int iterations = 1000)
        {
            if (string.IsNullOrEmpty(password))
            {
                throw new ArgumentException("Password must not be null or empty.");
            }
            if (keySize != 256)
            {
                throw new ArgumentException("Key size must not be different than 256 bits.");
            }
            if (iterations < 10000)
            {
                throw new ArgumentException("Iterations must be greater than 10000.");
            }

            byte[] salt = GetSalt();
            using (RNGCryptoServiceProvider rngCsp = new RNGCryptoServiceProvider())
            {
                // Fill the array with a random value.
                rngCsp.GetBytes(salt);
            }
            var keyGen = new Rfc2898DeriveBytes(password, salt, iterations);
            return keyGen.GetBytes(keySize / 8);
        }

        private void GenerateIV(int ivLength)
        {
            using (var ivGen = new RNGCryptoServiceProvider())
            {
                ivGen.GetBytes(IV);
            }
        }
    }
}