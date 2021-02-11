using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace TreeMap_Tutorial_CS
{
    public class TreeMap<K, V>
    {
        private IComparator<K> m_comparator;

        private Entry m_root;
        private int   m_count;

        public TreeMap(IComparator<K> comparator)
        {
            m_comparator = comparator;

            m_root  = null;
            m_count = 0;
        }

        private void PlaceInternal(Entry entry, Entry other)
        {
            Entry lastEntry = entry.Parent;
            Entry currEntry = entry;

            bool right = false;

            while(currEntry != null)
            {
                int comparison = m_comparator.Compare(other.Key, currEntry.Key);
                if(comparison < 0)
                {
                    lastEntry = currEntry;
                    currEntry = currEntry.Left;
                    right = false;
                }
                else if(comparison > 0)
                {
                    lastEntry = currEntry;
                    currEntry = currEntry.Right;
                    right = true;
                }
            }

            other.Parent = lastEntry;
            if(right)
                lastEntry.Right = other;
            else
                lastEntry.Left = other;
        }

        private Entry Get(K key)
        {
            Entry currEntry = m_root;

            while(currEntry != null)
            {
                int comparison = m_comparator.Compare(key, currEntry.Key);
                if(comparison < 0)
                    currEntry = currEntry.Left;
                else if(comparison > 0)
                    currEntry = currEntry.Right;
                else
                    return currEntry;
            }

            return null;
        }

        public int Count { get { return m_count; } }

        public V this[K key]
        {
            get
            {
                Entry foundEntry = Get(key);
                if(foundEntry == null)
                {
                    V result = default(V); 
                    Place(key, result);
                    return result;
                }
                return foundEntry.Value;
            }
            set
            {
                Entry foundEntry = Get(key);
                if(foundEntry == null)
                {
                    Place(key, value);
                    return;
                }
                foundEntry.Value = value;
            }
        }

        public bool Place(K key, V value)
        {
            Entry lastEntry = null;
            Entry currEntry = m_root;

            bool right = false;

            while(currEntry != null)
            {
                int comparison = m_comparator.Compare(key, currEntry.Key);
                if(comparison < 0)
                {
                    lastEntry = currEntry;
                    currEntry = currEntry.Left;
                    right = false;
                }
                else if(comparison > 0)
                {
                    lastEntry = currEntry;
                    currEntry = currEntry.Right;
                    right = true;
                }
                else
                {
                    currEntry.Value = value;
                    return true;
                }
            }

            Entry newEntry = new Entry(key, value, lastEntry);
            if(lastEntry == null)
                m_root = newEntry;
            else
            {
                if(right)
                    lastEntry.Right = newEntry;
                else
                    lastEntry.Left = newEntry;
            }
            m_count++;
            return false;
        }

        public bool Remove(K key)
        {
            Entry lastEntry = null;
            Entry currEntry = m_root;

            bool right = false;

            while(currEntry != null)
            {
                int comparison = m_comparator.Compare(key, currEntry.Key);
                if(comparison < 0)
                {
                    lastEntry = currEntry;
                    currEntry = currEntry.Left;
                    right = false;
                }
                else if(comparison > 0)
                {
                    lastEntry = currEntry;
                    currEntry = currEntry.Right;
                    right = true;
                }
                else
                {
                    Entry leftEntry  = currEntry.Left;
                    Entry rightEntry = currEntry.Right;

                    if(leftEntry != null)
                    {
                        if(lastEntry == null)
                            m_root = leftEntry;
                        else
                        {
                            if(right)
                                lastEntry.Right = leftEntry;
                            else
                                lastEntry.Left = leftEntry;
                        }
                        PlaceInternal(leftEntry, rightEntry);
                        return true;
                    }

                    if(lastEntry == null)
                        m_root = rightEntry;
                    else
                    {
                        if(right)
                            lastEntry.Right = rightEntry;
                        else
                            lastEntry.Left = rightEntry;
                    }
                    return true;
                }
            }

            return false;
        }

        public bool ContainsKey(K key) { return Get(key) != null; }

        public void Clear()
        {
            m_root  = null;
            m_count = 0;
        }

        public KeyCollection GetKeys(bool reverse = false) { return new KeyCollection(m_root, reverse); }
        public ValueCollection GetValues(bool reverse = false) { return new ValueCollection(m_root, reverse); }

        public EntryCollection GetEntries(bool reverse = false) { return new EntryCollection(m_root, reverse); }

        public class Entry
        {
            private K m_key;
            private V m_value;

            private Entry m_left;
            private Entry m_right;

            private Entry m_parent;

            public Entry(K key, V value, Entry parent)
            {
                m_key   = key;
                m_value = value;

                m_parent = parent;
            }

            public K Key { get { return m_key; } }

            public V Value
            {
                get { return m_value;  }
                set { m_value = value; }
            }

            public Entry Left
            {
                get { return m_left;  }
                set { m_left = value; }
            }

            public Entry Right
            {
                get { return m_right;  }
                set { m_right = value; }
            }

            public Entry Parent
            {
                get { return m_parent;  }
                set { m_parent = value; }
            }
        }

        public abstract class TreeIterator<T> : IEnumerator<T>
        {
            protected Entry m_root;
            protected Entry m_currEntry;
            protected Entry m_nextEntry;

            public TreeIterator(Entry root)
            {
                m_root = root;
                Reset();
            }

            public abstract T Current { get; }
            public abstract bool MoveNext();
            public abstract void Reset();

            public Entry CurrentEntry { get { return m_currEntry; } }

            object IEnumerator.Current { get { return Current; } }

            public void Dispose() {}
        }

        public abstract class ForwardIterator<T> : TreeIterator<T>
        {
            public ForwardIterator(Entry root) : base(root) {}

            public override bool MoveNext()
            {
                if(m_nextEntry == null)
                    return false;

                m_currEntry = m_nextEntry;
                if(m_nextEntry.Right != null)
                {
                    m_nextEntry = m_nextEntry.Right;
                    while(m_nextEntry.Left != null)
                        m_nextEntry = m_nextEntry.Left;

                    return true;
                }

                while(true)
                {
                    if(m_nextEntry.Parent == null)
                    {
                        m_nextEntry = null;
                        return true;
                    }
                    if(m_nextEntry.Parent.Left == m_nextEntry)
                    {
                        m_nextEntry = m_nextEntry.Parent;
                        return true;
                    }
                    m_nextEntry = m_nextEntry.Parent;
                }
            }

            public override void Reset()
            {
                m_nextEntry = m_root;
                if(m_nextEntry == null)
                    return;

                while(m_nextEntry.Left != null)
                    m_nextEntry = m_nextEntry.Left;
            }
        }

        public abstract class BackwardIterator<T> : TreeIterator<T>
        {
            public BackwardIterator(Entry root) : base(root) {}

            public override bool MoveNext()
            {
                if(m_nextEntry == null)
                    return false;

                m_currEntry = m_nextEntry;
                if(m_nextEntry.Left != null)
                {
                    m_nextEntry = m_nextEntry.Left;
                    while(m_nextEntry.Right != null)
                        m_nextEntry = m_nextEntry.Right;

                    return true;
                }

                while(true)
                {
                    if(m_nextEntry.Parent == null)
                    {
                        m_nextEntry = null;
                        return true;
                    }
                    if(m_nextEntry.Parent.Right == m_nextEntry)
                    {
                        m_nextEntry = m_nextEntry.Parent;
                        return true;
                    }
                    m_nextEntry = m_nextEntry.Parent;
                }
            }

            public override void Reset()
            {
                m_nextEntry = m_root;
                if(m_nextEntry == null)
                    return;

                while(m_nextEntry.Right != null)
                    m_nextEntry = m_nextEntry.Right;
            }
        }

        public class ForwardKeyIterator : ForwardIterator<K>
        {
            public ForwardKeyIterator(Entry root) : base(root) {}

            public override K Current { get { return CurrentEntry.Key; } }
        }

        public class ForwardValueIterator : ForwardIterator<V>
        {
            public ForwardValueIterator(Entry root) : base(root) {}

            public override V Current { get { return CurrentEntry.Value; } }
        }

        public class ForwardEntryIterator : ForwardIterator<Entry>
        {
            public ForwardEntryIterator(Entry root) : base(root) {}

            public override Entry Current { get { return CurrentEntry; } }
        }

        public class BackwardKeyIterator : BackwardIterator<K>
        {
            public BackwardKeyIterator(Entry root) : base(root) {}

            public override K Current { get { return CurrentEntry.Key; } }
        }

        public class BackwardValueIterator : BackwardIterator<V>
        {
            public BackwardValueIterator(Entry root) : base(root) {}

            public override V Current { get { return CurrentEntry.Value; } }
        }

        public class BackwardEntryIterator : BackwardIterator<Entry>
        {
            public BackwardEntryIterator(Entry root) : base(root) {}

            public override Entry Current { get { return CurrentEntry; } }
        }

        public class KeyCollection : IEnumerable<K>
        {
            private Entry m_root;
            private bool  m_reverse;

            public KeyCollection(Entry root, bool reverse)
            {
                m_root    = root;
                m_reverse = reverse;
            } 

            public IEnumerator<K> GetEnumerator()
            {
                if(!m_reverse)
                    return new ForwardKeyIterator(m_root);
                else
                    return new BackwardKeyIterator(m_root);
            }

            IEnumerator IEnumerable.GetEnumerator() { return GetEnumerator(); }
        }

        public class ValueCollection : IEnumerable<V>
        {
            private Entry m_root;
            private bool  m_reverse;

            public ValueCollection(Entry root, bool reverse)
            {
                m_root    = root;
                m_reverse = reverse;
            } 

            public IEnumerator<V> GetEnumerator()
            {
                if(!m_reverse)
                    return new ForwardValueIterator(m_root);
                else
                    return new BackwardValueIterator(m_root);
            }

            IEnumerator IEnumerable.GetEnumerator() { return GetEnumerator(); }
        }

        public class EntryCollection : IEnumerable<Entry>
        {
            private Entry m_root;
            private bool  m_reverse;

            public EntryCollection(Entry root, bool reverse)
            {
                m_root    = root;
                m_reverse = reverse;
            } 

            public IEnumerator<Entry> GetEnumerator()
            {
                if(!m_reverse)
                    return new ForwardEntryIterator(m_root);
                else
                    return new BackwardEntryIterator(m_root);
            }

            IEnumerator IEnumerable.GetEnumerator() { return GetEnumerator(); }
        }
    }
}
