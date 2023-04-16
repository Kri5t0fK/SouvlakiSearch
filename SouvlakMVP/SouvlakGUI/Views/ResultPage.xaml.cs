namespace SouvlakGUI.Views;

public partial class ResultPage : ContentPage
{
	public ResultPage()
	{
		InitializeComponent();
		BindingContext = ((App)Application.Current).Manager;
    }

    private async void Calculate(object sender, EventArgs e)
    {
        if (((App)Application.Current).Manager.SelectedGraph != null)
        {
            await Task.Run(() => ((App)Application.Current).Manager.Calculate());
        }
        else
        {
            await Application.Current.MainPage.DisplayAlert("Warning", "Please select a graph before calculating!", "OK");
        }
    }
}