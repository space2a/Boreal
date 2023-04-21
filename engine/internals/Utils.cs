using System;
using System.Collections.Generic;

namespace boreal.engine
{
    public static class Utils
    {

        public static T[] ResizeStaticArray<T>(T[] array, int size)
        {
            Array.Resize(ref array, size);
            return array;
        }

        public static bool AreEventsSubscribed(EventHandler[] eventHandlers)
        {
            bool result = false;

            for (int i = 0; i < eventHandlers.Length; i++)
            {
                if (eventHandlers[i] != null) result = true;
            }

            return result;
        }

        public static T GetItem<T>(T[] array, int index)
        {
            if (index >= array.Length) return array[index %  array.Length];
            else if (index < 0) return array[index % array.Length + array.Length];
            else return array[index];
        }

        public static T GetItem<T>(List<T> list, int index)
        {
            if (index >= list.Count) return list[index %  list.Count];
            else if (index < 0) return list[index % list.Count + list.Count];
            else return list[index];
        }

    }
}
