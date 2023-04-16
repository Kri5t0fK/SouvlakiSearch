using SouvlakGUI.Models;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace SouvlakGUI.Views;


public partial class GraphSelectPage : ContentPage
{
    public GraphSelectPage()
	{
		InitializeComponent();
        BindingContext = ((App)Application.Current).Manager;
    }

    #nullable enable
    private void NewExampleGraphSelected(object sender, CheckedChangedEventArgs e)    // At this point I don't care about warnings...
    {
        if (e.Value)
        {
            var filename = ((RadioButton)sender).Content.ToString() + ".json";
            JsonSerializerOptions _options = new()
            {
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
                Converters = { new Vector2Converter() }
            };

            try
            {
                //var assembly = typeof(App).GetTypeInfo().Assembly;
                //var assemblyName = assembly.GetName().Name;
                //var stream = assembly.GetManifestResourceStream($"{assemblyName}/{filename}");
                //if ( stream == null )
                //{
                //    ((RadioButton)sender).IsChecked = false;
                //    ((App)Application.Current).Manager.ReloadGraph(null);
                //    Application.Current.MainPage.DisplayAlert("ERROR", "Couldn't open file " + filename + "!", "OK");
                //}
                var stream = FileSystem.OpenAppPackageFileAsync(filename).Result;

                List<Graph.Vertex>? tempGraph = JsonSerializer.Deserialize<List<Graph.Vertex>>(stream, _options);
                if (tempGraph == null)
                {
                    ((RadioButton)sender).IsChecked = false;
                    ((App)Application.Current).Manager.ReloadGraph(null);
                    Application.Current.MainPage.DisplayAlert("ERROR", "Selected JSON file contains errors!", "OK");
                }
                else
                {
                    ((App)Application.Current).Manager.ReloadGraph(new Graph(tempGraph));
                }
            }
            catch (Exception ex)
            {
                ((RadioButton)sender).IsChecked = false;
                ((App)Application.Current).Manager.ReloadGraph(null);
                Application.Current.MainPage.DisplayAlert("ERROR", "Couldn't load file " + filename + "!", "OK");
            }
        }
    }
    #nullable disable

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