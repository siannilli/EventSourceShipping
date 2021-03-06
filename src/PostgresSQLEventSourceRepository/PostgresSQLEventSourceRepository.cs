﻿using System;
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
    public abstract class PostgresSQLEventSourceRepository<TAggregate, TIdentity> : IEventDispatcherRepository, IEventSourceCommandRepository<TAggregate, TIdentity> where TAggregate : IEventSourcedAggregate<TIdentity>
    {

        private readonly string connectionString;
        private readonly string tableName;
        private readonly Process process = Process.GetCurrentProcess();
        private readonly IEventDispatcher messageDispatcher;
        private readonly string applicationName;

        public PostgresSQLEventSourceRepository(            
            string database = null,
            string login = null, 
            string password = null,             
            string tableName = null,            
            string applicationName = "cqrs+es client",
            string host = "localhost",            
            int port = 5432,
            IEventDispatcher dispatcher = null)
        {
            this.connectionString = $"Host={host};Port={port};Username={login};Password={password};Application name={applicationName};Database={database}";
            this.tableName = tableName;
            this.applicationName = applicationName;
            this.messageDispatcher = dispatcher;
        }

        public abstract TAggregate Get(TIdentity id); // { throw new NotImplementedException(); }

        public void Save(TAggregate instance)
        {

            var eventsToDispatch = new List<DispatchEvent>();

            var keyValues = this.parseKeyValueFields(instance.Id);
            var commandText = $@"
INSERT INTO {this.tableName} 
(id, aggregate_type, date_time, payload, payload_type, version, event_name, source, { string.Join(", ", keyValues.Keys) })
VALUES (@id, @aggregate_type, @date_time, @payload, @payload_type, @version, @event_name, @source, { string.Join(", ", keyValues.Keys.Select( k => $"@{k}")) })
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
                            var jsonPayload = JsonConvert.SerializeObject(@event);
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
                                NpgsqlValue = jsonPayload,
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

                            cmd.Parameters.Add(new NpgsqlParameter()
                            {
                                ParameterName = "@event_name",
                                NpgsqlDbType = NpgsqlTypes.NpgsqlDbType.Varchar,
                                NpgsqlValue = @event.EventName,
                            });

                            cmd.Parameters.Add(new NpgsqlParameter()
                            {
                                ParameterName = "@source",
                                NpgsqlDbType = NpgsqlTypes.NpgsqlDbType.Varchar,
                                NpgsqlValue = (object)@event.Source ?? DBNull.Value,
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
                            eventsToDispatch.Add(new DispatchEvent
                            {
                                EventName = @event.EventName,
                                Id = @event.Id,
                                Source = @event.Source,
                                Version = @event.Version,
                                Timestamp = @event.Timestamp,
                                Payload = jsonPayload
                            });

                        }                        

                        transaction.Commit();

                        if (eventsToDispatch.Count > 0 && this.messageDispatcher != null)
                        {
                            Task.Factory.StartNew(() =>
                            {
                                foreach (var @event in eventsToDispatch)
                                {
                                    this.messageDispatcher.Publish(@event);
                                    this.CommitDispatchedEvent(@event.Id);
                                }
                            }); 
                        }

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

        protected IEnumerable<IEvent<TIdentity>> GetEventStream(TIdentity id)
        {

            var settings = new Newtonsoft.Json.JsonSerializerSettings()
            {
                ContractResolver = new JsonNet.PrivateSettersContractResolvers.PrivateSetterContractResolver()
            };

            var returnList = new List<IEvent<TIdentity>>();

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
                    returnList.Add(@event as IEvent<TIdentity>);
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
                commitEventCommand.CommandText = "select public.\"CommitDispatchedEvent\"(@id, @host, @process)";
                commitEventCommand.CommandType = System.Data.CommandType.Text;
                
                commitEventCommand.Parameters.Add(new NpgsqlParameter() { ParameterName = "@id", NpgsqlDbType = NpgsqlTypes.NpgsqlDbType.Uuid, Value = eventId });
                commitEventCommand.Parameters.Add(new NpgsqlParameter() { ParameterName = "@host", NpgsqlDbType = NpgsqlTypes.NpgsqlDbType.Varchar, Value = this.process.MachineName });
                commitEventCommand.Parameters.Add(new NpgsqlParameter() { ParameterName = "@process", NpgsqlDbType = NpgsqlTypes.NpgsqlDbType.Varchar, Value = this.applicationName });

                connection.Open();                
                commitEventCommand.ExecuteNonQuery();

            }
        }

        public bool GetNextEventToDispatch(out Guid eventId, out DispatchEvent @event)
        {
            eventId = Guid.Empty;
            @event = null;

            using (var connection = new NpgsqlConnection(this.connectionString))
            {
                var getEventCommand = connection.CreateCommand();
                getEventCommand.Connection = connection;
                getEventCommand.CommandText = @"SELECT * FROM public.""GetEventToDispatch""(@host, @process);";
                getEventCommand.CommandType = System.Data.CommandType.Text;

                getEventCommand.Parameters.Add(new NpgsqlParameter() { ParameterName = "@host", NpgsqlDbType = NpgsqlTypes.NpgsqlDbType.Varchar, Value = this.process.MachineName });
                getEventCommand.Parameters.Add(new NpgsqlParameter() { ParameterName = "@process", NpgsqlDbType = NpgsqlTypes.NpgsqlDbType.Varchar, Value = this.applicationName });

                connection.Open();
                var reader = getEventCommand.ExecuteReader();

                if (!reader.Read() || reader.IsDBNull(0))
                    return false;


                @event = new DispatchEvent()
                {
                    Id = eventId = reader.GetGuid(0),
                    EventName = reader.GetString(reader.GetOrdinal("event_name")),
                    Source = reader.IsDBNull(reader.GetOrdinal("source")) ? null : reader.GetString(reader.GetOrdinal("source")),
                    Version = reader.GetInt32(reader.GetOrdinal("version")),
                    Timestamp = reader.GetDateTime(reader.GetOrdinal("date_time")),
                    Payload = reader.GetString(reader.GetOrdinal("payload")), // (string)getEventCommand.Parameters["@payload"].Value;
            };
                return !eventId.Equals(Guid.Empty);
            }            
        }
    }
}
