using AltV.Net.Data;

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
    }
}
