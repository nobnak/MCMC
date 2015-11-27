using System.Collections.Generic;
using System.Threading;



namespace UnityThreading {

	public static class Parallel {
		public static void For(int fromInclusive, int toExclusive, System.Action<int> body) {
			var numThreads = 2 * System.Environment.ProcessorCount;
			var resets = new ManualResetEvent[numThreads];
			for (var i = 0; i < numThreads; i++) {
				var reset = new ManualResetEvent(false);
				resets[i] = reset;
				ThreadPool.QueueUserWorkItem((j) => {
					for (var k = (int)j; k < toExclusive; k += numThreads)
						body((int)k);
					reset.Set();
				}, fromInclusive + i);
			}

			for (var i = 0; i < numThreads; i++)
				resets [i].WaitOne ();
		}
	}
}
