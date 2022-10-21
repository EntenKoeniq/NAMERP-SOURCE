using Npgsql;

namespace NAMERP.Account
{
    internal class Helper
    {
        public static bool AccountAlreadyOnline(string email)
        {
            bool found = false;
            NpgsqlConnection conn = Database.GetInstance().GetConnection();
            NpgsqlCommand cmd = new("SELECT o.account_id FROM online o JOIN accounts a ON o.account_id = a.id WHERE a.email = @email LIMIT 1", conn);
            cmd.Parameters.AddWithValue("@email", email);
            NpgsqlDataReader rdr = cmd.ExecuteReader();
            if (rdr.HasRows)
                found = true;
            rdr.Close();
            Database.GetInstance().FreeConnection(conn);
            return found;
        }
    }
}
