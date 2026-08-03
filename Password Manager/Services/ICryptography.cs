namespace Password_Manager.Services
{
    internal interface ICryptography
    {
        byte[] HashMasterPassword(string password, byte[] salt);
        bool VerifyMasterPassword(string enteredPassword, string enteredLogin, string connection_string, ref int user_id);
        byte[] DeriveEncryptionKey(string masterPassword, byte[] salt);
        byte[] EncryptString(string plainText, byte[] key);
        string DecryptString(byte[] encryptedWithIv, byte[] key);
        bool Check(string plain_password);
        (string, string, string) Decryption(byte[] Login, byte[] Password, byte[] Note);
        string IsLoginUnique(string PlainLogin);
        bool CheckForSignIn(string plain_login, string plain_password);
        string GeneratePassword(int length = 16, bool useUpper = true, bool useDigits = true, bool useSymbols = true);
    }
}
