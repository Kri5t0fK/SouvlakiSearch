namespace SouvlakGUI.Views;

public partial class AlgorithmOptionsPage : ContentPage
{
	public AlgorithmOptionsPage()
	{
		InitializeComponent();
        BindingContext = ((App)Application.Current).Manager;
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