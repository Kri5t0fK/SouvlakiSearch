namespace SouvlakGUI.Views;

public partial class AlgorithmOptionsPage : ContentPage
{
	public AlgorithmOptionsPage()
	{
		InitializeComponent();
        BindingContext = ((App)Application.Current).Manager;
    }
}