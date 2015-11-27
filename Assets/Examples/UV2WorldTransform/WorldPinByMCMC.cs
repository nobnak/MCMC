using UnityEngine;
using System.Collections;
using MCMCSystem;
using System.Collections.Generic;

public class WorldPinByMCMC : MonoBehaviour {
	public int limitPinCount = 10000;
	public float stdDev;
	public float perturbation = 0.1f;
	public Vector3 angularVelocity;
	public UVWorld uvWorld;
	public Transform pinfab;
	public Texture2D probability;

	MCMC _mcmc;
	List<Transform> _pins;

	void Start() {
		_mcmc = new MCMC(probability, stdDev);
		_pins = new List<Transform>();
		StartCoroutine(Pinning(0.01f, 20));
	}
	void Update() {
		transform.localRotation *= Quaternion.Euler(angularVelocity * Time.deltaTime);
	}

	IEnumerator Pinning(float interval, int count) {
		while (true) {
			yield return new WaitForSeconds(interval);

			Vector3 posLocal, normalLocal;
			foreach (var uv in _mcmc.Sequence(10, count)) {
				if (uvWorld.World(uv, out posLocal, out normalLocal)) {
					posLocal += (Vector3)(perturbation * Random.insideUnitCircle);
					var pin = Instantiate(pinfab);
					pin.SetParent(transform, false);
					pin.position = uvWorld.transform.TransformPoint(posLocal);
					pin.forward= uvWorld.transform.TransformDirection(normalLocal);

					_pins.Add(pin);
				}
			}

			if (_pins.Count >= limitPinCount) {
				foreach (var p in _pins)
					Destroy(p.gameObject);
				_pins.Clear();
				yield return new WaitForSeconds(2f);
			}
		}
	}
}
