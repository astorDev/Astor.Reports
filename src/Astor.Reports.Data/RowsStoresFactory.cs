using MongoDB.Bson;
using MongoDB.Driver;

namespace Astor.Reports.Data
{
    public class RowsStoresFactory
    {
        public IMongoDatabase Db { get; }

        public RowsStoresFactory(IMongoDatabase db)
        {
            this.Db = db;
        }

        public RowsStore GetRowsStore(string reportName)
        {
            var collection = this.Db.GetCollection<BsonDocument>(reportName);
            return new RowsStore(collection);
        }
    }
}