using UnityEngine;
using System.Collections.Generic;
using nobnak.Geometry;

public class MCMC : MonoBehaviour {
	public const string SHADER_DIST_TEX = "_DistTex";

	public float epsilon = 1e-6f;
	public float height = 1f;
	public float stddev = 0.01f;
	public Material samplesMat;
	public Color sampleColor;
	public int nInitials = 1000;
	public int nSamples = 100;

	private Texture2D _samplesTex;
	private Texture2D _distTex;
	private Vector2 _curr;
	private float _densityCurr;

	void Start () {
		_distTex = (Texture2D)samplesMat.GetTexture(SHADER_DIST_TEX);
		_samplesTex = new Texture2D(_distTex.width, _distTex.height, TextureFormat.RGB24, false, true);
		samplesMat.mainTexture = _samplesTex;

	}

	void Update () {
		_curr = new Vector2(Random.value, Random.value);
		_densityCurr = Density(_curr);

		for (var i = 0; i < nInitials; i++)
			Next();
		for (var i = 0; i < nSamples; i++) {
			Next();
			var x = Mathf.RoundToInt(_curr.x * _samplesTex.width);
			var y = Mathf.RoundToInt(_curr.y * _samplesTex.height);
			_samplesTex.SetPixel(x, y, sampleColor);
		}
		_samplesTex.Apply();
	}

	void Next() {
		var next = stddev * BoxMuller.Gaussian() + _curr;
		next.x -= Mathf.Floor(next.x);
		next.y -= Mathf.Floor(next.y);
		
		var densityNext = Density(next);
		if (Mathf.Min(1f, densityNext / _densityCurr) >= Random.value) {
			_curr = next;
			_densityCurr = densityNext;
		}
	}
	float Density(Vector2 curr) {
		return height * _distTex.GetPixelBilinear(curr.x, curr.y).r + epsilon;
	}
}
