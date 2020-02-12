using System.Collections.ObjectModel;

namespace Cryptography_POC.Models
{
    public class CryptoModel
    {
        public string TitleText { get; set; }
        public string Description { get; set; }
    }

    public class CryptoList : ObservableCollection<CryptoModel>
    {
        public string GroupTitle { get; set; }
        public ObservableCollection<CryptoModel> Items => this;
    }
}