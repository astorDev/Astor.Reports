using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;
using Astor.Reports.Protocol.Models;

namespace Astor.Reports.Data
{
    public class RowsStore
    {
        public IMongoCollection<BsonDocument> Collection { get; }

        public RowsStore(IMongoCollection<BsonDocument> collection)
        {
            this.Collection = collection;
        }

        public async Task AddAsync(IEnumerable<dynamic> rows)
        {
            await this.Collection.InsertManyAsync(rows.Select(r => ((object)r).ToBsonDocument()));
        }

        public async Task<int> CountAsync(string filter)
        {
            var filterDefinition = new JsonFilterDefinition<BsonDocument>(filter ?? "{}");
            
            return (int)(await this.Collection.CountDocumentsAsync(filterDefinition));
        }

        public async Task<IEnumerable<dynamic>> GetAsync(RowsQuery query)
        {
            var afterIdFilter = new JsonFilterDefinition<BsonDocument>($"{{ '_id' : {{ '$gt' : '{query.AfterId}' }} }}");

            var passedFilterString = query.Filter ?? "{}";
            var passedFilter = new JsonFilterDefinition<BsonDocument>(passedFilterString);

            var filter = Builders<BsonDocument>.Filter.And(afterIdFilter, passedFilter);
            
            var projection = new JsonProjectionDefinition<BsonDocument>(query.Projection ?? "{}");

            var rawDocs = await this.Collection.Find(filter)
                .Project(projection)
                .Limit(query.Limit)
                .ToListAsync();
            
            return rawDocs.Select(BsonTypeMapper.MapToDotNetValue);
        }

        public async IAsyncEnumerable<dynamic> GetAsyncEnumerable(Queries.RowsQuery query)
        {
            var cursor = await this.Collection.FindAsync(query.Filter, new FindOptions<BsonDocument>
            {
                Projection = query.Projection,
                Limit = query.Limit,
                Sort = query.Sort
            });
            
            while (await cursor.MoveNextAsync())
            {
                foreach (var currentElement in cursor.Current)
                {
                    yield return BsonTypeMapper.MapToDotNetValue(currentElement);
                }
            }
        }
        
        public IAsyncEnumerable<dynamic> GetAsyncEnumerable(RowsQuery query)
        {
            return this.GetAsyncEnumerable(new Queries.RowsQuery
            {
                Filter = new JsonFilterDefinition<BsonDocument>(query.Filter ?? "{}"),
                Projection = new JsonProjectionDefinition<BsonDocument>(query.Projection ?? "{}"),
                Sort = new JsonSortDefinition<BsonDocument>(query.Sorting ?? "{}"),
                Limit = query.Limit
            });
        }
    }
}