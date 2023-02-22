using AltV.Net;
using AltV.Net.Elements.Entities;
using AltV.Net.Resources.Chat.Api;

namespace NAMERP.Player.Admin
{
    internal class Commands : IScript
    {
        [Command("createhouse")]
        public static void CMD_CreateHouse(CPlayer player, int price, short interior)
        {
            if (!player.IsAdmin(1))
                return;

            House.API.CreateHouse(player.Position, price, interior);
        }

        [Command("createveh")]
        public static void CMD_CreateVehicle(CPlayer player, uint model)
        {
            if (!player.IsAdmin(1))
                return;

            IVehicle veh = Alt.CreateVehicle(model, player.Position, player.Rotation);
            if (veh != null)
            {
                player.SetIntoVehicle(veh, 1);
                player.SendChatMessage($"{veh.Position.X}, {veh.Position.Y}, {veh.Position.Z}");
                player.SendChatMessage($"{veh.Rotation.Yaw}, {veh.Rotation.Pitch}, {veh.Rotation.Roll}");
            }
        }
    }
}
