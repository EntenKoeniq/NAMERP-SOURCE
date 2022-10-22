using AltV.Net;
using AltV.Net.Elements.Entities;
using AltV.Net.Enums;

using Npgsql;

namespace NAMERP
{
    public class Server : Resource
    {
        public static uint LastWeather = (uint)WeatherType.Clear;
        public static DateTime LastWeatherUpdate = DateTime.Now;
        private static readonly WeatherType[] _weatherTypes = {
                            WeatherType.Clear,
                            WeatherType.Clouds,
                            WeatherType.Rain,
                            WeatherType.Thunder,
                            WeatherType.ExtraSunny
                        };

        public override void OnStart()
        {
            Alt.LogInfo("Verbindung zur Datenbank wird hergestellt...");
            Database.GetInstance();

            Alt.LogInfo("Datenbank wird aufgeräumt...");
            {
                NpgsqlCommand cmd = new("DELETE FROM online");
                Database.ExecuteNonQuery(cmd);
            }

            Alt.LogInfo("Häuser werden geladen...");
            House.API.LoadAllHouses();

            Alt.LogInfo("Datum und Wetter werden angepasst...");
            Task.Run(() =>
            {
                while (true)
                {
                    DateTime currentTime = DateTime.Now;
                    
                    // Update every 20 minutes
                    if ((currentTime - LastWeatherUpdate).TotalMinutes > 20)
                    {
                        
                        Random rand = new();
                        uint randWeather = (uint)rand.Next(_weatherTypes.Length);
                        LastWeather = randWeather;

                        Alt.GetAllPlayers().All(player =>
                        {
                            player.SetDateTime(1, 1, 2022, currentTime.Hour, currentTime.Minute, 1);
                            player.SetWeather(randWeather);
                            return true;
                        });

                        LastWeatherUpdate = currentTime;
                    }

                    // Update every 60 seconds
                    Task.Delay(1000 * 60 * 1).Wait();
                }
            });
        }

        public override void OnStop()
        {
            Alt.LogWarning("Server wird gestoppt!");
            Alt.LogWarning("Spieler Fahrzeuge werden gespeichert...");
            foreach (CPlayer player in Alt.GetAllPlayers().Cast<CPlayer>())
            {
                Vehicle.API.SavePlayerVehicles(player.ID);
            }
        }

        public override IEntityFactory<IPlayer> GetPlayerFactory()
        {
            return new Account.Factory();
        }

        public override IEntityFactory<IVehicle> GetVehicleFactory()
        {
            return new Vehicle.Factory();
        }
    }
}
