namespace Cryptography_POC.Services
{
    public class AesCryptoConfiguration
    {
        public BitSizes BlockSize { get; set; }
        public BitSizes KeySize { get; set; }
        public SymmetricAlgorithms Algorithm { get; set; }
        public BitSizes SaltLength { get; set; }
        public int Iterations { get; set; }
    }
}