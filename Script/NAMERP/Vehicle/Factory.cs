using AltV.Net.Elements.Entities;
using AltV.Net;

namespace NAMERP.Vehicle
{
    public class Factory : IEntityFactory<IVehicle>
    {
        public IVehicle Create(ICore core, IntPtr entityPointer, ushort id)
        {
            return new CVehicle(core, entityPointer, id);
        }
    }
}
