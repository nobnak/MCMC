using UnityEngine;
using System.Collections.Generic;
using nobnak.Sampling;
using System.Collections;

public class InstanceGenerator : MonoBehaviour {
	public float epsilon = 1e-6f;
	public float height = 1f;
	public float stddev = 0.01f;
	public int nInitials = 1000;
	public int nSamples = 1;
	public int nSkip = 100;

	public float sleep = 1f;

	public Texture2D probTex;
	public GameObject objfab;

	private MCMC _mcmc;

	void Start () {
		var aspect = Screen.width / Screen.height;
		_mcmc = new MCMC(probTex, stddev, aspect, height, epsilon);

		StartCoroutine(Generate());
	}

	IEnumerator Generate() {
		while (enabled) {
			if (sleep <= 0f)
				yield return null;
			else
				yield return new WaitForSeconds(sleep);

			foreach (var uv in _mcmc.Sequence(nInitials, nSamples)) {
				var worldPos = Camera.main.ViewportToWorldPoint((Vector3)uv);
				worldPos.z = 0f;

				var go = (GameObject)Instantiate(objfab);
				go.transform.parent = transform;
				go.transform.position = worldPos;
				go.SetActive(true);
			}
		}
	}
}
