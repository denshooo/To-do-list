using System.Collections.ObjectModel;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Maui.Controls.Shapes;

namespace UI_TodoList;

public partial class MainPage : ContentPage
{
    private ObservableCollection<TodoItem> _todos = new();
    private ObservableCollection<TodoItem> _completed = new();
    
    // API Setup
    private static readonly HttpClient _httpClient = new HttpClient 
    { 
        BaseAddress = new Uri("https://todo-list.dcism.org/") 
    };
    
    // Store the logged-in user's ID for future API calls
    private int _currentUserId = 0;

    public MainPage()
    {
        InitializeComponent();
        TodoList.ItemsSource = _todos;
        CompletedList.ItemsSource = _completed;
    }

    // ─── Auth ────────────────────────────────────────────────

    private async void OnSignInClicked(object sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(EmailEntry.Text) || string.IsNullOrWhiteSpace(PasswordEntry.Text))
        {
            await DisplayAlert("Validation", "Please enter your email and password.", "OK");
            return;
        }

        try
        {
            // The Sign In API is a GET request with query parameters
            string email = Uri.EscapeDataString(EmailEntry.Text);
            string password = Uri.EscapeDataString(PasswordEntry.Text);
            string route = $"signin_action.php?email={email}&password={password}";

            var response = await _httpClient.GetAsync(route);
            var responseContent = await response.Content.ReadAsStringAsync();
            var apiResult = JsonSerializer.Deserialize<ApiResponse<UserData>>(responseContent);

            if (apiResult?.status == 200 && apiResult.data != null)
            {
                // Save user ID for future requests
                _currentUserId = apiResult.data.id;
                
                await AuthScroll.FadeTo(0, 200);
                AuthScroll.IsVisible = false;
                TodoView.IsVisible = true;
                TodoView.Opacity = 0;
                await TodoView.FadeTo(1, 200);
            }
            else
            {
                await DisplayAlert("Sign In Failed", apiResult?.message ?? "Invalid credentials.", "OK");
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Could not connect to the server: {ex.Message}", "OK");
        }
    }

    private async void OnSignUpClicked(object sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(NameEntry.Text) ||
            string.IsNullOrWhiteSpace(SignUpEmailEntry.Text) ||
            string.IsNullOrWhiteSpace(SignUpPasswordEntry.Text) ||
            string.IsNullOrWhiteSpace(ConfirmPasswordEntry.Text))
        {
            await DisplayAlert("Validation", "Please fill in all fields.", "OK");
            return;
        }
        
        if (SignUpPasswordEntry.Text != ConfirmPasswordEntry.Text)
        {
            await DisplayAlert("Validation", "Passwords do not match.", "OK");
            return;
        }

        try
        {
            // Split the single "Username" entry into first and last names for the API
            var nameParts = NameEntry.Text.Trim().Split(' ', 2);
            string firstName = nameParts[0];
            string lastName = nameParts.Length > 1 ? nameParts[1] : "";

            var payload = new
            {
                first_name = firstName,
                last_name = lastName,
                email = SignUpEmailEntry.Text,
                password = SignUpPasswordEntry.Text,
                confirm_password = ConfirmPasswordEntry.Text
            };

            var jsonContent = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");
            
            var response = await _httpClient.PostAsync("signup_action.php", jsonContent);
            var responseContent = await response.Content.ReadAsStringAsync();
            var apiResult = JsonSerializer.Deserialize<ApiResponse<object>>(responseContent);

            if (apiResult?.status == 200)
            {
                await DisplayAlert("Success", "Account created successfully. Please sign in.", "OK");
                await OnShowSignInClicked_Async();
            }
            else
            {
                await DisplayAlert("Sign Up Failed", apiResult?.message ?? "An error occurred.", "OK");
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Could not connect to the server: {ex.Message}", "OK");
        }
    }

    private async void OnShowSignUpClicked(object sender, EventArgs e)
    {
        await SignInView.FadeTo(0, 200);
        SignInView.IsVisible = false;
        SignUpView.IsVisible = true;
        SignUpView.Opacity = 0;
        await SignUpView.FadeTo(1, 200);
    }

    private async void OnShowSignInClicked(object sender, EventArgs e)
    {
        await OnShowSignInClicked_Async();
    }

    private async Task OnShowSignInClicked_Async()
    {
        await SignUpView.FadeTo(0, 200);
        SignUpView.IsVisible = false;
        SignInView.IsVisible = true;
        SignInView.Opacity = 0;
        await SignInView.FadeTo(1, 200);
    }

    // ─── Tab Navigation ──────────────────────────────────────

    private async void OnCompletedTabClicked(object sender, EventArgs e)
    {
        await TodoView.FadeTo(0, 200);
        TodoView.IsVisible = false;
        CompletedView.IsVisible = true;
        CompletedView.Opacity = 0;
        await CompletedView.FadeTo(1, 200);
    }

    private async void OnTodoTabClicked(object sender, EventArgs e)
    {
        await CompletedView.FadeTo(0, 200);
        CompletedView.IsVisible = false;
        TodoView.IsVisible = true;
        TodoView.Opacity = 0;
        await TodoView.FadeTo(1, 200);
    }

    private async void OnProfileTabClicked(object sender, EventArgs e)
    {
        await TodoView.FadeTo(0, 200);
        TodoView.IsVisible = false;
        ProfileView.IsVisible = true;
        ProfileView.Opacity = 0;
        await ProfileView.FadeTo(1, 200);
    }

    private async void OnProfileTabFromCompletedClicked(object sender, EventArgs e)
    {
        await CompletedView.FadeTo(0, 200);
        CompletedView.IsVisible = false;
        ProfileView.IsVisible = true;
        ProfileView.Opacity = 0;
        await ProfileView.FadeTo(1, 200);
    }

    private async void OnTodoTabFromProfileClicked(object sender, EventArgs e)
    {
        await ProfileView.FadeTo(0, 200);
        ProfileView.IsVisible = false;
        TodoView.IsVisible = true;
        TodoView.Opacity = 0;
        await TodoView.FadeTo(1, 200);
    }

    private async void OnCompletedTabFromProfileClicked(object sender, EventArgs e)
    {
        await ProfileView.FadeTo(0, 200);
        ProfileView.IsVisible = false;
        CompletedView.IsVisible = true;
        CompletedView.Opacity = 0;
        await CompletedView.FadeTo(1, 200);
    }

    private async void OnSignOutClicked(object sender, EventArgs e)
    {
        // Clear session data
        _currentUserId = 0;
        _todos.Clear();
        _completed.Clear();
        EmailEntry.Text = string.Empty;
        PasswordEntry.Text = string.Empty;

        await ProfileView.FadeTo(0, 200);
        ProfileView.IsVisible = false;
        AuthScroll.IsVisible = true;
        AuthScroll.Opacity = 0;
        await AuthScroll.FadeTo(1, 200);
    }

    // ─── Todo ─────────────────────────────────────────────────

    private async void OnAddTodoClicked(object sender, EventArgs e)
    {
        var titleEntry = new Entry { Placeholder = "Title" };
        var detailsEntry = new Entry { Placeholder = "Details" };

        var popup = new ContentPage
        {
            Title = "New Task",
            Content = new VerticalStackLayout
            {
                Padding = 24,
                Spacing = 16,
                Children =
                {
                    new Label { Text = "Title", TextColor = Colors.Gray, FontSize = 13 },
                    new Border
                    {
                        Stroke = Color.FromArgb("#E0E0E0"), StrokeThickness = 1,
                        BackgroundColor = Colors.White, HeightRequest = 48,
                        StrokeShape = new RoundRectangle { CornerRadius = 10 },
                        Content = titleEntry
                    },
                    new Label { Text = "Details", TextColor = Colors.Gray, FontSize = 13 },
                    new Border
                    {
                        Stroke = Color.FromArgb("#E0E0E0"), StrokeThickness = 1,
                        BackgroundColor = Colors.White, HeightRequest = 48,
                        StrokeShape = new RoundRectangle { CornerRadius = 10 },
                        Content = detailsEntry
                    },
                    new Button
                    {
                        Text = "Add Task",
                        BackgroundColor = Color.FromArgb("#b95d24"),
                        TextColor = Colors.White, CornerRadius = 10, HeightRequest = 52
                    }
                }
            }
        };

        var addButton = (Button)((VerticalStackLayout)popup.Content).Children[4];
        addButton.Clicked += async (s, args) =>
        {
            if (string.IsNullOrWhiteSpace(titleEntry.Text)) return;
            _todos.Add(new TodoItem { Title = titleEntry.Text, Details = detailsEntry.Text ?? string.Empty });
            await Navigation.PopModalAsync();
        };

        await Navigation.PushModalAsync(popup);
    }

    private async void OnEditTodoClicked(object sender, EventArgs e)
    {
        if (sender is not Button btn || btn.CommandParameter is not TodoItem item) return;

        var titleEntry = new Entry { Placeholder = "Title", Text = item.Title };
        var detailsEntry = new Entry { Placeholder = "Details", Text = item.Details };

        var popup = new ContentPage
        {
            Title = "Edit Task",
            Content = new VerticalStackLayout
            {
                Padding = 24,
                Spacing = 16,
                Children =
                {
                    new Label { Text = "Title", TextColor = Colors.Gray, FontSize = 13 },
                    new Border
                    {
                        Stroke = Color.FromArgb("#E0E0E0"), StrokeThickness = 1,
                        BackgroundColor = Colors.White, HeightRequest = 48,
                        StrokeShape = new RoundRectangle { CornerRadius = 10 },
                        Content = titleEntry
                    },
                    new Label { Text = "Details", TextColor = Colors.Gray, FontSize = 13 },
                    new Border
                    {
                        Stroke = Color.FromArgb("#E0E0E0"), StrokeThickness = 1,
                        BackgroundColor = Colors.White, HeightRequest = 48,
                        StrokeShape = new RoundRectangle { CornerRadius = 10 },
                        Content = detailsEntry
                    },
                    new Button
                    {
                        Text = "Save Changes",
                        BackgroundColor = Color.FromArgb("#b95d24"),
                        TextColor = Colors.White, CornerRadius = 10, HeightRequest = 52
                    }
                }
            }
        };

        var saveButton = (Button)((VerticalStackLayout)popup.Content).Children[4];
        saveButton.Clicked += async (s, args) =>
        {
            if (string.IsNullOrWhiteSpace(titleEntry.Text)) return;
            item.Title = titleEntry.Text;
            item.Details = detailsEntry.Text ?? string.Empty;
            var temp = new ObservableCollection<TodoItem>(_todos);
            _todos.Clear();
            foreach (var t in temp) _todos.Add(t);
            await Navigation.PopModalAsync();
        };

        await Navigation.PushModalAsync(popup);
    }

    private void OnDeleteTodoClicked(object sender, EventArgs e)
    {
        if (sender is Button btn && btn.CommandParameter is TodoItem item)
            _todos.Remove(item);
    }

    private void OnCompleteTodoClicked(object sender, EventArgs e)
    {
        if (sender is Button btn && btn.CommandParameter is TodoItem item)
        {
            _todos.Remove(item);
            _completed.Add(item);
        }
    }

    // ─── Completed ────────────────────────────────────────────

    private void OnDeleteCompletedClicked(object sender, EventArgs e)
    {
        if (sender is Button btn && btn.CommandParameter is TodoItem item)
            _completed.Remove(item);
    }

    private async void OnEditCompletedClicked(object sender, EventArgs e)
    {
        if (sender is not Button btn || btn.CommandParameter is not TodoItem item) return;

        var titleEntry = new Entry { Placeholder = "Title", Text = item.Title };
        var detailsEntry = new Entry { Placeholder = "Details", Text = item.Details };

        var popup = new ContentPage
        {
            Title = "Edit Task",
            Content = new VerticalStackLayout
            {
                Padding = 24,
                Spacing = 16,
                Children =
                {
                    new Label { Text = "Title", TextColor = Colors.Gray, FontSize = 13 },
                    new Border
                    {
                        Stroke = Color.FromArgb("#E0E0E0"), StrokeThickness = 1,
                        BackgroundColor = Colors.White, HeightRequest = 48,
                        StrokeShape = new RoundRectangle { CornerRadius = 10 },
                        Content = titleEntry
                    },
                    new Label { Text = "Details", TextColor = Colors.Gray, FontSize = 13 },
                    new Border
                    {
                        Stroke = Color.FromArgb("#E0E0E0"), StrokeThickness = 1,
                        BackgroundColor = Colors.White, HeightRequest = 48,
                        StrokeShape = new RoundRectangle { CornerRadius = 10 },
                        Content = detailsEntry
                    },
                    new Button
                    {
                        Text = "Save Changes",
                        BackgroundColor = Color.FromArgb("#b95d24"),
                        TextColor = Colors.White, CornerRadius = 10, HeightRequest = 52
                    },
                    new Button
                    {
                        Text = "Move back to To-Do",
                        BackgroundColor = Color.FromArgb("#ce8c5f"),
                        TextColor = Colors.White, CornerRadius = 10, HeightRequest = 52
                    }
                }
            }
        };

        var saveButton = (Button)((VerticalStackLayout)popup.Content).Children[4];
        saveButton.Clicked += async (s, args) =>
        {
            if (string.IsNullOrWhiteSpace(titleEntry.Text)) return;
            item.Title = titleEntry.Text;
            item.Details = detailsEntry.Text ?? string.Empty;
            var temp = new ObservableCollection<TodoItem>(_completed);
            _completed.Clear();
            foreach (var t in temp) _completed.Add(t);
            await Navigation.PopModalAsync();
        };

        var incompleteButton = (Button)((VerticalStackLayout)popup.Content).Children[5];
        incompleteButton.Clicked += async (s, args) =>
        {
            item.Title = string.IsNullOrWhiteSpace(titleEntry.Text) ? item.Title : titleEntry.Text;
            item.Details = detailsEntry.Text ?? string.Empty;
            item.IsCompleted = false;
            _completed.Remove(item);
            _todos.Add(item);
            await Navigation.PopModalAsync();
        };

        await Navigation.PushModalAsync(popup);
    }
}

// ─── Models ─────────────────────────────────────────────────

public class TodoItem
{
    public string Title { get; set; } = string.Empty;
    public string Details { get; set; } = string.Empty;
    public bool IsCompleted { get; set; } = false;
}

public class ApiResponse<T>
{
    public int status { get; set; }
    public string message { get; set; }
    public T data { get; set; }
}

public class UserData
{
    public int id { get; set; }
    public string fname { get; set; }
    public string lname { get; set; }
    public string email { get; set; }
    public string timemodified { get; set; }
}