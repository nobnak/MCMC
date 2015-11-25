using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace MCMCSystem {

	public class MCMC {
		public const int LIMIT_RESET_LOOP_COUNT = 100;
		public Texture2D ProbTex { get; private set; }
		public float StdDev { get; private set; }
		public float Aspect { get; private set; }
		public float Height { get; private set; }

		private Vector2 _curr;
		private float _currDensity = 0f;
		private Vector2 _stddevAspect;

		public MCMC(Texture2D probTex, float stddev) : this(probTex, stddev, 1f) {}
		public MCMC(Texture2D probTex, float stddev, float height) {
			this.ProbTex = probTex;
			this.Height = height;
			this.StdDev = stddev;
		}

		public void Reset()	{
			for (var i = 0; _currDensity <= 0f && i < LIMIT_RESET_LOOP_COUNT; i++) {
				_curr = new Vector2 (Random.value, Random.value);
				_currDensity = Density (_curr);
			}
			Aspect = (float)ProbTex.width / ProbTex.height;
			_stddevAspect = new Vector2 (StdDev, StdDev / Aspect);
		}

		public IEnumerable<Vector2> Sequence(int nInitialize, int limit) {
			return Sequence(nInitialize, limit, 0);
		}
		public IEnumerable<Vector2> Sequence(int nInitialize, int limit, int nSkip) {
			Reset ();

			for (var i = 0; i < nInitialize; i++)
				Next();

			for (var i = 0; i < limit; i++) {
				for (var j = 0; j < nSkip; j++)
					Next();
				yield return _curr;
				Next ();
			}
		}
		
		void Next() {
			var next = Vector2.Scale(_stddevAspect, BoxMuller.Gaussian()) + _curr;
			next.x -= Mathf.Floor(next.x);
			next.y -= Mathf.Floor(next.y);
			
			var densityNext = Density(next);
			if (_currDensity <= 0f || Mathf.Min(1f, densityNext / _currDensity) >= Random.value) {
				_curr = next;
				_currDensity = densityNext;
			}
		}
		float Density(Vector2 curr) {
			return Height * ProbTex.GetPixelBilinear(curr.x, curr.y).r;
		}
	}
}
