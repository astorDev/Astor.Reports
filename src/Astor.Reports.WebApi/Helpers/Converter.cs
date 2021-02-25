using Newtonsoft.Json.Linq;
using Astor.Reports.Protocol.Models;

namespace PickPoint.Reports.WebApi.Helpers
{
    public static class Converter
    {
        public static RowsQuery ToQuery(this ExportConditions exportConditions)
        {
            var sortingField = exportConditions.GetSortingField();
            var projectionString = $"{{ '{sortingField}' : 1 }}";

            return new RowsQuery
            {
                Filter = exportConditions.Filter?.ToString(),
                Projection = projectionString,
                Sorting = exportConditions.Sort.ToString()
            };
        }

        public static string GetSortingField(this ExportConditions exportConditions)
        {
            var sortingProperty = ((JProperty) ((JObject)exportConditions.Sort).First);
            return sortingProperty.Name;
        }
        
    }
}