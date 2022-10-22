using System.Numerics;

using AltV.Net.Elements.Entities;
using AltV.Net.Enums;
using AltV.Net.Resources.Chat.Api;
using AltV.Net;

namespace NAMERP.Vehicle
{
    public class Events : IScript
    {
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
                         .Where((el) => el.HasKey(player) && player.Position.Distance(el.Position) < 5f)
                         .OrderBy((el) => player.Position.Distance(el.Position))
                         .FirstOrDefault();

            if (veh != null)
            {
                veh.LockState = veh.LockState == VehicleLockState.Locked ? VehicleLockState.Unlocked : VehicleLockState.Locked;
                IPlayer[] targets = Alt.GetAllPlayers()
                                       .Where((el) => Vector3.Distance(el.Position, veh.Position) < 50)
                                       .ToArray();
                Alt.EmitClients(targets, "vehicle:indicatorLights", veh.Id, 8, 750);
            }
        }
    }
}
