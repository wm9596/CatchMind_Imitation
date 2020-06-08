using System.Collections;
using System.Collections.Generic;

using System.Linq;

namespace Common
{
    public class PriorityQueue<T>
    {
        private List<T> list;
        private IComparer<T> comparer;
      

        public PriorityQueue(IComparer<T> comparer)
        {
            list = new List<T>();
            this.comparer = comparer;
        }

        public int GetLength()
        {
            return list.Count;
        }

        public bool IsEmpty()
        {
            return list.Count <= 0;
        }

        public void Add(T item)
        {
            list.Add(item);
            list.Sort(comparer);
        }

        public List<T> ToList()
        {
            return list;
        }

        public T Dequeue()
        {
            if (!IsEmpty())
            {
                var item = list[0];
                list.RemoveAt(0);
                return item;
            }
            else
            {
                return default;
            }
        }
    }
}