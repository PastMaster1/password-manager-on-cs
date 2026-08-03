using CommunityToolkit.Mvvm.ComponentModel;
using Password_Manager.Models;
using Password_Manager.Services;
using System;
using System.Collections.ObjectModel;

namespace Password_Manager.ViewModels
{
    internal partial class AdditionWindowViewModel : ViewModelBase
    {
        [ObservableProperty]
        private string? _serviceName;
        [ObservableProperty]
        private string? _login;
        [ObservableProperty]
        private string? _password;
        [ObservableProperty]
        private string? _note;

        public event Action? OnRequestCloseAddition;
        private ObservableCollection<UsersInformation> InformationAboutUsers;

        public AdditionWindowViewModel(ObservableCollection<UsersInformation> Information)
        {
            InformationAboutUsers = Information;
        }

        public void Add()
        {
            ServiceName = ((ServiceName == null) ? "" : ServiceName);
            Login = ((Login == null) ? "" : Login);
            Password = ((Password == null) ? "" : Password);
            Note = ((Note == null) ? "" : Note);
            Connections._actionsWithDB.Add(ServiceName, Login, Password, Note, InformationAboutUsers);
            OnRequestCloseAddition?.Invoke();
        }

        public void Generate()
        {
            Password = Connections._cryptography.GeneratePassword();
        }
    }
}
