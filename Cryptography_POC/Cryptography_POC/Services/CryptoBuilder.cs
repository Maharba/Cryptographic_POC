using System;

namespace Cryptography_POC.Services
{
    public class CryptoBuilder : ICryptoBuilder
    {
        public CryptoBuilder(ICryptoPlatform cryptoPlatform)
        {
            CryptoPerformer.Instance.CryptoPlatform = cryptoPlatform;
        }
        
        public ICryptoBuilder SetCryptoConfiguration(ICryptoConfiguration config)
        {
            CryptoPerformer.Instance.CryptoConfiguration = config;
            return this;
        }

        public ICryptoBuilder SetCryptoResponse(Action<CryptoStatus> callback)
        {
            CryptoPerformer.Instance.CryptoResponse = callback;
            return this;
        }

        public ICryptoPerformer Build()
        {
            if (CryptoPerformer.Instance.CryptoConfiguration == null)
            {
                throw new ArgumentException("ICryptoBuilder requires to call method SetConfiguration before Build method");
            }
            return CryptoPerformer.Instance;
        }
    }

    public class CryptoStatus
    {
        public ResponseType Response { get; set; }
    }

    public enum ResponseType
    {
        Success = 0,
        InvalidPassword = 1
    }
}