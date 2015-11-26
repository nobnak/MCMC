using UnityEngine;
using System.Collections;
using MCMCSystem;

[RequireComponent(typeof(Collider))]
public class TranslationTest : MonoBehaviour {
	public UVWorld uvWorld;

	Transform _output;
	Collider _input;

	void Start() {
		_input = GetComponent<Collider>();
		_output = uvWorld.transform;
	}
	void Update() {
		if (Input.GetMouseButton(0)) {
			var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			RaycastHit hit;
			Vector3 pos;
			Vector3 normal;
			if (_input.Raycast(ray, out hit, float.MaxValue) 
			    && uvWorld.World(hit.textureCoord, out pos, out normal)) {
				var start = _output.TransformPoint(pos);
				var end = start + 3f * _output.TransformDirection(normal);
				Debug.DrawLine(start, end);
			}
		}
	}
}
