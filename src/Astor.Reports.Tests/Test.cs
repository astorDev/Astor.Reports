using System;
using Astor.Reports.Data;
using Microsoft.Extensions.DependencyInjection;

namespace Astor.Reports.Tests
{
    public class Test
    {
        public readonly WebApplicationFactory Factory = new WebApplicationFactory();

        public readonly string ReportId = Guid.NewGuid().ToString();

        public RowsStore GetStore()
        {
            var factory = this.Factory.ServiceProvider.GetRequiredService<RowsStoresFactory>();
            return factory.GetRowsStore(this.ReportId);
        }
    }
}