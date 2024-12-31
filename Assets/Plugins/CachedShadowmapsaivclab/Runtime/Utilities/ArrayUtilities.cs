using System;
using System.Collections;
using System.Collections.Generic;

namespace CSM.Runtime.Utilities {

    // Helpers for builtin arrays ...
    // These are O(n) operations (where List<T>() is used) - the arrays are actually copied (http://msdn.microsoft.com/en-us/library/fkbw11z0.aspx)
    // but its pretty helpful for now
    public static class ArrayUtilities
    {
        //appends ''item'' to the end of ''array''
        public static void Add<T>(ref T[] array, T item)
        {
            Array.Resize(array : ref array, newSize : array.Length + 1);
            array[array.Length - 1] = item;
        }

        public static int PositiveMod(int x, int m) {
            return (x%m + m)%m;
        }

        //compares two arrays
        public static bool ArrayEquals<T>(T[] lhs, T[] rhs)
        {
            if (lhs == null || rhs == null) {
                return lhs == rhs;
            }

            if (lhs.Length != rhs.Length) {
                return false;
            }

            for (var i = 0; i < lhs.Length; i++)
            {
                if (!lhs[i].Equals(obj : rhs[i])) {
                    return false;
                }
            }
            return true;
        }

        //compares two arrays
        public static bool ArrayReferenceEquals<T>(T[] lhs, T[] rhs)
        {
            if (lhs == null || rhs == null) {
                return lhs == rhs;
            }

            if (lhs.Length != rhs.Length) {
                return false;
            }

            for (var i = 0; i < lhs.Length; i++)
            {
                if (!ReferenceEquals(objA : lhs[i], objB : rhs[i])) {
                    return false;
                }
            }
            return true;
        }

        //appends items to the end of array
        public static void AddRange<T>(ref T[] array, T[] items)
        {
            var size = array.Length;
            Array.Resize(array : ref array, newSize : array.Length + items.Length);
            for (var i = 0; i < items.Length; i++) {
                array[size + i] = items[i];
            }
        }

        //inserts item ''item'' at position ''index''
        public static void Insert<T>(ref T[] array, int index, T item)
        {
            var a = new ArrayList();
            a.AddRange(c : array);
            a.Insert(index : index, value : item);
            array = a.ToArray(type : typeof(T)) as T[];
        }

        //removes ''item'' from ''array''
        public static void Remove<T>(ref T[] array, T item)
        {
            var new_list = new List<T>(collection : array);
            new_list.Remove(item : item);
            array = new_list.ToArray();
        }

        public static List<T> FindAll<T>(T[] array, Predicate<T> match)
        {
            var list = new List<T>(collection : array);
            return list.FindAll(match : match);
        }

        public static T Find<T>(T[] array, Predicate<T> match)
        {
            var list = new List<T>(collection : array);
            return list.Find(match : match);
        }

        //Find the index of the first element that satisfies the predicate
        public static int FindIndex<T>(T[] array, Predicate<T> match)
        {
            var list = new List<T>(collection : array);
            return list.FindIndex(match : match);
        }

        //index of first element with value ''value''
        public static int IndexOf<T>(T[] array, T value)
        {
            var list = new List<T>(collection : array);
            return list.IndexOf(item : value);
        }

        //index of the last element with value ''value''
        public static int LastIndexOf<T>(T[] array, T value)
        {
            var list = new List<T>(collection : array);
            return list.LastIndexOf(item : value);
        }

        //remove element at position ''index''
        public static void RemoveAt<T>(ref T[] array, int index)
        {
            var list = new List<T>(collection : array);
            list.RemoveAt(index : index);
            array = list.ToArray();
        }

        //determines if the array contains the item
        public static bool Contains<T>(T[] array, T item)
        {
            var list = new List<T>(collection : array);
            return list.Contains(item : item);
        }

        //Clears the array
        public static void Clear<T>(ref T[] array)
        {
            Array.Clear(array : array, 0, length : array.Length);
            Array.Resize(array : ref array, 0);
        }
    }
}

