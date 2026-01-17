using HealthSync.Views;

namespace HealthSync
{
    public partial class App : Application
    {
        public App(MainTabbedPage mainTabbedPage)
        {
            InitializeComponent();
            MainPage = mainTabbedPage;
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            return new Window(MainPage);
        }
    }
}