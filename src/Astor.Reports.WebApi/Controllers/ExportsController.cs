using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Astor.Reports.Data;
using Astor.Reports.Protocol;
using Astor.Reports.Protocol.Models;
using PickPoint.Reports.WebApi.Helpers;
using RowsQuery = Astor.Reports.Data.Queries.RowsQuery;

namespace PickPoint.Reports.WebApi.Controllers
{
    public class ExportsController : Controller
    {
        public const int BucketSize = 5000;

        public RowsStoresFactory StoresFactory { get; }
        public ReportsStore ReportsStore { get; }

        public ExportsController(RowsStoresFactory storesFactory, ReportsStore reportsStore)
        {
            this.StoresFactory = storesFactory;
            this.ReportsStore = reportsStore;
        }

        [HttpPost("{reportId}" + "/" + Uris.Exports)]
        public async Task<Export> CreateExportAsync(string reportId, [FromBody] ExportConditions conditions)
        {
            var store = this.StoresFactory.GetRowsStore(reportId);

            var count = await store.CountAsync((string)conditions.Filter?.ToString());
            Console.WriteLine("Export Elements Counted");
            var estimatedBucketLength = (int)Math.Ceiling((double)count / ExportBuckets.MaxCount);

            var rows = store.GetAsyncEnumerable(conditions.ToQuery()).Select(r => r[conditions.GetSortingField()]);

            var export = new Export
            {
                ElementsCount = (int) count,
                Buckets = await ExportBuckets.PackAsync(rows, estimatedBucketLength).ToArrayAsync(),
                Conditions = conditions
            };

            return await this.ReportsStore.AddExportAsync(reportId, export);
        }

        [HttpGet("{reportId}" + "/" + Uris.Exports + "/" + "{exportId}")]
        public async Task<ActionResult> GetExportAsync(string reportId, string exportId)
        {
            var export = await this.ReportsStore.SearchExportAsync(reportId, exportId);
            if (export == null)
            {
                return this.NotFound(Error.ExportNotFound);
            }

            return this.Ok(export);
        }

        [HttpPost("{reportId}" + "/" + Uris.Exports + "/" + "{exportId}" + "/" + Uris.Csv)]
        public async Task<ExportedCsv> CreateExportCsv(string reportId, string exportId,
            [FromBody] ExportFinalization finalization)
        {
            var export = await this.ReportsStore.SearchExportAsync(reportId, exportId);
            if (export == null)
            {
                throw new InvalidOperationException("export not found");
            }
           
            var bucketFiles = new ConcurrentDictionary<ExportBucket, string>();


            var bucketsExports = export.Buckets.Select(b => CreateExportCsvForBucket(bucketFiles, export, b, finalization.Columns));
            await Task.WhenAll(bucketsExports);

            var finalFilePath = Path.GetTempFileName();
            var columnNames = finalization.Columns.Select(c => c.Value);

            Console.WriteLine("all buckets done! joining csv");
            CsvWriter.Join(export.Buckets.Select(b => bucketFiles[b]), finalFilePath, columnNames);

            return new ExportedCsv
            {
                Path = finalFilePath
            };
        }

        public async Task CreateExportCsvForBucket(
            ConcurrentDictionary<ExportBucket, string> bucketFiles, 
            Export export, 
            ExportBucket bucket,
            Dictionary<string, string> columns)
        {
            var left = bucket.ElementsCount;
            var rowsStore = this.StoresFactory.GetRowsStore(export.ReportId);
            var query = RowsQuery.ForExport(export, bucket, columns);
            
            var valuesOrder = columns.Select(c => c.Key).ToArray();

            var resultFilePath = Path.GetTempFileName();
            await using (var writer = new StreamWriter(resultFilePath))
            {
                var csvWriter = new CsvWriter(writer);
                await foreach (var el in rowsStore.GetAsyncEnumerable(query))
                {
                    csvWriter.WriteRow(valuesOrder, el);
                    left--;

                    if (left % 10000 == 0)
                    {
                        var bucketIndex = Array.IndexOf(export.Buckets, bucket);
                        Console.WriteLine($"bucket {bucketIndex}: {left} rows left");
                    }
                }
            }

            bucketFiles.AddOrUpdate(bucket, resultFilePath, (e, p) => p);
        }
        
        [HttpGet("{reportId}" + "/" + Uris.Exports + "/" + Uris.Csv)]
        public async Task<FileStreamResult> GetCsvAsync(string reportId, [FromQuery] ExportQuery query)
        {
            var store = this.StoresFactory.GetRowsStore(reportId);
            
            var resultFilePath = Path.GetTempFileName();
            var left = query.Limit;
            
            var columns = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, string>>(query.Columns);
            var columnNames = columns.Select(c => c.Value);
            var valuesOrder = columns.Select(c => c.Key).ToArray();

            await using (var writer = new StreamWriter(resultFilePath))
            {
                var csvWriter = new CsvWriter(writer);
                csvWriter.WriteHeader(columnNames);
                await foreach (var el in store.GetAsyncEnumerable(query))
                {
                    csvWriter.WriteRow(valuesOrder, el);
                    left -= 1;

                    if (left % 10000 == 0)
                    {
                        Console.WriteLine($"{left} rows left");
                    }
                }
            }
            
            var resultStream = System.IO.File.OpenRead(resultFilePath);
            return new FileStreamResult(resultStream, "text/csv");
        } 
    }
}