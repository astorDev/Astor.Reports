using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Astor.Reports.Tests
{
    [TestClass]
    public class About_Should : Test
    {
        [TestMethod]
        public async Task ReturnValidMetadata()
        {
            var client = this.Factory.Create();

            var about = await client.GetAboutAsync();

            Assert.AreEqual("Astor.Reports - API отчётов", about.Description);
            Assert.AreEqual("1.0.0.0", about.Version);
            Assert.AreEqual("Development", about.Environment);
        }
    }
}