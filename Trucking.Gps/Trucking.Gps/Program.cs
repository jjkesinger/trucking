using System;
using System.Threading;
using System.Threading.Tasks;
using Trucking.Gps.Parsing;
using Trucking.Gps.Core;
using System.Threading.Tasks.Dataflow;
using Newtonsoft.Json;

namespace Trucking.Gps
{
    class Program
    {
        private static int cnt = 0;
        private static int calls = 0;

        static async Task Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            var pipeline = new BroadcastBlock<string>((msg) => new string(msg));
            var deadletter = new BufferBlock<string>();
            var parsing = new PingParsingBlock(deadletter, new GeometrisPingParser(), new SataPingParser());
            var storage = new PingStoringBlock(SaveMessage);

            pipeline.PropagateTo(storage);
            pipeline.PropagateTo(parsing);

            parsing.PropagateTo(new ActionBlock<Ping>((ping) =>
            {
                Console.WriteLine(JsonConvert.SerializeObject(ping));
            }));

            deadletter.PropagateTo(new ActionBlock<string>(msg =>
            {
                Console.WriteLine($"Deadlettered: {msg}");
            }));


            for (int i = 0; i < 1000; i++)
            {
                var msg = $"GEO{i}";
                if (i % 4 == 0)
                    msg = $"SATA{i}";

                if (i == 900)
                    msg = "$E#W";

                pipeline.Post(msg);
            }
            pipeline.Complete();
            await storage.Completion;
            await deadletter.Completion;
            await parsing.Completion;

            Console.WriteLine($"Saved {cnt} messages");
            Console.ReadKey();
        }

        private static async Task SaveMessage(string[] msgs)
        {
            //Logic to save to long term storage
            await Task.Run(() => {
                Thread.Sleep(200);
                
                Interlocked.Add(ref cnt, msgs.Length);
                Interlocked.Add(ref calls, 1);
            });
        }
    }
}
