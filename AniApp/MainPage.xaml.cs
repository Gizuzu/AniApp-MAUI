using AniApp.Pages;
using System.Diagnostics;

namespace AniApp;

public partial class MainPage : ContentPage
{
    public MainPage()
    {
        InitializeComponent();

        CheckCredentials();
    }

    private async void CheckCredentials()
    {
        LoginBtn.IsVisible = false;
        LoginProgress.IsRunning = true;

        var credentials = await Auth.GetCredentials();
        if (credentials != null && credentials.IsTokenExpired())
            credentials = await Auth.RefreshToken(credentials.RefreshToken);

        if (credentials != null)
        {
            Globals.Credentials = credentials;
            await Navigation.PushAsync(new MyAnimeList());
        }

        LoginBtn.IsVisible = true;
        LoginProgress.IsRunning = false;
    }

    private void OnLoginClicked(object sender, EventArgs e)
    {
        Debug.WriteLine("Login button clicked");
        LoginBtn.IsVisible = false;
        LoginProgress.IsRunning = true;

        var view = new WebView {
            VerticalOptions = LayoutOptions.Fill,
            Source = Auth.ConstructShikimoriAuthUrl()
        };

        Grid grid = new Grid();
        grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Star });
        Grid.SetRow(view, 0);
        grid.Children.Add(view);

        ContentPage signInContentPage = new ContentPage
        {
            Content = grid,
        };

        try
        {
            Navigation.PushModalAsync(signInContentPage);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
        Debug.WriteLine("View initialized");

        view.Navigating += async (sender, e) =>
        {
            Debug.WriteLine("Redirected to " + e.Url.ToString());
            if (e.Url.StartsWith("http://localhost"))
            {
                Debug.WriteLine("Found code, initializing client");
                await Navigation.PopModalAsync();

                var credentials = await Auth.RetrieveCredentials(e.Url);

                Debug.WriteLine($"Access granted! {credentials?.AccessToken}");

                LoginBtn.IsVisible = true;
                LoginProgress.IsRunning = false;
            }
        };
    }
}
