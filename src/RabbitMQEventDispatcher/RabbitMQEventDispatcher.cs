using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using EventDispatcherBase;
using RabbitMQ.Client;

namespace RabbitMQEventDispatcher
{
    public class RabbitMQEventDispatcher: RabbitMQClient, IEventDispatcher
    {
        private readonly string exchange;        

        public RabbitMQEventDispatcher(
            string host = "localhost", 
            string vhost = "/",
            int port = 5672,
            string username = "guest",
            string password = "guest",
            string exchangeName = null            
            )
            : base(host, vhost, port, username, password)
        {
            if (string.IsNullOrEmpty(exchangeName))
                throw new ArgumentNullException("exchangeName", "Exchange name cannot be null");
            this.exchange = exchangeName;
            this.exchangeDeclare();

        }

        private void exchangeDeclare()
        {            
            using (IModel model = currentConnection.CreateModel())
            {
                model.ExchangeDeclare(exchange, ExchangeType.Topic, durable: true, autoDelete: false);
            }
        }

        public void Publish(DispatchEvent @event)
        {            
            using(IModel model = currentConnection.CreateModel())
            {
                var props = model.CreateBasicProperties();
                props.ContentType = "application/json";
                props.MessageId = @event.Id.ToString();

                props.Headers = new Dictionary<string, object>()
                {
                    
                    { "event.name", @event.EventName },
                    { "event.timestamp", Newtonsoft.Json.JsonConvert.SerializeObject(@event.Timestamp) },
                    { "event.source", @event.Source },
                    { "event.version", @event.Version },                    
                };                  

                model.BasicPublish(this.exchange, $"{this.exchange}.{@event.EventName}", 
                    basicProperties: props, 
                    body: System.Text.Encoding.UTF8.GetBytes(@event.Payload));
            }
        }

    }
}
