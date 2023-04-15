using System.Diagnostics;

namespace SouvlakGUI
{
    public partial class App : Application
    {
        public Models.Manager Manager { get; set; }


        public App()
        {
            InitializeComponent();
            this.Manager = new Models.Manager();
            MainPage = new AppShell();
        }
    }
}