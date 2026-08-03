using CommunityToolkit.Mvvm.ComponentModel;
using Password_Manager.Models;
using Password_Manager.Services;
using System;
using System.Threading.Tasks;

namespace Password_Manager.ViewModels
{
    internal partial class SignInWindowViewModel : ViewModelBase
    {
        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(SignInHint))]
        private string? _plainLogin;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(SignInHint))]
        private string? _plainPassword;

        public event Action? OnRequestCloseSignIn;
        public event Action? OnRequestOpenSignUp;

        public string SignInHint
        {
            get
            {
                if (string.IsNullOrEmpty(PlainLogin) || string.IsNullOrEmpty(PlainPassword))
                {
                    return "Введите все данные";
                }
                return "Войти";
            }
        }

        public SignInWindowViewModel()
        {
            Base.Connection();
        }

        public void SignIn()
        {
            if (SignInHint == "Войти" && Connections._cryptography.CheckForSignIn(PlainLogin!, PlainPassword!))
            {
                OnRequestCloseSignIn?.Invoke();
            }
        }

        async public Task SignUp()
        {
            OnRequestOpenSignUp?.Invoke();
        }
    }
}
