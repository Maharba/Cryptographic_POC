namespace Cryptography_POC.Services
{
    public interface ICryptoFileService
    {
        void EncryptFile(string filePath, string password, int keySize, int blockSize, int saltLength,
            int iterations);

        void DecryptFile(string filePath);
    }
}