using System;
using System.Collections.Generic;

namespace Hashing
{
    class HashTableChaining
    {
        private readonly Func<ulong, ulong> _hash;
        private readonly LinkedList<(ulong key, long value)>[] _table;

        public HashTableChaining(Func<ulong, ulong> hash, int l)
        {
            _hash = hash;
            _table = new LinkedList<(ulong key, long value)>[1 << l];
            for (int i = 0; i < _table.Length; i++)
                _table[i] = new LinkedList<(ulong key, long value)>();
        }

        public long Get(ulong x)
        {
            foreach (var entry in _table[_hash(x)])
                if (entry.key == x) return entry.value;
            return 0;
        }

        public void Set(ulong x, long v)
        {
            var list = _table[_hash(x)];
            for (var node = list.First; node != null; node = node.Next)
            {
                if (node.Value.key == x) { node.Value = (x, v); return; }
            }
            list.AddFirst((x, v));
        }

        public void Increment(ulong x, long d)
        {
            var list = _table[_hash(x)];
            for (var node = list.First; node != null; node = node.Next)
            {
                if (node.Value.key == x) { node.Value = (x, node.Value.value + d); return; }
            }
            list.AddFirst((x, d));
        }

        public IEnumerable<long> AllValues()
        {
            foreach (var bucket in _table)
                foreach (var entry in bucket)
                    yield return entry.value;
        }
    }
}
