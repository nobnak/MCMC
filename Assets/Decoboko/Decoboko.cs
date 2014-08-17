using UnityEngine;
using System.Collections;

public class Decoboko : MonoBehaviour {
	public const string SHADER_RANDOM = "_Rand";

	public Material noiseMat;
	public FlowOut output;
	public bool debugOut;

	private RenderTexture _result;

	void OnDestroy() {
		Destroy(_result);
	}

	void Awake() {
		noiseMat.SetFloat(SHADER_RANDOM, Random.value);
	}

	void OnRenderImage(RenderTexture src, RenderTexture dst) {
		if (_result == null || _result.width != src.width || _result.height != src.height) {
			Destroy(_result);
			_result = new RenderTexture(src.width, src.height, 0, RenderTextureFormat.RFloat, RenderTextureReadWrite.Linear);
		}
		Graphics.Blit(null, _result, noiseMat);
		if (debugOut)
			Graphics.Blit(_result, dst);
	}

	[System.Serializable]
	public class FlowOut {
		public string name;
		public Material mat;
	}
}
