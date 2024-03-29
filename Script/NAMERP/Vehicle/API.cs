﻿using System.Diagnostics;
using System.Runtime.Intrinsics.Arm;
using AltV.Net;
using AltV.Net.Data;
using AltV.Net.Elements.Entities;
using AltV.Net.Enums;

using Npgsql;

namespace NAMERP.Vehicle
{
    internal static class API
    {
        public static void LoadPlayerVehicles(int owner)
        {
            NpgsqlConnection conn = Database.GetInstance().GetConnection();
            NpgsqlCommand cmd = new("SELECT v.*, c.hash, c.multi, c.fuel_tank, c.fuel_consumption FROM vehicles v JOIN cfg_vehicles c ON v.cfg_vehicle_id = c.id WHERE v.owner = @owner", conn);
            cmd.Parameters.AddWithValue("@owner", owner);
            NpgsqlDataReader rdr = cmd.ExecuteReader();
            if (rdr.HasRows)
            {
                while (rdr.Read())
                {
                    if (!rdr.GetBoolean(6)) // Destroyed?
                        continue;

                    CVehicle? veh = (CVehicle?)CreateVehicle((uint)rdr.GetInt64(18), new(rdr.GetFloat(8), rdr.GetFloat(9), rdr.GetFloat(10)), new(rdr.GetFloat(11), rdr.GetFloat(12), rdr.GetFloat(13)));
                    if (veh == null)
                        continue;
                    veh.Dimension = rdr.GetInt32(14);
                    int vehID = rdr.GetInt32(0);
                    veh.ID = vehID;
                    veh.Owner = owner;
                    veh.Organization = 0;
                    veh.Family = 0;
                    veh.EngineOn = rdr.GetBoolean(5);
                    veh.LockState = rdr.GetBoolean(7) ? VehicleLockState.Locked : VehicleLockState.Unlocked;
                    veh.SetSyncedMetaData("id", vehID);
                    veh.SetSyncedMetaData("fuel", rdr.GetFloat(17));
                    veh.SetSyncedMetaData("fuel_tank", rdr.GetFloat(20));
                    veh.SetSyncedMetaData("fuel_consumption", rdr.GetFloat(21));
                    veh.SetSyncedMetaData("multi", rdr.GetInt32(19));
                }
            }
            rdr.Close();
            Database.GetInstance().FreeConnection(conn);
        }

        public static IVehicle? CreateVehicle(uint hash, Position pos, Rotation rot)
        {
            IVehicle veh = Alt.CreateVehicle(hash, pos, rot);
            if (veh == null)
                return null;

            veh.ManualEngineControl = true;
            veh.EngineOn = false;
            veh.LockState = VehicleLockState.Locked;
            veh.NumberplateText = "NAMERP";

            return veh;
        }

        private static void SaveVehicle(ref CVehicle veh)
        {
            veh.GetSyncedMetaData("fuel", out float vehFuel);

            NpgsqlCommand cmd = new("UPDATE vehicles SET engine = @engine, alive = @alive, locked = @locked, p_x = @p_x, p_y = @p_y, p_z = @p_z, r_r = @r_r, r_p = @r_p, r_y = @r_y, dim = @dim, fuel = @fuel WHERE id = @id");
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
            cmd.Parameters.AddWithValue("@fuel", vehFuel);
            Database.ExecuteNonQuery(cmd);
        }

        public static void SavePlayerVehicles(int id, bool delete = false)
        {
            CVehicle[] vehs = Alt.GetAllVehicles().Cast<CVehicle>().Where(res => res.Owner == id).ToArray();
            for (int i = 0; i < vehs.Length; i++)
            {
                SaveVehicle(ref vehs[i]);
                if (delete)
                    vehs[i].Remove();
            }
        }

        public static void SaveAllVehicles()
        {
            CVehicle[] vehs = Alt.GetAllVehicles().Cast<CVehicle>().ToArray();
            for (int i = 0; i < vehs.Length; i++)
                SaveVehicle(ref vehs[0]);
        }
    }
}
