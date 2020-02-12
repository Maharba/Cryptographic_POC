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
using Humanizer;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace Cryptography_POC.ViewModels
{
    public class AesViewModel : INotifyPropertyChanged
    {
        private int _bitSelected;

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
            MeasurePasswordStrengthCommand = new Command(MeasurePasswordStrength);
            EncryptFileCommand = new Command(EncryptFile);
            DownloadImageCommand = new Command(DownloadImage);
            OpenFileCommand = new Command(OpenFile);
            BitSelectedCommand = new Command<string>(BitSelected);
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

        public byte[] GenerateEncryptionKey(string password, int iterations = 1000)
        {
            byte[] salt1 = GetSalt();
            using (RNGCryptoServiceProvider rngCsp = new 
                RNGCryptoServiceProvider())
            {
                // Fill the array with a random value.
                rngCsp.GetBytes(salt1);
            }
            var keyGen = new Rfc2898DeriveBytes(password, salt1, iterations);
            return keyGen.GetBytes(32);
        }
        
        private int saltLengthLimit = 32;
        private byte[] GetSalt()
        {
            return GetSalt(saltLengthLimit);
        }
        private byte[] GetSalt(int maximumSaltLength)
        {
            var salt = new byte[maximumSaltLength];
            using (var random = new RNGCryptoServiceProvider())
            {
                random.GetNonZeroBytes(salt);
            }
            return salt;
        }
        
        private async void EncryptFile()
        {
            try
            {
                if (File.Exists(FilePath))
                {
                    if (IsEncrypted)
                    {
                        var decrypted = Decrypt(File.ReadAllBytes(FilePath), Iterations);
                        File.WriteAllBytes(FilePath, decrypted);
                    }
                    else
                    {
                        var encrypted = Encrypt(File.ReadAllBytes(FilePath), Iterations);
                        File.WriteAllBytes(FilePath, encrypted);

                    }
                    IsEncrypted = !IsEncrypted;
                    CryptoButtonText = IsEncrypted ? "Decrypt File" : "Encrypt File";
                }
                else
                {
                       Console.WriteLine("No file found."); 
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERROR:"+ ex.Message);
            }
            
            using (Aes myAes = Aes.Create())
            {
                
            
                // var filePath = Path.Combine(folder, "hausmann_abcd.jpg");
                // if (File.Exists(filePath))
                // {
                //     FileStream fs = File.Open(filePath, FileMode.Open);
                //     
                //     RijndaelManaged myCrypto = new RijndaelManaged();
                //     
                //     byte[] key = {0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x10, 0x11, 0x12, 0x13, 0x14, 0x15, 0x16};  
                //     byte[] iv = {0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x10, 0x11, 0x12, 0x13, 0x14, 0x15, 0x16};  
                //
                //     CryptoStream cryptoStream = new CryptoStream(fs, myCrypto.CreateEncryptor(key, iv), CryptoStreamMode.Write);
                //      
                //     StreamWriter sw = new StreamWriter(cryptoStream);
                //     
                //     
                //     var name = fs.Name;
                //     // Encrypt the string to an array of bytes.
                //     byte[] encrypted = EncryptStringToBytes_Aes(PasswordText, myAes.Key, myAes.IV);
                //
                //     // Decrypt the bytes to a string.
                //     string roundtrip = DecryptStringFromBytes_Aes(encrypted, myAes.Key, myAes.IV);
                //  
                //     
                //  
                //     //Display the original data and the decrypted data.
                //     Console.WriteLine("Original:   {0}", PasswordText);
                //     Console.WriteLine("Round Trip: {0}", roundtrip);
                // }
            }
        }

        private void Something()
        {
            string pwd1 = "blabla";
            // Create a byte array to hold the random value. 
            byte[] salt1 = new byte[32];
            using (RNGCryptoServiceProvider rngCsp = new RNGCryptoServiceProvider())
            {
                // Fill the array with a random value.
                rngCsp.GetBytes(salt1);
            }

            //data1 can be a string or contents of a file.
            string data1 = "Some test data";
            //The default iteration count is 1000 so the two methods use the same iteration count.
            int myIterations = 7581;
            try
            {
                Rfc2898DeriveBytes k1 = new Rfc2898DeriveBytes(pwd1, salt1, myIterations);
                Rfc2898DeriveBytes k2 = new Rfc2898DeriveBytes(pwd1, salt1, myIterations);
                // Encrypt the data.
                var encAlg = RijndaelManaged.Create();
                encAlg.Padding = PaddingMode.PKCS7;
                encAlg.Key = k1.GetBytes(32);
                MemoryStream encryptionStream = new MemoryStream();
                CryptoStream encrypt = new CryptoStream(encryptionStream,
                    encAlg.CreateEncryptor(), CryptoStreamMode.Write);
                byte[] utfD1 = new System.Text.UTF8Encoding(false).GetBytes(data1);

                encrypt.Write(utfD1, 0, utfD1.Length);
                encrypt.FlushFinalBlock();
                encrypt.Close();
                byte[] edata1 = encryptionStream.ToArray();
                k1.Reset();

                // Try to decrypt, thus showing it can be round-tripped.
                var decAlg = RijndaelManaged.Create();
                decAlg.Padding = PaddingMode.PKCS7;
                decAlg.Key = k2.GetBytes(32);
                decAlg.IV = encAlg.IV;
                MemoryStream decryptionStreamBacking = new MemoryStream();
                CryptoStream decrypt = new CryptoStream(
                    decryptionStreamBacking, decAlg.CreateDecryptor(), CryptoStreamMode.Write);
                decrypt.Write(edata1, 0, edata1.Length);
                decrypt.Flush();
                decrypt.Close();
                k2.Reset();
                string data2 = new UTF8Encoding(false).GetString(decryptionStreamBacking.ToArray());

                if (!data1.Equals(data2))
                {
                    Console.WriteLine("Error: The two values are not equal.");
                }
                else
                {
                    Console.WriteLine("The two values are equal.");
                    Console.WriteLine("k1 iterations: {0}", k1.IterationCount);
                    Console.WriteLine("k2 iterations: {0}", k2.IterationCount);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: ", e);
            }
        }

        private byte[] something1;
        private byte[] key1;

        public byte[] Encrypt(byte[] data, int iterations)
        {
            using (var aes = new RijndaelManaged())
            {
                aes.BlockSize = _bitSelected;
                aes.Padding = PaddingMode.PKCS7;

                key1 = GenerateEncryptionKey(PasswordText, iterations);
                aes.Key = key1;
                aes.GenerateIV();
                something1 = aes.IV;

                using (var encryptor = aes.CreateEncryptor(aes.Key, aes.IV))
                {
                    return PerformCryptography(data, encryptor);
                }
            }
        }

        public byte[] Decrypt(byte[] data, int iterations)
        {
            using (var aes = new RijndaelManaged())
            {
                aes.BlockSize = _bitSelected;
                aes.Padding = PaddingMode.PKCS7;
                aes.Key = key1;
                aes.IV = something1;

                using (var decryptor = aes.CreateDecryptor(aes.Key, aes.IV))
                {
                    return PerformCryptography(data, decryptor);
                }
            }
        }

        private byte[] PerformCryptography(byte[] data, ICryptoTransform cryptoTransform)
        {
            using (var ms = new MemoryStream())
            using (var cryptoStream = new CryptoStream(ms, cryptoTransform, CryptoStreamMode.Write))
            {
                cryptoStream.Write(data, 0, data.Length);
                cryptoStream.FlushFinalBlock();

                return ms.ToArray();
            }
        }
        

        private byte[] EncryptStringToBytes_Aes(string plainText, byte[] Key, byte[] IV)
        {
            // Check arguments.
            if (plainText == null || plainText.Length <= 0)
                throw new ArgumentNullException("plainText");
            if (Key == null || Key.Length <= 0)
                throw new ArgumentNullException("Key");
            if (IV == null || IV.Length <= 0)
                throw new ArgumentNullException("IV");
            byte[] encrypted;
            
            // Create an Aes object
            // with the specified key and IV.
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = Key;
                aesAlg.IV = IV;

                // Create an encryptor to perform the stream transform.
                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                // Create the streams used for encryption.
                using (MemoryStream msEncrypt = new MemoryStream())
                using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                {
                    //Write all data to the stream.
                    swEncrypt.Write(plainText);
                    encrypted = msEncrypt.ToArray();
                }
            }

            // Return the encrypted bytes from the memory stream.
            return encrypted;
        }

        private string DecryptStringFromBytes_Aes(byte[] cipherText, byte[] Key, byte[] IV)
        {
            // Check arguments.
            if (cipherText == null || cipherText.Length <= 0)
                throw new ArgumentNullException("cipherText");
            if (Key == null || Key.Length <= 0)
                throw new ArgumentNullException("Key");
            if (IV == null || IV.Length <= 0)
                throw new ArgumentNullException("IV");

            // Declare the string used to hold
            // the decrypted text.
            string plaintext = null;

            // Create an Aes object
            // with the specified key and IV.
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = Key;
                aesAlg.IV = IV;

                // Create a decryptor to perform the stream transform.
                ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                // Create the streams used for decryption.
                using (MemoryStream msDecrypt = new MemoryStream(cipherText))
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                        {

                            // Read the decrypted bytes from the decrypting stream
                            // and place them in a string.
                            plaintext = srDecrypt.ReadToEnd();
                        }
                    }
                }
            }

            return plaintext;
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