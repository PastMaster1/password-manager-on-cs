using Avalonia.Controls;
using Password_Manager.ViewModels;

namespace Password_Manager.Views;

public partial class ConfirmationWindow : Window
{
    public ConfirmationWindow()
    {
        InitializeComponent();
        DataContextChanged += (_, _) =>
        {
            if (DataContext is ConfirmationWindowViewModel vm)
            {
                vm.OnRequestCloseConfirmationWindow += (f) => Close(f);
            }
        };
    }
}