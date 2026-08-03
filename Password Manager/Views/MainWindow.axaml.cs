using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Password_Manager.Models;
using Password_Manager.ViewModels;

namespace Password_Manager.Views
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContextChanged += (_, _) =>
            {
                if (DataContext is MainWindowViewModel vm)
                {
                    vm.OnRequestCloseMainWindow += () => Close();

                    vm.OnRequestOpenAdditionWindow += async (InformationAboutUsers) =>
                    {
                        var AdditionWindow = new AdditionWindow()
                        {
                            DataContext = new AdditionWindowViewModel(InformationAboutUsers),
                        };
                        await AdditionWindow.ShowDialog(this);
                    };

                    vm.OnRequestOpenConfirmationWindow += async (res) =>
                    {
                        var ConfirmationWindow = new ConfirmationWindow()
                        {
                            DataContext = new ConfirmationWindowViewModel(),
                        };
                        bool result = await ConfirmationWindow.ShowDialog<bool>(this);
                        res.SetResult(result);
                    };

                    vm.OnRequestOpenEditWindow += async (elem, InformationAboutUsers) =>
                    {
                        var EditWindow = new EditWindow()
                        {
                            DataContext = new EditWindowViewModel(elem, InformationAboutUsers),
                        };
                        await EditWindow.ShowDialog(this);
                    };

                    vm.OnRequestOpenViewWindow += async (elem) =>
                    {
                        var ViewInformationWindow = new ViewInformationWindow()
                        {
                            DataContext = new ViewInformationWindowViewModel(elem),
                        };
                        await ViewInformationWindow.ShowDialog(this);
                    };
                }
            };
        }
    }
}