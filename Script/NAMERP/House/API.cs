using Newtonsoft.Json;

using AltV.Net;
using AltV.Net.Data;
using AltV.Net.Elements.Entities;

using Npgsql;

namespace NAMERP.House
{
    internal static class API
    {
        private static readonly List<House> _houseList = new();
        private static readonly Position[] _interiors = new Position[]
        {
            new Position(-614.86f, 40.6783f, 97.60007f),        // Hochhaus Gehoben - 0
            new Position(152.2605f, -1004.471f, -98.99999f),    // Low Low End Apartment - 1
            new Position(261.4586f, -998.8196f, -99.00863f),    // Low End Apartment - 2
            new Position(347.2686f, -999.2955f, -99.19622f),    // Medium End Apartment - 3
            new Position(-1477.14f, -538.7499f, 55.5264f),      // Hochhaus Sehr Gehoben - 4
            new Position(-169.286f, 486.4938f, 137.4436f),      // Sehr Gehoben Hills 1 - 5
            new Position(340.9412f, 437.1798f, 149.3925f),      // Sehr Gehoben Hills 2 - 6
            new Position(373.023f, 416.105f, 145.7006f),        // Sehr Gehoben Hills 3 - 7
            new Position(-676.127f, 588.612f, 145.1698f),       // Sehr Gehoben Hills 4 - 8
            new Position(-763.107f, 615.906f, 144.1401f),       // Sehr Gehoben Hills 5 - 9
            new Position(-857.798f, 682.563f, 152.6529f),       // Sehr Gehoben Hills 6 - 10
            new Position(-1288, 440.748f, 97.69459f),           // Sehr Gehoben Hills 7 - 11
            new Position(1397.072f, 1142.011f, 114.3335f)       // Farm Ultra Luxus - 12
        };

        /// <summary>
        /// Load all houses for a specific player
        /// </summary>
        /// <param name="player">The player receiving the blips</param>
        public static void LoadAllHouses(IPlayer player)
        {
            _houseList.ForEach((el) =>
            {
                IBlip blip = Alt.CreateBlip(player, (byte)BlipType.Destination, el.Location);
                blip.Sprite = 411; // House
                blip.Color = (byte)(el.Owner == 0 ? 1 : 2); // House for sell?
                blip.Name = "Haus";
            });
        }

        /// <summary>
        /// Load all houses from the database
        /// </summary>
        public static void LoadAllHouses()
        {
            NpgsqlConnection conn = Database.GetInstance().GetConnection();
            NpgsqlCommand cmd = new("SELECT * FROM houses", conn);
            NpgsqlDataReader rdr = cmd.ExecuteReader();
            while (rdr.Read())
            {
                _houseList.Add(new()
                {
                    ID = rdr.GetInt32(0),
                    Location = new(rdr.GetFloat(1), rdr.GetFloat(2), rdr.GetFloat(3)),
                    Price = rdr.GetInt32(4),
                    Owner = rdr.GetInt32(5),
                    Locked = rdr.GetBoolean(6),
                    Interior = rdr.GetInt16(7)
                });
            }
            rdr.Close();
            Database.GetInstance().FreeConnection(conn);
        }

        /// <summary>
        /// Create a new house (NOT DONE YET!)
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="price"></param>
        /// <param name="interior"></param>
        public static void CreateHouse(Position pos, int price, short interior)
        {
            NpgsqlCommand cmd = new("INSERT INTO houses (p_x, p_y, p_z, price, interior) VALUES (@p_x, @p_y, @p_z, @price, @interior)");
            cmd.Parameters.AddWithValue("@p_x", pos.X);
            cmd.Parameters.AddWithValue("@p_y", pos.Y);
            cmd.Parameters.AddWithValue("@p_z", pos.Z);
            cmd.Parameters.AddWithValue("@price", price);
            cmd.Parameters.AddWithValue("@interior", interior);
            Database.ExecuteNonQuery(cmd);

            Alt.LogInfo($"Es wurde ein neues Haus hinzugefügt auf folgender Position: [{pos.X}, {pos.Y}, {pos.Z}]");
        }

        public static void DeleteHouse()
        {
            throw new NotImplementedException();
        }

        public static void EnterHouse()
        {
            throw new NotImplementedException();
        }

        public static void LeaveHouse()
        {
            throw new NotImplementedException();
        }

        public static void LockHouse()
        {
            throw new NotImplementedException();
        }

        public static void BuyHouse()
        {
            throw new NotImplementedException();
        }

        public static void SellHouse()
        {
            throw new NotImplementedException();
        }

        public static void SetNewOwner()
        {
            throw new NotImplementedException();
        }
    }
}
