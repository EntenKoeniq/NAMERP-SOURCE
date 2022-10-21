using AltV.Net;

namespace NAMERP
{
    internal class Chat : IScript
    {
        [ClientEvent("chat:message")]
        public static void EVENT_ChatMessage(CPlayer player, string msg)
        {
            if (msg == string.Empty || msg[0] == '/')
                return;

            Alt.EmitAllClients("chat:message", player.Name, msg);
        }
    }
}
