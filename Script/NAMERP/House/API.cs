using AltV.Net;
using AltV.Net.Data;
using AltV.Net.Elements.Entities;
using AltV.Net.Resources.Chat.Api;

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

        private static IBlip CreateBlip(Position position, int owner)
        {
            IBlip blip = Alt.CreateBlip((byte)BlipType.Destination, position);
            blip.Sprite = 411; // House
            blip.Color = (byte)(owner == 0 ? 1 : 2); // House for sell?
            blip.Name = "Haus";
            return blip;
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
                    Interior = rdr.GetInt16(7),
                    Blip = CreateBlip(new(rdr.GetFloat(1), rdr.GetFloat(2), rdr.GetFloat(3)), rdr.GetInt32(5))
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

        public static (int, int, int, bool) GetHouseInfoByPosition(Position position)
        {
            House? house = _houseList.Where((el) => position.Distance(el.Location) <= 2f).OrderBy((el) => position.Distance(el.Location)).FirstOrDefault();
            return house == null ? (0, 0, 0, false) : (house.ID, house.Owner, house.Price, house.Locked);
        }

        /// <summary>
        /// Let the player enter a house by its ID
        /// </summary>
        /// <param name="player"></param>
        /// <param name="houseID"></param>
        public static void EnterHouse(IPlayer player, int houseID)
        {
            House? house = _houseList.Where((el) => el.ID == houseID).FirstOrDefault();
            if (house == null || house.Locked)
                return;

            player.Dimension = house.ID;
            player.Position = _interiors[house.Interior];
        }

        /// <summary>
        /// Let the player get out of a house
        /// </summary>
        /// <param name="player"></param>
        public static void LeaveHouse(IPlayer player)
        {
            // He isn't in any house
            if (player.Dimension == 0)
                return;

            House? house = _houseList.Where((el) => el.ID == player.Dimension).FirstOrDefault();
            if (house == null)
                return;

            player.Dimension = 0;
            player.Position = house.Location;
        }

        /// <summary>
        /// Lock a house by its ID
        /// </summary>
        /// <param name="houseID"></param>
        public static void LockHouse(CPlayer player, int houseID)
        {
            House? house = _houseList.Where((el) => el.ID == houseID).FirstOrDefault();
            if (house == null || house.Owner != player.ID)
                return;

            house.Locked = !house.Locked;

            NpgsqlCommand cmd = new("UPDATE houses SET locked = @locked WHERE id = @id");
            cmd.Parameters.AddWithValue("@locked", house.Locked);
            cmd.Parameters.AddWithValue("@id", house.ID);
            Database.ExecuteNonQuery(cmd);
        }

        /// <summary>
        /// Buy a house by its ID
        /// </summary>
        /// <param name="player"></param>
        /// <param name="houseID"></param>
        public static void BuyHouse(CPlayer player, int houseID)
        {
            House? house = _houseList.Where((el) => el.ID == houseID).FirstOrDefault();
            if (house == null)
                return;

            // House isn't for sell
            if (house.Owner != 0)
                return;

            if (!player.SetMoney(house.Price, false))
            {
                player.SendChatMessage("{FF0000}Du hast nicht ausreichend Geld auf der Hand!");
                return;
            }

            if (!SetOwner(player.ID, house.ID))
            {
                // Give the player their money back
                player.SetMoney(house.Price, true);
                player.SendChatMessage("{FF0000}Bitte melde dich im Support! Etwas ist beim Kauf schief gelaufen!");
            }
        }

        /// <summary>
        /// NOT IMPLEMENTED YET!
        /// </summary>
        /// <exception cref="NotImplementedException"></exception>
        public static void SellHouse()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Set a new house owner
        /// </summary>
        /// <param name="playerID"></param>
        /// <param name="houseID"></param>
        /// <returns></returns>
        public static bool SetOwner(int playerID, int houseID)
        {
            House? house = _houseList.Where((el) => el.ID == houseID).FirstOrDefault();
            if (house == null)
                return false;

            NpgsqlCommand cmd = new("UPDATE houses SET owner = @owner WHERE id = @id");
            cmd.Parameters.AddWithValue("@owner", playerID);
            cmd.Parameters.AddWithValue("@id", houseID);
            Database.ExecuteNonQuery(cmd);

            house.Owner = playerID;
            if (house.Blip != null)
                house.Blip.Color = (byte)(house.Owner == 0 ? 1 : 2);

            return true;
        }
    }
}
