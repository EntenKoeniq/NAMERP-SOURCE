using AltV.Net;
using AltV.Net.Elements.Entities;

namespace NAMERP.Account
{
    public class Factory : IEntityFactory<IPlayer>
    {
        public IPlayer Create(ICore core, IntPtr entityPointer, ushort id)
        {
            return new CPlayer(core, entityPointer, id);
        }
    }
}
