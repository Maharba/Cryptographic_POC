namespace Cryptography_POC.Services
{
    public interface ICryptoFileService
    {
        SymmetricAlgorithms SymmetricAlgorithmType { get; set; }
        
        void EncryptFile(string filePath, string password, BitSizes keySize, BitSizes blockSize, int saltLength,
            int iterations);

        void DecryptFile(string filePath, string password);
    }
}