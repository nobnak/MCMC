using UnityEngine;
using System.Collections;

namespace nobnak.Timer {

	public class Repeater {
		public float Interval { get; private set; }

		private float _tPrev;

		public Repeater(float interval) {
			this.Interval = interval;
			_tPrev = Time.timeSinceLevelLoad;
		}

		public bool Tick() {
			var t = Time.timeSinceLevelLoad;
			if ((t - _tPrev) >= Interval) {
				_tPrev = t;
				return true;
			}
			return false;
		}
	}
}