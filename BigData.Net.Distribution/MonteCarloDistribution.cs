using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BigData.Net;

namespace BigData.Net
{
    public sealed class MonteCarloDistribution<T>
    {
        ConsistentHash<Guid,int> hash = new ConsistentHash<Guid,int>();

        public MonteCarloDistribution(IEnumerable<int> distribution)
        {
            var keys = Evaluate(distribution);
            keys.Select((group, i) =>
            {
                group.ForEach(key =>
                {
                    hash.Add(key, distribution.ElementAt(i));
                });
                return group.Count();
            }).ToArray();
            hash.Commit();
        }

        public int Evaluate(T value)
        {
            return hash[value];
        }

        public static List<List<Guid>> Evaluate(IEnumerable<int> distribution, int iterations = 10000, int retry = 10000)
        {
            int[] ring = new int[100];
            List<List<Guid>> hypothesis = new List<List<Guid>>(distribution.Select(i => { return new List<Guid>(); }));

            for (int y = 0; y < retry; y++)
            {
                int idx = 0;
                int count = distribution.ElementAt(idx);
                ConsistentHash<Guid, int> hash = new ConsistentHash<Guid, int>();
                int[] results = distribution.Select(i => 0).ToArray();

                ring.Select((u, i) =>
                {
                    if (count > 0)
                    {
                        if (hypothesis.ElementAt(idx).Count() < distribution.ElementAt(idx))
                        {
                            Guid guid = Guid.NewGuid();
                            hash.Add(guid, idx);
                            hypothesis.ElementAt(idx).Add(guid);
                        }
                        else
                        {
                            hash.Add(hypothesis.ElementAt(idx).ElementAt(count - 1), idx);
                        }

                    }
                    else
                    {
                        if (++idx < distribution.Count())
                        {
                            count = distribution.ElementAt(idx);
                            if (hypothesis.ElementAt(idx).Count() < distribution.ElementAt(idx))
                            {
                                Guid g = Guid.NewGuid();
                                hash.Add(g, idx);
                                hypothesis.ElementAt(idx).Add(g);
                            }
                            else
                            {
                                hash.Add(hypothesis.ElementAt(idx).ElementAt(count - 1), idx);
                            }
                        }
                    }
                    return --count;

                }).ToArray();
                hash.Commit();

                double ii = 0;
                for (; ii < iterations; ii++)
                {
                    idx = hash[Guid.NewGuid()];
                    results[idx] = results.ElementAt(idx) + 1;
                }

                results = results.Select(i => (int)((i / ii) * 100)).ToArray();
                hypothesis = results.Select((result, i) =>
                {
                    if (Math.Abs((int)result - distribution.ElementAt(i)) <= 1)
                    {
                        return hypothesis.ElementAt(i);
                    }
                    else
                    {
                        hypothesis.ElementAt(i).Clear();
                        return hypothesis.ElementAt(i);
                    }
                }).ToList();

                if (hypothesis.Where(h => h.Count() == 0).Count() == 0)
                    break;
            }
            return hypothesis; 
        }

        public static int[] Validate(List<List<Guid>> keys, IEnumerable<int> distribution, int iterations = 100000)
        {
            int[] results = distribution.Select(i => 0).ToArray();
            ConsistentHash<Guid,int> hash = new ConsistentHash<Guid,int>();

            keys.Select((group, i) =>
            {
                group.ForEach(key =>
                {
                    hash.Add(key, i);
                });
                return group.Count();
            }).ToArray();
            hash.Commit();

            int idx = 0;
            double ii = 0;
            for (; ii < iterations; ii++)
            {
                idx = hash[Guid.NewGuid()];
                results[idx] = results.ElementAt(idx) + 1;
            }

            results = results.Select(i => (int)((i / ii) * 100)).ToArray();
            return results.Select((result, i) => {
                return distribution.ElementAt(i) - result;
            }).ToArray();
        }

        public static double[] Evaluate(List<List<Guid>> keys, IEnumerable<int> distribution, List<T> items)
        {
            int[] results = distribution.Select(i => 0).ToArray();
            ConsistentHash<Guid, int> hash = new ConsistentHash<Guid, int>();

            keys.Select((group, i) =>
            {
                group.ForEach(key =>
                {
                    hash.Add(key, i);
                });
                return group.Count();
            }).ToArray();
            hash.Commit();

            int idx = 0;
            double ii = items.Count();
            items.ForEach(item =>
            {
                idx = hash[item];
                results[idx] = results.ElementAt(idx) + 1;
            });

            return results.Select(i => ((i / ii) * 100)).ToArray();

        }

        public static double[] Evaluate(List<List<Guid>> keys, IEnumerable<int> distribution, int iterations = 100000)
        {
            int[] results = distribution.Select(i => 0).ToArray();
            ConsistentHash<Guid,int> hash = new ConsistentHash<Guid,int>();

            keys.Select((group, i) =>
            {
                group.ForEach(key =>
                {
                    hash.Add(key, i);
                });
                return group.Count();
            }).ToArray();
            hash.Commit();

            int idx = 0;
            double ii = 0;
            for (; ii < iterations; ii++)
            {
                idx = hash[Guid.NewGuid()];
                results[idx] = results.ElementAt(idx) + 1;
            }

          return results.Select(i => ((i / ii) * 100)).ToArray();
        
        }

        
    }
}
