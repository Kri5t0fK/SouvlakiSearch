using SouvlakGUI.Drawables;

namespace SouvlakGUI.Views;

public partial class AlgorithmOptionsPage : ContentPage
{
	public AlgorithmOptionsPage()
	{
		InitializeComponent();
        BindingContext = ((App)Application.Current).Manager;
        MessagingCenter.Subscribe<GraphSelectPage>(this, "Clear", (sender) => { RedrawPlot(); });
        MessagingCenter.Subscribe<ResultPage>(this, "Redraw", (sender) => { RedrawPlot(); });
    }


    private void ValidateIterations(object sender, EventArgs e)
    {
        bool isOk = ((App)Application.Current).Manager.IterationsValidate();
        if (!isOk) { Application.Current.MainPage.DisplayAlert("Invalid data", "Number of iterations must be grater than 0!", "OK"); }
    }

    private void ValidateGenerationSize(object sender, EventArgs e)
    {
        bool isOk = ((App)Application.Current).Manager.GenerationSizeValidate();
        if (!isOk) { Application.Current.MainPage.DisplayAlert("Invalid data", "Generation size must be an even number grater than 0!", "OK"); }
    }

    private void ValidateSelectionSize(object sender, EventArgs e)
    {
        bool isOk = ((App)Application.Current).Manager.SelectionSizeValidate();
        if (!isOk) { Application.Current.MainPage.DisplayAlert("Invalid data", "Selection size must be an even number grater than 0 but smaller than generation size!", "OK"); }
    }

    private void ValidateMutationChance(object sender, EventArgs e)
    {
        bool isOk = ((App)Application.Current).Manager.MutationChanceValidate();
        if (!isOk) { Application.Current.MainPage.DisplayAlert("Invalid data", "Mutation chance must be grater or equal to 0 and lower or equal 100!", "OK"); }
    }

    private void ValidateStopConditionSize(object sender, EventArgs e)
    {
        bool isOk = ((App)Application.Current).Manager.StopConditionSizeValidate();
        if (!isOk) { Application.Current.MainPage.DisplayAlert("Invalid data", "In stop condition you can't check more elements than there is generations!", "OK"); }
    }


    //private void SelectionChosen(object sender, CheckedChangedEventArgs e)
    //{
    //    if (sender.ToString() == "n-best genotypes")
    //    {
    //        ((App)Application.Current).Manager.selectionType = Models.GeneticAlgorithm.SelectionType.NBest;
    //    }
    //    else if (sender.ToString() == "rank")
    //    {
    //        ((App)Application.Current).Manager.selectionType = Models.GeneticAlgorithm.SelectionType.Rank;
    //    }
        
    //}

    //private void CrossoverChosen(object sender, CheckedChangedEventArgs e)
    //{
    //    if (sender.ToString() == "two-point")
    //    {
    //        ((App)Application.Current).Manager.crossoverType = Models.GeneticAlgorithm.CrossoverType.TwoPoint;
    //    } else if (sender.ToString() == "one-point")
    //    {
    //        ((App)Application.Current).Manager.crossoverType = Models.GeneticAlgorithm.CrossoverType.OnePoint;
    //    }

    //}


    private void Calculate(object sender, EventArgs e)
    {
        MessagingCenter.Send(this, "Calculate");
    }

    private void RedrawPlot()
    {
        var graphicsView = this.PlotDrawableView;
        var plotDrawable = (PlotDrawable)graphicsView.Drawable;
        if (((App)Application.Current).Manager.Algorithm != null)
        {
            plotDrawable.bestWeights = ((App)Application.Current).Manager.Algorithm.BestWeightHistory;
            plotDrawable.medianWeights = ((App)Application.Current).Manager.Algorithm.MedianWeightHistory;
            plotDrawable.worstWeights = ((App)Application.Current).Manager.Algorithm.WorstWeightHistory;
        }
        else
        {
            plotDrawable.bestWeights = new List<float>();
            plotDrawable.medianWeights = new List<float>();
            plotDrawable.worstWeights = new List<float> ();
        }
        
        graphicsView.Invalidate();
    }
}