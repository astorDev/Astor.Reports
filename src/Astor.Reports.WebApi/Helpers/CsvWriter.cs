using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace PickPoint.Reports.WebApi.Helpers
{
    public class CsvWriter
    {
        public StreamWriter Writer { get; }


        public CsvWriter(StreamWriter writer)
        {
            this.Writer = writer;
        }

        public void WriteHeader(IEnumerable<string> columnNames)
        {
            this.Writer.WriteLine(String.Join(",", columnNames));
        }

        public void WriteRows(IEnumerable<string> order, IEnumerable<dynamic> rows)
        {
            foreach (var row in rows)
            {
                this.WriteRow(order, row);
            } 
        }

        public void WriteRow(IEnumerable<string> columnsOrder, dynamic row)
        {
            var values = columnsOrder.Select(valuePath => row[valuePath]?.ToString()).Cast<string>();
            this.Writer.WriteLine(String.Join(",", values));
        }

        public static void Join(IEnumerable<string> filePaths, string resultFilePath, IEnumerable<string> columnNames)
        {
            using var writer = new StreamWriter(resultFilePath);
            var csvWriter = new CsvWriter(writer);
            csvWriter.WriteHeader(columnNames);

            foreach (var path in filePaths)
            {
                using var streamReader = new StreamReader(path);
                string line;
                while ((line = streamReader.ReadLine()) != null)
                {
                    writer.WriteLine(line);   
                }
            }
        }
    }
}