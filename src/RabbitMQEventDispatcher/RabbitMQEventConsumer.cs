using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EventDispatcherBase;

using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace RabbitMQEventDispatcher
{
    public class RabbitMQEventConsumer : RabbitMQClient, IEventConsumer
    {
        public event EventReceived ReceivedEventHandler;
        private readonly Dictionary<Guid, ulong> incomingEvents = new Dictionary<Guid, ulong>();
        private EventingBasicConsumer consumer;
        private IModel channel;

        public RabbitMQEventConsumer(string host = "localhost", string vhost ="/", int port = 5672, string username = "guest", string password = "guest")
            : base(host, vhost, port, username, password)
        {
     
                
        }

        private void Consumer_Received(object sender, BasicDeliverEventArgs e)
        {
            try
            {
                string eventName = System.Text.Encoding.UTF8.GetString((byte[])e.BasicProperties.Headers["event.name"]);
                string source =  e.BasicProperties.Headers["event.source"] != null ? 
                    System.Text.Encoding.UTF8.GetString((byte[]) e.BasicProperties.Headers["event.source"])
                    : null;
                DateTime timestamp = Newtonsoft.Json.JsonConvert.DeserializeObject<DateTime>(System.Text.Encoding.UTF8.GetString((byte[])e.BasicProperties.Headers["event.timestamp"]));
                int version = (int)e.BasicProperties.Headers["event.version"];

                var dispatchEvent = new DispatchEvent()
                {
                    Id = Guid.Parse(e.BasicProperties.MessageId),
                    EventName = eventName,
                    Source = source,
                    Timestamp = timestamp,
                    Version = version,
                    Payload = System.Text.Encoding.UTF8.GetString(e.Body)
                };

                System.Console.WriteLine($"Received event type: {dispatchEvent.EventName}, Id: {dispatchEvent.Id}");
                incomingEvents[dispatchEvent.Id] = e.DeliveryTag;
                ReceivedEventHandler?.Invoke(dispatchEvent);

            }
            catch (Exception ex)
            {
                System.Console.Error.WriteLine($"Error unpacking incoming event {e.BasicProperties.MessageId}");
                System.Console.Error.WriteLine(ex);                
            }
        }

        public void AckReceived(Guid eventId)
        {
            if (!incomingEvents.Keys.Contains(eventId))
                throw new InvalidOperationException("Trying to acknowledge an invalid event id.");

            this.channel.BasicAck(incomingEvents[eventId], false);
            incomingEvents.Remove(eventId);
        }

        public override void Dispose()
        {           
            this.channel.Close();
            base.Dispose();
        }

        public void Quit()
        {
            // unregister consumer            
            if (this.consumer != null)
                this.consumer.Received -= Consumer_Received;
            if (channel != null && this.channel.IsOpen)
                this.channel.Close();

            this.currentConnection.Close();
        }

        public void StartConsumingEvents(string queueName)
        {
            if (string.IsNullOrEmpty(queueName))
                throw new NullReferenceException("Queue name cannot be null.");

            if (!this.currentConnection.IsOpen)
                throw new InvalidOperationException("Invalid object status. Connection is already closed.");

            if (this.channel == null)
                this.channel = this.currentConnection.CreateModel();

            this.channel.QueueDeclare(queue: queueName, durable: true, exclusive: false, autoDelete: false);

            consumer = new EventingBasicConsumer(this.channel);
            consumer.Received += Consumer_Received;
            this.channel.BasicConsume(queueName, false, consumer);      
        }

    }
}
