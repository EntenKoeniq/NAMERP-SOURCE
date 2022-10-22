using AltV.Net;
using AltV.Net.Data;
using AltV.Net.Elements.Entities;
using AltV.Net.Resources.Chat.Api;

using Npgsql;

namespace NAMERP
{
    public class Events : IScript
    {
        private static readonly Random _random = new();

        [ScriptEvent(ScriptEventType.PlayerConnect)]
        public static void OnPlayerConnect(CPlayer player, string _)
        {
            Alt.LogInfo($"Spieler {player.Name} ({player.Ip}) hat sich verbunden!");
            player.Dimension = int.MaxValue - _random.Next(999999);
            player.Spawn(new Position(344.3341f, -998.8612f, -99.19622f), 0);
            player.SetDateTime(1, 1, 2022, Server.LastWeatherUpdate.Hour, Server.LastWeatherUpdate.Minute, 1);
            player.SetWeather(Server.LastWeather);
        }

        [ScriptEvent(ScriptEventType.PlayerDisconnect)]
        public static void OnPlayerDisconnect(CPlayer player, string _)
        {
            if (!player.LoggedIn)
                return;

            Vehicle.API.SavePlayerVehicles(player.ID, true);

            NpgsqlCommand cmd = new("DELETE FROM online WHERE account_id = @id");
            cmd.Parameters.AddWithValue("@id", player.ID);
            Database.ExecuteNonQuery(cmd);
        }

        [ScriptEvent(ScriptEventType.PlayerDead)]
        public static void OnPlayerDead(CPlayer player, IEntity _1, uint _2)
        {
            player.SendChatMessage("{FFCC00}Du bist gestorben! Du wirst in 3 Sekunden wiederbelebt!");
            Task.Delay(3000).ContinueWith(_ => player.Spawn(new Position(-425, 1123, 325), 0));
        }
    }
}
