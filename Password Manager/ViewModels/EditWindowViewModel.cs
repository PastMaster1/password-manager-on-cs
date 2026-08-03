using CommunityToolkit.Mvvm.ComponentModel;
using Password_Manager.Models;
using Password_Manager.Services;
using System;
using System.Collections.ObjectModel;

namespace Password_Manager.ViewModels
{
    internal partial class EditWindowViewModel : ViewModelBase
    {
        [ObservableProperty]
        private string? _serviceName;
        [ObservableProperty]
        private string? _login;
        [ObservableProperty]
        private string? _password;
        [ObservableProperty]
        private string? _note;

        private int id;
        public event Action? OnRequestCloseEditWindow;
        private ObservableCollection<UsersInformation> InformationAboutUsers;

        public EditWindowViewModel(UsersInformation SelectedPassword, ObservableCollection<UsersInformation> Information)
        {
            id = SelectedPassword.VisualId - 1;
            ServiceName = SelectedPassword.ServiceName;
            (Login, Password, Note) = Connections._cryptography.Decryption(SelectedPassword.Login!, SelectedPassword.Password!, SelectedPassword.Note!);
            InformationAboutUsers = Information;
        }

        public void Save()
        {
            ServiceName = ((ServiceName == null) ? "" : ServiceName);
            Login = ((Login == null) ? "" : Login);
            Password = ((Password == null) ? "" : Password);
            Note = ((Note == null) ? "" : Note);
            Connections._actionsWithDB.Edit(ServiceName, Login, Password, Note, id, InformationAboutUsers);
            OnRequestCloseEditWindow?.Invoke();
        }

        public void Generate()
        {
            Password = Connections._cryptography.GeneratePassword();
        }
    }
}
