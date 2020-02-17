using System;

namespace Cryptography_POC.Services
{
    public class CryptoPerformer : ICryptoPerformer, ICryptoCallBacks
    {
        private static CryptoPerformer _instance;
        private static object _syncRoot = new object();
        
        private CryptoPerformer() {}
        
        public ICryptoPlatform CryptoPlatform { get; internal set; }
        public ICryptoConfiguration CryptoConfiguration { get; internal set; } 

        public static CryptoPerformer Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_syncRoot)
                    {
                        if (_instance == null)
                        {
                            _instance = new CryptoPerformer();
                        }
                    }
                }

                return _instance;
            }
        }
        
        public byte[] Encrypt(byte[] data)
        {
            if (CryptoConfiguration == null)
            {
                throw new ArgumentNullException();
            }

            if (CryptoPlatform == null)
            {
                throw new ArgumentNullException();
            }
            
            return CryptoPlatform.Encrypt(data, CryptoConfiguration);
        }

        public byte[] Decrypt(byte[] data)
        {
            if (CryptoConfiguration == null)
            {
                throw new ArgumentNullException();
            }

            if (CryptoPlatform == null)
            {
                throw new ArgumentNullException();
            }
            
            return CryptoPlatform.Decrypt(data, CryptoConfiguration);
        }

        public Action<CryptoStatus> CryptoResponse { get; set; }
    }
}