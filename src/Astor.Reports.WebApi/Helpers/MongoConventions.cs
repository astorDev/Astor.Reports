using MongoDB.Bson.Serialization.Conventions;

namespace PickPoint.Reports.WebApi.Helpers
{
    public class MongoConventions
    {
        public static void Register(IConvention[] conventions)
        {
            var pack = new ConventionPack();
            pack.AddRange(conventions);
            ConventionRegistry.Register("myConventions", pack, t => true);
        }
    }
}