using AltV.Net;
using AltV.Net.Elements.Entities;

namespace NAMERP.House
{
    public class Events : IScript
    {
        [ClientEvent("house:get")]
        public static void CE_GET_HOUSE(IPlayer player)
        {
            (int id, int owner, int price, bool locked) = API.GetHouseInfoByPosition(player.Position);
            if (id == 0)
                return;

            player.Emit("house:get", id, owner, price, locked);
        }

        [ClientEvent("house:buy")]
        public static void CE_BUY_HOUSE(CPlayer player, int id) => API.BuyHouse(player, id);

        //[ClientEvent("house:sell")]
        //public static void CE_SELL_HOUSE() => API.SellHouse();

        [ClientEvent("house:enter")]
        public static void CE_ENTER_HOUSE(IPlayer player, int id) => API.EnterHouse(player, id);

        [ClientEvent("house:leave")]
        public static void CE_LEAVE_HOUSE(IPlayer player) => API.LeaveHouse(player);

        [ClientEvent("house:lock")]
        public static void CE_LOCK_HOUSE(CPlayer player, int id) => API.LockHouse(player, id);
    }
}
