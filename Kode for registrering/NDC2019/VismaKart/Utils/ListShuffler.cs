using System;
using System.Collections.Generic;

namespace VismaKart.Utils
{
    public static class ListShuffler
    {
        private static readonly Random Rnd = new Random();

        public static void Shuffle<T>(this IList<T> list)
        {
            var n = list.Count;
            while (n > 1)
            {
                var k = (Rnd.Next(0, n) % n);
                n--;
                var value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }
    }
}
