using System;
using Astor.Reports.Protocol;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Astor.Reports.Tests.UnsafePlayground
{
    [TestClass]
    public class GetAllEventNames
    {
        [TestMethod]
        public void GetsAll()
        {
            var names = EventNames.GetAll();

            foreach (var name in names)
            {
                Console.WriteLine(name);
            }
        }
    }
}