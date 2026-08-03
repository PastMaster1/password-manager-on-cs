using System;
using CommunityToolkit.Mvvm.ComponentModel;
using Password_Manager.Services;

namespace Password_Manager.ViewModels
{
    internal partial class ConfirmationWindowViewModel : ViewModelBase
    {
        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(ConfirmationHint))]
        private string? _plainPassword;

        public event Action<bool> OnRequestCloseConfirmationWindow;
        public string ConfirmationHint
        {
            get
            {
                if (string.IsNullOrEmpty(PlainPassword))
                {
                    return "Введите мастер-пароль";
                }
                return "Ввести";
            }
        }

        public void Confirm()
        {
            if (ConfirmationHint == "Ввести" && Connections._cryptography.Check(PlainPassword!))
            {
                OnRequestCloseConfirmationWindow?.Invoke(true);
            }
        }
    }
}
