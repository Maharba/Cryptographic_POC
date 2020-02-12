using System.Collections.ObjectModel;
using System.Windows.Input;
using Cryptography_POC.Models;
using Cryptography_POC.Views;
using Xamarin.Forms;

namespace Cryptography_POC.ViewModels
{
    public class HomeViewModel
    {
        public ObservableCollection<CryptoList> CryptoItems { get; } = new ObservableCollection<CryptoList>();
        public ICommand ItemSelectedCommand { get; }

        public HomeViewModel()
        {
            ItemSelectedCommand = new Command<CryptoModel>(ItemSelected);
            
            CryptoItems = new ObservableCollection<CryptoList>
            {
                new CryptoList()
                {
                    GroupTitle = "Data Privacy",
                    Items =
                    {
                        new CryptoModel() { TitleText = "Aes", Description = "Advanced Encryption Standard (AES)"},
                    }
                },
                new CryptoList()
                {
                    GroupTitle = "Data Integrity",
                    Items =
                    {
                        new CryptoModel() { TitleText = "HMACSHA256", Description = "Hash-based Message Authentication Code (HMAC) - SHA256"},
                        new CryptoModel() { TitleText = "HMACSHA512", Description = "Hash-based Message Authentication Code (HMAC) - SHA512"},
                    }
                },
                new CryptoList()
                {
                    GroupTitle = "Digital Signature",
                    Items =
                    {
                        new CryptoModel() { TitleText = "ECDsa", Description = "Elliptic Curve Digital Signature Algorithm (ECDSA)"},
                        new CryptoModel() { TitleText = "RSA", Description = "Small crypto"},
                    }
                },
                new CryptoList()
                {
                    GroupTitle = "Key Exchange",
                    Items =
                    {
                        new CryptoModel() { TitleText = "ECDiffieHellman", Description = "Elliptic Curve Diffie-Hellman (ECDH)"},
                        new CryptoModel() { TitleText = "RSA", Description = "Small crypto"},
                    }
                },
                new CryptoList()
                {
                    GroupTitle = "Random Number Generator",
                    Items =
                    {
                        new CryptoModel() { TitleText = "RNGCryptoServiceProvider", Description = "Cryptographic Random Number Generator (RNG)"},
                    }
                },
                new CryptoList()
                {
                    GroupTitle = "Key from Password",
                    Items =
                    {
                        new CryptoModel() { TitleText = "Rfc2898DeriveBytes", Description = "Password-based key derivation functionality"},
                    }
                }
            };
        }

        private void ItemSelected(CryptoModel cryptoModel)
        {
            if (cryptoModel.TitleText == "Aes")
            {
                Application.Current.MainPage.Navigation.PushAsync(new AesView());
            }
        }
    }
}