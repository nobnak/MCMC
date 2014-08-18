using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace nobnak.Sampling {

	public class MCMC {
		public Texture2D ProbTex { get; private set; }
		public float StdDev { get; private set; }
		public float Aspect { get; private set; }
		public float Height { get; private set; }
		public float Epsilon { get; private set; }

		private Vector2 _curr;
		private float _currDensity;
		private Vector2 _stddevAspect;

		public MCMC(Texture2D probTex, float stddev, float aspect) : this(probTex, stddev, aspect, 1f, 1e-6f) {}
		public MCMC(Texture2D probTex, float stddev, float aspect, float height, float epsilon) {
			this.ProbTex = probTex;
			this.Aspect = aspect;
			this.Height = height;
			this.Epsilon = epsilon;
			this.StdDev = stddev;
		}

		public IEnumerable<Vector2> Sequence(int nInitialize, int limit) {
			return Sequence(nInitialize, limit, 0);
		}
		public IEnumerable<Vector2> Sequence(int nInitialize, int limit, int nSkip) {
			_curr = new Vector2(Random.value, Random.value);
			_currDensity = Density(_curr);
			_stddevAspect = new Vector2(StdDev, StdDev / Aspect);

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
			if (Mathf.Min(1f, densityNext / _currDensity) >= Random.value) {
				_curr = next;
				_currDensity = densityNext;
			}
		}
		float Density(Vector2 curr) {
			return Height * ProbTex.GetPixelBilinear(curr.x, curr.y).r + Epsilon;
		}

	}
}