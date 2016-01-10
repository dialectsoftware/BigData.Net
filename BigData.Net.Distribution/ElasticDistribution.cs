using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BigData.Net;

namespace BigData.Net
{
    public sealed class ElasticDistribution
    {
        class Factor
        {
            public float Value;
            public float Multiplier;
            public UInt64 OffSet;

            public override bool Equals(object obj)
            {
                return Value.Equals(((Factor)obj).Value);
            }

            public override int GetHashCode()
            {
                return Value.GetHashCode();
            }
        }

        Factor[] factors;

        public ElasticDistribution(IEnumerable<float> values)
        {
            factors = values.Select((f, i) => new Factor() { Value = (f * 100f), OffSet = (UInt64)i, Multiplier = (100f / (f * 100f)) }).ToArray();
        }

        public int Evaluate(UInt64 hash)
        {
            float value = hash % 99.9999f;
            var delta = factors.Select(f =>
            {
                bool result = f.Value >= value && value >= 0;
                value -= f.Value;
                return result;
            });
            
            return delta.Select((f, i) =>
            {
                return f ? i : -1;
            })
            .Max();
        }

        public static double[] Evaluate(float[] distribution, int iterations = 100000)
        {
            float[] results = distribution.Select(i => 0f).ToArray();
            ElasticDistribution eval = new ElasticDistribution(distribution);

            double ii = 0;
            for (; ii < iterations; ii++)
            {
                var ccv = eval.Evaluate(Guid.NewGuid().GetBigHashCode());
                results[ccv] = results.ElementAt(ccv) + 1;
            }

            return results.Select(i => ((i / ii) * 100f)).ToArray();

        }

    }
}
