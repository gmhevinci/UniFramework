using System;
using System.Collections;
using System.Collections.Generic;

namespace UniFramework.Utility
{
    [Serializable]
    public class StringPairs<T>
    {
        [Serializable]
        public struct Pair<V>
        {
            public string key;
            public V value;

            public Pair(string key_, V value_)
            {
                key = key_;
                value = value_;
            }
        }

        public List<Pair<T>> pairs = new List<Pair<T>>();

        public T this[string key]
        {
            get
            {
                int index = IndexOf(key);
                if (index > -1)
                {
                    return pairs[index].value;
                }
                else
                {
                    throw new Exception("not is hav key " + key);
                }
            }
            set
            {

                int index = IndexOf(key);
                if (index > -1)
                {
                    var temp = pairs[index];
                    temp.value = value;
                    pairs[index] = temp;
                }
                else
                {
                    throw new Exception("not is hav key " + key);
                }
            }
        }

        public Pair<T> this[int index]
        {
            get
            {
                return pairs[index];
            }
            set
            {
                pairs[index] = value;
            }
        }

        public int Count
        {
            get { return pairs.Count; }
        }

        public ValueCollection Values => new ValueCollection(this);

        public StringPairs()
        {
        }

        public StringPairs(StringPairs<T> value)
        {
            pairs = new List<Pair<T>>(value.pairs);
        }

        public void Add(string key, T value)
        {
            if (IndexOf(key) > -1)
            {
                throw new Exception("hav same key " + key);
            }
            else if (key == null)
            {
                throw new Exception("key not is null");
            }
            pairs.Add(new Pair<T>(key, value));
        }

        public void Remove(string key)
        {

            int index = IndexOf(key);
            if (index > -1) pairs.RemoveAt(index);
        }

        public bool ContainsKey(string key)
        {

            return IndexOf(key) > -1;
        }

        public bool TryGetValue(string key, out T value)
        {

            int index = IndexOf(key);
            if (index > -1)
            {
                value = pairs[index].value;
                return true;
            }
            else
            {
                value = default;
                return false;
            }
        }

        public int IndexOf(string key)
        {

            int index = -1;
            for (int i = 0; i < pairs.Count; i++)
            {
                if (pairs[i].key.Equals(key))
                {
                    index = i;
                    break;
                }
            }
            return index;
        }

        public void AddOrSet(string key, T value)
        {

            int index = IndexOf(key);
            if (index > -1)
            {
                var temp = pairs[index];
                temp.value = value;
                pairs[index] = temp;
            }
            else
            {
                pairs.Add(new Pair<T>(key, value));
            }
        }

        public void Clear()
        {
            pairs.Clear();
        }

        public override string ToString()
        {
            string info = string.Empty;

            foreach (var item in pairs)
            {
                info += "[" + item.key + "," + item.value + "]";
            }
            return info;
        }

        public sealed class ValueCollection : IEnumerable<T>, IEnumerator<T>
        {
            StringPairs<T> _pair;

            int cursor = -1;

            public ValueCollection(StringPairs<T> pair_)
            {
                _pair = pair_;
            }

            public T Current => _pair[cursor].value;

            object IEnumerator.Current => new ValueCollection(_pair);

            public void Dispose()
            {
                cursor = -1;
            }

            public IEnumerator<T> GetEnumerator()
            {
                for (int i = 0; i < _pair.Count; i++)
                {
                    yield return _pair[i].value;
                }
            }

            public bool MoveNext()
            {
                cursor++;
                return cursor < _pair.Count;
            }

            public void Reset()
            {
                cursor = -1;
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return new ValueCollection(_pair);
            }
        }
    }
}