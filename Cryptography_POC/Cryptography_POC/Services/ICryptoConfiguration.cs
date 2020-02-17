namespace Cryptography_POC.Services
{
    public interface ICryptoConfiguration
    {
        string Password { get; }
        int BlockSize { get; }
        int KeySize { get; }
        int SaltLength { get; }
        int Iterations { get; }
        
        byte[] IV { get; }
        byte[] Key { get; }
    }
}