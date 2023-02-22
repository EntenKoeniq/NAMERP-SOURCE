using AltV.Net;
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
    }
}
