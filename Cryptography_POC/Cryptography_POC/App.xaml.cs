using System;
using Cryptography_POC.Views;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

#if !DEBUG
[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
#endif
namespace Cryptography_POC
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
#if DEBUG
            HotReloader.Current.Run(this); 
#endif

            MainPage = new NavigationPage(new HomeView());
        }

        protected override void OnStart()
        {
            // Handle when your app starts
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
    }
}