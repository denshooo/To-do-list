namespace UI_TodoList;

public partial class MainPage : ContentPage
{
    public MainPage()
    {
        InitializeComponent();
    }

    private async void OnSignInClicked(object sender, EventArgs e)
    {
        var email = EmailEntry.Text;
        var password = PasswordEntry.Text;

        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
        {
            await DisplayAlert("Validation", "Please enter your email and password.", "OK");
            return;
        }

        // TODO: add your sign in logic here
    }

    private async void OnSignUpClicked(object sender, EventArgs e)
    {
        // TODO: add your sign up logic here
    }
}