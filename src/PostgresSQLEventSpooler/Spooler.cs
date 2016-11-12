using System;
using System.Diagnostics;

using MessageBrokerBase;
using Npgsql;

namespace PostgresSQLEventSpooler
{
    public class Spooler
    {
        private readonly string connectionString;
        private readonly IEventBroadcaster messageBroker;
        private readonly Process process;

        public Spooler(
            string database,
            string login, 
            string password,
            string event_table,
            IEventBroadcaster messageBroker,
            string applicationName = "event spooler",
            string host = "localhost",
            int port = 5432)
        {
            this.connectionString = $"Host={host};Port={port};Username={login};Password={password};Application name={applicationName};Database={database}";
            this.messageBroker = messageBroker;
            this.process = Process.GetCurrentProcess();
        }

        public void DispatchEvents()
        {
            using (var connection = new NpgsqlConnection(this.connectionString))
            {                
                var getEventCommand = connection.CreateCommand();
                getEventCommand.CommandText = "GetEventToDispatch";
                getEventCommand.CommandType = System.Data.CommandType.StoredProcedure;
                getEventCommand.Parameters.Add(new NpgsqlParameter() { ParameterName = "@host", NpgsqlDbType = NpgsqlTypes.NpgsqlDbType.Varchar, Value = this.process.MachineName });
                getEventCommand.Parameters.Add(new NpgsqlParameter() { ParameterName = "@process", NpgsqlDbType = NpgsqlTypes.NpgsqlDbType.Varchar, Value = this.process.ProcessName });
                getEventCommand.Parameters.Add(new NpgsqlParameter() { ParameterName = "@id", NpgsqlDbType = NpgsqlTypes.NpgsqlDbType.Uuid, Direction = System.Data.ParameterDirection.Output, Value = DBNull.Value });
                getEventCommand.Parameters.Add(new NpgsqlParameter() { ParameterName = "@payload", NpgsqlDbType = NpgsqlTypes.NpgsqlDbType.Json, Direction = System.Data.ParameterDirection.Output, Value = DBNull.Value });

                var commitEventCommand = connection.CreateCommand();
                commitEventCommand.CommandText = "CommitDispatchedEvent";
                commitEventCommand.CommandType = System.Data.CommandType.StoredProcedure;
                commitEventCommand.Parameters.Add(new NpgsqlParameter() { ParameterName = "@id", NpgsqlDbType = NpgsqlTypes.NpgsqlDbType.Uuid, Value = DBNull.Value });

                try
                {
                    connection.Open();

                    object id = DBNull.Value;
                    do
                    {
                        getEventCommand.ExecuteNonQuery();

                        id = getEventCommand.Parameters["@id"].Value;
                        var payload = getEventCommand.Parameters["@payload"].Value;

                        if (id != DBNull.Value && payload != DBNull.Value)
                        {
                            this.messageBroker.Publish(payload);

                            commitEventCommand.Parameters["@id"].Value = id;
                            commitEventCommand.ExecuteNonQuery();

                        }

                    } while (!id.Equals(DBNull.Value));

                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    if (connection.State == System.Data.ConnectionState.Open)
                        connection.Close();
                }
            }
        }
    }
}