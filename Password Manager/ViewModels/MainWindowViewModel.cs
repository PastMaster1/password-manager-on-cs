using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Password_Manager.Models;
using Password_Manager.Services;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace Password_Manager.ViewModels
{
    public partial class MainWindowViewModel : ViewModelBase
    {
        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(DeleteUserInformationCommand))]
        [NotifyCanExecuteChangedFor(nameof(EditUserInformationCommand))]
        [NotifyCanExecuteChangedFor(nameof(ViewUserInformationCommand))]
        private UsersInformation? _selectedPassword;
        public bool HasSelection() => (SelectedPassword != null);

        [ObservableProperty]
        private ObservableCollection<UsersInformation>? _informationAboutUsers;

        public MainWindowViewModel()
        {
            InformationAboutUsers = new ObservableCollection<UsersInformation>();
            Connections._actionsWithDB.FirstOn(InformationAboutUsers);
        }
        public event Action? OnRequestCloseMainWindow;
        public event Action<ObservableCollection<UsersInformation>> OnRequestOpenAdditionWindow;
        public event Action<UsersInformation, ObservableCollection<UsersInformation>> OnRequestOpenEditWindow;
        public event Action<UsersInformation> OnRequestOpenViewWindow;
        public event Action<TaskCompletionSource<bool>> OnRequestOpenConfirmationWindow;

        [RelayCommand]
        private void Exit(Window? window)
        {
            OnRequestCloseMainWindow?.Invoke();
        }
        async public Task AddUserInformation()
        {
            OnRequestOpenAdditionWindow?.Invoke(InformationAboutUsers);
        }

        [RelayCommand(CanExecute = nameof(HasSelection))]
        private async Task DeleteUserInformation()
        {
            UsersInformation elem = SelectedPassword!;
            var tcs = new TaskCompletionSource<bool>();
            OnRequestOpenConfirmationWindow?.Invoke(tcs);
            bool res = await tcs.Task;
            if (res)
            {
                Connections._actionsWithDB.Delete(elem, InformationAboutUsers);
            }
        }
        
        [RelayCommand(CanExecute = nameof(HasSelection))]
        private async Task EditUserInformation()
        {
            UsersInformation elem = SelectedPassword!;
            var tcs = new TaskCompletionSource<bool>();
            OnRequestOpenConfirmationWindow?.Invoke(tcs);
            bool res = await tcs.Task;
            if (res)
            {
                OnRequestOpenEditWindow?.Invoke(elem, InformationAboutUsers);
            }
        }
        
        [RelayCommand(CanExecute = nameof(HasSelection))]
        private async Task ViewUserInformation()
        {
            UsersInformation elem = SelectedPassword!;
            var tcs = new TaskCompletionSource<bool>();
            OnRequestOpenConfirmationWindow?.Invoke(tcs);
            bool res = await tcs.Task;
            if (res)
            {
                OnRequestOpenViewWindow?.Invoke(elem);
            }
        }
    }
}
