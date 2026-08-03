using Konscious.Security.Cryptography;
using Npgsql;
using Password_Manager.Services;
using System;
using System.Security.Cryptography;
using System.Text;

namespace Password_Manager.Models
{
    internal class Cryptography : ICryptography
    {
        public byte[] HashMasterPassword(string password, byte[] salt)
        {
            using var argon2 = new Argon2id(Encoding.UTF8.GetBytes(password));
            argon2.Salt = salt;
            argon2.DegreeOfParallelism = 4;
            argon2.MemorySize = 65536;
            argon2.Iterations = 4;
            return argon2.GetBytes(32);
        }

        public bool VerifyMasterPassword(string enteredPassword, string enteredLogin, string connection_string, ref int user_id)
        {
            using (var connection = new NpgsqlConnection(connection_string))
            {
                connection.Open();
                using (var cmd = new NpgsqlCommand("SELECT id, master_password, salt_for_master_password FROM Users WHERE master_login = @login", connection))
                {
                    cmd.Parameters.AddWithValue("@login", enteredLogin);
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            user_id = reader.GetInt32(0);
                            byte[] master_password = reader.GetFieldValue<byte[]>(1);
                            byte[] salt_for_master_password = reader.GetFieldValue<byte[]>(2);
                            byte[] computedHash = HashMasterPassword(enteredPassword, salt_for_master_password);
                            return (CryptographicOperations.FixedTimeEquals(computedHash, master_password));
                        }
                    }
                }
            }
            return false;
        }

        public byte[] DeriveEncryptionKey(string masterPassword, byte[] salt)
        {
            using var argon2 = new Argon2id(Encoding.UTF8.GetBytes(masterPassword));
            argon2.Salt = salt;
            argon2.DegreeOfParallelism = 4;
            argon2.MemorySize = 65536;
            argon2.Iterations = 4;
            return argon2.GetBytes(32);
        }
        
        public byte[] EncryptString(string plainText, byte[] key)
        {
            using var aes = Aes.Create();
            aes.Key = key;
            aes.GenerateIV();
            using var encryptor = aes.CreateEncryptor();
            byte[] plainBytes = Encoding.UTF8.GetBytes(plainText);
            byte[] cipherBytes = encryptor.TransformFinalBlock(plainBytes, 0, plainBytes.Length);
            byte[] result = new byte[aes.IV.Length + cipherBytes.Length];
            int i = 0;
            for (int j = 0; j < aes.IV.Length; i++, j++)
            {
                result[i] = aes.IV[j];
            }
            for (int j = 0; j < cipherBytes.Length; i++, j++)
            {
                result[i] = cipherBytes[j];
            }
            return result;
        }

        public string DecryptString(byte[] encryptedWithIv, byte[] key)
        {
            using var aes = Aes.Create();
            aes.Key = key;
            byte[] iv = new byte[16];
            byte[] cipherBytes = new byte[encryptedWithIv.Length - 16];
            int i = 0;
            for (int j = 0; j < 16; i++, j++)
            {
                iv[j] = encryptedWithIv[i];
            }
            for (int j = 0; j < cipherBytes.Length; i++, j++)
            {
                cipherBytes[j] = encryptedWithIv[i];
            }
            aes.IV = iv;
            using var decryptor = aes.CreateDecryptor();
            byte[] decryptedBytes = decryptor.TransformFinalBlock(cipherBytes, 0, cipherBytes.Length);
            return Encoding.UTF8.GetString(decryptedBytes);
        }

        public bool Check(string plain_password)
        {
            using (var connection = new NpgsqlConnection(Base.connection_string))
            {
                connection.Open();
                using (var cmd = new NpgsqlCommand("SELECT master_password, salt_for_master_password FROM Users WHERE id = @user_id", connection))
                {
                    cmd.Parameters.AddWithValue("@user_id", Base.user_id);
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            byte[] master_password = reader.GetFieldValue<byte[]>(0);
                            byte[] salt_for_master_password = reader.GetFieldValue<byte[]>(1);
                            byte[] computedHash = HashMasterPassword(plain_password, salt_for_master_password);
                            return CryptographicOperations.FixedTimeEquals(computedHash, master_password);
                        }
                    }
                }
            }
            return false;
        }

        public (string, string, string) Decryption(byte[] Login, byte[] Password, byte[] Note)
        {
            string res_Login = DecryptString(Login, Base.encryption_key);
            string res_Password = DecryptString(Password, Base.encryption_key);
            string res_Note = DecryptString(Note, Base.encryption_key);
            return (res_Login, res_Password, res_Note);
        }

        public string IsLoginUnique(string PlainLogin)
        {
            using (var connection = new NpgsqlConnection(Base.connection_string))
            {
                connection.Open();
                using (var cmd = new NpgsqlCommand("SELECT COUNT(*) from Users WHERE master_login = @login", connection))
                {
                    cmd.Parameters.AddWithValue("@login", PlainLogin);
                    if (Convert.ToInt32(cmd.ExecuteScalar()) != 0)
                    {
                        return ("Пользователь с таким именем уже существует");
                    }
                }
            }
            return ("Зарегистрироваться");
        }

        public bool CheckForSignIn(string plain_login, string plain_password)
        {
            using (var connection = new NpgsqlConnection(Base.connection_string))
            {
                connection.Open();
                using (var cmd = new NpgsqlCommand("SELECT id, master_password, salt_for_master_password, salt_for_key_derivation FROM Users WHERE master_login = @login", connection))
                {
                    cmd.Parameters.AddWithValue("@login", plain_login);
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            byte[] master_password = reader.GetFieldValue<byte[]>(1);
                            byte[] salt_for_master_password = reader.GetFieldValue<byte[]>(2);
                            byte[] salt_for_key_derivation = reader.GetFieldValue<byte[]>(3);
                            byte[] computedHash = HashMasterPassword(plain_password, salt_for_master_password);
                            if (CryptographicOperations.FixedTimeEquals(computedHash, master_password))
                            {
                                Base.user_id = reader.GetInt32(0);
                                Base.encryption_key = DeriveEncryptionKey(plain_password, salt_for_key_derivation);
                                return true;
                            }
                        }
                    }
                }
            }
            return false;
        }

        public string GeneratePassword(int length = 16, bool useUpper = true, bool useDigits = true, bool useSymbols = true)
        {
            const string lower = "abcdefghijklmnopqrstuvwxyz";
            const string upper = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            const string digits = "0123456789";
            const string symbols = "!@#$%^&*()_+-=[]{}|;:,.<>?";
            string charPool = lower;
            if (useUpper) charPool += upper;
            if (useDigits) charPool += digits;
            if (useSymbols) charPool += symbols;
            var password = new char[length];
            for (int i = 0; i < length; i++)
            {
                password[i] = charPool[RandomNumberGenerator.GetInt32(charPool.Length)];
            }
            return new string(password);
        }
    }
}
