namespace Cryptography_POC.Services
{
    public interface ICryptoPlatform
    {
        byte[] Encrypt(byte[] data, ICryptoConfiguration configuration);
        byte[] Decrypt(byte[] data, ICryptoConfiguration configuration);
    }
}