using AltV.Net;
using AltV.Net.Resources.Chat.Api;

using Npgsql;

namespace NAMERP
{
    public class CPlayer : AltV.Net.Elements.Entities.Player
    {
        public bool LoggedIn { get; set; } = false;

        public int ID { get; set; } = 0;
        public string Email { get; set; } = string.Empty;
        public int Money { get; set; } = 75000;
        public bool HasMoney(int value) => Money >= value;
        public bool SetMoney(int value, bool add, bool update = true)
        {
            if (!LoggedIn)
                return false;

            if (add)
                Money += value;
            else if (HasMoney(value))
                Money -= value;
            else
                return false;

            if (update)
            {
                NpgsqlCommand cmd = new("UPDATE accounts SET money = @money WHERE id = @id");
                cmd.Parameters.AddWithValue("@money", Money);
                cmd.Parameters.AddWithValue("@id", ID);
                Database.ExecuteNonQuery(cmd);
            }

            Emit("hud:updateMoney", Money);
            return true;
        }
        public int Bank { get; set; } = 0;
        public bool SetBank(int value, bool add, bool update = true)
        {
            if (!LoggedIn)
                return false;

            if (add)
                Bank += value;
            else if (HasMoney(value))
                Bank -= value;
            else
                return false;

            if (update)
            {
                NpgsqlCommand cmd = new("UPDATE accounts SET bank = @bank WHERE id = @id");
                cmd.Parameters.AddWithValue("@bank", Money);
                cmd.Parameters.AddWithValue("@id", ID);
                Database.ExecuteNonQuery(cmd);
            }

            Emit("hud:updateBank", Money);
            return true;
        }
        public byte Admin { get; set; } = 0;
        public bool IsAdmin(byte level, bool needed = false)
        {
            if (!LoggedIn)
                return false;

            if (Admin < level)
            {
                if (needed)
                    this.SendChatMessage("{FF0000}Du besitzt nicht die benötigten Berechtigungen!");
                return false;
            }

            return true;
        }
        public bool SetAdmin(byte level)
        {
            if (!LoggedIn)
                return false;

            NpgsqlCommand cmd = new("UPDATE accounts SET admin = @admin WHERE id = @id");
            cmd.Parameters.AddWithValue("@admin", level);
            cmd.Parameters.AddWithValue("@id", ID);
            Database.ExecuteNonQuery(cmd);

            Admin = level;
            return true;
        }
        public short Organization { get; set; } = 0;
        public short OrganizationRank { get; set; } = 0;
        public short Family { get; set; } = 0;
        public short FamilyRank { get; set; } = 0;

        public CPlayer(ICore core, IntPtr nativePointer, ushort id) : base(core, nativePointer, id) { }
    }
}
