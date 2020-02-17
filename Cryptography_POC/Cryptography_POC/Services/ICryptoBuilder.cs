using System;

namespace Cryptography_POC.Services
{
    public interface ICryptoBuilder
    {
        ICryptoBuilder SetCryptoConfiguration(ICryptoConfiguration config);
        ICryptoBuilder SetCryptoResponse(Action<CryptoStatus> callback);
        ICryptoPerformer Build();
    }

    public interface ICryptoCallBacks
    {
        Action<CryptoStatus> CryptoResponse { get; set; }
    }
}