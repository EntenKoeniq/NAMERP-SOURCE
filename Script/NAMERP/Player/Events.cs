using AltV.Net;
using AltV.Net.Data;
using AltV.Net.Enums;

using Npgsql;

namespace NAMERP.Player
{
    public class Events : IScript
    {
        [ClientEvent("login:pressed")]
        public static void CE_LOGIN_PRESSED(CPlayer player, string email, string password)
        {
            if (email == string.Empty || password == string.Empty)
                return;

            if (player.LoggedIn)
            {
                player.Emit("login:error", "Du bist bereits angemeldet!");
                return;
            }

            {
                NpgsqlConnection conn = Database.GetInstance().GetConnection();
                NpgsqlCommand cmd = new("SELECT * FROM accounts WHERE email = @email LIMIT 1", conn);
                cmd.Parameters.AddWithValue("@email", email);
                NpgsqlDataReader rdr = cmd.ExecuteReader();
                if (rdr.HasRows)
                {
                    rdr.Read();

                    if (!BCrypt.Net.BCrypt.Verify(password, rdr.GetString(2)))
                    {
                        player.Emit("login:error", "Passwort stimmt nicht überein!");
                    }
                    else if (Account.Helper.AccountAlreadyOnline(email))
                    {
                        player.Emit("login:error", "Dieser Account ist bereits angemeldet!");
                    }
                    else
                    {
                        player.LoggedIn = true;
                        player.ID = rdr.GetInt32(0);
                        player.Email = rdr.GetString(1);
                        player.Admin = rdr.GetByte(3);

                        player.Dimension = 0;
                        player.Position = new Position(-425, 1123, 325);
                        player.Model = (uint)PedModel.FreemodeMale01;
                    }
                }
                else
                    player.Emit("login:error", "Es wurde kein Account mit dieser Email gefunden!");

                rdr.Close();
                Database.GetInstance().FreeConnection(conn);
            }

            if (player.LoggedIn)
            {
                {
                    NpgsqlCommand cmd = new("INSERT INTO online (account_id) VALUES (@id)");
                    cmd.Parameters.AddWithValue("@id", player.ID);
                    Database.ExecuteNonQuery(cmd);
                }

                player.Emit("login:hide");
                player.SetSyncedMetaData("NAME", "Fremder");
                player.SetSyncedMetaData("ID", player.ID);

                player.Position = new(0, 0, 0);

                Vehicle.API.LoadPlayerVehicles(player.ID);
            }
        }
    }
}
