using System;
using System.Collections.Generic;
using Astor.Reports.Protocol.Models;

namespace PickPoint.Reports.WebApi.Helpers
{
    public static class ExportBuckets
    {
        public static int MaxCount { get; set; } = 10;
        
        public static async IAsyncEnumerable<ExportBucket> PackAsync(IAsyncEnumerable<object> values, int minimalBucketLength)
        {
            var allocated = 0;
            object bucketStart = null;
            object bucketEnd = null;
            object previousValue = null;
            var canFinalizeBucket = false;
            var bucketIndex = 0;

            await foreach (var value in values)
            {
                var differsFromPrevious = !Equals(value, previousValue);

                if (canFinalizeBucket && differsFromPrevious)
                {
                    Console.WriteLine($"bucket {bucketIndex} packed");
                    yield return new ExportBucket
                    {
                        Start = bucketStart,
                        End = bucketEnd,
                        ElementsCount = allocated
                    };

                    bucketIndex++;
                    allocated = 0;
                    bucketStart = null;
                    canFinalizeBucket = false;
                }

                allocated++;
                bucketEnd = value;
                
                if (bucketStart == null)
                {
                    bucketStart = value;
                }
                
                if (allocated >= minimalBucketLength)
                {
                    canFinalizeBucket = true;
                }

                previousValue = value;
            }

            Console.WriteLine($"bucket {bucketIndex} packed");
            yield return new ExportBucket
            {
                Start = bucketStart,
                End = bucketEnd,
                ElementsCount = allocated
            };
        }
    }
}