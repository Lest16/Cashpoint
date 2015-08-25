namespace Cashpoint
{
    using System.Collections.Generic;
    using System.Linq;

    using MongoDB.Bson.Serialization.Attributes;
    using MongoDB.Driver;

    public class MongoRepository : IRepository<CashpointState>
    {
        private const string Name = "cashpoint";

        private readonly IMongoCollection<Pair> collection;

        public MongoRepository(IMongoClient client)
        {
            var database = client.GetDatabase(Name);
            this.collection = database.GetCollection<Pair>(Name);
        }

        public async void Save(CashpointState state)
        {
            foreach (var doc in state.Bank.Select(pair => new Pair { Key = pair.Key, Value = pair.Value }))
            {
                await this.collection.InsertOneAsync(doc);
            }
        }

        public CashpointState GetContents()
        {
            var docs = this.collection.FindAsync(r => true);
            docs.Wait();
            docs.Result.MoveNextAsync().Wait();
            var state = new CashpointState();
            var dicState = new Dictionary<uint, uint>();
            foreach (var pair in docs.Result.Current)
            {
                var key  = pair.Key;
                var value = pair.Value;
                dicState.Add(key, value);
            }

            state.Bank = dicState;
            return state;
        }

        public void Clear()
        {
            this.collection.DeleteManyAsync(r => true).Wait();
        }

        private class Pair
        {
            [BsonId]
            public uint Key;

            public uint Value;
        }
    }
}
