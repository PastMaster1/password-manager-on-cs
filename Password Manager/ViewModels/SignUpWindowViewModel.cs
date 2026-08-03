using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using Password_Manager.Models;
using Password_Manager.Services;
using System.Collections.ObjectModel;
using System;

namespace Password_Manager.ViewModels
{
    internal partial class SignUpWindowViewModel : ViewModelBase
    {
        public ObservableCollection<UsersInformation> InformationAboutUser { get; set; }

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(RegistrationHint))]
        private string? _plainLogin;
        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(RegistrationHint))]
        private string? _plainPassword1;
        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(RegistrationHint))]
        private string? _plainPassword2;

        public event Action? OnRequestCloseSignUp;

        public SignUpWindowViewModel() 
        {
            InformationAboutUser = new ObservableCollection<UsersInformation>();
        }
        public string RegistrationHint
        {
            get
            {
                if (string.IsNullOrEmpty(PlainLogin) || string.IsNullOrEmpty(PlainPassword1) || string.IsNullOrEmpty(PlainPassword2))
                    return "Заполните все поля";

                if (PlainPassword1 != PlainPassword2)
                    return "Пароли должны совпадать";

                return Connections._cryptography.IsLoginUnique(PlainLogin);
            }
        }

        public void Registration(Window? window)
        {
            if (RegistrationHint == "Зарегистрироваться")
            {
                Connections._actionsWithDB.Registration(PlainLogin!, PlainPassword1!);
                OnRequestCloseSignUp?.Invoke();
            }
        }
    }
}
