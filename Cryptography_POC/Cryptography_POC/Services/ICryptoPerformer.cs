namespace Cryptography_POC.Services
{
    public interface ICryptoPerformer
    {
        byte[] Encrypt(byte[] data);
        byte[] Decrypt(byte[] data);
    }
}