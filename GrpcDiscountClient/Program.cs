using Grpc.Net.Client;
using GrpcDiscountService;
using Microsoft.Extensions.Configuration;

namespace GrpcDiscountClient
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();
            var grpcServerUrl = config["GrpcServerUrl"];

            var channel = GrpcChannel.ForAddress(grpcServerUrl);
            var client = new Discounter.DiscounterClient(channel);

            while (true)
            {
                Console.WriteLine("\nChoose an option:");
                Console.WriteLine("1 - Generate discount codes");
                Console.WriteLine("2 - Use a discount code");
                Console.WriteLine("0 - Exit");
                Console.Write("Your choice: ");

                var choice = Console.ReadLine();

                if (choice == "0")
                    break;

                switch (choice)
                {
                    case "1":
                        Console.Write("Enter count (1-2000): ");
                        if (!uint.TryParse(Console.ReadLine(), out var count))
                        {
                            Console.WriteLine("Invalid count.");
                            break;
                        }

                        Console.Write("Enter length (7 or 8): ");
                        if (!uint.TryParse(Console.ReadLine(), out var length))
                        {
                            Console.WriteLine("Invalid length.");
                            break;
                        }

                        Console.Write("Enter number of threads: ");
                        if (!int.TryParse(Console.ReadLine(), out var threadCountGenerate) || threadCountGenerate <= 0)
                        {
                            Console.WriteLine("Invalid number of threads.");
                            break;
                        }

                        // var generateReply = await client.GenerateAsync(new GenerateRequest
                        //     { Count = count, Length = length });
                        // Console.WriteLine($"Generate result: {generateReply.Result}");

                        await RunGenerateInParallel(threadCountGenerate, count, length);

                        break;

                    case "2":
                        Console.Write("Enter code: ");
                        var code = Console.ReadLine();

                        Console.Write("Enter number of threads: ");
                        if (!int.TryParse(Console.ReadLine(), out var threadCountUseCode) || threadCountUseCode <= 0)
                        {
                            Console.WriteLine("Invalid number of threads.");
                            break;
                        }

                        // var useCodeReply = await client.UseCodeAsync(new UseCodeRequest { Code = code });
                        // Console.WriteLine($"Use code result: {useCodeReply.Result} - {(UseCodeResultEnum)useCodeReply.Result}");

                        await RunUseCodeInParallel(threadCountUseCode, code);

                        break;

                    default:
                        Console.WriteLine("Invalid option, try again.");
                        break;
                }
            }

            async Task RunGenerateInParallel(int threadCount, uint count, uint length)
            {
                var tasks = new List<Task>();

                for (var i = 0; i < threadCount; i++)
                {
                    tasks.Add(Task.Run(async () =>
                    {
                        var generateReply = await client.GenerateAsync(new GenerateRequest
                        { Count = count, Length = length });
                        Console.WriteLine($"Generate result from thread {Thread.CurrentThread.ManagedThreadId}: {generateReply.Result}");
                    }));
                }

                await Task.WhenAll(tasks);
            }

            async Task RunUseCodeInParallel(int threadCount, string code)
            {
                var tasks = new List<Task>();

                for (var i = 0; i < threadCount; i++)
                {
                    tasks.Add(Task.Run(async () =>
                    {
                        var useCodeReply = await client.UseCodeAsync(new UseCodeRequest { Code = code });
                        Console.WriteLine($"UseCode result from thread {Thread.CurrentThread.ManagedThreadId}: {useCodeReply.Result} - {(UseCodeResultEnum)useCodeReply.Result}");
                    }));
                }

                await Task.WhenAll(tasks);
            }
        }
    }
}