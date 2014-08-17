using UnityEngine;
using System.Collections.Generic;

namespace nobnak.Timer {

	public class FPSMeter : MonoBehaviour {
		public Rect guiArea = new Rect(5f, 5f, 150f, 100f);

		private FPSLogger _logger = new FPSLogger();
		private float _fps;

		void OnGUI() {
			GUILayout.BeginArea(guiArea);
			GUILayout.BeginVertical();

			GUILayout.Label(string.Format("{0:f1} fps ", _fps));

			GUILayout.EndVertical();
			GUILayout.EndArea();
		}

		void Update() {
			_fps = _logger.Update();
		}
	}

	public class FPSLogger {
		public int AverageFrames { get; private set; }

		private Queue<Frame> _log;

		public FPSLogger() : this(100) {}
		public FPSLogger(int avgFrames) {
			this.AverageFrames = avgFrames;
			this._log = new Queue<Frame>(avgFrames);

			_log.Enqueue(new Frame(0, 0f));
		}

		public float Update() {
			var last = new Frame(Time.frameCount, Time.timeSinceLevelLoad);
			var first = (_log.Count < AverageFrames ? _log.Peek() : _log.Dequeue());
			_log.Enqueue(last);
			return (last.frameCount - first.frameCount) / (last.time - first.time);
		}

		struct Frame {
			public int frameCount;
			public float time;

			public Frame(int frameCount, float time) {
				this.frameCount = frameCount;
				this.time = time;
			}
		}
	}
}