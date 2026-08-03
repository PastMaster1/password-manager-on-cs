using Password_Manager.Models;

namespace Password_Manager.Services
{
    internal class Connections
    {
        public static ICryptography _cryptography { get; } = new Cryptography();
        public static IActionsWithDB _actionsWithDB { get; } = new ActionsWithDB();
    }
}
