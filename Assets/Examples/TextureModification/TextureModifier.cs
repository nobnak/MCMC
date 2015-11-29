using UnityEngine;
using System.Collections;
using UnityThreading;
using MCMCSystem;

[RequireComponent(typeof(Renderer))]
public class TextureModifier : MonoBehaviour {
	public const string PROP_MAIN_TEX = "_MainTex";

	public float radius = 100f;
	public float stdDev = 0.1f;
	public float perturbation = 0.01f;
	public Camera targetCamera;
	public UVWorld uvWorld;
	public Collider whiteboard;
	public ParticleSystem shuriken;
	public Texture2D input;
	public Texture2D output;

	int _width, _height, _pixelCount;
	bool _mousePressed;
	MCMC _mcmc;
	Color[] _inputs, _outputs;
	Vector2 _texSize;
	Renderer _rnd;
	MaterialPropertyBlock _block;

	void Start () {
		_rnd = GetComponent<Renderer>();
		_rnd.GetPropertyBlock(_block = new MaterialPropertyBlock());

		_inputs = input.GetPixels();

		_width = input.width;
		_height = input.height;
		_pixelCount = _inputs.Length;
		_texSize = new Vector2(_width, _height);

		output = new Texture2D(_width, _height, TextureFormat.ARGB32, false);
		_outputs = input.GetPixels();
		output.SetPixels(_outputs);
		output.Apply();
		
		_block.SetTexture(PROP_MAIN_TEX, output);
		_rnd.SetPropertyBlock(_block);
		_mcmc = new MCMC(output, stdDev);

		StartCoroutine (Pinning (0.01f, 200));
	}
	void Update () {
		RaycastHit hit;
		_mousePressed = Input.GetMouseButton(0);
		if (_mousePressed) {
			if (whiteboard.Raycast (targetCamera.ScreenPointToRay (Input.mousePosition), 
			                       out hit, float.MaxValue)) {
				var mousePosPixel = Vector2.Scale (hit.textureCoord, _texSize);
				var pixelRadiu = radius * _width;

				Parallel.For (0, _pixelCount, (i) => {
					var y = i / _width;
					var x = i - y * _width;
					var currPosPixel = new Vector2 (x, y);
					var path = currPosPixel - mousePosPixel;
					var w = Mathf.Lerp (1f, 0f, path.magnitude / pixelRadiu);

					_outputs [i] = _inputs [i] * w;
				});
				output.SetPixels (_outputs);
				output.Apply ();
			}
		} else {
			Parallel.For (0, _pixelCount, (i) => {
				_outputs [i] = Color.clear;
            });
            output.SetPixels (_outputs);
            output.Apply ();      
        }
    }

	IEnumerator Pinning(float interval, int count) {
		while (true) {
			yield return new WaitForSeconds(interval);
			if (!enabled || !_mousePressed)
				continue;

			_mcmc.Reset();
			foreach (var uv in _mcmc.Sequence(100, count)) {
				Vector3 posLocal;
				Vector3 normalLocal;
				if (uvWorld.World(uv, out posLocal, out normalLocal)) {
					posLocal += (Vector3)(perturbation * Random.insideUnitCircle);
					var pos = uvWorld.transform.TransformPoint(posLocal) + shuriken.startSize * Vector3.up;
					shuriken.Emit(pos, Vector3.zero, shuriken.startSize, shuriken.startLifetime, shuriken.startColor);
				}
			}
		}
	}
}
