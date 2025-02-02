﻿using System;
using System.Collections;
using System.Collections.Generic;

using System.Linq;

namespace Common
{
    public class PriorityQueue<T> where T : IComparable<T>
    {
        private List<T> list;
        private IComparer<T> comparer;
        
        public PriorityQueue()
        {
            list = new List<T>();
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
            list.Sort();
        }

        public List<T> ToList()
        {
            return new List<T>(list);
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