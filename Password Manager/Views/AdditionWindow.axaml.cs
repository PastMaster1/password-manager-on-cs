using Avalonia.Controls;
using Password_Manager.ViewModels;

namespace Password_Manager.Views;

public partial class AdditionWindow : Window
{
    public AdditionWindow()
    {
        InitializeComponent();
        DataContextChanged += (_, _) =>
        {
            if (DataContext is AdditionWindowViewModel vm)
            {
                vm.OnRequestCloseAddition += () =>
                {
                    Close();
                };
            }
        };
    }
}