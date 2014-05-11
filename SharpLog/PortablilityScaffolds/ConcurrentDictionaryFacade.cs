using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpLog.PortablilityScaffolds
{
    internal class ConcurrentDictionaryFacade<TKey, TValue> : ConcurrentDictionary<TKey, TValue>
    {
        public ConcurrentDictionaryFacade() {}
    }
}
