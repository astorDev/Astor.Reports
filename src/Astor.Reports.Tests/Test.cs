using System;
using Astor.Reports.Data;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MongoDB.Driver;

namespace Astor.Reports.Tests
{
    public class Test
    {
        public readonly WebApplicationFactory Factory = new WebApplicationFactory();

        public readonly string ReportId = Guid.NewGuid().ToString();

        [TestInitialize]
        public void Init()
        {
            var mongo = this.Factory.ServiceProvider.GetRequiredService<MongoClient>();
            mongo.DropDatabase("reports");
        }
        
        public RowsStore GetStore()
        {
            var factory = this.Factory.ServiceProvider.GetRequiredService<RowsStoresFactory>();
            return factory.GetRowsStoreInternal(this.ReportId);
        }
    }
}