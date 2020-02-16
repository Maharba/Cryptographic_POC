using System;

namespace Cryptography_POC.Services
{
    public class CryptoBuilder : ICryptoBuilder
    {
        public CryptoBuilder(ICryptoStarter cryptoStarter)
        {
            
        }
        
        public ICryptoBuilder SetCryptoConfiguration(AesCryptoConfiguration aesCryptoConfiguration)
        {
            throw new NotImplementedException();
        }

        public ICryptoBuilder SetCryptoResponse(Action<CryptoStatus> callback)
        {
            throw new NotImplementedException();
        }

        public ICryptoPerformer Build()
        {
            throw new NotImplementedException();
        }
    }

    public class CryptoStatus
    {
        private ResponseType Response { get; set; }
    }

    public enum ResponseType
    {
        Success = 0,
        InvalidPassword = 1
    }
}