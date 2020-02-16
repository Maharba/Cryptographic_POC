namespace Cryptography_POC.Services
{
    public interface ICryptoConfiguration
    {
        string Password { get; set; }
        int BlockSize { get; set; }
        int KeySize { get; set; }
    }
}