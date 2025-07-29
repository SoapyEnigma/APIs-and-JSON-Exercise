namespace APIsAndJSON
{
    public class Program
    {
        static async Task Main(string[] args)
        {
            var client = new HttpClient();

            do
            {
                Console.WriteLine("Would you like to see a conversation between Kanye and Ron Swanson\n"
                    + "or would you like to know the temperate of a city of your choice inside of the US?\n"
                    + "[1] Conversation\n"
                    + "[2] Weather\n"
                    + "[3] Exit");

                string input = Console.ReadLine();
                while (!input.Equals("1") && !input.Equals("2") && !input.Equals("3"))
                {
                    Console.WriteLine("Please only input 1, 2, or 3");
                    input = Console.ReadLine();
                }

                if (input.Equals("3"))
                    break;

                if (input.Equals("1"))
                {
                    RonVSKanyeAPI api = new(client);
                    await api.StartConversation();
                }
                else if (input.Equals("2"))
                {
                    OpenWeatherMapAPI api = new(client);
                    await api.GetWeather();
                }

                Console.WriteLine("Would you like to start over or would you like to exit?\n"
                    + "[1] Start Over\n"
                    + "[2] Exit");

                input = Console.ReadLine();
                while (!input.Equals("1") && !input.Equals("2"))
                {
                    Console.WriteLine("Please only input 1 or 2");
                    input = Console.ReadLine();
                }

                if (input.Equals("2"))
                    break;

                Console.Clear();

            } while (true);
        }
    }
}
