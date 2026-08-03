using Password_Manager.Models;
using System.Collections.ObjectModel;

namespace Password_Manager.Services
{
    internal interface IActionsWithDB
    {
        void FirstOn(ObservableCollection<UsersInformation> InformationAboutUsers);
        void Add(string ServiceName, string Login, string Password, string Note, ObservableCollection<UsersInformation> InformationAboutUsers);
        void Delete(UsersInformation elem, ObservableCollection<UsersInformation> InformationAboutUsers);
        void Edit(string ServiceName, string Login, string Password, string Note, int id, ObservableCollection<UsersInformation> InformationAboutUsers);
        void Registration(string PlainLogin, string PlainPassword);
    }
}
