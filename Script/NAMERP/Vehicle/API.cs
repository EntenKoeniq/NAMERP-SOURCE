using AltV.Net;
using AltV.Net.Enums;

using Npgsql;

namespace NAMERP.Vehicle
{
    internal static class API
    {
        public static void LoadPlayerVehicles(int id)
        {
            NpgsqlConnection conn = Database.GetInstance().GetConnection();
            NpgsqlCommand cmd = new("SELECT v.*, c.hash, c.multi, c.fuel_tank, c.fuel_consumption FROM vehicles v JOIN cfg_vehicles c ON v.cfg_vehicle_id = c.id WHERE v.owner = @id", conn);
            cmd.Parameters.AddWithValue("@id", id);
            NpgsqlDataReader rdr = cmd.ExecuteReader();
            if (rdr.HasRows)
            {
                while (rdr.Read())
                {
                    if (!rdr.GetBoolean(6)) // Destroyed?
                        continue;

                    CVehicle veh = (CVehicle)Alt.CreateVehicle((uint)rdr.GetInt64(18), new(rdr.GetFloat(8), rdr.GetFloat(9), rdr.GetFloat(10)), new(rdr.GetFloat(11), rdr.GetFloat(12), rdr.GetFloat(13)));
                    veh.Dimension = rdr.GetInt32(14);
                    veh.ID = rdr.GetInt32(0);
                    veh.Owner = id;
                    veh.Organization = 0;
                    veh.Family = 0;
                    veh.ManualEngineControl = true;
                    veh.EngineOn = rdr.GetBoolean(5);
                    veh.LockState = rdr.GetBoolean(7) ? VehicleLockState.Locked : VehicleLockState.Unlocked;
                    veh.NumberplateText = "ALT:RP";
                }
            }
            rdr.Close();
            Database.GetInstance().FreeConnection(conn);
        }

        public static void SavePlayerVehicles(int id)
        {
            foreach (CVehicle veh in Alt.GetAllVehicles().Cast<CVehicle>().Where(res => res.Owner == id))
            {
                NpgsqlCommand cmd = new("UPDATE vehicles SET engine = @engine, alive = @alive, locked = @locked, p_x = @p_x, p_y = @p_y, p_z = @p_z, r_r = @r_r, r_p = @r_p, r_y = @r_y, dim = @dim WHERE id = @id");
                cmd.Parameters.AddWithValue("@engine", veh.EngineOn);
                cmd.Parameters.AddWithValue("@alive", !veh.IsDestroyed);
                cmd.Parameters.AddWithValue("@locked", veh.LockState == VehicleLockState.Locked);
                cmd.Parameters.AddWithValue("@p_x", veh.Position.X);
                cmd.Parameters.AddWithValue("@p_y", veh.Position.Y);
                cmd.Parameters.AddWithValue("@p_z", veh.Position.Z);
                cmd.Parameters.AddWithValue("@r_r", veh.Rotation.Roll);
                cmd.Parameters.AddWithValue("@r_p", veh.Rotation.Pitch);
                cmd.Parameters.AddWithValue("@r_y", veh.Rotation.Yaw);
                cmd.Parameters.AddWithValue("@dim", veh.Dimension);
                Database.ExecuteNonQuery(cmd);

                veh.Remove();
            }
        }
    }
}
