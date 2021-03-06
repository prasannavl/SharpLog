﻿// Author: Prasanna V. Loganathar
// Project: SharpLog
// 
// Copyright 2014 Launchark. All Rights Reserved.
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//  
// Created: 5:36 PM 08-05-2014

namespace SharpLog.PortabilityScaffolds
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Threading;

    internal class SplitOrderedList<TKey, T>
    {
        private const int MaxLoad = 5;
        private const uint BucketSize = 512;

        private static readonly byte[] reverseTable =
            {
                0, 128, 64, 192, 32, 160, 96, 224, 16, 144, 80, 208, 48, 176,
                112, 240, 8, 136, 72, 200, 40, 168, 104, 232, 24, 152, 88, 216,
                56, 184, 120, 248, 4, 132, 68, 196, 36, 164, 100, 228, 20, 148,
                84, 212, 52, 180, 116, 244, 12, 140, 76, 204, 44, 172, 108, 236,
                28, 156, 92, 220, 60, 188, 124, 252, 2, 130, 66, 194, 34, 162,
                98, 226, 18, 146, 82, 210, 50, 178, 114, 242, 10, 138, 74, 202,
                42, 170, 106, 234, 26, 154, 90, 218, 58, 186, 122, 250, 6, 134,
                70, 198, 38, 166, 102, 230, 22, 150, 86, 214, 54, 182, 118, 246,
                14, 142, 78, 206, 46, 174, 110, 238, 30, 158, 94, 222, 62, 190,
                126, 254, 1, 129, 65, 193, 33, 161, 97, 225, 17, 145, 81, 209,
                49, 177, 113, 241, 9, 137, 73, 201, 41, 169, 105, 233, 25, 153,
                89, 217, 57, 185, 121, 249, 5, 133, 69, 197, 37, 165, 101, 229,
                21, 149, 85, 213, 53, 181, 117, 245, 13, 141, 77, 205, 45, 173,
                109, 237, 29, 157, 93, 221, 61, 189, 125, 253, 3, 131, 67, 195,
                35, 163, 99, 227, 19, 147, 83, 211, 51, 179, 115, 243, 11, 139,
                75, 203, 43, 171, 107, 235, 27, 155, 91, 219, 59, 187, 123, 251,
                7, 135, 71, 199, 39, 167, 103, 231, 23, 151, 87, 215, 55, 183,
                119, 247, 15, 143, 79, 207, 47, 175, 111, 239, 31, 159, 95, 223,
                63, 191, 127, 255
            };

        private static readonly byte[] logTable =
            {
                0xFF, 0, 1, 1, 2, 2, 2, 2, 3, 3, 3, 3, 3, 3, 3, 3, 4, 4, 4, 4, 4, 4,
                4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5,
                5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 6, 6, 6, 6,
                6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6,
                6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6,
                6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 7, 7, 7, 7, 7, 7, 7, 7, 7,
                7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7,
                7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7,
                7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7,
                7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7,
                7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7,
                7, 7, 7, 7
            };

        private readonly IEqualityComparer<TKey> comparer;

        private Node head;
        private Node tail;

        private Node[] buckets = new Node[BucketSize];
        private int count;
        private int size = 2;

        private SimpleRwLock slim = new SimpleRwLock();

        public SplitOrderedList(IEqualityComparer<TKey> comparer)
        {
            this.comparer = comparer;
            head = new Node().Init(0);
            tail = new Node().Init(ulong.MaxValue);
            head.Next = tail;
            SetBucket(0, head);
        }

        public int Count
        {
            get
            {
                return count;
            }
        }

        public T InsertOrUpdate(uint key, TKey subKey, Func<T> addGetter, Func<T, T> updateGetter)
        {
            Node current;
            bool result = InsertInternal(key, subKey, default (T), addGetter, out current);

            if (result)
            {
                return current.Data;
            }

            // FIXME: this should have a CAS-like behavior
            return current.Data = updateGetter(current.Data);
        }

        public T InsertOrUpdate(uint key, TKey subKey, T addValue, T updateValue)
        {
            Node current;
            if (InsertInternal(key, subKey, addValue, null, out current))
            {
                return current.Data;
            }

            // FIXME: this should have a CAS-like behavior
            return current.Data = updateValue;
        }

        public bool Insert(uint key, TKey subKey, T data)
        {
            Node current;
            return InsertInternal(key, subKey, data, null, out current);
        }

        public T InsertOrGet(uint key, TKey subKey, T data, Func<T> dataCreator)
        {
            Node current;
            InsertInternal(key, subKey, data, dataCreator, out current);
            return current.Data;
        }

        private bool InsertInternal(uint key, TKey subKey, T data, Func<T> dataCreator, out Node current)
        {
            Node node = new Node().Init(ComputeRegularKey(key), subKey, data);

            uint b = key % (uint)size;
            Node bucket;

            if ((bucket = GetBucket(b)) == null)
            {
                bucket = InitializeBucket(b);
            }

            if (!ListInsert(node, bucket, out current, dataCreator))
            {
                return false;
            }

            int csize = size;
            if (Interlocked.Increment(ref count) / csize > MaxLoad && (csize & 0x40000000) == 0)
            {
                Interlocked.CompareExchange(ref size, 2 * csize, csize);
            }

            current = node;

            return true;
        }

        public bool Find(uint key, TKey subKey, out T data)
        {
            Node node;
            uint b = key % (uint)size;
            data = default (T);
            Node bucket;

            if ((bucket = GetBucket(b)) == null)
            {
                bucket = InitializeBucket(b);
            }

            if (!ListFind(ComputeRegularKey(key), subKey, bucket, out node))
            {
                return false;
            }

            data = node.Data;

            return !node.Marked;
        }

        public bool CompareExchange(uint key, TKey subKey, T data, Func<T, bool> check)
        {
            Node node;
            uint b = key % (uint)size;
            Node bucket;

            if ((bucket = GetBucket(b)) == null)
            {
                bucket = InitializeBucket(b);
            }

            if (!ListFind(ComputeRegularKey(key), subKey, bucket, out node))
            {
                return false;
            }

            if (!check(node.Data))
            {
                return false;
            }

            node.Data = data;

            return true;
        }

        public bool Delete(uint key, TKey subKey, out T data)
        {
            uint b = key % (uint)size;
            Node bucket;

            if ((bucket = GetBucket(b)) == null)
            {
                bucket = InitializeBucket(b);
            }

            if (!ListDelete(bucket, ComputeRegularKey(key), subKey, out data))
            {
                return false;
            }

            Interlocked.Decrement(ref count);
            return true;
        }

        public IEnumerator<T> GetEnumerator()
        {
            Node node = head.Next;

            while (node != tail)
            {
                while (node.Marked || (node.Key & 1) == 0)
                {
                    node = node.Next;
                    if (node == tail)
                    {
                        yield break;
                    }
                }
                yield return node.Data;
                node = node.Next;
            }
        }

        private Node InitializeBucket(uint b)
        {
            Node current;
            uint parent = GetParent(b);
            Node bucket;

            if ((bucket = GetBucket(parent)) == null)
            {
                bucket = InitializeBucket(parent);
            }

            Node dummy = new Node().Init(ComputeDummyKey(b));
            if (!ListInsert(dummy, bucket, out current, null))
            {
                return current;
            }

            return SetBucket(b, dummy);
        }

        // Turn v's MSB off
        private static uint GetParent(uint v)
        {
            uint t, tt;

            // Find MSB position in v
            var pos = (tt = v >> 16) > 0
                          ? (t = tt >> 8) > 0 ? 24 + logTable[t] : 16 + logTable[tt]
                          : (t = v >> 8) > 0 ? 8 + logTable[t] : logTable[v];

            return (uint)(v & ~(1 << pos));
        }

        // Reverse integer bits and make sure LSB is set
        private static ulong ComputeRegularKey(uint key)
        {
            return ComputeDummyKey(key) | 1;
        }

        // Reverse integer bits
        private static ulong ComputeDummyKey(uint key)
        {
            return
                ((ulong)
                 (((uint)reverseTable[key & 0xff] << 24) | ((uint)reverseTable[(key >> 8) & 0xff] << 16)
                  | ((uint)reverseTable[(key >> 16) & 0xff] << 8) | ((uint)reverseTable[(key >> 24) & 0xff]))) << 1;
        }

        // Bucket storage is abstracted in a simple two-layer tree to avoid too much memory resize
        private Node GetBucket(uint index)
        {
            if (index >= buckets.Length)
            {
                return null;
            }
            return buckets[index];
        }

        private Node SetBucket(uint index, Node node)
        {
            try
            {
                slim.EnterReadLock();
                CheckSegment(index, true);

                Interlocked.CompareExchange(ref buckets[index], node, null);
                return buckets[index];
            }
            finally
            {
                slim.ExitReadLock();
            }
        }

        // When we run out of space for bucket storage, we use a lock-based array resize
        private void CheckSegment(uint segment, bool readLockTaken)
        {
            if (segment < buckets.Length)
            {
                return;
            }

            if (readLockTaken)
            {
                slim.ExitReadLock();
            }
            try
            {
                slim.EnterWriteLock();
                while (segment >= buckets.Length)
                {
                    Array.Resize(ref buckets, buckets.Length * 2);
                }
            }
            finally
            {
                slim.ExitWriteLock();
            }
            if (readLockTaken)
            {
                slim.EnterReadLock();
            }
        }

        private Node ListSearch(ulong key, TKey subKey, ref Node left, Node h)
        {
            Node leftNodeNext = null, rightNode = null;

            do
            {
                Node t = h;
                Node tNext = t.Next;
                do
                {
                    if (!tNext.Marked)
                    {
                        left = t;
                        leftNodeNext = tNext;
                    }
                    t = tNext.Marked ? tNext.Next : tNext;
                    if (t == tail)
                    {
                        break;
                    }

                    tNext = t.Next;
                }
                while (tNext.Marked || t.Key < key || (tNext.Key == key && !comparer.Equals(subKey, t.SubKey)));

                rightNode = t;

                if (leftNodeNext == rightNode)
                {
                    if (rightNode != tail && rightNode.Next.Marked)
                    {
                        continue;
                    }
                    else
                    {
                        return rightNode;
                    }
                }

                if (Interlocked.CompareExchange(ref left.Next, rightNode, leftNodeNext) == leftNodeNext)
                {
                    if (rightNode != tail && rightNode.Next.Marked)
                    {
                        continue;
                    }
                    else
                    {
                        return rightNode;
                    }
                }
            }
            while (true);
        }

        private bool ListDelete(Node startPoint, ulong key, TKey subKey, out T data)
        {
            Node rightNode = null, rightNodeNext = null, leftNode = null;
            data = default (T);
            Node markedNode = null;

            do
            {
                rightNode = ListSearch(key, subKey, ref leftNode, startPoint);
                if (rightNode == tail || rightNode.Key != key || !comparer.Equals(subKey, rightNode.SubKey))
                {
                    return false;
                }

                data = rightNode.Data;
                rightNodeNext = rightNode.Next;

                if (!rightNodeNext.Marked)
                {
                    if (markedNode == null)
                    {
                        markedNode = new Node();
                    }
                    markedNode.Init(rightNodeNext);

                    if (Interlocked.CompareExchange(ref rightNode.Next, markedNode, rightNodeNext) == rightNodeNext)
                    {
                        break;
                    }
                }
            }
            while (true);

            if (Interlocked.CompareExchange(ref leftNode.Next, rightNodeNext, rightNode) != rightNode)
            {
                ListSearch(rightNode.Key, subKey, ref leftNode, startPoint);
            }

            return true;
        }

        private bool ListInsert(Node newNode, Node startPoint, out Node current, Func<T> dataCreator)
        {
            ulong key = newNode.Key;
            Node rightNode = null, leftNode = null;

            do
            {
                rightNode = current = ListSearch(key, newNode.SubKey, ref leftNode, startPoint);
                if (rightNode != tail && rightNode.Key == key && comparer.Equals(newNode.SubKey, rightNode.SubKey))
                {
                    return false;
                }

                newNode.Next = rightNode;
                if (dataCreator != null)
                {
                    newNode.Data = dataCreator();
                }
                if (Interlocked.CompareExchange(ref leftNode.Next, newNode, rightNode) == rightNode)
                {
                    return true;
                }
            }
            while (true);
        }

        private bool ListFind(ulong key, TKey subKey, Node startPoint, out Node data)
        {
            Node rightNode = null, leftNode = null;
            data = null;

            rightNode = ListSearch(key, subKey, ref leftNode, startPoint);
            data = rightNode;

            return rightNode != tail && rightNode.Key == key && comparer.Equals(subKey, rightNode.SubKey);
        }

        private class Node
        {
            public bool Marked;
            public ulong Key;
            public TKey SubKey;
            public T Data;
            public Node Next;

            public Node Init(ulong key, TKey subKey, T data)
            {
                Key = key;
                SubKey = subKey;
                Data = data;

                Marked = false;
                Next = null;

                return this;
            }

            // Used to create dummy node
            public Node Init(ulong key)
            {
                Key = key;
                Data = default (T);

                Next = null;
                Marked = false;
                SubKey = default (TKey);

                return this;
            }

            // Used to create marked node
            public Node Init(Node wrapped)
            {
                Marked = true;
                Next = wrapped;

                Key = 0;
                Data = default (T);
                SubKey = default (TKey);

                return this;
            }
        }

        private struct SimpleRwLock
        {
            private const int RwWait = 1;
            private const int RwWrite = 2;
            private const int RwRead = 4;

            private int rwlock;

            public void EnterReadLock()
            {
                SpinWait sw = new SpinWait();
                do
                {
                    while ((rwlock & (RwWrite | RwWait)) > 0)
                    {
                        sw.SpinOnce();
                    }

                    if ((Interlocked.Add(ref rwlock, RwRead) & (RwWait | RwWait)) == 0)
                    {
                        return;
                    }

                    Interlocked.Add(ref rwlock, -RwRead);
                }
                while (true);
            }

            public void ExitReadLock()
            {
                Interlocked.Add(ref rwlock, -RwRead);
            }

            public void EnterWriteLock()
            {
                SpinWait sw = new SpinWait();
                do
                {
                    int state = rwlock;
                    if (state < RwWrite)
                    {
                        if (Interlocked.CompareExchange(ref rwlock, RwWrite, state) == state)
                        {
                            return;
                        }
                        state = rwlock;
                    }
                    // We register our interest in taking the Write lock (if upgradeable it's already done)
                    while ((state & RwWait) == 0
                           && Interlocked.CompareExchange(ref rwlock, state | RwWait, state) != state)
                    {
                        state = rwlock;
                    }
                    // Before falling to sleep
                    while (rwlock > RwWait)
                    {
                        sw.SpinOnce();
                    }
                }
                while (true);
            }

            public void ExitWriteLock()
            {
                Interlocked.Add(ref rwlock, -RwWrite);
            }
        }
    }

    public class ConcurrentDictionary<TKey, TValue> : ICollection<KeyValuePair<TKey, TValue>>,
                                                      IEnumerable<KeyValuePair<TKey, TValue>>,
                                                      IDictionary,
                                                      ICollection,
                                                      IEnumerable,
                                                      IDictionary<TKey, TValue>
    {
        private IEqualityComparer<TKey> comparer;

        private SplitOrderedList<TKey, KeyValuePair<TKey, TValue>> internalDictionary;

        public ConcurrentDictionary()
            : this(EqualityComparer<TKey>.Default)
        {
        }

        public ConcurrentDictionary(IEnumerable<KeyValuePair<TKey, TValue>> collection)
            : this(collection, EqualityComparer<TKey>.Default)
        {
        }

        public ConcurrentDictionary(IEqualityComparer<TKey> comparer)
        {
            this.comparer = comparer;
            internalDictionary = new SplitOrderedList<TKey, KeyValuePair<TKey, TValue>>(comparer);
        }

        public ConcurrentDictionary(
            IEnumerable<KeyValuePair<TKey, TValue>> collection,
            IEqualityComparer<TKey> comparer)
            : this(comparer)
        {
            foreach (KeyValuePair<TKey, TValue> pair in collection)
            {
                Add(pair.Key, pair.Value);
            }
        }

        // Parameters unused
        public ConcurrentDictionary(int concurrencyLevel, int capacity)
            : this(EqualityComparer<TKey>.Default)
        {
        }

        public ConcurrentDictionary(
            int concurrencyLevel,
            IEnumerable<KeyValuePair<TKey, TValue>> collection,
            IEqualityComparer<TKey> comparer)
            : this(collection, comparer)
        {
        }

        // Parameters unused
        public ConcurrentDictionary(int concurrencyLevel, int capacity, IEqualityComparer<TKey> comparer)
            : this(comparer)
        {
        }

        public bool IsEmpty
        {
            get
            {
                return Count == 0;
            }
        }

        void ICollection<KeyValuePair<TKey, TValue>>.Add(KeyValuePair<TKey, TValue> pair)
        {
            Add(pair.Key, pair.Value);
        }

        bool ICollection<KeyValuePair<TKey, TValue>>.Remove(KeyValuePair<TKey, TValue> pair)
        {
            return Remove(pair.Key);
        }

        bool ICollection<KeyValuePair<TKey, TValue>>.Contains(KeyValuePair<TKey, TValue> pair)
        {
            return ContainsKey(pair.Key);
        }

        public void Clear()
        {
            // Pronk
            internalDictionary = new SplitOrderedList<TKey, KeyValuePair<TKey, TValue>>(comparer);
        }

        public int Count
        {
            get
            {
                return internalDictionary.Count;
            }
        }

        bool ICollection<KeyValuePair<TKey, TValue>>.IsReadOnly
        {
            get
            {
                return false;
            }
        }

        void ICollection<KeyValuePair<TKey, TValue>>.CopyTo(KeyValuePair<TKey, TValue>[] array, int startIndex)
        {
            CopyTo(array, startIndex);
        }

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            return GetEnumeratorInternal();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return (IEnumerator)GetEnumeratorInternal();
        }

        bool IDictionary.Contains(object key)
        {
            if (!(key is TKey))
            {
                return false;
            }

            return ContainsKey((TKey)key);
        }

        void IDictionary.Remove(object key)
        {
            if (!(key is TKey))
            {
                return;
            }

            Remove((TKey)key);
        }

        object IDictionary.this[object key]
        {
            get
            {
                TValue obj;
                if (key is TKey && TryGetValue((TKey)key, out obj))
                {
                    return obj;
                }
                return null;
            }
            set
            {
                if (!(key is TKey) || !(value is TValue))
                {
                    throw new ArgumentException("key or value aren't of correct type");
                }

                this[(TKey)key] = (TValue)value;
            }
        }

        void IDictionary.Add(object key, object value)
        {
            if (!(key is TKey) || !(value is TValue))
            {
                throw new ArgumentException("key or value aren't of correct type");
            }

            Add((TKey)key, (TValue)value);
        }

        bool IDictionary.IsReadOnly
        {
            get
            {
                return false;
            }
        }

        ICollection IDictionary.Keys
        {
            get
            {
                return (ICollection)Keys;
            }
        }

        ICollection IDictionary.Values
        {
            get
            {
                return (ICollection)Values;
            }
        }

        void ICollection.CopyTo(Array array, int startIndex)
        {
            var arr = array as KeyValuePair<TKey, TValue>[];
            if (arr == null)
            {
                return;
            }

            CopyTo(arr, startIndex, Count);
        }

        IDictionaryEnumerator IDictionary.GetEnumerator()
        {
            return new ConcurrentDictionaryEnumerator(GetEnumeratorInternal());
        }

        object ICollection.SyncRoot
        {
            get
            {
                return this;
            }
        }

        bool IDictionary.IsFixedSize
        {
            get
            {
                return false;
            }
        }

        bool ICollection.IsSynchronized
        {
            get
            {
                return true;
            }
        }

        void IDictionary<TKey, TValue>.Add(TKey key, TValue value)
        {
            Add(key, value);
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            CheckKey(key);
            KeyValuePair<TKey, TValue> pair;
            bool result = internalDictionary.Find(Hash(key), key, out pair);
            value = pair.Value;

            return result;
        }

        public TValue this[TKey key]
        {
            get
            {
                return GetValue(key);
            }
            set
            {
                AddOrUpdate(key, value, value);
            }
        }

        bool IDictionary<TKey, TValue>.Remove(TKey key)
        {
            return Remove(key);
        }

        public bool ContainsKey(TKey key)
        {
            CheckKey(key);
            KeyValuePair<TKey, TValue> dummy;
            return internalDictionary.Find(Hash(key), key, out dummy);
        }

        public ICollection<TKey> Keys
        {
            get
            {
                return GetPart<TKey>((kvp) => kvp.Key);
            }
        }

        public ICollection<TValue> Values
        {
            get
            {
                return GetPart<TValue>((kvp) => kvp.Value);
            }
        }

        public bool TryAdd(TKey key, TValue value)
        {
            CheckKey(key);
            return internalDictionary.Insert(Hash(key), key, Make(key, value));
        }

        public TValue AddOrUpdate(
            TKey key,
            Func<TKey, TValue> addValueFactory,
            Func<TKey, TValue, TValue> updateValueFactory)
        {
            CheckKey(key);
            if (addValueFactory == null)
            {
                throw new ArgumentNullException("addValueFactory");
            }
            if (updateValueFactory == null)
            {
                throw new ArgumentNullException("updateValueFactory");
            }
            return
                internalDictionary.InsertOrUpdate(
                    Hash(key),
                    key,
                    () => Make(key, addValueFactory(key)),
                    (e) => Make(key, updateValueFactory(key, e.Value))).Value;
        }

        public TValue AddOrUpdate(TKey key, TValue addValue, Func<TKey, TValue, TValue> updateValueFactory)
        {
            return AddOrUpdate(key, (_) => addValue, updateValueFactory);
        }

        public bool TryUpdate(TKey key, TValue newValue, TValue comparisonValue)
        {
            CheckKey(key);
            return internalDictionary.CompareExchange(
                Hash(key),
                key,
                Make(key, newValue),
                (e) => e.Value.Equals(comparisonValue));
        }

        public TValue GetOrAdd(TKey key, Func<TKey, TValue> valueFactory)
        {
            CheckKey(key);
            return
                internalDictionary.InsertOrGet(
                    Hash(key),
                    key,
                    Make(key, default(TValue)),
                    () => Make(key, valueFactory(key))).Value;
        }

        public TValue GetOrAdd(TKey key, TValue value)
        {
            CheckKey(key);
            return internalDictionary.InsertOrGet(Hash(key), key, Make(key, value), null).Value;
        }

        public bool TryRemove(TKey key, out TValue value)
        {
            CheckKey(key);
            KeyValuePair<TKey, TValue> data;
            bool result = internalDictionary.Delete(Hash(key), key, out data);
            value = data.Value;
            return result;
        }

        public KeyValuePair<TKey, TValue>[] ToArray()
        {
            // This is most certainly not optimum but there is
            // not a lot of possibilities

            return new List<KeyValuePair<TKey, TValue>>(this).ToArray();
        }

        private void CheckKey(TKey key)
        {
            if (key == null)
            {
                throw new ArgumentNullException("key");
            }
        }

        private void Add(TKey key, TValue value)
        {
            while (!TryAdd(key, value))
            {
                ;
            }
        }

        private TValue AddOrUpdate(TKey key, TValue addValue, TValue updateValue)
        {
            CheckKey(key);
            return internalDictionary.InsertOrUpdate(Hash(key), key, Make(key, addValue), Make(key, updateValue)).Value;
        }

        private TValue GetValue(TKey key)
        {
            TValue temp;
            if (!TryGetValue(key, out temp))
            {
                throw new KeyNotFoundException(key.ToString());
            }
            return temp;
        }

        private bool Remove(TKey key)
        {
            TValue dummy;

            return TryRemove(key, out dummy);
        }

        private ICollection<T> GetPart<T>(Func<KeyValuePair<TKey, TValue>, T> extractor)
        {
            var temp = new List<T>();

            foreach (KeyValuePair<TKey, TValue> kvp in this)
            {
                temp.Add(extractor(kvp));
            }

            return new ReadOnlyCollection<T>(temp);
        }

        private void CopyTo(KeyValuePair<TKey, TValue>[] array, int startIndex)
        {
            CopyTo(array, startIndex, Count);
        }

        private void CopyTo(KeyValuePair<TKey, TValue>[] array, int startIndex, int num)
        {
            foreach (var kvp in this)
            {
                array[startIndex++] = kvp;

                if (--num <= 0)
                {
                    return;
                }
            }
        }

        private IEnumerator<KeyValuePair<TKey, TValue>> GetEnumeratorInternal()
        {
            return internalDictionary.GetEnumerator();
        }

        private static KeyValuePair<U, V> Make<U, V>(U key, V value)
        {
            return new KeyValuePair<U, V>(key, value);
        }

        private uint Hash(TKey key)
        {
            return (uint)comparer.GetHashCode(key);
        }

        private class ConcurrentDictionaryEnumerator : IDictionaryEnumerator
        {
            private IEnumerator<KeyValuePair<TKey, TValue>> internalEnum;

            public ConcurrentDictionaryEnumerator(IEnumerator<KeyValuePair<TKey, TValue>> internalEnum)
            {
                this.internalEnum = internalEnum;
            }

            public bool MoveNext()
            {
                return internalEnum.MoveNext();
            }

            public void Reset()
            {
                internalEnum.Reset();
            }

            public object Current
            {
                get
                {
                    return Entry;
                }
            }

            public DictionaryEntry Entry
            {
                get
                {
                    KeyValuePair<TKey, TValue> current = internalEnum.Current;
                    return new DictionaryEntry(current.Key, current.Value);
                }
            }

            public object Key
            {
                get
                {
                    return internalEnum.Current.Key;
                }
            }

            public object Value
            {
                get
                {
                    return internalEnum.Current.Value;
                }
            }
        }
    }

    internal static class MetadataServices
    {
        public static readonly IDictionary<string, object> EmptyMetadata = new ReadOnlyDictionary<string, object>(null);

        public static IDictionary<string, object> AsReadOnly(this IDictionary<string, object> metadata)
        {
            if (metadata == null)
            {
                return EmptyMetadata;
            }

            if (metadata is ReadOnlyDictionary<string, object>)
            {
                return metadata;
            }

            return new ReadOnlyDictionary<string, object>(metadata);
        }
    }
}