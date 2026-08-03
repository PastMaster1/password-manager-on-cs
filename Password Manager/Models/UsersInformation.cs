using CommunityToolkit.Mvvm.ComponentModel;

namespace Password_Manager.Models
{
    public partial class UsersInformation: ObservableObject
    {
        [ObservableProperty]
        private int _visualId;
        [ObservableProperty]
        private int _id;
        [ObservableProperty]
        private string? _serviceName;
        [ObservableProperty]
        private byte[]? _login;
        [ObservableProperty]
        private byte[]? _password;
        [ObservableProperty]
        private byte[]? _note;
    }
}
