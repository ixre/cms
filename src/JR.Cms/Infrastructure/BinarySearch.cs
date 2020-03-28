using System.Collections.Generic;

namespace JR.Cms.Infrastructure
{
    public delegate int BinarySeachCompare<T, JR>(T t, JR JR);

    public delegate JR BinarySearchGetValue<T, JR>(T t);

    /// <summary>
    /// 二分算法,  .NET已经包含　Array.BinarySearch
    /// </summary>
    public static class BinarySearch
    {
        public static int Search(int[] list, int low, int high, int find)
        {
            if (low > high) return -1;
            var mid = (low + high) / 2;
            if (mid > find)
                return Search(list, low, mid - 1, find);
            else if (mid < find) return Search(list, mid + 1, high, find);
            return list[mid];
        }

        /// <summary>
        /// 泛型二分
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="JR"></typeparam>
        /// <param name="list"></param>
        /// <param name="low"></param>
        /// <param name="high"></param>
        /// <param name="find"></param>
        /// <param name="compare"></param>
        /// <returns></returns>
        public static T TSearch<T, JR>(T[] list, int low, int high,
            JR find, BinarySeachCompare<T, JR> compare)
        {
            if (list == null || list.Length == 0) return default(T);
            if (low > high) return default(T);
            var mid = (low + high) / 2;
            if (compare(list[mid], find) > 0)
                return TSearch(list, low, mid - 1, find, compare);
            else if (compare(list[mid], find) < 0) return TSearch(list, mid + 1, high, find, compare);
            return list[mid];
        }

        public static T TSearch<T, JR>(IList<T> list, int low, int high,
            JR find, BinarySeachCompare<T, JR> compare)
        {
            if (list == null || list.Count == 0) return default(T);
            if (low > high) return default(T);
            var mid = (low + high) / 2;
            if (compare(list[mid], find) > 0)
                return TSearch(list, low, mid - 1, find, compare);
            else if (compare(list[mid], find) < 0) return TSearch(list, mid + 1, high, find, compare);
            return list[mid];
        }

        //Int二分
        public static T IntSearch<T>(T[] list, int low, int high,
            int find, BinarySearchGetValue<T, int> get)
        {
            if (list == null || list.Length == 0) return default(T);
            if (low > high) return default(T);
            var mid = (low + high) / 2;
            if (get(list[mid]) > find)
                return IntSearch(list, low, mid - 1, find, get);
            else if (get(list[mid]) < find) return IntSearch(list, mid + 1, high, find, get);
            return list[mid];
        }

        public static T IntSearch<T>(IList<T> list, int low, int high,
            int find, BinarySearchGetValue<T, int> get)
        {
            if (list == null || list.Count == 0) return default(T);
            if (low > high) return default(T);
            var mid = (low + high) / 2;
            if (mid >= list.Count) return default(T);
            if (get(list[mid]) > find)
                return IntSearch(list, low, mid - 1, find, get);
            else if (get(list[mid]) < find) return IntSearch(list, mid + 1, high, find, get);
            return list[mid];
        }
    }
}