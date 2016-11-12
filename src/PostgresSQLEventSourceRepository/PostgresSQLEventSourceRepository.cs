using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Diagnostics;

using BaseDomainObjects;
using EventDispatcherBase;

using Npgsql;
using Newtonsoft.Json;

namespace EventSourcePostgresRepository
{
    public abstract class PostgresSQLEventSourceRepository<TAggregate, TIdentity> : IEventDispatcherRepository, IEventSourceRepository<TAggregate, TIdentity> where TAggregate : IEventSourcedAggregate<TIdentity>
    {

        private readonly string connectionString;
        private readonly string tableName;
        private readonly Process process = Process.GetCurrentProcess();       

        public PostgresSQLEventSourceRepository(            
            string database,
            string login, 
            string password,             
            string tableName,            
            string applicationName = "cqrs+es client",
            string host = "localhost",
            int port = 5432)
        {
            this.connectionString = $"Host={host};Port={port};Username={login};Password={password};Application name={applicationName};Database={database}";
            this.tableName = tableName;

        }

        public abstract TAggregate Get(TIdentity id); // { throw new NotImplementedException(); }

        void IEventSourceRepository<TAggregate, TIdentity>.Save(TAggregate instance)
        {
            var keyValues = this.parseKeyValueFields(instance.Id);
            var commandText = $@"
INSERT INTO {this.tableName} 
(id, aggregate_type, date_time, payload, payload_type, version, { string.Join(", ", keyValues.Keys) })
VALUES (@id, @aggregate_type, @date_time, @payload, @payload_type, @version, { string.Join(", ", keyValues.Keys.Select( k => $"@{k}")) })
";

            using (var connection = new NpgsqlConnection(this.connectionString))
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {                        

                        foreach (var @event in instance.Events)
                        {                            
                            var cmd = connection.CreateCommand();
                            cmd.Transaction = transaction;

                            cmd.CommandText = commandText;
                            cmd.Parameters.Add(new NpgsqlParameter()
                            {
                                ParameterName = "@id",
                                NpgsqlDbType = NpgsqlTypes.NpgsqlDbType.Uuid,
                                NpgsqlValue = @event.Id,
                            });

                            cmd.Parameters.Add(new NpgsqlParameter()
                            {
                                ParameterName = "@aggregate_type",
                                NpgsqlDbType = NpgsqlTypes.NpgsqlDbType.Text,
                                NpgsqlValue = instance.GetType().FullName,

                            });

                            cmd.Parameters.Add(new NpgsqlParameter()
                            {
                                ParameterName = "@date_time",
                                NpgsqlDbType = NpgsqlTypes.NpgsqlDbType.TimestampTZ,
                                NpgsqlValue = DateTime.Now.ToUniversalTime(),
                            });

                            cmd.Parameters.Add(new NpgsqlParameter()
                            {
                                ParameterName = "@payload",
                                NpgsqlDbType = NpgsqlTypes.NpgsqlDbType.Json,
                                NpgsqlValue = JsonConvert.SerializeObject(@event)
                            });

                            cmd.Parameters.Add(new NpgsqlParameter()
                            {
                                ParameterName = "@payload_type",
                                NpgsqlDbType = NpgsqlTypes.NpgsqlDbType.Varchar,
                                NpgsqlValue = @event.GetType().AssemblyQualifiedName,
                            });

                            cmd.Parameters.Add(new NpgsqlParameter()
                            {
                                ParameterName = "@version",
                                NpgsqlDbType = NpgsqlTypes.NpgsqlDbType.Integer,
                                NpgsqlValue = @event.Version
                            });

                            foreach (var key in keyValues.Keys)
                            {
                                cmd.Parameters.Add(new NpgsqlParameter()
                                {
                                    ParameterName = $"@{key}",
                                    NpgsqlDbType = keyValues[key].Item1,
                                    NpgsqlValue = keyValues[key].Item2,
                                });
                            }

                            cmd.ExecuteNonQuery();
                        }                        

                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        throw ex;
                    }
                    finally
                    {
                        connection.Close();
                    }
                }
            }           
        }

        protected abstract IDictionary<string, Tuple<NpgsqlTypes.NpgsqlDbType,object>> parseKeyValueFields(TIdentity identity);

        protected IEnumerable<IEvent> GetEventStream(TIdentity id)
        {

            var settings = new Newtonsoft.Json.JsonSerializerSettings()
            {
                ContractResolver = new JsonNet.PrivateSettersContractResolvers.PrivateSetterContractResolver()
            };

            var returnList = new List<IEvent>();

            var keyValues = this.parseKeyValueFields(id);
            var commandText = $@"
SELECT payload, payload_type FROM {this.tableName} 
WHERE { string.Join(" AND ", keyValues.Keys.Select(k => $"{k}=@{k}" )) }
ORDER BY version, date_time
";

            using (var connection = new NpgsqlConnection(this.connectionString))
            {
                connection.Open();
                var cmd = connection.CreateCommand();
                cmd.CommandText = commandText;

                foreach (var key in keyValues.Keys)
                {
                    cmd.Parameters.Add(new NpgsqlParameter()
                    {
                        ParameterName = $"@{key}",
                        NpgsqlDbType = keyValues[key].Item1,
                        NpgsqlValue = keyValues[key].Item2,
                    });
                }


                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    var type = Type.GetType(reader.GetString(1)); 
                    var @event = JsonConvert.DeserializeObject(reader.GetString(0), type, settings);
                    returnList.Add(@event as IEvent);
                }

                return returnList;

            }
        }

        public void CommitDispatchedEvent(Guid eventId)
        {
            using (var connection = new NpgsqlConnection(this.connectionString))
            {
                var commitEventCommand = connection.CreateCommand();
                commitEventCommand.Connection = connection;
                commitEventCommand.CommandText = "select public.\"CommitDispatchedEvent\"(@id)";
                commitEventCommand.CommandType = System.Data.CommandType.Text;
                
                commitEventCommand.Parameters.Add(new NpgsqlParameter() { ParameterName = "@id", NpgsqlDbType = NpgsqlTypes.NpgsqlDbType.Uuid, Value = eventId });

                connection.Open();                
                commitEventCommand.ExecuteNonQuery();

            }
        }

        public bool GetNextEventToDispatch(out Guid eventId, out string payload)
        {
            eventId = Guid.Empty;
            payload = string.Empty;

            using (var connection = new NpgsqlConnection(this.connectionString))
            {
                var getEventCommand = connection.CreateCommand();
                getEventCommand.Connection = connection;
                getEventCommand.CommandText = @"SELECT * FROM public.""GetEventToDispatch""(@host, @process);";
                getEventCommand.CommandType = System.Data.CommandType.Text;                

                getEventCommand.Parameters.Add(new NpgsqlParameter() { ParameterName = "@host", NpgsqlDbType = NpgsqlTypes.NpgsqlDbType.Varchar, Value = this.process.MachineName });
                getEventCommand.Parameters.Add(new NpgsqlParameter() { ParameterName = "@process", NpgsqlDbType = NpgsqlTypes.NpgsqlDbType.Varchar, Value = this.process.ProcessName });
                //getEventCommand.Parameters.Add(new NpgsqlParameter() { ParameterName = "@id", NpgsqlDbType = NpgsqlTypes.NpgsqlDbType.Uuid, Direction = System.Data.ParameterDirection.InputOutput, Value = DBNull.Value });
                //getEventCommand.Parameters.Add(new NpgsqlParameter() { ParameterName = "@payload", NpgsqlDbType = NpgsqlTypes.NpgsqlDbType.Json, Direction = System.Data.ParameterDirection.InputOutput, Value = DBNull.Value });

                connection.Open();
                var reader = getEventCommand.ExecuteReader();

                if (!reader.Read())
                    return false;

                eventId = reader.GetGuid(0); // (Guid)getEventCommand.Parameters["@id"].Value;
                payload = reader.GetString(1); // (string)getEventCommand.Parameters["@payload"].Value;

                return !eventId.Equals(Guid.Empty);
            }            
        }
    }
}
