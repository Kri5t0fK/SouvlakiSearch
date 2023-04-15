namespace SouvlakGUI.Views;

public partial class ResultPage : ContentPage
{
	public ResultPage()
	{
		InitializeComponent();
		BindingContext = ((App)Application.Current).Manager;

    }
}