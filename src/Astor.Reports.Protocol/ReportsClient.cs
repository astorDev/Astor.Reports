using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Astor.JsonHttp;
using Astor.Reports.Protocol.Models;

namespace Astor.Reports.Protocol
{
    public class ReportsClient : RestApiClient
    {
        public ReportsClient(HttpClient httpClient) : base(httpClient)
        {
        }

        public async Task<About> GetAboutAsync()
        {
            var response = await this.HttpClient.GetAsync(Uris.About);
            return await this.ReadAsync<About>(response);
        }
        
        public async Task<Report> CreateReportAsync(ReportCandidate candidate)
        {
            var response = await this.HttpClient.PostJsonAsync("", candidate);
            return await this.ReadAsync<Report>(response);
        }

        public async Task<ReportsCollection> GetReportsAsync(ReportsQuery filter)
        {
            var response = await this.HttpClient.GetAsync(HttpHelper.GetQueryString("", filter));
            return await this.ReadAsync<ReportsCollection>(response);
        }
        
        public async Task AddPagesAsync(string reportId, PageCandidate candidate)
        {
            var response = await this.HttpClient.PostJsonAsync(Uris.ReportPages(reportId), candidate);
            await this.ReadAsync(response);
        }
        
        public async Task<RowsCollection> GetRows(string reportId, RowsQuery query)
        {
            var uri = HttpHelper.GetQueryString(Uris.ReportRows(reportId), query);
            var response = await this.HttpClient.GetAsync(uri);
            return await this.ReadAsync<RowsCollection>(response);
        }

        public async Task<ExportedCsv> CreateCsv(string reportId, string exportId, ExportFinalization finalization)
        {
            var uri = Uris.ReportExportCsv(reportId, exportId);
            var response = await this.HttpClient.PostJsonAsync(uri, finalization);
            return await this.ReadAsync<ExportedCsv>(response);
        }
        
        public async Task<Stream> GetCsv(string reportId, ExportQuery query)
        {
            var uri = HttpHelper.GetQueryString(Uris.ExportCsv(reportId), query);
            var response = await this.HttpClient.GetAsync(uri);
            if (!response.IsSuccessStatusCode)
            {
                await this.OnNoneSuccessStatusCode(response);
            }

            return await response.Content.ReadAsStreamAsync();
        }

        public async Task<Report> GetReportAsync(string id)
        {
            var response = await this.HttpClient.GetAsync(id);
            return await this.ReadAsync<Report>(response);
        }
        
        public async Task<Export> CreateExport(string reportId, ExportConditions exportConditions)
        {
            var response = await this.HttpClient.PostJsonAsync(Uris.ReportExports(reportId), exportConditions);
            return await this.ReadAsync<Export>(response);
        }

        public async Task<Export> GetExportAsync(string reportId, string exportId)
        {
            var uri = Uris.ReportExport(reportId, exportId);
            var response = await this.HttpClient.GetAsync(uri);
            return await this.ReadAsync<Export>(response);
        }
    }
}