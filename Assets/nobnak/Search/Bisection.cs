#define LOG_BISECTION
using UnityEngine;
using System.Collections.Generic;

namespace nobnak.Search {
    public static class Bisection {
        #if LOG_BISECTION
        public static readonly List<string> Log = new List<string>(new string[]{ "Bisection Log" });
        #endif

        public static int GreatestLowerBound(this float[] list, float value) {
            var index = GreatestLowerBound(list, 0, list.Length, value);
            #if LOG_BISECTION
            Log.Add(string.Format("{0}\t{1}", index, value));
            #endif
            return index;
        }

        private static int GreatestLowerBound(this float[] list, int leftmost, int length, float value) {
            if (length <= 1)
                return leftmost;

            var rightmost = leftmost + length - 1;
            var center = leftmost + ((length - 1) >> 1);
            var vCenter = list[center];
            var vNext = list[center + 1];

            if (value < vCenter)
                return GreatestLowerBound(list, leftmost, center - leftmost, value);
            if (vNext <= value)
                return GreatestLowerBound(list, center + 1, rightmost - center, value);
            return center;
        }

        public static void MakeCumulative(this float[] list) {
            var total = 0f;
            foreach (var v in list)
                total += v;
            var normalizer = 1f / total;

            var accum = 0f;
            for (var i = 0; i < list.Length; i++) {
                var val = list[i];
                list[i] = accum * normalizer;
                accum += val;
            }
        }
    }
}
