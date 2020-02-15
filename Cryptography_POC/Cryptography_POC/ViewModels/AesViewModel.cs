using System;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Net;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Cryptography_POC.Annotations;
using Cryptography_POC.Helpers;
using Cryptography_POC.Services;
using Humanizer;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace Cryptography_POC.ViewModels
{
    public class AesViewModel : INotifyPropertyChanged
    {
        private int _bitSelected;

        private ICryptoFileService _cryptoFileService;

        public ICommand MeasurePasswordStrengthCommand { get; private set; }
        public ICommand EncryptFileCommand { get; private set; }
        public ICommand DownloadImageCommand { get; private set; }
        public ICommand OpenFileCommand { get; private set; }
        public ICommand BitSelectedCommand { get; private set; }
        
        #region Properties

        private int _iterations;

        public int Iterations
        {
            get => _iterations;
            set
            {
                _iterations = value;
                OnPropertyChanged(nameof(_iterations));
            }
        }

        private string _fileName;
        public string FileName
        {
            get => _fileName;
            set
            {
                _fileName = value;
                OnPropertyChanged(nameof(FileName));
            }
        }

        private string _filePath;

        public string FilePath
        {
            get => _filePath;
            set
            {
                _filePath = value;
                OnPropertyChanged(nameof(FilePath));
            }
        }

        private string _cryptoButtonText;

        public string CryptoButtonText
        {
            get => _cryptoButtonText;
            set
            {
                _cryptoButtonText = value;
                OnPropertyChanged(nameof(CryptoButtonText));
            }
        }

        private bool _isEncrypted;

        public bool IsEncrypted
        {
            get => _isEncrypted;
            set
            {
                _isEncrypted = value;
                OnPropertyChanged(nameof(IsEncrypted));
            }
        }

        private string _passwordText;
        public string PasswordText
        {
            get => _passwordText;
            set
            {
                _passwordText = value;
                OnPropertyChanged(nameof(PasswordText));
            }
        }

        private string _entropy;
        public string Entropy
        {
            get => _entropy;
            set
            {
                _entropy = value;
                OnPropertyChanged(nameof(Entropy));
            } 
        }
        
        private string _crackTime;

        public string CrackTime
        {
            get => _crackTime;
            set
            {
                _crackTime = value;
                OnPropertyChanged(nameof(CrackTime));
            }
        }

        private double _downloadProgress;
        public double DownloadProgress
        {
            get => _downloadProgress;
            set
            {
                _downloadProgress = value;
                OnPropertyChanged(nameof(DownloadProgress));
            }
        }

        private string _crackTimeDisplay;

        public string CrackTimeDisplay
        {
            get => _crackTimeDisplay;
            set
            {
                _crackTimeDisplay = value;
                OnPropertyChanged(nameof(CrackTimeDisplay));
            }
        }

        private string _score;
        public string Score
        {
            get => _score;
            set
            {
                _score = value;
                OnPropertyChanged(nameof(Score));
            }
        }

        private string _matchSequence;
        public string MatchSequence
        {
            get => _matchSequence;
            set
            {
                _matchSequence = value;
                OnPropertyChanged(nameof(MatchSequence));
            }
        }

        private string _calculationTime;
        public string CalculationTime
        {
            get => _calculationTime;
            set
            {
                _calculationTime = value;
                OnPropertyChanged(nameof(CalculationTime));
            }
        }
        #endregion

        public AesViewModel()
        {
            _cryptoFileService = new CryptoFileService();

            MeasurePasswordStrengthCommand = new Command(MeasurePasswordStrength);
            EncryptFileCommand = new Command(EncryptFile);
            DownloadImageCommand = new Command(DownloadImage);
            OpenFileCommand = new Command(OpenFile);
            BitSelectedCommand = new Command<string>(BitSelected);
        }

        private void EncryptFile()
        {
            if (IsEncrypted)
            {
                _cryptoFileService.DecryptFile(FilePath, PasswordText);
                CryptoButtonText = "Encrypt File";
            }
            else
            {
                _cryptoFileService.EncryptFile(FilePath, PasswordText, BitSizes.Bit256, BitSizes.Bit128, 32, Iterations );
                CryptoButtonText = "Decrypt File";
            }

            IsEncrypted = !IsEncrypted;
        }

        private void BitSelected(string selectedBit)
        {
            _bitSelected = int.Parse(selectedBit);
        }

        private void OpenFile()
        {
            Launcher.OpenAsync(new OpenFileRequest { File = new ReadOnlyFile(FilePath)});
        }

        private void DownloadImage()
        {
            string folder = string.Empty;
            var exs = File.Exists(folder + "/hausmann_abcd.jpg");
            try
            {
                using (WebClient webClient = new WebClient())
                {
                    webClient.DownloadDataCompleted += WebClientOnDownloadDataCompleted;
                    webClient.DownloadProgressChanged += WebClientOnDownloadProgressChanged;
                    folder = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                    webClient.DownloadDataAsync(new Uri("http://www.dada-data.net/uploads/image/hausmann_abcd.jpg"), folder + "/hausmann_abcd.jpg");
                    CryptoButtonText = IsEncrypted ? "Decrypt File" : "Encrypt File";
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERROR:"+ ex.Message);
            }
        }

        private void WebClientOnDownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            Console.WriteLine(e.ProgressPercentage);
            DownloadProgress = (double)e.ProgressPercentage / 100;
        }

        private async void WebClientOnDownloadDataCompleted(object sender, DownloadDataCompletedEventArgs args)
        {
            try
            {
                FilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "hausmann_abcd.jpg");
                
                File.WriteAllBytes(FilePath, args.Result);

                FileName = "hausmann_abcd.jpg";
            }
            catch (Exception e)
            {
                Console.WriteLine("ERROR: " + e);
            }
        }

        private void MeasurePasswordStrength()
        {
            var result = Zxcvbn.Zxcvbn.MatchPassword(PasswordText);
            Entropy = result.Entropy.ToString(CultureInfo.CurrentCulture);
            CrackTime = TimeSpan.FromSeconds(result.CrackTime).Humanize((5));
            CrackTimeDisplay = result.CrackTimeDisplay;
            Score = result.Score.ToString();
            CalculationTime = TimeSpan.FromMilliseconds(result.CalcTime).Humanize(5);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}