using Newtonsoft.Json.Linq;

namespace APIsAndJSON
{
    public class OpenWeatherMapAPI
    {
        private HttpClient _client;
        private readonly string _apiKey = ""; // Shouldn't share api keys
        private string _url;
        private Location _loc;

        private struct Location
        {
            public string City { get; set; }
            public string State { get; set; }
            public string Country { get; set; }

            public string Longitude { get; set; }
            public string Latitude { get; set; }
        }

        public OpenWeatherMapAPI(HttpClient client)
        {
            _client = client;
        }

        public async Task GetWeather()
        {
            SetLocation();
            await ValidateLocation();
            await OutputWeatherData();
        }

        private void SetLocation()
        {
            string city = "";
            string state = "";
            string country = "US"; // Currently only support US

            Console.WriteLine("Please enter the name of a city:");
            while (string.IsNullOrEmpty(city))
                city = Console.ReadLine();

            Console.WriteLine("Please enter the 2 character state code for a state (ex: AL):");
            while (string.IsNullOrEmpty(state))
                state = Console.ReadLine();

            _loc = new Location()
            {
                City = city,
                State = state,
                Country = country
            };
        }

        private async Task ValidateLocation()
        {
            SetGeoURL();
            await ValidateGeoLocation();
            SetDataURL();
        }

        private async Task OutputWeatherData()
        {
            var jsonStr = await GetWeatherData();

            if (string.IsNullOrWhiteSpace(jsonStr) || jsonStr.Equals("{}"))
            {
                Console.WriteLine("Failed to retrieve weather data.\n"
                    + "Press enter to go back");

                Console.ReadLine();

                return;
            }

            var json = JObject.Parse(jsonStr);
            var temp = json.GetValue("main")["temp"].ToString();

            Console.WriteLine($"It is currntly {temp} degrees fahrenheit in {_loc.City}, {_loc.State}.");
        }

        private async Task<string> GetWeatherData()
        {
            try
            {
                return await _client.GetStringAsync(_url);
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine("GetWeatherData()::There was a problem with the HTTP Request:\n"
                    + e.Message);

                return "{}";
            }
        }

        private void SetDataURL()
        {
            _url = "https://api.openweathermap.org/data/2.5/weather?"
                + $"lat={_loc.Latitude}&lon={_loc.Longitude}&appid={_apiKey}&units=imperial";
        }

        private void SetGeoURL()
        {
            _url = "http://api.openweathermap.org/geo/1.0/direct?q="
                + $"{_loc.City},{_loc.State},{_loc.Country}&limit=1&appid={_apiKey}";
        }

        private async Task ValidateGeoLocation()
        {
            var jsonStr = await GetGeoData();

            if (string.IsNullOrWhiteSpace(jsonStr) || jsonStr.Equals("{}"))
            {
                Console.WriteLine("Failed to retrieve geo data.\n"
                    + "Press enter to go back");

                Console.ReadLine();

                return;
            }

            var jsonArr = JArray.Parse(jsonStr);
            var json = jsonArr[0];
            _loc.Latitude = json["lat"].ToString();
            _loc.Longitude = json["lon"].ToString();
            // v-- Override input info with retrieved info --v \\
            _loc.City = json["name"].ToString();
            _loc.State = json["state"].ToString();
        }

        private async Task<string> GetGeoData()
        {
            try
            {
                return await _client.GetStringAsync(_url);
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine("GetGeoData()::There was a problem with the HTTP Request:\n"
                    + e.Message);

                return "{}";
            }
        }
    }
}
