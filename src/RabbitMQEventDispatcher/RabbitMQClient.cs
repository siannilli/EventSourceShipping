using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


using RabbitMQ.Client;

namespace RabbitMQEventDispatcher
{
    public abstract class RabbitMQClient: IDisposable
    {
        protected readonly ConnectionFactory connectionFactory;
        protected readonly IConnection currentConnection;

        public RabbitMQClient(
            string host = "localhost",
            string vhost = "/",
            int port = 5672,
            string username = "guest",
            string password = "guest"            
            )
        {
            this.connectionFactory = new ConnectionFactory()
            {
                UserName = username,
                Password = password,
                HostName = host,
                VirtualHost = vhost,
                Port = port,
            };
            
            currentConnection = connectionFactory.CreateConnection();

        }

        public virtual void Dispose()
        {
            if (currentConnection.IsOpen)
                currentConnection.Close();
        }
    }
}
