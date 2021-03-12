using Astor.Reports.Domain;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Astor.Reports.Data
{
    public class RowsStoresFactory : IRowsStoreFactory
    {
        public IMongoDatabase Db { get; }

        public RowsStoresFactory(IMongoDatabase db)
        {
            this.Db = db;
        }

        public RowsStore GetRowsStoreInternal(string reportName)
        {
            var collection = this.Db.GetCollection<BsonDocument>(reportName);
            return new RowsStore(collection);
        }

        public IRowsStore GetRowsStore(string reportId) => this.GetRowsStoreInternal(reportId);
    }
}