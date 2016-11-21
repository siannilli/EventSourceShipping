using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using RabbitMQEventDispatcher;
using SpotCharterViewUpdater;
using Shipping.Repositories;

namespace SpotCharterViewUpdaterHost
{

    /// <summary>
    /// Simple host for SpotCharterViewUpdater
    /// </summary>
    public class Program
    {

        public static void Main(string[] args)
        {
            //TODO: read from configuration / env variables configuration parameters
            var workerProcess = new WorkerProcess(new RabbitMQEventConsumer(host: "message-broker", vhost: "/test", username: "siannilli", password: "siannilli"), 
                new SpotCharterEventSourceRepository("SpotCharters", "spot_user", "spot_user", host: "sql-db"), 
                new SpotCharterQueryRepository("doc-db", "spotService", username: "spotService", password: "spotService"));

            Task.WaitAll(Task.Factory.StartNew(() =>
            {
                Console.WriteLine("Consumer start waiting events from message broker.");
                workerProcess.Start("chartering.spot.viewupdate");
                while (Console.ReadLine() != "quit") ;
                Console.WriteLine("Quitting consumer ...");
                workerProcess.Quit();
                Console.WriteLine("Closing");
            }));
        }
    }
}
