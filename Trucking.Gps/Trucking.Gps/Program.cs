using System;
using System.Threading;
using System.Threading.Tasks;
using Trucking.Gps.Parsing;
using Trucking.Gps.Core;
using System.Threading.Tasks.Dataflow;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Linq;

namespace Trucking.Gps
{
    class Program
    {
        private static int cnt = 0;
        private static int calls = 0;
        private static int _process1 = 0;

        static async Task Main(string[] args)
        {
            var receive = new BroadcastBlock<string>(msg => msg);
            var parse = new PingParsingBlock(new GeometrisPingParser(), new SataPingParser());
            var store = new PingStoringBlock(SaveMessage);
            
            var process = new ActionBlock<Ping>(
             ping => 
             {
                //Console.WriteLine(Thread.CurrentThread.ManagedThreadId);
                Interlocked.Add(ref _process1, 1);
                Thread.Sleep(300);
                //Console.WriteLine(JsonConvert.SerializeObject(ping));
             }, new ExecutionDataflowBlockOptions { 
                 MaxDegreeOfParallelism = DataflowBlockOptions.Unbounded 
             });

            var deadletter = new ActionBlock<string>(msg =>
            {
                Thread.Sleep(100);
                Console.WriteLine($"Deadlettered: {msg}");
            });

            receive.Then(store, parse);
            parse.Then(process);
            parse.Then(deadletter);

            var sw = Stopwatch.StartNew();
            await SendMessages(receive);

            receive.Complete();
            
            await process.Completion;
            await deadletter.Completion;
            await store.Completion;
            sw.Stop();

            Console.WriteLine($"Processed in {sw.Elapsed.TotalSeconds} seconds");
            Console.WriteLine($"Saved {cnt} messages in {calls} calls");
            Console.WriteLine($"Process 1: {_process1}");
            Console.ReadKey();
        }

        private static async Task SendMessages(ITargetBlock<string> receive)
        {
            for (int i = 0; i < 1000; i++)
            {
                var msg = $"GEO{i}";
                if (i % 4 == 0)
                    msg = $"SATA{i}";

                if (i % 500 == 0)
                    msg = "$E#W";

                await receive.SendAsync(msg);
            }
        }

        private static async Task SaveMessage(string[] msgs)
        {
            //Logic to save to long term storage
            await Task.Run(() => {
                Thread.Sleep(200);
                Console.WriteLine(Thread.CurrentThread.ManagedThreadId);
                Interlocked.Add(ref cnt, msgs.Length);
                Interlocked.Add(ref calls, 1);
            });
        }
    }
}
