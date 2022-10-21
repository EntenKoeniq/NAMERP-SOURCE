using AltV.Net;

namespace NAMERP
{
    public class CVehicle : AltV.Net.Elements.Entities.Vehicle
    {
        public int ID { get; set; } = 0;
        public int Owner { get; set; } = 0;
        public short Organization { get; set; } = 0;
        public short OrganizationRank { get; set; } = 0;
        public short Family { get; set; } = 0;
        public short FamilyRank { get; set; } = 0;
        public int[] Keys { get; set; } = Array.Empty<int>();

        public CVehicle(ICore core, IntPtr nativePointer, ushort id) : base(core, nativePointer, id) { }

        public bool HasKey(CPlayer player) => player.ID == Owner                                                                    ||
                                              Keys.Contains(player.ID)                                                              ||
                                              player.Family == Family && player.FamilyRank >= FamilyRank                            ||
                                              player.Organization == Organization && player.OrganizationRank >= OrganizationRank;
    }
}
