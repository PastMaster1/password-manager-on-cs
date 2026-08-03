using Avalonia.Controls;
using Password_Manager.ViewModels;


namespace Password_Manager.Views;

public partial class SignUpWindow : Window
{
    public SignUpWindow()
    {
        InitializeComponent();
        DataContextChanged += (_, _) =>
        {
            if (DataContext is SignUpWindowViewModel vm)
            {
                vm.OnRequestCloseSignUp += () =>
                {
                    Close();
                };
            }
        };
    }
}