namespace Cryptography_POC.Services
{
    public interface ICryptoPlatform
    {
        void Encrypt(ICryptoConfiguration configuration);
        void Decrypt(ICryptoConfiguration configuration);
    }
}