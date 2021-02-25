using Newtonsoft.Json.Serialization;

namespace Astor.Reports.Data
{
    public class ReportsJsonContractResolver : DefaultContractResolver
    {
        protected override string ResolvePropertyName(string propertyName)
        {
            var n = base.ResolvePropertyName(propertyName);
            return n.Replace("*dollar*", "$");
        }
    }
}