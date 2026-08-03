using Npgsql;
using Password_Manager.Services;
using System.Collections.ObjectModel;
using System.Security.Cryptography;

namespace Password_Manager.Models
{
    internal class ActionsWithDB : IActionsWithDB
    {
        public void FirstOn(ObservableCollection<UsersInformation> InformationAboutUsers)
        {
            using (var connection = new NpgsqlConnection(Base.connection_string))
            {
                connection.Open();
                using (var cmd = new NpgsqlCommand("SELECT id, service_name_enc, login_enc, password_enc, note_enc from UsersInformation WHERE user_id = @id", connection))
                {
                    cmd.Parameters.AddWithValue("@id", Base.user_id);
                    using (var reader = cmd.ExecuteReader())
                    {
                        int i = 1;
                        while (reader.Read())
                        {
                            InformationAboutUsers!.Add(new UsersInformation
                            {
                                VisualId = i++,
                                Id = reader.GetInt32(0),
                                ServiceName = Connections._cryptography.DecryptString(reader.GetFieldValue<byte[]>(1), Base.encryption_key!),
                                Login = reader.GetFieldValue<byte[]>(2),
                                Password = reader.GetFieldValue<byte[]>(3),
                                Note = reader.GetFieldValue<byte[]>(4)
                            });
                        }
                    }
                }
            }
        }

        public void Add(string ServiceName, string Login, string Password, string Note, ObservableCollection<UsersInformation> InformationAboutUsers)
        {
            byte[] service_name_enc = Connections._cryptography.EncryptString(ServiceName, Base.encryption_key!);
            byte[] login_enc = Connections._cryptography.EncryptString(Login, Base.encryption_key!);
            byte[] password_enc = Connections._cryptography.EncryptString(Password, Base.encryption_key!);
            byte[] note_enc = Connections._cryptography.EncryptString(Note, Base.encryption_key!);
            using (var connection = new NpgsqlConnection(Base.connection_string))
            {
                connection.Open();
                using (var cmd = new NpgsqlCommand("INSERT INTO UsersInformation (user_id, service_name_enc, login_enc, password_enc, note_enc) VALUES (@user_id, @service_name_enc, @login_enc, @password_enc, @note_enc)", connection))
                {
                    cmd.Parameters.AddWithValue("@user_id", Base.user_id);
                    cmd.Parameters.AddWithValue("@service_name_enc", service_name_enc);
                    cmd.Parameters.AddWithValue("@login_enc", login_enc);
                    cmd.Parameters.AddWithValue("@password_enc", password_enc);
                    cmd.Parameters.AddWithValue("@note_enc", note_enc);
                    cmd.ExecuteNonQuery();
                }
            }
            int i = ((InformationAboutUsers!.Count > 0) ? InformationAboutUsers[^1].VisualId + 1 : 1);
            InformationAboutUsers!.Add(new UsersInformation
            {
                VisualId = i,
                Id = Base.user_id,
                ServiceName = ServiceName,
                Login = login_enc,
                Password = password_enc,
                Note = note_enc
            });
        }

        public void Delete(UsersInformation elem, ObservableCollection<UsersInformation> InformationAboutUsers)
        {
            using (var connection = new NpgsqlConnection(Base.connection_string))
            {
                connection.Open();
                using (var cmd = new NpgsqlCommand("DELETE FROM UsersInformation WHERE id = @id", connection))
                {
                    cmd.Parameters.AddWithValue("@id", elem!.Id);
                    cmd.ExecuteNonQuery();
                }
                for (int j = elem.VisualId; j < InformationAboutUsers!.Count; j++)
                {
                    InformationAboutUsers[j].VisualId = j;
                }
                InformationAboutUsers!.Remove(elem);
            }
        }

        public void Edit(string ServiceName, string Login, string Password, string Note, int id, ObservableCollection<UsersInformation> InformationAboutUsers)
        {
            byte[] service_name_enc = Connections._cryptography.EncryptString(ServiceName, Base.encryption_key!);
            byte[] login_enc = Connections._cryptography.EncryptString(Login, Base.encryption_key!);
            byte[] password_enc = Connections._cryptography.EncryptString(Password, Base.encryption_key!);
            byte[] note_enc = Connections._cryptography.EncryptString(Note, Base.encryption_key!);
            using (var connection = new NpgsqlConnection(Base.connection_string))
            {
                connection.Open();
                using (var cmd = new NpgsqlCommand("UPDATE UsersInformation SET service_name_enc = @service, login_enc = @login, password_enc = @password, note_enc = @note WHERE id = @id", connection))
                {
                    cmd.Parameters.AddWithValue("@service", service_name_enc);
                    cmd.Parameters.AddWithValue("@login", login_enc);
                    cmd.Parameters.AddWithValue("@password", password_enc);
                    cmd.Parameters.AddWithValue("@note", note_enc);
                    cmd.Parameters.AddWithValue("@id", id + 1);
                    cmd.ExecuteNonQuery();
                }
            }
            InformationAboutUsers[id].ServiceName = ServiceName;
            InformationAboutUsers[id].Login = login_enc;
            InformationAboutUsers[id].Password = password_enc;
            InformationAboutUsers[id].Note = note_enc;
        }

        public void Registration(string PlainLogin, string PlainPassword)
        {
            using (var connection = new NpgsqlConnection(Base.connection_string))
            {
                connection.Open();
                byte[] salt = RandomNumberGenerator.GetBytes(16);
                byte[] saltForKeyDerivation = RandomNumberGenerator.GetBytes(16);
                byte[] hash = Connections._cryptography.HashMasterPassword(PlainPassword, salt);
                using (var cmd = new NpgsqlCommand("INSERT INTO Users (master_login, master_password, salt_for_master_password, salt_for_key_derivation) VALUES (@master_login, @master_password, @salt_for_master_password, @salt_for_key_derivation)", connection))
                {
                    cmd.Parameters.AddWithValue("@master_login", PlainLogin);
                    cmd.Parameters.AddWithValue("@master_password", hash);
                    cmd.Parameters.AddWithValue("@salt_for_master_password", salt);
                    cmd.Parameters.AddWithValue("@salt_for_key_derivation", saltForKeyDerivation);
                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}
