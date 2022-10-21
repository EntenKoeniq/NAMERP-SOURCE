using AltV.Net.Data;
using AltV.Net.Elements.Entities;

namespace NAMERP.House
{
    internal class House
    {
        public int ID { get; set; } = 0;
        public Position Location { get; set; } = new();
        public int Price { get; set; } = 0;
        public int Owner { get; set; } = 0;
        public bool Locked { get; set; } = false;
        public short Interior { get; set; } = 0;

        public IBlip? Blip { get; set; } = null;
    }
}
