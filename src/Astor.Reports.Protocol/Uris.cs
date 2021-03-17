namespace Astor.Reports.Protocol
{
    public class Uris
    {
        public const string About = "about";

        public const string Rows = "rows";

        public const string Exports = "exports";

        public const string Csv = "csv";

        public const string Maps = "maps";

        public const string Pages = "pages";

        public const string Events = "events";

        public static string ReportPages(string reportId) => $"{reportId}/{Pages}";

        public static string ReportExportCsv(string reportId, string exportId) =>
            $"{ReportExport(reportId, exportId)}/{Csv}";

        public static string ReportExports(string reportId) => $"{reportId}/{Exports}";

        public static string ReportExport(string reportId, string exportId) => $"{reportId}/{Exports}/{exportId}";

        public static string ExportCsv(string reportId) => $"{reportId}/exports/csv";
        
        public static string ReportRows(string reportId) => $"{reportId}/{Rows}";

        public static string Event(string id) => $"{Events}/{id}";
    }
}