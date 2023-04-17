using SouvlakGUI.Drawables;
using SouvlakGUI.Models;
namespace SouvlakGUI.Views;

public partial class ResultPage : ContentPage
{
	public ResultPage()
	{
		InitializeComponent();
		BindingContext = ((App)Application.Current).Manager;
        MessagingCenter.Subscribe<GraphSelectPage>(this, "Clear", (sender) => { RedrawGraph(); });
        MessagingCenter.Subscribe<GraphSelectPage>(this, "Calculate", (sender) => { DoCalculations(); });
        MessagingCenter.Subscribe<AlgorithmOptionsPage>(this, "Calculate", (sender) => { DoCalculations(); });
    }

    public void RedrawGraph()
    {
        var graphicsView = this.GraphDrawableView;
        var graphDrawable = (GraphDrawable)graphicsView.Drawable;
        graphDrawable.Graph = ((App)Application.Current).Manager.UpdatedGraph;
        graphicsView.Invalidate();
    }


    private async void DoCalculations()
    {
        if (((App)Application.Current).Manager.SelectedGraph != null)
        {
            await Task.Run(() => ((App)Application.Current).Manager.Calculate());
            RedrawGraph();
        }
        else
        {
            await Application.Current.MainPage.DisplayAlert("Warning", "Please select a graph before calculating!", "OK");
            RedrawGraph();
        }
    }


    private void Calculate(object sender, EventArgs e)
    {
        DoCalculations();
    }
}