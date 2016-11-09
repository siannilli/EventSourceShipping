using System;
using System.Collections.Generic;
using System.Linq;

using BaseDomainObjects;

using Npgsql;
using Newtonsoft.Json;

namespace EventSourcePostgresRepository
{
    public abstract class PostgresSQLEventSourceRepository<T, TIdentity> : IEventSourceRepository<T, TIdentity> where T : IEventSourcedAggregate<TIdentity>
    {

        private readonly string connectionString;
        private readonly string tableName;

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

        public abstract T Get(TIdentity id); // { throw new NotImplementedException(); }

        void IEventSourceRepository<T, TIdentity>.Save(T instance)
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
    }
}
