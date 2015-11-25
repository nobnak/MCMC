using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using MCMCSystem;

public class InstanceGenerator : MonoBehaviour {
	public float height = 1f;
	public float stddev = 0.01f;
	public int nInitials = 1000;
	public int nSamples = 1;
	public int nSkip = 100;

	public float sleep = 1f;

	public Texture2D probTex;
	public GameObject pointfab;

	private MCMC _mcmc;

	void Start () {
		_mcmc = new MCMC(probTex, stddev, height);
		StartCoroutine(Generate());
	}

	IEnumerator Generate() {
		while (true) {
			if (sleep <= 0f)
				yield return null;
			else
				yield return new WaitForSeconds(sleep);

			foreach (var uv in _mcmc.Sequence(nInitials, nSamples)) {
				var worldPos = Camera.main.ViewportToWorldPoint((Vector3)uv);
				worldPos.z = 0f;

				var go = (GameObject)Instantiate(pointfab);
				go.transform.parent = transform;
				go.transform.position = worldPos;
				go.SetActive(true);
			}
		}
	}
}
