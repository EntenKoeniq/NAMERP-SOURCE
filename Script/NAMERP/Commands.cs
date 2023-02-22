using AltV.Net;
using AltV.Net.Elements.Entities;
using AltV.Net.Enums;
using AltV.Net.Resources.Chat.Api;

using Npgsql;

namespace NAMERP
{
    public class Commands : IScript
    {
        [CommandEvent(CommandEventType.CommandNotFound)]
        public static void OnCommandNotFound(IPlayer player, string cmd)
        {
            player.SendChatMessage($"{{FF0000}}Befehl \"{cmd}\" nicht gefunden!");
        }

        [Command("leave")]
        public static void CMD_LeaveHouse(IPlayer player)
        {
            House.API.LeaveHouse(player);
        }

        //[Command("register")]
        //public static void CMD_Register(CPlayer player, string email, string password)
        //{
        //    if (email == string.Empty || password == string.Empty)
        //        return;
        //
        //    if (player.LoggedIn)
        //    {
        //        player.SendChatMessage("{FF0000}Du bist bereits angemeldet!");
        //        return;
        //    }
        //    
        //    {
        //        bool error = false;
        //        NpgsqlConnection conn = Database.GetInstance().GetConnection();
        //        NpgsqlCommand cmd = new("SELECT id FROM accounts WHERE email = @email LIMIT 1", conn);
        //        cmd.Parameters.AddWithValue("@email", email);
        //        NpgsqlDataReader rdr = cmd.ExecuteReader();
        //        if (rdr.HasRows)
        //            error = true;
        //        rdr.Close();
        //        Database.GetInstance().FreeConnection(conn);
        //
        //        if (error)
        //        {
        //            player.SendChatMessage("{FF0000}Es existiert bereits ein Account mit dieser Email!");
        //            return;
        //        }
        //    }
        //
        //    string password_hash = BCrypt.Net.BCrypt.HashPassword(password);
        //
        //    {
        //        NpgsqlCommand cmd = new("INSERT INTO accounts (email, password_hash) VALUES (@email, @hash)");
        //        cmd.Parameters.AddWithValue("@email", email);
        //        cmd.Parameters.AddWithValue("@hash", password_hash);
        //        Database.ExecuteNonQuery(cmd);
        //    }
        //
        //    player.SendChatMessage("Account wurde angelegt! Nutze den login Befehl um dich anzumelden!");
        //}

        //[Command("rcon")]
        //public static void CMD_RCON(CPlayer player, string password)
        //{
        //    if (password != "DieVomMotel")
        //    {
        //        player.SendChatMessage("{FF0000}Dies ist das falsche Passwort!");
        //        return;
        //    }
        //
        //    player.SendChatMessage(player.SetAdmin(6) ? $"Du bist nun als Level 6 Admin angemeldet!" : "{FF0000}Etwas ist schief gelaufen!");
        //}
    }
}
