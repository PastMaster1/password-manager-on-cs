using CommunityToolkit.Mvvm.ComponentModel;
using Password_Manager.Models;
using Password_Manager.Services;

namespace Password_Manager.ViewModels
{
    internal partial class ViewInformationWindowViewModel : ViewModelBase
    {
        [ObservableProperty]
        private string? _serviceName;
        [ObservableProperty]
        private string? _login;
        [ObservableProperty]
        private string? _password;
        [ObservableProperty]
        private string? _note;

        public ViewInformationWindowViewModel(UsersInformation SelectedPassword)
        {
            ServiceName = SelectedPassword.ServiceName;
            (Login, Password, Note) = Connections._cryptography.Decryption(SelectedPassword.Login, SelectedPassword.Password, SelectedPassword.Note);
        }
    }
}
