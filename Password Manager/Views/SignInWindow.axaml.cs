using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Password_Manager.ViewModels;

namespace Password_Manager.Views;

public partial class SignInWindow : Window
{
    public SignInWindow()
    {
        InitializeComponent();
        DataContextChanged += (_, _) =>
        {
            if (DataContext is SignInWindowViewModel vm)
            {
                vm.OnRequestCloseSignIn += () =>
                {
                    if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
                    {
                        var newWindow = new MainWindow()
                        {
                            DataContext = new MainWindowViewModel()
                        };
                        desktop.MainWindow = newWindow;
                        newWindow.Show();
                    }
                    Close();
                };

                vm.OnRequestOpenSignUp += async () =>
                {
                    var SignUpWindow = new SignUpWindow()
                    {
                        DataContext = new SignUpWindowViewModel(),
                    };
                    await SignUpWindow.ShowDialog(this);
                };
            }
        };
    }
}