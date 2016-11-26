using System.Collections.Generic;
using System.Linq;
using SharedShippingDomainsObjects.ValueObjects;

using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Linq.Expressions;

using SpotCharterViewModel;

namespace Shipping.Repositories
{
    public class SpotCharterQueryRepository: ISpotCharterUpdateViewRepository, ISpotCharterQueryRepository
    {
        private const string spotChartersCollectionName = "spot.charters";
        private readonly MongoCredential credential;
        private readonly string database;
        private readonly string password;
        private readonly MongoClientSettings clientSettings;
        private readonly string username;

        public SpotCharterQueryRepository(string host = "localhost", string database = "spotService", int port = 27017, string username = null, string password = null)
        {
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

        private IMongoCollection<SpotCharterView> GetSpotCollection()
        {
            var client = new MongoClient(this.clientSettings);            
            return client.GetDatabase(this.database).GetCollection<SpotCharterView>(SpotCharterQueryRepository.spotChartersCollectionName);
        }

        public SpotCharterView GetById(SpotCharterId id)
        {
            var filter = Builders<SpotCharterView>.Filter.Eq("_id", id);
            var collection = GetSpotCollection();
            return collection.Find(filter).FirstOrDefault();
        }

        public void Save(SpotCharterView document)
        {
            var collection = GetSpotCollection();
            var filter = Builders<SpotCharterView>.Filter.Eq("_id", document.Id);
        
            var existing = collection.Find(filter).FirstOrDefault();

            if (existing == null)
                collection.InsertOne(document);
            else
                collection.ReplaceOne(filter, document);            

        }

        public void Remove(SpotCharterId id)
        {
            var collection = GetSpotCollection();
            var filter = Builders<SpotCharterView>.Filter.Eq("_id", id);

            collection.DeleteOne(filter);

        }
        public void Remove(SpotCharterView document)
        {
            Remove(document.Id);
        }

        private MongoClient GetClientConnection()
        {
            return new MongoClient(this.clientSettings);
        }

        public IQueryable<SpotCharterView> Find(Expression<Func<SpotCharterView, bool>> predicate)
        {
            return Find().Where(predicate);
        }

        public IQueryable<SpotCharterView> Find()
        {
            return GetSpotCollection().AsQueryable();
        }

        public SpotCharterView GetBySpotCharterId(SpotCharterId spotId)
        {
            return GetById(spotId);
        }

        public IQueryable<SpotCharterView> ChartersMissingFreightRate()
        {
            throw new NotImplementedException();
        }

        public IQueryable<SpotCharterView> ChartersMissingDemurrageTerms()
        {
            throw new NotImplementedException();
        }

        public IQueryable<SpotCharterView> ChartersMissingPortfolio()
        {
            throw new NotImplementedException();
        }

        public IQueryable<SpotCharterView> ScheduledCharters()
        {
            throw new NotImplementedException();
        }
    }
}
