using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using RabbitMQEventDispatcher;
using SpotCharterViewUpdater;

namespace SpotCharterViewUpdaterHost
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var workerProcess = new WorkerProcess(new RabbitMQEventConsumer(host: "message-broker", vhost: "/test", username: "siannilli", password: "siannilli"));

            Task.WaitAll(Task.Factory.StartNew(() =>
            {
                Console.WriteLine("Consumer start waiting events from message broker.");
                while (Console.ReadLine() != "quit") ;
                Console.WriteLine("Quitting consumer ...");
                workerProcess.Quit();
                Console.WriteLine("Closing");
            }));
        }
    }
}
