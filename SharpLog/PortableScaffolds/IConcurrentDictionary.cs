using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpLog.PortableScaffolds
{
    public interface IConcurrentDictionary<TKey, TValue> : IDictionary<TKey, TValue>
    {
        bool IsEmpty { get; }
        bool TryAdd(TKey key, TValue value);

        TValue AddOrUpdate(
            TKey key,
            Func<TKey, TValue> addValueFactory,
            Func<TKey, TValue, TValue> updateValueFactory);

        TValue AddOrUpdate(TKey key, TValue addValue, Func<TKey, TValue, TValue> updateValueFactory);
        bool TryUpdate(TKey key, TValue newValue, TValue comparisonValue);
        TValue GetOrAdd(TKey key, Func<TKey, TValue> valueFactory);
        TValue GetOrAdd(TKey key, TValue value);
        bool TryRemove(TKey key, out TValue value);
        KeyValuePair<TKey, TValue>[] ToArray();
    }
}
