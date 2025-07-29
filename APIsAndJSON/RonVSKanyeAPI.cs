using Newtonsoft.Json.Linq;

namespace APIsAndJSON
{
    public class RonVSKanyeAPI
    {
        private HttpClient _client;
        private readonly string _yeURL = "https://api.kanye.rest/";
        private readonly string _ronURL = "https://ron-swanson-quotes.herokuapp.com/v2/quotes";
        private bool _isRunning = false;
        private int _exchangesTotal = 0;

        public RonVSKanyeAPI(HttpClient client)
        {
            _client = client;
        }

        public async Task StartConversation()
        {
            Console.WriteLine("Starting Kanye vs. Ron conversation...");

            Console.WriteLine("how many exchanges (each person makes a comment) would you like to see?");

            int exchangeLimit;
            while (!int.TryParse(Console.ReadLine(), out exchangeLimit))
                Console.WriteLine("Please input a valid integer.");

            Console.WriteLine("\nType '1' and press Enter to end the conversation early.\n");

            var cts = new CancellationTokenSource();
            var inputTask = MonitorInput(cts.Token);

            var conversationTask = Conversation(exchangeLimit);
            await conversationTask;

            cts.Cancel();
            await inputTask;

            Console.WriteLine("\nConversation ended.\n");
        }

        private async Task Conversation(int exchangeCount)
        {
            _exchangesTotal = 0;

            _isRunning = true;
            while (_isRunning && _exchangesTotal++ < exchangeCount)
            {
                Console.WriteLine($"Kanye: {await GetYeQuote()}");
                await Task.Delay(1000);
                Console.WriteLine($"Ron: {await GetRonQuote()}");
                await Task.Delay(1000);
            }

            EndConversation();
        }

        private void EndConversation()
        {
            _isRunning = false;
            _exchangesTotal = 0;
        }

        private Task MonitorInput(CancellationToken token)
        {
            return Task.Run(() =>
            {
                while (!token.IsCancellationRequested)
                {
                    if (Console.KeyAvailable)
                    {
                        string? input = Console.ReadLine();
                        if (input == "1")
                        {
                            EndConversation();
                            break;
                        }
                    }

                    Thread.Sleep(100); // Doesn't need to run full force
                }
            });
        }

        private async Task<string> GetYeQuote()
        {
            var json = JObject.Parse(await GetResponse(_yeURL));
            return json.GetValue("quote").ToString();
        }

        private async Task<string> GetRonQuote()
        {
            var json = JArray.Parse(await GetResponse(_ronURL));
            return json[0].ToString();
        }

        private async Task<string> GetResponse(string url)
        {
            try
            {
                return await _client.GetStringAsync(url);
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine("Error fetching quote:\n"
                    + e.Message);

                return "{}";
            }
        }
    }
}
