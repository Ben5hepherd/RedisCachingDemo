using StackExchange.Redis;

var redisConnectionString = "localhost:6379";
var redis = await ConnectionMultiplexer.ConnectAsync(redisConnectionString);

IDatabase db = redis.GetDatabase();

Console.WriteLine("Connected to Redis.");
Console.WriteLine("Commands: SET, GET, EXIT");

while (true)
{
    Console.Write("\nEnter command: ");
    var input = Console.ReadLine()?.Trim();

    if (string.IsNullOrEmpty(input))
        continue;

    var parts = input.Split(' ', 3, StringSplitOptions.RemoveEmptyEntries);

    var command = parts[0].ToUpperInvariant();

    switch (command)
    {
        case "SET":
            if (parts.Length < 3)
            {
                Console.WriteLine("Usage: SET <key> <value>");
                break;
            }

            string keyToSet = parts[1];
            string valueToSet = parts[2];

            await db.StringSetAsync(keyToSet, valueToSet, TimeSpan.FromMinutes(5));
            Console.WriteLine($"Set key '{keyToSet}' to '{valueToSet}' (expires in 5 minutes)");
            break;

        case "GET":
            if (parts.Length < 2)
            {
                Console.WriteLine("Usage: GET <key>");
                break;
            }

            string keyToGet = parts[1];
            var value = await db.StringGetAsync(keyToGet);

            if (value.IsNull)
                Console.WriteLine($"Key '{keyToGet}' not found.");
            else
                Console.WriteLine($"Value: {value}");
            break;

        case "EXIT":
            Console.WriteLine("Exiting...");
            return;

        default:
            Console.WriteLine("Unknown command. Use SET, GET, or EXIT.");
            break;
    }
}
