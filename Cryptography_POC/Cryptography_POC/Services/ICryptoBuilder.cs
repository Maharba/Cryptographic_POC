using System;

namespace Cryptography_POC.Services
{
    public interface ICryptoBuilder
    {
        ICryptoBuilder SetCryptoConfiguration(AesCryptoConfiguration aesCryptoConfiguration);
        ICryptoBuilder SetCryptoResponse(Action<CryptoStatus> callback);
        ICryptoPerformer Build();
    }

    public interface ICryptoCallBacks
    {
        Action<CryptoStatus> CryptoResponse { get; set; }
    }
}