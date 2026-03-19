using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AYellowpaper
{
    [System.Serializable]
    public class InterfaceComponentList<I, U> where U : UnityEngine.Object where I : class
    {
        [SerializeField] List<U> list = new();

        public I this[int i]
        {
            get => list[i] as I;
            set => list[i] = value as U;
        }

        public int Count => list.Count;

        public void Add(I item) => list.Add(item as U);
        public void AddU(U item) => list.Add(item);
        public void Clear() => list.Clear();
        public bool Contains(I item) => list.Contains(item as U);
        public bool ContainsU(U item) => list.Contains(item);
        public void CopyTo(I[] array, int arrayIndex) => list.CopyTo(array as U[], arrayIndex);
        public void CopyToU(U[] array, int arrayIndex) => list.CopyTo(array, arrayIndex);
        public IEnumerator<U> GetUnityEnumerator() => list.GetEnumerator();
        public IEnumerator<I> GetInterfaceEnumerator() => list.GetEnumerator() as IEnumerator<I>;
        public int IndexOf(I item) => list.IndexOf(item as U);
        public int IndexOfU(U item) => list.IndexOf(item);
        public void Insert(int index, I item) => list.Insert(index, item as U);
        public void InsertU(int index, U item) => list.Insert(index, item);
        public bool Remove(I item) => list.Remove(item as U);
        public bool RemoveU(U item) => list.Remove(item);
        public void RemoveAt(int index) => list.RemoveAt(index);

        public void ClearNull()
        {
            for (int i = list.Count - 1; i >= 0; i--)
                if (list[i] == null)
                    list.RemoveAt(i);
        }
    }
}