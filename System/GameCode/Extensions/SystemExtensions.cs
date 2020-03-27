using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace VovTech
{
    public static class SystemExtensions
    {
        public static void ForEach<T>(this IEnumerable<T> source, Action<T> action)
        {
            foreach (T element in source)
            {
                action(element);
            }
        }

        public static T RandomItem<T>(this IList<T> list)
        {
            if (list.Count == 0) throw new IndexOutOfRangeException("Cannot select a random item from an empty list");
            return list[UnityEngine.Random.Range(0, list.Count)];
        }

        public static bool IsIn(this Enum someEnum, params Enum[] enumList)
        {
            if (enumList.Length == 0) return true;
            for(int i = 0; i < enumList.Length; i++)
            {
                if (someEnum.Equals(enumList[i])) return true;
            }
            return false;
        }
    }

    public class NavigationList<T> : List<T>
    {
        private int _currentIndex = 0;
        public int CurrentIndex
        {
            get
            {
                if (_currentIndex > Count - 1) { _currentIndex = Count - 1; }
                if (_currentIndex < 0) { _currentIndex = 0; }
                return _currentIndex;
            }
            set { _currentIndex = value; }
        }

        public T MoveNext
        {
            get
            {
                if (_currentIndex == Count - 1)
                    _currentIndex = 0;
                else
                    _currentIndex++;
                return this[CurrentIndex];
            }
        }

        public T MovePrevious
        {
            get
            {
                if (_currentIndex == 0)
                    _currentIndex = Count - 1;
                else
                    _currentIndex--;
                return this[CurrentIndex];
            }
        }

        public T Current
        {
            get { return this[CurrentIndex]; }
        }
    }
}