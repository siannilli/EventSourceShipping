using System.Collections.Generic;
using System.Linq;
using SharedShippingDomainsObjects.ValueObjects;
using SpotCharterDomain;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Shipping.Repositories
{
    public class SpotCharterQueryRepository: ISpotCharterUpdateViewRepository
    {
        private const string spotChartersCollectionName = "spot.charters";
        private readonly string connectionString;
        private readonly MongoCredential credential;
        private readonly string database;
        private readonly string password;
        private readonly MongoClientSettings clientSettings;
        private readonly string username;

        public SpotCharterQueryRepository(string host = "localhost", string database = "spotService", int port = 27017, string username = null, string password = null)
        {
            this.connectionString = $"mongodb://{username}:{password}@{host}:{port}/{database}";
            this.username = username;
            this.password = password;
            this.database = database;

            this.credential = MongoCredential.CreateCredential(this.database, this.username, this.password);

            this.clientSettings = new MongoClientSettings()
            {
                Server = new MongoServerAddress(host, port),                                                
                GuidRepresentation = GuidRepresentation.CSharpLegacy,
                Credentials = new List<MongoCredential>() { credential },
            };
        }

        public SpotCharter GetById(SpotCharterId id)
        {
            var client = new MongoClient(this.clientSettings);            
            var collection = client.GetDatabase(this.database).GetCollection<SpotCharter>(SpotCharterQueryRepository.spotChartersCollectionName);
            var filter = Builders<SpotCharter>.Filter.Eq("_id", id);

            return collection.Find(filter).FirstOrDefault();
        }

        public void Save(SpotCharter document)
        {
            var client = new MongoClient(this.clientSettings);            
            var collection = client.GetDatabase(this.database).GetCollection<SpotCharter>(SpotCharterQueryRepository.spotChartersCollectionName);
            var filter = Builders<SpotCharter>.Filter.Eq("_id", document.Id);
        
            var existing = collection.Find(filter).FirstOrDefault();

            if (existing == null)
                collection.InsertOne(document);
            else
                collection.ReplaceOne(filter, document);            

        }

        public void Remove(SpotCharterId id)
        {
            var client = new MongoClient(this.clientSettings);            
            var collection = client.GetDatabase(this.database).GetCollection<SpotCharter>(SpotCharterQueryRepository.spotChartersCollectionName);
            var filter = Builders<SpotCharter>.Filter.Eq("_id", id);

            collection.DeleteOne(filter);

        }
        public void Remove(SpotCharter document)
        {
            Remove(document.Id);
        }

        private MongoClient GetClientConnection()
        {
            return new MongoClient(this.clientSettings);
        }
    }
}
