using Avalonia.Controls;
using Password_Manager.ViewModels;

namespace Password_Manager.Views;

public partial class EditWindow : Window
{
    public EditWindow()
    {
        InitializeComponent();
        DataContextChanged += (_, _) =>
        {
            if (DataContext is EditWindowViewModel vm)
            {
                vm.OnRequestCloseEditWindow += () => Close();
            }
        };
    }
}