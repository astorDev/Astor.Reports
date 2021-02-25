using System;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Astor.Reports.Tests.UnsafePlayground
{
    [TestClass]
    [Ignore("Tests nothing - Playground")]
    public class Compressing
    {
        [TestMethod]
        public void CompressMe()
        {
            var bytes = Encoding.UTF8.GetBytes(
                "eJxTSsvMKUktUrJSqOZSgAMlz7yy/MzkVL/S3CSwpJKhqaWZsYmFhZGhhRJcYa0OnKlUnF9Ugm5KfGYKUEjXEKEBALEYF+A=");
            var base64 = Convert.ToBase64String(bytes);
            
            Console.WriteLine("just base 64");
            Console.WriteLine("RQAAAANmaWx0ZXIAJAAAAAJJbnZvaWNlTnVtYmVyAAwAAAAxNTk2MzQ4ODIxOAAAA3NvcnQADgAAABBfaWQA/////wAA");
            
            Console.WriteLine("base64 separately");
            Console.WriteLine("JAAAAAJJbnZvaWNlTnVtYmVyAAwAAAAxNTk2MzQ4ODIxOAAA-DgAAABBfaWQA/////wA=");
            
            Console.WriteLine("base64 after gzip");
            Console.WriteLine(base64);
        }
        
    }
}