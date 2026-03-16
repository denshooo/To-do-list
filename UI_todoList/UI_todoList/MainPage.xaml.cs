using System.Collections.ObjectModel;
using Microsoft.Maui.Controls.Shapes;

namespace UI_TodoList;

public partial class MainPage : ContentPage
{
    private ObservableCollection<TodoItem> _todos = new();
    private ObservableCollection<TodoItem> _completed = new();

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
        await AuthScroll.FadeTo(0, 200);
        AuthScroll.IsVisible = false;
        TodoView.IsVisible = true;
        TodoView.Opacity = 0;
        await TodoView.FadeTo(1, 200);
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
        await SignUpView.FadeTo(0, 200);
        SignUpView.IsVisible = false;
        SignInView.IsVisible = true;
        SignInView.Opacity = 0;
        await SignInView.FadeTo(1, 200);
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

public class TodoItem
{
    public string Title { get; set; } = string.Empty;
    public string Details { get; set; } = string.Empty;
    public bool IsCompleted { get; set; } = false;
}