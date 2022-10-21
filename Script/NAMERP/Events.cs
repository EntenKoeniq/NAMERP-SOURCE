using System.Numerics;

using AltV.Net;
using AltV.Net.Data;
using AltV.Net.Elements.Entities;
using AltV.Net.Enums;
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

            Vehicle.API.SavePlayerVehicles(player.ID);

            if (!Account.Helper.AccountAlreadyOnline(player.Email))
                return;
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

        [ScriptEvent(ScriptEventType.PlayerChangeVehicleSeat)]
        public static void OnPlayerChangeVehicleSeat(IVehicle _1, CPlayer player, byte _2, byte newSeat)
        {
            player.Emit("vehicle:enter", newSeat == 1);
        }

        [ScriptEvent(ScriptEventType.PlayerEnterVehicle)]
        public static void OnPlayerEnterVehicle(IVehicle _, CPlayer player, byte seat)
        {
            player.Emit("vehicle:enter", seat == 1);
        }

        [ScriptEvent(ScriptEventType.PlayerLeaveVehicle)]
        public static void OnPlayerLeaveVehicle(IVehicle _1, CPlayer player, byte _2)
        {
            player.Emit("vehicle:leave");
        }

        [ClientEvent("vehicle:toggleEngine")]
        public static void CE_VEHICLE_TOGGLE_ENGINE(CPlayer player)
        {
            if (!player.IsInVehicle)
                return;
            CVehicle veh = (CVehicle)player.Vehicle;
            if (veh == null)
                return;
            if (!veh.HasKey(player))
            {
                player.SendChatMessage("{FF0000}Du besitzt keinen Schlüssel!");
                return;
            }

            veh.EngineOn = !veh.EngineOn;
        }

        [ClientEvent("vehicle:lock")]
        public static void CE_VEHICLE_LOCK(CPlayer player)
        {
            CVehicle? veh = null;

            if (player.IsInVehicle)
            {
                veh = (CVehicle)player.Vehicle;
                if (!veh.HasKey(player))
                {
                    player.SendChatMessage("{FF0000}Du besitzt keinen Schlüssel!");
                    return;
                }
            }
            else
                veh = Alt.GetAllVehicles()
                         .Cast<CVehicle>()
                         .Where(res => res.HasKey(player) && player.Position.Distance(res.Position) < 5f)
                         .OrderBy(res => player.Position.Distance(res.Position))
                         .FirstOrDefault();

            if (veh != null)
            {
                veh.LockState = veh.LockState == VehicleLockState.Locked ? VehicleLockState.Unlocked : VehicleLockState.Locked;
                IPlayer[] targets = Alt.GetAllPlayers()
                                       .Where(target => Vector3.Distance(target.Position, veh.Position) < 50)
                                       .ToArray();
                Alt.EmitClients(targets, "vehicle:indicatorLights", veh.Id, 8, 750);
            }
        }

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

                Vehicle.API.LoadPlayerVehicles(player.ID);
                player.Position = new(0, 0, 0);
            }
        }
    }
}
