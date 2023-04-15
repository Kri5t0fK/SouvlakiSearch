using Microsoft.Maui.Controls;
using Microsoft.Maui.Storage;
using SouvlakGUI.Models;
using System.Reflection;
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
                var stream = FileSystem.Current.OpenAppPackageFileAsync(filename).Result;

                //List<Graph.Vertex>? tempGraph = JsonSerializer.Deserialize<List<Graph.Vertex>>(stream);
                //if (tempGraph == null)
                //{
                //    await Application.Current.MainPage.DisplayAlert("ERROR", "Failed to load selected JSON file!", "OK");
                //}
                //else
                //{
                //    ((App)Application.Current).Manager.ReloadGraph(new Graph(tempGraph));
                //}
            }
            catch (Exception ex)
            {
                ((RadioButton)sender).IsChecked= false;
                ((App)Application.Current).Manager.ReloadGraph(null);
                Application.Current.MainPage.DisplayAlert("ERROR", "Couldn't find file " + filename + "!", "OK");
            }
        }
    }
    #nullable disable
}