using UnityEngine;
using System.Collections;

namespace nobnak.Sampling {

    public static class BoxMuller {
        public const float TWO_PI = 2f * Mathf.PI;

        public static Vector2 Gaussian() {
            var u1 = Random.value;
            var u2 = Random.value;

            var sqrtLnU1 = Mathf.Sqrt(-2f * Mathf.Log(u1));
            return new Vector2(sqrtLnU1 * Mathf.Cos(TWO_PI * u2), sqrtLnU1 * Mathf.Sin(TWO_PI * u2));
        }
    }
}
